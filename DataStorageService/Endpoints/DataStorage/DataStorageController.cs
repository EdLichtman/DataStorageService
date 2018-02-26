using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using DataStorageService.AppSettings;
using Newtonsoft.Json;
using System.Net;

namespace DataStorageService.Endpoints.DataStorage
{
    [Route("api/[controller]/[action]")]
    public class DataStorageController : Controller
    {
        const int UnprocessableEntityHttpStatusCode = 422;
        private readonly string _sqliteStorageFolder;
        private readonly IApplicationSettings _appSettings;
        public DataStorageController(IApplicationSettings appSettings)
        {
            _appSettings = appSettings;
            _sqliteStorageFolder = _appSettings.SqliteStorageFolderLocation;
            Directory.CreateDirectory(_sqliteStorageFolder);

        }
        [HttpGet]
        public object Ping()
        {
            Response.StatusCode = (int)HttpStatusCode.OK;
            return new
            {
                Status = "OK"
            };
        }
        [HttpPost]
        public object StoreFile([FromBody]StoreFileRequest requestParameters)
        {
            
            var isDataSecure = WriteDataToFile(requestParameters);
            var isMetaDataSecure = WriteMetadataToFile(requestParameters.Metadata);

            int responseStatus;

            if (isDataSecure && isMetaDataSecure)
                responseStatus = (int)HttpStatusCode.OK;
            else
                responseStatus = UnprocessableEntityHttpStatusCode;

            if (Response != null)
                Response.StatusCode = responseStatus;
            return new
            {
                Result = "Completed"
            };
        }

        private bool WriteDataToFile(StoreFileRequest requestParameters) {

            var fileLocation = $"{_appSettings.SqliteStorageFolderLocation}/{requestParameters.Metadata.FileName}";
            try {
                var filePath = System.IO.File.Create(fileLocation);
                var fileWriter = new BinaryWriter(filePath);
                var bytes = Convert.FromBase64String(requestParameters.SqliteDataAsBase64);
                fileWriter.Write(bytes, 0, bytes.Length);
                fileWriter.Dispose();
            } catch {
                System.IO.File.Delete(fileLocation);
                return false;
            }
            return true;
        }
        private bool WriteMetadataToFile(StoreFileMetadata metadata) {
            var fileLocation = $"{_appSettings.SqliteStorageFolderLocation}/{metadata.FileName}.metadata.json";
            try
            {
                var filePath = System.IO.File.Create(fileLocation);
                var fileWriter = new StreamWriter(filePath);
                fileWriter.Write(JsonConvert.SerializeObject(metadata));
                fileWriter.Dispose();
            } catch {
                System.IO.File.Delete(fileLocation);
                return false;
            }
            return true;
        }

        // PUT api/values/5
        [HttpPut]
        public void ImportToDatabase(int id, [FromBody]string value)
        {
        }

    }
}
