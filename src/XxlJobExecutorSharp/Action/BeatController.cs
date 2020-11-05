using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using XxlJobExecutorSharp.Abstractions;
using XxlJobExecutorSharp.Entity;

namespace XxlJobExecutorSharp.Action
{
    public class BeatController : IJobController
    {
        public async Task<IActionResponse> ActionAsync(HttpContext context)
        {
            return await context.JsonAsync(ReturnT.Success());
        }
    }
}
