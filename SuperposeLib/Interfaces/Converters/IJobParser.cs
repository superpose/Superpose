using SuperposeLib.Interfaces.JobThings;
using SuperposeLib.Models;

namespace SuperposeLib.Interfaces.Converters
{
    public interface IJobParser
    {
        IJobLoad Execute(string data);
    }
}