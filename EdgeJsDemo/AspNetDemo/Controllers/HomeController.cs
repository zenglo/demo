using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace AspNetDemo.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";
            string result = CallNodeJs("function(end){end('abc');}");
            return View();
        }

        public static  string CallNodeJs(string js)
        {
            Process process = new Process();
            ProcessStartInfo startInfo = new ProcessStartInfo();
            startInfo.FileName = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"bin\ConsoleDemo.exe");
            startInfo.Arguments = js;
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardInput = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.CreateNoWindow = true;
            process.StartInfo = startInfo;
            process.Start();
            var strResult =  process.StandardOutput.ReadToEnd();
            process.WaitForExit();
            process.Close();
            return strResult;
        }
    }
}
