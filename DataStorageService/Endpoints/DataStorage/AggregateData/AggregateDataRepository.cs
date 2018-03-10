using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using DataStorageService.Helpers;
using DataStorageService.Endpoints.DataStorage.DatabaseInterfaces;
using Newtonsoft.Json;

namespace DataStorageService.Endpoints.DataStorage.AggregateData
{
    public class AggregateDataRepository : IAggregateDataRepository
    {
        private readonly AggregateDataContext _database;
        private readonly IImportedDataPointRepository _importedDataPointRepository;

        public AggregateDataRepository(AggregateDataContext database, 
                                       IImportedDataPointRepository importedDataPointRepository)
        {
            _database = database;
            _importedDataPointRepository = importedDataPointRepository;
        }
        public IList<AggregateDataPoint> AddDataPoints(IList<AggregateDataPoint> dataPoints) {
            var trackedDataPoints = new List<AggregateDataPoint>();
            foreach(var dataPoint in dataPoints) {
                trackedDataPoints.Add(_database.Add(dataPoint).Entity);
            }
            _database.SaveChanges();
            return trackedDataPoints;
        }

        public IList<AggregateDataPoint> GetAllDataPoints() {

            return _database.AggregateDataPoints.ToList();
        }

        public IList<AggregateDataPoint> ImportFolder(string folderLocation) {
            var files = Directory.GetFiles(folderLocation).Select(file => file.Replace(folderLocation + "/", "")).ToList();
            var dbFiles = files.Where(file => file.EndsWith(".db")).ToList();
            foreach(var dbFile in dbFiles) {
   
                try {
                    var importedData = _importedDataPointRepository.ReadFromDatabase(dbFile);
                    var metadataFileName = dbFile.GetSqliteAssociatedMetadataFileName();
                    StoreFileMetadata associatedMetaData;
                    using (var fileReader = new StreamReader($"{folderLocation}/{metadataFileName}"))  {
                        associatedMetaData = JsonConvert.DeserializeObject<StoreFileMetadata>(fileReader.ReadToEnd());
                    }

                    var aggregateDataPoints = importedData.Select(data => new AggregateDataPoint
                    {
                        TimeStamp = data.TimeStamp,
                        RawIntensity = data.RawIntensity,
                        RoomNumber = associatedMetaData.RoomNumber,
                        RackIdentifier = associatedMetaData.RackIdentifier,
                        RackCoordinateX = associatedMetaData.RackCoordinates.X,
                        RackCoordinateY = associatedMetaData.RackCoordinates.Y,
                        ConversionKey = Convert.ToDouble(associatedMetaData.ConversionKey)
                    });
                    AddDataPoints(aggregateDataPoints.ToList());
                } catch(Exception e) {
                    
                }

            }

            return GetAllDataPoints();
        }
    }
}
