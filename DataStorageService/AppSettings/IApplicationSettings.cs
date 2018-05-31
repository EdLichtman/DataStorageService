using System;
using DataStorageService.Features.EmailClient;

namespace DataStorageService.AppSettings
{
    public interface IApplicationSettings
    {
        string SqliteStorageFolderName { get; }
        string SqliteStorageFolderLocation { get; }
        string AggregateSqliteFileName { get; }
        string CompletedImportSqliteStorageFolderLocation { get; }
    }
}
