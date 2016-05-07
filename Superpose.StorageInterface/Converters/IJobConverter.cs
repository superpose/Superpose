namespace Superpose.StorageInterface.Converters
{
    public interface IJobConverter
    {
        IJobParser JobParser { set; get; }
        IJobSerializer JobSerializer { set; get; }

        string Serialize(IJobLoad jobLoad);

        IJobLoad Parse(string data);
    }
}