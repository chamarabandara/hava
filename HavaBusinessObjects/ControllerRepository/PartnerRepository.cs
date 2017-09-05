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
                    .Where(a => a.PartnerId == partnerId && a.LocationId == locationId && a.PartnerProduct.Product.IsMainProduct == true).ToList();

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
                    if (partnerViewModel.productGridData.Count > 0)
                    {
                        foreach (var partProd in partnerViewModel.productGridData)
                        {
                            PartnerProduct objProd = new PartnerProduct();
                            objProd.PartnerId = partnerId;
                            objProd.ProductId = partProd.productId;
                            objProd.CreatedBy = partnerViewModel.createdBy;
                            objProd.CreatedDate = DateTime.Now;
                            objProd.ModifiedBy = partnerViewModel.createdBy;
                            objProd.ModifiedDate = DateTime.Now;
                            objProd.IsActive = true;
                            this.ObjContext.PartnerProducts.Add(objProd);
                            this.ObjContext.SaveChanges();

                            foreach (var prodLoc in partProd.partnerLocations)
                            {
                                LocationDetail location = new LocationDetail();
                                location.FromLocation = prodLoc.fromLocation;
                                location.ToLocation = prodLoc.toLocation;
                                location.IsActive = true;
                                location.IsAirPortTour = prodLoc.isAirPortTour;
                                location.name = prodLoc.locationName;
                                location.PartnerId = partnerId;
                                this.ObjContext.LocationDetails.Add(location);
                                this.ObjContext.SaveChanges();

                                PartnerProductRate productRate = new PartnerProductRate();
                                productRate.AdditionaDayRate = prodLoc.additionalDayRate;
                                productRate.AdditionaHourRate = prodLoc.additionalHourRate;
                                productRate.AirportRate = prodLoc.airPortRate;
                                productRate.ChildSeatRate = prodLoc.childSeatRate;
                                productRate.ChufferDailyRate = prodLoc.chufferDailyRate;
                                productRate.ChufferKMRate = prodLoc.chufferKMRate;
                                productRate.HavaPrice = string.IsNullOrEmpty(prodLoc.havaPrice) ? 0 : Decimal.Parse(prodLoc.havaPrice);
                                productRate.HavaPriceReturn = prodLoc.havaPriceReturn;
                                productRate.IsMarkUp = prodLoc.isMarkup;
                                productRate.LocationId = location.Id;
                                productRate.MarketPrice = string.IsNullOrEmpty(prodLoc.marketPrice) ? 0 : Decimal.Parse(prodLoc.marketPrice);
                                productRate.MarketPriceReturn = prodLoc.marketPriceReturn;
                                productRate.Markup = string.IsNullOrEmpty(prodLoc.partnerMarkup) ? 0 : Decimal.Parse(prodLoc.partnerMarkup);
                                productRate.PartnerId = partnerId;
                                productRate.PartnerProductId = objProd.Id;
                                productRate.PartnerSellingPrice = string.IsNullOrEmpty(prodLoc.partnerSellingPrice) ? 0 : Decimal.Parse(prodLoc.partnerSellingPrice);
                                productRate.PartnerSellingPriceReturn = prodLoc.partnerSellPriceReturn;
                                productRate.Percentage = string.IsNullOrEmpty(prodLoc.partnerPercentage) ? 0 : Decimal.Parse(prodLoc.partnerPercentage);
                                productRate.Rate = prodLoc.rate;
                                productRate.CreateDate = DateTime.Now;

                                this.ObjContext.PartnerProductRates.Add(productRate);
                                this.ObjContext.SaveChanges();
                            }
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

                JArray products = new JArray();

                #region partner product details
                if (objPartner.PartnerProducts.Count > 0)
                {
                    foreach (var partProd in objPartner.PartnerProducts)
                    {
                        JObject product = new JObject();
                        product.Add("id" , partProd.Id);
                        product.Add("partnerId" , objPartner.Id);
                        product.Add("productId" , partProd.ProductId);
                        product.Add("productName" , partProd.Product.Name);
                        product.Add("isActive" , partProd.IsActive);

                        JArray rates = new JArray();
                        foreach (var prodRate in partProd.PartnerProductRates)
                        {
                            JObject rate = new JObject();
                            rate.Add("id" , prodRate.Id);
                            rate.Add("fromLocation" , prodRate.LocationDetail.FromLocation);
                            rate.Add("toLocation" , prodRate.LocationDetail.ToLocation);
                            rate.Add("isActive" , prodRate.LocationDetail.IsActive);
                            rate.Add("isAirPortTour" , prodRate.LocationDetail.IsAirPortTour);
                            rate.Add("locationName" , prodRate.LocationDetail.name);

                            rate.Add("additionaDayRate" , prodRate.AdditionaDayRate);
                            rate.Add("additionaHourRate" , prodRate.AdditionaHourRate);
                            rate.Add("airportRate" , prodRate.AirportRate);
                            rate.Add("childSeatRate" , prodRate.ChildSeatRate);
                            rate.Add("chufferDailyRate" , prodRate.ChufferDailyRate);
                            rate.Add("chufferKMRate" , prodRate.ChufferKMRate);
                            rate.Add("havaPrice" , prodRate.HavaPrice);
                            rate.Add("havaPriceReturn" , prodRate.HavaPriceReturn);
                            rate.Add("isMarkUp" , prodRate.IsMarkUp);
                            rate.Add("locationId" , prodRate.LocationId);
                            rate.Add("marketPrice" , prodRate.MarketPrice);
                            rate.Add("marketPriceReturn" , prodRate.MarketPriceReturn);
                            rate.Add("markup" , prodRate.Markup);
                            rate.Add("partnerProductId" , prodRate.Id);
                            rate.Add("partnerSellingPrice" , prodRate.PartnerSellingPrice);
                            rate.Add("partnerSellingPriceReturn" , prodRate.PartnerSellingPriceReturn);
                            rate.Add("percentage" , prodRate.Percentage);
                            rate.Add("rate" , prodRate.Rate);
                            rate.Add("createDate" , prodRate.CreateDate);

                            rates.Add(rate);
                        }
                        product.Add("rates" , rates);
                        products.Add(product);
                    }
                }
                returnObj.Add("products" , products);
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
                                //foreach (var prod in partnerViewModel.productGridData)
                                //{
                                //    if (prod.id > 0)
                                //    {
                                //        if (prod.isActive == true)
                                //        {
                                //            foreach (var prodLoc in prod.partnerLocations)
                                //            {
                                //                if (prodLoc.locationId > 0)
                                //                {
                                //                    var location = this.ObjContext.LocationDetails.Find(prodLoc.locationId);
                                //                    location.FromLocation = prodLoc.fromLocation;
                                //                    location.ToLocation = prodLoc.toLocation;
                                //                    location.IsActive = true;
                                //                    location.IsAirPortTour = prodLoc.isAirPortTour;
                                //                    location.name = prodLoc.locationName;
                                //                    location.PartnerId = partner.Id;
                                //                    this.ObjContext.LocationDetails.Add(location);
                                //                    this.ObjContext.SaveChanges();
                                //                    var productRate = this.ObjContext.PartnerProductRates.Find(prodLoc.productRateId);
                                //                    productRate.AdditionaDayRate = prodLoc.additionalDayRate;
                                //                    productRate.AdditionaHourRate = prodLoc.additionalHourRate;
                                //                    productRate.AirportRate = prodLoc.airPortRate;
                                //                    productRate.ChildSeatRate = prodLoc.childSeatRate;
                                //                    productRate.ChufferDailyRate = prodLoc.chufferDailyRate;
                                //                    productRate.ChufferKMRate = prodLoc.chufferKMRate;
                                //                    productRate.HavaPrice = string.IsNullOrEmpty(prodLoc.havaPrice) ? 0 : Decimal.Parse(prodLoc.havaPrice);
                                //                    productRate.HavaPriceReturn = prodLoc.havaPriceReturn;
                                //                    productRate.IsMarkUp = prodLoc.isMarkup;
                                //                    productRate.LocationId = location.Id;
                                //                    productRate.MarketPrice = string.IsNullOrEmpty(prodLoc.marketPrice) ? 0 : Decimal.Parse(prodLoc.marketPrice);
                                //                    productRate.MarketPriceReturn = prodLoc.marketPriceReturn;
                                //                    productRate.Markup = string.IsNullOrEmpty(prodLoc.partnerMarkup) ? 0 : Decimal.Parse(prodLoc.partnerMarkup);
                                //                    productRate.PartnerId = partner.Id;
                                //                    productRate.PartnerProductId = prod.id;
                                //                    productRate.PartnerSellingPrice = string.IsNullOrEmpty(prodLoc.partnerSellingPrice) ? 0 : Decimal.Parse(prodLoc.partnerSellingPrice);
                                //                    productRate.PartnerSellingPriceReturn = prodLoc.partnerSellPriceReturn;
                                //                    productRate.Percentage = string.IsNullOrEmpty(prodLoc.partnerPercentage) ? 0 : Decimal.Parse(prodLoc.partnerPercentage);
                                //                    productRate.Rate = prodLoc.rate;
                                //                    productRate.CreateDate = DateTime.Now;

                                //                    this.ObjContext.Entry(productRate).State = System.Data.Entity.EntityState.Modified;
                                //                    this.ObjContext.Entry(location).State = System.Data.Entity.EntityState.Modified;
                                //                    this.ObjContext.SaveChanges();
                                //                }
                                //                else
                                //                {
                                //                    LocationDetail location = new LocationDetail();
                                //                    location.FromLocation = prodLoc.fromLocation;
                                //                    location.ToLocation = prodLoc.toLocation;
                                //                    location.IsActive = true;
                                //                    location.IsAirPortTour = prodLoc.isAirPortTour;
                                //                    location.name = prodLoc.locationName;
                                //                    location.PartnerId = partner.Id;
                                //                    this.ObjContext.LocationDetails.Add(location);
                                //                    this.ObjContext.SaveChanges();

                                //                    PartnerProductRate productRate = new PartnerProductRate();
                                //                    productRate.AdditionaDayRate = prodLoc.additionalDayRate;
                                //                    productRate.AdditionaHourRate = prodLoc.additionalHourRate;
                                //                    productRate.AirportRate = prodLoc.airPortRate;
                                //                    productRate.ChildSeatRate = prodLoc.childSeatRate;
                                //                    productRate.ChufferDailyRate = prodLoc.chufferDailyRate;
                                //                    productRate.ChufferKMRate = prodLoc.chufferKMRate;
                                //                    productRate.HavaPrice = string.IsNullOrEmpty(prodLoc.havaPrice) ? 0 : Decimal.Parse(prodLoc.havaPrice);
                                //                    productRate.HavaPriceReturn = prodLoc.havaPriceReturn;
                                //                    productRate.IsMarkUp = prodLoc.isMarkup;
                                //                    productRate.LocationId = location.Id;
                                //                    productRate.MarketPrice = string.IsNullOrEmpty(prodLoc.marketPrice) ? 0 : Decimal.Parse(prodLoc.marketPrice);
                                //                    productRate.MarketPriceReturn = prodLoc.marketPriceReturn;
                                //                    productRate.Markup = string.IsNullOrEmpty(prodLoc.partnerMarkup) ? 0 : Decimal.Parse(prodLoc.partnerMarkup);
                                //                    productRate.PartnerId = partner.Id;
                                //                    productRate.PartnerProductId = prod.id;
                                //                    productRate.PartnerSellingPrice = string.IsNullOrEmpty(prodLoc.partnerSellingPrice) ? 0 : Decimal.Parse(prodLoc.partnerSellingPrice);
                                //                    productRate.PartnerSellingPriceReturn = prodLoc.partnerSellPriceReturn;
                                //                    productRate.Percentage = string.IsNullOrEmpty(prodLoc.partnerPercentage) ? 0 : Decimal.Parse(prodLoc.partnerPercentage);
                                //                    productRate.Rate = prodLoc.rate;
                                //                    productRate.CreateDate = DateTime.Now;

                                //                    this.ObjContext.PartnerProductRates.Add(productRate);
                                //                    this.ObjContext.SaveChanges();
                                //                }

                                //            }
                                //        }
                                //        else
                                //        {
                                //            var removedProd = this.ObjContext.PartnerProducts.Find(prod.id);
                                //            removedProd.IsActive = false;
                                //            removedProd.ModifiedBy = partnerViewModel.createdBy;
                                //            removedProd.ModifiedDate = DateTime.Now;
                                //            foreach (var prodLoc in prod.partnerLocations)
                                //            {
                                //                var removedLoc = this.ObjContext.LocationDetails.Find(prodLoc.locationId);
                                //                removedLoc.IsActive = false;
                                //                this.ObjContext.Entry(removedLoc).State = System.Data.Entity.EntityState.Modified;
                                //                this.ObjContext.SaveChanges();
                                //            }
                                //            this.ObjContext.Entry(partner).State = System.Data.Entity.EntityState.Modified;
                                //            this.ObjContext.SaveChanges();

                                //        }
                                //    }
                                //    else
                                //    {
                                //        foreach (var partProd in partnerViewModel.productGridData)
                                //        {
                                //            PartnerProduct objProd = new PartnerProduct();
                                //            objProd.PartnerId = partner.Id;
                                //            objProd.ProductId = partProd.productId;
                                //            objProd.CreatedBy = partnerViewModel.createdBy;
                                //            objProd.CreatedDate = DateTime.Now;
                                //            objProd.ModifiedBy = partnerViewModel.createdBy;
                                //            objProd.ModifiedDate = DateTime.Now;
                                //            objProd.IsActive = true;
                                //            this.ObjContext.PartnerProducts.Add(objProd);
                                //            this.ObjContext.SaveChanges();

                                //            foreach (var prodLoc in partProd.partnerLocations)
                                //            {
                                //                LocationDetail location = new LocationDetail();
                                //                location.FromLocation = prodLoc.fromLocation;
                                //                location.ToLocation = prodLoc.toLocation;
                                //                location.IsActive = true;
                                //                location.IsAirPortTour = prodLoc.isAirPortTour;
                                //                location.name = prodLoc.locationName;
                                //                location.PartnerId = partner.Id;
                                //                this.ObjContext.LocationDetails.Add(location);
                                //                this.ObjContext.SaveChanges();

                                //                PartnerProductRate productRate = new PartnerProductRate();
                                //                productRate.AdditionaDayRate = prodLoc.additionalDayRate;
                                //                productRate.AdditionaHourRate = prodLoc.additionalHourRate;
                                //                productRate.AirportRate = prodLoc.airPortRate;
                                //                productRate.ChildSeatRate = prodLoc.childSeatRate;
                                //                productRate.ChufferDailyRate = prodLoc.chufferDailyRate;
                                //                productRate.ChufferKMRate = prodLoc.chufferKMRate;
                                //                productRate.HavaPrice = string.IsNullOrEmpty(prodLoc.havaPrice) ? 0 : Decimal.Parse(prodLoc.havaPrice);
                                //                productRate.HavaPriceReturn = prodLoc.havaPriceReturn;
                                //                productRate.IsMarkUp = prodLoc.isMarkup;
                                //                productRate.LocationId = location.Id;
                                //                productRate.MarketPrice = string.IsNullOrEmpty(prodLoc.marketPrice) ? 0 : Decimal.Parse(prodLoc.marketPrice);
                                //                productRate.MarketPriceReturn = prodLoc.marketPriceReturn;
                                //                productRate.Markup = string.IsNullOrEmpty(prodLoc.partnerMarkup) ? 0 : Decimal.Parse(prodLoc.partnerMarkup);
                                //                productRate.PartnerId = partner.Id;
                                //                productRate.PartnerProductId = objProd.Id;
                                //                productRate.PartnerSellingPrice = string.IsNullOrEmpty(prodLoc.partnerSellingPrice) ? 0 : Decimal.Parse(prodLoc.partnerSellingPrice);
                                //                productRate.PartnerSellingPriceReturn = prodLoc.partnerSellPriceReturn;
                                //                productRate.Percentage = string.IsNullOrEmpty(prodLoc.partnerPercentage) ? 0 : Decimal.Parse(prodLoc.partnerPercentage);
                                //                productRate.Rate = prodLoc.rate;
                                //                productRate.CreateDate = DateTime.Now;

                                //                this.ObjContext.PartnerProductRates.Add(productRate);
                                //                this.ObjContext.SaveChanges();
                                //            }
                                //        }
                                //    }

                                //}
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