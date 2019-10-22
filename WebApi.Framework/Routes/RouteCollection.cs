using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace WebApi.Framework
{
    public class RouteCollection : ICollection<RouteBase>
    {
        /// <summary>
        /// 需要忽略的路径
        /// </summary>
        private String m_ignore;
        private List<RouteBase> m_route = new List<RouteBase>();
        public int Count => m_route.Count;

        public bool IsReadOnly => true;

        public void Add(RouteBase item)
        {
            m_route.Add(item);
        }
        public void IgnoreUrl(String url)
        {
            m_ignore = url;
        }
        public void Clear()
        {
            m_ignore = null;
            m_route.Clear();
        }

        public bool Contains(RouteBase item)
        {
            return m_route.Contains(item);
        }

        public void CopyTo(RouteBase[] array, int arrayIndex)
        {
            m_route.CopyTo(array, arrayIndex);
        }

        public IEnumerator<RouteBase> GetEnumerator()
        {
            return m_route.GetEnumerator();
        }

        public bool Remove(RouteBase item)
        {
            return m_route.Remove(item);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        public RouteData GetRouteData(HttpBaseContext httpContext)
        {
            if (!String.IsNullOrWhiteSpace(m_ignore) && Regex.IsMatch(httpContext.Request.Uri.ToString(), m_ignore))
            {
                return null;
            }
            foreach (var route in this)
            {
                RouteData routeData = route.GetRouteData(httpContext);
                if (routeData != null)
                {
                    return routeData;
                }
            }
            return null;
        }
    }
}
