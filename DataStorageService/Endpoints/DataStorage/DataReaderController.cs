using Microsoft.AspNetCore.Mvc;
using System.Linq;
using DataStorageService.Endpoints.DataStorage.AggregateData;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DataStorageService.Endpoints
{
    [Route("api/[controller]/[action]")]
    public class DataReaderController : InjectedController
    {
        public DataReaderController(AggregateDataContext dataContext) : base(dataContext) { }

        [HttpGet]
        public IActionResult GetDataPoints()
        {
            var payload = _db.AggregateDataPoints.ToList();
            return Ok(payload);
        }

        [HttpPost]
        public async Task<IActionResult> AddDataPoints([FromBody]IList<AggregateDataPoint> dataPoints)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            await _db.AddRangeAsync(dataPoints);
            await _db.SaveChangesAsync();
            return Ok();
        }
    }
}
