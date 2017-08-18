using AutoMapper;
using HavaBusiness;
using HavaBusinessObjects.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace HavaBusinessObjects.ControllerRepository
{
    public class LocationDetailsRepository : IDisposable
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

        public List<LocationDetailViewModel> GetAllByPartnerId(int id)
        {
            var locations = this.ObjContext.LocationDetails
                .Where(a => a.PartnerId == id).ToList();
            
            return Mapper.Map<List<LocationDetail>, List<LocationDetailViewModel>>(locations);
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