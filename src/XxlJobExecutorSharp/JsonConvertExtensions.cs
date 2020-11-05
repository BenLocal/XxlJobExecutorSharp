using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace XxlJobExecutorSharp
{
    internal static class JsonConvertExtensions
    {
        public static T DeserializeObject<T>(this string str, JsonSerializerSettings settings) where T : class
        {
            if (string.IsNullOrEmpty(str))
            {
                return null;
            }

            try
            {
                return JsonConvert.DeserializeObject<T>(str, settings);
            }
            catch(Exception)
            {
                return null;
            }
        }

        public static T DeserializeObject<T>(this string str) where T : class
            => DeserializeObject<T>(str, null);

        public static T DeserializeSnakeCaseObject<T>(this string str) where T : class
            => DeserializeObject<T>(str, new JsonSerializerSettings()
            {
                ContractResolver = new DefaultContractResolver()
                {
                    NamingStrategy = new SnakeCaseNamingStrategy()
                }
            });

        public static string SerializeObject(this object obj, JsonSerializerSettings settings)
        {
            if (obj == null)
            {
                return null;
            }

            try
            {
                return JsonConvert.SerializeObject(obj, settings);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static string SerializeObject(this object obj)
            => SerializeObject(obj, null);

        public static string SerializeSnakeCaseObject(this object obj)
           => SerializeObject(obj, new JsonSerializerSettings()
           {
               ContractResolver = new DefaultContractResolver
               {
                   NamingStrategy = new SnakeCaseNamingStrategy()
               }
           });
    }
}
