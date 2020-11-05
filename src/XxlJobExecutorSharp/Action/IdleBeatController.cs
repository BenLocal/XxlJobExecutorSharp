using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using XxlJobExecutorSharp.Abstractions;
using XxlJobExecutorSharp.Entity;

namespace XxlJobExecutorSharp.Action
{
    public class IdleBeatController : IJobController
    {
        private readonly IJobDispatcher _dispatcher;

        public IdleBeatController(IJobDispatcher dispatcher)
        {
            _dispatcher = dispatcher;
        }

        public async Task<IActionResponse> ActionAsync(HttpContext context)
        {
            var info = await context.Request.FromBody<JobBaseRequest>();
            if (info == null || info.JobId <= 0)
            {
                return await context.JsonAsync(ReturnT.Failed("参数错误"));
            }

            var isRunning = await _dispatcher.Exist(info.JobId);
            if (isRunning)
            {
                return await context.JsonAsync(ReturnT.Failed("任务执行中"));
            }

            return await context.JsonAsync(ReturnT.Success());
        }
    }
}
