using System;
using System.Collections.Generic;
using System.Text;

namespace WebApi.Framework
{
    public class DefaultApiHandler : IApiHandler
    {
        private RequestContext m_requestContext;
        private IControllerFactory m_controllerFactory;
        public DefaultApiHandler(RequestContext request)
        {
            m_requestContext = request;
            m_controllerFactory = new DefaultControllerFactory();
        }
        public void ProcessRequest()
        {
            String controllername = m_requestContext.RouteData.Controller;
            IApiController controller = m_controllerFactory.CreateController(controllername);
            if (controller == null) return;
            controller.Execute(m_requestContext);
        }
    }
}
