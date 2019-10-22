using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace WebApi.Framework
{
    /// <summary>
    /// 默认对执行方法进行解析,针对绑定模型参数进行处理
    /// </summary>
    public class DefaultActionInvoker : IActionInvoker
    {
        private IModelBinder m_modelBinder;
        private ActionResult m_actionResult;
        public DefaultActionInvoker()
        {
            m_modelBinder = new DefaultModelBinder();
        }
        public void InvokeAction(ControllerContext context, String actionName)
        {
            Type type = context.Controller.GetType();
            MethodInfo methodinfo = ApiControllerActionCache.GetMethodInfo(type, actionName);
            if (methodinfo == null) throw new Exception("找不到对应webapi的路径");
            Func<IApiController, Object[], Object> func = ApiControllerActionCache.GetMethodFunc(methodinfo);
            ParameterInfo[] parameters = methodinfo.GetParameters();
            List<Object> paramsValue = new List<Object>();
            foreach(var param in parameters)
            {
                paramsValue.Add(m_modelBinder.BindModel(context, param.Name, param.ParameterType));
            }
            Object result = func.Invoke(context.Controller, paramsValue.ToArray());
            if (typeof(ActionResult).IsAssignableFrom(result.GetType()))
            {
                m_actionResult = (ActionResult)result;
            }
            else
            {
                m_actionResult = new DefaultActionResult();
                m_actionResult.Data = result;
            }
            m_actionResult.ExecuteResult(context);
        }
    }
}
