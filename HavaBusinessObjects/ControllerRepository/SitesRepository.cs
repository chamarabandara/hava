using HavaBusiness;
using HavaBusinessObjects.ViewModels;
using Newtonsoft.Json.Linq;
using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Text;
using System.Web;

namespace HavaBusinessObjects.ControllerRepository
{

    public class SitesRepository : IDisposable
    {
        #region repository db context

        private HAVA_DBModelEntities context;

        private HAVA_DBModelEntities ObjContext
        {
            get
            {
                if (context == null)
                    context = new HAVA_DBModelEntities();
                return context;
            }
        }
        #endregion db context


        #region save sites
        /// <summary>
        /// save sites.
        /// </summary>
        /// <param name="Id">The identifier.</param>
        /// <returns></returns>
        public bool SaveSites(SitesViewModel sitesViewModel)
        {
            bool isSuccess = false;

            using (var dbContextTransaction = this.ObjContext.Database.BeginTransaction())
            {
                try
                {
                    JObject returnObj = new JObject();
                    Site objSites = new Site();
                    objSites.siteAlias = sitesViewModel.code;
                    objSites.siteName = sitesViewModel.name;
                    objSites.CreatedBy = 1;
                    objSites.ModifiedBy = 1;
                    objSites.CreatedDate = DateTime.Now;
                    objSites.ModifiedDate = DateTime.Now;

                    #region sites banner image
                    if (sitesViewModel.productLogoImage != null && !string.IsNullOrEmpty(sitesViewModel.productLogoImage.documentPath))
                    {
                        var productLogoImagePath = ConfigurationManager.AppSettings["SitesImagePath"];

                        if (!Directory.Exists(HttpContext.Current.Server.MapPath("~" + productLogoImagePath)))
                            Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~" + productLogoImagePath));

                        StringBuilder docPath = new StringBuilder();
                        StringBuilder fileNameBuilder = new StringBuilder();

                        var name = Path.GetFileNameWithoutExtension(sitesViewModel.productLogoImage.name);
                        var ext = Path.GetExtension(sitesViewModel.productLogoImage.name);
                        var generatedName = new StringBuilder().Append(name).Append("_").Append(Guid.NewGuid().ToString().Substring(0 , 4)).Append(ext).ToString();

                        using (WebClient client = new WebClient())
                        {
                            client.UseDefaultCredentials = true;
                            client.DownloadFile(new Uri(sitesViewModel.productLogoImage.documentPath) , fileNameBuilder.Append(HttpContext.Current.Server.MapPath("~" + productLogoImagePath)).Append(generatedName).ToString());
                            File.Delete(sitesViewModel.productLogoImage.documentPath);

                            if (!string.IsNullOrEmpty(objSites.SiteBannerPath))
                            {
                                File.Delete(HttpContext.Current.Server.MapPath("~" + productLogoImagePath) + objSites.siteBannerName);
                            }
                        }

                        objSites.SiteBannerPath = docPath.Append(productLogoImagePath).Append(generatedName).ToString();
                        objSites.siteBannerName = generatedName;
                        objSites.siteBannerSize = sitesViewModel.productLogoImage.size;
                        this.ObjContext.Sites.Add(objSites);
                        this.ObjContext.SaveChanges();
                    }

                    #endregion
                    dbContextTransaction.Commit();
                    isSuccess = true;
                    // }
                    return isSuccess;
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                    throw ex;
                }
            }

        }
        #endregion

        #region Get partner by Id
        /// <summary>
        /// Gets the partner.
        /// </summary>
        /// <param name="Id">The identifier.</param>
        /// <returns></returns>
        public JObject GetPartner()
        {
            JObject obj = new JObject();
            JArray returnArr = new JArray();
            var partner = this.ObjContext.Sites;
            foreach (var part in partner)
            {
                JObject sitesObj = new JObject();
                sitesObj.Add("id" , part.Id);
                sitesObj.Add("name" , part.siteName);
                sitesObj.Add("code" , part.siteAlias);

                returnArr.Add(sitesObj);
            }
            obj.Add("data" , returnArr);
            return obj;
        }
        #endregion

        #region Dispose
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            this.ObjContext.Dispose();
        }

        #endregion
    }


}
