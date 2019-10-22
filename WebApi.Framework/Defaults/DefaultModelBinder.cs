using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace WebApi.Framework
{
    public class DefaultModelBinder : IModelBinder
    {
        private Object m_lock = new Object();
        public Object BindModel(ControllerContext controllerContext, String modelName, Type modelType)
        {
            //值类型全匹配,即 A与a都匹配成功
            //对象类型只做首字母大小写匹配 及 AModel与aModel为一个,但是与Amodel不为同一个
            if (modelType == null) throw new Exception("绑定模型操作失败");
            Object instance;
            //如果是值类型的,那么直接绑定
            if (modelType.IsValueType || typeof(String) == modelType)
            {
                if (GetValueTypeInstance(controllerContext, modelName, modelType, out instance))
                {
                    return instance;
                }
            }
            //如果是类型对象,那么执行全匹配,只需要其内部某个字段有值,那么便做赋值操作
            //如果 class A{ B, C} 传递的值{B:1, D:1} =>就可以将A类做匹配操作,丢掉D属性赋值
            //但是如果传递的值为 {D:1},没有匹配的对象,那么便不做赋值处理
            else if (GetObjectInstance(controllerContext, modelName, modelType, out instance)) return instance;
            return null;
        }
        /// <summary>
        /// 只从body中获取值,如果Body为null,那么从url中获取对应的值
        /// </summary>
        /// <param name="controllerContext"></param>
        /// <param name="modelName"></param>
        /// <param name="modelType"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private Boolean GetObjectInstance(ControllerContext controllerContext, String modelName, Type modelType, out Object value)
        {
            //从http中获取到的资源汇总
            Dictionary<String, Object> dataSource = new Dictionary<String, Object>();
            RouteData data = controllerContext.RequestContext.RouteData;
            //只从body中获取
            foreach (var param in data.Params)
            {
                String key = param.Key;
                if (dataSource.ContainsKey(key))
                {
                    continue;
                }
                dataSource.Add(key.ToLower(), data.Params[key]);
            }
            //这里可以使用ioc创建Type,现在不做绑定
            value = BindValue(modelType, dataSource);
            if (value == null) return false;
            return true;
        }
        /// <summary>
        /// 注意此处对于对象类型处理,首先递归,创建对象,然后执行赋值
        /// </summary>
        /// <param name="type"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private Object BindValue(Type type, Dictionary<String, Object> value)
        {
            Dictionary<String, Object> exist = new Dictionary<String, Object>();
            PropertyInfo[] properties = type.GetProperties();
            Object model = null;
            foreach (var item in properties)
            {
                String propertyName = item.Name;
                String firstLatterLowerCase = propertyName[0].ToString().ToLower() + propertyName.Substring(1);
                Type propertyType = item.PropertyType;
                Object obj = null;
                //这里做首字母大小写的适配
                if (value.ContainsKey(propertyName))
                {
                    obj = value[propertyName];
                }
                else if (value.ContainsKey(firstLatterLowerCase))
                {
                    obj = value[firstLatterLowerCase];
                }
                else
                {
                    continue;
                }
                if (obj == null) continue;
                if (!type.IsValueType && !(type == typeof(String)))
                {
                    Dictionary<String, Object> child = GetValuePairs(obj);
                    if (child != null)
                    {
                        obj = BindValue(propertyType, child);
                    }
                }
                exist.Add(propertyName, obj);
            }
            if(exist.Count != 0)
            {
                model = Activator.CreateInstance(type);
                foreach(var item in exist)
                {
                    ModelExpressionCache.TryToSetValue(in model, item.Key, item.Value);
                }
            }
            return model;
        }
        private Dictionary<String, Object> GetValuePairs(Object source)
        {
            Type type = source.GetType();
            if (type.IsValueType || type == typeof(String)) return null;
            if (type == typeof(Dictionary<String, Object>)) return (Dictionary<String, Object>)source;
            String str = JsonConvert.SerializeObject(source);
            return JsonConvert.DeserializeObject<Dictionary<String, Object>>(str);
        }
        /// <summary>
        /// 执行绑定值类型及String类型的操作
        /// </summary>
        /// <param name="controllerContext"></param>
        /// <param name="modelName"></param>
        /// <param name="modelType"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        private bool GetValueTypeInstance(ControllerContext controllerContext, String modelName, Type modelType, out Object value)
        {
            Dictionary<String, Object> dataSource = new Dictionary<String, Object>();
            RouteData data = controllerContext.RequestContext.RouteData;
            foreach (String key in data.QueryString)
            {
                if (dataSource.ContainsKey(key.ToLower()))
                {
                    continue;
                }
                dataSource.Add(key.ToLower(), data.QueryString[key]);
            }
            foreach (var param in data.Params)
            {
                String key = param.Key;
                if (dataSource.ContainsKey(key))
                {
                    continue;
                }
                dataSource.Add(key.ToLower(), data.Params[key]);
            }
            if (dataSource.TryGetValue(modelName.ToLower(), out value))
            {
                value = Convert.ChangeType(value, modelType);
                return true;
            }
            return false;
        }
    }
}
