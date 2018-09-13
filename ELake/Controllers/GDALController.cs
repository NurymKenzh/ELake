﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;

namespace ELake.Controllers
{
    public class GDALController : Controller
    {
        private IHostingEnvironment _hostingEnvironment;

        public GDALController(IHostingEnvironment hostingEnvironment)
        {
            _hostingEnvironment = hostingEnvironment;
        }

        private string PythonExecuteWithParameters(params string[] Arguments)
        {
            Process process = new Process();
            try
            {
                Arguments[0] = Path.GetFullPath(
                    Path.ChangeExtension(
                        Path.Combine(
                            _hostingEnvironment.ContentRootPath,
                            Path.Combine("Python", Arguments[0])),
                        "py")
                    );

                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.FileName = Startup.Configuration["GDAL:PythonFullPath"];
                process.StartInfo.Arguments = string.Join(' ', Arguments);
                process.Start();
                string pyhonOutput = process.StandardOutput.ReadToEnd();
                string pyhonError = process.StandardError.ReadToEnd();
                process.WaitForExit();
                if (!string.IsNullOrEmpty(pyhonError))
                {
                    throw new Exception(pyhonError);
                }
                else
                {
                    return pyhonOutput;
                }
            }
            catch (Exception exception)
            {
                throw new Exception(exception.ToString(), exception.InnerException);
            }
        }

        private string PythonExecute(params string[] Arguments)
        {
            Process process = new Process();
            try
            {
                Arguments[0] = Path.GetFullPath(
                    Path.ChangeExtension(
                        Path.Combine(
                            _hostingEnvironment.ContentRootPath,
                            Path.Combine("Python", Arguments[0])),
                        "py")
                    );
                Arguments[0] = $"\"{Arguments[0]}\"";

                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardInput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.FileName = Startup.Configuration["GDAL:PythonFullPath"];
                process.StartInfo.Arguments = Arguments[0];
                process.Start();
                for (int i = 1; i < Arguments.Count(); i++)
                {
                    process.StandardInput.WriteLine(Arguments[i]);
                }
                string pyhonOutput = process.StandardOutput.ReadToEnd();
                string pyhonError = process.StandardError.ReadToEnd();
                process.WaitForExit();
                if (!string.IsNullOrEmpty(pyhonError))
                {
                    throw new Exception(pyhonError);
                }
                else
                {
                    return pyhonOutput;
                }
            }
            catch (Exception exception)
            {
                throw new Exception(exception.ToString(), exception.InnerException);
            }
        }

        private string GDALShellExecute(params string[] Arguments)
        {
            Process process = new Process();
            try
            {
                string startInfoFileName = Arguments[0];
                string[] arguments = new string[Arguments.Count() - 1];
                Array.Copy(Arguments, 1, arguments, 0, arguments.Count());

                process.StartInfo.UseShellExecute = false;
                process.StartInfo.RedirectStandardOutput = true;
                process.StartInfo.RedirectStandardError = true;
                process.StartInfo.FileName = startInfoFileName;
                process.StartInfo.Arguments = string.Join(' ', arguments);
                process.Start();
                string shellOutput = process.StandardOutput.ReadToEnd();
                string shellError = process.StandardError.ReadToEnd();
                process.WaitForExit();
                if (!string.IsNullOrEmpty(shellError))
                {
                    throw new Exception(shellError);
                }
                else
                {
                    return shellOutput;
                }
            }
            catch (Exception exception)
            {
                throw new Exception(exception.ToString(), exception.InnerException);
            }
        }

        public int[] GetShpColumnValues(string File, string Field)
        {
            int[] values = new int[1];
            try
            {
                string jsonArray = PythonExecute("GetShpValues", $"{File}", Field);
                values = Newtonsoft.Json.JsonConvert.DeserializeObject<int[]>(jsonArray);
                return values;
            }
            catch (Exception exception)
            {
                throw new Exception(exception.ToString(), exception.InnerException);
            }
        }

        public void DeleteFeatures(string File, string FieldName, string FieldValue)
        {
            try
            {
                PythonExecute("FilterAndDelete", File, FieldName, FieldValue);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.ToString(), exception.InnerException);
            }
        }

        public void MergeShapes(string Folder, string OutFileName)
        {
            try
            {
                PythonExecute("MergeShpLayers", Folder, OutFileName);
            }
            catch (Exception exception)
            {
                throw new Exception(exception.ToString(), exception.InnerException);
            }
        }

        public string GetGeometry(string File, string FieldName, string FieldValue)
        {
            string geometry = "";
            try
            {
                geometry = PythonExecute("GetGeometryRef", $"{File}", FieldName, FieldValue);
                return geometry;
            }
            catch (Exception exception)
            {
                throw new Exception(exception.ToString(), exception.InnerException);
            }
        }

        /// <summary>
        /// Возвращает пересечение объекта (геометрии) с объектами в shape-файле
        /// </summary>
        /// <param name="File">
        /// Shape-файл
        /// </param>
        /// <param name="Field">
        /// Поле в shape-файле, значения которого будут выданы (объектов, с которыми есть пересечение)
        /// </param>
        /// <param name="Geometry">
        /// Геометрия в формате WKT объекта, пересечения которого надо найти
        /// </param>
        /// <returns></returns>
        public string[] GetFeatureCrossFeatures(string File,
            string Field,
            string Geometry)
        {
            string[] values = new string[1];
            try
            {
                string jsonArray = PythonExecute("GetIdByLocation", $"{File}", Field, Geometry);
                values = Newtonsoft.Json.JsonConvert.DeserializeObject<string[]>(jsonArray);
                return values;
            }
            catch (Exception exception)
            {
                throw new Exception(exception.ToString(), exception.InnerException);
            }
        }

        public string GetFeatureValue(string File,
            string Field,
            string Value,
            string FieldToShow)
        {
            string value = "";
            try
            {
                value = PythonExecute("GetFeatureField", $"{File}", Field, Value, FieldToShow);
                return value;
            }
            catch (Exception exception)
            {
                throw new Exception(exception.ToString(), exception.InnerException);
            }
        }
    }
}