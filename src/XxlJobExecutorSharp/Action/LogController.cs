using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using XxlJobExecutorSharp.Abstractions;
using XxlJobExecutorSharp.Entity;

namespace XxlJobExecutorSharp.Action
{
    public class LogController : IJobController
    {
        public async Task<IActionResponse> ActionAsync(HttpContext context)
        {
            // TODO
            var res = ReturnT.Success();
            res.Content = new
            {
                fromLineNum = 0,        // 本次请求，日志开始行数
                toLineNum = 100,        // 本次请求，日志结束行号
                logContent = "暂无日志",     // 本次请求日志内容
                isEnd = true            // 日志是否全部加载完
            };

            return await context.JsonAsync(res);
        }
    }
}
