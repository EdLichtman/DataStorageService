using System;
using NUnit.Framework;
using System.IO;
using DataStorageService.AppSettings;
using DataStorageServiceTests.TestData;
using DataStorageService.Endpoints.DataStorage;
using Microsoft.EntityFrameworkCore;

namespace DataStorageServiceTests.Endpoints.DataStorage
{
    [TestFixture]
    public class ImportedDataRepositoryTests
    {
        private readonly IApplicationSettings _appSettings;
        private readonly IImportedDataRepository _importedData;
        private string currentTestSqliteFile =>
            $"{_appSettings.SqliteStorageFolderLocation}/{_appSettings.AggregateSqliteFileName}";
        public ImportedDataRepositoryTests()
        {
            _appSettings = new TestApplicationSettings();
            var inMemoryDbOption = new DbContextOptionsBuilder<AggregateDataContext>()
                .UseInMemoryDatabase(databaseName: "Add_writes_to_database")
                .Options;
            var context = new ImportedDataContext(inMemoryDbOption);

            _importedData = new IImportedDataRepository(context);
        }

        [SetUp]
        public void SetUp()
        {
            Directory.CreateDirectory(_appSettings.SqliteStorageFolderLocation);
        }
        [TearDown]
        public void TearDown()
        {
            try
            {
                File.Delete(currentTestSqliteFile);
                Directory.Delete(_appSettings.SqliteStorageFolderLocation, false);
            }
            catch
            {
                //if it fails it's nbd, each file is based on a time stamp
            }
        }
        [Test]
        public void Can_convert_sqlite_to_memory()
        {

        }

        [Test]
        public void Can_commit_memory_to_sqlite()
        {


            Assert.That(currentTestSqliteFile, Does.Exist);
        }
    }
}
