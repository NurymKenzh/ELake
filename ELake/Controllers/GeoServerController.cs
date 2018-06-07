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
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Localization;

namespace ELake.Controllers
{
    public class GeoServerController : Controller
    {
        private IHostingEnvironment _hostingEnvironment;
        private readonly ApplicationDbContext _context;
        private readonly IStringLocalizer<SharedResources> _sharedLocalizer;

        public GeoServerController(IHostingEnvironment hostingEnvironment,
            ApplicationDbContext context,
            IStringLocalizer<SharedResources> sharedLocalizer)
        {
            _hostingEnvironment = hostingEnvironment;
            _context = context;
            _sharedLocalizer = sharedLocalizer;
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

                Process process1 = CurlExecute($" -u " +
                    $"{Startup.Configuration["GeoServer:User"]}:" +
                    $"{Startup.Configuration["GeoServer:Password"]}" +
                    $" -v -u -XPOST" +
                    $" -H \"Content-type: text/xml\"" +
                    $" -d \"<style><name>{Style}</name><filename>{Style}.sld</filename></style>\"" +
                    $" http://{Startup.Configuration["GeoServer:Address"]}:" +
                    $"{Startup.Configuration["GeoServer:Port"]}/geoserver/rest/workspaces/{WorkspaceName}/styles");
                process1.WaitForExit();
                Process process2 = CurlExecute($" -v -u " +
                    $"{Startup.Configuration["GeoServer:User"]}:" +
                    $"{Startup.Configuration["GeoServer:Password"]}" +
                    $" -v -u -XPUT" +
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
            return geoTIFFFiles.ToArray();
        }

        /// <summary>
        /// Загрузка GeoTIFF-файлов в папку рабочей области GeoServer
        /// </summary>
        /// <param name="WorkspaceName"></param>
        /// <param name="Files"></param>
        private async void UploadGeoTIFFFiles(string WorkspaceName, List<IFormFile> Files)
        {
            try
            {
                foreach (IFormFile file in Files)
                {
                    var filePath = Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), Path.GetFileName(file.FileName));
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }

                }
                List<string> unzipfiles = new List<string>();
                foreach (IFormFile file in Files)
                {
                    if (Path.GetExtension(file.FileName) == ".zip")
                    {
                        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                        using (ZipFile zip = ZipFile.Read(Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), Path.GetFileName(file.FileName))))
                        {
                            foreach (ZipEntry filefromzip in zip)
                            {
                                filefromzip.Extract(GetWorkspaceDirectoryPath(WorkspaceName), ExtractExistingFileAction.OverwriteSilently);
                                unzipfiles.Add(Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), filefromzip.FileName));
                            }
                        }
                    }
                }
                IConfigurationSection geoTIFFFileExtentions = Startup.Configuration.GetSection("GeoServer:GeoTIFFFileExtentions");
                foreach (IFormFile file in Files)
                {
                    if (!geoTIFFFileExtentions.AsEnumerable().Select(l => l.Value).Contains(Path.GetExtension(file.FileName)))
                    {
                        System.IO.File.Delete(Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), Path.GetFileName(file.FileName)));
                    }
                }
                foreach (string file in unzipfiles)
                {
                    if (!geoTIFFFileExtentions.AsEnumerable().Select(l => l.Value).Contains(Path.GetExtension(file)))
                    {
                        System.IO.File.Delete(Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), file));
                    }
                }
            }
            catch (Exception exception)
            {
                throw new Exception(exception.ToString(), exception.InnerException);
            }
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
                    throw new Exception($"{FileName} can't be deleted because it is used in current layers: {string.Join(", ", geoTIFFFileLayers)}!");
                }
                System.IO.File.Delete(Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), FileName));
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
        private async void UploadShapeFiles(string WorkspaceName, List<IFormFile> Files)
        {
            try
            {
                foreach (IFormFile file in Files)
                {
                    var filePath = Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), Path.GetFileName(file.FileName));
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }

                }
                List<string> unzipfiles = new List<string>();
                foreach (IFormFile file in Files)
                {
                    if (Path.GetExtension(file.FileName) == ".zip")
                    {
                        Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);
                        using (ZipFile zip = ZipFile.Read(Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), Path.GetFileName(file.FileName))))
                        {
                            foreach (ZipEntry filefromzip in zip)
                            {
                                filefromzip.Extract(GetWorkspaceDirectoryPath(WorkspaceName), ExtractExistingFileAction.OverwriteSilently);
                                unzipfiles.Add(Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), filefromzip.FileName));
                            }
                        }
                    }
                }
                IConfigurationSection geoTIFFFileExtentions = Startup.Configuration.GetSection("GeoServer:ShapeFileExtentions");
                foreach (IFormFile file in Files)
                {
                    if (!geoTIFFFileExtentions.AsEnumerable().Select(l => l.Value).Contains(Path.GetExtension(file.FileName)))
                    {
                        System.IO.File.Delete(Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), Path.GetFileName(file.FileName)));
                    }
                }
                foreach (string file in unzipfiles)
                {
                    if (!geoTIFFFileExtentions.AsEnumerable().Select(l => l.Value).Contains(Path.GetExtension(file)))
                    {
                        System.IO.File.Delete(Path.Combine(GetWorkspaceDirectoryPath(WorkspaceName), file));
                    }
                }
                MoveShapeFilesToDirectories(WorkspaceName);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.ToString(), exception.InnerException);
            }
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
                    throw new Exception($"{FileName} can't be deleted because it is used in current layers: {string.Join(", ", shapeFileLayers)}!");
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
                    throw new Exception($"Layer {fileNameWithoutExtension} is already exist in {WorkspaceName} workspace!");
                }

                if (Path.GetExtension(FileName).ToLower() != ".shp")
                {
                    throw new Exception("File extension must be \"shp\"!");
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
        //===========================================================================================================================================================================
        /// <summary>
        /// Загрузка GeoTIFF-файлов в папку рабочей области GeoServer (Get)
        /// </summary>
        /// <remarks>
        /// Рабочая область всегда "ELake"
        /// Файлы могут быть разделены на zip-архивы
        /// </remarks>
        [DisableRequestSizeLimit]
        [RequestSizeLimit(long.MaxValue)]
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
        [RequestSizeLimit(long.MaxValue)]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<string> UploadGeoTIFFFiles(List<IFormFile> Files)
        {
            string message = _sharedLocalizer["FilesUploaded"];
            try
            {
                UploadGeoTIFFFiles(Startup.Configuration["GeoServer:Workspace"], Files);
            }
            catch (Exception exception)
            {
                message = $"{exception.ToString()}. {exception.InnerException?.Message}";
            }
            return message;
        }

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
                ViewData["Message"] = $"File {GeoTIFFFile} was deleted!";
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
                UploadShapeFiles(Startup.Configuration["GeoServer:Workspace"], Files);
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
        public async Task<IActionResult> PublishGeoTIFF(string GeoTIFFFile, string Style, string NameKK, string NameRU, string NameEN)
        {
            string message = "";
            try
            {
                PublishGeoTIFF(Startup.Configuration["GeoServer:Workspace"], GeoTIFFFile, Style);
                Layer layer = new Layer() {
                    NameKK = NameKK,
                    NameRU = NameRU,
                    NameEN = NameEN,
                    GeoServerStyle = Style,
                    GeoServerName = Path.GetFileNameWithoutExtension(GeoTIFFFile),
                    FileNameWithPath = Path.Combine(GetWorkspaceDirectoryPath(Startup.Configuration["GeoServer:Workspace"]), GeoTIFFFile)
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
                ViewData["Message"] = $"File {ShapeFile} was deleted!";
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
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [DisableRequestSizeLimit]
        [RequestSizeLimit(long.MaxValue)]
        [Authorize(Roles = "Administrator, Moderator")]
        public async Task<IActionResult> PublishShape(string ShapeFile, string Style, string NameKK, string NameRU, string NameEN)
        {
            string message = "";
            try
            {
                PublishShape(Startup.Configuration["GeoServer:Workspace"], ShapeFile, Style);
                Layer layer = new Layer()
                {
                    NameKK = NameKK,
                    NameRU = NameRU,
                    NameEN = NameEN,
                    GeoServerStyle = Style,
                    GeoServerName = Path.GetFileNameWithoutExtension(ShapeFile),
                    FileNameWithPath = Path.Combine(GetWorkspaceDirectoryPath(Startup.Configuration["GeoServer:Workspace"]), Path.GetFileNameWithoutExtension(ShapeFile), ShapeFile)
                };
                _context.Add(layer);
                await _context.SaveChangesAsync();
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
            return View();
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
    }
}