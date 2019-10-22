using System;
using System.Collections.Generic;
using System.Text;

namespace WebApi.Framework
{
    public abstract class RouteBase
    {
        public abstract RouteData GetRouteData(HttpBaseContext httpContext);
    }
}
