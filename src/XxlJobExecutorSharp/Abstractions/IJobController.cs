using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace XxlJobExecutorSharp.Abstractions
{
    public interface IJobController
    {
        Task<IActionResponse> ActionAsync(HttpContext context);
    }
}
