using HavaBusiness;
using HavaBusinessObjects.ViewModels;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace HavaBusinessObjects.ControllerRepository
{
    public class TSPRepository : IDisposable
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

        #region Get Products
        public JObject GetProducts()
        {
            try
            {
                JObject obj = new JObject();
                JArray masterProducts = new JArray();
                JArray nonMasterProducts = new JArray();
                var product = this.ObjContext.Products;
                foreach (var prd in product)
                {
                    JObject productObj = new JObject();
                    if (prd.IsMainProduct == true)
                    {
                        productObj.Add("id" , prd.Id);
                        productObj.Add("name" , prd.Name);
                        productObj.Add("code" , prd.Code);
                        productObj.Add("imagePath" , prd.ProductImagePath);
                        masterProducts.Add(productObj);
                    }
                    else
                    {

                        productObj.Add("id" , prd.Id);
                        productObj.Add("name" , prd.Name);
                        productObj.Add("code" , prd.Code);
                        productObj.Add("imagePath" , prd.ProductImagePath);
                        nonMasterProducts.Add(productObj);
                    }

                }
                obj.Add("masterProducts" , masterProducts);
                obj.Add("nonMasterProducts" , nonMasterProducts);

                return obj;
            }
            catch (Exception ex)
            {

                throw ex;
            }
        }
        #endregion

        #region Add TSP
        /// <summary>
        /// Save TSP to DB
        /// </summary>
        /// </summary>
        /// Date           Author/(Reviewer)        Description
        /// ------------------------------------------------------------------------------------          
        /// 13-Aug-2017    VISH                 Created.
        /// 
        public int SaveTSP(TSPDetailViewModel tspViewModel)
        {
            try
            {
                if (tspViewModel == null && tspViewModel.vehicles.Count <= 0)
                {
                    throw new ArgumentNullException("item");
                }
                else
                {
                    TSP tsp = new TSP();
                    using (var dbContextTransaction = this.ObjContext.Database.BeginTransaction())
                    {
                        try
                        {
                            tsp.Name = tspViewModel.name;
                            tsp.Email = tspViewModel.email;
                            tsp.TelLandLine = tspViewModel.telephoneLand;
                            tsp.TelMobile = tspViewModel.telephoneMobile;
                            tsp.FullAddress = tspViewModel.address;
                            tsp.IsActive = tspViewModel.isActive;
                            tsp.IsCompany = true;
                            tsp.CreatedBy = tspViewModel.createdBy;
                            tsp.CreatedDate = DateTime.Now;
                            tsp.ModifiedBy = tspViewModel.createdBy;
                            tsp.ModifiedDate = DateTime.Now;

                            #region TSP Vehicles
                            List<TSPVehicle> tspVehicles = new List<TSPVehicle>();
                            foreach (var vehcl in tspViewModel.vehicles)
                            {
                                TSPVehicle vehicle = new TSPVehicle();
                                vehicle.IsAcive = vehcl.isActive;
                                vehicle.VehicleNo = vehcl.vehicleNo;
                                vehicle.RegistrationNo = vehcl.regNo;
                                vehicle.DriverName = vehcl.driverName;
                                vehicle.DriverIdOrDLNo = vehcl.driverName;
                                vehicle.MaxPasengers = vehcl.maxPassengers;
                                vehicle.MaxLuggages = vehcl.maxLuggages;
                                vehicle.ProductId = vehcl.productId;
                                vehicle.CreatedBy = tspViewModel.createdBy;
                                vehicle.ModifiedBy = tspViewModel.createdBy;
                                vehicle.CreatedDate = DateTime.Now;
                                vehicle.ModifiedDate = DateTime.Now;

                                tspVehicles.Add(vehicle);
                            }
                            tsp.TSPVehicles = tspVehicles;
                            #endregion

                            #region TSP Products
                            List<TSPProduct> tspProducts = new List<TSPProduct>();
                            foreach (var prod in tspViewModel.products)
                            {
                                TSPProduct product = new TSPProduct();
                                product.IsActive = prod.isActive;
                                product.ProductPrice = prod.productPrice;
                                product.ProductId = prod.productId;
                                product.CreatedBy = tspViewModel.createdBy;
                                product.ModifiedBy = tspViewModel.createdBy;
                                product.CreatedDate = DateTime.Now;
                                product.ModifiedDate = DateTime.Now;

                                tspProducts.Add(product);
                            }
                            tsp.TSPProducts = tspProducts;
                            #endregion

                            this.ObjContext.TSPs.Add(tsp);
                            this.ObjContext.SaveChanges();
                            dbContextTransaction.Commit();
                            return tsp.Id;
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

        #region Update TSP
        /// <summary>
        /// Update TSP
        /// </summary>
        /// </summary>
        /// Date           Author/(Reviewer)        Description
        /// ------------------------------------------------------------------------------------          
        /// 13-Aug-2017    VISH                 Created.
        /// 
        public bool UpdateTSP(TSPDetailViewModel tspViewModel)
        {
            try
            {
                if (tspViewModel == null && tspViewModel.vehicles.Count <= 0)
                {
                    throw new ArgumentNullException("item");
                }
                else
                {
                    TSP tsp = new TSP();
                    using (var dbContextTransaction = this.ObjContext.Database.BeginTransaction())
                    {
                        try
                        {
                            tsp = this.ObjContext.TSPs.Find(tspViewModel.id);
                            if (tsp != null)
                            {
                                tsp.Name = tspViewModel.name;
                                tsp.Email = tspViewModel.email;
                                tsp.TelLandLine = tspViewModel.telephoneLand;
                                tsp.TelMobile = tspViewModel.telephoneMobile;
                                tsp.FullAddress = tspViewModel.address;
                                tsp.IsActive = tspViewModel.isActive;
                                tsp.IsCompany = true;
                                tsp.ModifiedBy = tspViewModel.createdBy;
                                tsp.ModifiedDate = DateTime.Now;

                                #region TSP Vehicles
                                List<TSPVehicle> tspVehicles = new List<TSPVehicle>();
                                foreach (var vehcl in tspViewModel.vehicles)
                                {
                                    TSPVehicle vehicle = new TSPVehicle();
                                    vehicle = this.ObjContext.TSPVehicles.Find(vehcl.id);
                                    vehicle.IsAcive = vehcl.isActive;
                                    vehicle.VehicleNo = vehcl.vehicleNo;
                                    vehicle.RegistrationNo = vehcl.regNo;
                                    vehicle.DriverName = vehcl.driverName;
                                    vehicle.DriverIdOrDLNo = vehcl.driverName;
                                    vehicle.MaxPasengers = vehcl.maxPassengers;
                                    vehicle.MaxLuggages = vehcl.maxLuggages;
                                    vehicle.ProductId = vehcl.productId;
                                    vehicle.ModifiedBy = tspViewModel.createdBy;
                                    vehicle.ModifiedDate = DateTime.Now;
                                    vehicle.TSPId = tsp.Id;

                                    if (vehcl.id > 0)
                                    {
                                        this.ObjContext.Entry(vehicle).State = System.Data.Entity.EntityState.Modified;
                                        this.ObjContext.SaveChanges();
                                    }
                                    else
                                    {
                                        tspVehicles.Add(vehicle);
                                    }
                                }
                                if (tspVehicles.Count > 0)
                                    tsp.TSPVehicles = tspVehicles;
                                #endregion

                                #region TSP Products
                                List<TSPProduct> tspProducts = new List<TSPProduct>();
                                foreach (var prod in tspViewModel.products)
                                {
                                    TSPProduct product = new TSPProduct();
                                    product.IsActive = prod.isActive;
                                    product.ProductPrice = prod.productPrice;
                                    product.ProductId = prod.productId;
                                    product.ModifiedBy = tspViewModel.createdBy;
                                    product.ModifiedDate = DateTime.Now;
                                    if (prod.id > 0)
                                    {
                                        this.ObjContext.Entry(product).State = System.Data.Entity.EntityState.Modified;
                                        this.ObjContext.SaveChanges();
                                    }
                                    else
                                    {
                                        tspProducts.Add(product);
                                    }
                                }
                                if (tspProducts.Count > 0)
                                    tsp.TSPProducts = tspProducts;
                                #endregion

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

        #region Delete TSP
        /// <summary>
        /// Delete TSP
        /// </summary>
        /// </summary>
        /// Date           Author/(Reviewer)        Description
        /// ------------------------------------------------------------------------------------          
        /// 13-Aug-2017    VISH                 Created.
        /// 
        public bool DeleteTSP(int id)
        {
            bool isSuccess = false;

            using (var dbContextTransaction = this.ObjContext.Database.BeginTransaction())
            {
                try
                {
                    JObject returnObj = new JObject();

                    var objTSP = this.ObjContext.TSPs.Find(id);
                    objTSP.IsActive = false;
                    objTSP.ModifiedBy = 1;
                    objTSP.ModifiedDate = DateTime.Now;
                    foreach (var prod in objTSP.TSPProducts)
                    {
                        prod.IsActive = false;
                        prod.ModifiedBy = 1;
                        prod.ModifiedDate = DateTime.Now;
                    }
                    foreach (var vehcl in objTSP.TSPVehicles)
                    {
                        vehcl.IsAcive = false;
                        vehcl.ModifiedBy = 1;
                        vehcl.ModifiedDate = DateTime.Now;
                    }
                    this.ObjContext.Entry(objTSP).State = System.Data.Entity.EntityState.Modified;
                    this.ObjContext.SaveChanges();


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