using System.Data.Entity.Migrations;
using Newtonsoft.Json;
using Superpose.StorageInterface;

namespace Superpose.Storage.SqlServer
{
    public class SqlServerJobSaver : IJobSaver
    {
        public SqlServerJobSaver()
        {
            Db = new SuperPoseContext();
        }

        public SuperPoseContext Db { get; set; }

        public void Dispose()
        {
            Db.Dispose();
        }

        public void SaveNew(string data, string Id)
        {
            var jobLoad = JsonConvert.DeserializeObject<SerializableJobLoad>(data);
            Db.JobLoads.Add(jobLoad);
            Db.SaveChanges();
        }

        public void Update(string data, string Id)
        {
            var jobLoad = JsonConvert.DeserializeObject<SerializableJobLoad>(data);
            Db.JobLoads.AddOrUpdate(jobLoad);
            Db.SaveChanges();
        }
    }
}