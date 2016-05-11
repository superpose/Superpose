using System.Data.Entity;
using Superpose.StorageInterface;

namespace Superpose.Storage.SqlServer
{
    public class SuperPoseContext : DbContext
    {
        public SuperPoseContext(): base("DefaultConnection")
        {
            
        }
        public DbSet<SerializableJobLoad> JobLoads { get; set; }
        public DbSet<JobQueue> JobQueue { set; get; }
    }
}