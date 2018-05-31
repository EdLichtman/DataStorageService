using System;
using System.IO;
using NUnit.Framework;
using DataStorageService.Endpoints.DataStorage.AggregateData;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DataStorageServiceTests.TestData;
using Microsoft.EntityFrameworkCore;
using DataStorageService.Endpoints.DataStorage.DatabaseInterfaces;
using DataStorageService.AppSettings;


namespace DataStorageServiceTests.Endpoints.DataStorage.AggregateData
{
    [TestFixture]
    public class AggregateDataRepositoryTests
    {
        private IImportedDataPointRepository _dataPointRepository;
        private IAggregateDataRepository _aggregateDataRepository;
        private IApplicationSettings _applicationSettings;

        [SetUp]
        public void SetUp()
        {
            _applicationSettings = new TestApplicationSettings();
            TearDown();
            var aggregateDataFileLocation = Path.Combine(_applicationSettings.SqliteStorageFolderLocation,_applicationSettings.AggregateSqliteFileName);
            //File.Create(aggregateDataFileLocation);

            var sqliteOption = new DbContextOptionsBuilder<AggregateDataContext>()
                .UseSqlite(AggregateDataContext.GetSqliteString(aggregateDataFileLocation))
                .Options;
            var context = new AggregateDataContext(sqliteOption);

            _dataPointRepository = new SqliteImportedDataPointRepository(_applicationSettings);
            _aggregateDataRepository = new AggregateDataRepository(context, _dataPointRepository);

        }

        [TearDown]
        public void TearDown()
        {
            if (_aggregateDataRepository != null)
                _aggregateDataRepository.Dispose();
            var folderLocation = _applicationSettings.SqliteStorageFolderLocation;
            foreach (var file in Directory.GetFiles(folderLocation))
            {
                File.Delete(file);
            }
            var alreadyImportedFolderLocation = _applicationSettings.CompletedImportSqliteStorageFolderLocation;
            foreach (var file in Directory.GetFiles(alreadyImportedFolderLocation))
            {
                File.Delete(file);
            }
        }


        [Test]
        public void Can_write_to_aggregate_data_repository() {

            var dataPoints = TestDataGenerator.GetTestData();
            var dataPointsWritten = _aggregateDataRepository.AddDataPoints(dataPoints);

            var isDatabaseWrittenTo = dataPointsWritten.All(m => m.TimeStamp != default(DateTime));
            Assert.That(isDatabaseWrittenTo);
        }

        [Test]
        public void Can_read_from_aggregate_data_repository() {

            var dataPoints = TestDataGenerator.GetTestData();
            var dataPointsWritten = _aggregateDataRepository.AddDataPoints(dataPoints);

            var dataPointsRead = _aggregateDataRepository.GetAllDataPoints();
            Assert.That(dataPointsRead, Is.Not.Empty);
        }

        [Test, Ignore("Takes too long to run and generates a weeks worth of fake data")]
        public void Can_import_data()
        {
            var weeksWorthOfDataCount = _dataPointRepository.GenerateWeeksWorthOfData(_applicationSettings);

            _aggregateDataRepository.ImportFolder(_applicationSettings.SqliteStorageFolderLocation);

            var totalRecordCount = _aggregateDataRepository.GetAllDataPoints().Count();
            Assert.That(totalRecordCount, Is.EqualTo(weeksWorthOfDataCount));
        }


    }
}
