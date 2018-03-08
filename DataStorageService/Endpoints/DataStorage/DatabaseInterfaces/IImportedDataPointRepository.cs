using System;
using System.Collections.Generic;
using DataStorageService.Endpoints.DataStorage;

namespace DataStorageService.Endpoints.DataStorage.DatabaseInterfaces
{
    public interface IImportedDataPointRepository
    {
        IList<ImportedDataPoint> ReadFromDatabase(string fileName);
        //bool WriteToDatabase(ImportedDataPoint dataPoint);
        bool WriteRangeToDatabase(string fileName, IList<ImportedDataPoint> dataPoints);
    }
}
