using System.Collections.Generic;

namespace DataStorageService.Endpoints.DataStorage.DatabaseInterfaces
{
    public interface IImportedDataPointRepository
    {
        IList<ImportedDataPoint> ReadFromDatabase(string fileName);
        
        bool WriteRangeToDatabase(string fileName, IList<ImportedDataPoint> dataPoints);
    }
}
