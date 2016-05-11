using System;
using System.Data.Entity.Migrations;
using Newtonsoft.Json;
using Superpose.StorageInterface;

namespace Superpose.Storage.SqlServer
{
    public class SqlServerJobSaver : IJobSaver
    {
       

        public void Dispose()
        {
           
        }

        public void SaveNew(string data, string Id)
        {
            using (var Db = new SuperPoseContext())
            {
  var jobLoad = JsonConvert.DeserializeObject<SerializableJobLoad>(data);
                Db.Entry(jobLoad).State = System.Data.Entity.EntityState.Added;
                // Db.JobLoads.Add(jobLoad);
                Db.SaveChanges();
            }

          
           
        }

        public void Update(string data, string Id)
        {
            using (var Db = new SuperPoseContext())
            {
  var jobLoad = JsonConvert.DeserializeObject<SerializableJobLoad>(data);
            Db.JobLoads.AddOrUpdate(jobLoad);
            Db.SaveChanges();
            }
          
        }
    }
}