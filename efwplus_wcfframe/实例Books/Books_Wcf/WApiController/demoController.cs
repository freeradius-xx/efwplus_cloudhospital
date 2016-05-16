using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EFWCoreLib.WebFrame.WebAPI;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Data;
using Books_Wcf.Entity;
using EFWCoreLib.CoreFrame.Business.AttributeInfo;


namespace Books_Wcf.WApiController
{
    [efwplusApiController(PluginName = "Books_Wcf")]
    public class demoController : WebApiController
    {
        // GET api/<controller>
        public List<Books> Get()
        {
            List<Books> list = NewObject<Books>().getlist<Books>();
            return list;
        }

        // GET api/<controller>/5
        public Books Get(int id)
        {
            //throw new Exception("err");
            return NewObject<Books>().getmodel(id) as Books;
        }

        // GET api/<controller>/5
        public Books Get(int id,string name)
        {
            //throw new Exception("err");
            return NewObject<Books>().getmodel(id) as Books;
        }

        // POST api/<controller>
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        public void Delete(int id)
        {
        }
    }
 
}
