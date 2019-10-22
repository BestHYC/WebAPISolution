using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace WebApi.Framework
{
    public class HostApplication : IHostApplication<HttpBaseContext>
    {
        private IRoutingModule m_routingmodule;
        public HostApplication()
        {
            m_routingmodule = new UrlRoutingModule();
        }

        public HttpBaseContext CreateContext()
        {
            return new HttpContext();
        }
        /// <summary>
        /// 格式 head authorize:123456,form-data:json#$url:home/index?query=1#$body age:1,name="洪移潮"
        /// </summary>
        /// <param name="context"></param>
        public Task ProcessRequestAsync(HttpBaseContext context)
        {
            return new Task(() => m_routingmodule.Init(context));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        /// <param name="exception"></param>
        public void DisposeContext(HttpBaseContext context, Exception exception)
        {
            
        }
    }
    /// <summary>
    /// 此处为伪代码,模拟http传输解析使用
    /// </summary>
    public class HttpTranferApplication
    {
        private IHostApplication<HttpBaseContext> m_application;
        public HttpTranferApplication()
        {
            m_application = new HostApplication();
        }
        /// <summary>
        /// 这里实质是通过http去做解析,但是这里就做了一个简单的类型,实现后期功能
        /// </summary>
        /// <param name="model"></param>
        public void Execute(HttpTranferModel model)
        {
            HttpBaseContext context = m_application.CreateContext();
            context.Request.Headers = JsonConvert.SerializeObject(model.Headers);
            context.Request.RequestContent = JsonConvert.SerializeObject(model.Body);
            foreach(var item in model.QueryParams)
            {
                context.Request.QueryParams += item.Key + "=" + item.Value + "&";
            }
            context.Request.Uri = model.Url;
            Task task = m_application.ProcessRequestAsync(context);
            task.ContinueWith(t => Console.WriteLine("请求结束"));
            task.ContinueWith(t => Console.WriteLine("请求报错,错误为" + t.Exception), TaskContinuationOptions.OnlyOnFaulted);
            task.Start();
        }
    }
    public class HttpTranferModel
    {
        public HttpTranferModel()
        {
            Headers = new Dictionary<string, object>();
            Body = new Dictionary<string, object>();
            QueryParams = new Dictionary<string, object>();
        }
        public Dictionary<String, Object> Headers { get; set; }
        public Dictionary<String, Object> Body { get; set; }
        public Dictionary<String, Object> QueryParams { get; set; }
        public String Url { get; set; }
    }
}
