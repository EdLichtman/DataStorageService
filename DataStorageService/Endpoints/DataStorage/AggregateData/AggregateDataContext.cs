using System;
using Microsoft.EntityFrameworkCore;
using DataStorageService.Helpers;
using Microsoft.Data.Sqlite;

namespace DataStorageService.Endpoints.DataStorage.AggregateData
{
    public class AggregateDataContext : DbContext
    {

        public static string GetDbLocation(string connectionStringFileLocation)
        {
            //if (OperatingSystemHelpers.IsOSWindows)
            //    if (connectionStringFileLocation.EndsWith(".sqlite"))
            //        connectionStringFileLocation = connectionStringFileLocation
            //            .Remove(connectionStringFileLocation.Length - 3);
            return connectionStringFileLocation;
        }

        public static string GetSqliteString(string connectionStringFileLocation)
        {
            connectionStringFileLocation = GetDbLocation(connectionStringFileLocation);

            var databaseConnectionString = new SqliteConnectionStringBuilder
            {
                DataSource = connectionStringFileLocation
            };

            return $"{databaseConnectionString}";
        }
        public AggregateDataContext(DbContextOptions options) : base(options) {
            try
            {
                Database.EnsureDeleted();
                Database.EnsureCreated();
                Database.Migrate();

            }
            catch (SqliteException e)
            {
                //bug that apparently creates it anyway
            }
        }

        public DbSet<AggregateDataPoint> AggregateDataPoints { get; set; }
    }
}
