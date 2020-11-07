using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using XxlJobExecutorSharp.Abstractions;
using XxlJobExecutorSharp.Entity;

namespace XxlJobExecutorSharp.Services
{
    public class XxlJobQueue : IDisposable
    {

        private readonly Queue<JobMessage> _jobs;

        private bool _delegateQueuedOrRunning = false;

        private readonly CancellationTokenSource _cts = new CancellationTokenSource();

        private readonly ILogger<XxlJobQueue> _logger;

        private readonly IJobSender _sender;

        private readonly XxlJobOptions _options;

        private readonly IServiceProvider _serviceProvider;

        public XxlJobQueue(ILogger<XxlJobQueue> logger,
            IJobSender sender,
            XxlJobOptions options,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _sender = sender;
            _options = options;
            _serviceProvider = serviceProvider;
            _jobs = new Queue<JobMessage>();
        }

        public Task Enqueue(JobMessage job)
        {
            lock (_jobs)
            {
                _jobs.Enqueue(job);
                if (!_delegateQueuedOrRunning)
                {
                    _delegateQueuedOrRunning = true;
                    ThreadPool.UnsafeQueueUserWorkItem(ProcessQueuedItems, null);
                }
            }

            return Task.CompletedTask;
        }

        private void ProcessQueuedItems(object ignored)
        {
            while (true)
            {
                JobMessage job;
                lock (_jobs)
                {
                    if (_jobs.Count == 0)
                    {
                        _delegateQueuedOrRunning = false;
                        break;
                    }

                    job = _jobs.Dequeue();
                }

                XxlJobUtilExtensions.RunSync(async () =>
                {
                    try
                    {
                        var result = await _sender.Execute(job);
                        if (result.Code == XxlJobConstant.HTTP_FAIL_CODE)
                        {
                            _logger.LogError($"执行失败. Id:{job.Id}, Msg: {result.Msg}");
                        }

                        job.CallBackCode = result.Code;
                        job.Reason = result.Msg;
                        await _sender.CallBack(new List<JobMessage>() { job });
                    }
                    catch (Exception ex)
                    {
                        var failedInfo = new FailedInfo()
                        {
                            ServiceProvider = _serviceProvider,
                            Exception = ex,
                            Job = job,
                            Message = $"执行异常. Id:{job.Id}"
                        };

                        _options?.FailedCallback?.Invoke(failedInfo);
                        _logger.LogError(ex, failedInfo.Message);
                    }
                });
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _cts.Cancel();
            }
        }

        public int Count => _jobs.Count;

        public List<JobMessage> Kill(string killedReason)
        {
            var killedJobs = new List<JobMessage>();
            foreach (var item in _jobs)
            {
                if (item.Kill())
                {
                    item.Reason = killedReason;
                    killedJobs.Add(item);
                }
            }

            return killedJobs;
        }
    }
}
