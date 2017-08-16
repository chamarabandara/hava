using HavaBusiness;
using HavaBusinessObjects.ViewModels;
using Newtonsoft.Json.Linq;
using System;

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
                            if (rep.status == "Active") objRep.IsActive = true; else if (rep.status == "Inactive") objRep.IsActive = false;

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
                            objProd.HavaPrice = Decimal.Parse(prod.havaPrice);
                            objProd.MarketPrice = Decimal.Parse(prod.marketPrice);
                            objProd.PartnerSellingPrice = Decimal.Parse(prod.partnerSellingPrice);
                            objProd.IsMarkUp = prod.isMarkup;
                            objProd.Markup = Decimal.Parse(prod.partnerMarkup);
                            objProd.Percentage = Decimal.Parse(prod.partnerPercentage);
                            objProd.ProductId = prod.productId;

                            this.ObjContext.PartnerProducts.Add(objProd);
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