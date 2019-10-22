using System;
using System.Collections.Generic;
using System.Text;

namespace WebApi.Framework
{
    /// <summary>
    /// 控制器的上下文
    /// </summary>
    public class ControllerContext
    {
        public RequestContext RequestContext { get; set; }
        public IApiController Controller { get; set; }
    }
}
