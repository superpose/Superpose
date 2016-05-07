namespace Superpose.StorageInterface.Converters
{
    public interface IJobSerializer
    {
        string Execute<TJob>(TJob jobLoad) where TJob : IJobLoad;
    }
}