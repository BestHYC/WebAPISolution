using System;
using System.Collections.Generic;
using System.Text;

namespace WebApi.Framework
{
    public abstract class ActionResult
    {
        public String Message { get; set; }
        public ResponseCode Code { get; set; }
        public Object Data { get; set; }
        public abstract void ExecuteResult(ControllerContext context);
    }
    public enum ResponseCode
    {
        Success=200, Error =500, Authorize= 400
    }
    public static class ResponseCodeString
    {
        public static String Message(ResponseCode code)
        {
            switch (code)
            {
                case ResponseCode.Success:return "Success";
                case ResponseCode.Error:return "Error";
                case ResponseCode.Authorize:return "Authorize error";
                default:
                    return "Success";
            }
        }
    }
}
