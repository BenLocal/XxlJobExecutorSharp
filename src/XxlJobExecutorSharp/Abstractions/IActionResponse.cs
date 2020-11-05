using Microsoft.AspNetCore.Http;

namespace XxlJobExecutorSharp.Abstractions
{
    public interface IActionResponse
    {
        object Result { get; set; }

        HttpContext HttpContext { get; set; }

        string ExecuteResult(HttpContext context);
    }
}
