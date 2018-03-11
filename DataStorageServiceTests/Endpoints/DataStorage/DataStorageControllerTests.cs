using System;
using System.IO;
using NUnit.Framework;
using DataStorageService.Helpers;
using DataStorageServiceTests.TestData;
using DataStorageService.Endpoints.DataStorage;
using DataStorageService.Endpoints.DataStorage.DatabaseInterfaces;
using DataStorageService.AppSettings;
using Microsoft.EntityFrameworkCore;
using DataStorageService.Endpoints.DataStorage.AggregateData;


namespace DataStorageServiceTests.Endpoints.DataStorage
{
    [TestFixture]
    public class DataStorageControllerTests
    {
        private readonly IApplicationSettings _appSettings;
        private readonly IImportedDataPointRepository _dataPointRepository;
        private readonly DataStorageController _dataStorageController;

        public DataStorageControllerTests()
        {
            _appSettings = new TestApplicationSettings();
            _dataPointRepository = new SqliteImportedDataPointRepository(_appSettings);

            var aggregateDataFileLocation = Path.Combine(_appSettings.SqliteStorageFolderLocation, _appSettings.AggregateSqliteFileName);
            File.Create(aggregateDataFileLocation);

            var sqliteOption = new DbContextOptionsBuilder<AggregateDataContext>()
                .UseSqlite(AggregateDataContext.GetSqliteString(aggregateDataFileLocation))
                .Options;
            var context = new AggregateDataContext(sqliteOption);
            var aggregateDataRepository = new AggregateDataRepository(context, _dataPointRepository);

            _dataStorageController = new DataStorageController(_appSettings, aggregateDataRepository);
        }
        [TearDown]
        public void TearDown() {
            try {
                var folderLocation = _appSettings.SqliteStorageFolderLocation;
                foreach (var file in Directory.GetFiles(folderLocation))
                {
                    File.Delete(file);
                }
                var alreadyImportedFolderLocation = _appSettings.CompletedImportSqliteStorageFolderLocation;
                foreach (var file in Directory.GetFiles(alreadyImportedFolderLocation))
                {
                    File.Delete(file);
                }
            } catch {
                //if it fails it's nbd, each file is based on a time stamp
            }
        }

        [Test]
        public void Can_save_sqlite_file_to_solution() {
            var fileName = GetExpectedFileName();
            var sqliteFolderLocation = _appSettings.SqliteStorageFolderLocation;
            var currentTestSqliteFile = Path.Combine(sqliteFolderLocation,fileName);

            var requestParameters = new StoreFileRequest
            {
                Metadata = new StoreFileMetadata
                {
                    FileName = fileName,
                    RoomNumber = "D73234",
                    RackIdentifier = 1,
                    RackCoordinates = new RackCoordinate
                    {
                        X = 1,
                        Y = 1
                    },
                    ConversionKey = "1",
                },

                SqliteDataAsBase64 = GetSqliteAsBase64String()
            };

            _dataStorageController.StoreFile(requestParameters);

            Assert.That(currentTestSqliteFile, Does.Exist);
        }

        [Test]
        public void Can_save_sqlite_file_without_losing_any_bytes()
        {
            var fileName = GetExpectedFileName();
            var sqliteFolderLocation = _appSettings.SqliteStorageFolderLocation;
            var currentTestSqliteFile = Path.Combine(sqliteFolderLocation,fileName);

            var transferredDbAsBase64string = GetSqliteAsBase64String();
            var requestParameters = new StoreFileRequest
            {
                Metadata = new StoreFileMetadata
                {
                    FileName = fileName,
                    RoomNumber = "D73234",
                    RackIdentifier = 1,
                    RackCoordinates = new RackCoordinate
                    {
                        X = 1,
                        Y = 1
                    },
                    ConversionKey = "1",
                },

                SqliteDataAsBase64 = transferredDbAsBase64string
            };

            _dataStorageController.StoreFile(requestParameters);

            Assert.That(transferredDbAsBase64string, 
                        Is.EqualTo(GetFileAsBase64String(currentTestSqliteFile)));
        }

        [Test]
        public void Can_import_data() {
            var expectedGeneratedRows = _dataPointRepository.GenerateWeeksWorthOfData(_appSettings);

            var totalGeneratedRows = _dataStorageController.AggregateResults();
            Assert.That(totalGeneratedRows, Is.EqualTo(expectedGeneratedRows));
        }
       
        private string GetExpectedFileName() {
            var now = DateTime.UtcNow;
            var year = now.Year.ToString();
            var month = now.Month.ToString().PadLeft(2, '0');
            var day = now.Day.ToString().PadLeft(2, '0');
            var hour = now.Hour.ToString().PadLeft(2, '0');
            return $"Vibration_BaseLine_{year}{month}{day}{hour}.db";
        }



        private string GetSqliteAsBase64String() {
            var solutionRoot = DirectoryHelpers.GetSolutionRoot();
            return GetFileAsBase64String(Path.Combine(solutionRoot,"DataStorageServiceTests","TestData","Chinook.db"));
        }

        private string GetFileAsBase64String(string filePath) {
            Byte[] bytes = File.ReadAllBytes(filePath);
            return Convert.ToBase64String(bytes);
        }
    }
}


