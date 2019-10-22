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
            return Task.Run(() => m_routingmodule.Init(context));
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
    public class HttpTranferApplication
    {
        private IHostApplication<HttpBaseContext> m_application;
        public HttpTranferApplication()
        {
            m_application = new HostApplication();
        }
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
            m_application.ProcessRequestAsync(context);
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
