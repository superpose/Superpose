using System;
using LiteDB;
using Newtonsoft.Json;
using Superpose.StorageInterface;

namespace Superpose.Storage.LiteDB
{
    public class LiteDBJobSaver : IJobSaver
    {
        public LiteDBJobSaver()
        {
            BsonMapper.Global.RegisterType(state => Enum.GetName(typeof (JobStateType), state),
                bson => (JobStateType) Enum.Parse(typeof (JobStateType), bson)
                );
        }

        public void Dispose()
        {
        }

        public void SaveNew(string data, string Id)
        {
            LiteDbCollectionsFactory.UseLiteDatabase(jobLoadCollection =>
            {
                var doc = JsonConvert.DeserializeObject<SerializableJobLoad>(data);

                jobLoadCollection.Insert(doc);

                CreateIndexes(jobLoadCollection);
            });
        }

        public void Update(string data, string Id)
        {
            LiteDbCollectionsFactory.UseLiteDatabase(jobLoadCollection =>
            {
                var doc = JsonConvert.DeserializeObject<SerializableJobLoad>(data);
                jobLoadCollection.Update(doc);
                CreateIndexes(jobLoadCollection);
            });
        }

        private static void CreateIndexes(LiteCollection<SerializableJobLoad> jobLoadCollection)
        {
            jobLoadCollection.EnsureIndex(x => x.Id);
            jobLoadCollection.EnsureIndex(x => x.LastUpdated);
            jobLoadCollection.EnsureIndex(x => x.JobStateTypeName);
            jobLoadCollection.EnsureIndex(x => x.JobTypeFullName);
            jobLoadCollection.EnsureIndex(x => x.Started);
            jobLoadCollection.EnsureIndex(x => x.TimeToRun);
        }
    }
}