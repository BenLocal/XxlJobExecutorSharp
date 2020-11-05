using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using XxlJobExecutorSharp.Abstractions;
using XxlJobExecutorSharp.Entity;

namespace XxlJobExecutorSharp.Action
{
    public class KillController : IJobController
    {
        private readonly IJobDispatcher _dispatcher;

        public KillController(IJobDispatcher dispatcher)
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

            if (await _dispatcher.Kill(info.JobId, "终止任务[调度中心主动停止任务]"))
            {
                return await context.JsonAsync(ReturnT.Success());
            }

            return await context.JsonAsync(ReturnT.Failed("终止任务失败"));
        }
    }
}
