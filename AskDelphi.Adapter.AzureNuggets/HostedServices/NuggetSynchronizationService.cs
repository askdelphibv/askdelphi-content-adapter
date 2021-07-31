using AskDelphi.Adapter.AzureNuggets.Services;
using AskDelphi.Adapter.AzureNuggets.Services.AzureFilesNuggets;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AskDelphi.Adapter.AzureNuggets.HostedServices
{
    public class NuggetSynchronizationService : IHostedService, IDisposable
    {
        private readonly ILogger logger;
        private readonly INuggetContentProvider nuggetContentProvider;
        private readonly IOperationContextFactory operationContextFactory;
        private Timer timer;

        public NuggetSynchronizationService(ILogger<NuggetSynchronizationService> logger, INuggetContentProvider nuggetContentProvider, IOperationContextFactory operationContextFactory)
        {
            this.logger = logger;
            this.nuggetContentProvider = nuggetContentProvider;
            this.operationContextFactory = operationContextFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("NuggetSynchronizationService is starting.");

            timer = new Timer(async (_) => await DoWorkAsync(), null, TimeSpan.Zero, TimeSpan.FromMinutes(1));

            return Task.CompletedTask;
        }

        private async Task DoWorkAsync()
        {
            logger.LogInformation("NuggetSynchronizationService is working.");
            await nuggetContentProvider.SynchronizeCache(operationContextFactory.CreateBackgroundOperationContext());
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("NuggetSynchronizationService is stopping.");

            timer?.Change(Timeout.Infinite, 0);

            return Task.CompletedTask;
        }

        public void Dispose()
        {
            timer?.Dispose();
        }
    }
}
