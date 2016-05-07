using Superpose.StorageInterface.Storage;

namespace Superpose.Storage.LiteDB
{
    public class LiteDBStorageResetter : IJobStorageReseter
    {
        public void ReSet()
        {
            LiteDbCollectionsFactory.UseLiteDatabase(jobLoadCollection =>
            {
                foreach (var jobLoadCollectionEntity in jobLoadCollection.FindAll())
                {
                    jobLoadCollection.Delete(x => x.Id == jobLoadCollectionEntity.Id);
                }
            });
        }
    }
}