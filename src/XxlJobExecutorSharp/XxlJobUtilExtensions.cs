using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using XxlJobExecutorSharp.Abstractions;
using XxlJobExecutorSharp.Entity;

namespace XxlJobExecutorSharp
{
    public static class XxlJobUtilExtensions
    {
        public static long GetTimeStamp(this DateTime dateTime)
        {
            DateTime d1 = new DateTime(1970, 1, 1);
            DateTime d2 = dateTime.ToUniversalTime();
            TimeSpan ts = new TimeSpan(d2.Ticks - d1.Ticks);
            return (long)ts.TotalMilliseconds;
        }

        public static string EnsureTrailingSlash(this string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;
            return Regex.Replace(input, "/+$", string.Empty) + "/";
        }

        public static string EnsureStartSlash(this string input)
        {
            if (input == null) return string.Empty;
            if (!input.StartsWith("/"))
            {
                return "/" + input;
            }
            return input;
        }

        public static async Task<T> FromBody<T>(this HttpRequest request) where T : class
        {
            var stream = request.Body;
            var reader = new StreamReader(stream);
            var contentFromBody = await reader.ReadToEndAsync();
            if (stream.CanSeek)
            {
                stream.Seek(0, SeekOrigin.Begin);
            }
            return contentFromBody.DeserializeObject<T>();
        }

        public static List<(Type, T)> CollectByAttributeTypes<T>() where T : Attribute
        {
            List<(Type, T)> batchBaseTypes = new List<(Type, T)>();

            foreach (var asm in AppDomain.CurrentDomain.GetAssemblies())
            {
                if (asm.FullName.StartsWith("System") || asm.FullName.StartsWith("Microsoft.Extensions")) continue;
                Type[] types;
                try
                {
                    types = asm.GetTypes();
                }
                catch (ReflectionTypeLoadException ex)
                {
                    types = ex.Types;
                }

                foreach (var item in types)
                {
                    var attr = item.GetCustomAttribute<T>();
                    if (attr != null)
                    {
                        batchBaseTypes.Add((item, attr));
                    }
                }
            }

            return batchBaseTypes;
        }

        public static async Task<IActionResponse> JsonAsync(this HttpContext context, object value)
        {
            return await Task.FromResult(new JsonActionResponse(context, value));
        }
    }
}
