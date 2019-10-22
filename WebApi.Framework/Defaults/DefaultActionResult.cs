using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace WebApi.Framework
{
    public class DefaultActionResult : ActionResult
    {
        public override void ExecuteResult(ControllerContext context)
        {
            HttpBaseContext baseContext = context.RequestContext.Context;
            if(this.Code != default(ResponseCode))
            {
                this.Code = ResponseCode.Success;
            }
            if (String.IsNullOrEmpty(this.Message))
            {
                this.Message = ResponseCodeString.Message(this.Code);
            }
            baseContext.Response.ResponseContent = JsonConvert.SerializeObject(this);
        }
    }
}
