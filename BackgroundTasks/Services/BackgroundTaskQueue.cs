using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace BackgroundTasks.Services
{
    public interface IBackgroundTaskQueue
    {
        void QueueBackgroundWorkItem(Func<CancellationToken, Task> workItem);
        Task<Func<CancellationToken, Task>> DequeueAsync(
            CancellationToken cancellationToken);
    }

    public class BackgroundTaskQueue : IBackgroundTaskQueue
    {
        private ConcurrentQueue<Func<CancellationToken, Task>> _workItems =
            new ConcurrentQueue<Func<CancellationToken, Task>>();
        private SemaphoreSlim _signal = new SemaphoreSlim(0);

        public void QueueBackgroundWorkItem(
            Func<CancellationToken, Task> workItem)
        {
            if (workItem == null)
            {
                throw new ArgumentNullException(nameof(workItem));
            }

            _workItems.Enqueue(workItem);
            _signal.Release();
        }

        public async Task<Func<CancellationToken, Task>> DequeueAsync(
            CancellationToken cancellationToken)
        {
            await _signal.WaitAsync(cancellationToken);
            _workItems.TryDequeue(out var workItem);

            return workItem;
        }
    }

    public class QueueBackgroundService : BackgroundService
    {
        private readonly ILogger _logger;

        public QueueBackgroundService(IBackgroundTaskQueue taskQueue, ILoggerFactory loggerFactory)
        {
            TaskQueue = taskQueue;
            _logger = loggerFactory.CreateLogger<QueueBackgroundService>();
        }

        public IBackgroundTaskQueue TaskQueue { get; }

        protected async override Task ExecuteAsync(
            CancellationToken cancellationToken)
        {
            _logger.LogInformation("Queued Hosted Service is starting.");

            while (!cancellationToken.IsCancellationRequested)
            {
                var workItem = await TaskQueue.DequeueAsync(cancellationToken);

                try
                {
                    await workItem(cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex,
                       $"Error occurred executing {nameof(workItem)}.");
                }
            }

            _logger.LogInformation("Queued Hosted Service is stopping.");
        }

        // Schedules a task which can run in the background, independent of any request.
        // This differs from a normal ThreadPool work item in that ASP.NET can keep track
        // of how many work items registered through this API are currently running, and
        // the ASP.NET runtime will try not to delay AppDomain shutdown until these work
        // items have finished executing.
        //
        // Usage notes:
        // - This API cannot be called outside of an ASP.NET-managed AppDomain.
        // - The caller's ExecutionContext is not flowed to the work item.
        // - Scheduled work items are not guaranteed to ever execute, e.g., when AppDomain
        //   shutdown has already started by the time this API was called.
        // - The provided CancellationToken will be signaled when the application is
        //   shutting down. The work item should make every effort to honor this token.
        //   If a work item does not honor this token and continues executing it will
        //   eventually be considered rogue, and the ASP.NET runtime will rudely unload
        //   the AppDomain without waiting for the work item to finish.
        //

    }
}


