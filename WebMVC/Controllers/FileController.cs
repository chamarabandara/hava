using HavaBusinessObjects.Utilities;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;

namespace WebMVC.Controllers
{
    [RoutePrefix("api/File")]
    public class FileController : ApiController
    {
        #region Product logo image uploads
        /// <summary>
        /// upload product image
        /// 
        /// Date                            Author/(Reviewer)                       Description
        /// ------------------------------------------------------------------------------------          
        /// 21-April-2017                      Chamara Bandara
        /// </summary>
        /// <returns>JSON obj</returns>
        /// 
        [Route("UploadProductLogoImage")]
        [HttpPost] // This is from System.Web.Http, and not from System.Web.Mvc
        public async Task<string> UploadProductLogoImage()
        {
            FileUploadService fileService = new FileUploadService();

            if (!Request.Content.IsMimeMultipartContent())
            {
                this.Request.CreateResponse(HttpStatusCode.UnsupportedMediaType);
            }
            var uploadFolder = new StringBuilder().Append("~").Append(ConfigurationManager.AppSettings["ProductImagePath"]).Append("/Temp").ToString();
            var provider = fileService.GetMultipartProvider(uploadFolder);
            var result = await Request.Content.ReadAsMultipartAsync(provider);
            return result.FileData.First().LocalFileName;
        }

        #endregion


        #region Site main image uploads
        /// <summary>
        /// upload Site main image
        /// 
        /// Date                            Author/(Reviewer)                       Description
        /// ------------------------------------------------------------------------------------          
        /// 21-April-2017                      Chamara Bandara
        /// </summary>
        /// <returns>JSON obj</returns>
        /// 
        [Route("UploadSiteImage")]
        [HttpPost] // This is from System.Web.Http, and not from System.Web.Mvc
        public async Task<string> UploadSiteImage()
        {
            FileUploadService fileService = new FileUploadService();

            if (!Request.Content.IsMimeMultipartContent())
            {
                this.Request.CreateResponse(HttpStatusCode.UnsupportedMediaType);
            }
            var uploadFolder = new StringBuilder().Append("~").Append(ConfigurationManager.AppSettings["SitesImagePath"]).Append("/Temp").ToString();
            var provider = fileService.GetMultipartProvider(uploadFolder);
            var result = await Request.Content.ReadAsMultipartAsync(provider);
            return result.FileData.First().LocalFileName;
        }

        #endregion
    }
}
