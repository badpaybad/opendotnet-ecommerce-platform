using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using DomainDrivenDesign.CorePermission.Reflections;

namespace Core.FrontEnd.Controllers
{
    public class OddsController : ApiController
    {
        [System.Web.Http.Route("api")]
        [RightDescription("api odds 1x2")]
        public string Odd1x2()
        {
            return string.Empty;
        }
    }
    public class ValuesController : ApiController
    {
        // GET api/values
        public IEnumerable<string> Get()
        {
            return new string[] { "value1", "value2" };
        }
        
        // GET api/values/5
        public string Get(int id)
        {
            return "value";
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }
    }
}
