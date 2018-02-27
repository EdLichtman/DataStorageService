using System;
namespace DataStorageService.AppSettings
{
    public interface IApplicationSettings
    {
        string SqliteStorageFolderName { get; }
        string SqliteStorageFolderLocation { get; }
        string AggregateSqliteFileName { get; }
    }
}
