using SuperposeLib.Interfaces.JobThings;

namespace SuperposeLib.Interfaces.Converters
{
    public interface IJobParser
    {
        IJobLoad Execute(string data);
    }
}