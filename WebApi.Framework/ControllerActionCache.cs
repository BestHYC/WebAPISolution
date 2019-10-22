using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using WebApi.Framework.Attributes;

namespace WebApi.Framework
{
    /// <summary>
    /// 全局所有IApiController类型的操作都是由此处进行缓存
    /// 其他地方只做类型处理,比如 A/B,那么是对应的是AController 还是A,都是其他地方做处理
    /// 注意此处,只当做类型及方法的缓存,不做任何对执行返回结果及传递对象的处理,保持功能单一
    /// 保持路径单一,即A/B中A控制器只有1个,B方法也只有1个,即使有重载,也必须得通过 路由名 进行区分
    /// </summary>
    internal static class ApiControllerActionCache
    {
        private static Dictionary<String, Type> s_controllerTypes = new Dictionary<string, Type>();
        private static Dictionary<Type, ActionCache> s_actionCache = new Dictionary<Type, ActionCache>();
        static ApiControllerActionCache()
        {
            Assembly assembly = Assembly.GetEntryAssembly();
            foreach (Type type in assembly.GetTypes().Where(type => typeof(IApiController).IsAssignableFrom(type)))
            {
                String name = type.Name.ToLower();
                if(type.GetCustomAttribute<PreRouteAttribute>() != null)
                {
                    name = type.GetCustomAttribute<PreRouteAttribute>().PreRouteName.ToLower();
                }
                if (s_controllerTypes.ContainsKey(name)) throw new Exception($"{name}存在相同的路由名,请保持路由唯一");
                s_controllerTypes.Add(name, type);
                s_actionCache.Add(type, new ActionCache(type));
            }
        }
        public static Type GetController(String controllername)
        {
            if (!s_controllerTypes.ContainsKey(controllername)) return null;
            return s_controllerTypes[controllername];
        }
        /// <summary>
        /// 通过路由值获取对应的委托方法
        /// </summary>
        /// <param name="controllername"></param>
        /// <param name="actionname"></param>
        /// <returns></returns>
        public static Func<IApiController, Object[], Object> GetMethodFunc(Type controller, String actionname)
        {
            if(!s_actionCache.ContainsKey(controller)) throw new Exception("没有此路由对应的类型");
            ActionCache cache = s_actionCache[controller];
            if (!cache.ContainsKey(actionname)) throw new Exception("没有此路由对应的方法");
            return cache.GetMethodFunc(actionname);
        }
        public static Func<IApiController, Object[], Object> GetMethodFunc(MethodInfo methodInfo)
        {
            Type reference = methodInfo.ReflectedType;
            ActionCache cache = s_actionCache[reference];
            if(cache == null) throw new Exception("没有此路由对应的方法");
            return cache.GetMethodFunc(methodInfo);
        }
        public static MethodInfo GetMethodInfo(Type controller, String actionname)
        {
            if (!s_actionCache.ContainsKey(controller)) throw new Exception("没有此路由对应的类型");
            ActionCache cache = s_actionCache[controller];
            if (!cache.ContainsKey(actionname)) throw new Exception("没有此路由对应的方法");
            return cache[actionname];
        }
    }
    public class ActionCache
    {
        private Dictionary<String, MethodInfo> m_methodinfo;
        private Dictionary<MethodInfo, Func<IApiController, Object[], Object>> m_FuncCache ;
        public MethodInfo this[String name]
        {
            get
            {
                return m_methodinfo[name];
            }
        }

        public Boolean ContainsKey(String name)
        {
            return m_methodinfo.ContainsKey(name);
        }
        private Object m_lock = new Object();
        /// <summary>
        /// 可以考虑延迟加载
        /// </summary>
        /// <param name="type"></param>
        public ActionCache(Type type)
        {
            m_methodinfo = new Dictionary<String, MethodInfo>();
            m_FuncCache = new Dictionary<MethodInfo, Func<IApiController, object[], object>>();
            foreach(MethodInfo info in type.GetMethods())
            {
                String name = info.Name.ToLower();
                if(info.GetCustomAttribute<RouteAttribute>() != null)
                {
                    name = info.GetCustomAttribute<RouteAttribute>().RouteName.ToLower();
                }
                if (m_methodinfo.ContainsKey(name)) throw new Exception($"{type.Name}中{name}重复,请保持路径唯一");
                m_methodinfo.Add(name, info);
            }
        }
        public Func<IApiController, Object[], Object> GetMethodFunc(MethodInfo methodInfo)
        {
            if (methodInfo == null) return null;
            if (!m_FuncCache.ContainsKey(methodInfo))
            {
                lock (m_lock)
                {
                    if (!m_FuncCache.ContainsKey(methodInfo))
                    {
                        m_FuncCache.Add(methodInfo, CreateExecutor(methodInfo));
                    }
                }
            }
            return m_FuncCache[methodInfo];
        }
        /// <summary>
        /// 通过名称获取对应的委托
        /// </summary>
        /// <param name="methodInfo"></param>
        /// <returns>IApiController:传递的执行方法, Object[]:方法参数, Object 返回值,void为空</returns>
        public Func<IApiController, Object[], Object> GetMethodFunc(String methodName)
        {
            if (!m_methodinfo.ContainsKey(methodName)) return null;
            MethodInfo methodInfo = m_methodinfo[methodName];
            return GetMethodFunc(methodInfo);
        }
        private Func<Object, Object[], Object> CreateExecutor(MethodInfo methodInfo)
        {
            ParameterExpression target = Expression.Parameter(typeof(Object), "target");
            ParameterExpression arguments = Expression.Parameter(typeof(Object[]), "arguments");
            List<Expression> parameters = new List<Expression>();
            ParameterInfo[] paramInfos = methodInfo.GetParameters();
            for (Int32 i = 0; i < paramInfos.Length; i++)
            {
                ParameterInfo paramInfo = paramInfos[i];
                BinaryExpression getElementByIndex = Expression.ArrayIndex(arguments, Expression.Constant(i));
                UnaryExpression converToParamterType = Expression.Convert(getElementByIndex, paramInfo.ParameterType);
                parameters.Add(converToParamterType);
            }
            UnaryExpression instanceCast = Expression.Convert(target, methodInfo.ReflectedType);
            MethodCallExpression methodCall = Expression.Call(instanceCast, methodInfo, parameters);
            UnaryExpression converToObjectType = Expression.Convert(methodCall, typeof(Object));
            return Expression.Lambda<Func<Object, Object[], Object>>(converToObjectType, target, arguments).Compile();
        }
    }
}
