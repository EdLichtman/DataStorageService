using Microsoft.EntityFrameworkCore;
using DataStorageService.Helpers;
using Microsoft.Data.Sqlite;

namespace DataStorageService.Endpoints.DataStorage.AggregateData
{
    public class AggregateDataContext : DbContext
    {
     
        public static string GetSqliteString(string connectionStringFileLocation)
        {
            if (OperatingSystemHelpers.IsOSWindows)
                if (connectionStringFileLocation.EndsWith(".db"))
                    connectionStringFileLocation = connectionStringFileLocation
                        .Remove(connectionStringFileLocation.Length - 3);

            var databaseConnectionString = new SqliteConnectionStringBuilder
            {
                DataSource = connectionStringFileLocation
            };

            return $"{databaseConnectionString}";
        }
        public AggregateDataContext(DbContextOptions options) : base(options) { 
            
        }

        public DbSet<AggregateDataPoint> AggregateDataPoints { get; set; }
    }
}
