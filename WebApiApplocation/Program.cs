using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using WebApi.Framework;

namespace WebApiApplication
{
    public static class WebApiConfig
    {
        public static void Register(RouteCollection config)
        {
            config.IgnoreUrl(".js");

            config.Add(new Route(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{action}/{id}",
                defaults: 
                new Dictionary<string, string>() { { "controller", "home" }, { "action", "index" }, { "id", "1" } }
            )
            );
        }
    }
    class Program
    {
        static void Global()
        {
            WebApiConfig.Register(RouteTable.Routes);
        }
        static void Main(string[] args)
        {
            //设置初始路由,即默认的路由模板
            Global();
            HttpTranferApplication application = new HttpTranferApplication();
            //此处是默认路由执行
            //{"Headers":{},"Body":{},"QueryParams":{},"Url":"api"}
            //此处是执行对应的方法
            //{"Headers":{},"Body":{},"QueryParams":{},"Url":"api/home/index"}
            //此处是执行post上传,检测模型绑定
            //{"Headers":{},"Body":{"Name":"Best_Hong","age":29},"QueryParams":{},"Url":"api/home/Post"}
            //此处是执行的是从路由中获取值
            //{"Headers":{},"Body":{},"QueryParams":{"id":29},"Url":"api/home/get"}
            //直接复制上面的字符串,在控制台运行测试即可,目前就不实现服务器端解析
            while (true)
            {
                String url = Console.ReadLine();
                try
                {
                    HttpTranferModel model = JsonConvert.DeserializeObject<HttpTranferModel>(url);
                    application.Execute(model);
                }catch(Exception e)
                {
                    Console.WriteLine("执行错误,请检查传递的参数");
                }
            }
        }
    }
}
