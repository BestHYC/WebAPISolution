using System;
using System.Collections.Generic;
using System.Text;

namespace WebApi.Framework
{
    public interface IRouteHandler
    {
        IApiHandler GetApiHandler(RequestContext requestContext);
    }
}
