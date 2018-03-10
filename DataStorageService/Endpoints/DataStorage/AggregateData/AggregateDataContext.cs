using Microsoft.EntityFrameworkCore;
namespace DataStorageService.Endpoints.DataStorage.AggregateData
{
    public class AggregateDataContext : DbContext
    {
        
        public AggregateDataContext(DbContextOptions options) : base(options) { 
            //Database.SetInitializer(new DropCreateDatabaseIfModelChanges<MyContext>());
        }

        public DbSet<AggregateDataPoint> AggregateDataPoints { get; set; }
    }
}
