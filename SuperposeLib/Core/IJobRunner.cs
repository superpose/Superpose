using SuperposeLib.Interfaces.JobThings;

namespace SuperposeLib.Core
{
    public interface IJobRunner
    {
        bool Run();
        IJobFactory JobFactory { get; }
    }
}