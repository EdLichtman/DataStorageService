﻿using System;
using System.Collections.Generic;
namespace DataStorageService.Endpoints.DataStorage.AggregateData
{

    public interface IAggregateDataRepository
    {
        IList<AggregateDataPoint> AddDataPoints(IList<AggregateDataPoint> dataPoints);

        IList<AggregateDataPoint> GetAllDataPoints();

        IList<AggregateDataPoint> ImportFolder(string folderLocation);
        void Dispose();
    }
}
