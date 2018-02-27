using System;
using System.ComponentModel.DataAnnotations;

namespace DataStorageService.Endpoints.DataStorage
{
    public class ImportedDataPoint
    {
        [Required]
        public DateTime TimeStamp { get; set; }

        [Required]
        public int RawIntensity { get; set; }
    }
}
