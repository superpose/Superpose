using SuperposeLib.Models;

namespace SuperposeLib.Interfaces.Converters
{
    public interface IJobParser
    {
        TJob Execute<TJob>(string data) where TJob : JobLoad, new();
    }
}