using System.Collections.Generic;
using System.Threading.Tasks;
using XxlJobExecutorSharp.Entity;

namespace XxlJobExecutorSharp.Abstractions
{
    public interface IJobLoggerStore
    {
        Task Save(JobLoggerInfo entry);

        Task<(List<JobLoggerInfo>, int totalCount)> FilterJobList(int index, int limit, int jobLogId);
    }
}
