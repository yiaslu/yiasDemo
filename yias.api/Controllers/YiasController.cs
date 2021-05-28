using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;

namespace yias.mangeui.Controllers
{
    [Route("[controller]")]
    [EnableCors("*")]
    [ApiController]
    public class YiasController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        public ActionResult<IEnumerable<string>> Get()
        {
            return new string[] { "value1", "value2" };
        }

        [HttpPost]
        [Route("send")]
        public object SendService([FromBody]object value)
        {
            GearClass request = new GearClass(value + "");
            //返回调用方法获取的值
            return request.Send();
        }
    }
}