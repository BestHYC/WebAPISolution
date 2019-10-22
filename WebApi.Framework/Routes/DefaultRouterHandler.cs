using System;
using System.Collections.Generic;
using System.Text;

namespace WebApi.Framework
{
    public class DefaultRouterHandler : IRouteHandler
    {
        public IApiHandler GetApiHandler(RequestContext requestContext)
        {
            return new DefaultApiHandler(requestContext);
        }
    }
}
