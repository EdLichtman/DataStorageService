using Microsoft.EntityFrameworkCore;
namespace DataStorageService.Endpoints.DataStorage.AggregateData
{
    public class AggregateDataContext : DbContext
    {
        public AggregateDataContext(DbContextOptions options) : base(options) { }

        public DbSet<AggregateDataPoint> AggregateDataPoints { get; set; }
    }
}
