using SuperposeLib.Models;

namespace SuperposeLib.Interfaces.Converters
{
    public interface IJobConverter
    {
        IJobParser JobParser { set; get; }
        IJobSerializer JobSerializer { set; get; }

        string Serialize<TJob>(TJob jobLoad) where TJob : JobLoad, new();

        TJob Parse<TJob>(string data) where TJob : JobLoad, new();
    }
}