﻿using System;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using DataStorageService.AppSettings;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http;
using DataStorageService.Helpers;
using DataStorageService.Endpoints.DataStorage.AggregateData;
using DataStorageService.Features.EmailClient;

namespace DataStorageService.Endpoints.DataStorage
{
    [Route("api/[controller]/[action]")]
    public class DataStorageController : Controller
    {
        const int UnprocessableEntityHttpStatusCode = 422;
        private readonly string _sqliteStorageFolder;
        private readonly IApplicationSettings _appSettings;
        private readonly IAggregateDataRepository _aggregateDataRepository;
        private readonly IEmailServiceLayer _emailServiceLayer;

        public DataStorageController(
            IApplicationSettings appSettings, 
            IAggregateDataRepository aggregateDataRepository, 
            IEmailServiceLayer emailServiceLayer)
        {
            _appSettings = appSettings;
            _aggregateDataRepository = aggregateDataRepository;
            _emailServiceLayer = emailServiceLayer;
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

            var fileLocation = Path.Combine(_appSettings.SqliteStorageFolderLocation,requestParameters.Metadata.FileName);
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
            var fileLocation = Path.Combine(_appSettings.SqliteStorageFolderLocation,metadata.FileName.GetSqliteAssociatedMetadataFileName());
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


        [HttpPost]
        public int AggregateResults(string importLocation, string fileName = "AggregateData.db")
        {
            if (importLocation == null)
                importLocation = _appSettings.SqliteStorageFolderLocation;
            var results = _aggregateDataRepository.ImportFolder(importLocation);
            var emailFileRequest = new EmailFileRequest
            {
                Name = fileName,
                Location = AggregateDataContext.GetDbLocation(Path.Combine(_sqliteStorageFolder, "AggregateData.db"))
            };
            _emailServiceLayer.SendFile("", emailFileRequest, "");
            return results.Count;
        }

        [HttpGet]
        public void MockSqliteFile() {
            
        }

    }

    public class EmailFileRequest
    {
        public string Name { get; set; }
        public string Location { get; set; }
    }
}
