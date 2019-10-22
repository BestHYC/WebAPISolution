using System;
using System.Collections.Generic;
using System.Text;

namespace WebApi.Framework
{
    /// <summary>
    /// 收集路由解析对象的值
    /// </summary>
    public class RouteTable
    {
        public static RouteCollection Routes;
        static RouteTable()
        {
            Routes = new RouteCollection();
        }
    }
}
