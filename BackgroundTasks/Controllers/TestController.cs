using BackgroundTasks.Data;
using BackgroundTasks.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace BackgroundTasks.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly IBackgroundTaskQueue _queue;
        private readonly ILogger _logger;
        private readonly IServiceScopeFactory _serviceScopeFactory;

        public TestController(AppDbContext db, IBackgroundTaskQueue queue,
            ILogger<TestController> logger, IServiceScopeFactory serviceScopeFactory)
        {
            _db = db;
            _queue = queue;
            _logger = logger;
            _serviceScopeFactory = serviceScopeFactory;
        }

        /// <summary>
        /// Queues a request
        /// </summary>
        /// <remarks>
       ///Queue a request
        /// </remarks>
        /// <response code="200">Returns success</response>
        [Route("AddMessage")]
        [ProducesResponseType(200)]
        [HttpPost]
   
        public IActionResult AddMessage()
        {
            _queue.QueueBackgroundWorkItem(async token =>
            {
                var guid = Guid.NewGuid().ToString();

                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var scopedServices = scope.ServiceProvider;
                    var db = scopedServices.GetRequiredService<AppDbContext>();
                        try
                        {
                        db.Messages.Add(
                            new Message()
                            {
                                Text = $"Queued Background Task {guid} "
                                });
                            await db.SaveChangesAsync();
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex,
                                "An error occurred writing to the " +
                                $"database. Error: {ex.Message}");
                        }
                        await Task.Delay(TimeSpan.FromSeconds(5), token);
                }

            });

            return Ok("request queued");
        }
    }
}
