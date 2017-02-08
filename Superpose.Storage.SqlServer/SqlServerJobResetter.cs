using System.Data.Entity;
using System.Linq;
using Superpose.StorageInterface;

namespace Superpose.Storage.SqlServer
{
    public class SqlServerJobResetter : IJobStorageReseter
    {
        public void ReSet(string instanceId)
        {
            Database.SetInitializer(new DropCreateDatabaseAlways<SuperPoseContext>());
        }
    }
}