using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace WebApi.Framework
{
    /// <summary>
    /// 用来对模型绑定表达式树做缓存
    /// 1.不建议使用表达式创建对象,因为无法耦合ioc,所以此处只做对象的赋值
    /// </summary>
    public class ModelExpressionCache
    {
        private static Dictionary<Type, ModelPropertyCache> S_Cache;
        static ModelExpressionCache()
        {
            S_Cache = new Dictionary<Type, ModelPropertyCache>();
        }
        private static Object S_lock = new Object();
        public static Boolean TryToSetValue<K>(in Object target, String name, K value)
        {
            Type type = target.GetType();
            if (!S_Cache.ContainsKey(type))
            {
                lock (S_lock)
                {
                    if (!S_Cache.ContainsKey(type))
                    {
                        ModelPropertyCache modelCache = new ModelPropertyCache(type);
                        S_Cache.Add(type, modelCache);
                    }
                }
            }
            ModelPropertyCache cache = S_Cache[type];
            if (cache.Contains(name))
            {
                if(cache.ExecuteExpression(in target, name, value))
                {
                    return true;
                }
            }
            return false;
        }
        /// <summary>
        /// 判断是否具有对象名
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static Boolean Contains(Type type, String name)
        {
            if (!S_Cache.ContainsKey(type)) return false;
            if (!S_Cache[type].Contains(name)) return false;
            return true;
        }
    }
    /// <summary>
    /// 对模型的属性赋值表达式树进行缓存
    /// </summary>
    public class ModelPropertyCache
    {
        //String 为当前的属性名,表达式树名称
        private Dictionary<String, Delegate> PropertyCache;
        private Type m_Type;
        public ModelPropertyCache(Type t)
        {
            PropertyCache = new Dictionary<String, Delegate>();
            m_Type = t;
            InitPropertyExpression(t);
        }
        public Boolean Contains(String name)
        {
            return PropertyCache.ContainsKey(name);
        }
        public Boolean ExecuteExpression<K>(in Object target, String name, K value)
        {
            if (target == null) return false;
            Type type = target.GetType();
            if (m_Type != type) throw new ArgumentException("请传递正确的执行对象");
            Type propertyType = type.GetProperty(name).PropertyType;
            object propertyValue = null;
            //如果可以强转,那么就强转,如果不可以,则跳过
            if (propertyType != typeof(K))
            {
                //对传递的Value值进行强行更改类型
                try
                {
                    propertyValue = Convert.ChangeType(value, propertyType);
                }
                catch (InvalidCastException e)
                {
                    return false;
                }
                catch (OverflowException ex)
                {
                    return false;
                }
            }
            if(propertyValue != null)
            {
                PropertyCache[name].DynamicInvoke(target, propertyValue);
            }
            else
            {
                PropertyCache[name].DynamicInvoke(target, value);
            }
            return true;
        }
        /// <summary>
        /// 构建Action<T, Object> action = (a,b) => a.id=b;的表达式树
        /// 注意此处只构建基本类型,如果遇到对象类型,那么不会执行递归操作,只会判断当前的类型是否一致
        /// </summary>
        /// <param name="t"></param>
        private void InitPropertyExpression(Type t)
        {
            PropertyInfo[] properties = t.GetProperties();
            ParameterExpression target = Expression.Parameter(t, "target");
            ParameterExpression value = Expression.Parameter(typeof(Object), "value");
            foreach (var prop in properties)
            {
                String name = prop.Name;
                Type propType = prop.PropertyType;
                UnaryExpression convert = Expression.Convert(value, propType);
                var setPropertyValue = Expression.Call(target, prop.GetSetMethod(), convert);
                ParameterExpression[] p = new ParameterExpression[] { target, value };
                Delegate fun = Expression.Lambda(setPropertyValue, target, value).Compile();
                PropertyCache.Add(name, fun);
            }
        }
    }
}
