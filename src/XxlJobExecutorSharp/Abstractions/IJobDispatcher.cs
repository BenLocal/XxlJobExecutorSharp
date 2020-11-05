using System.Threading.Tasks;
using XxlJobExecutorSharp.Entity;

namespace XxlJobExecutorSharp.Abstractions
{
    public interface IJobDispatcher
    {
        Task Enqueue(JobMessage job);

        Task<bool> Exist(int jobId);

        Task<bool> Kill(int jobId, string killedReason);
    }
}
