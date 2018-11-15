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
using ELake.Data;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ELake.Controllers
{
    public class HomeController : Controller
    {
        private readonly GeoServerController _GeoServer;
        private readonly GDALController _GDAL;
        private readonly ApplicationDbContext _context;

        public HomeController(GeoServerController GeoServer,
            GDALController GDAL,
            ApplicationDbContext context)
        {
            _GeoServer = GeoServer;
            _GDAL = GDAL;
            _context = context;
        }

        public IActionResult Index()
        {
            //ViewBag.KATO1 = new SelectList(_context.KATO.Where(k => k.Level == 1).OrderBy(k => k.Name), "Number", "Name");
            //ViewBag.KATO2 = new SelectList(_context.KATO.Where(k => k.Level == 2).OrderBy(k => k.Name), "Number", "Name");
            //ViewBag.KATO3 = new SelectList(_context.KATO.Where(k => k.Level == 3).OrderBy(k => k.Name), "Number", "Name");
            //ViewBag.Lakes = new SelectList(new List<Lake>(), "Id", "Name");

            //List<string> lakes = _GDAL.GetShpColumnValuesStr(_context.Layer.FirstOrDefault(l => l.Lake)?.FileNameWithPath, Startup.Configuration["Lakes:NameField"]).ToList();
            //lakes = lakes.Where(l => !string.IsNullOrEmpty(l)).ToList();
            //ViewBag.Lakes = lakes;

            //foreach(var lake in _context.Lake)
            //{
            //    lake.Latitude = lake.Latitude.Replace("@", "\"");
            //    lake.Longitude = lake.Longitude.Replace("@", "\"");
            //    _context.Update(lake);
            //}
            //_context.SaveChanges();

            ViewBag.Lakes = new SelectList(_context.Lake.Where(l => !string.IsNullOrEmpty(l.Name)).OrderBy(l => l.Name), "LakeId", "Name");

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

                //string styleText = "<?xml version=\"1.0\" encoding=\"Windows-1251\"?>" +
                //    "<StyledLayerDescriptor version=\"1.0.0\" " +
                //    " xsi:schemaLocation=\"http://www.opengis.net/sld StyledLayerDescriptor.xsd\" " +
                //    " xmlns=\"http://www.opengis.net/sld\" " +
                //    " xmlns:ogc=\"http://www.opengis.net/ogc\" " +
                //    " xmlns:xlink=\"http://www.w3.org/1999/xlink\" " +
                //    " xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">" +
                //    "  <!-- a Named Layer is the basic building block of an SLD document -->" +
                //    "  <NamedLayer>" +
                //    "    <Name>default_line</Name>" +
                //    "    <UserStyle>" +
                //    "    <!-- Styles can have names, titles and abstracts -->" +
                //    "      <Title>Default Line</Title>" +
                //    "      <Abstract>A sample style that draws a line</Abstract>" +
                //    "      <!-- FeatureTypeStyles describe how to render different features -->" +
                //    "      <!-- A FeatureTypeStyle for rendering lines -->" +
                //    "      <FeatureTypeStyle>" +
                //    "        <Rule>" +
                //    "          <Name>rule1</Name>" +
                //    "          <Title>Blue Line</Title>" +
                //    "          <Abstract>A solid blue line with a 1 pixel width</Abstract>" +
                //    "          <LineSymbolizer>" +
                //    "            <Stroke>" +
                //    "              <CssParameter name=\"stroke\">#00FF00</CssParameter>" +
                //    "            </Stroke>" +
                //    "          </LineSymbolizer>" +
                //    "        </Rule>" +
                //    "      </FeatureTypeStyle>" +
                //    "    </UserStyle>" +
                //    "  </NamedLayer>" +
                //    "</StyledLayerDescriptor>";


                //_GeoServer.ChangeStyle(Startup.Configuration["GeoServer:Workspace"], "line", styleText);

                //_GDAL.GetLayerMetaData("D:\\Documents\\New\\ArcGIS2\\Adm_Obl_RK_1mln.shp.xml");
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

        public IActionResult Information()
        {
            ViewData["Message"] = "Your Information page.";

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

        //[Authorize(Roles = "Administrator")]
        //public IActionResult WaterTables()
        //{
        //    return View();
        //}

        [HttpPost]
        public JsonResult GetKATO2(string KATO1Number)
        {
            if (string.IsNullOrEmpty(KATO1Number))
            {
                JsonResult resultNull = new JsonResult(null);
                return resultNull;
            }
            var KATO2 = _context.KATO
                .Where(k => k.Number.Substring(0, 2) == KATO1Number.Substring(0, 2) && k.Level == 2)
                .OrderBy(k => k.Name);
            JsonResult result = new JsonResult(KATO2);
            return result;
        }

        [HttpPost]
        public JsonResult GetKATO3(string KATO2Number)
        {
            if(string.IsNullOrEmpty(KATO2Number))
            {
                JsonResult resultNull = new JsonResult(null);
                return resultNull;
            }
            var KATO3 = _context.KATO
                .Where(k => k.Number.Substring(0, 4) == KATO2Number.Substring(0, 4) && k.Level == 3)
                .OrderBy(k => k.Name);
            JsonResult result = new JsonResult(KATO3);
            return result;
        }

        [HttpPost]
        public JsonResult GetLakes(string KATONumber, int KATOLevel)
        {
            var lakes = _GeoServer.FindLakesInKATO(KATONumber, KATOLevel);
            JsonResult result = new JsonResult(lakes);
            return result;
        }

        public IActionResult SearchLakes()
        {
            return View();
        }
    }
}
