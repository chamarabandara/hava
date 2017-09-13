using HavaBusiness;
using HavaBusinessObjects.ViewModels;
using Newtonsoft.Json.Linq;
using System;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace HavaBusinessObjects.ControllerRepository
{

    public class ProductRepository : IDisposable
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

        #region Get Product List
        public JObject GetProductsList()
        {
            JObject obj = new JObject();
            JArray returnArr = new JArray();
            var products = this.ObjContext.Products.ToList();
            foreach (var product in products)
            {
                JObject productObj = new JObject();
                productObj.Add("id" , product.Id);
                productObj.Add("code" , product.Code);
                productObj.Add("description" , product.Name);
                productObj.Add("isMainProduct" , product.IsMainProduct == true ? "Yes" : "-");
                returnArr.Add(productObj);
            }
            obj.Add("data" , returnArr);
            return obj;
        }
        #endregion

        #region save product
        /// <summary>
        /// save product.
        /// </summary>
        /// <param name="Id">The identifier.</param>
        /// <returns></returns>
        public bool SaveProduct(ProductViewModel productViewModel)
        {
            bool isSuccess = false;

            using (var dbContextTransaction = this.ObjContext.Database.BeginTransaction())
            {
                try
                {
                    JObject returnObj = new JObject();
                    Product objProduct = new Product();
                    objProduct.Code = productViewModel.code;
                    objProduct.Name = productViewModel.name;
                    objProduct.Description = productViewModel.description;
                    objProduct.IsMainProduct = productViewModel.isMainProduct;

                    #region product logo image
                    if (productViewModel.productLogoImage != null && !string.IsNullOrEmpty(productViewModel.productLogoImage.documentPath))
                    {
                        var productLogoImagePath = ConfigurationManager.AppSettings["ProductImagePath"];

                        if (!Directory.Exists(HttpContext.Current.Server.MapPath("~" + productLogoImagePath)))
                            Directory.CreateDirectory(HttpContext.Current.Server.MapPath("~" + productLogoImagePath));

                        StringBuilder docPath = new StringBuilder();
                        StringBuilder fileNameBuilder = new StringBuilder();

                        var name = Path.GetFileNameWithoutExtension(productViewModel.productLogoImage.name);
                        var ext = Path.GetExtension(productViewModel.productLogoImage.name);
                        var generatedName = new StringBuilder().Append(name).Append("_").Append(Guid.NewGuid().ToString().Substring(0 , 4)).Append(ext).ToString();

                        using (WebClient client = new WebClient())
                        {
                            client.UseDefaultCredentials = true;
                            client.DownloadFile(new Uri(productViewModel.productLogoImage.documentPath) , fileNameBuilder.Append(HttpContext.Current.Server.MapPath("~" + productLogoImagePath)).Append(generatedName).ToString());
                            File.Delete(productViewModel.productLogoImage.documentPath);

                            if (!string.IsNullOrEmpty(objProduct.ProductImagePath))
                            {
                                File.Delete(HttpContext.Current.Server.MapPath("~" + productLogoImagePath) + objProduct.ProductImageName);
                            }
                        }

                        objProduct.ProductImagePath = docPath.Append(productLogoImagePath).Append(generatedName).ToString();
                        objProduct.ProductImageName = generatedName;
                        objProduct.ProductImageSize = productViewModel.productLogoImage.size;
                        this.ObjContext.Products.Add(objProduct);
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
