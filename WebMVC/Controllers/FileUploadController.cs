using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebMVC.Controllers
{
    [RoutePrefix("api/FileUpload")]
    public class FileUploadController : ApiController
    {

        [HttpGet]
        [Route("GetFileResult", Name = "GetFileResult")]
        public IHttpActionResult GetFileResult()
        {
            return Ok();
        }

    }
}
