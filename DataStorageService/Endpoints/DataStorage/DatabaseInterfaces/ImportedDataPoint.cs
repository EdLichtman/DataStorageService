using System;
using System.ComponentModel.DataAnnotations;

namespace DataStorageService.Endpoints.DataStorage.DatabaseInterfaces
{
    public class ImportedDataPoint
    {
        [Required]
        public DateTime TimeStampInUtc { get; set; }

        [Required]
        public int Bits { get; set; }
    }
}
