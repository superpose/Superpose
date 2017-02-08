using Superpose.StorageInterface;

namespace Superpose.Storage.LiteDB
{
    public class LiteDBStorageResetter : IJobStorageReseter
    {
        public void ReSet(string instanceId)
        {
            LiteDbCollectionsFactory.UseLiteDatabase(instanceId, jobLoadCollection =>
            {
                foreach (var jobLoadCollectionEntity in jobLoadCollection.FindAll())
                {
                    jobLoadCollection.Delete(x => x.Id == jobLoadCollectionEntity.Id);
                }
            });
        }
    }
}