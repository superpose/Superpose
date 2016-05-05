using LiteDB;
using Newtonsoft.Json;
using SuperposeLib.Interfaces.Storage;
using SuperposeLib.Models;

namespace Superpose.Storage.LiteDB
{
    public class LiteDBJobSaver : IJobSaver
    {
        private LiteCollection<JobLoadCollectionEntity> GetJobLoadCollection()
        {
            var jobLoadCollection = DB.GetCollection<JobLoadCollectionEntity>(typeof(JobLoadCollectionEntity).Name);
            return jobLoadCollection;
        }

        public LiteDBJobSaver()
        {
            DB = new LiteDatabase(@"MyData.db");
        }
        public LiteDatabase DB { get; set; }

        public void Dispose()
        {
            DB.Dispose();
        }

        public void SaveNew(string data, string Id)
        {
            GetJobLoadCollection().Insert(new JobLoadCollectionEntity()
            {
                Id = Id,
                JobLoad = JsonConvert.DeserializeObject<JobLoad>(data)
            });
        }

        public void Update(string data, string Id)
        {
            GetJobLoadCollection().Update(new JobLoadCollectionEntity()
            {
                Id = Id,
                JobLoad = JsonConvert.DeserializeObject<JobLoad>(data)
            });
        }
    }
}