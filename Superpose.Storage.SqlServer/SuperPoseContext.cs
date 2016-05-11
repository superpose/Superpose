using System.Data.Entity;
using Superpose.StorageInterface;

namespace Superpose.Storage.SqlServer
{
    public class SuperPoseContext : DbContext
    {
        public DbSet<SerializableJobLoad> JobLoads { get; set; }
    }
}