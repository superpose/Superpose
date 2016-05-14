using System;
using System.Threading;
using SuperposeLib.Interfaces.JobThings;

namespace SuperposeLib.Core
{
    public interface IJobRunner
    {
        Timer Timer { set; get; }
        IJobFactory JobFactory { get; set; }
        bool Run(Action<string> onRunning, Action<string> runningCompleted);
    }
}