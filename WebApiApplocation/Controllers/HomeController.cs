using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebApi.Framework;
using WebApi.Framework.Attributes;

namespace WebApiApplication
{
    [PreRouteAttribute("api/Home")]
    public class HomeController : ApiBaseController
    {
        public Int32 Index()
        {
            return 1;
        }
        public ActionResult Get(Int32 id)
        {
            HomeModel model = new HomeModel() { Age = id, Name = "Best_Hong" };
            ActionResult result = new DefaultActionResult()
            {
                Code = ResponseCode.Authorize,
                Message = "自定义的返回方式",
                Data = model
            };
            return result;
        }
        public HomeModel Post(HomeModel model)
        {
            return model;
        }
    }
    public class HomeModel
    {
        public Int32 Age { get; set; }
        public String Name { get; set; }
    }
}
