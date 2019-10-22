using System;
using System.Collections.Generic;
using System.Text;

namespace WebApi.Framework
{
    public interface IModelBinder
    {
        Object BindModel(ControllerContext controllerContext, String modelName, Type modelType);
    }
}
