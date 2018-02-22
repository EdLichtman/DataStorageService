using System;
namespace DataStorageService.Endpoints.DataStorage
{
    public class StoreFileMetadata
    {
        public string FileName { get; set; }
        public string RoomNumber { get; set; }
        public int RackIdentifier {get;set;}
        public RackCoordinate RackCoordinates { get; set; }
        public string ConversionKey { get; set; }
    }
}
