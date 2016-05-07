using Superpose.StorageInterface;


namespace SuperposeLib.Interfaces.Converters
{
    public interface IJobParser
    {
        IJobLoad Execute(string data);
    }
}