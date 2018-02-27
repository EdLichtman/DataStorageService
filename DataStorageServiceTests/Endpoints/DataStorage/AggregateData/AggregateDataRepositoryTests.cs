using System;
using NUnit.Framework;
using DataStorageService.Endpoints.DataStorage.AggregateData;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace DataStorageServiceTests.Endpoints.DataStorage.AggregateData
{
    [TestFixture]
    public class AggregateDataRepositoryTests
    {
        private readonly IAggregateDataRepository _aggregateDataRepository;
        public AggregateDataRepositoryTests()
        {
            var inMemoryDbOption = new DbContextOptionsBuilder<AggregateDataContext>()
                .UseInMemoryDatabase(databaseName: "Add_writes_to_database")
                .Options;
            var context = new AggregateDataContext(inMemoryDbOption);

            _aggregateDataRepository = new AggregateDataRepository(context);
        }
        
        [SetUp]
        public void Setup()
        {
            
        }

        [TearDown]
        public void TearDown()
        {

        }

        [Test]
        public void Can_write_to_aggregate_data_repository() {

            var dataPoints = GetTestDataPoints();
            var dataPointsWritten = _aggregateDataRepository.AddDataPoints(dataPoints);

            var isDatabaseWrittenTo = dataPointsWritten.All(m => m.Id != 0);
            Assert.That(isDatabaseWrittenTo);
        }

        private IList<AggregateDataPoint> GetTestDataPoints()
        {
            return new List<AggregateDataPoint>
            {
                new AggregateDataPoint
                {
                    TimeStamp = DateTime.Now,
                    RawIntensity = 1,
                    RoomNumber = "F1234",
                    RackIdentifier = 2,
                    RackCoordinateX = 3,
                    RackCoordinateY = 4,
                    ConversionKey = 1.4
                }
            };
        }

        [Test]
        public void Can_read_from_aggregate_data_repository() {

            var dataPoints = GetTestDataPoints();
            var dataPointsWritten = _aggregateDataRepository.AddDataPoints(dataPoints);

            var dataPointsRead = _aggregateDataRepository.GetAllDataPoints();
            Assert.That(dataPointsRead, Is.Not.Empty);
        }
    }
}
