using Microsoft.AspNetCore.Mvc;
using DataStorageService.Endpoints.DataStorage.AggregateData;

namespace DataStorageService.Endpoints
{
    // Helper class to take care of db context injection.
    public class InjectedController : ControllerBase
    {
        protected readonly AggregateDataContext _db;

        public InjectedController(AggregateDataContext context)
        {
            _db = context;
        }
    }
}
