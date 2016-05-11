using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Superpose.StorageInterface;

namespace Superpose.Storage.SqlServer
{
    public class SqlServerJobResetter : IJobStorageReseter
    {
        public void ReSet()
        {
            ClearDatabase<SuperPoseContext>();
        }

        public static void ClearDatabase<T>() where T : DbContext, new()
        {
            using (var context = new T())
            {
                var tableNames = context.Database.SqlQuery<string>("SELECT TABLE_NAME FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_TYPE = 'BASE TABLE' AND TABLE_NAME NOT LIKE '%Migration%'").ToList();
                foreach (var tableName in tableNames)
                {
                    context.Database.ExecuteSqlCommand($"DELETE FROM {tableName}");
                }

                context.SaveChanges();
            }
        }
    }
}