using System;
using System.Linq;
using System.Text;
using System.Threading;
using SuperposeLib.Interfaces.JobThings;

namespace Superpose.JobRunnerInterface
{



    public interface IJobRunner
    {
        Timer Timer { set; get; }
        IJobFactory JobFactory { get; set; }
        bool Run(Action<string> onRunning, Action<string> runningCompleted);





    }
}
