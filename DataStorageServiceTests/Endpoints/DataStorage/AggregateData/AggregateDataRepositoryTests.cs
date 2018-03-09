using System;
using System.IO;
using NUnit.Framework;
using DataStorageService.Endpoints.DataStorage.AggregateData;
using System.Linq;
using DataStorageServiceTests.TestData;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using DataStorageService.Endpoints.DataStorage.DatabaseInterfaces;
using DataStorageService.AppSettings;
using DataStorageService.Endpoints.DataStorage;
using DataStorageService.Helpers;
using Newtonsoft.Json;

namespace DataStorageServiceTests.Endpoints.DataStorage.AggregateData
{
    [TestFixture]
    public class AggregateDataRepositoryTests
    {
        private IList<string> deletableTestSqliteFiles;
        private readonly IImportedDataPointRepository _dataPointRepository;
        private readonly IAggregateDataRepository _aggregateDataRepository;
        private readonly IApplicationSettings _applicationSettings;

        public AggregateDataRepositoryTests()
        {
            var inMemoryDbOption = new DbContextOptionsBuilder<AggregateDataContext>()
                .UseInMemoryDatabase(databaseName: "Add_writes_to_database")
                .Options;
            var context = new AggregateDataContext(inMemoryDbOption);

            _applicationSettings = new TestApplicationSettings();
            _dataPointRepository = new SqliteImportedDataPointRepository(_applicationSettings);
            _aggregateDataRepository = new AggregateDataRepository(context, _dataPointRepository);

        }
        
        [SetUp]
        public void Setup()
        {
            deletableTestSqliteFiles = new List<string>();   
        }

        [TearDown]
        public void TearDown()
        {
            var folderLocation = _applicationSettings.SqliteStorageFolderLocation;
            foreach (var deletableTestSqliteFile in deletableTestSqliteFiles)
            {
                File.Delete($"{folderLocation}/{deletableTestSqliteFile}");
                File.Delete($"{folderLocation}/{deletableTestSqliteFile.GetSqliteAssociatedMetadataFileName()}");
            }
        }

        [Test]
        public void Can_write_to_aggregate_data_repository() {

            var dataPoints = TestDataGenerator.GetTestData();
            var dataPointsWritten = _aggregateDataRepository.AddDataPoints(dataPoints);

            var isDatabaseWrittenTo = dataPointsWritten.All(m => m.Id != 0);
            Assert.That(isDatabaseWrittenTo);
        }

        [Test]
        public void Can_read_from_aggregate_data_repository() {

            var dataPoints = TestDataGenerator.GetTestData();
            var dataPointsWritten = _aggregateDataRepository.AddDataPoints(dataPoints);

            var dataPointsRead = _aggregateDataRepository.GetAllDataPoints();
            Assert.That(dataPointsRead, Is.Not.Empty);
        }

        [Test]
        public void Can_import_data()
        {
            var weeksWorthOfDataCount = WriteWeeksWorthOfData();

            _aggregateDataRepository.ImportFolder(_applicationSettings.SqliteStorageFolderLocation);

            var totalRecordCount = _aggregateDataRepository.GetAllDataPoints().Count();
            Assert.That(totalRecordCount, Is.EqualTo(weeksWorthOfDataCount));
        }

        private int WriteWeeksWorthOfData()
        {
            var totalCount = 0;
            var typesOfSavedData = 1;
            var daysInWeek = 1;
            var hoursInDay = 24;
            var minutesInHour = 60;
            var readingsInMinute = 12;
            var oneWeekOfData = daysInWeek * hoursInDay;
            var readingsPerHour = minutesInHour * readingsInMinute;

            var sqliteFolderLocation = _applicationSettings.SqliteStorageFolderLocation;

            for (var fileTypeIndex = 0; fileTypeIndex < typesOfSavedData; fileTypeIndex++)
            {
                for (var fileIndex = 0; fileIndex < oneWeekOfData; fileIndex++)
                {
                    var databaseName = $"VibrationType{fileTypeIndex}{fileIndex}.db";
                    var fileTypeData = new List<ImportedDataPoint>();
                    for (var dataPointIndex = 0; dataPointIndex < readingsPerHour; dataPointIndex++)
                    {
                        fileTypeData.Add(new ImportedDataPoint
                        {
                            TimeStamp = DateTime.Now.AddDays(-7).AddMinutes(5 * dataPointIndex),
                            RawIntensity = dataPointIndex
                        });
                        totalCount++;
                    }

                    var metadata = new StoreFileMetadata
                    {
                        FileName = databaseName,
                        RoomNumber = fileIndex.ToString(),
                        RackIdentifier = fileIndex,
                        RackCoordinates = new RackCoordinate
                        {
                            X = fileIndex,
                            Y = fileIndex
                        },
                        ConversionKey = fileIndex.ToString()
                    };
                    var json = JsonConvert.SerializeObject(metadata);
                    _dataPointRepository.WriteRangeToDatabase(databaseName, fileTypeData);
                    deletableTestSqliteFiles.Add(databaseName);
                    File.WriteAllText($"{sqliteFolderLocation}/{databaseName.GetSqliteAssociatedMetadataFileName()}", json);

                }
            }
            return totalCount;
        }
    }
}
