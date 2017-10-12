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
                productObj.Add("id" , part.Id);
                productObj.Add("name" , part.Name);
                productObj.Add("telephone" , part.TelLandLine);
                productObj.Add("address" , part.FullAddress);
                productObj.Add("email" , part.Email);
                returnArr.Add(productObj);
            }
            obj.Add("data" , returnArr);
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
                productObj.Add("id" , part.Id);
                productObj.Add("name" , part.Name);
                productObj.Add("code" , part.Code);
                productObj.Add("imagePath" , part.ProductImagePath);
                returnArr.Add(productObj);
            }
            obj.Add("data" , returnArr);
            return obj;
        }
        #endregion

        public Site GetPartnerSiteBySiteId(int partnerId , int siteId)
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
                    .Where(a => a.PartnerId == partnerId && a.LocationId == locationId && a.Product.IsMainProduct == true).ToList();

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
                    .Where(a => a.PartnerId == partnerId && a.Product.IsMainProduct == false).ToList();

                //var partnerRouteProducts = Mapper.Map<List<PartnerProductRate> , List<PartnerProductRateViewModel>>(partnerProducts);

                return partnerProducts;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public List<PartnerProduct> GetPartnerProducts(int partnerId , int locationId)
        {
            try
            {
                JObject obj = new JObject();
                JArray returnArr = new JArray();
                var partnerProducts = this.ObjContext.PartnerProducts
                    .Include(x => x.Product)
                    .Include(x => x.LocationDetail)
                    .Where(a => a.PartnerId == partnerId && a.LocationId == locationId && a.Product.IsMainProduct == true).ToList();

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
                    .Where(a => a.PartnerId == partnerId && a.Product.IsMainProduct == true).ToList();

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

                    #region partner product details
                    if (partnerViewModel.productGridData.Count > 0)
                    {
                        var locations = partnerViewModel.productGridData.Where(a => a.Product.isMainProduct == true).Select(b => b.LocationDetail).ToList();

                        foreach (var location in locations)
                        {
                            LocationDetail newLocation = new LocationDetail();
                            newLocation.FromLocation = location.FromLocation;
                            newLocation.ToLocation = location.ToLocation;
                            newLocation.IsActive = true;
                            newLocation.IsAirPortTour = location.IsAirPortTour;
                            newLocation.name = location.name;
                            newLocation.PartnerId = partnerId;
                            this.ObjContext.LocationDetails.Add(newLocation);
                            this.ObjContext.SaveChanges();

                            var locationProducts = partnerViewModel.productGridData.Where(a => a.LocationDetail.ToLocation == location.ToLocation).ToList();

                            foreach (var partProd in locationProducts)
                            {
                                PartnerProduct objProd = new PartnerProduct();
                                objProd.PartnerId = partnerId;
                                objProd.ProductId = partProd.Product.Id;
                                objProd.CreatedBy = partnerViewModel.createdBy;
                                objProd.HavaPrice = partProd.HavaPrice;
                                objProd.MarketPrice = partProd.MarketPrice;
                                objProd.PartnerSellingPrice = partProd.PartnerSellingPrice;
                                objProd.Percentage = partProd.Percentage;
                                objProd.LocationId = newLocation.Id;
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
                    if (partnerViewModel.subProductData.Count > 0)
                    {
                        foreach (var partProd in partnerViewModel.subProductData)
                        {
                            PartnerProduct objProd = new PartnerProduct();
                            objProd.PartnerId = partnerId;
                            objProd.ProductId = partProd.Product.Id;
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
            if (objPartner != null)
            {
                returnObj.Add("id" , objPartner.Id);
                returnObj.Add("code" , objPartner.Code);
                returnObj.Add("name" , objPartner.Name);
                returnObj.Add("address" , objPartner.FullAddress);
                returnObj.Add("email" , objPartner.Email);
                returnObj.Add("telephoneLand" , objPartner.TelLandLine);
                returnObj.Add("telephoneMobile" , objPartner.TelMobile);

                JArray repList = new JArray();
                if (objPartner.PartnerRepresentatives.Count > 0)
                {
                    foreach (var rep in objPartner.PartnerRepresentatives)
                    {
                        JObject objRep = new JObject();
                        objRep.Add("id" , rep.Id);
                        objRep.Add("repId" , rep.Id);
                        objRep.Add("name" , rep.Name);
                        objRep.Add("teleNo" , rep.TelephoneNo);
                        objRep.Add("mobileNo" , rep.MobileNo);
                        objRep.Add("email" , rep.Email);
                        objRep.Add("status" , rep.IsActive == true ? "Active" : "Inactive");
                        objRep.Add("userName" , rep.UserId != null ? rep.User.UserName : string.Empty);
                        objRep.Add("password" , rep.UserId != null ? rep.User.PasswordEncrypt : string.Empty);
                        repList.Add(objRep);
                    }
                }
                returnObj.Add("representativeGridData" , repList);

                JArray mainProducts = new JArray();
                JArray subProducts = new JArray();

                #region partner product details
                if (objPartner.PartnerProducts.Count > 0)
                {
                    var prodList = objPartner.PartnerProducts.Where(p => p.IsActive == true && p.Product.IsMainProduct == true).ToList();
                    foreach (var partProd in prodList)
                    {
                        JObject product = new JObject();
                        JObject productLocation = new JObject();

                        product.Add("id" , partProd.Id);
                        product.Add("partnerId" , objPartner.Id);
                        product.Add("productId" , partProd.ProductId);
                        product.Add("productName" , partProd.Product.Name);
                        product.Add("LocationId", partProd.LocationId);
                        product.Add("HavaPrice", partProd.HavaPrice);
                        product.Add("MarketPrice", partProd.MarketPrice);
                        product.Add("PartnerSellingPrice", partProd.PartnerSellingPrice);
                        product.Add("Percentage", partProd.Percentage);


                        productLocation.Add("id", partProd.LocationDetail.Id);
                        productLocation.Add("name", partProd.LocationDetail.name);
                        productLocation.Add("fromLocation", partProd.LocationDetail.FromLocation);
                        productLocation.Add("toLocation", partProd.LocationDetail.ToLocation);
                        productLocation.Add("isAirportTour", partProd.LocationDetail.IsAirPortTour);

                        product.Add("LocationDetails", productLocation);

                        mainProducts.Add(product);
                    }
                }
                returnObj.Add("products" , mainProducts);
                #endregion

                #region partner sub product details
                if (objPartner.PartnerProducts.Count > 0)
                {
                    var prodList = objPartner.PartnerProducts.Where(p => p.IsActive == true && p.Product.IsMainProduct == false).ToList();
                    foreach (var partProd in prodList)
                    {
                        JObject product = new JObject();
                        JObject productLocation = new JObject();

                        product.Add("id", partProd.Id);
                        product.Add("partnerId", objPartner.Id);
                        product.Add("productId", partProd.ProductId);
                        product.Add("productName", partProd.Product.Name);
                        product.Add("LocationId", partProd.LocationId);
                        product.Add("HavaPrice", partProd.HavaPrice);
                        product.Add("MarketPrice", partProd.MarketPrice);
                        product.Add("PartnerSellingPrice", partProd.PartnerSellingPrice);
                        product.Add("Percentage", partProd.Percentage);
                        
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
                        st.Add("id" , site.ID);
                        st.Add("siteId" , site.SiteId);
                        st.Add("name" , site.Site.siteName);

                        siteList.Add(st);
                    }
                }
                returnObj.Add("siteGridData" , siteList);
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

                                foreach (var subProd in partnerViewModel.subProductData)
                                {
                                    if (subProd.id > 0)
                                    {
                                        var partnerProd = this.ObjContext.PartnerProducts.Find(subProd.id);
                                        partnerProd.HavaPrice   = subProd.HavaPrice;
                                        partnerProd.IsActive    = subProd.isActive;
                                        partnerProd.IsMarkUp    = subProd.IsMarkUp;
                                        partnerProd.MarketPrice = subProd.MarketPrice;
                                        partnerProd.Markup      = subProd.Markup;
                                        partnerProd.ModifiedBy  = partnerViewModel.createdBy;
                                        partnerProd.ModifiedDate = DateTime.Now;

                                        this.ObjContext.Entry(partnerProd).State = System.Data.Entity.EntityState.Modified;
                                        this.ObjContext.SaveChanges();
                                    }
                                    else
                                    {
                                        PartnerProduct partnerProd = new PartnerProduct();
                                        partnerProd.PartnerId = partner.Id;
                                        partnerProd.HavaPrice   = subProd.HavaPrice;
                                        partnerProd.IsActive    = subProd.isActive;
                                        partnerProd.IsMarkUp    = subProd.IsMarkUp;
                                        partnerProd.LocationId  = null;
                                        partnerProd.MarketPrice = subProd.MarketPrice;
                                        partnerProd.Markup      = subProd.Markup;
                                        partnerProd.CreatedBy   = partnerViewModel.createdBy;
                                        partnerProd.CreatedDate = DateTime.Now;

                                        this.ObjContext.PartnerProducts.Add(partnerProd);
                                        this.ObjContext.SaveChanges();
                                    }
                                }

                                #endregion

                                #region Partner Products
                                foreach (var prod in partnerViewModel.productGridData)
                                {
                                    if (prod.id > 0)
                                    {
                                        if (prod.isActive == true)
                                        {
                                            if (prod.LocationDetail.Id > 0)
                                            {
                                                var partnerProd = this.ObjContext.PartnerProducts.Find(prod.id);
                                                partnerProd.HavaPrice = prod.HavaPrice;
                                                partnerProd.IsActive = prod.isActive;
                                                partnerProd.IsMarkUp = prod.IsMarkUp;
                                                partnerProd.LocationId = prod.LocationDetail.Id;
                                                partnerProd.MarketPrice = prod.MarketPrice;
                                                partnerProd.Markup = prod.Markup;
                                                partnerProd.ModifiedBy = partnerViewModel.createdBy;
                                                partnerProd.ModifiedDate = DateTime.Now;

                                                this.ObjContext.Entry(partnerProd).State = System.Data.Entity.EntityState.Modified;

                                                this.ObjContext.SaveChanges();
                                            }
                                            else
                                            {
                                                LocationDetail location = new LocationDetail();
                                                location.FromLocation = prod.LocationDetail.FromLocation;
                                                location.ToLocation = prod.LocationDetail.ToLocation;
                                                location.IsActive = true;
                                                location.IsAirPortTour = prod.LocationDetail.IsAirPortTour;
                                                location.name = prod.LocationDetail.name;
                                                location.PartnerId = partner.Id;
                                                this.ObjContext.LocationDetails.Add(location);
                                                this.ObjContext.SaveChanges();

                                                PartnerProduct partnerProd = new PartnerProduct();
                                                partnerProd.HavaPrice = prod.HavaPrice;
                                                partnerProd.IsActive = prod.isActive;
                                                partnerProd.IsMarkUp = prod.IsMarkUp;
                                                partnerProd.LocationId = location.Id;
                                                partnerProd.MarketPrice = prod.MarketPrice;
                                                partnerProd.Markup = prod.Markup;
                                                partnerProd.CreatedBy = partnerViewModel.createdBy;
                                                partnerProd.CreatedDate = DateTime.Now;
                                                
                                                this.ObjContext.PartnerProducts.Add(partnerProd);
                                                this.ObjContext.SaveChanges();
                                            }
                                        }
                                        else
                                        {
                                            var removedProd = this.ObjContext.PartnerProducts.Find(prod.id);
                                            removedProd.IsActive = false;
                                            removedProd.ModifiedBy = partnerViewModel.createdBy;
                                            removedProd.ModifiedDate = DateTime.Now;
                                            
                                            this.ObjContext.Entry(partner).State = System.Data.Entity.EntityState.Modified;
                                            this.ObjContext.SaveChanges();

                                        }
                                    }
                                    else
                                    {
                                        
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