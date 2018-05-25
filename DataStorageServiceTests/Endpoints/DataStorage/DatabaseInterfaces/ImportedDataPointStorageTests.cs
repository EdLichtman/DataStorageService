using System;
using System.IO;
using NUnit.Framework;
using DataStorageService.Endpoints.DataStorage;
using DataStorageService.Endpoints.DataStorage.DatabaseInterfaces;
using System.Collections.Generic;
using DataStorageServiceTests.TestData;
using DataStorageService.AppSettings;
using System.Linq;
using DataStorageService.Helpers;

namespace DataStorageServiceTests.Endpoints.DataStorage.DatabaseInterfaces
{
    [TestFixture]
    public class ImportedDataPointStorageTests
    {
        private IApplicationSettings _applicationSettings;
        private IImportedDataPointRepository _importedDataPointStorage;
        private string currentTestSqliteFile;


        [SetUp]
        public void Setup() {
            
            _applicationSettings = new TestApplicationSettings();
            _importedDataPointStorage = new SqliteImportedDataPointRepository(_applicationSettings);

        }
        [TearDown]
        public void TearDown() {
            if (!string.IsNullOrWhiteSpace(currentTestSqliteFile))
                File.Delete(currentTestSqliteFile);
        }

        [Test]
        public void Can_create_Sqlite_database(){
            var fileName = GetExpectedFileName();
            var sqliteFolderLocation = _applicationSettings.SqliteStorageFolderLocation;
            currentTestSqliteFile = Path.Combine(sqliteFolderLocation,fileName);
            TearDown();
            var rawDataSet = GetTestData();

            _importedDataPointStorage.WriteRangeToDatabase(fileName, rawDataSet);
            Assert.That(currentTestSqliteFile, Does.Exist);
        } 

        [Test]
        public void Can_write_to_Sqlite_database(){
            var fileName = GetExpectedFileName();
            var sqliteFolderLocation = _applicationSettings.SqliteStorageFolderLocation;
            currentTestSqliteFile = Path.Combine(sqliteFolderLocation,fileName);
            TearDown();
            var rawDataSet = GetTestData();

            _importedDataPointStorage.WriteRangeToDatabase(fileName, rawDataSet);
            Assert.That(new FileInfo(currentTestSqliteFile).Length, Is.GreaterThan(0));
        } 

        [Test]
        public void Can_read_total_rows_from_Sqlite_database() {
            var fileName = GetExpectedFileName();
            var sqliteFolderLocation = _applicationSettings.SqliteStorageFolderLocation;
            currentTestSqliteFile = Path.Combine(sqliteFolderLocation,fileName);
            TearDown();
            var rawDataSet = GetTestData();

            _importedDataPointStorage.WriteRangeToDatabase(fileName, rawDataSet);
            var importedData = _importedDataPointStorage.ReadFromDatabase(sqliteFolderLocation, fileName);
            Assert.That(importedData.Count, Is.EqualTo(rawDataSet.Count));
        }
        [Test]
        public void Can_get_data_from_Sqlite_database() {
            var fileName = GetExpectedFileName();
            var sqliteFolderLocation = _applicationSettings.SqliteStorageFolderLocation;
            currentTestSqliteFile = Path.Combine(sqliteFolderLocation,fileName);
            TearDown();
            var rawDataSet = GetTestData();

            _importedDataPointStorage.WriteRangeToDatabase(fileName, rawDataSet);
            var importedData = _importedDataPointStorage.ReadFromDatabase(sqliteFolderLocation, fileName);
            Assert.That(importedData.FirstOrDefault(m => m.Bits == 1).TimeStampInUtc
                        , Is.EqualTo(rawDataSet.FirstOrDefault(m => m.Bits == 1).TimeStampInUtc));
        }

        private IList<ImportedDataPoint> GetTestData() {
            return new List<ImportedDataPoint> {
                new ImportedDataPoint {
                    Bits = 1,
                    TimeStampInUtc = new DateTime(DateTime.Now.AddDays(-1).Ticks).Truncate(TimeSpan.TicksPerSecond)
                },
                new ImportedDataPoint {
                    Bits = 2,
                    TimeStampInUtc = new DateTime(DateTime.Now.AddDays(-1).AddSeconds(5).Ticks).Truncate(TimeSpan.TicksPerSecond)
                },
                new ImportedDataPoint {
                    Bits = 3,
                    TimeStampInUtc = new DateTime(DateTime.Now.AddDays(-1).AddSeconds(10).Ticks).Truncate(TimeSpan.TicksPerSecond)
                }
            };
        }
        private string GetExpectedFileName()
        {
            var now = DateTime.UtcNow;
            var year = now.Year.ToString();
            var month = now.Month.ToString().PadLeft(2, '0');
            var day = now.Day.ToString().PadLeft(2, '0');
            var hour = now.Hour.ToString().PadLeft(2, '0');
            return $"Vibration_BaseLine_{year}{month}{day}{hour}.db";
        }
    }
}
