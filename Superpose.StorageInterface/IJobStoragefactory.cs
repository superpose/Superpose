namespace Superpose.StorageInterface
{
    public interface IJobStoragefactory
    {
        string GetCurrentExecutionInstance();
        IJobStorage GetJobStorage(string instanceId=null);
    }
}