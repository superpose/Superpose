namespace Superpose.StorageInterface
{
    public enum EnqueueStrategy
    {
        Unknown = 0,
        //inline
        Cpu,
        //tell
        Queue,
        //ask
        QueueCpu
    }
}