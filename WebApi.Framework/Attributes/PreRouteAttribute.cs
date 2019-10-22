using System;
using System.Collections.Generic;
using System.Text;

namespace WebApi.Framework.Attributes
{
    /// <summary>
    /// 路径前提,标识不同命名空间下相同controller名称的路由
    /// </summary>
    public class PreRouteAttribute:Attribute
    {
        public String PreRouteName { get;}
        public PreRouteAttribute(String name)
        {
            PreRouteName = name;
        }
    }
}
