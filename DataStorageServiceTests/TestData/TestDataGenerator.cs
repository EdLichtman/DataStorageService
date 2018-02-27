using System;
using DataStorageService.Endpoints.DataStorage.AggregateData;
using System.Collections.Generic;
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
    }
}
