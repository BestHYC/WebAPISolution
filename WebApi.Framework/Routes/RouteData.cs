using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;

namespace WebApi.Framework
{
    /// <summary>
    /// 放置路由值
    /// </summary>
    public class RouteData
    {
        /// <summary>
        /// 路由值实现
        /// </summary>
        public NameValueCollection Values { get; }
        /// <summary>
        /// 查询Body中的参数
        /// </summary>
        public Dictionary<String, Object> Params { get; }
        /// <summary>
        /// Url中的拼接参数
        /// </summary>
        public NameValueCollection QueryString { get; }
        /// <summary>
        /// Headers中的参数
        /// </summary>
        public Dictionary<String, Object> Headers { get; }
        /// <summary>
        /// 路由句柄
        /// </summary>
        public IRouteHandler RouteHandler { get; set; }
        public RouteData()
        {
            Values = new NameValueCollection();
            Params = new Dictionary<String, Object>();
            QueryString = new NameValueCollection();
            Headers = new Dictionary<String, Object>();
        }
        public String Controller
        {
            get
            {
                return this.Values.Get("controller");
            }
        }
        public String ActionName
        {
            get
            {
                return this.Values.Get("action");
            }
        }
    }
}
