using HavaBusiness;
using HavaBusinessObjects.ViewModels;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace HavaBusinessObjects.ControllerRepository
{
    public class PartnerRepository : IDisposable
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
            var partner = this.ObjContext.Partners.Where(p => p.IsActive == true).ToList();
            foreach (var part in partner)
            {
                JObject productObj = new JObject();
                productObj.Add("id", part.Id);
                productObj.Add("name", part.Name);
                productObj.Add("telephone", part.TelLandLine);
                productObj.Add("address", part.FullAddress);
                productObj.Add("email", part.Email);
                returnArr.Add(productObj);
            }
            obj.Add("data", returnArr);
            return obj;
        }
        #endregion

        #region Get Product List
        /// <summary>
        /// Gets the product.
        /// </summary>
        /// <param name="Id">The identifier.</param>
        /// <returns></returns>
        public JObject GetProduct()
        {
            JObject obj = new JObject();
            JArray returnArr = new JArray();
            var product = this.ObjContext.Products;
            foreach (var part in product)
            {
                JObject productObj = new JObject();
                productObj.Add("id", part.Id);
                productObj.Add("name", part.Name);
                productObj.Add("code", part.Code);
                productObj.Add("imagePath", part.ProductImagePath);
                returnArr.Add(productObj);
            }
            obj.Add("data", returnArr);
            return obj;
        }
        #endregion

        public Site GetPartnerSiteBySiteId(int partnerId, int siteId)
        {
            try
            {
                var partnerSite = this.ObjContext.PartnerSites
                    .Include(x => x.Site)
                    .Where(a => a.PartnerId == partnerId && a.SiteId == siteId).Select(x => x.Site).FirstOrDefault();

                //var partnerRouteProducts = Mapper.Map<List<PartnerProductRate> , List<PartnerProductRateViewModel>>(partnerProducts);

                return partnerSite;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<PartnerProduct> GetPartnerMainProductsBySite(int partnerId, int locationId)
        {
            try
            {
                JObject obj = new JObject();
                JArray returnArr = new JArray();
                var partnerProducts = this.ObjContext.PartnerProducts
                    .Include(a => a.Product)
                    .Include(a => a.LocationDetail)
                    .Where(a => a.PartnerId == partnerId && a.LocationId == locationId && a.Product.IsMainProduct == true && a.IsActive == true).ToList();

                //var partnerRouteProducts = Mapper.Map<List<PartnerProductRate> , List<PartnerProductRateViewModel>>(partnerProducts);

                return partnerProducts;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<PartnerProduct> GetPartnerSubProducts(int partnerId)
        {
            try
            {
                JObject obj = new JObject();
                JArray returnArr = new JArray();
                var partnerProducts = this.ObjContext.PartnerProducts
                    .Include(a => a.Product)
                    .Where(a => a.PartnerId == partnerId && a.Product.IsMainProduct == false && a.IsActive == true).ToList();

                //var partnerRouteProducts = Mapper.Map<List<PartnerProductRate> , List<PartnerProductRateViewModel>>(partnerProducts);

                return partnerProducts;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<PartnerProduct> GetPartnerProducts(int partnerId, int locationId)
        {
            try
            {
                JObject obj = new JObject();
                JArray returnArr = new JArray();
                var partnerProducts = this.ObjContext.PartnerProducts
                    .Include(x => x.Product)
                    .Include(x => x.LocationDetail)
                    .Where(a => a.PartnerId == partnerId && a.LocationId == locationId && a.Product.IsMainProduct == true && a.IsActive == true).ToList();

                //var partnerRouteProducts = Mapper.Map<List<PartnerProductRate> , List<PartnerProductRateViewModel>>(partnerProducts);

                return partnerProducts;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<PartnerChauffeurProduct> GetPartnerProductsChuffer(int partnerId)
        {
            try
            {
                JObject obj = new JObject();
                JArray returnArr = new JArray();
                var partnerProducts = this.ObjContext.PartnerChauffeurProducts
                    .Include(x => x.Product)
                    .Where(a => a.PartnerId == partnerId && a.Product.IsMainProduct == true && a.IsActive == true).ToList();

                //var partnerRouteProducts = Mapper.Map<List<PartnerProductRate> , List<PartnerProductRateViewModel>>(partnerProducts);

                return partnerProducts.Distinct().ToList();
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }

        public bool SavePartner(PartnerViewModel partnerViewModel)
        {
            bool isSuccess = false;

            using (var dbContextTransaction = this.ObjContext.Database.BeginTransaction())
            {
                try
                {
                    JObject returnObj = new JObject();
                    Partner objPartner = new Partner();
                    objPartner.Code = partnerViewModel.code;
                    objPartner.Name = partnerViewModel.name;
                    objPartner.FullAddress = partnerViewModel.address;
                    objPartner.Email = partnerViewModel.email;
                    objPartner.TelLandLine = partnerViewModel.telephoneLand;
                    objPartner.TelMobile = partnerViewModel.telephoneMobile;
                    objPartner.IsActive = true;
                    objPartner.CreatedBy = 1;
                    objPartner.ModifiedBy = 1;
                    objPartner.CreatedDate = DateTime.Now;
                    objPartner.ModifiedDate = DateTime.Now;


                    this.ObjContext.Partners.Add(objPartner);
                    this.ObjContext.SaveChanges();
                    var partnerId = objPartner.Id;


                    #region representative details

                    if (partnerViewModel.representativeData != null)
                    {

                        foreach (var rep in partnerViewModel.representativeData)
                        {
                            PartnerRepresentative objRep = new PartnerRepresentative();
                            int repId = 0;
                            objRep.PartnerId = partnerId;
                            objRep.Name = rep.name;
                            objRep.TelephoneNo = rep.teleNo;
                            objRep.MobileNo = rep.mobileNo;
                            objRep.Email = rep.email;
                            objRep.CreatedBy = 1;
                            objRep.ModifiedBy = 1;
                            objRep.CreatedDate = DateTime.Now;
                            objRep.ModifiedDate = DateTime.Now;
                            objRep.IsActive = rep.status == "Active" ? true : false;

                            this.ObjContext.PartnerRepresentatives.Add(objRep);
                            this.ObjContext.SaveChanges();
                            repId = objRep.Id;
                            if (rep.userName != null && rep.password != null)
                            {
                                User user = new User();
                                user.UserName = rep.userName;
                                user.PasswordEncrypt = rep.password;
                                user.FirstName = rep.name;
                                user.Email = rep.email;
                                user.TelLandLine = rep.teleNo;
                                user.TelMobile = rep.mobileNo;
                                user.CreatedBy = 1;
                                user.ModifiedBy = 1;
                                user.CreatedDate = DateTime.Now;
                                user.ModifiedDate = DateTime.Now;
                                this.ObjContext.Users.Add(user);
                                this.ObjContext.SaveChanges();
                                objRep.UserId = user.Id;
                                this.ObjContext.SaveChanges();
                            }

                        }
                    }
                    #endregion

                    #region partner main product with Location details

                    if (partnerViewModel.locationProducts.Count > 0)
                    {
                        foreach (var location in partnerViewModel.locationProducts)
                        {
                            foreach (var partProd in location.products)
                            {
                                PartnerProduct objProd = new PartnerProduct();
                                objProd.PartnerId = partnerId;
                                objProd.ProductId = partProd.ProductId;
                                objProd.CreatedBy = partnerViewModel.createdBy;
                                objProd.HavaPrice = partProd.HavaPrice;
                                objProd.MarketPrice = partProd.MarketPrice;
                                objProd.PartnerSellingPrice = partProd.PartnerSellingPrice;
                                objProd.Percentage = partProd.Percentage;
                                objProd.LocationId = location.location.Id;
                                objProd.CreatedDate = DateTime.Now;
                                objProd.ModifiedBy = partnerViewModel.createdBy;
                                objProd.ModifiedDate = DateTime.Now;
                                objProd.IsActive = true;
                                this.ObjContext.PartnerProducts.Add(objProd);
                                this.ObjContext.SaveChanges();

                            }
                        }

                    }
                    #endregion

                    #region partner sub product details

                    if (partnerViewModel.subProductDetails.Count > 0)
                    {
                        foreach (var partProd in partnerViewModel.subProductDetails)
                        {
                            PartnerProduct objProd = new PartnerProduct();
                            objProd.PartnerId = partnerId;
                            objProd.ProductId = partProd.ProductId;
                            objProd.CreatedBy = partnerViewModel.createdBy;
                            objProd.HavaPrice = partProd.HavaPrice;
                            objProd.MarketPrice = partProd.MarketPrice;
                            objProd.PartnerSellingPrice = partProd.PartnerSellingPrice;
                            objProd.Percentage = partProd.Percentage;
                            objProd.LocationId = null;
                            objProd.CreatedDate = DateTime.Now;
                            objProd.ModifiedBy = partnerViewModel.createdBy;
                            objProd.ModifiedDate = DateTime.Now;
                            objProd.IsActive = true;
                            this.ObjContext.PartnerProducts.Add(objProd);
                            this.ObjContext.SaveChanges();

                        }

                    }
                    #endregion

                    #region partner Chauffeur product details

                    if (partnerViewModel.mainProductDetails.Count > 0)
                    {
                        foreach (var partProd in partnerViewModel.mainProductDetails)
                        {
                            PartnerChauffeurProduct objProd = new PartnerChauffeurProduct();
                            objProd.PartnerId = partnerId;
                            objProd.ProductId = partProd.ProductId;
                            objProd.CreatedBy = partnerViewModel.createdBy;
                            objProd.HavaPrice = partProd.HavaPrice;
                            objProd.MarketPrice = partProd.MarketPrice;
                            objProd.PartnerSellingPrice = partProd.PartnerSellingPrice;
                            objProd.Percentage = partProd.Percentage;
                            objProd.CreatedDate = DateTime.Now;
                            objProd.ModifiedBy = partnerViewModel.createdBy;
                            objProd.ModifiedDate = DateTime.Now;
                            objProd.IsActive = true;
                            this.ObjContext.PartnerChauffeurProducts.Add(objProd);
                            this.ObjContext.SaveChanges();

                        }

                    }
                    #endregion

                    #region partner sites

                    if (partnerViewModel.siteGridData != null)
                    {

                        foreach (var site in partnerViewModel.siteGridData)
                        {

                            PartnerSite partSite = new PartnerSite();
                            partSite.SiteId = site.siteId;
                            partSite.PartnerId = partnerId;

                            this.ObjContext.PartnerSites.Add(partSite);
                            this.ObjContext.SaveChanges();
                        }
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

        #region Get partner by Id
        /// <summary>
        /// Gets the partner.
        /// </summary>
        /// <param name="Id">The identifier.</param>
        /// <returns></returns>
        public JObject GetPartnerById(int Id)
        {
            JObject returnObj = new JObject();
            Partner objPartner = new Partner();
            objPartner = this.ObjContext.Partners.Find(Id);

            List<Product> extMainProducts = this.ObjContext.Products.Where(a => a.IsMainProduct == true).ToList();
            List<Product> extSubProducts = this.ObjContext.Products.Where(a => a.IsMainProduct == false).ToList();

            if (objPartner != null)
            {
                returnObj.Add("id", objPartner.Id);
                returnObj.Add("code", objPartner.Code);
                returnObj.Add("name", objPartner.Name);
                returnObj.Add("address", objPartner.FullAddress);
                returnObj.Add("email", objPartner.Email);
                returnObj.Add("telephoneLand", objPartner.TelLandLine);
                returnObj.Add("telephoneMobile", objPartner.TelMobile);

                JArray repList = new JArray();
                if (objPartner.PartnerRepresentatives.Count > 0)
                {
                    foreach (var rep in objPartner.PartnerRepresentatives)
                    {
                        JObject objRep = new JObject();
                        objRep.Add("id", rep.Id);
                        objRep.Add("repId", rep.Id);
                        objRep.Add("name", rep.Name);
                        objRep.Add("teleNo", rep.TelephoneNo);
                        objRep.Add("mobileNo", rep.MobileNo);
                        objRep.Add("email", rep.Email);
                        objRep.Add("status", rep.IsActive == true ? "Active" : "Inactive");
                        objRep.Add("userName", rep.UserId != null ? rep.User.UserName : string.Empty);
                        objRep.Add("password", rep.UserId != null ? rep.User.PasswordEncrypt : string.Empty);
                        repList.Add(objRep);
                    }
                }
                returnObj.Add("representativeGridData", repList);

                JArray chaffeurProducts = new JArray();
                JArray mainProducts = new JArray();
                JArray subProducts = new JArray();

                #region partner product details
                if (objPartner.PartnerProducts.Count > 0)
                {
                    var prodList = objPartner.PartnerProducts.Where(p => p.IsActive == true && p.Product.IsMainProduct == true).ToList();

                    var locations = prodList.Select(a => a.LocationDetail).Distinct().ToList();

                    var notAdded = extMainProducts.Where(a => !prodList.Select(b => b.ProductId).ToList().Contains(a.Id));

                    foreach (var location in locations)
                    {
                        JObject productsRoute = new JObject();
                        JArray mainProductArr = new JArray();
                        JObject productLocation = new JObject();

                        productLocation.Add("id", location.Id);
                        productLocation.Add("name", location.name);
                        productLocation.Add("fromLocation", location.FromLocation);
                        productLocation.Add("toLocation", location.ToLocation);
                        productLocation.Add("isAirportTour", location.IsAirPortTour);

                        productsRoute.Add("location", productLocation);


                        foreach (var partProd in prodList.Where(a => a.LocationId == location.Id))
                        {
                            JObject product = new JObject();

                            product.Add("id", partProd.Id);
                            product.Add("partnerId", objPartner.Id);
                            product.Add("productId", partProd.ProductId);
                            product.Add("Name", partProd.Product.Name);
                            product.Add("LocationId", partProd.LocationId);
                            product.Add("HavaPrice", partProd.HavaPrice);
                            product.Add("MarketPrice", partProd.MarketPrice);
                            product.Add("PartnerSellingPrice", partProd.PartnerSellingPrice);
                            product.Add("Percentage", partProd.Percentage);
                            product.Add("IsInclude", true);

                            mainProductArr.Add(product);
                        }

                        foreach (var notAddedPrd in notAdded)
                        {
                            JObject product = new JObject();

                            product.Add("id", 0);
                            product.Add("partnerId", objPartner.Id);
                            product.Add("productId", notAddedPrd.Id);
                            product.Add("Name", notAddedPrd.Name);
                            product.Add("LocationId", location.Id);
                            product.Add("HavaPrice", 0);
                            product.Add("MarketPrice", 0);
                            product.Add("PartnerSellingPrice", 0);
                            product.Add("Percentage", 0);
                            product.Add("IsInclude", false);

                            mainProductArr.Add(product);
                        }

                        productsRoute.Add("products", mainProductArr);

                        mainProducts.Add(productsRoute);
                    }

                }
                returnObj.Add("locationProducts", mainProducts);
                #endregion

                #region partner chauffeur product details

                if (objPartner.PartnerProducts.Count > 0)
                {
                    var prodList = objPartner.PartnerChauffeurProducts.Where(p => p.IsActive == true).ToList();
                    var notAdded = extMainProducts.Where(a => !prodList.Select(b => b.ProductId).ToList().Contains(a.Id));

                    foreach (var partProd in prodList)
                    {
                        JObject product = new JObject();
                        JObject productLocation = new JObject();

                        product.Add("Id", partProd.Id);
                        product.Add("partnerId", objPartner.Id);
                        product.Add("ProductId", partProd.ProductId);
                        product.Add("Name", partProd.Product.Name);
                        product.Add("HavaPrice", partProd.HavaPrice);
                        product.Add("MarketPrice", partProd.MarketPrice);
                        product.Add("PartnerSellingPrice", partProd.PartnerSellingPrice);
                        product.Add("Percentage", partProd.Percentage);
                        product.Add("IsInclude", true);

                        chaffeurProducts.Add(product);
                    }

                    foreach (var notAddedPrd in notAdded)
                    {
                        JObject product = new JObject();

                        product.Add("Id", 0);
                        product.Add("partnerId", objPartner.Id);
                        product.Add("ProductId", notAddedPrd.Id);
                        product.Add("Name", notAddedPrd.Name);
                        product.Add("HavaPrice", 0);
                        product.Add("MarketPrice", 0);
                        product.Add("PartnerSellingPrice", 0);
                        product.Add("Percentage", 0);
                        product.Add("IsInclude", false);

                        chaffeurProducts.Add(product);
                    }
                }
                else
                {
                    foreach (var notAddedPrd in extMainProducts)
                    {
                        JObject product = new JObject();

                        product.Add("Id", 0);
                        product.Add("partnerId", objPartner.Id);
                        product.Add("ProductId", notAddedPrd.Id);
                        product.Add("Name", notAddedPrd.Name);
                        product.Add("HavaPrice", 0);
                        product.Add("MarketPrice", 0);
                        product.Add("PartnerSellingPrice", 0);
                        product.Add("Percentage", 0);
                        product.Add("IsInclude", false);

                        chaffeurProducts.Add(product);
                    }
                }
                returnObj.Add("mainProductDetails", chaffeurProducts);
                #endregion

                #region partner sub product details
                if (objPartner.PartnerProducts.Count > 0)
                {
                    var prodList = objPartner.PartnerProducts.Where(p => p.IsActive == true && p.Product.IsMainProduct == false).ToList();
                    var notAddedSub = extSubProducts.Where(a => !prodList.Select(b => b.ProductId).ToList().Contains(a.Id));

                    foreach (var partProd in prodList)
                    {
                        JObject product = new JObject();
                        JObject productLocation = new JObject();

                        product.Add("Id", partProd.Id);
                        product.Add("partnerId", objPartner.Id);
                        product.Add("ProductId", partProd.ProductId);
                        product.Add("Name", partProd.Product.Name);
                        product.Add("LocationId", partProd.LocationId);
                        product.Add("HavaPrice", partProd.HavaPrice);
                        product.Add("MarketPrice", partProd.MarketPrice);
                        product.Add("PartnerSellingPrice", partProd.PartnerSellingPrice);
                        product.Add("Percentage", partProd.Percentage);
                        product.Add("IsInclude", true);

                        product.Add("LocationDetails", productLocation);

                        subProducts.Add(product);
                    }

                    foreach (var partProd in notAddedSub)
                    {
                        JObject product = new JObject();
                        JObject productLocation = new JObject();

                        product.Add("Id", partProd.Id);
                        product.Add("partnerId", objPartner.Id);
                        product.Add("ProductId", partProd.Id);
                        product.Add("Name", partProd.Name);
                        product.Add("LocationId", 0);
                        product.Add("HavaPrice", 0);
                        product.Add("MarketPrice", 0);
                        product.Add("PartnerSellingPrice", 0);
                        product.Add("Percentage", 0);
                        product.Add("IsInclude", false);

                        product.Add("LocationDetails", productLocation);

                        subProducts.Add(product);
                    }
                }
                else
                {
                    foreach (var partProd in extSubProducts)
                    {
                        JObject product = new JObject();
                        JObject productLocation = new JObject();

                        product.Add("Id", partProd.Id);
                        product.Add("partnerId", objPartner.Id);
                        product.Add("ProductId", partProd.Id);
                        product.Add("Name", partProd.Name);
                        product.Add("LocationId", 0);
                        product.Add("HavaPrice", 0);
                        product.Add("MarketPrice", 0);
                        product.Add("PartnerSellingPrice", 0);
                        product.Add("Percentage", 0);
                        product.Add("IsInclude", false);

                        product.Add("LocationDetails", productLocation);

                        subProducts.Add(product);
                    }
                }

                returnObj.Add("subProducts", subProducts);
                #endregion

                #region partner sites
                JArray siteList = new JArray();
                if (objPartner.PartnerSites.Count > 0)
                {
                    foreach (var site in objPartner.PartnerSites)
                    {
                        JObject st = new JObject();
                        st.Add("id", site.ID);
                        st.Add("siteId", site.SiteId);
                        st.Add("name", site.Site.siteName);

                        siteList.Add(st);
                    }
                }
                returnObj.Add("siteGridData", siteList);
                #endregion
            }

            return returnObj;
        }
        #endregion

        #region Update Partner
        /// <summary>
        /// Update Partner
        /// </summary>
        /// </summary>
        /// Date           Author/(Reviewer)        Description
        /// ------------------------------------------------------------------------------------          
        /// 23-Aug-2017    VISH                 Created.
        /// 
        public bool UpdatePartner(PartnerViewModel partnerViewModel)
        {
            try
            {
                if (partnerViewModel == null)
                {
                    throw new ArgumentNullException("item");
                }
                else
                {
                    Partner partner = new Partner();
                    using (var dbContextTransaction = this.ObjContext.Database.BeginTransaction())
                    {
                        try
                        {
                            partner = this.ObjContext.Partners.Find(partnerViewModel.id);
                            if (partner != null)
                            {
                                partner.Name = partnerViewModel.name;
                                partner.Email = partnerViewModel.email;
                                partner.TelLandLine = partnerViewModel.telephoneLand;
                                partner.TelMobile = partnerViewModel.telephoneMobile;
                                partner.FullAddress = partnerViewModel.address;
                                partner.IsActive = true;
                                partner.IsCompany = true;
                                partner.ModifiedBy = partnerViewModel.createdBy;
                                partner.ModifiedDate = DateTime.Now;

                                #region Partner Representative
                                var recentRepss = partner.PartnerRepresentatives.ToList();
                                foreach (var rep in recentRepss)
                                {
                                    List<int> ids = partnerViewModel.representativeData.Select(i => i.id).ToList();
                                    if (ids.Contains(rep.Id))
                                    {
                                        var selRep = partnerViewModel.representativeData.FirstOrDefault(o => o.id == rep.Id);
                                        rep.PartnerId = partner.Id;
                                        rep.Name = selRep.name;
                                        rep.TelephoneNo = selRep.teleNo;
                                        rep.MobileNo = selRep.mobileNo;
                                        rep.Email = selRep.email;
                                        rep.ModifiedBy = partnerViewModel.createdBy;
                                        rep.ModifiedDate = DateTime.Now;
                                        if (selRep.status == "Active") rep.IsActive = true; else if (selRep.status == "Inactive") rep.IsActive = false;
                                        if (rep.User != null)
                                        {
                                            User user = new User();
                                            user.UserName = selRep.userName;
                                            user.PasswordEncrypt = selRep.password;
                                            user.FirstName = selRep.name;
                                            user.Email = selRep.email;
                                            user.TelLandLine = selRep.teleNo;
                                            user.TelMobile = selRep.mobileNo;
                                            user.ModifiedBy = partnerViewModel.createdBy;
                                            user.ModifiedDate = DateTime.Now;
                                        }
                                    }
                                    else
                                    {
                                        rep.IsActive = false;
                                        rep.ModifiedBy = partnerViewModel.createdBy;
                                        rep.ModifiedDate = DateTime.Now;
                                        if (rep.User != null)
                                        {
                                            rep.User.IsActive = false;
                                            rep.User.ModifiedBy = partnerViewModel.createdBy;
                                            rep.User.ModifiedDate = DateTime.Now;
                                        }

                                    }
                                    this.ObjContext.Entry(rep).State = System.Data.Entity.EntityState.Modified;
                                    this.ObjContext.SaveChanges();
                                }
                                var newReps = partnerViewModel.representativeData.Where(p => p.id <= 0).ToList();
                                foreach (var newrep in newReps)
                                {
                                    PartnerRepresentative objRep = new PartnerRepresentative();
                                    int repId = 0;
                                    objRep.PartnerId = partner.Id;
                                    objRep.Name = newrep.name;
                                    objRep.TelephoneNo = newrep.teleNo;
                                    objRep.MobileNo = newrep.mobileNo;
                                    objRep.Email = newrep.email;
                                    objRep.CreatedBy = partnerViewModel.createdBy;
                                    objRep.ModifiedBy = partnerViewModel.createdBy;
                                    objRep.CreatedDate = DateTime.Now;
                                    objRep.ModifiedDate = DateTime.Now;
                                    objRep.IsActive = newrep.status == "Active" ? true : false;

                                    this.ObjContext.PartnerRepresentatives.Add(objRep);
                                    this.ObjContext.SaveChanges();
                                    repId = objRep.Id;
                                    if (newrep.userName != null && newrep.password != null)
                                    {
                                        User user = new User();
                                        user.UserName = newrep.userName;
                                        user.PasswordEncrypt = newrep.password;
                                        user.FirstName = newrep.name;
                                        user.Email = newrep.email;
                                        user.TelLandLine = newrep.teleNo;
                                        user.TelMobile = newrep.mobileNo;
                                        user.CreatedBy = partnerViewModel.createdBy;
                                        user.ModifiedBy = partnerViewModel.createdBy;
                                        user.CreatedDate = DateTime.Now;
                                        user.ModifiedDate = DateTime.Now;
                                        this.ObjContext.Users.Add(user);
                                        this.ObjContext.SaveChanges();
                                        objRep.UserId = user.Id;
                                        this.ObjContext.SaveChanges();
                                    }
                                }

                                #endregion

                                #region Partner Sub Products

                                foreach (var subProd in partnerViewModel.subProductDetails)
                                {
                                    if (subProd.Id > 0)
                                    {
                                        var partnerProd = this.ObjContext.PartnerProducts.Find(subProd.Id);
                                        partnerProd.HavaPrice = subProd.HavaPrice;
                                        partnerProd.IsActive = subProd.IsActive;
                                        partnerProd.IsMarkUp = subProd.IsMarkUp;
                                        partnerProd.MarketPrice = subProd.MarketPrice;
                                        partnerProd.Markup = subProd.Markup;
                                        partnerProd.ModifiedBy = partnerViewModel.createdBy;
                                        partnerProd.ModifiedDate = DateTime.Now;

                                        this.ObjContext.Entry(partnerProd).State = System.Data.Entity.EntityState.Modified;
                                        this.ObjContext.SaveChanges();
                                    }
                                    else
                                    {
                                        PartnerProduct partnerProd = new PartnerProduct();
                                        partnerProd.PartnerId = partner.Id;
                                        partnerProd.HavaPrice = subProd.HavaPrice;
                                        partnerProd.IsActive = subProd.IsActive;
                                        partnerProd.IsMarkUp = subProd.IsMarkUp;
                                        partnerProd.LocationId = null;
                                        partnerProd.MarketPrice = subProd.MarketPrice;
                                        partnerProd.Markup = subProd.Markup;
                                        partnerProd.CreatedBy = partnerViewModel.createdBy;
                                        partnerProd.CreatedDate = DateTime.Now;

                                        this.ObjContext.PartnerProducts.Add(partnerProd);
                                        this.ObjContext.SaveChanges();
                                    }
                                }

                                #endregion

                                #region Partner Chauffeur Products
                                foreach (var prod in partnerViewModel.mainProductDetails)
                                {
                                    if (prod.Id > 0)
                                    {
                                        if (prod.IsActive == true)
                                        {
                                            var partnerProd = this.ObjContext.PartnerChauffeurProducts.Find(prod.Id);
                                            partnerProd.HavaPrice = prod.HavaPrice;
                                            partnerProd.IsActive = prod.IsActive;
                                            partnerProd.IsMarkUp = prod.IsMarkUp;
                                            partnerProd.MarketPrice = prod.MarketPrice;
                                            partnerProd.Markup = prod.Markup;
                                            partnerProd.ModifiedBy = partnerViewModel.createdBy;
                                            partnerProd.ModifiedDate = DateTime.Now;

                                            this.ObjContext.Entry(partnerProd).State = System.Data.Entity.EntityState.Modified;
                                            this.ObjContext.SaveChanges();
                                        }
                                        else
                                        {
                                            var removedProd = this.ObjContext.PartnerChauffeurProducts.Find(prod.Id);
                                            removedProd.IsActive = false;
                                            removedProd.ModifiedBy = partnerViewModel.createdBy;
                                            removedProd.ModifiedDate = DateTime.Now;

                                            this.ObjContext.Entry(partner).State = System.Data.Entity.EntityState.Modified;
                                            this.ObjContext.SaveChanges();

                                        }
                                    }
                                    else
                                    {
                                        PartnerChauffeurProduct objProd = new PartnerChauffeurProduct();
                                        objProd.PartnerId = partnerViewModel.id;
                                        objProd.ProductId = prod.Product.Id;
                                        objProd.CreatedBy = partnerViewModel.createdBy;
                                        objProd.HavaPrice = prod.HavaPrice;
                                        objProd.MarketPrice = prod.MarketPrice;
                                        objProd.PartnerSellingPrice = prod.PartnerSellingPrice;
                                        objProd.Percentage = prod.Percentage;
                                        objProd.CreatedDate = DateTime.Now;
                                        objProd.ModifiedBy = partnerViewModel.createdBy;
                                        objProd.ModifiedDate = DateTime.Now;
                                        objProd.IsActive = true;
                                        this.ObjContext.PartnerChauffeurProducts.Add(objProd);
                                        this.ObjContext.SaveChanges();
                                    }

                                }
                                #endregion

                                #region Partner Main Products for Locations

                                if (partnerViewModel.locationProducts.Count > 0)
                                {
                                    foreach (var location in partnerViewModel.locationProducts)
                                    {
                                        foreach (var partProd in location.products)
                                        {
                                            if (partProd.Id > 0)
                                            {
                                                var partnerProd = this.ObjContext.PartnerProducts.Find(partProd.Id);
                                                partnerProd.LocationId = location.location.Id;
                                                partnerProd.HavaPrice = partProd.HavaPrice;
                                                partnerProd.IsActive = partProd.IsActive;
                                                partnerProd.IsMarkUp = partProd.IsMarkUp;
                                                partnerProd.PartnerSellingPrice =
                                                partnerProd.MarketPrice = partProd.MarketPrice;
                                                partnerProd.Markup = partProd.Markup;
                                                partnerProd.ModifiedBy = partnerViewModel.createdBy;
                                                partnerProd.ModifiedDate = DateTime.Now;

                                                this.ObjContext.Entry(partnerProd).State = System.Data.Entity.EntityState.Modified;
                                                this.ObjContext.SaveChanges();
                                            }
                                            else
                                            {
                                                PartnerProduct objProd = new PartnerProduct();
                                                objProd.PartnerId = partnerViewModel.id;
                                                objProd.ProductId = partProd.Product.Id;
                                                objProd.CreatedBy = partnerViewModel.createdBy;
                                                objProd.HavaPrice = partProd.HavaPrice;
                                                objProd.MarketPrice = partProd.MarketPrice;
                                                objProd.PartnerSellingPrice = partProd.PartnerSellingPrice;
                                                objProd.Percentage = partProd.Percentage;
                                                objProd.LocationId = location.location.Id;
                                                objProd.CreatedDate = DateTime.Now;
                                                objProd.ModifiedBy = partnerViewModel.createdBy;
                                                objProd.ModifiedDate = DateTime.Now;
                                                objProd.IsActive = true;
                                                this.ObjContext.PartnerProducts.Add(objProd);
                                                this.ObjContext.SaveChanges();

                                            }
                                        }
                                    }

                                }

                                #endregion

                                #region partner sites
                                var recentSites = partner.PartnerSites.ToList();
                                foreach (var site in recentSites)
                                {
                                    List<int> ids = partnerViewModel.siteGridData.Select(i => i.id).ToList();
                                    if (!ids.Contains(site.ID))
                                    {
                                        this.ObjContext.PartnerSites.Remove(site);
                                        this.ObjContext.SaveChanges();
                                    }
                                }
                                var newSites = partnerViewModel.siteGridData.Where(p => p.id <= 0).ToList();
                                foreach (var newsite in newSites)
                                {
                                    PartnerSite partSite = new PartnerSite();
                                    partSite.SiteId = newsite.siteId;
                                    partSite.PartnerId = partner.Id;

                                    this.ObjContext.PartnerSites.Add(partSite);
                                    this.ObjContext.SaveChanges();

                                }

                                #endregion

                                this.ObjContext.Entry(partner).State = System.Data.Entity.EntityState.Modified;
                                this.ObjContext.SaveChanges();
                                dbContextTransaction.Commit();

                                return true;
                            }
                            else
                                return false;
                        }
                        catch (Exception ex)
                        {
                            dbContextTransaction.Rollback();
                            throw ex;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
        #endregion

        public bool DeletePartner(int id)
        {
            try
            {
                var partner = this.ObjContext.Partners.Find(id);
                if (partner != null)
                {
                    partner.IsActive = false;
                    partner.ModifiedDate = DateTime.Now;
                    this.ObjContext.Entry(partner).State = System.Data.Entity.EntityState.Modified;
                    this.ObjContext.SaveChanges();
                    return true;

                }
                else
                    return false;

            }
            catch (Exception ex)
            {

                throw ex;
            }
        }

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