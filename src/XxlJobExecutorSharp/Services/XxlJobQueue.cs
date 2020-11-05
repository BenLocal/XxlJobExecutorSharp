using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using XxlJobExecutorSharp.Abstractions;
using XxlJobExecutorSharp.Entity;

namespace XxlJobExecutorSharp.Services
{
    public class XxlJobQueue : IDisposable
    {
        private readonly Subject<JobMessage> _jobs;

        private readonly CancellationTokenSource _cts = new CancellationTokenSource();

        private readonly ILogger<XxlJobQueue> _logger;

        private readonly IJobSender _sender;

        public XxlJobQueue(ILogger<XxlJobQueue> logger,
            IJobSender sender)
        {
            _logger = logger;
            _sender = sender;

            _jobs = new Subject<JobMessage>();
            _jobs.Subscribe(async job =>
            {
                await Run(job);
            });
        }

        public Task Enqueue(JobMessage job)
        {
            return Task.Run(()=> _jobs.OnNext(job));
        }

        private async Task Run(JobMessage job)
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
                _logger.LogError(ex, $"执行异常. Id:{job.Id}");
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

        public int Count => _jobs.Count().Wait();

        public List<JobMessage> Kill(string killedReason)
        {
            var killedJobs = new List<JobMessage>();
            foreach (var item in _jobs.ToList().Wait())
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
