using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.FrontEndService.Converter
{
    using Newtonsoft.Json;

    using RestServer.Core.Extensions;

    public class JsonSecureStringRequestModelConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value != null)
            {
                writer.WriteValue(value.ToString());
            }
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (!objectType.IsAssignableFrom(typeof(System.Security.SecureString)))
            {
                throw new ArgumentException("Invalid object type for converter");
            }

            if (null != reader.Value)
            {
                return reader.Value.ToString().ToSecureString();
            }

            return null;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType.IsAssignableFrom(typeof(System.Security.SecureString));
        }
    }
}
