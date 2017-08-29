using HavaBusinessObjects.ControllerRepository;
using HavaBusinessObjects.ViewModels;
using Newtonsoft.Json.Linq;
using System;
using System.Web.Mvc;

namespace HavaWeb.Controllers
{
    public class ProductController : Controller
    {
        //
        // GET: /Product/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Add()
        {
            return View();
        }

        public ActionResult ProductView()
        {
            return View();
        }

        public ActionResult List()
        {
            return View();
        }

        #region add Product
        /// <summary>
        /// Adds the specified Product.
        /// Date		    Author/(Reviewer)		    Description
        /// -------------------------------------------------------	
        /// 11 Aug 2015     Chamara Bandara           Creation
        /// </summary>
        /// <param name="customerViewModel">The customer view model.</param>
        /// <returns></returns>
        public JObject Post(ProductViewModel productViewModel)
        {
            try
            {
                JObject obj = new JObject();
                ProductRepository productRepository = new ProductRepository();
                bool status = productRepository.SaveProduct(productViewModel);
                obj.Add("status", status);
                return obj;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        #endregion

        //public JObject GetList()
        //{

        //    ProductRepository productRepository = new ProductRepository();

        //    return productRepository.GetProductsList();
        //}

    }
}