using System;
using System.Collections.Generic;
using System.Text;

namespace WebApi.Framework
{
    public class RequestContext
    {
        /// <summary>
        /// 中间节请求
        /// </summary>
        public HttpBaseContext Context { get; set; }
        /// <summary>
        /// 路由值请求
        /// </summary>
        public RouteData RouteData { get; set; }
    }
}
