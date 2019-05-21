using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace BackgroundTasks.services
{
    public interface IExportService
    {
        Task Export(System.Threading.CancellationToken cancellationToken);
    }

    public class ExportService : IExportService
    {
        public async Task Export(CancellationToken cancellationToken)
        {
            await Task.Run(() => Debug.WriteLine("Exporting..."));
            await Task.Delay(5000);
            await Task.Run(() => Debug.WriteLine("Finishing export..."));
        }
    }

}