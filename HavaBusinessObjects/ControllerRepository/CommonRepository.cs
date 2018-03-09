using HavaBusiness;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HavaBusinessObjects.ControllerRepository
{
    public class CommonRepository
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

        public JArray GetAllCardTypes()
        {
            try
            {
                var cardTypes = this.ObjContext.Commons
                     .Where(a => a.Code == "CardType").ToList();
                
                JArray returnArr = new JArray();
                foreach (Common item in cardTypes)
                {
                    JObject bk = new JObject();
                    bk.Add("id", item.Id);
                    bk.Add("name", item.Name);

                    returnArr.Add(bk);
                }
                return returnArr;
                
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public JArray GetAllCountry()
        {
            try
            {
                var countries = this.ObjContext.Countries.ToList();

                JArray returnArr = new JArray();
                foreach (Country item in countries)
                {
                    JObject bk = new JObject();
                    bk.Add("id", item.Id);
                    bk.Add("name", item.Name);

                    returnArr.Add(bk);
                }
                return returnArr;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public JArray GetAllBookingStatus()
        {
            try
            {
                var bookings = this.ObjContext.BookingStatus.ToList();

                JArray returnArr = new JArray();
                foreach (BookingStatu item in bookings)
                {
                    JObject bk = new JObject();
                    bk.Add("id", item.Id);
                    bk.Add("name", item.Name);

                    returnArr.Add(bk);
                }
                return returnArr;

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