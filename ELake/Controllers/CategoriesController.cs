using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;

namespace ELake.Controllers
{
    public class CategoriesController : Controller
    {
        private IHostingEnvironment _hostingEnvironment;

        public CategoriesController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            //ViewBag.Categories = "[{\"id\":\"j1_1\",\"text\":\"Root node\",\"icon\":true,\"li_attr\":{\"id\":\"j1_1\"},\"a_attr\":{\"href\":\"#\",\"id\":\"j1_1_anchor\"},\"state\":{\"loaded\":true,\"opened\":true,\"selected\":false,\"disabled\":false},\"data\":[{\"textKK\":\"Түбір торабы\",\"textRU\":\"Корневой узел\",\"textEN\":\"Root node\"}],\"parent\":\"#\"},{\"id\":\"j1_2\",\"text\":\"Child 1\",\"icon\":true,\"li_attr\":{\"id\":\"j1_2\"},\"a_attr\":{\"href\":\"#\",\"id\":\"j1_2_anchor\"},\"state\":{\"loaded\":true,\"opened\":false,\"selected\":false,\"disabled\":false},\"data\":[{\"textKK\":\"Бала 1\",\"textRU\":\"Ребенок 1\",\"textEN\":\"Child 1\"}],\"parent\":\"j1_1\"}]";
            ViewBag.Categories = System.IO.File.ReadAllText(Path.Combine(_hostingEnvironment.WebRootPath, "JSON", "Categories.json"));
            return View();
        }

        [HttpPost]
        public JsonResult SaveCategories(string Json)
        {
            System.IO.File.WriteAllText(Path.Combine(_hostingEnvironment.WebRootPath, "JSON", "Categories.json"), Json);
            JsonResult result = new JsonResult("OK");
            return result;
        }
    }
}