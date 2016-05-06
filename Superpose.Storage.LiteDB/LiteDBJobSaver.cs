using System;
using LiteDB;
using Newtonsoft.Json;
using SuperposeLib.Extensions;
using SuperposeLib.Interfaces.Storage;
using SuperposeLib.Models;

namespace Superpose.Storage.LiteDB
{
    public class LiteDBJobSaver : IJobSaver
    {
        public LiteDBJobSaver()
        {
            BsonMapper.Global.RegisterType(
             serialize: (state) => Enum.GetName(typeof(JobStateType), state),
             deserialize: (bson) => (JobStateType)Enum.Parse(typeof(JobStateType), bson)
           );
        }

        public void Dispose()
        {

        }

        public void SaveNew(string data, string Id)
        {
            LiteDbCollectionsFactory.UseLiteDatabase((jobLoadCollection) =>
            {
                var doc = JsonConvert.DeserializeObject<SerializableJobLoad>(data);

                jobLoadCollection.Insert(doc);

                CreateIndexes(jobLoadCollection);
            });


        }

        private static void CreateIndexes(LiteCollection<SerializableJobLoad> jobLoadCollection)
        {
            jobLoadCollection.EnsureIndex(x => x.Id);
            jobLoadCollection.EnsureIndex(x => x.Ended);
            jobLoadCollection.EnsureIndex(x => x.JobStateTypeName);
            jobLoadCollection.EnsureIndex(x => x.JobTypeFullName);
            jobLoadCollection.EnsureIndex(x => x.Started);
            jobLoadCollection.EnsureIndex(x => x.TimeToRun);
        }

        public void Update(string data, string Id)
        {
            LiteDbCollectionsFactory.UseLiteDatabase((jobLoadCollection) =>
            {
                var doc = JsonConvert.DeserializeObject<SerializableJobLoad>(data);
                jobLoadCollection.Update(doc);
                CreateIndexes(jobLoadCollection);
            });

        }
    }
}