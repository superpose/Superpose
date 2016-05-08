namespace Superpose.StorageInterface.Converters
{
    public interface IJobSerializer
    {
        string Execute(object jobLoad);
    }
}