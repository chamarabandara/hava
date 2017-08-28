using HavaBusiness;
using HavaBusinessObjects.ViewModels;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

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

        //public decimal GetByCode(string promotionCode)
        //{
        //    try
        //    {
        //        var booking = this.ObjContext.Promotions
        //             .Include(x => x.)
        //             .Where(a => a.Id == id).FirstOrDefault();

        //        return Mapper.Map<Booking, BookingViewModel>(booking);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}



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