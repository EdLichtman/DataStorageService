using System;
using System.ComponentModel.DataAnnotations;
namespace DataStorageService.Endpoints.DataStorage.AggregateData
{
    public class AggregateDataPoint
    {
        
        public int Id { get; set; }
        [Required]
        public DateTime TimeStamp { get; set; }

        [Required]
        public int RawIntensity { get; set; }

        [Required]
        public string RoomNumber { get; set; }

        [Required]
        public int RackIdentifier { get; set; }

        [Required]
        public int RackCoordinateX { get; set; }

        [Required]
        public int RackCoordinateY { get; set; }

        [Required]
        public double ConversionKey { get; set; }
    }
}
