using System;
using System.Collections.Generic;
using System.Text;

namespace WebApi.Framework
{
    public class HttpRequest
    {
        /// <summary>
        /// 从哪里来的
        /// </summary>
        public String UrlReferrer { get; }
        /// <summary>
        /// 到哪里去
        /// </summary>
        public String Uri { get; set; }
        /// <summary>
        /// Uri请求参数处理
        /// </summary>
        public String QueryParams { get; set; }
        /// <summary>
        /// 请求的内容
        /// </summary>
        public String RequestContent { get; set; }
        /// <summary>
        /// 请求头参数
        /// </summary>
        public String Headers { get; set; }
    }
    public class HttpResponse
    {
        /// <summary>
        /// 返回的内容
        /// </summary>
        public String ResponseContent { get; set; }
    }
    public class HttpContext: HttpBaseContext
    {
        public HttpContext()
        {
            Request = new HttpRequest();
            Response = new HttpResponse();
            Context = this;
        }
        public override HttpRequest Request { get; }

        public override HttpResponse Response { get; }

        public override HttpBaseContext Context { get; }
    }

    public abstract class HttpBaseContext
    {
        public abstract HttpRequest Request { get; }
        public abstract HttpResponse Response { get; }
        public abstract HttpBaseContext Context { get; }
    }
}
