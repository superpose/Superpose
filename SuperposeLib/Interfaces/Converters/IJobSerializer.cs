using SuperposeLib.Models;

namespace SuperposeLib.Interfaces.Converters
{
    public interface IJobSerializer
    {
        string Execute<TJob>(TJob jobLoad) where TJob : JobLoad, new();
    }
}