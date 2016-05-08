namespace Superpose.StorageInterface.Converters
{
    public interface IJobConverter
    {
        IJobParser JobParser { set; get; }
        IJobSerializer JobSerializer { set; get; }

        string SerializeJobLoad(IJobLoad jobLoad);
        string SerializeJobCommand(AJobCommand aJobLoad);
        IJobLoad Parse(string data);
    }
}