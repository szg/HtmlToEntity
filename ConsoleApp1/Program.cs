using AngleSharp.Html.Parser;
using System;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleApp1
{
    class Program
    {
        static void Main(string[] args)
        {
            TestHtmlToEntity();

            Console.ReadKey();
        }

        static void TestHtmlToEntity()
        {
            var html = HttpClient.Get("https://hz.lianjia.com/ershoufang/xihu/", null, out var success, 10);


            var entitys = html.ToList<FangJia>();
            if (entitys != null)
                foreach (var entity in entitys)
                    Console.WriteLine(entity);
        }
    }
}
