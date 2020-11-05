using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using XxlJobExecutorSharp.Abstractions;

namespace XxlJobExecutorSharp.Action
{
    public class OutCompleteController : IJobController
    {
        public Task<IActionResponse> ActionAsync(HttpContext context)
        {
            throw new NotImplementedException();
        }
    }
}
