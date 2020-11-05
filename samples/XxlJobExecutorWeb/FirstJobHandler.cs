using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using XxlJobExecutorSharp.Abstractions;
using XxlJobExecutorSharp.Entity;

namespace XxlJobExecutorWeb
{
    [XxlJobHandler("first")]
    public class FirstJobHandler : IXxlJobExecutorHandler
    {
        private readonly ILogger<FirstJobHandler> _logger;

        public FirstJobHandler(ILogger<FirstJobHandler> logger)
        {
            _logger = logger;
        }

        public Task<ReturnT> Execute(JobExecuteContext context)
        {
            _logger.LogInformation($"Hello, {context.LogId}, {context.JobParameter}");

            return Task.FromResult(ReturnT.Success());
        }
    }
}
