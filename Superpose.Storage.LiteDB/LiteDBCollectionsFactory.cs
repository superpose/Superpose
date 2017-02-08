using System;
using LiteDB;
using Superpose.StorageInterface;

namespace Superpose.Storage.LiteDB
{
    public class LiteDbCollectionsFactory
    {
        public static void UseLiteDatabase(string instanceId,Action<LiteCollection<SerializableJobLoad>> dbOperation)
        {
            if (dbOperation == null) throw new ArgumentNullException(nameof(dbOperation));

            using (var db = new LiteDatabase(@"LITEDB_SUPERPOSE_DB_"+ instanceId + ".db"))
            {
                var jobLoadCollection = db.GetCollection<SerializableJobLoad>(typeof (SerializableJobLoad).Name);
                dbOperation(jobLoadCollection);
            }
        }
    }
}