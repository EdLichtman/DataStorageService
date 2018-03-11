using System.IO;
using DataStorageService.Helpers;
using DataStorageService.AppSettings;

namespace DataStorageServiceTests.TestData            
{
    public class TestApplicationSettings : IApplicationSettings
    {

        private const string sqliteStorageFolderName = "TransmittedFiles";
        public string SqliteStorageFolderName => sqliteStorageFolderName;

        public string SqliteStorageFolderLocation => Path.Combine(DirectoryHelpers.GetSolutionRoot(),"DataStorageServiceTests",sqliteStorageFolderName);

        private const string aggregateSqliteFileName = "AggregateData.db";
        public string AggregateSqliteFileName => aggregateSqliteFileName;

        private const string completedImportSqliteStorageFolderLocation = "TransmittedFilesAlreadyImported";
        public string CompletedImportSqliteStorageFolderLocation => Path.Combine(DirectoryHelpers.GetSolutionRoot(),"DataStorageServiceTests",completedImportSqliteStorageFolderLocation);
    }
}
