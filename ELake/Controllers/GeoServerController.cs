using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ELake.Data;
using ELake.Models;
using HtmlAgilityPack;
using Ionic.Zip;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;

namespace ELake.Controllers
{
    public class GeoServerController : Controller
    {
        private IHostingEnvironment _hostingEnvironment;
        private readonly ApplicationDbContext _context;
        private readonly IStringLocalizer<SharedResources> _sharedLocalizer;
        private readonly GDALController _GDAL;

        public GeoServerController(IHostingEnvironment hostingEnvironment,
            ApplicationDbContext context,
            IStringLocalizer<SharedResources> sharedLocalizer,
            GDALController GDAL)
        {
            _hostingEnvironment = hostingEnvironment;
            _context = context;
            _sharedLocalizer = sharedLocalizer;
            _GDAL = GDAL;
        }

        /// <summary>
        /// Запуск CURL с параметрами
        /// </summary>
        /// <param name="Arguments"></param>
        /// <returns></returns>
        private Process CurlExecute(string Arguments)
        {
            Process process = new Process();
            try
            {
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.FileName = Startup.Configuration["GeoServer:CurlFullPath"];
                process.StartInfo.Arguments = Arguments;
                process.Start();
            }
            catch (Exception exception)
            {
                throw new Exception(exception.ToString(), exception.InnerException);
            }
            return process;
        }

        /// <summary>
        /// Возвращает путь к папке рабочей области
        /// </summary>
        /// <param name="WorkspaceName"></param>
        /// <returns></returns>
        private string GetWorkspaceDirectoryPath(string WorkspaceName)
        {
            try
            {
                return Path.Combine(Path.Combine(Startup.Configuration["GeoServer:DataDir"], "data"), WorkspaceName);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.ToString(), exception.InnerException);
            }
        }

        /// <summary>
        /// Возвращает список всех рабочих областей GeoServer
        /// </summary>
        /// <returns></returns>
        private string[] GetWorkspaces()
        {
            try
            {
                Process process = CurlExecute($" -u " +
                    $"{Startup.Configuration["GeoServer:User"]}:" +
                    $"{Startup.Configuration["GeoServer:Password"]}" +
                    $" -XGET" +
                    $" http://{Startup.Configuration["GeoServer:Address"]}:" +
                    $"{Startup.Configuration["GeoServer:Port"]}/geoserver/rest/workspaces");
                string html = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                HtmlDocument htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(html);
                HtmlNode root = htmlDocument.DocumentNode;
                List<string> workspaces = new List<string>();
                foreach (HtmlNode node in root.Descendants())
                {
                    if (node.Name == "title" && node.InnerText.ToLower().Contains("error"))
                    {
                        throw new Exception(node.InnerText);
                    }
                    if (node.Name == "a")
                    {
                        workspaces.Add(node.InnerText);
                    }
                }
                return workspaces.ToArray();
            }
            catch (Exception exception)
            {
                throw new Exception(exception.ToString(), exception.InnerException);
            }
        }

        /// <summary>
        /// Создает рабочую область в GeoServer и ее папку
        /// </summary>
        /// <remarks>
        /// Если рабочая область уже существует, будет выдано исключение
        /// </remarks>
        /// <param name="WorkspaceName"></param>
        public void CreateWorkspace(string WorkspaceName)
        {
            try
            {
                Process process = CurlExecute($" -v -u " +
                    $"{Startup.Configuration["GeoServer:User"]}:" +
                    $"{Startup.Configuration["GeoServer:Password"]}" +
                    $" -POST -H \"Content-type: text/xml\"" +
                    $" -d \"<workspace><name>{WorkspaceName}</name></workspace>\"" +
                    $" http://{Startup.Configuration["GeoServer:Address"]}:" +
                    $"{Startup.Configuration["GeoServer:Port"]}/geoserver/rest/workspaces");
                string output = process.StandardOutput.ReadToEnd();
                if (!string.IsNullOrEmpty(output))
                {
                    throw new Exception(output);
                }
                process.WaitForExit();
                Directory.CreateDirectory(GetWorkspaceDirectoryPath(WorkspaceName));
            }
            catch (Exception exception)
            {
                throw new Exception(exception.ToString(), exception.InnerException);
            }
        }

        /// <summary>
        /// Возвращает список всех хранилищ рабочей области GeoServer
        /// </summary>
        /// <param name="WorkspaceName"></param>
        /// <returns></returns>
        private string[] GetWorkspaceStores(string WorkspaceName)
        {
            try
            {
                if (!GetWorkspaces().Contains(WorkspaceName))
                {
                    throw new Exception($"No workspace {WorkspaceName}!");
                }
                if (!string.IsNullOrEmpty(WorkspaceName))
                {
                    Process process = CurlExecute($" -u " +
                        $"{Startup.Configuration["GeoServer:User"]}:" +
                        $"{Startup.Configuration["GeoServer:Password"]}" +
                        $" -XGET" +
                        $" http://{Startup.Configuration["GeoServer:Address"]}:" +
                        $"{Startup.Configuration["GeoServer:Port"]}/geoserver/rest/workspaces/{WorkspaceName}");
                    string html = process.StandardOutput.ReadToEnd();
                    process.WaitForExit();

                    HtmlDocument htmlDocument = new HtmlDocument();
                    htmlDocument.LoadHtml(html);
                    HtmlNode root = htmlDocument.DocumentNode;
                    List<string> stores = new List<string>();
                    foreach (HtmlNode node in root.Descendants())
                    {
                        if (node.Name == "a")
                        {
                            stores.Add(node.InnerText);
                        }
                    }
                    return stores.ToArray();
                }
                else
                {
                    throw new Exception("WorkspaceName must be non-empty!");
                }
            }
            catch (Exception exception)
            {
                throw new Exception(exception.ToString(), exception.InnerException);
            }
        }

        /// <summary>
        /// Возвращает тип хранилища GeoServer
        /// </summary>
        /// <param name="WorkspaceName"></param>
        /// <param name="StoreName"></param>
        /// <returns>
        /// "datastores" или "coveragestores"
        /// </returns>
        private string GetStoreType(string WorkspaceName, string StoreName)
        {
            try
            {
                Process process = CurlExecute($" -u " +
                    $"{Startup.Configuration["GeoServer:User"]}:" +
                    $"{Startup.Configuration["GeoServer:Password"]}" +
                    $" -XGET" +
                    $" http://{Startup.Configuration["GeoServer:Address"]}:" +
                    $"{Startup.Configuration["GeoServer:Port"]}/geoserver/rest/workspaces/{WorkspaceName}");
                string html = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                HtmlDocument htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(html);
                HtmlNode root = htmlDocument.DocumentNode;
                string storeType = "";
                foreach (HtmlNode node in root.Descendants())
                {
                    if (node.Name == "a")
                    {
                        if (node.InnerText == StoreName)
                        {
                            storeType = node.GetAttributeValue("href", "").Split(WorkspaceName)[1].Split(StoreName)[0].Replace("/", "");
                            break;
                        }
                    }
                }
                return storeType;
            }
            catch (Exception exception)
            {
                throw new Exception(exception.ToString(), exception.InnerException);
            }
        }

        /// <summary>
        /// Возвращает список всех слоев хранилища GeoServer
        /// </summary>
        /// <param name="WorkspaceName"></param>
        /// <param name="StoreName"></param>
        /// <returns></returns>
        private string[] GetStoreLayers(string WorkspaceName, string StoreName)
        {
            try
            {
                if (!GetWorkspaces().Contains(WorkspaceName))
                {
                    throw new Exception($"No workspace {WorkspaceName}!");
                }
                if (!GetWorkspaceStores(WorkspaceName).Contains(StoreName))
                {
                    throw new Exception($"No store {WorkspaceName} in workspace {WorkspaceName}!");
                }
                if (string.IsNullOrEmpty(WorkspaceName))
                {
                    throw new Exception("WorkspaceName must be non-empty!");
                }
                if (string.IsNullOrEmpty(StoreName))
                {
                    throw new Exception("StoreName must be non-empty!");
                }
                string storeType = GetStoreType(WorkspaceName, StoreName);
                Process process = CurlExecute($" -u " +
                    $"{Startup.Configuration["GeoServer:User"]}:" +
                    $"{Startup.Configuration["GeoServer:Password"]}" +
                    $" -XGET" +
                    $" http://{Startup.Configuration["GeoServer:Address"]}:" +
                    $"{Startup.Configuration["GeoServer:Port"]}/geoserver/rest/workspaces/{WorkspaceName}/{storeType}/{StoreName}");
                string html = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                HtmlDocument htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(html);
                HtmlNode root = htmlDocument.DocumentNode;
                List<string> layers = new List<string>();
                foreach (HtmlNode node in root.Descendants())
                {
                    if (node.Name == "a")
                    {
                        layers.Add(node.InnerText);
                    }
                }
                return layers.ToArray();
            }
            catch (Exception exception)
            {
                throw new Exception(exception.ToString(), exception.InnerException);
            }
        }

        /// <summary>
        /// Возвращает список всех слоев рабочей области GeoServer
        /// </summary>
        /// <param name="WorkspaceName"></param>
        /// <returns></returns>
        private string[] GetWorkspaceLayers(string WorkspaceName)
        {
            try
            {
                List<string> layers = new List<string>();
                foreach (string store in GetWorkspaceStores(WorkspaceName))
                {
                    layers.AddRange(GetStoreLayers(WorkspaceName, store));
                }
                return layers.ToArray();
            }
            catch (Exception exception)
            {
                throw new Exception(exception.ToString(), exception.InnerException);
            }
        }

        /// <summary>
        /// Возвращает список всех GeoTIFF слоев рабочей области GeoServer
        /// </summary>
        /// <param name="WorkspaceName"></param>
        /// <returns></returns>
        private string[] GetWorkspaceGeoTIFFLayers(string WorkspaceName)
        {
            try
            {
                List<string> layers = new List<string>();
                foreach (string store in GetWorkspaceStores(WorkspaceName))
                {
                    if(GetStoreType(WorkspaceName, store) == "coveragestores")
                    {
                        layers.AddRange(GetStoreLayers(WorkspaceName, store));
                    }
                }
                return layers.ToArray();
            }
            catch (Exception exception)
            {
                throw new Exception(exception.ToString(), exception.InnerException);
            }
        }

        /// <summary>
        /// Возвращает список всех Shape слоев рабочей области GeoServer
        /// </summary>
        /// <param name="WorkspaceName"></param>
        /// <returns></returns>
        private string[] GetWorkspaceShapeLayers(string WorkspaceName)
        {
            try
            {
                List<string> layers = new List<string>();
                foreach (string store in GetWorkspaceStores(WorkspaceName))
                {
                    if (GetStoreType(WorkspaceName, store) == "datastores")
                    {
                        layers.AddRange(GetStoreLayers(WorkspaceName, store));
                    }
                }
                return layers.ToArray();
            }
            catch (Exception exception)
            {
                throw new Exception(exception.ToString(), exception.InnerException);
            }
        }

        /// <summary>
        /// Возвращает стили GeoServer рабочей области
        /// </summary>
        /// <param name="WorkspaceName"></param>
        public string[] GetWorkspaceStyles(string WorkspaceName)
        {
            try
            {
                if (!GetWorkspaces().Contains(WorkspaceName))
                {
                    throw new Exception($"No workspace {WorkspaceName}!");
                }
                Process process = CurlExecute($" -u " +
                    $"{Startup.Configuration["GeoServer:User"]}:" +
                    $"{Startup.Configuration["GeoServer:Password"]}" +
                    $" -XGET" +
                    $" http://{Startup.Configuration["GeoServer:Address"]}:" +
                    $"{Startup.Configuration["GeoServer:Port"]}/geoserver/rest/workspaces/{WorkspaceName}/styles");
                string html = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                HtmlDocument htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(html);
                HtmlNode root = htmlDocument.DocumentNode;
                List<string> stores = new List<string>();
                foreach (HtmlNode node in root.Descendants())
                {
                    if (node.Name == "a")
                    {
                        stores.Add(node.InnerText);
                    }
                }
                return stores.ToArray();
            }
            catch (Exception exception)
            {
                throw new Exception(exception.ToString(), exception.InnerException);
            }
        }

        /// <summary>
        /// Проверяет, существует ли в рабочей области GeoServer стиль
        /// </summary>
        /// <param name="WorkspaceName"></param>
        /// <param name="style"></param>
        /// <returns></returns>
        public bool ExistWorkspaceStyle(string WorkspaceName, string Style)
        {
            try
            {
                if (GetWorkspaceStyles(WorkspaceName).Contains(Style))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception exception)
            {
                throw new Exception(exception.ToString(), exception.InnerException);
            }
        }

        /// <summary>
        /// Создает стиль GeoServer
        /// </summary>
        /// <param name="WorkspaceName"></param>
        /// <param name="Style">
        /// Название стиля
        /// </param>
        /// <param name="StyleText">
        /// Текст стиля
        /// </param>
        public void CreateStyle(string WorkspaceName, string Style, string StyleText)
        {
            try
            {
                string styleFile = Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), Path.ChangeExtension(Style, ".sld"));
                if (!System.IO.File.Exists(styleFile))
                {
                    using (StreamWriter sw = System.IO.File.CreateText(styleFile))
                    {
                        sw.WriteLine(StyleText);
                    }
                }

                Process process1 = CurlExecute($" -v -u " +
                    $"{Startup.Configuration["GeoServer:User"]}:" +
                    $"{Startup.Configuration["GeoServer:Password"]}" +
                    $" -XPOST" +
                    $" -H \"Content-type: text/xml\"" +
                    $" -d \"<style><name>{Style}</name><filename>{Style}.sld</filename></style>\"" +
                    $" http://{Startup.Configuration["GeoServer:Address"]}:" +
                    $"{Startup.Configuration["GeoServer:Port"]}/geoserver/rest/workspaces/{WorkspaceName}/styles");
                process1.WaitForExit();
                Process process2 = CurlExecute($" -v -u " +
                    $"{Startup.Configuration["GeoServer:User"]}:" +
                    $"{Startup.Configuration["GeoServer:Password"]}" +
                    $" -XPUT" +
                    $" -H \"Content-type: application/vnd.ogc.sld+xml\"" +
                    $" --data-binary @\"{styleFile}\"" +
                    $" http://{Startup.Configuration["GeoServer:Address"]}" +
                    $":{Startup.Configuration["GeoServer:Port"]}/geoserver/rest/workspaces/{WorkspaceName}/styles/{Style}");
                process2.WaitForExit();

                System.IO.File.Delete(styleFile);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.ToString(), exception.InnerException);
            }
        }

        /// <summary>
        /// Изменяет стиль GeoServer
        /// </summary>
        /// <param name="WorkspaceName"></param>
        /// <param name="Style">
        /// Название стиля
        /// </param>
        /// <param name="StyleText">
        /// Текст стиля
        /// </param>
        public void ChangeStyle(string WorkspaceName, string Style, string StyleText)
        {
            try
            {
                string styleFile = Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), Path.ChangeExtension(Style, ".sld"));
                if (!System.IO.File.Exists(styleFile))
                {
                    using (StreamWriter sw = System.IO.File.CreateText(styleFile))
                    {
                        sw.WriteLine(StyleText);
                    }
                }

                Process process1 = CurlExecute($" -u " +
                    $"{Startup.Configuration["GeoServer:User"]}:" +
                    $"{Startup.Configuration["GeoServer:Password"]}" +
                    $" -XPUT" +
                    $" -H \"Content-type: application/vnd.ogc.sld+xml\"" +
                    $" -d @\"{styleFile}\"" +
                    $" http://{Startup.Configuration["GeoServer:Address"]}" +
                    $":{Startup.Configuration["GeoServer:Port"]}/geoserver/rest/workspaces/{WorkspaceName}/styles/{Style}");
                process1.WaitForExit();

                System.IO.File.Delete(styleFile);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.ToString(), exception.InnerException);
            }
        }

        /// <summary>
        /// Удаляет слой GeoServer
        /// </summary>
        /// <param name="WorkspaceName"></param>
        /// <param name="Style"></param>
        public void DeleteStyle(string WorkspaceName, string Style)
        {
            try
            {
                Process process = CurlExecute($" -u " +
                    $"{Startup.Configuration["GeoServer:User"]}:" +
                    $"{Startup.Configuration["GeoServer:Password"]}" +
                    $" -u -XDELETE" +
                    $" http://{Startup.Configuration["GeoServer:Address"]}:" +
                    $"{Startup.Configuration["GeoServer:Port"]}/geoserver/rest/workspaces/{WorkspaceName}/styles/{Style}");
                process.WaitForExit();
            }
            catch (Exception exception)
            {
                throw new Exception(exception.ToString(), exception.InnerException);
            }
        }

        /// <summary>
        /// Публикация GeoTIFF-файла в GeoServer
        /// </summary>
        /// <remarks>
        /// Будет опубликован слой в GeoServer с именем, совпадающим с именем файла (без расширения)
        /// Будет создано новое хранилище в GeoServer с именем, совпадающим с именем файла (без расширения)
        /// Если существует опубликованный слой с именем, совпадающим с именем файла (без расширения), в рабочей области с именем, совпадающим с именем файла (без расширения), будет выдано исключение
        /// </remarks>
        /// <param name="WorkspaceName">
        /// Рабочая область GeoServer, в которой публикуется слой
        /// </param>
        /// <param name="FileName">
        /// Имя публикуемого GeoTIFF-файла с расширением без пути в папке данных GeoServer
        /// </param>
        /// <param name="Style">
        /// Стиль GeoServer, с которым будет опубликован слой. Должен быть в рабочей области "WorkspaceName"
        /// </param>
        private void PublishGeoTIFF(string WorkspaceName, string FileName, string Style)
        {
            try
            {
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(FileName);

                if (GetWorkspaceLayers(WorkspaceName).Contains(fileNameWithoutExtension))
                {
                    throw new Exception($"Layer {fileNameWithoutExtension} is already exist in {WorkspaceName} workspace!");
                }

                if (Path.GetExtension(FileName).ToLower()!=".tif" || Path.GetExtension(FileName).ToLower() != ".tif")
                {
                    throw new Exception("File extension must be \"tif\" or \"tiff\"!");
                }

                Process process1 = CurlExecute($" -u " +
                    $"{Startup.Configuration["GeoServer:User"]}:" +
                    $"{Startup.Configuration["GeoServer:Password"]}" +
                    $" -v -XPOST" +
                    $" -H \"Content-type: text/xml\"" +
                    $" \\ -d \"<coverageStore><name>{fileNameWithoutExtension}</name>" +
                    $"<workspace>{WorkspaceName}</workspace>" +
                    $"<enabled>true</enabled>" +
                    $"<type>GeoTIFF</type>" +
                    $"<url>/data/{WorkspaceName}/{FileName}</url></coverageStore>\"" +
                    $" \\ http://{Startup.Configuration["GeoServer:Address"]}:" +
                    $"{Startup.Configuration["GeoServer:Port"]}/geoserver/rest/workspaces/{WorkspaceName}/coveragestores?configure=all");
                process1.WaitForExit();
                Process process2 = CurlExecute($" -u " +
                    $"{Startup.Configuration["GeoServer:User"]}:" +
                    $"{Startup.Configuration["GeoServer:Password"]}" +
                    $" -v -XPOST" +
                    $" -H \"Content-type: text/xml\"" +
                    $" -d \"<coverage><name>{fileNameWithoutExtension}</name>" +
                    $"<title>{fileNameWithoutExtension}</title>" +
                    $"<nativeCRS>EPSG:3857</nativeCRS>" +
                    $"<srs>EPSG:3857</srs>" +
                    $"<projectionPolicy>FORCE_DECLARED</projectionPolicy>" +
                    $"<defaultInterpolationMethod><name>nearest neighbor</name></defaultInterpolationMethod></coverage>\"" +
                    $" \\ \"http://{Startup.Configuration["GeoServer:Address"]}" +
                    $":{Startup.Configuration["GeoServer:Port"]}/geoserver/rest/workspaces/{WorkspaceName}/coveragestores/{fileNameWithoutExtension}/coverages?recalculate=nativebbox\"");
                process2.WaitForExit();
                Process process3 = CurlExecute($" -v -u " +
                    $"{Startup.Configuration["GeoServer:User"]}:" +
                    $"{Startup.Configuration["GeoServer:Password"]}" +
                    $" -XPUT" +
                    $" -H \"Content-type: text/xml\"" +
                    $" -d \"<layer><defaultStyle><name>{WorkspaceName}:{Style}</name></defaultStyle></layer>\"" +
                    $" http://{Startup.Configuration["GeoServer:Address"]}" +
                    $":{Startup.Configuration["GeoServer:Port"]}/geoserver/rest/layers/{WorkspaceName}:{fileNameWithoutExtension}");
                process3.WaitForExit();
            }
            catch (Exception exception)
            {
                throw new Exception(exception.ToString(), exception.InnerException);
            }
        }

        /// <summary>
        /// Отмена публикации GeoTIFF-файла в GeoServer
        /// </summary>
        /// <remarks>
        /// Имя хранилища слоя GeoServer должно совпадать с именем слоя GeoServer
        /// </remarks>
        /// <param name="WorkspaceName">
        /// Рабочая область GeoServer, в которой отменяется публикация слоя
        /// </param>
        /// <param name="LayerName">
        /// Имя слоя GeoServer, отмена публикации которого происходит
        /// </param>
        private void UnpublishGeoTIFF(string WorkspaceName, string LayerName)
        {
            try
            {
                if(!GetWorkspaceLayers(WorkspaceName).Contains(LayerName))
                {
                    throw new Exception($"Layer {LayerName} isn't exist in {WorkspaceName} workspace!");
                }

                Process process1 = CurlExecute($" -u " +
                    $"{Startup.Configuration["GeoServer:User"]}:" +
                    $"{Startup.Configuration["GeoServer:Password"]}" +
                    $" -v -XDELETE" +
                    $" http://{Startup.Configuration["GeoServer:Address"]}:" +
                    $"{Startup.Configuration["GeoServer:Port"]}/" +
                    $"geoserver/rest/layers/{LayerName}");
                process1.WaitForExit();
                Process process2 = CurlExecute($" -u " +
                    $"{Startup.Configuration["GeoServer:User"]}:" +
                    $"{Startup.Configuration["GeoServer:Password"]}" +
                    $" -v -XDELETE" +
                    $" http://{Startup.Configuration["GeoServer:Address"]}" +
                    $":{Startup.Configuration["GeoServer:Port"]}/" +
                    $"geoserver/rest/workspaces/{WorkspaceName}/coveragestores/{LayerName}/coverages/{LayerName}");
                process2.WaitForExit();
                Process process3 = CurlExecute($" -v -u " +
                    $"{Startup.Configuration["GeoServer:User"]}:" +
                    $"{Startup.Configuration["GeoServer:Password"]}" +
                    $" -XDELETE" +
                    $" http://{Startup.Configuration["GeoServer:Address"]}" +
                    $":{Startup.Configuration["GeoServer:Port"]}/" +
                    $"geoserver/rest/workspaces/{WorkspaceName}/coveragestores/{LayerName}");
                process3.WaitForExit();
            }
            catch (Exception exception)
            {
                throw new Exception(exception.ToString(), exception.InnerException);
            }
        }

        /// <summary>
        /// Возвращает все GeoTIFF-файлы (без пути, с расширением) с папки рабочей области GeoServer
        /// </summary>
        /// <param name="WorkspaceName"></param>
        /// <returns></returns>
        public string[] GetGeoTIFFFiles(string WorkspaceName)
        {
            IConfigurationSection geoTIFFFileExtentions = Startup.Configuration.GetSection("GeoServer:GeoTIFFFileExtentions");
            List<string> geoTIFFFiles = Directory.GetFiles(GetWorkspaceDirectoryPath(WorkspaceName), "*.*", SearchOption.TopDirectoryOnly).OrderBy(l => l).ToList();
            geoTIFFFiles.RemoveAll(l => !geoTIFFFileExtentions.AsEnumerable().Select(e => e.Value).Contains(Path.GetExtension(l)));
            geoTIFFFiles = geoTIFFFiles.Select(l => { return Path.GetFileName(l); }).ToList();
            for (int i = geoTIFFFiles.Count - 1; i >= 0; i--)
            {
                bool ok = false;
                foreach(string ext in geoTIFFFileExtentions.AsEnumerable().Select(e => e.Value))
                {
                    if((Path.GetExtension(geoTIFFFiles[i]) == ext)&&(ext!=".xml"))
                    {
                        ok = true;
                        break;
                    }
                }
                if(!ok)
                {
                    geoTIFFFiles.RemoveAt(i);
                }
            }
            return geoTIFFFiles.ToArray();
        }

        /// <summary>
        /// Загрузка GeoTIFF-файлов в папку рабочей области GeoServer
        /// </summary>
        /// <param name="WorkspaceName"></param>
        /// <param name="Files"></param>
        private string[] UploadGeoTIFFFiles(string WorkspaceName, List<IFormFile> Files)
        {
            List<string> filesreport = new List<string>(),
            report = new List<string>();
            try
            {
                foreach (IFormFile file in Files)
                {
                    var filePath = Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), "Upload", Path.GetFileName(file.FileName));
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                }
                List<string> unzipfiles = new List<string>();
                List<string> zipfiles = new List<string>();
                foreach (string file in Directory.GetFiles(Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), "Upload"), "*.zip", SearchOption.TopDirectoryOnly))
                {
                    if (Path.GetExtension(file) == ".zip")
                    {
                        try
                        {
                            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                            using (ZipFile zip = ZipFile.Read(Path.Combine(Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), "Upload"), Path.GetFileName(file))))
                            {
                                foreach (ZipEntry filefromzip in zip)
                                {
                                    filefromzip.Extract(Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), "Upload"), ExtractExistingFileAction.OverwriteSilently);
                                    unzipfiles.Add(Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), "Upload", filefromzip.FileName));
                                }
                            }
                            //zipfiles.Add(file);
                            zipfiles.AddRange(Directory.GetFiles(Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), "Upload"), Path.GetFileNameWithoutExtension(file) + ".z*", SearchOption.TopDirectoryOnly));
                        }
                        catch
                        {

                        }
                    }
                }
                IConfigurationSection geoTIFFFileExtentions = Startup.Configuration.GetSection("GeoServer:GeoTIFFFileExtentions");
                foreach (string file in zipfiles)
                {
                    if (!geoTIFFFileExtentions.AsEnumerable().Select(l => l.Value).Contains(Path.GetExtension(file)))
                    {
                        System.IO.File.Delete(Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), "Upload", file));
                    }
                }
                foreach (string file in Files.Select(f => f.FileName))
                {
                    var fileName = Path.GetFileName(file);
                    if(geoTIFFFileExtentions.AsEnumerable().Select(l => l.Value).Contains(Path.GetExtension(fileName).ToLower()))
                    {
                        if (System.IO.File.Exists(Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), fileName)))
                        {
                            report.Add($"{fileName}: {_sharedLocalizer["exist"]}!");
                            System.IO.File.Delete(Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), "Upload", fileName));
                        }
                        else
                        {
                            string metaDataFile = fileName + ".xml";
                            if(!System.IO.File.Exists(Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), "Upload", metaDataFile))
                                &&(Path.GetExtension(fileName)!=".xml"))
                            {
                                report.Add($"{fileName}: {_sharedLocalizer["noMetaData"]}!");
                                System.IO.File.Delete(Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), "Upload", fileName));
                            }
                            else
                            {
                                if(Path.GetExtension(fileName)==".tif")
                                {
                                    string cs = _GDAL.GetLayerCoordinateSystemName(Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), "Upload", fileName));
                                    if (cs != "EPSG:3857")
                                    {

                                        _GDAL.SaveLayerWithNewCoordinateSystem(Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), "Upload", fileName),
                                            Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), fileName),
                                            "EPSG:3857");

                                        report.Add($"{fileName}: {_sharedLocalizer["not3857Changed"]}!");
                                        System.IO.File.Delete(Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), "Upload", fileName));
                                    }
                                    else
                                    {
                                        System.IO.File.Move(Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), "Upload", fileName),
                                            Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), fileName));
                                        report.Add($"{fileName}: {_sharedLocalizer["uploaded"]}!");
                                    }
                                }
                                else
                                {
                                    System.IO.File.Move(Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), "Upload", fileName),
                                            Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), fileName));
                                    report.Add($"{fileName}: {_sharedLocalizer["uploaded"]}!");
                                }
                            }
                        }
                    }
                    else if(Path.GetExtension(file)[1] != 'z')
                    {
                        report.Add($"{fileName}: {_sharedLocalizer["notGeoTIFF"]}!");
                        System.IO.File.Delete(Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), "Upload", fileName));
                    }
                }
                foreach(string file in unzipfiles)
                {
                    var fileName = Path.GetFileName(file);
                    if (geoTIFFFileExtentions.AsEnumerable().Select(l => l.Value).Contains(Path.GetExtension(fileName).ToLower()))
                    {
                        if (System.IO.File.Exists(Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), fileName)))
                        {
                            report.Add($"{fileName}: {_sharedLocalizer["exist"]}!");
                            System.IO.File.Delete(Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), "Upload", fileName));
                        }
                        else
                        {
                            string metaDataFile = fileName + ".xml";
                            if (!System.IO.File.Exists(Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), "Upload", metaDataFile))
                                && (Path.GetExtension(fileName) != ".xml"))
                            {
                                report.Add($"{fileName}: {_sharedLocalizer["noMetaData"]}!");
                                System.IO.File.Delete(Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), "Upload", fileName));
                            }
                            else
                            {
                                //System.IO.File.Move(Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), "Upload", fileName), Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), fileName));
                                //report.Add($"{fileName}: {_sharedLocalizer["uploaded"]}!");
                                if (Path.GetExtension(fileName) == ".tif")
                                {
                                    //string cs = _GDAL.GetLayerCoordinateSystemName(Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), "Upload", fileName));
                                    string cs = "";
                                    if (cs != "EPSG:3857")
                                    {

                                        _GDAL.SaveLayerWithNewCoordinateSystem(Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), "Upload", fileName),
                                            Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), fileName),
                                            "EPSG:3857");

                                        report.Add($"{fileName}: {_sharedLocalizer["not3857Changed"]}!");
                                        System.IO.File.Delete(Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), "Upload", fileName));
                                    }
                                    else
                                    {
                                        System.IO.File.Move(Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), "Upload", fileName),
                                            Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), fileName));
                                        report.Add($"{fileName}: {_sharedLocalizer["uploaded"]}!");
                                    }
                                }
                                else
                                {
                                    System.IO.File.Move(Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), "Upload", fileName),
                                            Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), fileName));
                                    report.Add($"{fileName}: {_sharedLocalizer["uploaded"]}!");
                                }
                            }
                        }
                    }
                    else
                    {
                        report.Add($"{fileName}: {_sharedLocalizer["notGeoTIFF"]}!");
                        System.IO.File.Delete(Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), "Upload", fileName));
                    }
                }
            }
            catch (Exception exception)
            {
                throw new Exception(exception.ToString(), exception.InnerException);
            }
            return report.ToArray();
        }

        private string[] UploadGeoTIFFFilesWater(string WorkspaceName, string Folder, int Year, int Month, List<IFormFile> Files)
        {
            string monthS = Month.ToString();
            if (monthS.Length < 2)
            {
                monthS = "0" + monthS;
            }
            string subfolder = Year.ToString();
            if (Folder == "MonthlyHistory")
            {
                subfolder += "_" + monthS;
            }
            string fullfolder = Path.Combine(Folder, subfolder);

            Folder = Path.Combine(Startup.Configuration["WaterFolder"], fullfolder);

            List<string> filesreport = new List<string>(),
            report = new List<string>();
            try
            {
                foreach (IFormFile file in Files)
                {
                    var filePath = Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), "Upload", Path.GetFileName(file.FileName));
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                }
                List<string> unzipfiles = new List<string>();
                List<string> zipfiles = new List<string>();
                foreach (string file in Directory.GetFiles(Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), "Upload"), "*.zip", SearchOption.TopDirectoryOnly))
                {
                    if (Path.GetExtension(file) == ".zip")
                    {
                        try
                        {
                            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                            using (ZipFile zip = ZipFile.Read(Path.Combine(Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), "Upload"), Path.GetFileName(file))))
                            {
                                foreach (ZipEntry filefromzip in zip)
                                {
                                    filefromzip.Extract(Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), "Upload"), ExtractExistingFileAction.OverwriteSilently);
                                    unzipfiles.Add(Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), "Upload", filefromzip.FileName));
                                }
                            }
                            //zipfiles.Add(file);
                            zipfiles.AddRange(Directory.GetFiles(Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), "Upload"), Path.GetFileNameWithoutExtension(file) + ".z*", SearchOption.TopDirectoryOnly));
                        }
                        catch
                        {

                        }
                    }
                }
                IConfigurationSection geoTIFFFileExtentions = Startup.Configuration.GetSection("GeoServer:GeoTIFFFileExtentions");
                foreach (string file in zipfiles)
                {
                    if (!geoTIFFFileExtentions.AsEnumerable().Select(l => l.Value).Contains(Path.GetExtension(file)))
                    {
                        System.IO.File.Delete(Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), "Upload", file));
                    }
                }
                
                foreach (string file in Files.Select(f => f.FileName))
                {
                    var fileName = Path.GetFileName(file);
                    if (geoTIFFFileExtentions.AsEnumerable().Select(l => l.Value).Contains(Path.GetExtension(fileName).ToLower()))
                    {
                        if (System.IO.File.Exists(Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), Folder, fileName)))
                        {
                            report.Add($"{fileName}: {_sharedLocalizer["exist"]}!");
                            System.IO.File.Delete(Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), "Upload", fileName));
                        }
                        else
                        {
                            if (!Directory.Exists(Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), Folder)))
                            {
                                Directory.CreateDirectory(Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), Folder));
                            }
                            System.IO.File.Move(Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), "Upload", fileName),
                                Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), Folder, fileName));
                            report.Add($"{fileName}: {_sharedLocalizer["uploaded"]}!");
                        }
                    }
                    else if (Path.GetExtension(file)[1] != 'z')
                    {
                        report.Add($"{fileName}: {_sharedLocalizer["notGeoTIFF"]}!");
                        System.IO.File.Delete(Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), "Upload", fileName));
                    }
                }
                foreach (string file in unzipfiles)
                {
                    var fileName = Path.GetFileName(file);
                    if (geoTIFFFileExtentions.AsEnumerable().Select(l => l.Value).Contains(Path.GetExtension(fileName).ToLower()))
                    {
                        if (System.IO.File.Exists(Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), Folder, fileName)))
                        {
                            report.Add($"{fileName}: {_sharedLocalizer["exist"]}!");
                            System.IO.File.Delete(Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), "Upload", fileName));
                        }
                        else
                        {
                            string metaDataFile = fileName + ".xml";
                            if (!System.IO.File.Exists(Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), "Upload", metaDataFile))
                                && (Path.GetExtension(fileName) != ".xml"))
                            {
                                report.Add($"{fileName}: {_sharedLocalizer["noMetaData"]}!");
                                System.IO.File.Delete(Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), "Upload", fileName));
                            }
                            else
                            {
                                if (!Directory.Exists(Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), Folder)))
                                {
                                    Directory.CreateDirectory(Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), Folder));
                                }
                                System.IO.File.Move(Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), "Upload", fileName),
                                    Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), Folder, fileName));
                                report.Add($"{fileName}: {_sharedLocalizer["uploaded"]}!");
                            }
                        }
                    }
                    else
                    {
                        report.Add($"{fileName}: {_sharedLocalizer["notGeoTIFF"]}!");
                        System.IO.File.Delete(Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), "Upload", fileName));
                    }
                }
            }
            catch (Exception exception)
            {
                throw new Exception(exception.ToString(), exception.InnerException);
            }
            return report.ToArray();
        }

        /// <summary>
        /// Возвращает файл (путь, расширение) хранилища типа "coveragestores"
        /// </summary>
        /// <param name="WorkspaceName"></param>
        /// <param name="StoreName"></param>
        private string GetCoveragestoreGeoTIFFFile(string WorkspaceName, string StoreName)
        {
            try
            {
                string GeoTIFFFile = "";

                Process process = CurlExecute($" -u " +
                    $"{Startup.Configuration["GeoServer:User"]}:" +
                    $"{Startup.Configuration["GeoServer:Password"]}" +
                    $" -XGET" +
                    $" http://{Startup.Configuration["GeoServer:Address"]}:" +
                    $"{Startup.Configuration["GeoServer:Port"]}/geoserver/rest/workspaces/{WorkspaceName}/coveragestores/{StoreName}.xml");
                string html = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                HtmlDocument htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(html);
                HtmlNode root = htmlDocument.DocumentNode;
                foreach (HtmlNode node in root.Descendants())
                {
                    if (node.Name == "url")
                    {
                        GeoTIFFFile = Startup.Configuration["GeoServer:DataDir"] + node.InnerText.Replace("/", "\\");
                        break;
                    }
                }
                return GeoTIFFFile;
            }
            catch (Exception exception)
            {
                throw new Exception(exception.ToString(), exception.InnerException);
            }
        }

        /// <summary>
        /// Возвращает список слоев, в которых используется GeoTIFF-файл
        /// </summary>
        /// <param name="WorkspaceName"></param>
        /// <param name="FileName">
        /// Полный путь к файлу с расширением
        /// </param>
        private string[] GetGeoTIFFFileLayers(string WorkspaceName, string FileName)
        {
            try
            {
                Process process = CurlExecute($" -u " +
                $"{Startup.Configuration["GeoServer:User"]}:" +
                $"{Startup.Configuration["GeoServer:Password"]}" +
                $" -XGET" +
                $" http://{Startup.Configuration["GeoServer:Address"]}:" +
                $"{Startup.Configuration["GeoServer:Port"]}/geoserver/rest/workspaces/{WorkspaceName}");
                string html = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                HtmlDocument htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(html);
                HtmlNode root = htmlDocument.DocumentNode;
                List<string> layers = new List<string>();
                foreach (HtmlNode node in root.Descendants())
                {
                    if (node.Name == "a")
                    {
                        if (node.GetAttributeValue("href", "").Contains("coveragestores"))
                        {
                            if (GetCoveragestoreGeoTIFFFile(WorkspaceName, node.InnerText) == Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), FileName))
                            {
                                layers.AddRange(GetStoreLayers(WorkspaceName, node.InnerText));
                            }
                        }
                    }
                }
                return layers.ToArray();
            }
            catch (Exception exception)
            {
                throw new Exception(exception.ToString(), exception.InnerException);
            }
        }

        /// <summary>
        /// Возвращает список слоев, в которых используется Shape-файл
        /// </summary>
        /// <param name="WorkspaceName"></param>
        /// <param name="FileName">
        /// Имя файла с расширением, без пути
        /// </param>
        private string[] GetShapeFileLayers(string WorkspaceName, string FileName)
        {
            try
            {
                Process process = CurlExecute($" -u " +
                $"{Startup.Configuration["GeoServer:User"]}:" +
                $"{Startup.Configuration["GeoServer:Password"]}" +
                $" -XGET" +
                $" http://{Startup.Configuration["GeoServer:Address"]}:" +
                $"{Startup.Configuration["GeoServer:Port"]}/geoserver/rest/workspaces/{WorkspaceName}");
                string html = process.StandardOutput.ReadToEnd();
                process.WaitForExit();

                HtmlDocument htmlDocument = new HtmlDocument();
                htmlDocument.LoadHtml(html);
                HtmlNode root = htmlDocument.DocumentNode;
                List<string> layers = new List<string>();
                foreach (HtmlNode node in root.Descendants())
                {
                    if (node.Name == "a")
                    {
                        if (node.GetAttributeValue("href", "").Contains("datastores"))
                        {
                            //if (GetDatastoreShapeFile(WorkspaceName, node.InnerText) == Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), FileName))
                            //{
                            //    layers.AddRange(GetStoreLayers(WorkspaceName, node.InnerText));
                            //}
                            // не очень корректно сделано, надо как выше (закоментировано)
                            if(node.InnerText == Path.GetFileNameWithoutExtension(FileName))
                            {
                                layers.AddRange(GetStoreLayers(WorkspaceName, node.InnerText));
                            }
                        }
                    }
                }
                return layers.ToArray();
            }
            catch (Exception exception)
            {
                throw new Exception(exception.ToString(), exception.InnerException);
            }
        }

        /// <summary>
        /// Удаление GeoTIFF-файла
        /// </summary>
        /// <remarks>
        /// Елси GeoTIFF-файл используется в слоях GeoServer, будет выдано исключение
        /// </remarks>
        /// <param name="WorkspaceName"></param>
        /// <param name="FileName">
        /// Полный путь к файлу с расширением
        /// </param>
        private void DeleteGeoTIFFFile(string WorkspaceName, string FileName)
        {
            try
            {
                string[] geoTIFFFileLayers = GetGeoTIFFFileLayers(WorkspaceName, FileName);
                if (geoTIFFFileLayers.Count()>0)
                {
                    //throw new Exception($"{FileName} can't be deleted because it is used in current layers: {string.Join(", ", geoTIFFFileLayers)}!");
                    throw new Exception(string.Format(_sharedLocalizer["MessageCantBeDeleted"], FileName, string.Join(", ", geoTIFFFileLayers)));
                }
                System.IO.File.Delete(Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), FileName));
                try
                {
                    System.IO.File.Delete(Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), FileName + ".xml"));
                }
                catch
                {

                }
            }
            catch (Exception exception)
            {
                throw new Exception(exception.ToString(), exception.InnerException);
            }
        }

        /// <summary>
        /// Возвращает все Shape-файлы (с путем, с расширением) с папки рабочей области GeoServer
        /// </summary>
        /// <param name="WorkspaceName"></param>
        /// <returns>
        /// Возвращает список Shape-файлов с полным путем и расширением
        /// </returns>
        private string[] GetShapeFilesInDataDirectory(string WorkspaceName)
        {
            try
            {
                List<string> shapeFiles = Directory.GetFiles(GetWorkspaceDirectoryPath(WorkspaceName), "*.shp", SearchOption.TopDirectoryOnly).OrderBy(l => l).ToList();
                return shapeFiles.ToArray();
            }
            catch (Exception exception)
            {
                throw new Exception(exception.ToString(), exception.InnerException);
            }
        }

        /// <summary>
        /// Возвращает все Shape-файлы (без пути, с расширением) с папки рабочей области GeoServer и вложенных папок
        /// </summary>
        /// <param name="WorkspaceName"></param>
        /// <returns>
        /// Возвращает список Shape-файлов с расширением, без пути
        /// </returns>
        private string[] GetShapeFiles(string WorkspaceName)
        {
            try
            {
                List<string> shapeFiles = Directory.GetFiles(GetWorkspaceDirectoryPath(WorkspaceName), "*.shp", SearchOption.AllDirectories).OrderBy(l => l).ToList();
                shapeFiles = shapeFiles.Select(s => { s = Path.GetFileName(s); return s; }).ToList();
                return shapeFiles.ToArray();
            }
            catch (Exception exception)
            {
                throw new Exception(exception.ToString(), exception.InnerException);
            }
        }

        /// <summary>
        /// Перемещает Shape-файл и его вспомогательные файлы в папку с именем, совпадающим с именем Shape-файла без расширения
        /// </summary>
        /// <remarks>
        /// В процессе будет создана папка в папке рабочей области с именем, совпадающим с именем Shape-файла без расширения
        /// </remarks>
        /// <param name="WorkspaceName"></param>
        /// <param name="FileName">
        /// Полный путь к Shape-файлу с расширением
        /// </param>
        private void MoveShapeFileToDirectory(string WorkspaceName, string FileName)
        {
            try
            {
                string fileNameWithoutPathWithoutExtension = Path.GetFileNameWithoutExtension(FileName);
                List<string> files = Directory.GetFiles(GetWorkspaceDirectoryPath(WorkspaceName), $"{fileNameWithoutPathWithoutExtension}.*", SearchOption.TopDirectoryOnly).OrderBy(l => l).ToList();
                string directory = Directory.CreateDirectory(Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), fileNameWithoutPathWithoutExtension)).Name;
                foreach(string file in files)
                {
                    string newFile = Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), directory, Path.GetFileName(file));
                    System.IO.File.Move(file, newFile);
                }
            }
            catch (Exception exception)
            {
                throw new Exception(exception.ToString(), exception.InnerException);
            }
        }

        /// <summary>
        /// Перемещает Shape-файлы и их вспомогательные файлы в папки с именами, совпадающими с именами Shape-файлоа без расширения
        /// </summary>
        /// /// <remarks>
        /// В процессе будут созданы папки в папке рабочей области с именами, совпадающими с именами Shape-файлоа без расширения
        /// </remarks>
        /// <param name="WorkspaceName"></param>
        private void MoveShapeFilesToDirectories(string WorkspaceName)
        {
            try
            {
                foreach(string shapeFile in GetShapeFilesInDataDirectory(WorkspaceName))
                {
                    MoveShapeFileToDirectory(WorkspaceName, shapeFile);
                }
            }
            catch (Exception exception)
            {
                throw new Exception(exception.ToString(), exception.InnerException);
            }
        }

        /// <summary>
        /// Загрузка Shape-файлов в папку рабочей области GeoServer
        /// </summary>
        /// <param name="WorkspaceName"></param>
        /// <param name="Files"></param>
        private string[] UploadShapeFiles(string WorkspaceName, List<IFormFile> Files)
        {
            List<string> filesreport = new List<string>(),
            report = new List<string>();
            try
            {
                foreach (IFormFile file in Files)
                {
                    var filePath = Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), "Upload", Path.GetFileName(file.FileName));
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                }
                List<string> unzipfiles = new List<string>();
                List<string> zipfiles = new List<string>();
                foreach (string file in Directory.GetFiles(Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), "Upload"), "*.zip", SearchOption.TopDirectoryOnly))
                {
                    if (Path.GetExtension(file) == ".zip")
                    {
                        try
                        {
                            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                            using (ZipFile zip = ZipFile.Read(Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), "Upload", Path.GetFileName(file))))
                            {
                                foreach (ZipEntry filefromzip in zip)
                                {
                                    filefromzip.Extract(Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), "Upload"), ExtractExistingFileAction.OverwriteSilently);
                                    unzipfiles.Add(Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), "Upload", filefromzip.FileName));
                                }
                            }
                            zipfiles.AddRange(Directory.GetFiles(Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), "Upload"), Path.GetFileNameWithoutExtension(file) + ".z*", SearchOption.TopDirectoryOnly));
                        }
                        catch
                        {

                        }
                    }
                }
                IConfigurationSection shapeFileExtentions = Startup.Configuration.GetSection("GeoServer:ShapeFileExtentions");
                foreach (string file in zipfiles)
                {
                    if (!shapeFileExtentions.AsEnumerable().Select(l => l.Value).Contains(Path.GetExtension(file)))
                    {
                        System.IO.File.Delete(Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), "Upload", file));
                    }
                }
                foreach (string file in Files.Select(f => f.FileName))
                {
                    var filePath = Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), "Upload", Path.GetFileName(file));
                    var fileName = Path.GetFileName(file);
                    if (shapeFileExtentions.AsEnumerable().Select(l => l.Value).Contains(Path.GetExtension(fileName).ToLower()))
                    {
                        if (System.IO.File.Exists(Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), Path.GetFileNameWithoutExtension(fileName), fileName)))
                        {
                            System.IO.File.Delete(Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), "Upload", fileName));
                            if(Path.GetExtension(fileName) == ".shp")
                            {
                                report.Add($"{fileName}: {_sharedLocalizer["exist"]}!");
                            }
                        }
                        else
                        {
                            string metaDataFile = fileName + ".xml",
                                metaDataFile2 = Path.ChangeExtension(fileName, ".shp") + ".xml";
                            if ((!System.IO.File.Exists(Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), "Upload", metaDataFile))
                                &&(!System.IO.File.Exists(Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), "Upload", metaDataFile2))))
                                && (!System.IO.File.Exists(Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), metaDataFile)))
                                && (!System.IO.File.Exists(Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), metaDataFile2)))
                                && (Path.GetExtension(fileName) != ".xml"))
                            {
                                report.Add($"{fileName}: {_sharedLocalizer["noMetaData"]}!");
                                System.IO.File.Delete(Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), "Upload", fileName));
                            }
                            else
                            {
                                if (Path.GetExtension(fileName) == ".shp")
                                {
                                    string cs = _GDAL.GetLayerCoordinateSystemNameShp(Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), "Upload", fileName));
                                    if (cs != "EPSG:3857")
                                    {
                                        _GDAL.SaveLayerWithNewCoordinateSystem(Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), "Upload", fileName),
                                            Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), fileName),
                                            "EPSG:3857");
                                        report.Add($"{fileName}: {_sharedLocalizer["not3857Changed"]}!");
                                        System.IO.File.Delete(Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), "Upload", fileName));
                                    }
                                    else
                                    {
                                        System.IO.File.Move(filePath, Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), fileName));
                                        report.Add($"{fileName}: {_sharedLocalizer["uploaded"]}!");
                                    }
                                }
                                else
                                {
                                    System.IO.File.Move(filePath, Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), fileName));
                                    report.Add($"{fileName}: {_sharedLocalizer["uploaded"]}!");
                                }
                            }
                        }
                    }
                    else if (Path.GetExtension(file)[1] != 'z')
                    {
                        report.Add($"{fileName}: {_sharedLocalizer["notShape"]}!");
                        System.IO.File.Delete(Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), "Upload", fileName));
                    }
                }
                foreach (string file in unzipfiles)
                {
                    var fileName = Path.GetFileName(file);
                    if (shapeFileExtentions.AsEnumerable().Select(l => l.Value).Contains(Path.GetExtension(fileName).ToLower()))
                    {
                        if (System.IO.File.Exists(Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), Path.GetFileNameWithoutExtension(fileName), fileName)))
                        {
                            System.IO.File.Delete(Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), "Upload", fileName));
                            if (Path.GetExtension(fileName) == ".shp")
                            {
                                report.Add($"{fileName}: {_sharedLocalizer["exist"]}!");
                            }
                        }
                        else
                        {
                            string metaDataFile = fileName + ".xml",
                                metaDataFile2 = Path.ChangeExtension(fileName, ".shp") + ".xml";
                            if ((!System.IO.File.Exists(Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), "Upload", metaDataFile))
                                && (!System.IO.File.Exists(Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), "Upload", metaDataFile2))))
                                && (!System.IO.File.Exists(Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), metaDataFile)))
                                && (!System.IO.File.Exists(Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), metaDataFile2)))
                                && (Path.GetExtension(fileName) != ".xml"))
                            {
                                report.Add($"{fileName}: {_sharedLocalizer["noMetaData"]}!");
                                System.IO.File.Delete(Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), "Upload", fileName));
                            }
                            else
                            {
                                if (Path.GetExtension(fileName) == ".shp")
                                {
                                    string cs = _GDAL.GetLayerCoordinateSystemName(Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), "Upload", fileName));
                                    if (cs != "EPSG:3857")
                                    {
                                        _GDAL.SaveLayerWithNewCoordinateSystem(Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), "Upload", fileName),
                                            Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), fileName),
                                            "EPSG:3857");
                                        report.Add($"{fileName}: {_sharedLocalizer["not3857Changed"]}!");
                                        System.IO.File.Delete(Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), "Upload", fileName));
                                    }
                                    else
                                    {
                                        System.IO.File.Move(file, Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), fileName));
                                        report.Add($"{fileName}: {_sharedLocalizer["uploaded"]}!");
                                    }
                                }
                                else
                                {
                                    System.IO.File.Move(file, Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), fileName));
                                    report.Add($"{fileName}: {_sharedLocalizer["uploaded"]}!");
                                }
                            }
                        }
                    }
                    else
                    {
                        report.Add($"{fileName}: {_sharedLocalizer["notShape"]}!");
                        System.IO.File.Delete(Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), "Upload", fileName));
                    }
                }
                MoveShapeFilesToDirectories(WorkspaceName);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.ToString(), exception.InnerException);
            }
            return report.ToArray();
        }

        /// <summary>
        /// Удаление Shape-файла
        /// </summary>
        /// <remarks>
        /// Елси Shape-файл используется в слоях GeoServer, будет выдано исключение
        /// </remarks>
        /// <param name="WorkspaceName"></param>
        /// <param name="FileName">
        /// Имя файла с раширением, без пути
        /// </param>
        private void DeleteShapeFile(string WorkspaceName, string FileName)
        {
            try
            {
                string[] shapeFileLayers = GetShapeFileLayers(WorkspaceName, FileName);
                if (shapeFileLayers.Count() > 0)
                {
                    //throw new Exception($"{FileName} can't be deleted because it is used in current layers: {string.Join(", ", shapeFileLayers)}!");
                    throw new Exception(string.Format(_sharedLocalizer["MessageCantBeDeleted"], FileName, string.Join(", ", shapeFileLayers)));
                }
                Directory.Delete(Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), Path.GetFileNameWithoutExtension(FileName)), true);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.ToString(), exception.InnerException);
            }
        }

        /// <summary>
        /// Публикация Shape-файла в GeoServer
        /// </summary>
        /// <remarks>
        /// Будет опубликован слой в GeoServer с именем, совпадающим с именем Shape-файла (без расширения)
        /// Будет создано новое хранилище в GeoServer с именем, совпадающим с именем Shape-файла (без расширения)
        /// Если существует опубликованный слой с именем, совпадающим с именем файла (без расширения), в рабочей области с именем, совпадающим с именем файла (без расширения), будет выдано исключение
        /// </remarks>
        /// <param name="WorkspaceName">
        /// Рабочая область GeoServer, в которой публикуется слой
        /// </param>
        /// <param name="FileName">
        /// Имя публикуемого Shape-файла с расширением, без пути в папке данных GeoServer
        /// </param>
        /// <param name="Style">
        /// Стиль GeoServer, с которым будет опубликован слой. Должен быть в рабочей области "WorkspaceName"
        /// </param>
        private void PublishShape(string WorkspaceName, string FileName, string Style)
        {
            try
            {
                string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(FileName),
                    filesDirectory = Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), fileNameWithoutExtension),
                    fileNameWithPath = Path.Combine(filesDirectory, FileName),
                    zipFile = Path.ChangeExtension(fileNameWithPath, ".zip");

                System.IO.File.Delete(zipFile);

                using (ZipFile zip = new ZipFile())
                {
                    Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                    zip.AddFiles(Directory.GetFiles(filesDirectory), false, "");
                    zip.ParallelDeflateThreshold = -1;
                    zip.Save(zipFile);
                }


                if (GetWorkspaceLayers(WorkspaceName).Contains(fileNameWithoutExtension))
                {
                    //throw new Exception($"Layer {fileNameWithoutExtension} is already exist in {WorkspaceName} workspace!");
                    throw new Exception(string.Format(_sharedLocalizer["MessageLayerAlreadyExist"], fileNameWithoutExtension, WorkspaceName));
                }

                if (Path.GetExtension(FileName).ToLower() != ".shp")
                {
                    //throw new Exception("File extension must be \"shp\"!");
                    throw new Exception(_sharedLocalizer["MessageFileExtensionMustBeShp"]);
                }

                Process process1 = CurlExecute($" -u " +
                    $"{Startup.Configuration["GeoServer:User"]}:" +
                    $"{Startup.Configuration["GeoServer:Password"]}" +
                    $" -v -XPUT" +
                    $" -H \"Content-type: application/zip\"" +
                    $" --data-binary @\"{zipFile}\"" +
                    $" http://{Startup.Configuration["GeoServer:Address"]}:" +
                    $"{Startup.Configuration["GeoServer:Port"]}/geoserver/rest/workspaces/{WorkspaceName}/datastores/{fileNameWithoutExtension}/file.shp");
                process1.WaitForExit();
                Process process2 = CurlExecute($" -v -u " +
                    $"{Startup.Configuration["GeoServer:User"]}:" +
                    $"{Startup.Configuration["GeoServer:Password"]}" +
                    $" -XPUT" +
                    $" -H \"Content-type: text/xml\"" +
                    $" -d \"<layer><defaultStyle><name>{WorkspaceName}:{Style}</name></defaultStyle></layer>\"" +
                    $" http://{Startup.Configuration["GeoServer:Address"]}" +
                    $":{Startup.Configuration["GeoServer:Port"]}/geoserver/rest/layers/{WorkspaceName}:{fileNameWithoutExtension}");
                process2.WaitForExit();
            }
            catch (Exception exception)
            {
                throw new Exception(exception.ToString(), exception.InnerException);
            }
        }

        ///// <summary>
        ///// Публикация Shape-файла в GeoServer
        ///// </summary>
        ///// <remarks>
        ///// Будет опубликован слой в GeoServer с именем, совпадающим с именем Shape-файла (без расширения)
        ///// Будет создано новое хранилище в GeoServer с именем, совпадающим с именем Shape-файла (без расширения)
        ///// Если существует опубликованный слой с именем, совпадающим с именем файла (без расширения), в рабочей области с именем, совпадающим с именем файла (без расширения), будет выдано исключение
        ///// </remarks>
        ///// <param name="WorkspaceName">
        ///// Рабочая область GeoServer, в которой публикуется слой
        ///// </param>
        ///// <param name="FileName">
        ///// Имя публикуемого Shape-файла с расширением, без пути в папке данных GeoServer
        ///// </param>
        ///// <param name="Style">
        ///// Стиль GeoServer, с которым будет опубликован слой. Должен быть в рабочей области "WorkspaceName"
        ///// </param>
        //private void PublishShape(string WorkspaceName, string FileName, string LayerName, string Style)
        //{
        //    try
        //    {
        //        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(FileName),
        //            filesDirectory = Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), fileNameWithoutExtension),
        //            fileNameWithPath = Path.Combine(filesDirectory, FileName),
        //            zipFile = Path.ChangeExtension(fileNameWithPath, ".zip");

        //        System.IO.File.Delete(zipFile);

        //        using (ZipFile zip = new ZipFile())
        //        {
        //            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
        //            zip.AddFiles(Directory.GetFiles(filesDirectory), false, "");
        //            zip.ParallelDeflateThreshold = -1;
        //            zip.Save(zipFile);
        //        }


        //        if (GetWorkspaceLayers(WorkspaceName).Contains(LayerName))
        //        {
        //            throw new Exception($"Layer {LayerName} is already exist in {WorkspaceName} workspace!");
        //        }

        //        if (Path.GetExtension(FileName).ToLower() != ".shp")
        //        {
        //            throw new Exception("File extension must be \"shp\"!");
        //        }

        //        string s1 = $" -v -u " +
        //            $"{Startup.Configuration["GeoServer:User"]}:" +
        //            $"{Startup.Configuration["GeoServer:Password"]}" +
        //            $" -XPOST" +
        //            $" -H \"Content-type: text/xml\"" +

        //            $" -d \"<dataStore><name>{LayerName}</name><type>Shapefile</type><enabled>true</enabled><workspace><name>{WorkspaceName}</name><atom:link type=\\\"application/xml\\\" href=\\\"http://localhost:8080/geoserver/rest/workspaces/" +
        //            $"{WorkspaceName}" +
        //            $".xml\\\" rel=\\\"alternate\\\" xmlns:atom=\\\"http://www.w3.org/2005/Atom\\\"/></workspace><connectionParameters><entry key=\\\"namespace\\\">http://{WorkspaceName}</entry><entry key=\\\"url\\\">file:/{System.Web.HttpUtility.UrlPathEncode(filesDirectory)}/</entry></connectionParameters><__default>false</__default><featureTypes><atom:link type=\\\"application/xml\\\" href=\\\"http://localhost:8080/geoserver/rest/workspaces/" +
        //            $"WorkspaceName" +
        //            $"/datastores/{LayerName}/featuretypes.xml\\\" rel=\\\"alternate\\\" xmlns:atom=\\\"http://www.w3.org/2005/Atom\\\"/></featureTypes></dataStore>\"" +

        //            $" \"http://{Startup.Configuration["GeoServer:Address"]}" +
        //            $":{Startup.Configuration["GeoServer:Port"]}/geoserver/rest/workspaces/{WorkspaceName}/datastores\"",
        //            s2 = $" -u " +
        //            $"{Startup.Configuration["GeoServer:User"]}:" +
        //            $"{Startup.Configuration["GeoServer:Password"]}" +
        //            $" -v -XPOST" +
        //            $" -H \"Content-type: text/xml\"" +
        //            $" -d \"<featureType><name>{LayerName}</name>" +
        //            $"<title>{LayerName}</title>" +
        //            $"<nativeCRS>EPSG:3857</nativeCRS>" +
        //            $"<srs>EPSG:3857</srs>" +
        //            $"<projectionPolicy>FORCE_DECLARED</projectionPolicy>" +
        //            $"<defaultInterpolationMethod><name>nearest neighbor</name></defaultInterpolationMethod></featureType>\"" +
        //            $" \\ \"http://{Startup.Configuration["GeoServer:Address"]}" +
        //            $":{Startup.Configuration["GeoServer:Port"]}/geoserver/rest/workspaces/{WorkspaceName}/datastores/{LayerName}/featuretypes?recalculate=nativebbox\"",
        //            s3 = $" -v -u " +
        //            $"{Startup.Configuration["GeoServer:User"]}:" +
        //            $"{Startup.Configuration["GeoServer:Password"]}" +
        //            $" -XPUT" +
        //            $" -H \"Content-type: text/xml\"" +
        //            $" -d \"<layer><defaultStyle><name>{WorkspaceName}:{Style}</name></defaultStyle></layer>\"" +
        //            $" http://{Startup.Configuration["GeoServer:Address"]}" +
        //            $":{Startup.Configuration["GeoServer:Port"]}/geoserver/rest/layers/{WorkspaceName}:{LayerName}";

        //        Process process1 = CurlExecute($" -v -u " +
        //            $"{Startup.Configuration["GeoServer:User"]}:" +
        //            $"{Startup.Configuration["GeoServer:Password"]}" +
        //            $" -XPOST" +
        //            $" -H \"Content-type: text/xml\"" +
        //            $" -d <dataStore><name>{LayerName}</name><type>Shapefile</type><enabled>true</enabled><workspace><name>{WorkspaceName}</name><atom:link type=\"application/xml\" href=\"http://localhost:8080/geoserver/rest/workspaces/" +
        //            $"{WorkspaceName}" +
        //            $".xml\" rel=\"alternate\" xmlns:atom=\"http://www.w3.org/2005/Atom\"/></workspace><connectionParameters><entry key=\"namespace\">http://{WorkspaceName}</entry><entry key=\"url\">file:/{System.Web.HttpUtility.UrlPathEncode(filesDirectory)}/</entry></connectionParameters><__default>false</__default><featureTypes><atom:link type=\"application/xml\" href=\"http://localhost:8080/geoserver/rest/workspaces/" +
        //            $"WorkspaceName" +
        //            $"/datastores/{LayerName}/featuretypes.xml\" rel=\"alternate\" xmlns:atom=\"http://www.w3.org/2005/Atom\"/></featureTypes></dataStore>" +
        //            $" \"http://{Startup.Configuration["GeoServer:Address"]}" +
        //            $":{Startup.Configuration["GeoServer:Port"]}/geoserver/rest/workspaces/{WorkspaceName}/datastores\"");
        //        process1.WaitForExit();
        //        Process process2 = CurlExecute($" -u " +
        //            $"{Startup.Configuration["GeoServer:User"]}:" +
        //            $"{Startup.Configuration["GeoServer:Password"]}" +
        //            $" -v -XPOST" +
        //            $" -H \"Content-type: text/xml\"" +
        //            $" -d \"<featureType><name>{LayerName}</name>" +
        //            $"<title>{LayerName}</title>" +
        //            $"<nativeCRS>EPSG:3857</nativeCRS>" +
        //            $"<srs>EPSG:3857</srs>" +
        //            $"<projectionPolicy>FORCE_DECLARED</projectionPolicy>" +
        //            $"<defaultInterpolationMethod><name>nearest neighbor</name></defaultInterpolationMethod></featureType>\"" +
        //            $" \\ \"http://{Startup.Configuration["GeoServer:Address"]}" +
        //            $":{Startup.Configuration["GeoServer:Port"]}/geoserver/rest/workspaces/{WorkspaceName}/datastores/{LayerName}/featuretypes?recalculate=nativebbox\"");
        //        process2.WaitForExit();
        //        Process process3 = CurlExecute($" -v -u " +
        //            $"{Startup.Configuration["GeoServer:User"]}:" +
        //            $"{Startup.Configuration["GeoServer:Password"]}" +
        //            $" -XPUT" +
        //            $" -H \"Content-type: text/xml\"" +
        //            $" -d \"<layer><defaultStyle><name>{WorkspaceName}:{Style}</name></defaultStyle></layer>\"" +
        //            $" http://{Startup.Configuration["GeoServer:Address"]}" +
        //            $":{Startup.Configuration["GeoServer:Port"]}/geoserver/rest/layers/{WorkspaceName}:{LayerName}");
        //    }
        //    catch (Exception exception)
        //    {
        //        throw new Exception(exception.ToString(), exception.InnerException);
        //    }
        //}

        /// <summary>
        /// Отмена публикации Shape-файла в GeoServer
        /// </summary>
        /// <remarks>
        /// Имя хранилища слоя GeoServer должно совпадать с именем слоя GeoServer
        /// </remarks>
        /// <param name="WorkspaceName">
        /// Рабочая область GeoServer, в которой отменяется публикация слоя
        /// </param>
        /// <param name="LayerName">
        /// Имя слоя GeoServer, отмена публикации которого происходит
        /// </param>
        private void UnpublishShape(string WorkspaceName, string LayerName)
        {
            try
            {
                if (!GetWorkspaceLayers(WorkspaceName).Contains(LayerName))
                {
                    //throw new Exception($"Layer {LayerName} isn't exist in {WorkspaceName} workspace!");
                    throw new Exception(string.Format(_sharedLocalizer["MessageLayerIsntExist"], LayerName, WorkspaceName));
                }

                Process process1 = CurlExecute($" -u " +
                    $"{Startup.Configuration["GeoServer:User"]}:" +
                    $"{Startup.Configuration["GeoServer:Password"]}" +
                    $" -v -XDELETE" +
                    $" http://{Startup.Configuration["GeoServer:Address"]}:" +
                    $"{Startup.Configuration["GeoServer:Port"]}/" +
                    $"geoserver/rest/layers/{LayerName}");
                process1.WaitForExit();
                Process process2 = CurlExecute($" -u " +
                    $"{Startup.Configuration["GeoServer:User"]}:" +
                    $"{Startup.Configuration["GeoServer:Password"]}" +
                    $" -v -XDELETE" +
                    $" http://{Startup.Configuration["GeoServer:Address"]}" +
                    $":{Startup.Configuration["GeoServer:Port"]}/" +
                    $"geoserver/rest/workspaces/{WorkspaceName}/datastores/{LayerName}/featuretypes/{LayerName}");
                process2.WaitForExit();
                Process process3 = CurlExecute($" -v -u " +
                    $"{Startup.Configuration["GeoServer:User"]}:" +
                    $"{Startup.Configuration["GeoServer:Password"]}" +
                    $" -XDELETE" +
                    $" http://{Startup.Configuration["GeoServer:Address"]}" +
                    $":{Startup.Configuration["GeoServer:Port"]}/" +
                    $"geoserver/rest/workspaces/{WorkspaceName}/datastores/{LayerName}");
                process3.WaitForExit();
            }
            catch (Exception exception)
            {
                throw new Exception(exception.ToString(), exception.InnerException);
            }
        }

        private void FindKATO(string LayerFile)
        {
            _context.LakeKATO.RemoveRange(_context.LakeKATO);
            _context.SaveChanges();
            int[] lakesIds = _GDAL.GetShpColumnValues(LayerFile, Startup.Configuration["Lakes:IdField"]);
            // one thread
            //foreach (int lakeId in lakesIds)
            //{
            //    string geometry = _GDAL.GetGeometry(LayerFile, Startup.Configuration["Lakes:IdField"], lakeId.ToString());
            //    string[] katoes1 = _GDAL.GetFeatureCrossFeatures(Startup.Configuration["KATO:Adm1File"], Startup.Configuration["KATO:KATOField"], geometry),
            //        katoes2 = _GDAL.GetFeatureCrossFeatures(Startup.Configuration["KATO:Adm2File"], Startup.Configuration["KATO:KATOField"], geometry),
            //        katoes3 = _GDAL.GetFeatureCrossFeatures(Startup.Configuration["KATO:Adm3File"], Startup.Configuration["KATO:KATOField"], geometry);
            //    foreach (string kato in katoes1)
            //    {
            //        _context.LakeKATO.Add(new LakeKATO()
            //        {
            //            LakeId = lakeId,
            //            KATOId = _context.KATO.FirstOrDefault(k => k.Number == kato).Id
            //        });
            //    }
            //    foreach (string kato in katoes2)
            //    {
            //        _context.LakeKATO.Add(new LakeKATO()
            //        {
            //            LakeId = lakeId,
            //            KATOId = _context.KATO.FirstOrDefault(k => k.Number == kato).Id
            //        });
            //    }
            //    foreach (string kato in katoes3)
            //    {
            //        _context.LakeKATO.Add(new LakeKATO()
            //        {
            //            LakeId = lakeId,
            //            KATOId = _context.KATO.FirstOrDefault(k => k.Number == kato).Id
            //        });
            //    }
            //}
            //_context.SaveChanges();
            // multi thread
            foreach (int lakeId in lakesIds)
            {
                Task task = Task.Run(() =>
                {
                    var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
                    optionsBuilder.UseNpgsql(Startup.Configuration["ConnectionStrings:DefaultConnection"]);

                    using (var _taskContext = new ApplicationDbContext(optionsBuilder.Options))
                    {
                        string geometry = _GDAL.GetGeometry(LayerFile, Startup.Configuration["Lakes:IdField"], lakeId.ToString());
                        string[] katoes1 = _GDAL.GetFeatureCrossFeatures(Startup.Configuration["KATO:Adm1File"], Startup.Configuration["KATO:KATOField"], geometry),
                            katoes2 = _GDAL.GetFeatureCrossFeatures(Startup.Configuration["KATO:Adm2File"], Startup.Configuration["KATO:KATOField"], geometry),
                            katoes3 = _GDAL.GetFeatureCrossFeatures(Startup.Configuration["KATO:Adm3File"], Startup.Configuration["KATO:KATOField"], geometry);

                        foreach (string kato in katoes1)
                        {
                            _taskContext.LakeKATO.Add(new LakeKATO()
                            {
                                LakeId = lakeId,
                                KATOId = _taskContext.KATO.FirstOrDefault(k => k.Number == kato).Id
                            });
                        }
                        foreach (string kato in katoes2)
                        {
                            _taskContext.LakeKATO.Add(new LakeKATO()
                            {
                                LakeId = lakeId,
                                KATOId = _taskContext.KATO.FirstOrDefault(k => k.Number == kato).Id
                            });
                        }
                        foreach (string kato in katoes3)
                        {
                            _taskContext.LakeKATO.Add(new LakeKATO()
                            {
                                LakeId = lakeId,
                                KATOId = _taskContext.KATO.FirstOrDefault(k => k.Number == kato).Id
                            });
                        }
                        _taskContext.SaveChanges();
                    }
                });
            }
        }

        private int[] FindLakeIdsInKATO(string KATONumber, int KATOLevel)
        {
            string KATOFile = Startup.Configuration["KATO:Adm1File"];
            if(KATOLevel == 2)
            {
                KATOFile = Startup.Configuration["KATO:Adm2File"];
            }
            if(KATOLevel == 3)
            {
                KATOFile = Startup.Configuration["KATO:Adm3File"];
            }
            string geometry = _GDAL.GetGeometry(KATOFile, Startup.Configuration["KATO:KATOField"], KATONumber);
            string lakesFile = _context.Layer.FirstOrDefault(l => l.Lake).FileNameWithPath;
            if(string.IsNullOrEmpty(lakesFile))
            {
                return new int[0];
            }
            string[] lakesIdsS = _GDAL.GetFeatureCrossFeatures(lakesFile, Startup.Configuration["Lakes:IdField"], geometry);
            int[] lakesIds = new int[lakesIdsS.Count()];
            for(int i=0;i<lakesIdsS.Count();i++)
            {
                lakesIds[i] = Convert.ToInt32(lakesIdsS[i]);
            }
            return lakesIds;
        }

        public Lake[] FindLakesInKATO(string KATONumber, int KATOLevel)
        {
            int[] lakesIds = FindLakeIdsInKATO(KATONumber, KATOLevel);
            string lakesFile = _context.Layer.FirstOrDefault(l => l.Lake).FileNameWithPath;
            if (string.IsNullOrEmpty(lakesFile))
            {
                return new Lake[0];
            }
            List<Lake> lakes = new List<Lake>();
            foreach(int lakeId in lakesIds)
            {
                lakes.Add(new Lake() {
                    Id = lakeId,
                    Name = _GDAL.GetFeatureValue(lakesFile, Startup.Configuration["Lakes:IdField"], lakeId.ToString(), Startup.Configuration["Lakes:NameField"])
                });
            }
            return lakes.ToArray();
        }
        //===========================================================================================================================================================================
        /// <summary>
        /// Загрузка GeoTIFF-файлов в папку рабочей области GeoServer (Get)
        /// </summary>
        /// <remarks>
        /// Рабочая область всегда "ELake"
        /// Файлы могут быть разделены на zip-архивы
        /// </remarks>
        [DisableRequestSizeLimit]
        //[RequestSizeLimit(long.MaxValue)]
        [Authorize(Roles = "Administrator, Moderator")]
        public IActionResult UploadGeoTIFFFiles()
        {
            return View();
        }

        /// <summary>
        /// Загрузка GeoTIFF-файлов в папку рабочей области GeoServer (Post)
        /// </summary>
        /// <remarks>
        /// Рабочая область всегда "ELake"
        /// Файлы могут быть разделены на zip-архивы
        /// </remarks>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [DisableRequestSizeLimit]
        //[RequestSizeLimit(long.MaxValue)]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<string> UploadGeoTIFFFiles(List<IFormFile> Files)
        {
            string message = _sharedLocalizer["FilesUploaded"];
            try
            {
                message = string.Join("<br/>", UploadGeoTIFFFiles(Startup.Configuration["GeoServer:Workspace"], Files));
            }
            catch (Exception exception)
            {
                message = $"{exception.ToString()}. {exception.InnerException?.Message}";
            }
            return message;
        }

        [DisableRequestSizeLimit]
        //[RequestSizeLimit(long.MaxValue)]
        [Authorize(Roles = "Administrator, Moderator")]
        public IActionResult UploadGeoTIFFFilesWater()
        {
            ViewBag.Folder = new List<SelectListItem>()
            {
                new SelectListItem() { Text=_sharedLocalizer["MonthlyHistory"], Value="MonthlyHistory"},
                new SelectListItem() { Text=_sharedLocalizer["YearlyHistory"], Value="YearlyHistory"}
            };
            ViewBag.Year = new SelectList(Enumerable.Range(Convert.ToInt32(Startup.Configuration["MinYear"]),
                (DateTime.Now.Year - Convert.ToInt32(Startup.Configuration["MinYear"])) + 1), DateTime.Now.Year);
            ViewBag.Month = new SelectList(Enumerable.Range(1, 12));
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [DisableRequestSizeLimit]
        //[RequestSizeLimit(long.MaxValue)]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<string> UploadGeoTIFFFilesWater(List<IFormFile> Files, string Folder, int Year, int Month)
        {
            ViewBag.Folder = new List<SelectListItem>()
            {
                new SelectListItem() { Text=_sharedLocalizer["MonthlyHistory"], Value="MonthlyHistory"},
                new SelectListItem() { Text=_sharedLocalizer["YearlyHistory"], Value="YearlyHistory"}
            };
            ViewBag.Year = new SelectList(Enumerable.Range(Convert.ToInt32(Startup.Configuration["MinYear"]),
                (DateTime.Now.Year - Convert.ToInt32(Startup.Configuration["MinYear"])) + 1), DateTime.Now.Year);
            ViewBag.Month = new SelectList(Enumerable.Range(1, 12));

            string message = _sharedLocalizer["FilesUploaded"];
            try
            {
                message = string.Join("<br/>", UploadGeoTIFFFilesWater(Startup.Configuration["GeoServer:Workspace"], Folder, Year, Month, Files));
            }
            catch (Exception exception)
            {
                message = $"{exception.ToString()}. {exception.InnerException?.Message}";
            }
            return message;
        }

        //[Authorize(Roles = "Administrator, Moderator")]
        //public IActionResult UploadGeoTIFFFiles_()
        //{
        //    return View();
        //}

        //[HttpPost]
        //[DisableFormValueModelBinding]
        //public async Task<IActionResult> UploadGeoTIFFFiles_(string username)
        //{
        //    FormValueProvider formModel;
        //    using (var stream = System.IO.File.Create("d:\\temp\\myfile.temp"))
        //    {
        //        formModel = await Request.StreamFile(stream);
        //    }

        //    //FormValueProvider formModel;
        //    ////var webRoot = _env.ContentRootPath;
        //    //var uploadPath = "d:\\temp";
        //    //var filename = Request.Form.Files[0].FileName;
        //    //using (var stream = System.IO.File.Create(uploadPath + "\\" + filename))
        //    //{
        //    //    formModel = await Request.StreamFile(stream);
        //    //}



        //    var viewModel = new MyViewModel();

        //    var bindingSuccessful = await TryUpdateModelAsync(viewModel, prefix: "",
        //       valueProvider: formModel);

        //    if (!bindingSuccessful)
        //    {
        //        if (!ModelState.IsValid)
        //        {
        //            return BadRequest(ModelState);
        //        }
        //    }

        //    return Ok(viewModel);
        //}

        /// <summary>
        /// Удаление GeoTIFF-файлов из рабочей области GeoServer (Get)
        /// </summary>
        /// <remarks>
        /// Рабочая область всегда "ELake"
        /// </remarks>
        [Authorize(Roles = "Administrator, Moderator")]
        public IActionResult DeleteGeoTIFFFile()
        {
            ViewBag.GeoTIFFFiles = new SelectList(GetGeoTIFFFiles(Startup.Configuration["GeoServer:Workspace"]));
            return View();
        }

        /// <summary>
        /// Удаление GeoTIFF-файлов из рабочей области GeoServer (Post)
        /// </summary>
        /// <remarks>
        /// Рабочая область всегда "ELake"
        /// </remarks>
        [HttpPost]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> DeleteGeoTIFFFile(string GeoTIFFFile)
        {
            ViewData["Message"] = "";
            try
            {
                DeleteGeoTIFFFile(Startup.Configuration["GeoServer:Workspace"], GeoTIFFFile);
                //ViewData["Message"] = $"File {GeoTIFFFile} was deleted!";
                ViewData["Message"] = string.Format(_sharedLocalizer["MessageFileWasDeleted"], GeoTIFFFile);
            }
            catch (Exception exception)
            {
                ViewData["Message"] = $"{exception.ToString()}. {exception.InnerException?.Message}";
            }
            ViewBag.GeoTIFFFiles = new SelectList(GetGeoTIFFFiles(Startup.Configuration["GeoServer:Workspace"]));
            return View();
        }

        /// <summary>
        /// Загрузка Shape-файлов в папку рабочей области GeoServer (Get)
        /// </summary>
        /// <remarks>
        /// Рабочая область всегда "ELake"
        /// Файлы могут быть разделены на zip-архивы
        /// </remarks>
        [DisableRequestSizeLimit]
        [RequestSizeLimit(long.MaxValue)]
        [Authorize(Roles = "Administrator, Moderator")]
        public IActionResult UploadShapeFiles()
        {
            return View();
        }

        /// <summary>
        /// Загрузка GeoTIFF-файлов в папку рабочей области GeoServer (Post)
        /// </summary>
        /// <remarks>
        /// Рабочая область всегда "ELake"
        /// Большие файлы могут быть разделены на zip-архивы
        /// </remarks>
        [HttpPost]
        [ValidateAntiForgeryToken]
        [DisableRequestSizeLimit]
        [RequestSizeLimit(long.MaxValue)]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<string> UploadShapeFiles(List<IFormFile> Files)
        {
            string message = _sharedLocalizer["FilesUploaded"];
            try
            {
                message = string.Join("<br/>", UploadShapeFiles(Startup.Configuration["GeoServer:Workspace"], Files));
            }
            catch (Exception exception)
            {
                message = $"{exception.ToString()}. {exception.InnerException?.Message}";
            }
            return message;
        }

        /// <summary>
        /// Публикация GeoTIFF-файла в рабочей области ELake (Get)
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Administrator, Moderator")]
        public IActionResult PublishGeoTIFF()
        {
            var publicshedLayers = GetWorkspaceLayers(Startup.Configuration["GeoServer:Workspace"]);
            ViewBag.GeoTIFFFiles = new SelectList(GetGeoTIFFFiles(Startup.Configuration["GeoServer:Workspace"])
                .Where(l => !publicshedLayers.Contains(Path.GetFileNameWithoutExtension(l))));
            ViewBag.Styles = new SelectList(GetWorkspaceStyles(Startup.Configuration["GeoServer:Workspace"]));
            return View();
        }

        /// <summary>
        /// Публикация GeoTIFF-файла в рабочей области ELake (Post)
        /// </summary>
        /// <param name="GeoTIFFFile">
        /// Имя GeoTIFF-файла с расширенем, без пути
        /// </param>
        /// <param name="Style"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> PublishGeoTIFF(string GeoTIFFFile, string Style, string NameKK, string NameRU, string NameEN, string Tags,
            string DescriptionKK,
            string DescriptionRU,
            string DescriptionEN)
        {
            string message = "";
            try
            {
                PublishGeoTIFF(Startup.Configuration["GeoServer:Workspace"], GeoTIFFFile, Style);
                string xml = GeoTIFFFile + ".xml";
                xml = Path.Combine(GetWorkspaceDirectoryPath(Startup.Configuration["GeoServer:Workspace"]), xml);
                Layer layer = new Layer() {
                    NameKK = NameKK,
                    NameRU = NameRU,
                    NameEN = NameEN,
                    GeoServerStyle = Style,
                    Tags = Tags,
                    DescriptionKK = DescriptionKK,
                    DescriptionRU = DescriptionRU,
                    DescriptionEN = DescriptionEN,
                    GeoServerName = Path.GetFileNameWithoutExtension(GeoTIFFFile),
                    FileNameWithPath = Path.Combine(GetWorkspaceDirectoryPath(Startup.Configuration["GeoServer:Workspace"]), GeoTIFFFile),
                    MetaData = _GDAL.GetLayerMetaData(xml)
                };
                _context.Add(layer);
                await _context.SaveChangesAsync();
            }
            catch (Exception exception)
            {
                message = $"{exception.ToString()}. {exception.InnerException?.Message}";
            }
            var publicshedLayers = GetWorkspaceLayers(Startup.Configuration["GeoServer:Workspace"]);
            ViewBag.GeoTIFFFiles = new SelectList(GetGeoTIFFFiles(Startup.Configuration["GeoServer:Workspace"])
                .Where(l => !publicshedLayers.Contains(Path.GetFileNameWithoutExtension(l))));
            ViewBag.Styles = new SelectList(GetWorkspaceStyles(Startup.Configuration["GeoServer:Workspace"]));
            ViewBag.Message = message;
            return View();
        }

        /// <summary>
        /// Отмена публикации GeoTIFF-файла в рабочей области ELake (Get)
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Administrator, Moderator")]
        public IActionResult UnpublishGeoTIFF()
        {
            ViewBag.GeoTIFFLayers = new SelectList(GetWorkspaceGeoTIFFLayers(Startup.Configuration["GeoServer:Workspace"]));
            return View();
        }

        /// <summary>
        /// Отмена публикации GeoTIFF-файла в рабочей области ELake (Get)
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Administrator, Moderator")]
        public IActionResult UnpublishGeoTIFF(string LayerName)
        {
            ViewData["Message"] = "";
            try
            {
                UnpublishGeoTIFF(Startup.Configuration["GeoServer:Workspace"], LayerName);
                var layer = _context.Layer.SingleOrDefault(m => m.GeoServerName == LayerName);
                if (layer != null)
                {
                    _context.Layer.Remove(layer);
                    _context.SaveChanges();
                }
            }
            catch (Exception exception)
            {
                ViewData["Message"] = $"{exception.ToString()}. {exception.InnerException?.Message}";
            }
            ViewBag.GeoTIFFLayers = new SelectList(GetWorkspaceGeoTIFFLayers(Startup.Configuration["GeoServer:Workspace"]));
            return View();
        }

        /// <summary>
        /// Удаление Shape-файлов из рабочей области GeoServer (Get)
        /// </summary>
        /// <remarks>
        /// Рабочая область всегда "ELake"
        /// </remarks>
        [Authorize(Roles = "Administrator, Moderator")]
        public IActionResult DeleteShapeFile()
        {
            ViewBag.ShapeFiles = new SelectList(GetShapeFiles(Startup.Configuration["GeoServer:Workspace"]));
            return View();
        }

        /// <summary>
        /// Удаление GeoTIFF-файлов из рабочей области GeoServer (Post)
        /// </summary>
        /// <remarks>
        /// Рабочая область всегда "ELake"
        /// </remarks>
        [HttpPost]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> DeleteShapeFile(string ShapeFile)
        {
            ViewData["Message"] = "";
            try
            {
                DeleteShapeFile(Startup.Configuration["GeoServer:Workspace"], ShapeFile);
                //ViewData["Message"] = $"File {ShapeFile} was deleted!";
                ViewData["Message"] = string.Format(_sharedLocalizer["MessageFileWasDeleted"], ShapeFile);
            }
            catch (Exception exception)
            {
                ViewData["Message"] = $"{exception.ToString()}. {exception.InnerException?.Message}";
            }
            ViewBag.ShapeFiles = new SelectList(GetShapeFiles(Startup.Configuration["GeoServer:Workspace"]));
            return View();
        }

        [DisableRequestSizeLimit]
        [RequestSizeLimit(long.MaxValue)]
        [Authorize(Roles = "Administrator, Moderator")]
        public IActionResult PublishShape()
        {
            ViewBag.Styles = new SelectList(GetWorkspaceStyles(Startup.Configuration["GeoServer:Workspace"]));
            //ViewBag.ShapeFiles = new SelectList(GetShapeFiles(Startup.Configuration["GeoServer:Workspace"]));
            var publicshedLayers = GetWorkspaceLayers(Startup.Configuration["GeoServer:Workspace"]);
            ViewBag.ShapeFiles = new SelectList(GetShapeFiles(Startup.Configuration["GeoServer:Workspace"])
                .Where(l => !publicshedLayers.Contains(Path.GetFileNameWithoutExtension(l))));

            Layer layer = new Layer() {
                LayerIntervalsWaterLevel = new List<LayerInterval>()
                {
                    new LayerInterval()
                    {
                        MinValue = 0,
                        Color = "#ffffff"
                    }
                },
                LayerIntervalsSurfaceFlow = new List<LayerInterval>()
                {
                    new LayerInterval()
                    {
                        MinValue = 0,
                        Color = "#ffffff"
                    }
                },
                LayerIntervalsPrecipitation = new List<LayerInterval>()
                {
                    new LayerInterval()
                    {
                        MinValue = 0,
                        Color = "#ffffff"
                    }
                },
                LayerIntervalsUndergroundFlow = new List<LayerInterval>()
                {
                    new LayerInterval()
                    {
                        MinValue = 0,
                        Color = "#ffffff"
                    }
                },
                LayerIntervalsSurfaceOutflow = new List<LayerInterval>()
                {
                    new LayerInterval()
                    {
                        MinValue = 0,
                        Color = "#ffffff"
                    }
                },
                LayerIntervalsEvaporation = new List<LayerInterval>()
                {
                    new LayerInterval()
                    {
                        MinValue = 0,
                        Color = "#ffffff"
                    }
                },
                LayerIntervalsUndergroundOutflow = new List<LayerInterval>()
                {
                    new LayerInterval()
                    {
                        MinValue = 0,
                        Color = "#ffffff"
                    }
                },
                LayerIntervalsHydrochemistryMineralization = new List<LayerInterval>()
                {
                    new LayerInterval()
                    {
                        MinValue = 0,
                        Color = "#ffffff"
                    }
                },
                LayerIntervalsHydrochemistryTotalHardness = new List<LayerInterval>()
                {
                    new LayerInterval()
                    {
                        MinValue = 0,
                        Color = "#ffffff"
                    }
                },
                LayerIntervalsHydrochemistryDissOxygWater = new List<LayerInterval>()
                {
                    new LayerInterval()
                    {
                        MinValue = 0,
                        Color = "#ffffff"
                    }
                },
                LayerIntervalsHydrochemistryPercentOxygWater = new List<LayerInterval>()
                {
                    new LayerInterval()
                    {
                        MinValue = 0,
                        Color = "#ffffff"
                    }
                },
                LayerIntervalsHydrochemistrypH = new List<LayerInterval>()
                {
                    new LayerInterval()
                    {
                        MinValue = 0,
                        Color = "#ffffff"
                    }
                },
                LayerIntervalsHydrochemistryOrganicSubstances = new List<LayerInterval>()
                {
                    new LayerInterval()
                    {
                        MinValue = 0,
                        Color = "#ffffff"
                    }
                },
                LayerIntervalsHydrochemistryCa = new List<LayerInterval>()
                {
                    new LayerInterval()
                    {
                        MinValue = 0,
                        Color = "#ffffff"
                    }
                },
                LayerIntervalsHydrochemistryMg = new List<LayerInterval>()
                {
                    new LayerInterval()
                    {
                        MinValue = 0,
                        Color = "#ffffff"
                    }
                },
                LayerIntervalsHydrochemistryNaK = new List<LayerInterval>()
                {
                    new LayerInterval()
                    {
                        MinValue = 0,
                        Color = "#ffffff"
                    }
                },
                LayerIntervalsHydrochemistryCl = new List<LayerInterval>()
                {
                    new LayerInterval()
                    {
                        MinValue = 0,
                        Color = "#ffffff"
                    }
                },
                LayerIntervalsHydrochemistryHCO = new List<LayerInterval>()
                {
                    new LayerInterval()
                    {
                        MinValue = 0,
                        Color = "#ffffff"
                    }
                },
                LayerIntervalsHydrochemistrySO = new List<LayerInterval>()
                {
                    new LayerInterval()
                    {
                        MinValue = 0,
                        Color = "#ffffff"
                    }
                },
                LayerIntervalsHydrochemistryNH = new List<LayerInterval>()
                {
                    new LayerInterval()
                    {
                        MinValue = 0,
                        Color = "#ffffff"
                    }
                },
                LayerIntervalsHydrochemistryNO2 = new List<LayerInterval>()
                {
                    new LayerInterval()
                    {
                        MinValue = 0,
                        Color = "#ffffff"
                    }
                },
                LayerIntervalsHydrochemistryNO3 = new List<LayerInterval>()
                {
                    new LayerInterval()
                    {
                        MinValue = 0,
                        Color = "#ffffff"
                    }
                },
                LayerIntervalsHydrochemistryPPO = new List<LayerInterval>()
                {
                    new LayerInterval()
                    {
                        MinValue = 0,
                        Color = "#ffffff"
                    }
                },
                LayerIntervalsHydrochemistryCu = new List<LayerInterval>()
                {
                    new LayerInterval()
                    {
                        MinValue = 0,
                        Color = "#ffffff"
                    }
                },
                LayerIntervalsHydrochemistryZn = new List<LayerInterval>()
                {
                    new LayerInterval()
                    {
                        MinValue = 0,
                        Color = "#ffffff"
                    }
                },
                LayerIntervalsHydrochemistryMn = new List<LayerInterval>()
                {
                    new LayerInterval()
                    {
                        MinValue = 0,
                        Color = "#ffffff"
                    }
                },
                LayerIntervalsHydrochemistryPb = new List<LayerInterval>()
                {
                    new LayerInterval()
                    {
                        MinValue = 0,
                        Color = "#ffffff"
                    }
                },
                LayerIntervalsHydrochemistryNi = new List<LayerInterval>()
                {
                    new LayerInterval()
                    {
                        MinValue = 0,
                        Color = "#ffffff"
                    }
                },
                LayerIntervalsHydrochemistryCd = new List<LayerInterval>()
                {
                    new LayerInterval()
                    {
                        MinValue = 0,
                        Color = "#ffffff"
                    }
                },
                LayerIntervalsHydrochemistryCo = new List<LayerInterval>()
                {
                    new LayerInterval()
                    {
                        MinValue = 0,
                        Color = "#ffffff"
                    }
                },
                LayerIntervalsHydrochemistryCIWP = new List<LayerInterval>()
                {
                    new LayerInterval()
                    {
                        MinValue = 0,
                        Color = "#ffffff"
                    }
                }
            };            

            return View(layer);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [DisableRequestSizeLimit]
        [RequestSizeLimit(long.MaxValue)]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> PublishShape(string ShapeFile, string Style, bool AsLakeLayer, string NameKK, string NameRU, string NameEN, string Tags,
            string DescriptionKK,
            string DescriptionRU,
            string DescriptionEN, Layer Layer)
        {
            if (Layer.LayerIntervalsWaterLevel == null)
            {
                Layer.LayerIntervalsWaterLevel = new List<LayerInterval>();
            }
            for (int i = Layer.LayerIntervalsWaterLevel.Count - 1; i >= 0; i--)
            {
                if (string.IsNullOrEmpty(Layer.LayerIntervalsWaterLevel[i].Color))
                {
                    Layer.LayerIntervalsWaterLevel.RemoveAt(i);
                }
            }
            Layer.MinValuesWaterLevel = Layer.LayerIntervalsWaterLevel.Select(m => m.MinValue).ToArray();
            Layer.ColorsWaterLevel = Layer.LayerIntervalsWaterLevel.Select(m => m.Color).ToArray();

            if (Layer.LayerIntervalsSurfaceFlow == null)
            {
                Layer.LayerIntervalsSurfaceFlow = new List<LayerInterval>();
            }
            for (int i = Layer.LayerIntervalsSurfaceFlow.Count - 1; i >= 0; i--)
            {
                if (string.IsNullOrEmpty(Layer.LayerIntervalsSurfaceFlow[i].Color))
                {
                    Layer.LayerIntervalsSurfaceFlow.RemoveAt(i);
                }
            }
            Layer.MinValuesSurfaceFlow = Layer.LayerIntervalsSurfaceFlow.Select(m => m.MinValue).ToArray();
            Layer.ColorsSurfaceFlow = Layer.LayerIntervalsSurfaceFlow.Select(m => m.Color).ToArray();

            if (Layer.LayerIntervalsPrecipitation == null)
            {
                Layer.LayerIntervalsPrecipitation = new List<LayerInterval>();
            }
            for (int i = Layer.LayerIntervalsPrecipitation.Count - 1; i >= 0; i--)
            {
                if (string.IsNullOrEmpty(Layer.LayerIntervalsPrecipitation[i].Color))
                {
                    Layer.LayerIntervalsPrecipitation.RemoveAt(i);
                }
            }
            Layer.MinValuesPrecipitation = Layer.LayerIntervalsPrecipitation.Select(m => m.MinValue).ToArray();
            Layer.ColorsPrecipitation = Layer.LayerIntervalsPrecipitation.Select(m => m.Color).ToArray();

            if (Layer.LayerIntervalsUndergroundFlow == null)
            {
                Layer.LayerIntervalsUndergroundFlow = new List<LayerInterval>();
            }
            for (int i = Layer.LayerIntervalsUndergroundFlow.Count - 1; i >= 0; i--)
            {
                if (string.IsNullOrEmpty(Layer.LayerIntervalsUndergroundFlow[i].Color))
                {
                    Layer.LayerIntervalsUndergroundFlow.RemoveAt(i);
                }
            }
            Layer.MinValuesUndergroundFlow = Layer.LayerIntervalsUndergroundFlow.Select(m => m.MinValue).ToArray();
            Layer.ColorsUndergroundFlow = Layer.LayerIntervalsUndergroundFlow.Select(m => m.Color).ToArray();

            if (Layer.LayerIntervalsSurfaceOutflow == null)
            {
                Layer.LayerIntervalsSurfaceOutflow = new List<LayerInterval>();
            }
            for (int i = Layer.LayerIntervalsSurfaceOutflow.Count - 1; i >= 0; i--)
            {
                if (string.IsNullOrEmpty(Layer.LayerIntervalsSurfaceOutflow[i].Color))
                {
                    Layer.LayerIntervalsSurfaceOutflow.RemoveAt(i);
                }
            }
            Layer.MinValuesSurfaceOutflow = Layer.LayerIntervalsSurfaceOutflow.Select(m => m.MinValue).ToArray();
            Layer.ColorsSurfaceOutflow = Layer.LayerIntervalsSurfaceOutflow.Select(m => m.Color).ToArray();

            if (Layer.LayerIntervalsEvaporation == null)
            {
                Layer.LayerIntervalsEvaporation = new List<LayerInterval>();
            }
            for (int i = Layer.LayerIntervalsEvaporation.Count - 1; i >= 0; i--)
            {
                if (string.IsNullOrEmpty(Layer.LayerIntervalsEvaporation[i].Color))
                {
                    Layer.LayerIntervalsEvaporation.RemoveAt(i);
                }
            }
            Layer.MinValuesEvaporation = Layer.LayerIntervalsEvaporation.Select(m => m.MinValue).ToArray();
            Layer.ColorsEvaporation = Layer.LayerIntervalsEvaporation.Select(m => m.Color).ToArray();

            if (Layer.LayerIntervalsUndergroundOutflow == null)
            {
                Layer.LayerIntervalsUndergroundOutflow = new List<LayerInterval>();
            }
            for (int i = Layer.LayerIntervalsUndergroundOutflow.Count - 1; i >= 0; i--)
            {
                if (string.IsNullOrEmpty(Layer.LayerIntervalsUndergroundOutflow[i].Color))
                {
                    Layer.LayerIntervalsUndergroundOutflow.RemoveAt(i);
                }
            }
            Layer.MinValuesUndergroundOutflow = Layer.LayerIntervalsUndergroundOutflow.Select(m => m.MinValue).ToArray();
            Layer.ColorsUndergroundOutflow = Layer.LayerIntervalsUndergroundOutflow.Select(m => m.Color).ToArray();
                        
            if (Layer.LayerIntervalsHydrochemistryMineralization == null)
            {
                Layer.LayerIntervalsHydrochemistryMineralization = new List<LayerInterval>();
            }
            for (int i = Layer.LayerIntervalsHydrochemistryMineralization.Count - 1; i >= 0; i--)
            {
                if (string.IsNullOrEmpty(Layer.LayerIntervalsHydrochemistryMineralization[i].Color))
                {
                    Layer.LayerIntervalsHydrochemistryMineralization.RemoveAt(i);
                }
            }
            Layer.MinValuesHydrochemistryMineralization = Layer.LayerIntervalsHydrochemistryMineralization.Select(m => m.MinValue).ToArray();
            Layer.ColorsHydrochemistryMineralization = Layer.LayerIntervalsHydrochemistryMineralization.Select(m => m.Color).ToArray();

            if (Layer.LayerIntervalsHydrochemistryTotalHardness == null)
            {
                Layer.LayerIntervalsHydrochemistryTotalHardness = new List<LayerInterval>();
            }
            for (int i = Layer.LayerIntervalsHydrochemistryTotalHardness.Count - 1; i >= 0; i--)
            {
                if (string.IsNullOrEmpty(Layer.LayerIntervalsHydrochemistryTotalHardness[i].Color))
                {
                    Layer.LayerIntervalsHydrochemistryTotalHardness.RemoveAt(i);
                }
            }
            Layer.MinValuesHydrochemistryTotalHardness = Layer.LayerIntervalsHydrochemistryTotalHardness.Select(m => m.MinValue).ToArray();
            Layer.ColorsHydrochemistryTotalHardness = Layer.LayerIntervalsHydrochemistryTotalHardness.Select(m => m.Color).ToArray();

            if (Layer.LayerIntervalsHydrochemistryDissOxygWater == null)
            {
                Layer.LayerIntervalsHydrochemistryDissOxygWater = new List<LayerInterval>();
            }
            for (int i = Layer.LayerIntervalsHydrochemistryDissOxygWater.Count - 1; i >= 0; i--)
            {
                if (string.IsNullOrEmpty(Layer.LayerIntervalsHydrochemistryDissOxygWater[i].Color))
                {
                    Layer.LayerIntervalsHydrochemistryDissOxygWater.RemoveAt(i);
                }
            }
            Layer.MinValuesHydrochemistryDissOxygWater = Layer.LayerIntervalsHydrochemistryDissOxygWater.Select(m => m.MinValue).ToArray();
            Layer.ColorsHydrochemistryDissOxygWater = Layer.LayerIntervalsHydrochemistryDissOxygWater.Select(m => m.Color).ToArray();

            if (Layer.LayerIntervalsHydrochemistryPercentOxygWater == null)
            {
                Layer.LayerIntervalsHydrochemistryPercentOxygWater = new List<LayerInterval>();
            }
            for (int i = Layer.LayerIntervalsHydrochemistryPercentOxygWater.Count - 1; i >= 0; i--)
            {
                if (string.IsNullOrEmpty(Layer.LayerIntervalsHydrochemistryPercentOxygWater[i].Color))
                {
                    Layer.LayerIntervalsHydrochemistryPercentOxygWater.RemoveAt(i);
                }
            }
            Layer.MinValuesHydrochemistryPercentOxygWater = Layer.LayerIntervalsHydrochemistryPercentOxygWater.Select(m => m.MinValue).ToArray();
            Layer.ColorsHydrochemistryPercentOxygWater = Layer.LayerIntervalsHydrochemistryPercentOxygWater.Select(m => m.Color).ToArray();

            if (Layer.LayerIntervalsHydrochemistrypH == null)
            {
                Layer.LayerIntervalsHydrochemistrypH = new List<LayerInterval>();
            }
            for (int i = Layer.LayerIntervalsHydrochemistrypH.Count - 1; i >= 0; i--)
            {
                if (string.IsNullOrEmpty(Layer.LayerIntervalsHydrochemistrypH[i].Color))
                {
                    Layer.LayerIntervalsHydrochemistrypH.RemoveAt(i);
                }
            }
            Layer.MinValuesHydrochemistrypH = Layer.LayerIntervalsHydrochemistrypH.Select(m => m.MinValue).ToArray();
            Layer.ColorsHydrochemistrypH = Layer.LayerIntervalsHydrochemistrypH.Select(m => m.Color).ToArray();

            if (Layer.LayerIntervalsHydrochemistryOrganicSubstances == null)
            {
                Layer.LayerIntervalsHydrochemistryOrganicSubstances = new List<LayerInterval>();
            }
            for (int i = Layer.LayerIntervalsHydrochemistryOrganicSubstances.Count - 1; i >= 0; i--)
            {
                if (string.IsNullOrEmpty(Layer.LayerIntervalsHydrochemistryOrganicSubstances[i].Color))
                {
                    Layer.LayerIntervalsHydrochemistryOrganicSubstances.RemoveAt(i);
                }
            }
            Layer.MinValuesHydrochemistryOrganicSubstances = Layer.LayerIntervalsHydrochemistryOrganicSubstances.Select(m => m.MinValue).ToArray();
            Layer.ColorsHydrochemistryOrganicSubstances = Layer.LayerIntervalsHydrochemistryOrganicSubstances.Select(m => m.Color).ToArray();

            if (Layer.LayerIntervalsHydrochemistryCa == null)
            {
                Layer.LayerIntervalsHydrochemistryCa = new List<LayerInterval>();
            }
            for (int i = Layer.LayerIntervalsHydrochemistryCa.Count - 1; i >= 0; i--)
            {
                if (string.IsNullOrEmpty(Layer.LayerIntervalsHydrochemistryCa[i].Color))
                {
                    Layer.LayerIntervalsHydrochemistryCa.RemoveAt(i);
                }
            }
            Layer.MinValuesHydrochemistryCa = Layer.LayerIntervalsHydrochemistryCa.Select(m => m.MinValue).ToArray();
            Layer.ColorsHydrochemistryCa = Layer.LayerIntervalsHydrochemistryCa.Select(m => m.Color).ToArray();

            if (Layer.LayerIntervalsHydrochemistryMg == null)
            {
                Layer.LayerIntervalsHydrochemistryMg = new List<LayerInterval>();
            }
            for (int i = Layer.LayerIntervalsHydrochemistryMg.Count - 1; i >= 0; i--)
            {
                if (string.IsNullOrEmpty(Layer.LayerIntervalsHydrochemistryMg[i].Color))
                {
                    Layer.LayerIntervalsHydrochemistryMg.RemoveAt(i);
                }
            }
            Layer.MinValuesHydrochemistryMg = Layer.LayerIntervalsHydrochemistryMg.Select(m => m.MinValue).ToArray();
            Layer.ColorsHydrochemistryMg = Layer.LayerIntervalsHydrochemistryMg.Select(m => m.Color).ToArray();

            if (Layer.LayerIntervalsHydrochemistryNaK == null)
            {
                Layer.LayerIntervalsHydrochemistryNaK = new List<LayerInterval>();
            }
            for (int i = Layer.LayerIntervalsHydrochemistryNaK.Count - 1; i >= 0; i--)
            {
                if (string.IsNullOrEmpty(Layer.LayerIntervalsHydrochemistryNaK[i].Color))
                {
                    Layer.LayerIntervalsHydrochemistryNaK.RemoveAt(i);
                }
            }
            Layer.MinValuesHydrochemistryNaK = Layer.LayerIntervalsHydrochemistryNaK.Select(m => m.MinValue).ToArray();
            Layer.ColorsHydrochemistryNaK = Layer.LayerIntervalsHydrochemistryNaK.Select(m => m.Color).ToArray();

            if (Layer.LayerIntervalsHydrochemistryCl == null)
            {
                Layer.LayerIntervalsHydrochemistryCl = new List<LayerInterval>();
            }
            for (int i = Layer.LayerIntervalsHydrochemistryCl.Count - 1; i >= 0; i--)
            {
                if (string.IsNullOrEmpty(Layer.LayerIntervalsHydrochemistryCl[i].Color))
                {
                    Layer.LayerIntervalsHydrochemistryCl.RemoveAt(i);
                }
            }
            Layer.MinValuesHydrochemistryCl = Layer.LayerIntervalsHydrochemistryCl.Select(m => m.MinValue).ToArray();
            Layer.ColorsHydrochemistryCl = Layer.LayerIntervalsHydrochemistryCl.Select(m => m.Color).ToArray();

            if (Layer.LayerIntervalsHydrochemistryHCO == null)
            {
                Layer.LayerIntervalsHydrochemistryHCO = new List<LayerInterval>();
            }
            for (int i = Layer.LayerIntervalsHydrochemistryHCO.Count - 1; i >= 0; i--)
            {
                if (string.IsNullOrEmpty(Layer.LayerIntervalsHydrochemistryHCO[i].Color))
                {
                    Layer.LayerIntervalsHydrochemistryHCO.RemoveAt(i);
                }
            }
            Layer.MinValuesHydrochemistryHCO = Layer.LayerIntervalsHydrochemistryHCO.Select(m => m.MinValue).ToArray();
            Layer.ColorsHydrochemistryHCO = Layer.LayerIntervalsHydrochemistryHCO.Select(m => m.Color).ToArray();

            if (Layer.LayerIntervalsHydrochemistrySO == null)
            {
                Layer.LayerIntervalsHydrochemistrySO = new List<LayerInterval>();
            }
            for (int i = Layer.LayerIntervalsHydrochemistrySO.Count - 1; i >= 0; i--)
            {
                if (string.IsNullOrEmpty(Layer.LayerIntervalsHydrochemistrySO[i].Color))
                {
                    Layer.LayerIntervalsHydrochemistrySO.RemoveAt(i);
                }
            }
            Layer.MinValuesHydrochemistrySO = Layer.LayerIntervalsHydrochemistrySO.Select(m => m.MinValue).ToArray();
            Layer.ColorsHydrochemistrySO = Layer.LayerIntervalsHydrochemistrySO.Select(m => m.Color).ToArray();

            if (Layer.LayerIntervalsHydrochemistryNH == null)
            {
                Layer.LayerIntervalsHydrochemistryNH = new List<LayerInterval>();
            }
            for (int i = Layer.LayerIntervalsHydrochemistryNH.Count - 1; i >= 0; i--)
            {
                if (string.IsNullOrEmpty(Layer.LayerIntervalsHydrochemistryNH[i].Color))
                {
                    Layer.LayerIntervalsHydrochemistryNH.RemoveAt(i);
                }
            }
            Layer.MinValuesHydrochemistryNH = Layer.LayerIntervalsHydrochemistryNH.Select(m => m.MinValue).ToArray();
            Layer.ColorsHydrochemistryNH = Layer.LayerIntervalsHydrochemistryNH.Select(m => m.Color).ToArray();

            if (Layer.LayerIntervalsHydrochemistryNO2 == null)
            {
                Layer.LayerIntervalsHydrochemistryNO2 = new List<LayerInterval>();
            }
            for (int i = Layer.LayerIntervalsHydrochemistryNO2.Count - 1; i >= 0; i--)
            {
                if (string.IsNullOrEmpty(Layer.LayerIntervalsHydrochemistryNO2[i].Color))
                {
                    Layer.LayerIntervalsHydrochemistryNO2.RemoveAt(i);
                }
            }
            Layer.MinValuesHydrochemistryNO2 = Layer.LayerIntervalsHydrochemistryNO2.Select(m => m.MinValue).ToArray();
            Layer.ColorsHydrochemistryNO2 = Layer.LayerIntervalsHydrochemistryNO2.Select(m => m.Color).ToArray();

            if (Layer.LayerIntervalsHydrochemistryNO3 == null)
            {
                Layer.LayerIntervalsHydrochemistryNO3 = new List<LayerInterval>();
            }
            for (int i = Layer.LayerIntervalsHydrochemistryNO3.Count - 1; i >= 0; i--)
            {
                if (string.IsNullOrEmpty(Layer.LayerIntervalsHydrochemistryNO3[i].Color))
                {
                    Layer.LayerIntervalsHydrochemistryNO3.RemoveAt(i);
                }
            }
            Layer.MinValuesHydrochemistryNO3 = Layer.LayerIntervalsHydrochemistryNO3.Select(m => m.MinValue).ToArray();
            Layer.ColorsHydrochemistryNO3 = Layer.LayerIntervalsHydrochemistryNO3.Select(m => m.Color).ToArray();

            if (Layer.LayerIntervalsHydrochemistryPPO == null)
            {
                Layer.LayerIntervalsHydrochemistryPPO = new List<LayerInterval>();
            }
            for (int i = Layer.LayerIntervalsHydrochemistryPPO.Count - 1; i >= 0; i--)
            {
                if (string.IsNullOrEmpty(Layer.LayerIntervalsHydrochemistryPPO[i].Color))
                {
                    Layer.LayerIntervalsHydrochemistryPPO.RemoveAt(i);
                }
            }
            Layer.MinValuesHydrochemistryPPO = Layer.LayerIntervalsHydrochemistryPPO.Select(m => m.MinValue).ToArray();
            Layer.ColorsHydrochemistryPPO = Layer.LayerIntervalsHydrochemistryPPO.Select(m => m.Color).ToArray();

            if (Layer.LayerIntervalsHydrochemistryCu == null)
            {
                Layer.LayerIntervalsHydrochemistryCu = new List<LayerInterval>();
            }
            for (int i = Layer.LayerIntervalsHydrochemistryCu.Count - 1; i >= 0; i--)
            {
                if (string.IsNullOrEmpty(Layer.LayerIntervalsHydrochemistryCu[i].Color))
                {
                    Layer.LayerIntervalsHydrochemistryCu.RemoveAt(i);
                }
            }
            Layer.MinValuesHydrochemistryCu = Layer.LayerIntervalsHydrochemistryCu.Select(m => m.MinValue).ToArray();
            Layer.ColorsHydrochemistryCu = Layer.LayerIntervalsHydrochemistryCu.Select(m => m.Color).ToArray();

            if (Layer.LayerIntervalsHydrochemistryZn == null)
            {
                Layer.LayerIntervalsHydrochemistryZn = new List<LayerInterval>();
            }
            for (int i = Layer.LayerIntervalsHydrochemistryZn.Count - 1; i >= 0; i--)
            {
                if (string.IsNullOrEmpty(Layer.LayerIntervalsHydrochemistryZn[i].Color))
                {
                    Layer.LayerIntervalsHydrochemistryZn.RemoveAt(i);
                }
            }
            Layer.MinValuesHydrochemistryZn = Layer.LayerIntervalsHydrochemistryZn.Select(m => m.MinValue).ToArray();
            Layer.ColorsHydrochemistryZn = Layer.LayerIntervalsHydrochemistryZn.Select(m => m.Color).ToArray();

            if (Layer.LayerIntervalsHydrochemistryMn == null)
            {
                Layer.LayerIntervalsHydrochemistryMn = new List<LayerInterval>();
            }
            for (int i = Layer.LayerIntervalsHydrochemistryMn.Count - 1; i >= 0; i--)
            {
                if (string.IsNullOrEmpty(Layer.LayerIntervalsHydrochemistryMn[i].Color))
                {
                    Layer.LayerIntervalsHydrochemistryMn.RemoveAt(i);
                }
            }
            Layer.MinValuesHydrochemistryMn = Layer.LayerIntervalsHydrochemistryMn.Select(m => m.MinValue).ToArray();
            Layer.ColorsHydrochemistryMn = Layer.LayerIntervalsHydrochemistryMn.Select(m => m.Color).ToArray();

            if (Layer.LayerIntervalsHydrochemistryPb == null)
            {
                Layer.LayerIntervalsHydrochemistryPb = new List<LayerInterval>();
            }
            for (int i = Layer.LayerIntervalsHydrochemistryPb.Count - 1; i >= 0; i--)
            {
                if (string.IsNullOrEmpty(Layer.LayerIntervalsHydrochemistryPb[i].Color))
                {
                    Layer.LayerIntervalsHydrochemistryPb.RemoveAt(i);
                }
            }
            Layer.MinValuesHydrochemistryPb = Layer.LayerIntervalsHydrochemistryPb.Select(m => m.MinValue).ToArray();
            Layer.ColorsHydrochemistryPb = Layer.LayerIntervalsHydrochemistryPb.Select(m => m.Color).ToArray();

            if (Layer.LayerIntervalsHydrochemistryNi == null)
            {
                Layer.LayerIntervalsHydrochemistryNi = new List<LayerInterval>();
            }
            for (int i = Layer.LayerIntervalsHydrochemistryNi.Count - 1; i >= 0; i--)
            {
                if (string.IsNullOrEmpty(Layer.LayerIntervalsHydrochemistryNi[i].Color))
                {
                    Layer.LayerIntervalsHydrochemistryNi.RemoveAt(i);
                }
            }
            Layer.MinValuesHydrochemistryNi = Layer.LayerIntervalsHydrochemistryNi.Select(m => m.MinValue).ToArray();
            Layer.ColorsHydrochemistryNi = Layer.LayerIntervalsHydrochemistryNi.Select(m => m.Color).ToArray();

            if (Layer.LayerIntervalsHydrochemistryCd == null)
            {
                Layer.LayerIntervalsHydrochemistryCd = new List<LayerInterval>();
            }
            for (int i = Layer.LayerIntervalsHydrochemistryCd.Count - 1; i >= 0; i--)
            {
                if (string.IsNullOrEmpty(Layer.LayerIntervalsHydrochemistryCd[i].Color))
                {
                    Layer.LayerIntervalsHydrochemistryCd.RemoveAt(i);
                }
            }
            Layer.MinValuesHydrochemistryCd = Layer.LayerIntervalsHydrochemistryCd.Select(m => m.MinValue).ToArray();
            Layer.ColorsHydrochemistryCd = Layer.LayerIntervalsHydrochemistryCd.Select(m => m.Color).ToArray();

            if (Layer.LayerIntervalsHydrochemistryCo == null)
            {
                Layer.LayerIntervalsHydrochemistryCo = new List<LayerInterval>();
            }
            for (int i = Layer.LayerIntervalsHydrochemistryCo.Count - 1; i >= 0; i--)
            {
                if (string.IsNullOrEmpty(Layer.LayerIntervalsHydrochemistryCo[i].Color))
                {
                    Layer.LayerIntervalsHydrochemistryCo.RemoveAt(i);
                }
            }
            Layer.MinValuesHydrochemistryCo = Layer.LayerIntervalsHydrochemistryCo.Select(m => m.MinValue).ToArray();
            Layer.ColorsHydrochemistryCo = Layer.LayerIntervalsHydrochemistryCo.Select(m => m.Color).ToArray();

            if (Layer.LayerIntervalsHydrochemistryCIWP == null)
            {
                Layer.LayerIntervalsHydrochemistryCIWP = new List<LayerInterval>();
            }
            for (int i = Layer.LayerIntervalsHydrochemistryCIWP.Count - 1; i >= 0; i--)
            {
                if (string.IsNullOrEmpty(Layer.LayerIntervalsHydrochemistryCIWP[i].Color))
                {
                    Layer.LayerIntervalsHydrochemistryCIWP.RemoveAt(i);
                }
            }
            Layer.MinValuesHydrochemistryCIWP = Layer.LayerIntervalsHydrochemistryCIWP.Select(m => m.MinValue).ToArray();
            Layer.ColorsHydrochemistryCIWP = Layer.LayerIntervalsHydrochemistryCIWP.Select(m => m.Color).ToArray();

            string message = "";
            try
            {
                if(!AsLakeLayer)
                {
                    PublishShape(Startup.Configuration["GeoServer:Workspace"], ShapeFile, Style);
                    string xml = ShapeFile + ".xml";
                    xml = Path.Combine(GetWorkspaceDirectoryPath(Startup.Configuration["GeoServer:Workspace"]), Path.GetFileNameWithoutExtension(ShapeFile), xml);
                    Layer layer = new Layer()
                    {
                        NameKK = NameKK,
                        NameRU = NameRU,
                        NameEN = NameEN,
                        Tags = Tags,
                        DescriptionKK = DescriptionKK,
                        DescriptionRU = DescriptionRU,
                        DescriptionEN = DescriptionEN,
                        GeoServerStyle = Style,
                        GeoServerName = Path.GetFileNameWithoutExtension(ShapeFile),
                        FileNameWithPath = Path.Combine(GetWorkspaceDirectoryPath(Startup.Configuration["GeoServer:Workspace"]), Path.GetFileNameWithoutExtension(ShapeFile), ShapeFile),
                        Lake = false,
                        MetaData = _GDAL.GetLayerMetaData(xml)
                    };
                    _context.Add(layer);
                    await _context.SaveChangesAsync();
                }
                else
                {
                    string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(ShapeFile),
                        filesDirectory = Path.Combine(GetWorkspaceDirectoryPath(Startup.Configuration["GeoServer:Workspace"]), fileNameWithoutExtension),
                        fileNameWithPath = Path.Combine(filesDirectory, ShapeFile);

                    List<int> shp = new List<int>();
                    shp = _GDAL.GetShpColumnValues(fileNameWithPath, "id").ToList();

                    //List<LakeData> waterLevels = new List<LakeData>();
                    //foreach(WaterLevel waterLevel in _context.WaterLevel)
                    //{
                    //    waterLevels.Add(new LakeData() {
                    //        LakeId = waterLevel.LakeId,
                    //        Year = waterLevel.Year,
                    //        Value = waterLevel.WaterLavelM
                    //    });
                    //}

                    string styleText = CreateStyleForLakes(shp.ToArray());
                    CreateStyle(Startup.Configuration["GeoServer:Workspace"], Path.GetFileNameWithoutExtension(ShapeFile), styleText);
                    PublishShape(Startup.Configuration["GeoServer:Workspace"], ShapeFile, Path.GetFileNameWithoutExtension(ShapeFile));
                    string xml = ShapeFile + ".xml";
                    xml = Path.Combine(GetWorkspaceDirectoryPath(Startup.Configuration["GeoServer:Workspace"]), Path.GetFileNameWithoutExtension(ShapeFile), xml);
                    Layer layer = new Layer()
                    {
                        NameKK = NameKK,
                        NameRU = NameRU,
                        NameEN = NameEN,
                        Tags = Tags,
                        DescriptionKK = DescriptionKK,
                        DescriptionRU = DescriptionRU,
                        DescriptionEN = DescriptionEN,
                        MetaData = _GDAL.GetLayerMetaData(xml),
                        GeoServerStyle = Path.GetFileNameWithoutExtension(ShapeFile),
                        GeoServerName = Path.GetFileNameWithoutExtension(ShapeFile),
                        FileNameWithPath = Path.Combine(GetWorkspaceDirectoryPath(Startup.Configuration["GeoServer:Workspace"]), Path.GetFileNameWithoutExtension(ShapeFile), ShapeFile),
                        MinValuesWaterLevel = Layer.MinValuesWaterLevel,
                        ColorsWaterLevel = Layer.ColorsWaterLevel,
                        MinValuesSurfaceFlow = Layer.MinValuesSurfaceFlow,
                        ColorsSurfaceFlow = Layer.ColorsSurfaceFlow,
                        MinValuesPrecipitation = Layer.MinValuesPrecipitation,
                        ColorsPrecipitation = Layer.ColorsPrecipitation,
                        MinValuesUndergroundFlow = Layer.MinValuesUndergroundFlow,
                        ColorsUndergroundFlow = Layer.ColorsUndergroundFlow,
                        MinValuesSurfaceOutflow = Layer.MinValuesSurfaceOutflow,
                        ColorsSurfaceOutflow = Layer.ColorsSurfaceOutflow,
                        MinValuesEvaporation = Layer.MinValuesEvaporation,
                        ColorsEvaporation = Layer.ColorsEvaporation,
                        MinValuesUndergroundOutflow = Layer.MinValuesUndergroundOutflow,
                        ColorsUndergroundOutflow = Layer.ColorsUndergroundOutflow,
                        MinValuesHydrochemistryMineralization = Layer.MinValuesHydrochemistryMineralization,
                        ColorsHydrochemistryMineralization = Layer.ColorsHydrochemistryMineralization,
                        MinValuesHydrochemistryTotalHardness = Layer.MinValuesHydrochemistryTotalHardness,
                        ColorsHydrochemistryTotalHardness = Layer.ColorsHydrochemistryTotalHardness,
                        MinValuesHydrochemistryDissOxygWater = Layer.MinValuesHydrochemistryDissOxygWater,
                        ColorsHydrochemistryDissOxygWater = Layer.ColorsHydrochemistryDissOxygWater,
                        MinValuesHydrochemistryPercentOxygWater = Layer.MinValuesHydrochemistryPercentOxygWater,
                        ColorsHydrochemistryPercentOxygWater = Layer.ColorsHydrochemistryPercentOxygWater,
                        MinValuesHydrochemistrypH = Layer.MinValuesHydrochemistrypH,
                        ColorsHydrochemistrypH = Layer.ColorsHydrochemistrypH,
                        MinValuesHydrochemistryOrganicSubstances = Layer.MinValuesHydrochemistryOrganicSubstances,
                        ColorsHydrochemistryOrganicSubstances = Layer.ColorsHydrochemistryOrganicSubstances,
                        MinValuesHydrochemistryCa = Layer.MinValuesHydrochemistryCa,
                        ColorsHydrochemistryCa = Layer.ColorsHydrochemistryCa,
                        MinValuesHydrochemistryMg = Layer.MinValuesHydrochemistryMg,
                        ColorsHydrochemistryMg = Layer.ColorsHydrochemistryMg,
                        MinValuesHydrochemistryNaK = Layer.MinValuesHydrochemistryNaK,
                        ColorsHydrochemistryNaK = Layer.ColorsHydrochemistryNaK,
                        MinValuesHydrochemistryCl = Layer.MinValuesHydrochemistryCl,
                        ColorsHydrochemistryCl = Layer.ColorsHydrochemistryCl,
                        MinValuesHydrochemistryHCO = Layer.MinValuesHydrochemistryHCO,
                        ColorsHydrochemistryHCO = Layer.ColorsHydrochemistryHCO,
                        MinValuesHydrochemistrySO = Layer.MinValuesHydrochemistrySO,
                        ColorsHydrochemistrySO = Layer.ColorsHydrochemistrySO,
                        MinValuesHydrochemistryNH = Layer.MinValuesHydrochemistryNH,
                        ColorsHydrochemistryNH = Layer.ColorsHydrochemistryNH,
                        MinValuesHydrochemistryNO2 = Layer.MinValuesHydrochemistryNO2,
                        ColorsHydrochemistryNO2 = Layer.ColorsHydrochemistryNO2,
                        MinValuesHydrochemistryNO3 = Layer.MinValuesHydrochemistryNO3,
                        ColorsHydrochemistryNO3 = Layer.ColorsHydrochemistryNO3,
                        MinValuesHydrochemistryPPO = Layer.MinValuesHydrochemistryPPO,
                        ColorsHydrochemistryPPO = Layer.ColorsHydrochemistryPPO,
                        MinValuesHydrochemistryCu = Layer.MinValuesHydrochemistryCu,
                        ColorsHydrochemistryCu = Layer.ColorsHydrochemistryCu,
                        MinValuesHydrochemistryZn = Layer.MinValuesHydrochemistryZn,
                        ColorsHydrochemistryZn = Layer.ColorsHydrochemistryZn,
                        MinValuesHydrochemistryMn = Layer.MinValuesHydrochemistryMn,
                        ColorsHydrochemistryMn = Layer.ColorsHydrochemistryMn,
                        MinValuesHydrochemistryPb = Layer.MinValuesHydrochemistryPb,
                        ColorsHydrochemistryPb = Layer.ColorsHydrochemistryPb,
                        MinValuesHydrochemistryNi = Layer.MinValuesHydrochemistryNi,
                        ColorsHydrochemistryNi = Layer.ColorsHydrochemistryNi,
                        MinValuesHydrochemistryCd = Layer.MinValuesHydrochemistryCd,
                        ColorsHydrochemistryCd = Layer.ColorsHydrochemistryCd,
                        MinValuesHydrochemistryCo = Layer.MinValuesHydrochemistryCo,
                        ColorsHydrochemistryCo = Layer.ColorsHydrochemistryCo,
                        MinValuesHydrochemistryCIWP = Layer.MinValuesHydrochemistryCIWP,
                        ColorsHydrochemistryCIWP = Layer.ColorsHydrochemistryCIWP,
                        Lake = true
                    };
                    _context.Add(layer);
                    await _context.SaveChangesAsync();
                }
            }
            catch (Exception exception)
            {
                message = $"{exception.ToString()}. {exception.InnerException?.Message}";
            }
            ViewBag.Styles = new SelectList(GetWorkspaceStyles(Startup.Configuration["GeoServer:Workspace"]));
            //ViewBag.ShapeFiles = new SelectList(GetShapeFiles(Startup.Configuration["GeoServer:Workspace"]));
            var publicshedLayers = GetWorkspaceLayers(Startup.Configuration["GeoServer:Workspace"]);
            ViewBag.ShapeFiles = new SelectList(GetShapeFiles(Startup.Configuration["GeoServer:Workspace"])
                .Where(l => !publicshedLayers.Contains(Path.GetFileNameWithoutExtension(l))));
            ViewBag.Message = message;

            Layer layer_ = new Layer()
            {
                LayerIntervalsWaterLevel = new List<LayerInterval>()
                {
                    new LayerInterval()
                    {
                        MinValue = 0,
                        Color = "#ffffff"
                    }
                },
                LayerIntervalsSurfaceFlow = new List<LayerInterval>()
                {
                    new LayerInterval()
                    {
                        MinValue = 0,
                        Color = "#ffffff"
                    }
                },
                LayerIntervalsPrecipitation = new List<LayerInterval>()
                {
                    new LayerInterval()
                    {
                        MinValue = 0,
                        Color = "#ffffff"
                    }
                },
                LayerIntervalsUndergroundFlow = new List<LayerInterval>()
                {
                    new LayerInterval()
                    {
                        MinValue = 0,
                        Color = "#ffffff"
                    }
                },
                LayerIntervalsSurfaceOutflow = new List<LayerInterval>()
                {
                    new LayerInterval()
                    {
                        MinValue = 0,
                        Color = "#ffffff"
                    }
                },
                LayerIntervalsEvaporation = new List<LayerInterval>()
                {
                    new LayerInterval()
                    {
                        MinValue = 0,
                        Color = "#ffffff"
                    }
                },
                LayerIntervalsUndergroundOutflow = new List<LayerInterval>()
                {
                    new LayerInterval()
                    {
                        MinValue = 0,
                        Color = "#ffffff"
                    }
                },
                LayerIntervalsHydrochemistryMineralization = new List<LayerInterval>()
                {
                    new LayerInterval()
                    {
                        MinValue = 0,
                        Color = "#ffffff"
                    }
                },
                LayerIntervalsHydrochemistryTotalHardness = new List<LayerInterval>()
                {
                    new LayerInterval()
                    {
                        MinValue = 0,
                        Color = "#ffffff"
                    }
                },
                LayerIntervalsHydrochemistryDissOxygWater = new List<LayerInterval>()
                {
                    new LayerInterval()
                    {
                        MinValue = 0,
                        Color = "#ffffff"
                    }
                },
                LayerIntervalsHydrochemistryPercentOxygWater = new List<LayerInterval>()
                {
                    new LayerInterval()
                    {
                        MinValue = 0,
                        Color = "#ffffff"
                    }
                },
                LayerIntervalsHydrochemistrypH = new List<LayerInterval>()
                {
                    new LayerInterval()
                    {
                        MinValue = 0,
                        Color = "#ffffff"
                    }
                },
                LayerIntervalsHydrochemistryOrganicSubstances = new List<LayerInterval>()
                {
                    new LayerInterval()
                    {
                        MinValue = 0,
                        Color = "#ffffff"
                    }
                },
                LayerIntervalsHydrochemistryCa = new List<LayerInterval>()
                {
                    new LayerInterval()
                    {
                        MinValue = 0,
                        Color = "#ffffff"
                    }
                },
                LayerIntervalsHydrochemistryMg = new List<LayerInterval>()
                {
                    new LayerInterval()
                    {
                        MinValue = 0,
                        Color = "#ffffff"
                    }
                },
                LayerIntervalsHydrochemistryNaK = new List<LayerInterval>()
                {
                    new LayerInterval()
                    {
                        MinValue = 0,
                        Color = "#ffffff"
                    }
                },
                LayerIntervalsHydrochemistryCl = new List<LayerInterval>()
                {
                    new LayerInterval()
                    {
                        MinValue = 0,
                        Color = "#ffffff"
                    }
                },
                LayerIntervalsHydrochemistryHCO = new List<LayerInterval>()
                {
                    new LayerInterval()
                    {
                        MinValue = 0,
                        Color = "#ffffff"
                    }
                },
                LayerIntervalsHydrochemistrySO = new List<LayerInterval>()
                {
                    new LayerInterval()
                    {
                        MinValue = 0,
                        Color = "#ffffff"
                    }
                },
                LayerIntervalsHydrochemistryNH = new List<LayerInterval>()
                {
                    new LayerInterval()
                    {
                        MinValue = 0,
                        Color = "#ffffff"
                    }
                },
                LayerIntervalsHydrochemistryNO2 = new List<LayerInterval>()
                {
                    new LayerInterval()
                    {
                        MinValue = 0,
                        Color = "#ffffff"
                    }
                },
                LayerIntervalsHydrochemistryNO3 = new List<LayerInterval>()
                {
                    new LayerInterval()
                    {
                        MinValue = 0,
                        Color = "#ffffff"
                    }
                },
                LayerIntervalsHydrochemistryPPO = new List<LayerInterval>()
                {
                    new LayerInterval()
                    {
                        MinValue = 0,
                        Color = "#ffffff"
                    }
                },
                LayerIntervalsHydrochemistryCu = new List<LayerInterval>()
                {
                    new LayerInterval()
                    {
                        MinValue = 0,
                        Color = "#ffffff"
                    }
                },
                LayerIntervalsHydrochemistryZn = new List<LayerInterval>()
                {
                    new LayerInterval()
                    {
                        MinValue = 0,
                        Color = "#ffffff"
                    }
                },
                LayerIntervalsHydrochemistryMn = new List<LayerInterval>()
                {
                    new LayerInterval()
                    {
                        MinValue = 0,
                        Color = "#ffffff"
                    }
                },
                LayerIntervalsHydrochemistryPb = new List<LayerInterval>()
                {
                    new LayerInterval()
                    {
                        MinValue = 0,
                        Color = "#ffffff"
                    }
                },
                LayerIntervalsHydrochemistryNi = new List<LayerInterval>()
                {
                    new LayerInterval()
                    {
                        MinValue = 0,
                        Color = "#ffffff"
                    }
                },
                LayerIntervalsHydrochemistryCd = new List<LayerInterval>()
                {
                    new LayerInterval()
                    {
                        MinValue = 0,
                        Color = "#ffffff"
                    }
                },
                LayerIntervalsHydrochemistryCo = new List<LayerInterval>()
                {
                    new LayerInterval()
                    {
                        MinValue = 0,
                        Color = "#ffffff"
                    }
                },
                LayerIntervalsHydrochemistryCIWP = new List<LayerInterval>()
                {
                    new LayerInterval()
                    {
                        MinValue = 0,
                        Color = "#ffffff"
                    }
                },
            };
            return View(layer_);
        }

        /// <summary>
        /// Отмена публикации Shape-файла в рабочей области ELake (Get)
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "Administrator, Moderator")]
        public IActionResult UnpublishShape()
        {
            ViewBag.ShapeLayers = new SelectList(GetWorkspaceShapeLayers(Startup.Configuration["GeoServer:Workspace"]));
            return View();
        }

        /// <summary>
        /// Отмена публикации Shape-файла в рабочей области ELake (Get)
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = "Administrator, Moderator")]
        public IActionResult UnpublishShape(string LayerName)
        {
            ViewData["Message"] = "";
            try
            {
                UnpublishShape(Startup.Configuration["GeoServer:Workspace"], LayerName);
                var layer = _context.Layer.SingleOrDefault(m => m.GeoServerName == LayerName);
                if (layer != null)
                {
                    _context.Layer.Remove(layer);
                    _context.SaveChanges();
                }
            }
            catch (Exception exception)
            {
                ViewData["Message"] = $"{exception.ToString()}. {exception.InnerException?.Message}";
            }
            ViewBag.ShapeLayers = new SelectList(GetWorkspaceShapeLayers(Startup.Configuration["GeoServer:Workspace"]));
            return View();
        }

        // GET: Layers/Edit/5
        public async Task<IActionResult> AddLakes(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var layer = await _context.Layer.SingleOrDefaultAsync(m => m.Id == id);
            if (layer == null)
            {
                return NotFound();
            }

            var publicshedLayers = GetWorkspaceLayers(Startup.Configuration["GeoServer:Workspace"]);
            ViewBag.ShapeFiles = new SelectList(GetShapeFiles(Startup.Configuration["GeoServer:Workspace"])
                .Where(l => !publicshedLayers.Contains(Path.GetFileNameWithoutExtension(l))));
            return View(layer);
        }

        // POST: Layers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddLakes(Layer layer, string ShapeFile, int[] ReplaceIds, bool[] Replaces)
        {
            //200090  B don't replace
            //601810  T replace


            Layer layerOld = _context.Layer.FirstOrDefault(l => l.Id == layer.Id);
            string oldShape = layerOld.FileNameWithPath,
                newShape = Path.Combine(GetWorkspaceDirectoryPath(Startup.Configuration["GeoServer:Workspace"]), Path.GetFileNameWithoutExtension(ShapeFile), ShapeFile);
            List<int> replaceIds = new List<int>(),
                dontReplaceIds = new List<int>();
            for (int i = 0; i < ReplaceIds.Count(); i++)
            {
                if (Replaces[i])
                {
                    replaceIds.Add(ReplaceIds[i]);
                }
                else
                {
                    dontReplaceIds.Add(ReplaceIds[i]);
                }
            }

            // копирование shape файлов в одну папку
            foreach(string file in Directory.GetFiles(Path.Combine(GetWorkspaceDirectoryPath(Startup.Configuration["GeoServer:Workspace"]), "_Bufer"), "*.*"))
            {
                System.IO.File.Delete(file);
            }
            foreach(string file in Directory.GetFiles(Path.GetDirectoryName(oldShape), Path.GetFileNameWithoutExtension(oldShape) + ".*"))
            {
                System.IO.File.Copy(file, Path.Combine(GetWorkspaceDirectoryPath(Startup.Configuration["GeoServer:Workspace"]), "_Bufer", Path.GetFileName(file)));
            }
            foreach(string file in Directory.GetFiles(Path.GetDirectoryName(newShape), Path.GetFileNameWithoutExtension(newShape) + ".*"))
            {
                System.IO.File.Copy(file, Path.Combine(GetWorkspaceDirectoryPath(Startup.Configuration["GeoServer:Workspace"]), "_Bufer", Path.GetFileName(file)));
            }

            // удаление объектов с файлов
            foreach(int feature in replaceIds)
            {
                _GDAL.DeleteFeatures(Path.Combine(GetWorkspaceDirectoryPath(Startup.Configuration["GeoServer:Workspace"]), "_Bufer", Path.GetFileName(oldShape)), "id", feature.ToString());
            }
            foreach(int feature in dontReplaceIds)
            {
                _GDAL.DeleteFeatures(Path.Combine(GetWorkspaceDirectoryPath(Startup.Configuration["GeoServer:Workspace"]), "_Bufer", Path.GetFileName(newShape)), "id", feature.ToString());
            }

            _GDAL.MergeShapes(Path.Combine(GetWorkspaceDirectoryPath(Startup.Configuration["GeoServer:Workspace"]), "_Bufer"), Path.Combine(GetWorkspaceDirectoryPath(Startup.Configuration["GeoServer:Workspace"]), "_Bufer", Path.GetFileName(oldShape)));

            foreach (string file in Directory.GetFiles(Path.Combine(GetWorkspaceDirectoryPath(Startup.Configuration["GeoServer:Workspace"]), "_Bufer"), Path.GetFileNameWithoutExtension(oldShape) + ".*"))
            {
                System.IO.File.Delete(Path.Combine(Path.GetDirectoryName(oldShape), Path.GetFileName(file)));
                System.IO.File.Move(Path.Combine(GetWorkspaceDirectoryPath(Startup.Configuration["GeoServer:Workspace"]), "_Bufer", Path.GetFileName(file)), Path.Combine(Path.GetDirectoryName(oldShape), Path.GetFileName(file)));
            }

            foreach (string file in Directory.GetFiles(Path.Combine(GetWorkspaceDirectoryPath(Startup.Configuration["GeoServer:Workspace"]), "_Bufer"), "*.*"))
            {
                System.IO.File.Delete(file);
            }

            var publishedLayers = GetWorkspaceLayers(Startup.Configuration["GeoServer:Workspace"]);
            ViewBag.ShapeFiles = new SelectList(GetShapeFiles(Startup.Configuration["GeoServer:Workspace"])
                .Where(l => !publishedLayers.Contains(Path.GetFileNameWithoutExtension(l))));
            return View();
        }

        [HttpPost]
        public ActionResult GetComparedLakes(int OldLakeLayerId, string NewLakeShapeFileName)
        {
            Layer layer = _context.Layer.FirstOrDefault(l => l.Id == OldLakeLayerId);
            string oldShape = layer.FileNameWithPath,
                newShape = Path.Combine(GetWorkspaceDirectoryPath(Startup.Configuration["GeoServer:Workspace"]), Path.GetFileNameWithoutExtension(NewLakeShapeFileName), NewLakeShapeFileName);
            int[] oldLakes = _GDAL.GetShpColumnValues(oldShape, "id"),
                newLakes = _GDAL.GetShpColumnValues(newShape, "id");
            int[] commonLakes = oldLakes.Intersect(newLakes).ToArray();
            return Json(new
            {
                commonLakes
            });
        }

        public struct LakeData
        {
            public int LakeId;
            public int Year;
            public decimal Value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="LakeDatas">
        /// Данные с таблиц по озерам
        /// </param>
        /// <param name="Shp">
        /// Значения поля Id Shape-файла
        /// </param>
        /// <returns></returns>
        public string CreateStyleForLakes(int[] Shp)
        {
            string style = "<?xml version=\"1.0\" encoding=\"UTF-8\"?>" +
                "<StyledLayerDescriptor version=\"1.0.0\" " +
                "                       xsi:schemaLocation=\"http://www.opengis.net/sld StyledLayerDescriptor.xsd\" " +
                "                       xmlns=\"http://www.opengis.net/sld\" " +
                "                       xmlns:ogc=\"http://www.opengis.net/ogc\" " +
                "                       xmlns:xlink=\"http://www.w3.org/1999/xlink\" " +
                "                       xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">" +
                "  <!-- a Named Layer is the basic building block of an SLD document -->" +
                "  <NamedLayer>" +
                "    <Name>default_polygon</Name>" +
                "    <UserStyle>" +
                "      <!-- Styles can have names, titles and abstracts -->" +
                "      <Title>Default Polygon</Title>" +
                "      <Abstract>A sample style that draws a polygon</Abstract>" +
                "      <!-- FeatureTypeStyles describe how to render different features -->" +
                "      <!-- A FeatureTypeStyle for rendering polygons -->" +
                "      <FeatureTypeStyle>" +
                "        <Rule>" +
                "          <PolygonSymbolizer>" +
                "            <Fill>" +
                "              <CssParameter name=\"fill\">#ffffff</CssParameter>" +
                "              <CssParameter name=\"fill-opacity\">0.0</CssParameter>" +
                "            </Fill>" +
                "            <Stroke>" +
                "              <CssParameter name=\"stroke\">#000000</CssParameter>" +
                "              <CssParameter name=\"stroke-width\">1</CssParameter>" +
                "            </Stroke>" +
                "          </PolygonSymbolizer>" +
                "        </Rule>";
            foreach(int shpId in Shp)
            {
                style +=
                "        <Rule>" +
                "          <ogc:Filter xmlns:ogc=\"http://www.opengis.net/ogc\">" +
                "            <ogc:PropertyIsEqualTo>" +
                "              <ogc:PropertyName>id</ogc:PropertyName>" +
                $"              <ogc:Literal>{shpId.ToString()}</ogc:Literal>" +
                "            </ogc:PropertyIsEqualTo>" +
                "          </ogc:Filter>" +
                "          <PolygonSymbolizer>" +
                "            <WellKnownName><ogc:Function name=\"env\">" +
                "              <ogc:Literal>name</ogc:Literal>" +
                "              </ogc:Function>" +
                "            </WellKnownName>" +
                "            <Fill>" +
                "              <CssParameter name=\"fill\">" +
                "          #<ogc:Function name=\"env\">" +
                $"            <ogc:Literal>color{shpId.ToString()}</ogc:Literal>" +
                "            <ogc:Literal>ffffff</ogc:Literal>" +
                "         </ogc:Function>" +
                "        </CssParameter>" +
                "            </Fill>" +
                "            <Stroke>" +
                "              <CssParameter name=\"stroke\">#000000</CssParameter>" +
                "              <CssParameter name=\"stroke-width\">1</CssParameter>" +
                "            </Stroke>" +
                "          </PolygonSymbolizer>" +
                "        </Rule>";
            }
            style +=
                "      </FeatureTypeStyle>" +
                "    </UserStyle>" +
                "  </NamedLayer>" +
                "</StyledLayerDescriptor>";
            return style;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ShapeFile">
        /// roads.shp
        /// </param>
        public void UpdateStyleForLakes(string ShapeFile)
        {
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(ShapeFile),
                filesDirectory = Path.Combine(GetWorkspaceDirectoryPath(Startup.Configuration["GeoServer:Workspace"]), fileNameWithoutExtension),
                fileNameWithPath = Path.Combine(filesDirectory, ShapeFile);
            List<int> shp = new List<int>();
            shp = _GDAL.GetShpColumnValues(fileNameWithPath, "id").ToList();
            string styleText = CreateStyleForLakes(shp.ToArray());
            ChangeStyle(Startup.Configuration["GeoServer:Workspace"], Path.GetFileNameWithoutExtension(ShapeFile), styleText);
        }

        [Authorize(Roles = "Administrator, Moderator")]
        public IActionResult UpdateLakeLayerStyle()
        {
            //ViewBag.ShapeFiles = new SelectList(GetShapeFiles(Startup.Configuration["GeoServer:Workspace"]));
            ViewBag.ShapeFiles = new SelectList(_context.Layer.Where(l => l.Lake).Select(l => l.GeoServerName + ".shp"));
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> UpdateLakeLayerStyle(string ShapeFile)
        {
            ViewData["Message"] = "";
            try
            {
                UpdateStyleForLakes(ShapeFile);
                ViewData["Message"] = $"Layer {ShapeFile} style was updated!";
            }
            catch (Exception exception)
            {
                ViewData["Message"] = $"{exception.ToString()}. {exception.InnerException?.Message}";
            }
            //ViewBag.ShapeFiles = new SelectList(GetShapeFiles(Startup.Configuration["GeoServer:Workspace"]));
            ViewBag.ShapeFiles = new SelectList(_context.Layer.Where(l => l.Lake).Select(l => l.GeoServerName + ".shp"));
            return View();
        }

        public IActionResult Instruction(string Type)
        {
            ViewBag.Type = Type;
            return View();
        }

        [Authorize(Roles = "Administrator, Moderator")]
        public IActionResult FindKATO(int LayerId)
        {
            ViewBag.LakeLayers = new SelectList(_context.Layer.Where(l => l.Lake), "Id", "Name");
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> FindKATO(int LayerId, string KATOLayerFile)
        {
            ViewBag.LakeLayers = new SelectList(_context.Layer.Where(l => l.Lake), "Id", "Name", LayerId);
            FindKATO(_context.Layer.FirstOrDefault(l => l.Id == LayerId).FileNameWithPath);
            return View();
        }
    }
}