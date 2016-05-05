using LiteDB;
using SuperposeLib.Interfaces.Storage;

namespace Superpose.Storage.LiteDB
{
    public class LiteDBStorageResetter : IJobStorageReseter
    {
        private LiteCollection<JobLoadCollectionEntity> GetJobLoadCollection()
        {
            var jobLoadCollection = DB.GetCollection<JobLoadCollectionEntity>(typeof(JobLoadCollectionEntity).Name);
            return jobLoadCollection;
        }
        public LiteDatabase DB { get; set; }
        public LiteDBStorageResetter()
        {
            DB = new LiteDatabase(@"MyData.db");
        }
        public void ReSet()
        {
            foreach (var jobLoadCollectionEntity in GetJobLoadCollection().FindAll())
            {
                GetJobLoadCollection().Delete(x => x.Id == jobLoadCollectionEntity.Id);
            }
        }
    }
}