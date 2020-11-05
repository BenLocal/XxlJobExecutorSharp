using System.Threading.Tasks;

namespace XxlJobExecutorSharp.Abstractions
{
    public interface IXxlJobServicesProvider<TInterface>
    {
        TInterface GetInstance(string key);

        Task<TInterface> GetInstanceAsync(string key);
    }
}
