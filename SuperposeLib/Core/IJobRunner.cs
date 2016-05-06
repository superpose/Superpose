using SuperposeLib.Interfaces.JobThings;

namespace SuperposeLib.Core
{
    public interface IJobRunner
    {
        IJobFactory JobFactory { get; }
        bool Run();
    }
}