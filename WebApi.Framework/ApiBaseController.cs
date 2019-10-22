using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace WebApi.Framework
{
    /// <summary>
    /// 控制器的基类,拆分出来,可以实现控制器层级的异步操作
    /// </summary>
    public abstract class ApiBaseController : IApiController, IAsyncResult, IAuthorization
    {
        protected IActionInvoker ActionInvoker { get; }
        public ApiBaseController()
        {
            ActionInvoker = new DefaultActionInvoker();
        }
        public void Execute(RequestContext requestContext)
        {
            ControllerContext context = new ControllerContext()
            {
                Controller = this,
                RequestContext = requestContext
            };
            //默认对方法执行一次解析,但是遵从默认的解析方式
            String actionName = requestContext.RouteData.ActionName;
            ActionInvoker.InvokeAction(context, actionName);
        }
        public object AsyncState => throw new NotImplementedException();

        public WaitHandle AsyncWaitHandle => throw new NotImplementedException();

        public bool CompletedSynchronously => throw new NotImplementedException();

        public bool IsCompleted => throw new NotImplementedException();
    }
}
