using System;
using System.IO;
using NUnit.Framework;
using DataStorageService.Helpers;
using DataStorageServiceTests.TestData;
using DataStorageService.Endpoints.DataStorage;
using DataStorageService.AppSettings;


namespace DataStorageServiceTests.Endpoints.DataStorage
{
    [TestFixture]
    public class DataStorageControllerTests
    {
        private readonly IApplicationSettings _appSettings;
        private string currentTestSqliteFile;

        public DataStorageControllerTests()
        {
            _appSettings = new TestApplicationSettings();
        }
        [TearDown]
        public void TearDown() {
            try {
                File.Delete(currentTestSqliteFile);
                File.Delete($"{currentTestSqliteFile.Replace(".","_")}_metadata.json");
                Directory.Delete(_appSettings.SqliteStorageFolderLocation, false);
            } catch {
                //if it fails it's nbd, each file is based on a time stamp
            }
        }

        [Test]
        public void Can_save_sqlite_file_to_solution() {
            var fileName = GetExpectedFileName();
            var sqliteFolderLocation = _appSettings.SqliteStorageFolderLocation;
            currentTestSqliteFile = $"{sqliteFolderLocation}/{fileName}";

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
            var controller = new DataStorageController(_appSettings);

            controller.StoreFile(requestParameters);

            Assert.That(currentTestSqliteFile, Does.Exist);
        }

        [Test]
        public void Can_save_sqlite_file_without_losing_any_bytes()
        {
            var fileName = GetExpectedFileName();
            var sqliteFolderLocation = _appSettings.SqliteStorageFolderLocation;
            currentTestSqliteFile = $"{sqliteFolderLocation}/{fileName}";

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
            var controller = new DataStorageController(_appSettings);

            controller.StoreFile(requestParameters);



            Assert.That(transferredDbAsBase64string, 
                        Is.EqualTo(GetFileAsBase64String(currentTestSqliteFile)));
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
            return GetFileAsBase64String($"{solutionRoot}/DataStorageServiceTests/TestData/Chinook.db");
        }

        private string GetFileAsBase64String(string filePath) {
            Byte[] bytes = File.ReadAllBytes(filePath);
            return Convert.ToBase64String(bytes);
        }
    }
}


