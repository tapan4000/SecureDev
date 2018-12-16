using RestServer.Business.Core.Interfaces.Activities;
using RestServer.IoC.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.Business.Core.Activities
{
    public class ActivityFactory : IActivityFactory
    {
        private readonly IDependencyContainer dependencyContainer;
        public ActivityFactory(IDependencyContainer dependencyContainer)
        {
            this.dependencyContainer = dependencyContainer;
        }

        public IActivity<TActivityRequest, TActivityResponse> CreateActivity<TActivity, TActivityRequest, TActivityResponse>() where TActivity : IActivity<TActivityRequest, TActivityResponse>
        {
            var activityName = typeof(TActivity).Name;
            var activity = this.dependencyContainer.Resolve<TActivity>(activityName);

            if(null == activity)
            {
                throw new ArgumentException("Failed to create activity.");
            }

            return activity;
        }

        public IActivity<TActivityRequest, TActivityResponse> CreateGenericActivity<TActivityRequest, TActivityResponse>()
        {
            throw new NotImplementedException();
        }
    }
}
