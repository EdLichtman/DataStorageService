using System;
using DataStorageService.Helpers;
using DataStorageService.AppSettings;

namespace DataStorageServiceTests.TestData            
{
    public class TestApplicationSettings : IApplicationSettings
    {

        private const string sqliteStorageFolderName = "TransmittedFiles";
        public string SqliteStorageFolderName => sqliteStorageFolderName;

        public string SqliteStorageFolderLocation => $"{DirectoryHelpers.GetSolutionRoot()}/DataStorageServiceTests/{sqliteStorageFolderName}";

        private const string aggregateSqliteFileName = "AggregateData.db";
        public string AggregateSqliteFileName => aggregateSqliteFileName;
    }
}
