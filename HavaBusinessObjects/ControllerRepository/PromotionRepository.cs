using AutoMapper;
using HavaBusiness;
using HavaBusinessObjects.ViewModels;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace HavaBusinessObjects.ControllerRepository
{
    public class PromotionRepository : IDisposable
    {
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

        public PromotionDiscount InsertDiscount(PromotionDiscount model)
        {
            using (var dbContextTransaction = this.ObjContext.Database.BeginTransaction())
            {
                try
                {
                    ObjContext.PromotionDiscounts.Add(model);
                    ObjContext.SaveChanges();

                    dbContextTransaction.Commit();

                    return model;
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                    throw ex;
                }
            }
        }

        public PromotionDiscount UpdateDiscount(PromotionDiscount model)
        {
            using (var dbContextTransaction = this.ObjContext.Database.BeginTransaction())
            {
                try
                {
                    var discount = ObjContext.PromotionDiscounts.Where(a => a.Id == model.Id).FirstOrDefault();
                    discount.AmountOrPercentage = model.AmountOrPercentage;
                    discount.PromoDiscountTypeId = model.PromoDiscountTypeId;

                    ObjContext.Entry(discount).State = System.Data.Entity.EntityState.Modified;
                    ObjContext.SaveChanges();

                    dbContextTransaction.Commit();


                    return (ObjContext.PromotionDiscounts.Where(a => a.Id == model.Id).FirstOrDefault());
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                    throw ex;
                }
            }
        }

        public List<PromotionDiscountViewModel> GetAllDiscount()
        {
            var promotions = ObjContext.PromotionDiscounts
                .Include(a => a.PromotionDiscountType)
                .ToList();

            return Mapper.Map<List<PromotionDiscount>, List<PromotionDiscountViewModel>>(promotions);
        }

        public PromotionDiscount GetDiscountById(int id)
        {
            var discount = ObjContext.PromotionDiscounts
                .Include(a => a.PromotionDiscountType)
                .Where(a => a.Id == id)
                .FirstOrDefault();

            return discount;
        }

        public PromotionViewModel InsertPromotion(PromotionViewModel promotion)
        {
            using (var dbContextTransaction = this.ObjContext.Database.BeginTransaction())
            {
                try
                {
                    var insertPromotion = Mapper.Map<PromotionViewModel, Promotion>(promotion);

                    insertPromotion.PromotionDiscount = null;
                    insertPromotion.Partner = null;

                    ObjContext.Promotions.Add(insertPromotion);
                    ObjContext.SaveChanges();

                    dbContextTransaction.Commit();

                    var addedObj = ObjContext.Promotions
                                    .Include(a => a.Partner)
                                    .Include(a => a.PromotionDiscount)
                                    .Include(a => a.PromotionDiscount.PromotionDiscountType)
                                    .Where(a => a.Id == insertPromotion.Id)
                                    .FirstOrDefault();

                    return Mapper.Map<Promotion, PromotionViewModel>(addedObj);
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                    throw ex;
                }
            }
        }

        public Promotion UpdatePromotion(Promotion promotion)
        {
            using (var dbContextTransaction = this.ObjContext.Database.BeginTransaction())
            {
                try
                {
                    var extPromotion = ObjContext.Promotions.Where(a => a.Id == promotion.Id).FirstOrDefault();
                    extPromotion.Code = promotion.Code;
                    extPromotion.IsAvtive = promotion.IsAvtive;
                    extPromotion.ModifiedDate = DateTime.UtcNow;
                    extPromotion.Name = promotion.Name;
                    extPromotion.PartnerId = promotion.PartnerId;
                    extPromotion.PromotionDiscountId = promotion.PromotionDiscountId; 

                    ObjContext.Entry(extPromotion).State = System.Data.Entity.EntityState.Modified;
                    ObjContext.SaveChanges();

                    dbContextTransaction.Commit();

                    return (ObjContext.Promotions.Where(a => a.Id == promotion.Id).FirstOrDefault());
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                    throw ex;
                }
            }
        }

        public PromotionViewModel GetPromotionById(int id)
        {
            var promotion = ObjContext.Promotions
                .Include(a => a.PromotionDiscount)
                .Include(a => a.PromotionDiscount.PromotionDiscountType)
                .Where(a => a.Id == id)
                .FirstOrDefault();

            return Mapper.Map<Promotion, PromotionViewModel>(promotion);
        }

        public List<PromotionViewModel> GetPromotions()
        {
            var promotions = ObjContext.Promotions
                .Include(a => a.PromotionDiscount)
                .Include(a => a.PromotionDiscount.PromotionDiscountType)
                .ToList();

            return Mapper.Map<List<Promotion>, List<PromotionViewModel>>(promotions);
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