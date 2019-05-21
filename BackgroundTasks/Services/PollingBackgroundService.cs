using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Threading;
using System.Threading.Tasks;

namespace BackgroundTasks.services
{
    public class PollingBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public PollingBackgroundService(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                    await Task.Delay(5000);
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        //example of using service locator pattern to call a scoped service from a hosted service
                        //service locator is not my favorite as you can't tell by looking at the constructor what the dependences of the class are
                        var scopedProcessingService = scope.ServiceProvider.GetRequiredService<IExportService>();
                        await scopedProcessingService.Export(cancellationToken);
                    }
            }
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            //customize startup tasks as web server (host) starts
            return base.StartAsync(cancellationToken);
        }

        public override Task StopAsync(CancellationToken cancellationToken)
        {
            //do any extra clean up
            //base method will call cancel and wait on any outstanding tasks to finish
            return base.StopAsync(cancellationToken);
        }
    }
}
