using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using XxlJobExecutorSharp.Abstractions;

namespace XxlJobExecutorSharp
{
    public class XxlJobBackgroundService : IHostedService, IDisposable
    {
        private bool _started = false;

        private readonly IServiceProvider _provider;

        private readonly IProcessor _processor;

        public XxlJobBackgroundService(IServiceProvider provider,
            IProcessor processor)
        {
            _provider = provider;
            _processor = processor;
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
                Task.Run(() => StopInnerAsync(new CancellationToken()));
            }
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            if (!_started)
            {
                await _processor.Start(cancellationToken);

                _started = true;
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await StopInnerAsync(cancellationToken);
        }

        private async Task StopInnerAsync(CancellationToken cancellationToken)
        {
            if (_started)
            {
                await _processor.Stop(cancellationToken);
                _started = false;
            }
        }
    }
}
