namespace Superpose.StorageInterface.Converters
{
    public interface IJobParser
    {
        IJobLoad Execute(string data);
    }
}