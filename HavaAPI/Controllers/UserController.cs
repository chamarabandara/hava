using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace HavaAPI.Controllers
{
    public class UserController : Controller
    {
        //
        // GET: /User/
        public ActionResult Index()
        {
            return View();
        }

        public JObject Get() 
        {
           JObject obj =  new JObject();
            obj.Add("status",true);
            return obj;
        } 
	}
}