using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace DataStorageService.FrontEnd.DataStorage
{
    [Route("[controller]/[action]")]
    public class DataStorageController : Controller
    {
        [HttpGet]
        public IActionResult ImportData()
        {
            return View("ImportData");
        }
    }
}