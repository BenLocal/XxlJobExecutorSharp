using System;
using System.Threading.Tasks;
using XxlJobExecutorSharp.Abstractions;

namespace XxlJobExecutorSharp.Services
{
    public class XxlJobServicesProvider<TInterface> : IXxlJobServicesProvider<TInterface>
    {
        private readonly IServiceProvider _provider;

        public XxlJobServicesProvider(IServiceProvider provider)
        {
            _provider = provider;
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
            return (Func<string, object>)_provider.GetService(typeof(Func<string, TInterface>));
        }
    }
}
