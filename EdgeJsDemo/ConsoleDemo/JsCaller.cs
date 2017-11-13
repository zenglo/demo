using EdgeJs;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDemo
{
    public static class JsCaller
    {
        public static string CallFuncInExe(string jsFunc, string jsonPara = null)
        {
            if (string.IsNullOrWhiteSpace(jsFunc))
            {
                throw new ArgumentException("jsFunc can't be null or empty", nameof(jsFunc));
            }
            using (Process process = new Process())
            {
                ProcessStartInfo startInfo = new ProcessStartInfo();
                string assLocation = Assembly.GetExecutingAssembly().Location;//在web中，可能不准
                if (assLocation.StartsWith(AppDomain.CurrentDomain.BaseDirectory))
                {
                    startInfo.FileName = assLocation;
                }
                else
                {
                    startInfo.FileName =
                        System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "bin",
                        System.IO.Path.GetFileName(assLocation));
                }
                startInfo.Arguments = string.Format("{0} {1}",
                    Uri.EscapeDataString(jsFunc),
                    Uri.EscapeDataString(jsonPara ?? string.Empty));
                startInfo.UseShellExecute = false;
                startInfo.RedirectStandardInput = false;
                startInfo.RedirectStandardOutput = true;
                startInfo.CreateNoWindow = true;
                process.StartInfo = startInfo;
                process.Start();
                var strResult = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                process.Close();
                return strResult;
            }
        }

        public static string CallJsFunWithCmdArgs(string[] args)
        {
            if (args == null || args.Length == 0)
                throw new Exception("Can't found javascript string parameter!");
            string encodedJsFunc = args[0];
            string encodedJsonPara = args.Length > 1 ? args[1] : string.Empty;
            string decodedJsFunc = null;
            string decodedJsonPara = null;
            try
            {
                decodedJsFunc = Uri.UnescapeDataString(encodedJsFunc);
            }
            catch (Exception exc)
            {
                throw new Exception("Decode javascript string parameter error", exc);
            }
            try
            {
                decodedJsonPara = Uri.UnescapeDataString(encodedJsonPara);
            }
            catch (Exception exc)
            {
                throw new Exception("Decode json paremeter error", exc);
            }
            string result = null;
            try
            {
                result = CallFunc(decodedJsFunc, decodedJsonPara).Result;
            }
            catch (Exception exc)
            {
                throw new Exception("execute javascript error", exc);
            }

            return result;
        }

        public static async Task<string> CallFunc(string jsFunc, string jsonPara = null)
        {
            var func = NewFunc(jsFunc);
            return await func(jsonPara);
        }

        public static Func<string, Task<string>> NewFunc(string jsFunc)
        {
            var func = Edge.Func(@"
            return function (jsonPara, callback) {
                var endCallback = function(resultObj){
                    var resultJson = '';
                    if(resultObj==null)
                        resultJson = null;
                    else
                        resultJson = JSON.stringify(resultObj);
                    callback(null,resultJson);
                };
                var para = null;
                if(jsonPara==null)
                    para = null;
                else
                    para = eval('('+jsonPara+')');
                (" + jsFunc + @")(para,endCallback);
            }
        ");
            return async jsonPara =>
            {
                var result = await func(string.IsNullOrWhiteSpace(jsonPara) ? null : jsonPara.Trim());
                if (result == null)
                    return null;
                else
                    return result.ToString();
            };
        }
    }
}
