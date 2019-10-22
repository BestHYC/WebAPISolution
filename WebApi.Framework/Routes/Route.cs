using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Web;

namespace WebApi.Framework
{
    /// <summary>
    /// 路由对外提供统一的解析规则,并返回解析后的RouteData
    /// </summary>
    public class Route : RouteBase
    {
        private IRouteHandler m_routeHandler;
        /// <summary>
        /// 对模板进行解析
        /// </summary>
        private String[] m_templateurl;
        public Route(String name, String routeTemplate, Dictionary<String, String> defaults = null)
        {
            m_routeHandler = new DefaultRouterHandler();
            Name = name;
            Template = routeTemplate;
            if (!String.IsNullOrEmpty(routeTemplate))
            {
                m_templateurl = Template.Split('/');
            }
            Defaults = defaults;
        }
        /// <summary>
        /// 当前的路由名
        /// </summary>
        public String Name { get; }
        /// <summary>
        /// 模板配置
        /// </summary>
        public String Template { get;}
        /// <summary>
        /// 初始化参数设置
        /// </summary>
        public Dictionary<String, String> Defaults { get; }
        public override RouteData GetRouteData(HttpBaseContext httpContext)
        {
            Dictionary<String, String> values = null;
            if(IsMatch(httpContext.Request.Uri, out values))
            {
                //目前只对json数据进行解析,其他的包括form表单之类的,以此类推即可
                RouteData routeData = new RouteData();
                routeData.RouteHandler = m_routeHandler;
                GetControllerAction(values, routeData.Values);
                GetHeaderParams(httpContext.Request.Headers, routeData.Headers);
                GetQueryParams(httpContext.Request.QueryParams, routeData.QueryString);
                GetBodyParams(httpContext.Request.RequestContent, routeData.Params);
                return routeData;
            }
            return null;
        }
        protected void GetControllerAction(Dictionary<String, String> param, NameValueCollection collection)
        {
            if (collection == null)
            {
                throw new ArgumentNullException(nameof(collection));
            }

            foreach (var item in param)
            {
                collection.Add(item.Key, item.Value);
            }
        }
        /// <summary>
        /// 这里的解析只是做基本的解析方式,避免重写HttpContext及RequestContext
        /// </summary>
        /// <param name="headers"></param>
        /// <param name="collection"></param>
        protected void GetHeaderParams(String headers,in Dictionary<String, Object> collection)
        {
            if (String.IsNullOrEmpty(headers)) return;
            var values = JsonConvert.DeserializeObject<Dictionary<String, Object>>(headers);
            foreach(var val in values)
            {
                collection.Add(val.Key, val.Value);
            }
        }
        protected void GetQueryParams(String query, in NameValueCollection collection)
        {
            if (String.IsNullOrEmpty(query)) return;
            String[] values = query.Split('&');
            foreach(String value in values)
            {
                if (String.IsNullOrEmpty(value)) continue;
                String[] keyValue = value.Split('=');
                if (keyValue.Length < 2)
                {
                    collection.Add(HttpUtility.UrlEncode(keyValue[0]), "");
                }
                else
                {
                    collection.Add(HttpUtility.UrlEncode(keyValue[0]), HttpUtility.UrlEncode(keyValue[1]));
                }
                
            }
        }
        protected void GetBodyParams(String body,in Dictionary<String, Object> collection, String contentType= "application/json")
        {
            if (String.IsNullOrEmpty(body)) return;
            switch (contentType)
            {
                case "application/json":
                    var values = JsonConvert.DeserializeObject<Dictionary<String, Object>>(body);
                    foreach(var val in values)
                    {
                        collection.Add(val.Key, val.Value);
                    }
                    break;
                default:
                    throw new Exception("传递正确格式");
            }
        }

        /// <summary>
        /// 判断不同的路由比对是否正确
        /// "{controller}/{action}/{id}" 与 Home/Index/""之间的正确与否
        /// </summary>
        /// <param name="requestUrl"></param>
        /// <param name="values"></param>
        /// <returns></returns>
        protected Boolean IsMatch(String requestUrl, out Dictionary<String, String> values)
        {
            values = new Dictionary<String, String>();
            String[] strArray1 = requestUrl.Split('/');
            //1.如果与模板的长度不一致
            //2.如果默认的长度与模板的长度不一致 如http://test => 对应的是 http://test/home/index
            //3.如果默认的与传递的长度不一致 如 http://test/api/home 对应的是 http://test/api/home/index
            if (strArray1.Length != m_templateurl.Length
                && Defaults.Count != m_templateurl.Length
                && (Defaults.Count + strArray1.Length < m_templateurl.Length)
                ) return false;

            //匹配路由,如 api 路径 与 模板 api/{controller}/{action}/{index}中进行比对
            //api/home 限定为一个字符,api/{controller}/test/{action}中 test/index限定为一个字符,注意全小写,url不分大小写
            String path = "";
            for (Int32 i = 0; i < m_templateurl.Length; i++)
            {
                //如果{controller}可以进行匹配,如果找不到那么从default中获取,如果获取不到,那么直接return false
                //如果模板中无大括号的,即为完全匹配 比如 传过来的数据为 home/index,没有api,那么直接return false
                String routesplit = m_templateurl[i].Trim();
                if (routesplit.StartsWith("{") && routesplit.EndsWith("}"))
                {
                    String name = m_templateurl[i].Trim("{}".ToCharArray()).Trim().ToLower();
                    String value = path;
                    path = "";
                    if (i >= strArray1.Length)
                    {
                        if (!Defaults.ContainsKey(name)) throw new Exception("没有对应的默认路由");
                        value += Defaults[name].ToLower();
                    }
                    else
                    {
                        value += strArray1[i].ToLower();
                    }
                    values.Add(name, value);
                }else if(String.Equals(routesplit, strArray1[i]))
                {
                    path += routesplit.ToLower() + "/";
                }
                else 
                {
                    //必须固定的形式{controller}/{action}/{index},否则直接报错
                    return false;
                }
            }
            return true;
        }
    }
}
