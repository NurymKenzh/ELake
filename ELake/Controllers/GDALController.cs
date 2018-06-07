using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ELake.Controllers
{
    public class GDALController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}