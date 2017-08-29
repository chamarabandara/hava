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
            var partner = this.ObjContext.Partners;
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


        public List<PartnerProductRate> GetPartnerProducts(int partnerId , int locationId)
        {
            try
            {
                JObject obj = new JObject();
                JArray returnArr = new JArray();
                var partnerProducts = this.ObjContext.PartnerProductRates
                    .Include(x => x.LocationDetail)
                    .Include(x => x.PartnerProduct)
                    .Where(a => a.PartnerId == partnerId && a.LocationId == locationId).ToList();

                //var partnerRouteProducts = Mapper.Map<List<PartnerProductRate> , List<PartnerProductRateViewModel>>(partnerProducts);

                return partnerProducts;
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

                    if (partnerViewModel.productGridData != null)
                    {

                        foreach (var prod in partnerViewModel.productGridData)
                        {

                            PartnerProduct objProd = new PartnerProduct();
                            objProd.PartnerId = partnerId;
                            objProd.ProductId = prod.productId;
                            objProd.CreatedBy = partnerViewModel.createdBy;
                            objProd.CreatedDate = DateTime.Now;
                            objProd.ModifiedBy = partnerViewModel.createdBy;
                            objProd.ModifiedDate = DateTime.Now;
                            objProd.IsActive = true;

                            PartnerProductRate productRate = new PartnerProductRate();
                            productRate.HavaPrice = string.IsNullOrEmpty(prod.havaPrice) ? 0 : Decimal.Parse(prod.havaPrice);
                            productRate.MarketPrice = string.IsNullOrEmpty(prod.marketPrice) ? 0 : Decimal.Parse(prod.marketPrice);
                            productRate.PartnerSellingPrice = string.IsNullOrEmpty(prod.partnerSellingPrice) ? 0 : Decimal.Parse(prod.partnerSellingPrice);
                            productRate.IsMarkUp = prod.isMarkup;
                            productRate.Markup = string.IsNullOrEmpty(prod.partnerMarkup) ? 0 : Decimal.Parse(prod.partnerMarkup);
                            productRate.Percentage = string.IsNullOrEmpty(prod.partnerPercentage) ? 0 : Decimal.Parse(prod.partnerPercentage);
                            productRate.PartnerId = partnerId;
                            productRate.CreateDate = DateTime.Now;
                            objProd.PartnerProductRates.Add(productRate);

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

                #region partner product details
                JArray prodList = new JArray();
                if (objPartner.PartnerProducts.Count > 0)
                {
                    var list = objPartner.PartnerProducts.Where(p => p.IsActive == true).ToList();
                    foreach (var objProd in list)
                    {

                        JObject prod = new JObject();
                        prod.Add("id" , objProd.Id);
                        //prod.Add("havaPrice" , objProd.HavaPrice);
                        //prod.Add("marketPrice" , objProd.MarketPrice);
                        //prod.Add("partnerSellingPrice" , objProd.PartnerSellingPrice);
                        //prod.Add("isMarkup" , objProd.IsMarkUp == true ? "True" : "False");
                        //prod.Add("partnerMarkup" , objProd.Markup);
                        //prod.Add("partnerPercentage" , objProd.Percentage);
                        prod.Add("productId" , objProd.ProductId);
                        prod.Add("productName" , objProd.Product.Name);

                        var prodRate = objProd.PartnerProductRates.FirstOrDefault();
                        if (prodRate != null)
                        {
                            prod.Add("havaPrice" , prodRate.HavaPrice);
                            prod.Add("marketPrice" , prodRate.MarketPrice);
                            prod.Add("partnerSellingPrice" , prodRate.PartnerSellingPrice);
                            prod.Add("isMarkup" , prodRate.IsMarkUp == true ? "True" : "False");
                            prod.Add("partnerMarkup" , prodRate.Markup);
                            prod.Add("partnerPercentage" , prodRate.Percentage);
                        }

                        prodList.Add(prod);
                    }
                }
                returnObj.Add("productGridData" , prodList);
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

                                #region Partner Products
                                var recentProds = partner.PartnerProducts.ToList();
                                foreach (var prod in recentProds)
                                {
                                    List<int> ids = partnerViewModel.productGridData.Select(i => i.id).ToList();
                                    if (ids.Contains(prod.Id))
                                    {
                                        var selProd = partnerViewModel.productGridData.FirstOrDefault(o => o.id == prod.Id);
                                        prod.PartnerId = partner.Id;
                                        prod.ProductId = selProd.productId;
                                        prod.ModifiedBy = partnerViewModel.createdBy;
                                        prod.ModifiedDate = DateTime.Now;

                                        var partProdRate = prod.PartnerProductRates.FirstOrDefault();
                                        if (partProdRate != null)
                                        {
                                            partProdRate.HavaPrice = string.IsNullOrEmpty(selProd.havaPrice) ? 0 : Decimal.Parse(selProd.havaPrice);
                                            partProdRate.MarketPrice = string.IsNullOrEmpty(selProd.marketPrice) ? 0 : Decimal.Parse(selProd.marketPrice);
                                            partProdRate.PartnerSellingPrice = string.IsNullOrEmpty(selProd.partnerSellingPrice) ? 0 : Decimal.Parse(selProd.partnerSellingPrice);
                                            partProdRate.IsMarkUp = selProd.isMarkup;
                                            partProdRate.Markup = string.IsNullOrEmpty(selProd.partnerMarkup) ? 0 : Decimal.Parse(selProd.partnerMarkup);
                                            partProdRate.Percentage = string.IsNullOrEmpty(selProd.partnerPercentage) ? 0 : Decimal.Parse(selProd.partnerPercentage);
                                        }

                                    }
                                    else
                                    {
                                        prod.IsActive = false;
                                        prod.ModifiedBy = partnerViewModel.createdBy;
                                        prod.ModifiedDate = DateTime.Now;

                                    }
                                    this.ObjContext.Entry(prod).State = System.Data.Entity.EntityState.Modified;
                                    this.ObjContext.SaveChanges();
                                }
                                var newProds = partnerViewModel.productGridData.Where(p => p.id <= 0).ToList();
                                foreach (var newprod in newProds)
                                {
                                    PartnerProduct objProd = new PartnerProduct();
                                    int repId = 0;
                                    objProd.PartnerId = partner.Id;
                                    objProd.ProductId = newprod.productId;
                                    objProd.CreatedBy = partnerViewModel.createdBy;
                                    objProd.CreatedDate = DateTime.Now;
                                    objProd.ModifiedBy = partnerViewModel.createdBy;
                                    objProd.ModifiedDate = DateTime.Now;
                                    objProd.IsActive = true;

                                    PartnerProductRate productRate = new PartnerProductRate();
                                    productRate.HavaPrice = string.IsNullOrEmpty(newprod.havaPrice) ? 0 : Decimal.Parse(newprod.havaPrice);
                                    productRate.MarketPrice = string.IsNullOrEmpty(newprod.marketPrice) ? 0 : Decimal.Parse(newprod.marketPrice);
                                    productRate.PartnerSellingPrice = string.IsNullOrEmpty(newprod.partnerSellingPrice) ? 0 : Decimal.Parse(newprod.partnerSellingPrice);
                                    productRate.IsMarkUp = newprod.isMarkup;
                                    productRate.Markup = string.IsNullOrEmpty(newprod.partnerMarkup) ? 0 : Decimal.Parse(newprod.partnerMarkup);
                                    productRate.Percentage = string.IsNullOrEmpty(newprod.partnerPercentage) ? 0 : Decimal.Parse(newprod.partnerPercentage);
                                    productRate.PartnerId = partner.Id; ;
                                    productRate.CreateDate = DateTime.Now;
                                    objProd.PartnerProductRates.Add(productRate);

                                    this.ObjContext.PartnerProducts.Add(objProd);
                                    this.ObjContext.SaveChanges();

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