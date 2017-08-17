using HavaBusiness;
using System;
using System.Linq;

namespace HavaBusinessObjects
{
    public class Utility : IDisposable
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


        #region
        public Nullable<int> GetUserId(string signature)
        {
            try
            {
                return this.ObjContext.Users.Where(u => u.UserName.ToLower() == signature.ToLower()).FirstOrDefault<User>().Id;
            }
            catch (Exception ex)
            {
                return null;
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