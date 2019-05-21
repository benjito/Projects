using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Web.Hosting;
using System.Web.Http;


namespace BackgroundTasksDotNet.Controllers
{
    [RoutePrefix("Test")]
    public class TestController : ApiController
    {
        [HttpPost]
        [Route("QueueSomething")]
        public void QueueSomething()
        {
            if (ModelState.IsValid)
            {
                HostingEnvironment.QueueBackgroundWorkItem(x => DoSomethingAsync(Guid.NewGuid()));
            }

        }

        private async Task  DoSomethingAsync(Guid guid)
        {
            Debug.WriteLine($"Queueing {guid}");
            await Task.Delay(5000);
            Debug.WriteLine($"Finishing {guid}");
        }
    }
}
