using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.Configuration
{
    public static class ConfigurationExtensions
    {
        public static Tuple<object, Exception> ParseType(this object objValue, Type destType)
        {
            object value = null;
            try
            {
                if (destType == typeof(string))
                {
                    value = objValue.ToString();
                }
                else if (destType == typeof(TimeSpan))
                {
                    value = TimeSpan.Parse(objValue.ToString(), CultureInfo.InvariantCulture);
                }
                else if (destType == typeof(Guid))
                {
                    value = Guid.Parse(objValue.ToString());
                }
                else if (destType == typeof(DateTimeOffset))
                {
                    value = DateTimeOffset.Parse(objValue.ToString(), null, DateTimeStyles.AssumeUniversal);
                }
                else if (destType.IsEnum)
                {
                    value = Enum.Parse(destType, objValue.ToString());
                }
                else if (destType == typeof(byte[]))
                {
                    value = Convert.FromBase64String(objValue.ToString());
                }
                else if (destType == typeof(bool))
                {
                    value = Convert.ToBoolean(objValue.ToString());
                }
                else
                {
                    // JSonConvert handles all type conversions except the above
                    value = JsonConvert.DeserializeObject(objValue.ToString(), destType);
                }
            }
            catch (Exception ex)
            {
                return new Tuple<object, Exception>(null, ex);
            }

            return new Tuple<object, Exception>(value, null);
        }
    }
}
