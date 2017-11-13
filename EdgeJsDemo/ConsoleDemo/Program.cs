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
            if (args == null || args.Length == 0)
            {
                args = new string[]
                {
                    @"
                function(para,end){
                    para.a+=1;
                    para.b+='你好!';
                    end(para);
                }
            ",
                    "{a:1,b:'zenglong'}"
                };
                args = args.Select(m => Uri.EscapeDataString(m)).ToArray();
            }
            try
            {
                string result = JsCaller.CallJsFunWithCmdArgs(args);
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
    }
}
