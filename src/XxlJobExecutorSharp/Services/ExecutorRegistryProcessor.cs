using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using XxlJobExecutorSharp.Abstractions;

namespace XxlJobExecutorSharp.Processor
{
    public class ExecutorRegistryProcessor : IProcessor
    {
        private readonly ILogger _logger;

        private readonly XxlJobOptions _options;

        private readonly IXxlJobExecutor _xxlJobExecutor;

        public ExecutorRegistryProcessor(ILogger<ExecutorRegistryProcessor> logger,
            XxlJobOptions options,
            IXxlJobExecutor xxlJobExecutor)
        {
            _logger = logger;
            _options = options;
            _xxlJobExecutor = xxlJobExecutor;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Task.Run(() => Stop(new CancellationToken()));
            }
        }

        public Task Start(CancellationToken cancellationToken)
        {
            Task.Factory.StartNew(async () =>
            {
                var heartbeatIntervalSecond = Math.Max(_options.HeartbeatIntervalSecond, 30);
                _logger.LogInformation($"开始定时注册执行器,间隔{heartbeatIntervalSecond}秒......");
                try
                {
                    // Registry
                    while (!cancellationToken.IsCancellationRequested)
                    {
                        try
                        {
                            await _xxlJobExecutor.RegistryExecutor();
                            await Task.Delay(TimeSpan.FromSeconds(heartbeatIntervalSecond), cancellationToken);
                        }
                        catch (TaskCanceledException)
                        {
                            _logger.LogInformation("程序关闭");
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "xxljob注册执行器错误，重试中......");
                            await Task.Delay(TimeSpan.FromSeconds(heartbeatIntervalSecond), cancellationToken);
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    //ignore
                }
                catch (Exception e)
                {
                    _logger.LogError(e, e.Message);
                }
            }, cancellationToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);

            return Task.CompletedTask;
        }

        public async Task Stop(CancellationToken cancellationToken)
        {
            try
            {
                await _xxlJobExecutor.RemoveRegistryExecutor();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "xxljob删除执行器错误");
            }
        }
    }
}
