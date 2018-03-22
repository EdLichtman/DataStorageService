using System;
using System.ComponentModel.DataAnnotations;

namespace DataStorageService.Endpoints.DataStorage.DatabaseInterfaces
{
    public class ImportedDataPoint
    {
        [Required]
        public DateTime TimeStamp { get; set; }

        [Required]
        public int RawIntensity { get; set; }
    }
}
