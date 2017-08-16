using HavaBusiness;
using Newtonsoft.Json.Linq;
using System;
using System.Linq;


namespace HavaBusinessObjects.ControllerRepository
{
    public class UserRepository : IDisposable
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

        #region Get User menues
        /// <summary>
        /// Get User menues.
        /// </summary>
        /// <param name="Id">The identifier.</param>
        /// <returns></returns>
        public JObject GetMenues()
        {
            JObject obj = new JObject();
            JArray returnArr = new JArray();
            var menues = (from menus in this.ObjContext.Menues
                          join menuTypes in this.ObjContext.MenuTypes on menus.MenuTypeId equals menuTypes.Id
                          //where cusStore.CustomerId == id
                          select new
                          {
                              Id = menus.Id,
                              name = menus.name,
                              route = menus.route,
                              mainCatogory = menuTypes.Type,
                              mainCatogoryId = menuTypes.Id,
                          }).ToList();


            foreach (var part in menues)
            {

                JObject menuObj = new JObject();
                menuObj.Add("id", part.Id);
                menuObj.Add("name", part.name);
                menuObj.Add("route", part.route);
                menuObj.Add("mainCatogoryId", part.mainCatogoryId);
                menuObj.Add("mainCatogory", part.mainCatogory);

                returnArr.Add(menuObj);
            }
            obj.Add("dataSub", returnArr);

            //main Data
            JObject objM = new JObject();
            JArray returnArrM = new JArray();
            var menueMain = this.ObjContext.MenuTypes;


            foreach (var partM in menueMain)
            {

                JObject menuMObj = new JObject();
                menuMObj.Add("id", partM.Id);
                menuMObj.Add("name", partM.Type);
                menuMObj.Add("icon", partM.icon);

                returnArrM.Add(menuMObj);
            }
            obj.Add("dataMain", returnArrM);
            return obj;
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