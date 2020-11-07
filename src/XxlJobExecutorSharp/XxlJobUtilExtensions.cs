using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using XxlJobExecutorSharp.Abstractions;
using XxlJobExecutorSharp.Entity;

namespace XxlJobExecutorSharp
{
    internal static class XxlJobUtilExtensions
    {
        private static readonly TaskFactory _myTaskFactory = new TaskFactory(CancellationToken.None, TaskCreationOptions.None, TaskContinuationOptions.None, TaskScheduler.Default);

        internal static long GetTimeStamp(this DateTime dateTime)
        {
            DateTime d1 = new DateTime(1970, 1, 1);
            DateTime d2 = dateTime.ToUniversalTime();
            TimeSpan ts = new TimeSpan(d2.Ticks - d1.Ticks);
            return (long)ts.TotalMilliseconds;
        }

        internal static string EnsureTrailingSlash(this string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;
            return Regex.Replace(input, "/+$", string.Empty) + "/";
        }

        internal static string EnsureStartSlash(this string input)
        {
            if (input == null) return string.Empty;
            if (!input.StartsWith("/"))
            {
                return "/" + input;
            }
            return input;
        }

        internal static async Task<T> FromBody<T>(this HttpRequest request) where T : class
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

        internal static List<(Type, T)> CollectByAttributeTypes<T>() where T : Attribute
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

        internal static async Task<IActionResponse> JsonAsync(this HttpContext context, object value)
        {
            return await Task.FromResult(new JsonActionResponse(context, value));
        }

        internal static string GetEnvironmentOrConfigStr(this IConfiguration configuration, string key)
        {
            // key1:ke2 => key1_key2
            var evnKey = key;
            if (key.Contains(":"))
            {
                evnKey = string.Join("_", key.Split(':'));
            }

            var value = Environment.GetEnvironmentVariable($"ASPNETCORE_{evnKey.ToUpper()}", EnvironmentVariableTarget.Process);

            if (string.IsNullOrEmpty(value))
            {
                value = configuration?.GetSection(key)?.Value;
            }

            return value;
        }

        internal static int CreventInt(this string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return 0;
            }

            try
            {
                return Convert.ToInt32(value);
            }
            catch
            {
                return 0;
            }
        }

        internal static void RunSync(Func<Task> func)
        {
            CultureInfo cultureUi = CultureInfo.CurrentUICulture;
            CultureInfo culture = CultureInfo.CurrentCulture;
            _myTaskFactory.StartNew(delegate
            {
                Thread.CurrentThread.CurrentCulture = culture;
                Thread.CurrentThread.CurrentUICulture = cultureUi;
                return func();
            }).Unwrap().GetAwaiter().GetResult();
        }
    }
}
