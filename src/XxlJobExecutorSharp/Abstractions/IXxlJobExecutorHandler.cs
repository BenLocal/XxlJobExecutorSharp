using System.Threading.Tasks;
using XxlJobExecutorSharp.Entity;

namespace XxlJobExecutorSharp.Abstractions
{
    public interface IXxlJobExecutorHandler
    {
        Task<ReturnT> Execute(JobExecuteContext context);
    }
}
