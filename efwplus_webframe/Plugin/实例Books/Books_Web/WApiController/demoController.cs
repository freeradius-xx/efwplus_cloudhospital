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
using Books_Web.Entity;


namespace Books_Web.WApiController
{

    public class demoController : WebApiController
    {
        // GET api/<controller>
        public List<Books> Get()
        {
            List<Books> list = NewObject<Books>().getlist<Books>();
            //List<book> list = new List<book>();
            //book book = new book();
            //book.name = "1111";
            //book.price = Convert.ToDecimal(11.11);
            //list.Add(book);
            //book = new book();
            //book.name = "222";
            //book.price = Convert.ToDecimal(22.11);
            //list.Add(book);
            return list;
        }

        // GET api/<controller>/5
        public List<Books> Get(int id)
        {
            //return wcfClientLink.Request("Books_Wcf@bookWcfController", "GetBooks", "[]");
            return ToListObj<Books>(InvokeWcfService("Books_Wcf", "bookWcfController", "GetBooks"));
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

        protected override string SetPluginName()
        {
            return "Books_Web";
        }
    }

    [Serializable]
    public class book
    {
        private string _name;
        public string name
        {
            get { return _name; }
            set { _name = value; }
        }

        public decimal price { get; set; }
    }
}
