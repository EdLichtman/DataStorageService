using System;
using System.Linq;
using System.Collections.Generic;

namespace DataStorageService.Endpoints.DataStorage.AggregateData
{
    public class AggregateDataRepository : IAggregateDataRepository
    {
        private readonly AggregateDataContext _database;
        public AggregateDataRepository(AggregateDataContext database)
        {
            _database = database;
        }
        public IList<AggregateDataPoint> AddDataPoints(IList<AggregateDataPoint> dataPoints) {
            var trackedDataPoints = new List<AggregateDataPoint>();
            foreach(var dataPoint in dataPoints) {
                trackedDataPoints.Add(_database.Add(dataPoint).Entity);
            }
            _database.SaveChanges();
            return trackedDataPoints.ToList();
        }

        public IList<AggregateDataPoint> GetAllDataPoints() {

            return _database.AggregateDataPoints.ToList();
        }
    }
}
