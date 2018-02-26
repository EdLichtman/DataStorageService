using System;
using System.Collections.Generic;
namespace DataStorageService.Endpoints.DataStorage.AggregateData
{
    
    public interface IAggregateDataRepository
    {
        IList<AggregateDataPoint> AddDataPoints(IList<AggregateDataPoint> dataPoints);
    }
}
