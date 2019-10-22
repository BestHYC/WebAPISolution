using System;
using System.Collections.Generic;
using System.Text;

namespace WebApi.Framework
{
    public interface IActionInvoker
    {
        void InvokeAction(ControllerContext context, String actionName);
    }
}
