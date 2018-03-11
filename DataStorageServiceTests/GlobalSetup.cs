using NUnit.Framework;
using DataStorageServiceTests.TestData;
using System.IO;
using DataStorageService.AppSettings;

namespace DataStorageServiceTests
{
    [SetUpFixture]
    public class GlobalSetup
    {
        private readonly IApplicationSettings _applicationSettings;
        public GlobalSetup()
        {
            _applicationSettings = new TestApplicationSettings();
        }
        [OneTimeSetUp]
        public void SetUp()
        {
            //var applicationSettings = new TestApplicationSettings();
            Directory.CreateDirectory(_applicationSettings.SqliteStorageFolderLocation);
            Directory.CreateDirectory(_applicationSettings.CompletedImportSqliteStorageFolderLocation);
        }
    }
}
