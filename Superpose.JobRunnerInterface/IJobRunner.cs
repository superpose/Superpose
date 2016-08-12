using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using SuperposeLib.Interfaces.JobThings;

namespace Superpose.JobRunnerInterface
{
    public interface IJobRunner
    {
        Timer Timer { set; get; }
        IJobFactory JobFactory { get; set; }
        Task<bool> RunAsync(Action<string> onRunning, Action<string> runningCompleted);
    }
}
