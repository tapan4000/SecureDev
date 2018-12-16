namespace RestServer.IoC
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Xml;

    using RestServer.IoC.Interfaces;
    using RestServer.Logging;

    using Unity;
    using Unity.Interception.ContainerIntegration;
    using Unity.Interception.InterceptionBehaviors;
    using Unity.Interception.Interceptors.InstanceInterceptors.InterfaceInterception;
    using Unity.Lifetime;
    using Unity.Registration;
    using Unity.RegistrationByConvention;

    public class IoCUnityHelper
    {
        private const string ConfigurationFileNameFormat = @"\Package\Package.{0}.Config";

        private const string ServerNamespace = "RestServer";

        private static string traceId;

        private static Lazy<IUnityContainer> Container = new Lazy<IUnityContainer>(
            () =>
                {
                    var container = GetRegistrations();
                    RegisterByConvention(container);
                    traceId = Guid.NewGuid().ToString();
                    return container;
                });

        public static IUnityContainer GetConfiguredContainer()
        {
            return Container.Value;
        }

        public static string GetRegistrationMapping()
        {
            StringBuilder sb = new StringBuilder();
            var container = Container.Value;
            foreach(var registration in container.Registrations)
            {
                sb.AppendLine($"'{registration.Name}' >> {registration.RegisteredType}:{registration.MappedToType}");
            }

            return sb.ToString();
        }

        public static IDependencyContainer OverrideDependencies(IUnityContainer container, IDictionary<Type, Type> explicitTypes = null, IDictionary<Type, object> explicitInstances = null)
        {
            if (null == container)
            {
                throw new ArgumentNullException(nameof(container));
            }

            // Override with explicit types for registration
            if (null != explicitTypes && explicitTypes.Any())
            {
                foreach (var registration in explicitTypes)
                {
                    container.RegisterType(registration.Key, registration.Value);
                }
            }

            // Override with explicit instances for registration
            if (null != explicitInstances && explicitInstances.Any())
            {
                foreach (var registration in explicitInstances)
                {
                    container.RegisterInstance(registration.Key, registration.Value);
                }
            }

            return new UnityDependencyContainer(container);
        }

        private static IUnityContainer RegisterByConvention(IUnityContainer container, IEnumerable<Assembly> assembliesToLoad = null, string region = null)
        {
            if (container == null)
            {
                container = new UnityContainer();
            }

            // Add calling assembly for exe applications, this is required for Service Fabric projects
            var assembliesToBeRegistered = GetAssembliesInBasePath().ToList();
            var entryAssembly = Assembly.GetEntryAssembly();
            if (entryAssembly != null && !assembliesToBeRegistered.Contains(entryAssembly))
            {
                assembliesToBeRegistered.Add(entryAssembly);
            }

            RegisterDependencies(container, assembliesToBeRegistered);
            return container;
        }

        private static IUnityContainer GetRegistrations()
        {
            var container = new UnityContainer();

            // Registration of IDependency container is needed so that we can define the IDependendency container as the dependencyresolver in the HttpConfiguration
            // DependencyResolver property. Thereby, the REST controller API can contain the IDependencyContainer as a constructor property that can be resolved.
            var basePathAssemblies = GetAssembliesInBasePath().ToList();
            var entryAssembly = Assembly.GetEntryAssembly();
            if (!basePathAssemblies.Contains(entryAssembly))
            {
                basePathAssemblies.Add(entryAssembly);
            }

            return container;
        }

        private static void RegisterDependencies(IUnityContainer container, IEnumerable<Assembly> assembliesToLoad)
        {
            var interfaceTypeMapping = new Dictionary<Type, HashSet<Type>>();
            var classes = GetClassesFromAssembliesInBasePath(assembliesToLoad);
            foreach (var type in classes)
            {
                var interfacesToBeRegsitered = GetInterfacesToBeRegistered(type);
                AddToInterfaceTypeMap(type, interfacesToBeRegsitered, interfaceTypeMapping);
            }

            RegisterDependencies(container, interfaceTypeMapping);
        }

        private static void AddToInterfaceTypeMap(Type type, IEnumerable<Type> interfaces, Dictionary<Type, HashSet<Type>> interfaceTypeMapping)
        {
            foreach (var interfaceOnType in interfaces)
            {
                if (!interfaceTypeMapping.ContainsKey(interfaceOnType))
                {
                    interfaceTypeMapping[interfaceOnType] = new HashSet<Type>();
                }

                interfaceTypeMapping[interfaceOnType].Add(type);
            }
        }

        private static IEnumerable<Type> GetClassesFromAssembliesInBasePath(IEnumerable<Assembly> assemblies = null)
        {
            // filtering the assemblies from which the classes should not be registered.
            var allClasses = assemblies != null ? AllClasses.FromAssemblies(assemblies) : AllClasses.FromAssembliesInBasePath(skipOnError: true);
            return allClasses
               .Where(n => n.Namespace != null
                   && n.Namespace.StartsWith(ServerNamespace, StringComparison.OrdinalIgnoreCase)
                   && !n.IsAbstract);
                   //&& !IoCHelper.ShouldIgnoreType(n));
        }

        private static IEnumerable<Type> GetInterfacesToBeRegistered(Type type)
        {
            // NOTE: For Generic interfaces the convention is that the class name should be the interface name without an I
            // e.g. IDataStoreStrategy<TEntity> => DataStoreStrategy<TEntity>
            var allInterfacesOnType =
                type.GetInterfaces()
                    .Where(
                        i =>
                        (!i.IsGenericType || i.Name.StartsWith(string.Format(CultureInfo.InvariantCulture, "I{0}", type.Name), StringComparison.OrdinalIgnoreCase)
                         || (type.BaseType != null && type.BaseType.IsAbstract && i.Namespace != null && i.Namespace.StartsWith(ServerNamespace, StringComparison.OrdinalIgnoreCase))))
                    .Select(i => i.IsGenericType ? i.GetGenericTypeDefinition() : i)
                    .ToList();

            var directTypes = allInterfacesOnType.Except(allInterfacesOnType.SelectMany(i => i.GetInterfaces())).ToList();

            // TODO: Registering classes against a base class to have it resolved as there would be tight class coupling
            // Will need to refactor this later.
            if (type.BaseType != null && type.BaseType.IsAbstract)
            {
                directTypes.Add(type.BaseType);
            }

            return directTypes;
        }

        private static void RegisterDependencies(IUnityContainer container, Dictionary<Type, HashSet<Type>> interfaceTypeMapping)
        {
            foreach (var typeMapping in interfaceTypeMapping)
            {
                if (typeMapping.Value.Count == 1)
                {
                    var type = typeMapping.Value.First();
                    var lifetime = GetLifetimeManager(type);
                    var interceptionBehaviors = GetInterceptionBehaviors(type, interfaceTypeMapping);
                    container.RegisterType(typeMapping.Key, type, lifetime, interceptionBehaviors);
                }
                else
                {
                    foreach (var type in typeMapping.Value)
                    {
                        var lifetime = GetLifetimeManager(type);
                        var interceptionBehaviors = GetInterceptionBehaviors(type, interfaceTypeMapping);
                        container.RegisterType(typeMapping.Key, type, GetNameForRegsitration(type), lifetime, interceptionBehaviors);
                    }
                }
            }
        }

        private static InjectionMember[] GetInterceptionBehaviors(Type type, Dictionary<Type, HashSet<Type>> interfaceTypeMapping)
        {
            var injectionMembers = new List<InjectionMember>();

            if (!HasInterceptionBehaviors(type))
            {
                return injectionMembers.ToArray();
            }

            var iocAttribute = (IoCRegistrationAttribute)Attribute.GetCustomAttribute(type, typeof(IoCRegistrationAttribute));
            injectionMembers.Add(new Interceptor<InterfaceInterceptor>());

            foreach (var interception in iocAttribute.Interceptions)
            {
                var interceptionConvention = string.Format(CultureInfo.InvariantCulture, ConventionConstants.InterceptionConvention, interception);
                var interceptionType = GetInterceptionBehavior(interceptionConvention, interfaceTypeMapping);
                injectionMembers.Add(interceptionType);
            }

            return injectionMembers.ToArray();
        }

        private static InterceptionBehavior GetInterceptionBehavior(string interceptionConvention, Dictionary<Type, HashSet<Type>> interfaceTypeMapping)
        {
            if (!interfaceTypeMapping.ContainsKey(typeof(IInterceptionBehavior)))
            {
                throw new Exception("No Interception Behaviors found");
            }

            var allbehaviors = interfaceTypeMapping[typeof(IInterceptionBehavior)];
            if (allbehaviors == null || !allbehaviors.Any())
            {
                throw new Exception("No Interception Behaviors found");
            }

            var behaviour = allbehaviors.FirstOrDefault(b => b.Name == interceptionConvention);
            if (behaviour == null)
            {
                throw new Exception(string.Format(CultureInfo.InvariantCulture, "{0} does not exists", interceptionConvention));
            }

            return new InterceptionBehavior(behaviour);
        }

        private static bool HasInterceptionBehaviors(Type type)
        {
            var iocAttribute = (IoCRegistrationAttribute)Attribute.GetCustomAttribute(type, typeof(IoCRegistrationAttribute));
            return iocAttribute != null
                && iocAttribute.Interceptions != null
                && iocAttribute.Interceptions.Length != 0;
        }

        private static string GetNameForRegsitration(Type type)
        {
            var name = type.Name;
            var iocAttribute = (IoCRegistrationAttribute)Attribute.GetCustomAttribute(type, typeof(IoCRegistrationAttribute));
            if (iocAttribute != null)
            {
                name = iocAttribute.ShouldAppendClassName ? iocAttribute.NamePrefix + name : iocAttribute.NamePrefix;
            }

            return name.ToLowerInvariant();
        }

        private static LifetimeManager GetLifetimeManager(Type type)
        {
            var iocAttribute = (IoCRegistrationAttribute)Attribute.GetCustomAttribute(type, typeof(IoCRegistrationAttribute));
            if (iocAttribute == null || string.IsNullOrEmpty(iocAttribute.Lifetime) || iocAttribute.Lifetime == IoCLifetime.Transient)
            {
                return null;
            }

            if (iocAttribute.Lifetime == IoCLifetime.Hierarchical)
            {
                return new HierarchicalLifetimeManager();
            }

            if (iocAttribute.Lifetime == IoCLifetime.ContainerControlled)
            {
                return new ContainerControlledLifetimeManager();
            }

            throw new ArgumentException("Invalid value for Lifetime", iocAttribute.Lifetime);
        }

        /// <summary>
        /// Gets the excluded assemblies.
        /// </summary>
        /// <param name="regionName">Name of the region.</param>
        /// <returns>
        /// List of excluded assemblies.
        /// </returns>
        /// <exception cref="System.FormatException">Could not load the configuration</exception>
        private static List<string> GetExcludedAssemblies(string regionName)
        {
            if (!string.IsNullOrEmpty(regionName))
            {
                var configuration = new XmlDocument();
                configuration.XmlResolver = null;
                var basePath = AppDomain.CurrentDomain.RelativeSearchPath ?? AppDomain.CurrentDomain.BaseDirectory;

                var filePath = basePath + string.Format(CultureInfo.InvariantCulture, ConfigurationFileNameFormat, regionName);
                var fileExists = File.Exists(filePath);
                LoggerEventSource.Current.Verbose(traceId, $"{filePath} exists: {fileExists}", 0);

                if (fileExists)
                {
                    using (var reader = new XmlTextReader(filePath))
                    {
                        reader.DtdProcessing = DtdProcessing.Ignore;
                        configuration.Load(reader);
                    }

                    var documentExists = configuration.DocumentElement != null;
                    LoggerEventSource.Current.Verbose(traceId, $"configuration.DocumentElement exists: {documentExists}", 0);
                    if (documentExists)
                    {
                        var exclusionsList = new List<string>();
                        var exclusions = configuration.DocumentElement.SelectNodes("Service/Exclusions/Add");
                        if (exclusions != null && exclusions.Count > 0)
                        {
                            exclusionsList =
                                exclusions.Cast<XmlNode>()
                                    .Select(exclusion => GetAssemblyNameFromConfigText(exclusion.InnerText))
                                    .ToList();
                        }

                        // Exclude Service Fabric related DLLs
                        var serviceFabricExclusions = configuration.DocumentElement.SelectNodes("ServiceFabric/Exclusions/Add");
                        if (serviceFabricExclusions != null && serviceFabricExclusions.Count > 0)
                        {
                            exclusionsList.AddRange(
                                serviceFabricExclusions.Cast<XmlNode>()
                                    .Select(exclusion => GetAssemblyNameFromConfigText(exclusion.InnerText))
                                    .ToList());
                        }

                        exclusionsList.ForEach(x => LoggerEventSource.Current.Verbose(traceId, $"Excluded: {x}", 0));
                        return exclusionsList;
                    }
                }
            }

            return new List<string>();
        }

        private static IEnumerable<Assembly> GetAssembliesInBasePath()
        {
            var basePath = AppDomain.CurrentDomain.RelativeSearchPath ?? AppDomain.CurrentDomain.BaseDirectory;
            var basePathAssemblies = new List<Assembly>();

            foreach (var dll in Directory.GetFiles(basePath, "*.dll"))
            {
                try
                {
                    if (dll.Contains("RestServer."))
                    {
                        basePathAssemblies.Add(Assembly.LoadFile(dll));
                    }
                }
                catch (Exception ex)
                {
                    // Ignore the error
                    LoggerEventSource.Current.Critical(traceId, ex, 0);
                }
            }

            return basePathAssemblies;
        }

        private static string GetAssemblyNameFromConfigText(string innerText)
        {
            var split = innerText.Split(new[] { @"\" }, StringSplitOptions.RemoveEmptyEntries);
            return split[split.Length - 1];
        }

        public static void LogRegistrationMappings(IUnityContainer unityContainer)
        {
            var registrations = new StringBuilder(string.Empty);
            registrations.Append("RegisteredType : MappedType : Named Registration");
            int count = 0;
            foreach (var containerRegistration in unityContainer.Registrations)
            {
                count++;
                registrations.AppendFormat(
                    CultureInfo.InvariantCulture,
                    "{0} : {1} : {2} {3}",
                    containerRegistration.RegisteredType.Name,
                    containerRegistration.MappedToType.Name,
                    containerRegistration.Name,
                    Environment.NewLine);
                if (count % 100 == 0)
                {
                    LoggerEventSource.Current.Verbose(traceId, registrations.ToString(), 0);
                    registrations.Clear();
                }
            }

            LoggerEventSource.Current.Verbose(traceId, registrations.ToString(), 0);
        }
    }
}
