using System;
using DataStorageService.Helpers;
namespace DataStorageService.AppSettings
{
    public class ApplicationSettings: IApplicationSettings
    {
        private const string sqliteStorageFolderName = "TransmittedFiles";
        public string SqliteStorageFolderName => sqliteStorageFolderName;

        public string SqliteStorageFolderLocation => DirectoryHelpers.GetSqliteStorageRoot();

        private const string aggregateSqliteFileName = "AggregateData.db";
        public string AggregateSqliteFileName => aggregateSqliteFileName;

        private const string completedImportSqliteStorageFolderLocation = "TransmittedFilesAlreadyImported";
        public string CompletedImportSqliteStorageFolderLocation => 
            $"{DirectoryHelpers.GetDataStorageServiceProjectRoot()}/" +
            $"{completedImportSqliteStorageFolderLocation}";
    }
}
