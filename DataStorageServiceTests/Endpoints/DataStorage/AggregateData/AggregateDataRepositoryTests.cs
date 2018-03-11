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
        private readonly IImportedDataPointRepository _dataPointRepository;
        private readonly IAggregateDataRepository _aggregateDataRepository;
        private readonly IApplicationSettings _applicationSettings;


        public AggregateDataRepositoryTests()
        {
            _applicationSettings = new TestApplicationSettings();
            TearDown();
            var fileLocation = Path.Combine(_applicationSettings.SqliteStorageFolderLocation,_applicationSettings.AggregateSqliteFileName);
            File.Create(fileLocation);

            var sqliteOption = new DbContextOptionsBuilder<AggregateDataContext>()
                .UseSqlite(AggregateDataContext.GetSqliteString(fileLocation))
                .Options;

            var context = new AggregateDataContext(sqliteOption);
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
            context.Database.Migrate();


            _dataPointRepository = new SqliteImportedDataPointRepository(_applicationSettings);
            _aggregateDataRepository = new AggregateDataRepository(context, _dataPointRepository);

        }

        [TearDown]
        public void TearDown()
        {
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

        [Test]
        public void Can_import_data()
        {
            var weeksWorthOfDataCount = WriteWeeksWorthOfData();

            _aggregateDataRepository.ImportFolder(_applicationSettings.SqliteStorageFolderLocation, _applicationSettings.CompletedImportSqliteStorageFolderLocation);

            var totalRecordCount = _aggregateDataRepository.GetAllDataPoints().Count();
            Assert.That(totalRecordCount, Is.EqualTo(weeksWorthOfDataCount));
        }

        private int WriteWeeksWorthOfData()
        {
            var totalCount = 0;
            var typesOfSavedData = 1;
            var daysInWeek = 1;
            var hoursInDay = 1;
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
                    File.WriteAllText(Path.Combine(sqliteFolderLocation,databaseName.GetSqliteAssociatedMetadataFileName()), json);

                }
            }
            return totalCount;
        }
    }
}
