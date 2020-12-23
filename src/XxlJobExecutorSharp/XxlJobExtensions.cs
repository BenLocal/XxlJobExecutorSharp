using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using RestSharp;
using XxlJobExecutorSharp.Abstractions;
using XxlJobExecutorSharp.Entity;
using XxlJobExecutorSharp.Logger;
using XxlJobExecutorSharp.Processor;
using XxlJobExecutorSharp.Services;

namespace XxlJobExecutorSharp
{
    public static class XxlJobExtensions
    {
        public static IServiceCollection AddXxlJob(this IServiceCollection services, Action<XxlJobOptions> action)
        {
            var options = new XxlJobOptions();
            action?.Invoke(options);
            services.AddSingleton(options);

            // xxl job service
            services.Add(new ServiceDescriptor(typeof(IXxlJobExecutor), typeof(XxlJobExecutor), ServiceLifetime.Singleton));
            services.AddSingleton<IProcessor, ExecutorRegistryProcessor>();
            services.AddTransient<IRestClient>(p =>
            {
                var conf = p.GetRequiredService<XxlJobOptions>();
                return new RestClient(conf.AdminUrl);
            });
            services.AddSingleton<IJobDispatcher, JobDispatcher>();
            services.AddSingleton<IJobSender, XxlJobSender>();
            services.AddTransient<XxlJobQueue>();
            services.AddSingleton<IJobLoggerStore, LiteDBLoggerStore>();

            // route
            var routeTable = new XxlJobRouteTable();
            services.AddSingleton(routeTable);
            foreach (var route in routeTable)
            {
                services.AddScoped(route.TypeController);
            }

            // host service
            services.AddHostedService<XxlJobBackgroundService>();

            // job handler
            services.Add(new ServiceDescriptor(typeof(IXxlJobServicesProvider<>), typeof(XxlJobServicesProvider<>), ServiceLifetime.Singleton));
            services.ScanDynamicJobExecutorHandler(options.HandlerServiceLifetime);
            return services;
        }

        public static IServiceCollection AddXxlJob(this IServiceCollection services, IConfiguration configuration, Action<XxlJobOptions> action)
            => AddXxlJob(services, options =>
            {
                options.CallBackRetryCount = configuration.GetEnvironmentOrConfigStr("XxlJob:CallBackRetryCount").CreventInt();
                options.ExecutorAppName = configuration.GetEnvironmentOrConfigStr("XxlJob:ExecutorAppName");
                options.ExecutorUrl = configuration.GetEnvironmentOrConfigStr("XxlJob:ExecutorUrl");
                options.HeartbeatIntervalSecond = configuration.GetEnvironmentOrConfigStr("XxlJob:HeartbeatIntervalSecond").CreventInt();
                options.Token = configuration.GetEnvironmentOrConfigStr("XxlJob:Token");
                options.AdminUrl = configuration.GetEnvironmentOrConfigStr("XxlJob:AdminUrl");

                action?.Invoke(options);
            });

        public static IServiceCollection AddXxlJob(this IServiceCollection services, IConfiguration configuration)
            => AddXxlJob(services, configuration, null);

        public static IApplicationBuilder UseXxlJob(this IApplicationBuilder app)
        {
            app.UseMiddleware<XxlJobMiddleware>();

            var loggerFactory = app.ApplicationServices.GetRequiredService<ILoggerFactory>();
            loggerFactory.AddProvider(new CustomJobLoggerProvider(
                          new JobLoggerConfiguration
                          {
                          }, app.ApplicationServices.GetRequiredService<IJobLoggerStore>()));
            return app;
        }

        private static void AddScopedDynamicJobHandler(this IServiceCollection services, Type interfaceType, IEnumerable<Type> types, ServiceLifetime lifetime)
        {
            var typeFunc = typeof(Func<,>).MakeGenericType(typeof(string), interfaceType);
            ScopedDynamicDelegate f = serviceProvider => tenant => {
                var type = types.FilterByInterface(interfaceType)
                               .FirstOrDefault(x => x.GetCustomAttribute<XxlJobHandlerAttribute>()?.Name == tenant);

                if (null == type)
                    throw new KeyNotFoundException("No instance found for the given tenant.");

                return serviceProvider.GetService(type);
            };

            services.Add(new ServiceDescriptor(typeFunc, serviceProvider => f(serviceProvider), lifetime));
        }

        private static IEnumerable<Type> FilterByInterface(this IEnumerable<Type> types, Type interfaceType)
        {
            return types.Where(i => interfaceType.IsAssignableFrom(i) && i != interfaceType);
        }

        internal delegate Func<string, dynamic> ScopedDynamicDelegate(IServiceProvider serviceProvider);

        private static void ScanDynamicJobExecutorHandler(this IServiceCollection services, ServiceLifetime lifetime)
        {
            // get all types
            var typesToRegisterAndAttr = XxlJobUtilExtensions.CollectByAttributeTypes<XxlJobHandlerAttribute>()
                    .Where(x => x.Item1.IsClass && !x.Item1.IsAbstract).ToList();

            var types = typesToRegisterAndAttr.Select(x =>
            {
                if (x.Item2.InterfaceType == null)
                {
                    // get default
                    var interfaces = x.Item1.GetInterfaces();

                    return (x.Item1, interfaces.FirstOrDefault());
                }

                return (x.Item1, x.Item2.InterfaceType);
            });

            var typesToRegisterList = types.GroupBy(x => x.Item2, y => y).ToDictionary(x => x.Key, y => y.ToList());

            typesToRegisterAndAttr.ForEach(x => services.Add(new ServiceDescriptor(x.Item1, x.Item1, lifetime)));

            if (typesToRegisterList != null)
            {
                foreach (var typesToRegister in typesToRegisterList)
                {
                    if (typesToRegister.Value != null)
                    {
                        services.AddScopedDynamicJobHandler(typesToRegister.Key, typesToRegister.Value.Select(x => x.Item1), lifetime);
                    }
                }
            }
        }
    }
}
