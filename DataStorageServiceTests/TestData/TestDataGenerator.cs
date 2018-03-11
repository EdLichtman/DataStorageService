using System;
using DataStorageService.Endpoints.DataStorage.AggregateData;
using System.Collections.Generic;
using DataStorageService.AppSettings;
using DataStorageService.Endpoints.DataStorage;
using Newtonsoft.Json;
using System.IO;
using DataStorageService.Endpoints.DataStorage.DatabaseInterfaces;
using DataStorageService.Helpers;

namespace DataStorageServiceTests.TestData
{
    public static class TestDataGenerator
    {
        public static IList<AggregateDataPoint> GetTestData() {
                return new List<AggregateDataPoint>
            {
                new AggregateDataPoint
                {
                    TimeStamp = DateTime.Now.AddMinutes(20),
                    RawIntensity = 1,
                    RoomNumber = "F1234",
                    RackIdentifier = 2,
                    RackCoordinateX = 3,
                    RackCoordinateY = 4,
                    ConversionKey = 1.4
                },
                new AggregateDataPoint
                {
                    TimeStamp = DateTime.Now.AddMinutes(15),
                    RawIntensity = 5,
                    RoomNumber = "F1234",
                    RackIdentifier = 2,
                    RackCoordinateX = 3,
                    RackCoordinateY = 4,
                    ConversionKey = 1.4
                },
                new AggregateDataPoint
                {
                    TimeStamp = DateTime.Now.AddMinutes(10),
                    RawIntensity = 10,
                    RoomNumber = "F1234",
                    RackIdentifier = 2,
                    RackCoordinateX = 3,
                    RackCoordinateY = 4,
                    ConversionKey = 1.4
                },
                new AggregateDataPoint
                {
                    TimeStamp = DateTime.Now.AddMinutes(5),
                    RawIntensity = 15,
                    RoomNumber = "F1234",
                    RackIdentifier = 2,
                    RackCoordinateX = 3,
                    RackCoordinateY = 4,
                    ConversionKey = 1.4
                }
            };
        }
        public static int GenerateWeeksWorthOfData(this IImportedDataPointRepository dataPointRepository, IApplicationSettings applicationSettings)
        {
            var totalCount = 0;
            var typesOfSavedData = 1;
            var daysInWeek = 7;
            var hoursInDay = 24;
            var minutesInHour = 60;
            var readingsInMinute = 12;
            var oneWeekOfData = daysInWeek * hoursInDay;
            var readingsPerHour = minutesInHour * readingsInMinute;

            var sqliteFolderLocation = applicationSettings.SqliteStorageFolderLocation;

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
                    dataPointRepository.WriteRangeToDatabase(databaseName, fileTypeData);
                    File.WriteAllText(Path.Combine(sqliteFolderLocation, databaseName.GetSqliteAssociatedMetadataFileName()), json);

                }
            }
            return totalCount;
        }
    }
}
