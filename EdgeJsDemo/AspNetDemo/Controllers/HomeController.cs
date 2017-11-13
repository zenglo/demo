using ConsoleDemo;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;

namespace AspNetDemo.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";
            string result = JsCaller.CallFuncInExe(@"
                function(para,end){
                    para.a+=1;
                    para.b+='你好!';
                    end({a:1,b:'abc'});
                }
            ", "{a:1,b:'zenglong'}");
            return View();
        }
    }
}
