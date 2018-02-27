using System;
using Microsoft.EntityFrameworkCore;

namespace DataStorageService.Endpoints.DataStorage
{
    public class ImportedDataContext : DbContext
    {
        public ImportedDataContext(DbContextOptions options) : base(options) {}
        public DbSet<ImportedDataPoint> ImportedDataPoints { get; set; }
    }
}
