using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace WebApi.Framework
{
    public interface IControllerFactory
    {
        IApiController CreateController(String name);
    }
    public class DefaultControllerFactory : IControllerFactory
    {
        /// <summary>
        /// 这里对类型的名称进行解析,
        /// </summary>
        /// <param name="controllerName"></param>
        /// <returns></returns>
        public IApiController CreateController(String controllerName)
        {
            Type controller = ApiControllerActionCache.GetController(controllerName);
            if(controller == null)
            {
                controller = ApiControllerActionCache.GetController(controllerName + "controller");
            }
            if (controller == null) return null;

            return (IApiController)Activator.CreateInstance(controller);
        }
    }
}
