using System;
using LiteDB;
using SuperposeLib.Models;

namespace Superpose.Storage.LiteDB
{
    public class LiteDbCollectionsFactory
    {
        public static void UseLiteDatabase(Action<LiteCollection<SerializableJobLoad>> dbOPeration)
        {
            if (dbOPeration == null) throw new ArgumentNullException(nameof(dbOPeration));

            using (var db = new LiteDatabase(@"LITEDB_SUPERPOSE_DB.db"))
            {
                var jobLoadCollection = db.GetCollection<SerializableJobLoad>(typeof (SerializableJobLoad).Name);
                dbOPeration(jobLoadCollection);
            }
        }
    }
}