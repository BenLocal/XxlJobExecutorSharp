using System.Collections.Generic;
using System.Threading.Tasks;
using XxlJobExecutorSharp.Entity;

namespace XxlJobExecutorSharp.Abstractions
{
    public interface IJobSender
    {
        Task<ReturnT> Execute(JobMessage job);

        Task CallBack(List<JobMessage> jobs);
    }
}
