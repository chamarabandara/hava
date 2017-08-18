using HavaBusiness;
using HavaBusinessObjects.ViewModels;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

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
                                vehicle.DriverIdOrDLNo = vehcl.driverIDDLNo;
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
                                var recentvehicles = tsp.TSPVehicles.ToList();
                                recentvehicles.ForEach(a => a.IsAcive = false);
                                recentvehicles.ForEach(a => a.ModifiedBy = tspViewModel.createdBy);
                                recentvehicles.ForEach(a => a.ModifiedDate = DateTime.Now);
                                this.ObjContext.SaveChanges();

                                List<TSPVehicle> tspVehicles = new List<TSPVehicle>();


                                foreach (var vehcl in tspViewModel.vehicles)
                                {
                                    TSPVehicle vehicle = new TSPVehicle();
                                    if (vehcl.id > 0)
                                    {
                                        vehicle = this.ObjContext.TSPVehicles.Find(vehcl.id);
                                        vehicle.IsAcive = vehcl.isActive;
                                        vehicle.VehicleNo = vehcl.vehicleNo;
                                        vehicle.RegistrationNo = vehcl.regNo;
                                        vehicle.DriverName = vehcl.driverName;
                                        vehicle.DriverIdOrDLNo = vehcl.driverIDDLNo;
                                        vehicle.MaxPasengers = vehcl.maxPassengers;
                                        vehicle.MaxLuggages = vehcl.maxLuggages;
                                        vehicle.ProductId = vehcl.productId;
                                        this.ObjContext.Entry(vehicle).State = System.Data.Entity.EntityState.Modified;
                                        this.ObjContext.SaveChanges();
                                    }
                                    else
                                    {
                                        vehicle.IsAcive = vehcl.isActive;
                                        vehicle.VehicleNo = vehcl.vehicleNo;
                                        vehicle.RegistrationNo = vehcl.regNo;
                                        vehicle.DriverName = vehcl.driverName;
                                        vehicle.DriverIdOrDLNo = vehcl.driverIDDLNo;
                                        vehicle.MaxPasengers = vehcl.maxPassengers;
                                        vehicle.MaxLuggages = vehcl.maxLuggages;
                                        vehicle.ProductId = vehcl.productId;
                                        vehicle.CreatedBy = tspViewModel.createdBy;
                                        vehicle.CreatedDate = DateTime.Now;
                                        vehicle.ModifiedBy = tspViewModel.createdBy;
                                        vehicle.ModifiedDate = DateTime.Now;
                                        vehicle.TSPId = tspViewModel.id;
                                        tspVehicles.Add(vehicle);
                                    }
                                }
                                if (tspVehicles.Count > 0)
                                    tsp.TSPVehicles = tspVehicles;
                                #endregion

                                #region TSP Products
                                var recentproducts = tsp.TSPProducts.ToList();
                                recentproducts.ForEach(a => a.IsActive = false);
                                recentproducts.ForEach(a => a.ModifiedBy = tspViewModel.createdBy);
                                recentproducts.ForEach(a => a.ModifiedDate = DateTime.Now);
                                this.ObjContext.SaveChanges();
                                List<TSPProduct> tspProducts = new List<TSPProduct>();
                                foreach (var prod in tspViewModel.products)
                                {
                                    TSPProduct product = new TSPProduct();

                                    if (prod.id > 0)
                                    {
                                        product = this.ObjContext.TSPProducts.Find(prod.id);
                                        product.IsActive = prod.isActive;
                                        product.ProductPrice = prod.productPrice;
                                        product.ProductId = prod.productId;
                                        this.ObjContext.Entry(product).State = System.Data.Entity.EntityState.Modified;
                                        this.ObjContext.SaveChanges();
                                    }
                                    else
                                    {
                                        product.IsActive = prod.isActive;
                                        product.ProductPrice = prod.productPrice;
                                        product.ProductId = prod.productId;
                                        product.TspId = tspViewModel.id;
                                        product.CreatedBy = tspViewModel.createdBy;
                                        product.CreatedDate = DateTime.Now;
                                        product.ModifiedBy = tspViewModel.createdBy;
                                        product.ModifiedDate = DateTime.Now;
                                        tspProducts.Add(product);
                                    }
                                }
                                if (tspProducts.Count > 0)
                                    tsp.TSPProducts = tspProducts;
                                #endregion
                                this.ObjContext.Entry(tsp).State = System.Data.Entity.EntityState.Modified;
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

        #region Get all TSP
        /// <summary>
        /// Gets TSP List.
        /// </summary>
        /// <param ></param>
        /// <returns></returns>
        public JObject GetTSP()
        {
            JObject obj = new JObject();
            JArray returnArr = new JArray();
            var tsps = this.ObjContext.TSPs;
            foreach (var tsp in tsps)
            {
                JObject tspObj = new JObject();
                tspObj.Add("id" , tsp.Id);
                tspObj.Add("name" , tsp.Name);
                tspObj.Add("telephone" , tsp.TelLandLine);
                tspObj.Add("address" , tsp.FullAddress);
                tspObj.Add("email" , tsp.Email);
                returnArr.Add(tspObj);
            }
            obj.Add("data" , returnArr);
            return obj;
        }
        #endregion

        #region Get TSP By ID
        /// <summary>
        /// Gets TSP by id.
        /// </summary>
        /// <param ></param>
        /// <returns></returns>
        public JObject GetTSPById(int id)
        {
            JObject obj = new JObject();
            JObject tsp = new JObject();
            JArray tspVehicles = new JArray();
            JArray tspProducts = new JArray();

            var tspObj = this.ObjContext.TSPs.Find(id);
            if (tspObj != null)
            {
                tsp.Add("name" , string.IsNullOrEmpty(tspObj.Name) ? string.Empty : tspObj.Name);
                tsp.Add("email" , string.IsNullOrEmpty(tspObj.Email) ? string.Empty : tspObj.Email);
                tsp.Add("telephoneLand" , string.IsNullOrEmpty(tspObj.TelLandLine) ? string.Empty : tspObj.TelLandLine);
                tsp.Add("telephoneMobile" , string.IsNullOrEmpty(tspObj.TelMobile) ? string.Empty : tspObj.TelMobile);
                tsp.Add("address" , string.IsNullOrEmpty(tspObj.FullAddress) ? string.Empty : tspObj.FullAddress);
                tsp.Add("isActive" , tspObj.IsActive);
                tsp.Add("isCompany" , true);
                tsp.Add("id" , tspObj.Id);
                tsp.Add("rowId" , tspObj.Id);

                #region TSP Vehicles

                foreach (var vehcl in tspObj.TSPVehicles)
                {
                    JObject vehicle = new JObject();
                    vehicle.Add("isActive" , vehcl.IsAcive);
                    vehicle.Add("status" , vehcl.IsAcive == true ? "Active" : "Inactive");
                    vehicle.Add("vehicleNo" , string.IsNullOrEmpty(vehcl.VehicleNo) ? string.Empty : vehcl.VehicleNo);
                    vehicle.Add("regNo" , string.IsNullOrEmpty(vehcl.RegistrationNo) ? string.Empty : vehcl.RegistrationNo);
                    vehicle.Add("driverName" , string.IsNullOrEmpty(vehcl.DriverName) ? string.Empty : vehcl.DriverName);
                    vehicle.Add("driverIDDLNo" , string.IsNullOrEmpty(vehcl.DriverIdOrDLNo) ? string.Empty : vehcl.DriverIdOrDLNo);
                    vehicle.Add("maxPassengers" , vehcl.MaxPasengers == null ? 0 : vehcl.MaxLuggages);
                    vehicle.Add("maxLuggages" , vehcl.MaxLuggages == null ? 0 : vehcl.MaxLuggages);
                    vehicle.Add("productId" , vehcl.ProductId);
                    vehicle.Add("product" , vehcl.ProductId == null ? string.Empty : vehcl.Product.Name);
                    vehicle.Add("id" , vehcl.Id);
                    vehicle.Add("rowId" , vehcl.Id);

                    tspVehicles.Add(vehicle);
                }
                #endregion

                #region TSP Products

                foreach (var prod in tspObj.TSPProducts)
                {
                    JObject product = new JObject();
                    product.Add("isActive" , prod.IsActive);
                    product.Add("status" , prod.IsActive == true ? "Active" : "Inactive");
                    product.Add("productPrice" , prod.ProductPrice == null ? 0 : prod.ProductPrice);
                    product.Add("productId" , prod.ProductId);
                    product.Add("productName" , prod.ProductId == null ? string.Empty : prod.Product.Name);
                    product.Add("id" , prod.Id);

                    tspProducts.Add(product);
                }
                #endregion
            }

            obj.Add("details" , tsp);
            obj.Add("vehiclesGridData" , tspVehicles);
            obj.Add("productGridData" , tspProducts);
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