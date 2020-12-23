using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using XxlJobExecutorSharp.Abstractions;
using XxlJobExecutorSharp.Entity;

namespace XxlJobExecutorSharp.Services
{
    public class XxlJobSender : IJobSender
    {
        private readonly IXxlJobServicesProvider<IXxlJobExecutorHandler> _handlerProvider;

        private readonly ILogger<XxlJobSender> _logger;

        private readonly IXxlJobExecutor _adminExecutor;

        private readonly XxlJobOptions _options;

        public XxlJobSender(IXxlJobServicesProvider<IXxlJobExecutorHandler> handlerProvider,
            ILogger<XxlJobSender> logger,
            IXxlJobExecutor adminExecutor,
            XxlJobOptions options)
        {
            _handlerProvider = handlerProvider;
            _logger = logger;
            _adminExecutor = adminExecutor;
            _options = options;
        }

        public async Task CallBack(List<JobMessage> jobs)
        {
            if (jobs == null || jobs.Count <= 0)
            {
                return;
            }

            bool retry;
            var callBackResult = new CallBackResult();
            do
            {
                var executedResult = await CallBackWithoutRetryAsync(callBackResult, jobs);
                var result = executedResult.Item1;
                if (result)
                {
                    return;
                }
                retry = executedResult.Item2;
            } while (retry);
        }

        private async Task<(bool, bool)> CallBackWithoutRetryAsync(CallBackResult callBackResult, List<JobMessage> jobs)
        {
            var result = await _adminExecutor.CallBackExecutor(jobs.Select(x => new CallBackInfo()
            {
                Code = x.CallBackCode,
                LogId = x.LogId,
                Message = x.Reason
            }).ToList());

            var needRetry = false;
            if (!result)
            {
                var retries = ++callBackResult.Retries;
                var retryCount = Math.Min(_options.CallBackRetryCount, 3);
                if (retries >= retryCount)
                {
                    needRetry = false;
                    _logger.LogError($"xxljob任务结果回调失败,已超过重试次数");
                }

                needRetry = true;
                _logger.LogError($"xxljob任务结果回调失败,logIds:{string.Join(",", jobs.Select(x => x.Id))},retries:{retries}");
            }

            return (result, needRetry);
        }

        public async Task<ReturnT> Execute(JobMessage job)
        {
            var handler = await _handlerProvider.GetInstanceAsync(job.ExecutorHandler);
            if (handler == null)
            {
                return ReturnT.Failed($"没有找到名为{job.ExecutorHandler}的JobHandler");
            }

            // if killed
            if (job.Status == JobStatus.Killed)
            { 
                return ReturnT.Failed($"任务已被终止,id:{job.Id}, reason: {job.Reason}, logId: {job.LogId}");
            }

            job.Run();

            try
            {
                return await handler.Execute(new JobExecuteContext(job.Id, job.ExecutorParams));
            }
            catch (Exception e)
            {
                _logger.LogError(e, "xxljob执行任务错误");
                return ReturnT.Failed(e.Message);
            }
        }
    }
}
