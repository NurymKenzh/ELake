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
        public async Task<IActionResult> PublishShape(string ShapeFile, string Style, bool AsLakeLayer, string NameKK, string NameRU, string NameEN)
        {
            string message = "";
            try
            {
                if(!AsLakeLayer)
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
                else
                {
                    List<int> shp = new List<int>();
                    if(true)
                    {
                        shp.Add(500020);
                        shp.Add(500030);
                        shp.Add(500062);
                        shp.Add(500063);
                        shp.Add(500080);
                        shp.Add(500090);
                        shp.Add(500100);
                        shp.Add(500110);
                        shp.Add(500120);
                        shp.Add(500130);
                        shp.Add(500140);
                        shp.Add(500160);
                        shp.Add(500170);
                        shp.Add(500180);
                        shp.Add(500190);
                        shp.Add(500200);
                        shp.Add(500210);
                        shp.Add(500220);
                        shp.Add(500263);
                        shp.Add(500240);
                        shp.Add(500261);
                        shp.Add(500262);
                        shp.Add(500250);
                        shp.Add(500270);
                        shp.Add(500281);
                        shp.Add(500282);
                        shp.Add(500290);
                        shp.Add(500310);
                        shp.Add(500320);
                        shp.Add(500330);
                        shp.Add(500340);
                        shp.Add(500360);
                        shp.Add(500380);
                        shp.Add(500390);
                        shp.Add(500400);
                        shp.Add(500420);
                        shp.Add(500430);
                        shp.Add(500061);
                        shp.Add(500440);
                        shp.Add(500450);
                        shp.Add(500460);
                        shp.Add(500470);
                        shp.Add(500480);
                        shp.Add(500490);
                        shp.Add(500500);
                        shp.Add(500510);
                        shp.Add(500530);
                        shp.Add(500540);
                        shp.Add(500550);
                        shp.Add(500560);
                        shp.Add(500570);
                        shp.Add(500580);
                        shp.Add(500590);
                        shp.Add(500600);
                        shp.Add(500610);
                        shp.Add(500620);
                        shp.Add(500640);
                        shp.Add(500630);
                        shp.Add(500650);
                        shp.Add(500660);
                        shp.Add(500670);
                        shp.Add(501200);
                        shp.Add(501210);
                        shp.Add(501220);
                        shp.Add(501230);
                        shp.Add(501240);
                        shp.Add(501250);
                        shp.Add(501260);
                        shp.Add(501270);
                        shp.Add(501280);
                        shp.Add(501290);
                        shp.Add(501300);
                        shp.Add(501310);
                        shp.Add(501320);
                        shp.Add(501360);
                        shp.Add(501330);
                        shp.Add(501350);
                        shp.Add(501170);
                        shp.Add(501370);
                        shp.Add(501890);
                        shp.Add(501380);
                        shp.Add(501390);
                        shp.Add(501400);
                        shp.Add(501410);
                        shp.Add(501420);
                        shp.Add(501430);
                        shp.Add(501440);
                        shp.Add(501450);
                        shp.Add(501460);
                        shp.Add(501470);
                        shp.Add(501480);
                        shp.Add(501490);
                        shp.Add(501510);
                        shp.Add(501520);
                        shp.Add(501530);
                        shp.Add(501540);
                        shp.Add(501550);
                        shp.Add(501560);
                        shp.Add(501570);
                        shp.Add(501580);
                        shp.Add(501590);
                        shp.Add(501600);
                        shp.Add(501610);
                        shp.Add(501620);
                        shp.Add(501650);
                        shp.Add(501660);
                        shp.Add(501670);
                        shp.Add(501680);
                        shp.Add(501690);
                        shp.Add(501700);
                        shp.Add(501710);
                        shp.Add(501720);
                        shp.Add(501740);
                        shp.Add(500010);
                        shp.Add(500680);
                        shp.Add(500690);
                        shp.Add(500700);
                        shp.Add(500710);
                        shp.Add(500720);
                        shp.Add(500300);
                        shp.Add(500730);
                        shp.Add(500740);
                        shp.Add(500750);
                        shp.Add(500760);
                        shp.Add(500770);
                        shp.Add(500780);
                        shp.Add(500790);
                        shp.Add(500800);
                        shp.Add(500810);
                        shp.Add(500820);
                        shp.Add(500830);
                        shp.Add(500840);
                        shp.Add(500850);
                        shp.Add(500860);
                        shp.Add(500880);
                        shp.Add(500890);
                        shp.Add(500900);
                        shp.Add(500910);
                        shp.Add(500920);
                        shp.Add(500930);
                        shp.Add(500940);
                        shp.Add(500950);
                        shp.Add(500960);
                        shp.Add(500971);
                        shp.Add(500972);
                        shp.Add(500973);
                        shp.Add(500980);
                        shp.Add(500990);
                        shp.Add(501000);
                        shp.Add(501010);
                        shp.Add(501030);
                        shp.Add(501040);
                        shp.Add(501050);
                        shp.Add(501060);
                        shp.Add(501090);
                        shp.Add(501080);
                        shp.Add(501070);
                        shp.Add(501100);
                        shp.Add(501110);
                        shp.Add(501120);
                        shp.Add(501880);
                        shp.Add(501140);
                        shp.Add(501130);
                        shp.Add(501150);
                        shp.Add(501160);
                        shp.Add(501180);
                        shp.Add(501190);
                        shp.Add(500870);
                        shp.Add(501020);
                        shp.Add(500050);
                        shp.Add(500070);
                        shp.Add(500150);
                        shp.Add(500230);
                        shp.Add(500350);
                        shp.Add(500370);
                        shp.Add(500521);
                        shp.Add(500522);
                        shp.Add(501340);
                        shp.Add(501500);
                        shp.Add(501630);
                        shp.Add(501640);
                        shp.Add(501730);
                        shp.Add(501750);
                        shp.Add(501760);
                        shp.Add(501770);
                        shp.Add(501780);
                        shp.Add(501790);
                        shp.Add(501800);
                        shp.Add(501810);
                        shp.Add(501830);
                        shp.Add(500264);
                        shp.Add(501840);
                        shp.Add(501850);
                        shp.Add(501860);
                        shp.Add(501870);
                        shp.Add(500040);
                        shp.Add(501900);
                        shp.Add(500410);
                        shp.Add(400010);
                        shp.Add(400020);
                        shp.Add(400040);
                        shp.Add(400050);
                        shp.Add(400070);
                        shp.Add(400090);
                        shp.Add(400100);
                        shp.Add(400110);
                        shp.Add(400120);
                        shp.Add(400130);
                        shp.Add(400060);
                        shp.Add(400080);
                        shp.Add(400140);
                        shp.Add(400150);
                        shp.Add(400160);
                        shp.Add(400170);
                        shp.Add(401440);
                        shp.Add(400180);
                        shp.Add(400190);
                        shp.Add(400210);
                        shp.Add(400220);
                        shp.Add(400230);
                        shp.Add(400240);
                        shp.Add(400250);
                        shp.Add(400260);
                        shp.Add(400270);
                        shp.Add(400280);
                        shp.Add(400300);
                        shp.Add(400310);
                        shp.Add(400320);
                        shp.Add(400330);
                        shp.Add(400340);
                        shp.Add(400350);
                        shp.Add(400360);
                        shp.Add(400370);
                        shp.Add(400380);
                        shp.Add(400390);
                        shp.Add(400400);
                        shp.Add(400410);
                        shp.Add(400430);
                        shp.Add(400440);
                        shp.Add(400450);
                        shp.Add(400470);
                        shp.Add(400480);
                        shp.Add(400490);
                        shp.Add(400500);
                        shp.Add(400510);
                        shp.Add(400520);
                        shp.Add(400540);
                        shp.Add(400550);
                        shp.Add(400560);
                        shp.Add(400570);
                        shp.Add(400590);
                        shp.Add(400600);
                        shp.Add(400610);
                        shp.Add(400620);
                        shp.Add(400630);
                        shp.Add(400640);
                        shp.Add(400660);
                        shp.Add(400670);
                        shp.Add(400680);
                        shp.Add(400690);
                        shp.Add(400700);
                        shp.Add(400710);
                        shp.Add(400730);
                        shp.Add(400750);
                        shp.Add(400760);
                        shp.Add(400770);
                        shp.Add(400780);
                        shp.Add(400790);
                        shp.Add(400800);
                        shp.Add(400810);
                        shp.Add(400820);
                        shp.Add(400840);
                        shp.Add(400850);
                        shp.Add(400860);
                        shp.Add(400870);
                        shp.Add(400880);
                        shp.Add(400890);
                        shp.Add(400900);
                        shp.Add(400920);
                        shp.Add(400930);
                        shp.Add(400940);
                        shp.Add(400950);
                        shp.Add(400960);
                        shp.Add(400970);
                        shp.Add(400980);
                        shp.Add(400990);
                        shp.Add(401000);
                        shp.Add(401010);
                        shp.Add(401020);
                        shp.Add(401030);
                        shp.Add(401040);
                        shp.Add(401060);
                        shp.Add(401080);
                        shp.Add(401090);
                        shp.Add(401100);
                        shp.Add(401110);
                        shp.Add(401120);
                        shp.Add(401130);
                        shp.Add(401140);
                        shp.Add(401150);
                        shp.Add(401160);
                        shp.Add(401180);
                        shp.Add(401200);
                        shp.Add(401210);
                        shp.Add(401220);
                        shp.Add(401240);
                        shp.Add(401250);
                        shp.Add(401260);
                        shp.Add(401280);
                        shp.Add(401290);
                        shp.Add(401310);
                        shp.Add(401320);
                        shp.Add(401330);
                        shp.Add(401340);
                        shp.Add(401410);
                        shp.Add(401560);
                        shp.Add(401670);
                        shp.Add(401750);
                        shp.Add(401760);
                        shp.Add(401780);
                        shp.Add(401790);
                        shp.Add(401800);
                        shp.Add(401810);
                        shp.Add(401820);
                        shp.Add(401420);
                        shp.Add(401430);
                        shp.Add(401450);
                        shp.Add(401460);
                        shp.Add(401470);
                        shp.Add(401480);
                        shp.Add(401490);
                        shp.Add(401500);
                        shp.Add(401510);
                        shp.Add(401520);
                        shp.Add(401530);
                        shp.Add(401540);
                        shp.Add(401550);
                        shp.Add(401570);
                        shp.Add(401580);
                        shp.Add(401600);
                        shp.Add(401610);
                        shp.Add(401620);
                        shp.Add(401630);
                        shp.Add(401640);
                        shp.Add(401650);
                        shp.Add(401660);
                        shp.Add(401680);
                        shp.Add(401690);
                        shp.Add(401720);
                        shp.Add(401830);
                        shp.Add(401840);
                        shp.Add(401850);
                        shp.Add(401880);
                        shp.Add(401890);
                        shp.Add(401900);
                        shp.Add(401910);
                        shp.Add(401930);
                        shp.Add(401940);
                        shp.Add(401960);
                        shp.Add(401970);
                        shp.Add(401980);
                        shp.Add(401990);
                        shp.Add(402000);
                        shp.Add(402010);
                        shp.Add(402020);
                        shp.Add(402030);
                        shp.Add(402040);
                        shp.Add(402050);
                        shp.Add(402060);
                        shp.Add(402080);
                        shp.Add(402100);
                        shp.Add(402110);
                        shp.Add(402130);
                        shp.Add(402140);
                        shp.Add(402150);
                        shp.Add(402160);
                        shp.Add(402180);
                        shp.Add(402190);
                        shp.Add(402200);
                        shp.Add(402210);
                        shp.Add(402220);
                        shp.Add(402230);
                        shp.Add(402240);
                        shp.Add(402300);
                        shp.Add(402310);
                        shp.Add(402320);
                        shp.Add(402330);
                        shp.Add(402340);
                        shp.Add(402350);
                        shp.Add(402360);
                        shp.Add(402400);
                        shp.Add(402410);
                        shp.Add(402420);
                        shp.Add(402430);
                        shp.Add(402480);
                        shp.Add(402490);
                        shp.Add(402500);
                        shp.Add(402510);
                        shp.Add(402530);
                        shp.Add(402540);
                        shp.Add(402550);
                        shp.Add(402560);
                        shp.Add(402570);
                        shp.Add(402600);
                        shp.Add(400530);
                        shp.Add(400420);
                        shp.Add(400580);
                        shp.Add(400720);
                        shp.Add(401050);
                        shp.Add(401070);
                        shp.Add(401230);
                        shp.Add(401360);
                        shp.Add(401300);
                        shp.Add(401350);
                        shp.Add(401370);
                        shp.Add(401380);
                        shp.Add(401390);
                        shp.Add(401400);
                        shp.Add(402250);
                        shp.Add(402260);
                        shp.Add(402270);
                        shp.Add(402290);
                        shp.Add(402370);
                        shp.Add(402380);
                        shp.Add(402390);
                        shp.Add(402460);
                        shp.Add(401270);
                        shp.Add(402770);
                        shp.Add(402780);
                        shp.Add(402880);
                        shp.Add(402890);
                        shp.Add(402790);
                        shp.Add(402800);
                        shp.Add(402810);
                        shp.Add(402820);
                        shp.Add(402830);
                        shp.Add(402850);
                        shp.Add(402840);
                        shp.Add(402860);
                        shp.Add(402070);
                        shp.Add(400030);
                        shp.Add(400200);
                        shp.Add(400460);
                        shp.Add(401770);
                        shp.Add(401860);
                        shp.Add(401950);
                        shp.Add(402090);
                        shp.Add(402120);
                        shp.Add(402170);
                        shp.Add(402450);
                        shp.Add(402520);
                        shp.Add(402580);
                        shp.Add(402590);
                        shp.Add(402280);
                        shp.Add(400830);
                        shp.Add(401170);
                        shp.Add(402900);
                        shp.Add(402920);
                        shp.Add(402910);
                        shp.Add(401730);
                        shp.Add(402610);
                        shp.Add(402620);
                        shp.Add(402640);
                        shp.Add(402630);
                        shp.Add(402660);
                        shp.Add(402650);
                        shp.Add(402680);
                        shp.Add(402690);
                        shp.Add(402670);
                        shp.Add(402700);
                        shp.Add(402720);
                        shp.Add(402730);
                        shp.Add(402740);
                        shp.Add(402870);
                        shp.Add(402750);
                        shp.Add(402760);
                        shp.Add(400290);
                        shp.Add(401590);
                        shp.Add(401700);
                        shp.Add(401710);
                        shp.Add(402470);
                        shp.Add(400650);
                        shp.Add(400740);
                        shp.Add(401190);
                        shp.Add(401920);
                        shp.Add(401870);
                        shp.Add(401740);
                        shp.Add(402440);
                        shp.Add(402930);
                        shp.Add(402940);
                        shp.Add(402950);
                        shp.Add(402960);
                        shp.Add(402980);
                        shp.Add(402990);
                        shp.Add(403010);
                        shp.Add(403020);
                        shp.Add(403000);
                        shp.Add(403040);
                        shp.Add(403050);
                        shp.Add(403060);
                        shp.Add(403070);
                        shp.Add(403090);
                        shp.Add(403110);
                        shp.Add(403120);
                        shp.Add(403130);
                        shp.Add(403140);
                        shp.Add(403150);
                        shp.Add(403160);
                        shp.Add(403170);
                        shp.Add(403180);
                        shp.Add(403190);
                        shp.Add(403200);
                        shp.Add(403210);
                        shp.Add(403220);
                        shp.Add(403230);
                        shp.Add(403240);
                        shp.Add(403250);
                        shp.Add(403260);
                        shp.Add(403270);
                        shp.Add(403280);
                        shp.Add(403290);
                        shp.Add(403300);
                        shp.Add(403310);
                        shp.Add(403330);
                        shp.Add(403340);
                        shp.Add(403350);
                        shp.Add(403360);
                        shp.Add(403390);
                        shp.Add(403370);
                        shp.Add(403380);
                        shp.Add(403400);
                        shp.Add(403410);
                        shp.Add(403430);
                        shp.Add(403420);
                        shp.Add(403440);
                        shp.Add(403450);
                        shp.Add(403460);
                        shp.Add(403470);
                        shp.Add(403490);
                        shp.Add(403480);
                        shp.Add(403500);
                        shp.Add(403510);
                        shp.Add(403520);
                        shp.Add(403530);
                        shp.Add(403540);
                        shp.Add(403550);
                        shp.Add(403560);
                        shp.Add(403570);
                        shp.Add(403580);
                        shp.Add(403590);
                        shp.Add(403600);
                        shp.Add(403610);
                        shp.Add(403620);
                        shp.Add(403630);
                        shp.Add(403640);
                        shp.Add(403650);
                        shp.Add(403660);
                        shp.Add(403670);
                        shp.Add(403680);
                        shp.Add(403690);
                        shp.Add(403700);
                        shp.Add(403710);
                        shp.Add(403720);
                        shp.Add(403730);
                        shp.Add(403750);
                        shp.Add(403760);
                        shp.Add(403770);
                        shp.Add(403790);
                        shp.Add(403800);
                        shp.Add(403810);
                        shp.Add(403820);
                        shp.Add(403830);
                        shp.Add(403840);
                        shp.Add(403850);
                        shp.Add(403860);
                        shp.Add(403870);
                        shp.Add(403880);
                        shp.Add(403890);
                        shp.Add(403900);
                        shp.Add(403910);
                        shp.Add(403920);
                        shp.Add(403930);
                        shp.Add(403940);
                        shp.Add(403950);
                        shp.Add(403960);
                        shp.Add(403970);
                        shp.Add(403980);
                        shp.Add(403990);
                        shp.Add(404000);
                        shp.Add(404010);
                        shp.Add(404020);
                        shp.Add(404030);
                        shp.Add(404040);
                        shp.Add(404050);
                        shp.Add(404060);
                        shp.Add(404070);
                        shp.Add(404080);
                        shp.Add(404090);
                        shp.Add(404100);
                        shp.Add(404120);
                        shp.Add(404130);
                        shp.Add(404140);
                        shp.Add(404160);
                        shp.Add(404150);
                        shp.Add(404170);
                        shp.Add(404180);
                        shp.Add(404190);
                        shp.Add(404200);
                        shp.Add(404210);
                        shp.Add(404230);
                        shp.Add(404240);
                        shp.Add(404250);
                        shp.Add(404260);
                        shp.Add(404270);
                        shp.Add(404280);
                        shp.Add(404300);
                        shp.Add(404310);
                        shp.Add(404320);
                        shp.Add(404340);
                        shp.Add(404350);
                        shp.Add(404360);
                        shp.Add(404370);
                        shp.Add(404380);
                        shp.Add(404390);
                        shp.Add(404410);
                        shp.Add(404420);
                        shp.Add(404440);
                        shp.Add(404450);
                        shp.Add(404460);
                        shp.Add(404470);
                        shp.Add(404480);
                        shp.Add(404490);
                        shp.Add(404500);
                        shp.Add(404510);
                        shp.Add(404520);
                        shp.Add(404530);
                        shp.Add(404540);
                        shp.Add(404550);
                        shp.Add(404560);
                        shp.Add(404570);
                        shp.Add(404580);
                        shp.Add(404590);
                        shp.Add(404600);
                        shp.Add(404610);
                        shp.Add(404620);
                        shp.Add(404630);
                        shp.Add(404640);
                        shp.Add(404670);
                        shp.Add(404690);
                        shp.Add(404680);
                        shp.Add(404700);
                        shp.Add(404710);
                        shp.Add(404720);
                        shp.Add(404730);
                        shp.Add(404740);
                        shp.Add(404750);
                        shp.Add(404760);
                        shp.Add(404770);
                        shp.Add(404780);
                        shp.Add(404790);
                        shp.Add(404920);
                        shp.Add(404810);
                        shp.Add(404820);
                        shp.Add(404840);
                        shp.Add(404830);
                        shp.Add(404850);
                        shp.Add(404860);
                        shp.Add(404880);
                        shp.Add(404890);
                        shp.Add(404900);
                        shp.Add(404910);
                        shp.Add(402970);
                        shp.Add(403030);
                        shp.Add(403100);
                        shp.Add(403320);
                        shp.Add(403740);
                        shp.Add(403780);
                        shp.Add(404220);
                        shp.Add(404290);
                        shp.Add(404330);
                        shp.Add(404400);
                        shp.Add(404430);
                        shp.Add(404650);
                        shp.Add(404660);
                        shp.Add(404870);
                        shp.Add(404800);
                        shp.Add(404930);
                        shp.Add(400911);
                        shp.Add(403083);
                        shp.Add(403082);
                        shp.Add(403081);
                        shp.Add(400912);
                        shp.Add(404940);
                        shp.Add(600010);
                        shp.Add(600020);
                        shp.Add(600030);
                        shp.Add(600040);
                        shp.Add(600050);
                        shp.Add(600060);
                        shp.Add(600070);
                        shp.Add(600080);
                        shp.Add(600090);
                        shp.Add(600100);
                        shp.Add(600110);
                        shp.Add(600120);
                        shp.Add(600130);
                        shp.Add(600140);
                        shp.Add(600150);
                        shp.Add(600160);
                        shp.Add(600170);
                        shp.Add(600180);
                        shp.Add(600190);
                        shp.Add(600200);
                        shp.Add(600210);
                        shp.Add(600220);
                        shp.Add(600230);
                        shp.Add(600240);
                        shp.Add(600250);
                        shp.Add(600260);
                        shp.Add(600270);
                        shp.Add(600280);
                        shp.Add(600290);
                        shp.Add(600300);
                        shp.Add(600310);
                        shp.Add(600320);
                        shp.Add(600330);
                        shp.Add(600340);
                        shp.Add(600351);
                        shp.Add(600352);
                        shp.Add(600360);
                        shp.Add(600370);
                        shp.Add(600380);
                        shp.Add(600390);
                        shp.Add(600400);
                        shp.Add(600410);
                        shp.Add(600420);
                        shp.Add(600430);
                        shp.Add(600440);
                        shp.Add(600450);
                        shp.Add(600460);
                        shp.Add(600470);
                        shp.Add(600480);
                        shp.Add(600490);
                        shp.Add(600500);
                        shp.Add(600510);
                        shp.Add(600520);
                        shp.Add(600530);
                        shp.Add(600540);
                        shp.Add(600550);
                        shp.Add(600560);
                        shp.Add(600570);
                        shp.Add(600580);
                        shp.Add(600590);
                        shp.Add(600600);
                        shp.Add(600610);
                        shp.Add(600620);
                        shp.Add(600630);
                        shp.Add(600640);
                        shp.Add(600650);
                        shp.Add(600660);
                        shp.Add(600670);
                        shp.Add(600680);
                        shp.Add(600690);
                        shp.Add(600700);
                        shp.Add(600710);
                        shp.Add(600720);
                        shp.Add(600730);
                        shp.Add(600740);
                        shp.Add(600750);
                        shp.Add(600760);
                        shp.Add(600770);
                        shp.Add(600780);
                        shp.Add(600790);
                        shp.Add(600800);
                        shp.Add(600810);
                        shp.Add(600820);
                        shp.Add(600830);
                        shp.Add(600840);
                        shp.Add(600850);
                        shp.Add(600860);
                        shp.Add(600870);
                        shp.Add(600880);
                        shp.Add(600890);
                        shp.Add(600910);
                        shp.Add(600920);
                        shp.Add(600930);
                        shp.Add(600940);
                        shp.Add(600950);
                        shp.Add(600960);
                        shp.Add(601020);
                        shp.Add(601030);
                        shp.Add(601040);
                        shp.Add(601050);
                        shp.Add(601060);
                        shp.Add(601070);
                        shp.Add(601080);
                        shp.Add(601090);
                        shp.Add(601100);
                        shp.Add(601110);
                        shp.Add(601120);
                        shp.Add(601130);
                        shp.Add(601140);
                        shp.Add(601150);
                        shp.Add(601160);
                        shp.Add(601170);
                        shp.Add(601180);
                        shp.Add(601000);
                        shp.Add(601190);
                        shp.Add(601200);
                        shp.Add(601210);
                        shp.Add(601220);
                        shp.Add(601230);
                        shp.Add(601240);
                        shp.Add(601250);
                        shp.Add(601260);
                        shp.Add(601270);
                        shp.Add(601280);
                        shp.Add(601290);
                        shp.Add(601300);
                        shp.Add(601310);
                        shp.Add(600990);
                        shp.Add(600980);
                        shp.Add(601320);
                        shp.Add(601330);
                        shp.Add(601340);
                        shp.Add(600970);
                        shp.Add(601010);
                        shp.Add(601350);
                        shp.Add(601360);
                        shp.Add(601370);
                        shp.Add(601380);
                        shp.Add(601390);
                        shp.Add(601400);
                        shp.Add(601410);
                        shp.Add(601420);
                        shp.Add(601430);
                        shp.Add(601440);
                        shp.Add(601450);
                        shp.Add(601460);
                        shp.Add(601470);
                        shp.Add(601480);
                        shp.Add(601490);
                        shp.Add(601500);
                        shp.Add(601510);
                        shp.Add(601530);
                        shp.Add(601540);
                        shp.Add(601550);
                        shp.Add(601560);
                        shp.Add(601570);
                        shp.Add(601580);
                        shp.Add(601600);
                        shp.Add(601610);
                        shp.Add(601620);
                        shp.Add(601630);
                        shp.Add(601640);
                        shp.Add(601650);
                        shp.Add(601660);
                        shp.Add(601670);
                        shp.Add(601690);
                        shp.Add(601700);
                        shp.Add(601710);
                        shp.Add(601720);
                        shp.Add(601730);
                        shp.Add(601740);
                        shp.Add(601750);
                        shp.Add(601760);
                        shp.Add(601770);
                        shp.Add(601780);
                        shp.Add(601790);
                        shp.Add(601800);
                        shp.Add(601810);
                        shp.Add(601820);
                        shp.Add(601830);
                        shp.Add(601840);
                        shp.Add(601850);
                        shp.Add(601860);
                        shp.Add(601870);
                        shp.Add(601880);
                        shp.Add(601890);
                        shp.Add(601900);
                        shp.Add(601910);
                        shp.Add(601920);
                        shp.Add(601930);
                        shp.Add(601520);
                        shp.Add(601590);
                        shp.Add(601680);
                        shp.Add(601950);
                        shp.Add(601960);
                        shp.Add(601970);
                        shp.Add(601980);
                        shp.Add(602000);
                        shp.Add(602010);
                        shp.Add(602020);
                        shp.Add(602030);
                        shp.Add(602040);
                        shp.Add(602050);
                        shp.Add(602060);
                        shp.Add(602070);
                        shp.Add(601990);
                        shp.Add(602080);
                        shp.Add(602090);
                        shp.Add(602100);
                        shp.Add(602110);
                        shp.Add(602120);
                        shp.Add(602130);
                        shp.Add(602140);
                        shp.Add(602150);
                        shp.Add(602160);
                        shp.Add(602170);
                        shp.Add(602180);
                        shp.Add(602190);
                        shp.Add(602200);
                        shp.Add(602210);
                        shp.Add(602221);
                        shp.Add(602222);
                        shp.Add(602230);
                        shp.Add(602240);
                        shp.Add(602260);
                        shp.Add(602250);
                        shp.Add(602270);
                        shp.Add(602280);
                        shp.Add(602290);
                        shp.Add(602300);
                        shp.Add(602310);
                        shp.Add(602320);
                        shp.Add(602330);
                        shp.Add(602340);
                        shp.Add(602350);
                        shp.Add(602360);
                        shp.Add(602370);
                        shp.Add(602470);
                        shp.Add(602380);
                        shp.Add(602390);
                        shp.Add(602400);
                        shp.Add(602410);
                        shp.Add(602420);
                        shp.Add(602430);
                        shp.Add(600900);
                        shp.Add(602450);
                        shp.Add(602460);
                        shp.Add(602480);
                        shp.Add(602490);
                        shp.Add(602500);
                        shp.Add(602510);
                        shp.Add(602520);
                        shp.Add(602530);
                        shp.Add(602540);
                        shp.Add(602550);
                        shp.Add(602560);
                        shp.Add(602570);
                        shp.Add(602580);
                        shp.Add(602590);
                        shp.Add(602600);
                        shp.Add(602440);
                        shp.Add(601940);
                        shp.Add(700250);
                        shp.Add(704520);
                        shp.Add(705440);
                        shp.Add(705470);
                        shp.Add(705460);
                        shp.Add(705480);
                        shp.Add(705490);
                        shp.Add(705500);
                        shp.Add(705510);
                        shp.Add(705520);
                        shp.Add(705530);
                        shp.Add(705550);
                        shp.Add(705560);
                        shp.Add(705570);
                        shp.Add(705580);
                        shp.Add(705590);
                        shp.Add(705600);
                        shp.Add(705610);
                        shp.Add(705620);
                        shp.Add(705630);
                        shp.Add(705640);
                        shp.Add(705650);
                        shp.Add(705660);
                        shp.Add(705670);
                        shp.Add(705680);
                        shp.Add(705690);
                        shp.Add(705700);
                        shp.Add(705710);
                        shp.Add(705730);
                        shp.Add(705740);
                        shp.Add(705750);
                        shp.Add(705760);
                        shp.Add(705770);
                        shp.Add(705780);
                        shp.Add(705790);
                        shp.Add(705800);
                        shp.Add(705820);
                        shp.Add(705840);
                        shp.Add(705850);
                        shp.Add(705860);
                        shp.Add(705870);
                        shp.Add(705880);
                        shp.Add(705890);
                        shp.Add(700260);
                        shp.Add(705450);
                        shp.Add(705900);
                        shp.Add(705910);
                        shp.Add(705920);
                        shp.Add(705940);
                        shp.Add(705950);
                        shp.Add(705960);
                        shp.Add(705970);
                        shp.Add(705980);
                        shp.Add(705990);
                        shp.Add(706002);
                        shp.Add(706001);
                        shp.Add(706010);
                        shp.Add(706020);
                        shp.Add(706040);
                        shp.Add(706050);
                        shp.Add(706060);
                        shp.Add(706080);
                        shp.Add(706090);
                        shp.Add(706070);
                        shp.Add(706100);
                        shp.Add(706110);
                        shp.Add(706120);
                        shp.Add(706130);
                        shp.Add(706140);
                        shp.Add(706150);
                        shp.Add(706160);
                        shp.Add(706170);
                        shp.Add(706190);
                        shp.Add(706200);
                        shp.Add(706210);
                        shp.Add(706240);
                        shp.Add(706250);
                        shp.Add(706260);
                        shp.Add(706270);
                        shp.Add(706290);
                        shp.Add(706300);
                        shp.Add(706310);
                        shp.Add(706320);
                        shp.Add(706330);
                        shp.Add(706340);
                        shp.Add(706350);
                        shp.Add(706360);
                        shp.Add(706370);
                        shp.Add(706380);
                        shp.Add(706390);
                        shp.Add(706400);
                        shp.Add(706410);
                        shp.Add(706420);
                        shp.Add(706430);
                        shp.Add(706440);
                        shp.Add(706450);
                        shp.Add(706460);
                        shp.Add(706470);
                        shp.Add(706480);
                        shp.Add(706490);
                        shp.Add(706500);
                        shp.Add(706510);
                        shp.Add(706550);
                        shp.Add(706560);
                        shp.Add(706570);
                        shp.Add(706580);
                        shp.Add(706590);
                        shp.Add(706600);
                        shp.Add(706610);
                        shp.Add(706630);
                        shp.Add(706030);
                        shp.Add(706640);
                        shp.Add(706650);
                        shp.Add(706660);
                        shp.Add(706670);
                        shp.Add(706230);
                        shp.Add(700010);
                        shp.Add(700020);
                        shp.Add(700030);
                        shp.Add(700040);
                        shp.Add(700050);
                        shp.Add(700060);
                        shp.Add(700070);
                        shp.Add(700080);
                        shp.Add(700090);
                        shp.Add(700100);
                        shp.Add(700110);
                        shp.Add(700120);
                        shp.Add(700130);
                        shp.Add(700140);
                        shp.Add(700160);
                        shp.Add(700170);
                        shp.Add(700180);
                        shp.Add(700190);
                        shp.Add(700200);
                        shp.Add(700220);
                        shp.Add(700230);
                        shp.Add(700240);
                        shp.Add(700280);
                        shp.Add(700290);
                        shp.Add(705150);
                        shp.Add(700310);
                        shp.Add(700330);
                        shp.Add(700340);
                        shp.Add(700350);
                        shp.Add(700360);
                        shp.Add(700370);
                        shp.Add(700380);
                        shp.Add(700390);
                        shp.Add(700400);
                        shp.Add(700410);
                        shp.Add(700420);
                        shp.Add(700430);
                        shp.Add(700450);
                        shp.Add(700460);
                        shp.Add(700480);
                        shp.Add(700490);
                        shp.Add(700500);
                        shp.Add(700510);
                        shp.Add(700530);
                        shp.Add(700540);
                        shp.Add(700550);
                        shp.Add(700560);
                        shp.Add(700580);
                        shp.Add(700600);
                        shp.Add(700610);
                        shp.Add(700620);
                        shp.Add(700640);
                        shp.Add(700650);
                        shp.Add(700660);
                        shp.Add(700670);
                        shp.Add(700680);
                        shp.Add(700690);
                        shp.Add(700700);
                        shp.Add(700710);
                        shp.Add(700720);
                        shp.Add(700730);
                        shp.Add(700740);
                        shp.Add(700750);
                        shp.Add(700770);
                        shp.Add(700780);
                        shp.Add(700790);
                        shp.Add(700810);
                        shp.Add(700820);
                        shp.Add(700830);
                        shp.Add(700840);
                        shp.Add(700850);
                        shp.Add(700860);
                        shp.Add(700870);
                        shp.Add(700880);
                        shp.Add(700890);
                        shp.Add(700920);
                        shp.Add(700930);
                        shp.Add(700940);
                        shp.Add(700950);
                        shp.Add(700960);
                        shp.Add(700970);
                        shp.Add(700980);
                        shp.Add(700990);
                        shp.Add(701000);
                        shp.Add(701010);
                        shp.Add(701020);
                        shp.Add(701030);
                        shp.Add(701040);
                        shp.Add(701050);
                        shp.Add(701060);
                        shp.Add(701070);
                        shp.Add(701080);
                        shp.Add(701090);
                        shp.Add(701100);
                        shp.Add(701110);
                        shp.Add(701120);
                        shp.Add(701130);
                        shp.Add(701140);
                        shp.Add(701150);
                        shp.Add(701160);
                        shp.Add(701170);
                        shp.Add(701180);
                        shp.Add(701190);
                        shp.Add(701200);
                        shp.Add(701210);
                        shp.Add(701220);
                        shp.Add(701230);
                        shp.Add(701240);
                        shp.Add(701250);
                        shp.Add(701260);
                        shp.Add(701270);
                        shp.Add(701280);
                        shp.Add(701290);
                        shp.Add(701300);
                        shp.Add(701310);
                        shp.Add(701330);
                        shp.Add(701340);
                        shp.Add(701350);
                        shp.Add(701360);
                        shp.Add(701370);
                        shp.Add(701380);
                        shp.Add(701390);
                        shp.Add(701400);
                        shp.Add(701420);
                        shp.Add(701440);
                        shp.Add(701450);
                        shp.Add(701460);
                        shp.Add(701470);
                        shp.Add(701480);
                        shp.Add(701490);
                        shp.Add(701500);
                        shp.Add(701510);
                        shp.Add(701520);
                        shp.Add(701530);
                        shp.Add(701540);
                        shp.Add(701560);
                        shp.Add(701570);
                        shp.Add(701580);
                        shp.Add(701590);
                        shp.Add(701600);
                        shp.Add(701610);
                        shp.Add(701620);
                        shp.Add(701630);
                        shp.Add(701640);
                        shp.Add(701650);
                        shp.Add(701660);
                        shp.Add(701680);
                        shp.Add(701690);
                        shp.Add(701700);
                        shp.Add(701710);
                        shp.Add(701720);
                        shp.Add(701730);
                        shp.Add(701740);
                        shp.Add(701750);
                        shp.Add(701760);
                        shp.Add(701780);
                        shp.Add(701790);
                        shp.Add(701800);
                        shp.Add(701810);
                        shp.Add(701820);
                        shp.Add(701830);
                        shp.Add(701840);
                        shp.Add(701850);
                        shp.Add(701860);
                        shp.Add(701870);
                        shp.Add(701880);
                        shp.Add(701890);
                        shp.Add(701900);
                        shp.Add(701910);
                        shp.Add(701930);
                        shp.Add(701940);
                        shp.Add(701950);
                        shp.Add(701960);
                        shp.Add(701970);
                        shp.Add(701980);
                        shp.Add(701990);
                        shp.Add(702000);
                        shp.Add(702010);
                        shp.Add(702020);
                        shp.Add(702030);
                        shp.Add(702040);
                        shp.Add(702050);
                        shp.Add(702060);
                        shp.Add(702070);
                        shp.Add(702080);
                        shp.Add(702090);
                        shp.Add(702100);
                        shp.Add(702110);
                        shp.Add(702120);
                        shp.Add(702130);
                        shp.Add(702140);
                        shp.Add(702150);
                        shp.Add(702160);
                        shp.Add(702170);
                        shp.Add(702180);
                        shp.Add(702190);
                        shp.Add(702200);
                        shp.Add(702210);
                        shp.Add(702220);
                        shp.Add(702230);
                        shp.Add(702240);
                        shp.Add(702250);
                        shp.Add(702270);
                        shp.Add(702260);
                        shp.Add(702280);
                        shp.Add(702290);
                        shp.Add(702300);
                        shp.Add(702310);
                        shp.Add(702320);
                        shp.Add(702330);
                        shp.Add(702340);
                        shp.Add(702350);
                        shp.Add(702360);
                        shp.Add(702370);
                        shp.Add(702390);
                        shp.Add(702420);
                        shp.Add(702400);
                        shp.Add(702410);
                        shp.Add(702430);
                        shp.Add(702440);
                        shp.Add(702450);
                        shp.Add(702460);
                        shp.Add(702470);
                        shp.Add(702480);
                        shp.Add(702490);
                        shp.Add(702500);
                        shp.Add(702510);
                        shp.Add(704150);
                        shp.Add(702520);
                        shp.Add(702530);
                        shp.Add(702540);
                        shp.Add(702550);
                        shp.Add(702560);
                        shp.Add(702570);
                        shp.Add(702580);
                        shp.Add(702590);
                        shp.Add(702600);
                        shp.Add(702610);
                        shp.Add(702630);
                        shp.Add(702640);
                        shp.Add(702650);
                        shp.Add(702660);
                        shp.Add(702670);
                        shp.Add(702680);
                        shp.Add(702690);
                        shp.Add(702700);
                        shp.Add(702710);
                        shp.Add(702740);
                        shp.Add(702750);
                        shp.Add(702760);
                        shp.Add(702770);
                        shp.Add(702780);
                        shp.Add(702790);
                        shp.Add(702800);
                        shp.Add(702810);
                        shp.Add(702820);
                        shp.Add(702830);
                        shp.Add(702840);
                        shp.Add(702850);
                        shp.Add(702860);
                        shp.Add(702870);
                        shp.Add(702880);
                        shp.Add(702890);
                        shp.Add(702900);
                        shp.Add(702910);
                        shp.Add(702920);
                        shp.Add(702930);
                        shp.Add(702940);
                        shp.Add(702950);
                        shp.Add(702960);
                        shp.Add(702970);
                        shp.Add(702980);
                        shp.Add(702990);
                        shp.Add(703000);
                        shp.Add(703010);
                        shp.Add(703020);
                        shp.Add(703030);
                        shp.Add(703040);
                        shp.Add(703050);
                        shp.Add(703060);
                        shp.Add(703070);
                        shp.Add(703080);
                        shp.Add(703090);
                        shp.Add(703100);
                        shp.Add(703110);
                        shp.Add(703120);
                        shp.Add(703140);
                        shp.Add(703130);
                        shp.Add(703150);
                        shp.Add(703160);
                        shp.Add(703180);
                        shp.Add(703190);
                        shp.Add(703200);
                        shp.Add(703210);
                        shp.Add(703220);
                        shp.Add(703230);
                        shp.Add(703240);
                        shp.Add(703250);
                        shp.Add(703260);
                        shp.Add(703270);
                        shp.Add(703280);
                        shp.Add(703290);
                        shp.Add(703300);
                        shp.Add(703310);
                        shp.Add(703330);
                        shp.Add(703340);
                        shp.Add(703360);
                        shp.Add(703370);
                        shp.Add(703380);
                        shp.Add(703390);
                        shp.Add(703410);
                        shp.Add(703420);
                        shp.Add(703440);
                        shp.Add(703450);
                        shp.Add(703460);
                        shp.Add(703470);
                        shp.Add(703480);
                        shp.Add(703490);
                        shp.Add(703500);
                        shp.Add(703510);
                        shp.Add(703520);
                        shp.Add(703530);
                        shp.Add(703540);
                        shp.Add(703550);
                        shp.Add(703560);
                        shp.Add(703580);
                        shp.Add(703600);
                        shp.Add(703610);
                        shp.Add(703620);
                        shp.Add(703640);
                        shp.Add(703650);
                        shp.Add(703660);
                        shp.Add(703670);
                        shp.Add(703690);
                        shp.Add(703700);
                        shp.Add(703710);
                        shp.Add(703720);
                        shp.Add(703730);
                        shp.Add(703740);
                        shp.Add(703750);
                        shp.Add(703780);
                        shp.Add(703790);
                        shp.Add(703800);
                        shp.Add(703810);
                        shp.Add(703820);
                        shp.Add(703830);
                        shp.Add(703840);
                        shp.Add(703850);
                        shp.Add(703860);
                        shp.Add(703870);
                        shp.Add(703880);
                        shp.Add(703890);
                        shp.Add(703900);
                        shp.Add(703910);
                        shp.Add(703920);
                        shp.Add(703930);
                        shp.Add(703940);
                        shp.Add(703950);
                        shp.Add(703960);
                        shp.Add(703980);
                        shp.Add(704000);
                        shp.Add(704010);
                        shp.Add(704020);
                        shp.Add(704030);
                        shp.Add(704040);
                        shp.Add(704050);
                        shp.Add(704060);
                        shp.Add(704070);
                        shp.Add(704080);
                        shp.Add(704090);
                        shp.Add(704100);
                        shp.Add(704110);
                        shp.Add(704120);
                        shp.Add(704130);
                        shp.Add(704140);
                        shp.Add(704160);
                        shp.Add(704170);
                        shp.Add(704180);
                        shp.Add(704190);
                        shp.Add(704200);
                        shp.Add(704210);
                        shp.Add(704220);
                        shp.Add(704230);
                        shp.Add(704240);
                        shp.Add(704250);
                        shp.Add(704260);
                        shp.Add(704270);
                        shp.Add(704280);
                        shp.Add(704300);
                        shp.Add(704320);
                        shp.Add(704290);
                        shp.Add(704310);
                        shp.Add(704330);
                        shp.Add(704340);
                        shp.Add(704350);
                        shp.Add(704360);
                        shp.Add(704370);
                        shp.Add(704380);
                        shp.Add(704390);
                        shp.Add(704400);
                        shp.Add(704410);
                        shp.Add(704420);
                        shp.Add(704430);
                        shp.Add(704440);
                        shp.Add(704450);
                        shp.Add(704460);
                        shp.Add(704470);
                        shp.Add(704480);
                        shp.Add(704490);
                        shp.Add(704500);
                        shp.Add(704510);
                        shp.Add(704530);
                        shp.Add(704540);
                        shp.Add(704570);
                        shp.Add(704550);
                        shp.Add(704560);
                        shp.Add(704580);
                        shp.Add(704590);
                        shp.Add(704600);
                        shp.Add(704610);
                        shp.Add(704620);
                        shp.Add(704630);
                        shp.Add(704640);
                        shp.Add(704650);
                        shp.Add(704660);
                        shp.Add(704670);
                        shp.Add(704680);
                        shp.Add(704690);
                        shp.Add(704700);
                        shp.Add(704710);
                        shp.Add(704720);
                        shp.Add(704730);
                        shp.Add(704740);
                        shp.Add(704760);
                        shp.Add(704770);
                        shp.Add(704780);
                        shp.Add(704790);
                        shp.Add(704800);
                        shp.Add(704810);
                        shp.Add(704820);
                        shp.Add(704830);
                        shp.Add(704840);
                        shp.Add(704850);
                        shp.Add(704860);
                        shp.Add(704880);
                        shp.Add(704890);
                        shp.Add(704901);
                        shp.Add(704902);
                        shp.Add(704910);
                        shp.Add(704920);
                        shp.Add(704930);
                        shp.Add(704940);
                        shp.Add(704950);
                        shp.Add(704960);
                        shp.Add(704970);
                        shp.Add(704980);
                        shp.Add(704990);
                        shp.Add(705000);
                        shp.Add(705010);
                        shp.Add(705020);
                        shp.Add(705030);
                        shp.Add(705040);
                        shp.Add(705050);
                        shp.Add(705060);
                        shp.Add(705070);
                        shp.Add(705080);
                        shp.Add(705090);
                        shp.Add(705100);
                        shp.Add(705110);
                        shp.Add(705120);
                        shp.Add(705130);
                        shp.Add(705140);
                        shp.Add(700300);
                        shp.Add(705160);
                        shp.Add(705170);
                        shp.Add(705180);
                        shp.Add(705190);
                        shp.Add(705200);
                        shp.Add(705210);
                        shp.Add(705220);
                        shp.Add(705230);
                        shp.Add(705240);
                        shp.Add(705250);
                        shp.Add(705260);
                        shp.Add(705270);
                        shp.Add(705280);
                        shp.Add(705290);
                        shp.Add(705300);
                        shp.Add(705310);
                        shp.Add(705320);
                        shp.Add(705330);
                        shp.Add(705340);
                        shp.Add(705350);
                        shp.Add(705370);
                        shp.Add(705360);
                        shp.Add(705380);
                        shp.Add(700210);
                        shp.Add(705410);
                        shp.Add(700520);
                        shp.Add(705420);
                        shp.Add(703170);
                        shp.Add(703590);
                        shp.Add(703630);
                        shp.Add(703760);
                        shp.Add(703771);
                        shp.Add(703772);
                        shp.Add(700150);
                        shp.Add(705430);
                        shp.Add(705540);
                        shp.Add(705720);
                        shp.Add(705810);
                        shp.Add(705830);
                        shp.Add(705930);
                        shp.Add(706180);
                        shp.Add(706220);
                        shp.Add(706280);
                        shp.Add(706520);
                        shp.Add(706530);
                        shp.Add(706540);
                        shp.Add(700320);
                        shp.Add(700440);
                        shp.Add(700470);
                        shp.Add(700570);
                        shp.Add(700590);
                        shp.Add(700630);
                        shp.Add(700760);
                        shp.Add(700800);
                        shp.Add(700900);
                        shp.Add(700910);
                        shp.Add(701430);
                        shp.Add(701550);
                        shp.Add(701670);
                        shp.Add(701770);
                        shp.Add(701920);
                        shp.Add(702380);
                        shp.Add(702620);
                        shp.Add(702720);
                        shp.Add(702730);
                        shp.Add(703320);
                        shp.Add(703350);
                        shp.Add(703400);
                        shp.Add(703430);
                        shp.Add(703570);
                        shp.Add(703990);
                        shp.Add(703970);
                        shp.Add(704750);
                        shp.Add(704870);
                        shp.Add(706830);
                        shp.Add(703680);
                        shp.Add(706680);
                        shp.Add(706690);
                        shp.Add(706700);
                        shp.Add(706710);
                        shp.Add(706720);
                        shp.Add(706730);
                        shp.Add(706740);
                        shp.Add(706750);
                        shp.Add(706760);
                        shp.Add(706770);
                        shp.Add(706780);
                        shp.Add(706790);
                        shp.Add(706800);
                        shp.Add(706810);
                        shp.Add(706820);
                        shp.Add(706840);
                        shp.Add(706850);
                        shp.Add(706860);
                        shp.Add(706870);
                        shp.Add(706880);
                        shp.Add(706890);
                        shp.Add(706900);
                        shp.Add(706910);
                        shp.Add(706920);
                        shp.Add(706930);
                        shp.Add(706940);
                        shp.Add(706950);
                        shp.Add(706960);
                        shp.Add(706970);
                        shp.Add(701320);
                        shp.Add(701410);
                        shp.Add(705390);
                        shp.Add(705400);
                        shp.Add(706980);
                        shp.Add(706990);
                        shp.Add(800010);
                        shp.Add(800020);
                        shp.Add(800050);
                        shp.Add(800060);
                        shp.Add(800070);
                        shp.Add(800080);
                        shp.Add(800090);
                        shp.Add(800100);
                        shp.Add(800110);
                        shp.Add(800120);
                        shp.Add(800130);
                        shp.Add(800140);
                        shp.Add(800150);
                        shp.Add(800160);
                        shp.Add(800170);
                        shp.Add(800180);
                        shp.Add(800190);
                        shp.Add(800200);
                        shp.Add(800210);
                        shp.Add(800220);
                        shp.Add(800230);
                        shp.Add(800250);
                        shp.Add(800240);
                        shp.Add(800280);
                        shp.Add(800290);
                        shp.Add(800260);
                        shp.Add(800270);
                        shp.Add(800300);
                        shp.Add(800310);
                        shp.Add(800030);
                        shp.Add(800320);
                        shp.Add(800330);
                        shp.Add(800340);
                        shp.Add(800350);
                        shp.Add(800370);
                        shp.Add(800380);
                        shp.Add(800400);
                        shp.Add(800390);
                        shp.Add(800410);
                        shp.Add(800420);
                        shp.Add(800430);
                        shp.Add(800440);
                        shp.Add(800040);
                        shp.Add(800360);
                        shp.Add(100010);
                        shp.Add(100020);
                        shp.Add(100030);
                        shp.Add(100050);
                        shp.Add(100040);
                        shp.Add(100060);
                        shp.Add(100080);
                        shp.Add(100070);
                        shp.Add(100090);
                        shp.Add(100100);
                        shp.Add(100120);
                        shp.Add(100130);
                        shp.Add(100160);
                        shp.Add(100150);
                        shp.Add(100170);
                        shp.Add(100180);
                        shp.Add(100190);
                        shp.Add(100200);
                        shp.Add(100220);
                        shp.Add(100230);
                        shp.Add(100240);
                        shp.Add(100250);
                        shp.Add(100270);
                        shp.Add(100280);
                        shp.Add(100300);
                        shp.Add(100310);
                        shp.Add(100330);
                        shp.Add(100350);
                        shp.Add(100360);
                        shp.Add(100370);
                        shp.Add(100860);
                        shp.Add(100380);
                        shp.Add(100390);
                        shp.Add(100400);
                        shp.Add(100410);
                        shp.Add(100420);
                        shp.Add(100440);
                        shp.Add(100450);
                        shp.Add(100470);
                        shp.Add(100490);
                        shp.Add(100500);
                        shp.Add(100520);
                        shp.Add(100530);
                        shp.Add(100540);
                        shp.Add(100560);
                        shp.Add(100580);
                        shp.Add(100610);
                        shp.Add(100620);
                        shp.Add(100650);
                        shp.Add(100660);
                        shp.Add(100690);
                        shp.Add(100700);
                        shp.Add(100740);
                        shp.Add(100750);
                        shp.Add(100790);
                        shp.Add(100800);
                        shp.Add(100810);
                        shp.Add(100820);
                        shp.Add(100830);
                        shp.Add(100840);
                        shp.Add(100880);
                        shp.Add(100890);
                        shp.Add(100290);
                        shp.Add(100460);
                        shp.Add(100670);
                        shp.Add(100680);
                        shp.Add(100780);
                        shp.Add(100910);
                        shp.Add(100110);
                        shp.Add(100140);
                        shp.Add(100210);
                        shp.Add(100260);
                        shp.Add(100320);
                        shp.Add(100340);
                        shp.Add(100430);
                        shp.Add(100480);
                        shp.Add(100510);
                        shp.Add(100551);
                        shp.Add(100553);
                        shp.Add(100554);
                        shp.Add(100552);
                        shp.Add(100572);
                        shp.Add(100571);
                        shp.Add(100590);
                        shp.Add(100603);
                        shp.Add(100604);
                        shp.Add(100602);
                        shp.Add(100601);
                        shp.Add(100630);
                        shp.Add(100640);
                        shp.Add(100710);
                        shp.Add(100720);
                        shp.Add(100730);
                        shp.Add(100760);
                        shp.Add(100770);
                        shp.Add(100853);
                        shp.Add(100852);
                        shp.Add(100851);
                        shp.Add(100870);
                        shp.Add(100900);
                        shp.Add(100920);
                        shp.Add(100930);
                        shp.Add(100940);

                    }
                    
                    List<LakeData> waterLevels = new List<LakeData>();
                    foreach(WaterLevel waterLevel in _context.WaterLevel)
                    {
                        waterLevels.Add(new LakeData() {
                            LakeId = waterLevel.LakeId,
                            Year = waterLevel.Year,
                            Value = waterLevel.WaterLavelM
                        });
                    }

                    for(int year = waterLevels.Min(w => w.Year); year <= waterLevels.Max(w => w.Year); year++)
                    {
                        string styleText = CreateStyleForLakes(waterLevels.ToArray(), shp.ToArray(), year);
                        CreateStyle(Startup.Configuration["GeoServer:Workspace"], $"WaterLevels_{year.ToString()}", styleText);
                        //PublishShape(Startup.Configuration["GeoServer:Workspace"], ShapeFile, $"WaterLevels_{year.ToString()}", $"WaterLevels_{year.ToString()}");
                    }
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

        public struct LakeData
        {
            public int LakeId;
            public int Year;
            public decimal Value;
        }

        public string CreateStyleForLakes(LakeData[] LakeDatas, int[] Shp, int Year)
        {
            string style = "<?xml version=\"1.0\" encoding=\"ISO-8859-1\"?>" +
                "<StyledLayerDescriptor version=\"1.0.0\"" +
                "  xsi:schemaLocation=\"http://www.opengis.net/sld http://schemas.opengis.net/sld/1.0.0/StyledLayerDescriptor.xsd\"" +
                "  xmlns=\"http://www.opengis.net/sld\" xmlns:ogc=\"http://www.opengis.net/ogc\"" +
                "  xmlns:xlink=\"http://www.w3.org/1999/xlink\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">" +
                "  <NamedLayer>" +
                "    <Name>polygon</Name>" +
                "    <UserStyle>" +
                "      <FeatureTypeStyle>" +
                "        <Rule>" +
                "          <PolygonSymbolizer>" +
                "            <Fill>" +
                "              <CssParameter name=\"fill\">#ffffff</CssParameter>" +
                "            </Fill>" +
                "            <Stroke>" +
                "              <CssParameter name=\"stroke\">#000000</CssParameter>" +
                "              <CssParameter name=\"stroke-width\">0.5</CssParameter>" +
                "            </Stroke>" +
                "          </PolygonSymbolizer>" +
                "        </Rule>" +
                "        <Rule>" +
                "          <ogc:Filter xmlns:ogc=\"http://www.opengis.net/ogc\">" +
                "            <ogc:PropertyIsEqualTo>" +
                "              <ogc:PropertyName>id</ogc:PropertyName>" +
                "              <ogc:Literal>601810</ogc:Literal>" +
                "            </ogc:PropertyIsEqualTo>" +
                "          </ogc:Filter>" +
                "          <PolygonSymbolizer>" +
                "            <Fill>" +
                "              <CssParameter name=\"fill\">#ad5200</CssParameter>" +
                "            </Fill>" +
                "            <Stroke>" +
                "              <CssParameter name=\"stroke\">#000000</CssParameter>" +
                "              <CssParameter name=\"stroke-width\">0.5</CssParameter>" +
                "            </Stroke>" +
                "          </PolygonSymbolizer>" +
                "        </Rule>" +
                "      </FeatureTypeStyle>" +
                "    </UserStyle>" +
                "  </NamedLayer>" +
                "</StyledLayerDescriptor>";
            return style;
        }
    }
}