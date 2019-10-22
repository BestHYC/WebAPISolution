using System;
using System.Collections.Generic;
using System.Text;

namespace WebApi.Framework
{
    public interface IApiController
    {
        void Execute(RequestContext requestContext);
    }
}
