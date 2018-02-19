using System;
namespace DataStorageService.Endpoints.DataStorage
{
    public class StoreFileRequest
    {
        public string SqliteDataAsBase64 { get; set; }
        public StoreFileMetadata Metadata { get; set; }
    }
}
