using System;
namespace DataStorageService.Endpoints.DataStorage
{
    public class StoreFileMetadata
    {
        public string FileName { get; set; }
        public string RoomNumber { get; set; }
        public int RackNumber {get;set;}
        public string Position {get;set;}
        public string ConversionKey { get; set; }
    }
}
