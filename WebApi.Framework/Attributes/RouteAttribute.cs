using System;
using System.Collections.Generic;
using System.Text;

namespace WebApi.Framework.Attributes
{
    /// <summary>
    /// 路径的路由名,此处为单独路由
    /// </summary>
    public class RouteAttribute : Attribute
    {
        public String RouteName { get; }
        public RouteAttribute(String name)
        {
            RouteName = name;
        }
    }
}
