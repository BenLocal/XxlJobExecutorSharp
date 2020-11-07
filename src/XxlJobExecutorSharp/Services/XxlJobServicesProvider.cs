﻿using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using XxlJobExecutorSharp.Abstractions;

namespace XxlJobExecutorSharp.Services
{
    public class XxlJobServicesProvider<TInterface> : IXxlJobServicesProvider<TInterface>
    {
        private readonly IServiceProvider _provider;

        private readonly IHttpContextAccessor _contextAccessor;

        public XxlJobServicesProvider(IServiceProvider provider,
            IHttpContextAccessor contextAccessor)
        {
            _provider = provider;
            _contextAccessor = contextAccessor;
        }


        public TInterface GetInstance(string key)
        {
            var func = this.GetService();
            return (TInterface)func(key);
        }

        public Task<TInterface> GetInstanceAsync(string key)
        {
            var func = this.GetService();
            return Task.FromResult((TInterface)func(key));
        }

        private Func<string, object> GetService()
        {
            var provider = GetServiceProvider();
            return (Func<string, object>)provider.GetRequiredService(typeof(Func<string, TInterface>));
        }

        private IServiceProvider GetServiceProvider()
        {
            if (_contextAccessor?.HttpContext?.RequestServices == null)
            {
                var scope = _provider.CreateScope();
                return scope.ServiceProvider;
        }

            return _contextAccessor?.HttpContext?.RequestServices;
        }
    }
}
