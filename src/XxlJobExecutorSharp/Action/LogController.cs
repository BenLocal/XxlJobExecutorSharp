using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using XxlJobExecutorSharp.Abstractions;
using XxlJobExecutorSharp.Entity;
using XxlJobExecutorSharp.Logger;

namespace XxlJobExecutorSharp.Action
{
    public class LogController : IJobController
    {
        private readonly IJobLoggerStore _store;

        private const int Default_Limit = 20;

        public LogController(IJobLoggerStore store)
        {
            _store = store;
        }

        public async Task<IActionResponse> ActionAsync(HttpContext context)
        {
            var info = await context.Request.FromBody<JobLogRequest>();
            if (info == null || info.JobLogId <= 0)
            {
                return await context.JsonAsync(ReturnT.Failed("参数错误"));
            }

            (var logs, var totalCount) = await _store.FilterJobList(info.FromLineNum - 1, Default_Limit, info.JobLogId);

            StringBuilder sb = new StringBuilder();
            foreach (var log in logs)
            {
                sb.AppendLine($"[{log.InData.ToString("yyyy-MM-dd HH:mm:ss")}],{log.Message}");
            }

            // 从1开始
            var endLineNum = info.FromLineNum + logs.Count;

            var res = ReturnT.Success();
            res.Content = new
            {
                // 本次请求，日志开始行数
                fromLineNum = info.FromLineNum,
                // 本次请求，日志结束行号
                toLineNum = endLineNum,
                // 本次请求日志内容
                logContent = sb.ToString(),
                // 日志是否全部加载完
                isEnd = totalCount <= endLineNum      
            };

            return await context.JsonAsync(res);
        }
    }
}
