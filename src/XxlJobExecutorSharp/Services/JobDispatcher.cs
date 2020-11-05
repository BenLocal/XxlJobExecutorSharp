using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using XxlJobExecutorSharp.Abstractions;
using XxlJobExecutorSharp.Entity;

namespace XxlJobExecutorSharp.Services
{
    public class JobDispatcher : IJobDispatcher, IDisposable
    {
        private readonly ConcurrentDictionary<int, XxlJobQueue> _executingJobs;

        private readonly CancellationTokenSource _cts = new CancellationTokenSource();

        private readonly ILogger<JobDispatcher> _logger;

        private readonly Channel<JobMessage> _publishedChannel;

        private readonly IJobSender _sender;

        private readonly IServiceProvider _serviceProvider;

        public JobDispatcher(ILogger<JobDispatcher> logger,
            IJobSender sender,
            IServiceProvider serviceProvider)
        {
            _logger = logger;
            _sender = sender;
            _serviceProvider = serviceProvider;
            _publishedChannel = Channel.CreateUnbounded<JobMessage>(new UnboundedChannelOptions() { SingleReader = true, SingleWriter = true });
            _executingJobs = new ConcurrentDictionary<int, XxlJobQueue>();
            Task.Factory.StartNew(Sending, _cts.Token, TaskCreationOptions.LongRunning, TaskScheduler.Default);
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

        /// <summary>
        /// 添加任务
        /// </summary>
        /// <param name="jobInfo"></param>
        /// <returns></returns>
        public async Task Enqueue(JobMessage job)
        {
            await _publishedChannel.Writer.WriteAsync(job);
        }

        public Task<bool> Kill(int jobId, string killedReason)
        {
            var killedJobs = new List<JobMessage>();
            if (_executingJobs.TryGetValue(jobId, out XxlJobQueue queue))
            {
                killedJobs = queue.Kill(killedReason);
            }

            if (killedJobs.Count > 0)
            {
                Task.Run(async () => await _sender.CallBack(killedJobs));
            }

            return Task.FromResult(killedJobs.Count > 0);
        }

        private async Task Sending()
        {
            try
            {
                while (await _publishedChannel.Reader.WaitToReadAsync(_cts.Token))
                {
                    while (_publishedChannel.Reader.TryRead(out var job))
                    {
                        try
                        {
                            if (!_executingJobs.ContainsKey(job.Id))
                            {
                                var newQueue = _serviceProvider.GetRequiredService<XxlJobQueue>();
                                _executingJobs.TryAdd(job.Id, newQueue);
                            }

                            XxlJobQueue queue = _executingJobs[job.Id];
                            await queue.Enqueue(job);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, $"An exception occurred when sending a message to the queue. Id:{job.Id}");
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // expected
            }
        }

        public Task<bool> Exist(int jobId)
        {
            if (_executingJobs.TryGetValue(jobId, out XxlJobQueue queue))
            {
                return Task.FromResult(queue.Count > 0);
            }

            return Task.FromResult(false);
        }
    }
}
