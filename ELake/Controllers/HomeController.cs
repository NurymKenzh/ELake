using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ELake.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Http;

namespace ELake.Controllers
{
    public class HomeController : Controller
    {
        private readonly GeoServerController _GeoServer;

        public HomeController(GeoServerController GeoServer)
        {
            _GeoServer = GeoServer;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            try
            {
                //ViewData["Message"] = $"GeoServer create workspace {Startup.Configuration["GeoServer:Workspace"]}";
                //_GeoServer.CreateWorkspace(Startup.Configuration["GeoServer:Workspace"]);

                //ViewData["Message"] = $"GeoServer publish GeoTIFF \"T43TGH_20180423T053639_TCI1.tif\"";
                //_GeoServer.PublishGeoTIFF(Startup.Configuration["GeoServer:Workspace"], "T43TGH_20180423T053639_TCI1.tif", "raster");

                //ViewData["Message"] = $"GeoServer unpublish GeoTIFF \"T43TGH_20180423T053639_TCI1\"";
                //_GeoServer.UnpublishGeoTIFF(Startup.Configuration["GeoServer:Workspace"], "T43TGH_20180423T053639_TCI1");

                //ViewData["Message"] = string.Join("<br />", _GeoServer.GetWorkspaceStyles(Startup.Configuration["GeoServer:Workspace"]));

                //ViewData["Message"] = string.Join("<br />", _GeoServer.GetGeoTIFFFiles(Startup.Configuration["GeoServer:Workspace"]));

                //ViewData["Message"] = string.Join("; ", _GeoServer.GetGeoTIFFFileLayers(Startup.Configuration["GeoServer:Workspace"], @"D:\Documents\Google Drive\GeoServer 2.11.1\data_dir\data\ELake\T43TGH_20180423T053639_TCI1.tif"));

                //_GeoServer.DeleteGeoTIFFFile(
                //    Startup.Configuration["GeoServer:Workspace"],
                //    @"D:\Documents\Google Drive\GeoServer 2.11.1\data_dir\data\ELake\T43TGH_20180423T053639_TCI1.tif");

                //_GeoServer.PublishShape(Startup.Configuration["GeoServer:Workspace"], "Lakes.shp", "line");

                //_GeoServer.MoveShapeFilesToDirectories(Startup.Configuration["GeoServer:Workspace"]);
            }
            catch (Exception exception)
            {
                ViewData["Message"] = $"{exception.ToString()}. {exception.InnerException?.Message}";
            }

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public IActionResult SetLanguage(string culture, string returnUrl)
        {
            Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

            return LocalRedirect(returnUrl);
        }

        [Authorize(Roles = "Administrator")]
        public IActionResult Administrator()
        {
            return View();
        }
    }
}
