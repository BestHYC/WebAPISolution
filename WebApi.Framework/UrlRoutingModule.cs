using System;
using System.Collections.Generic;
using System.Text;

namespace WebApi.Framework
{
    public interface IRoutingModule
    {
        void Init(HttpBaseContext context);
    }
    public class UrlRoutingModule : IRoutingModule
    {
        public void Init(HttpBaseContext context)
        {
            RouteData routeData = RouteTable.Routes.GetRouteData(context);
            if (routeData == null) return;
            RequestContext request = new RequestContext()
            {
                Context = context,
                RouteData = routeData
            };
            IApiHandler handler = routeData.RouteHandler.GetApiHandler(request);
            handler.ProcessRequest();
            Console.WriteLine(request.Context.Response.ResponseContent);
        }
    }
}
