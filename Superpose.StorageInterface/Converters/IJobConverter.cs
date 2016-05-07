using Superpose.StorageInterface;


namespace SuperposeLib.Interfaces.Converters
{
    public interface IJobConverter
    {
        IJobParser JobParser { set; get; }
        IJobSerializer JobSerializer { set; get; }

        string Serialize(IJobLoad jobLoad);

        IJobLoad Parse(string data);
    }
}