using EdgeJs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleDemo
{
    class Program
    {

        static void Main(string[] args)
        {
            try
            {
                if (args == null || args.Length == 0)
                    throw new Exception("Can't found javascript string parameter!");
                string decodedJsFunc = null;
                try
                {
                    decodedJsFunc = Uri.UnescapeDataString(args[0]);
                }
                catch (Exception exc)
                {
                    throw new Exception("Decode javascript string parameter error", exc);
                }
                string result = null;
                try
                {
                    result = RunJsInNode(decodedJsFunc).Result;
                }
                catch (Exception exc)
                {
                    throw new Exception("execute javascript error", exc);
                }
                OutputResult(result);
            }
            catch (Exception exc)
            {
                OutputError(exc);
            }
        }

        private static void OutputError(Exception exc)
        {
            Console.Write("error,{0}", exc.ToString());
        }
        private static void OutputResult(object result)
        {
            Console.Write("succeed,{0}", result.ToString());
        }

        public static async Task<string> RunJsInNode(string js)
        {
            var func = Edge.Func(@"
            return function (data, callback) {
                var endCallback = function(result){
                    callback(null,JSON.stringify(result));
                };
                (" + js + @")(endCallback);
            }
        ");
            return (await func(null)).ToString();
        }
    }
}
