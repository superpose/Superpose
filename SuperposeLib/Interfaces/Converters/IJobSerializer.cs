using SuperposeLib.Interfaces.JobThings;

namespace SuperposeLib.Interfaces.Converters
{
    public interface IJobSerializer
    {
        string Execute<TJob>(TJob jobLoad) where TJob : IJobLoad;
    }
}