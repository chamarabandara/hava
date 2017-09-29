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
    public class BookingRepository : IDisposable
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

        public BookingViewModel Insert(BookingViewModel vm)
        {
            Booking booking = Mapper.Map<BookingViewModel, Booking>(vm);

            using (var dbContextTransaction = this.ObjContext.Database.BeginTransaction())
            {
                try
                {
                    booking.CreatedBy = vm.UserId;
                    booking.CreatedDate = DateTime.UtcNow;
                    booking.ModifiedBy = vm.UserId;
                    booking.ModifiedDate = DateTime.UtcNow;

                    booking.BookingStatu = null;
                    booking.BookingType = null;
                    booking.Partner = null;

                    this.ObjContext.Bookings.Add(booking);
                    this.ObjContext.SaveChanges();

                    int bookingId = booking.Id;

                    if (vm.BookingProducts != null && vm.BookingProducts.Count() > 0)
                    {
                        BookingProduct bookingProduct = Mapper.Map<BookingProductsViewModel, BookingProduct>(vm.BookingProducts.FirstOrDefault());
                        bookingProduct.BookingId = bookingId;

                        this.ObjContext.BookingProducts.Add(bookingProduct);
                        this.ObjContext.SaveChanges();
                    }

                    if (vm.BookingOptions != null && vm.BookingOptions.Count() > 0)
                    {
                        BookingOption bookingOption = Mapper.Map<BookingOptionViewModel, BookingOption>(vm.BookingOptions.FirstOrDefault());

                        bookingOption.CreatedDate = DateTime.UtcNow;
                        bookingOption.BookingId = bookingId;

                        this.ObjContext.BookingOptions.Add(bookingOption);
                        this.ObjContext.SaveChanges();
                    }

                    if (vm.BookingPayments != null && vm.BookingPayments.Count() > 0)
                    {
                        BookingPayment bookingPayment = Mapper.Map<BookingPaymentViewModel, BookingPayment>(vm.BookingPayments.FirstOrDefault());

                        bookingPayment.CreatedDate = DateTime.UtcNow;
                        bookingPayment.BookingId = bookingId;

                        this.ObjContext.BookingPayments.Add(bookingPayment);
                        this.ObjContext.SaveChanges();
                    }

                    dbContextTransaction.Commit();

                    var newBooking = this.ObjContext.Bookings
                     .Include(x => x.Partner)
                     .Include(x => x.BookingStatu)
                     .Include(x => x.BookingType)
                     .Include(x => x.BookingType)
                     .Include(x => x.BookingOptions)
                     .Include(x => x.BookingOptions.Select(y => y.Country))
                     .Include(x => x.BookingProducts.Select(y => y.Product))
                     .Include(x => x.BookingPayments)
                     .Where(a => a.Id == booking.Id).FirstOrDefault();

                    return Mapper.Map<Booking, BookingViewModel>(newBooking);
                }
                catch (Exception ex)
                {
                    dbContextTransaction.Rollback();
                    throw ex;
                }
            }
        }

        public BookingViewModel GetById(int id)
        {
            try
            {
                var booking = this.ObjContext.Bookings
                     .Include(x => x.LocationDetail)
                     .Include(x => x.Partner)
                     .Include(x => x.BookingStatu)
                     .Include(x => x.BookingType)
                     .Include(x => x.BookingType)
                     .Include(x => x.BookingOptions)
                     .Include(x => x.BookingProducts.Select(y => y.Product))
                     .Include(x => x.BookingPayments)
                     .Where(a => a.Id == id).FirstOrDefault();

                return Mapper.Map<Booking, BookingViewModel>(booking);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public Promotion GetPromotionCode(string promotionCode, int partnerId)
        {
            try
            {
                var promotion = this.ObjContext.Promotions
                     .Include(x => x.PromotionDiscount)
                     .Where(a => a.Code == promotionCode && a.PartnerId == partnerId && a.IsAvtive == true).FirstOrDefault();

                return promotion;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public JArray GetBookingList()
        {
            try
            {
                List<Booking> bookings = this.ObjContext.Bookings.ToList();
                bookings = bookings.OrderByDescending(x => x.CreatedDate).ToList();
                JArray returnArr = new JArray();
                foreach (Booking booking in bookings)
                {
                    JObject bk = new JObject();
                    bk.Add("id", booking.Id);
                    bk.Add("refNo", booking.RefNo);
                    bk.Add("partner", booking.PartnerId != null ? booking.Partner.Name : string.Empty);
                    bk.Add("bookingType", booking.BookingTypeId != null ? booking.BookingType.type : string.Empty);
                    bk.Add("pickupDate", booking.PickupDate != null ? booking.PickupDate.Value.ToString("yyyy-MMM-dd") : string.Empty);
                    bk.Add("pickupTime", booking.PickupTime != null ? booking.PickupTime.Value.ToString(@"hh\:mm") : string.Empty);
                    bk.Add("pickupLocation", booking.PickupLocation);
                    bk.Add("returnDate", booking.ReturnDate != null ? booking.ReturnDate.Value.ToString("yyyy-MMM-dd") : string.Empty);
                    bk.Add("returnTime", booking.ReturnTime != null ? booking.ReturnTime.Value.ToString(@"hh\:mm") : string.Empty);
                    bk.Add("dropLocation", booking.DropLocation);
                    bk.Add("bookingStatus", booking.BookingStatusId != null ? booking.BookingStatu.Name : string.Empty);

                    returnArr.Add(bk);
                }
                return returnArr;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public JArray GetUsersBookingHistory(int userId)
        {
            try
            {
                List<Booking> bookings = this.ObjContext.Bookings.Where(a=>a.UserId == userId).ToList();
                bookings = bookings.OrderByDescending(x => x.CreatedDate).ToList();
                JArray returnArr = new JArray();
                foreach (Booking booking in bookings)
                {
                    JObject bk = new JObject();
                    bk.Add("id", booking.Id);
                    bk.Add("refNo", booking.RefNo);
                    bk.Add("partner", booking.PartnerId != null ? booking.Partner.Name : string.Empty);
                    bk.Add("bookingType", booking.BookingTypeId != null ? booking.BookingType.type : string.Empty);
                    bk.Add("pickupDate", booking.PickupDate != null ? booking.PickupDate.Value.ToString("yyyy-MMM-dd") : string.Empty);
                    bk.Add("pickupTime", booking.PickupTime != null ? booking.PickupTime.Value.ToString(@"hh\:mm") : string.Empty);
                    bk.Add("pickupLocation", booking.PickupLocation);
                    bk.Add("returnDate", booking.ReturnDate != null ? booking.ReturnDate.Value.ToString("yyyy-MMM-dd") : string.Empty);
                    bk.Add("returnTime", booking.ReturnTime != null ? booking.ReturnTime.Value.ToString(@"hh\:mm") : string.Empty);
                    bk.Add("dropLocation", booking.DropLocation);
                    bk.Add("bookingStatus", booking.BookingStatusId != null ? booking.BookingStatu.Name : string.Empty);

                    returnArr.Add(bk);
                }
                return returnArr;

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public JArray BookingStatus()
        {
            try
            {
                List<BookingStatu> status = this.ObjContext.BookingStatus.ToList();
                JArray returnArr = new JArray();
                foreach (BookingStatu item in status.OrderBy(a=>a.Name))
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

        public BookingViewModel Update(BookingViewModel vm)
        {
            //Booking booking = Mapper.Map<BookingViewModel, Booking>(vm);

            using (var dbContextTransaction = this.ObjContext.Database.BeginTransaction())
            {
                try
                {
                    Booking booking = this.ObjContext.Bookings.Where(a => a.Id == vm.Id).FirstOrDefault();
                    booking.BookingTypeId       = vm.BookingType.Id;
                    booking.PickupDate          = vm.PickupDate;
                    booking.PickupTime          = vm.PickupTime;
                    booking.PickupLocation      = vm.PickupLocation;
                    booking.ReturnTime          = vm.ReturnTime;
                    booking.ReturnPickupLocation= vm.ReturnPickupLocation;
                    booking.DropLocation        = vm.DropLocation.Id;
                    booking.ReturnDate          = vm.ReturnDate;
                    booking.RefNo               = vm.RefNo;
                    booking.ModifiedBy          = 1;
                    booking.ModifiedDate        = DateTime.UtcNow;
                    booking.PartnerId           = vm.Partner.id;
                    booking.NumberOfDays        = vm.NumberOfDays;
                    booking.BookingStatusId     = vm.BookingStatu.Id;
                    booking.TSPId               = vm.TSPId;
                    booking.PaymentTypeId       = vm.PaymentTypeId;
                    booking.PromotionId         = vm.PromotionId;
                    booking.HasPromotions       = vm.HasPromotions;
                    booking.UserConfirmed       = vm.UserConfirmed;
                    booking.UserId              = vm.UserId;
                    booking.IsReturn            = vm.IsReturn;
                    booking.IsAirportTransfer   = vm.IsAirportTransfer;

                    this.ObjContext.Entry(booking).State = EntityState.Modified;
                    this.ObjContext.SaveChanges();
                    
                   
                   

                    if (vm.BookingPayments != null && vm.BookingPayments.Count() > 0)
                    {
                        BookingProduct bookingProduct = Mapper.Map<BookingProductsViewModel, BookingProduct>(vm.BookingProducts.FirstOrDefault());

                        if (bookingProduct.Id > 0)
                        {
                            BookingProduct extBookingProduct = this.ObjContext.BookingProducts.Where(a => a.Id == bookingProduct.Id).FirstOrDefault();
                            extBookingProduct.BookingId = bookingProduct.BookingId;
                            extBookingProduct.ProductId = bookingProduct.ProductId;
                            extBookingProduct.Price = bookingProduct.Price;
                            extBookingProduct.IsAirPortTour = bookingProduct.IsAirPortTour;
                            extBookingProduct.AdditionalDays = bookingProduct.AdditionalDays;
                            extBookingProduct.AdditionalHours = bookingProduct.AdditionalHours;
                            extBookingProduct.AdditionalChufferDate = bookingProduct.AdditionalChufferDate;
                            extBookingProduct.AdditionalChufferHours = bookingProduct.AdditionalChufferHours;
                            extBookingProduct.NoOfChildSeats = bookingProduct.NoOfChildSeats;
                            extBookingProduct.ChildSeatDays = bookingProduct.ChildSeatDays;

                            this.ObjContext.Entry(extBookingProduct).State = EntityState.Modified;
                            this.ObjContext.SaveChanges();
                        }
                        else
                        {
                            bookingProduct.BookingId = booking.Id;

                            this.ObjContext.BookingProducts.Add(bookingProduct);
                            this.ObjContext.SaveChanges();
                        }
                    }

                    if (vm.BookingOptions != null && vm.BookingOptions.Count() > 0)
                    {
                        BookingOption bookingOption = Mapper.Map<BookingOptionViewModel, BookingOption>(vm.BookingOptions.FirstOrDefault());


                        if (bookingOption.Id > 0)
                        {
                            BookingOption extBookingOption = this.ObjContext.BookingOptions.Where(a => a.Id == bookingOption.Id).FirstOrDefault();
                            extBookingOption.BookingId = bookingOption.BookingId;
                            extBookingOption.FlightNo = bookingOption.FlightNo;
                            extBookingOption.FlyerProgramId = bookingOption.FlyerProgramId;
                            extBookingOption.FlyerReffNo = bookingOption.FlyerReffNo;
                            extBookingOption.PickupSign = bookingOption.PickupSign;
                            extBookingOption.PickupSignReffNo = bookingOption.PickupSignReffNo;
                            extBookingOption.NoteToDriver = bookingOption.NoteToDriver;

                            this.ObjContext.Entry(extBookingOption).State = EntityState.Modified;
                            this.ObjContext.SaveChanges();
                        }
                        else
                        {
                            bookingOption.CreatedDate = DateTime.UtcNow;
                            bookingOption.BookingId = booking.Id;

                            this.ObjContext.BookingOptions.Add(bookingOption);
                            this.ObjContext.SaveChanges();
                        }
                    }

                    if (vm.BookingPayments != null && vm.BookingPayments.Count() > 0)
                    {
                        BookingPayment bookingPayment = Mapper.Map<BookingPaymentViewModel, BookingPayment>(vm.BookingPayments.FirstOrDefault());

                        if (bookingPayment.Id > 0)
                        {
                            BookingPayment extBookingPayment = this.ObjContext.BookingPayments.Where(a => a.Id == bookingPayment.Id).FirstOrDefault();
                            extBookingPayment.BookingId = bookingPayment.BookingId;
                            extBookingPayment.CardHolderName = bookingPayment.CardHolderName;
                            extBookingPayment.ExpireDate = bookingPayment.ExpireDate;
                            extBookingPayment.CardNo = bookingPayment.CardNo;
                            extBookingPayment.CardType = bookingPayment.CardType;

                            this.ObjContext.Entry(extBookingPayment).State = EntityState.Modified;
                            this.ObjContext.SaveChanges();
                        }
                        else
                        {
                            bookingPayment.CreatedDate = DateTime.UtcNow;
                            bookingPayment.BookingId = booking.Id;

                            this.ObjContext.BookingPayments.Add(bookingPayment);
                            this.ObjContext.SaveChanges();
                        }
                    }
                                                        
                    dbContextTransaction.Commit();

                    var updatedBooking = this.ObjContext.Bookings
                     .Include(x => x.Partner)
                     .Include(x => x.BookingStatu)
                     .Include(x => x.BookingType)
                     .Include(x => x.BookingType)
                     .Include(x => x.BookingOptions)
                     .Include(x => x.BookingProducts)
                     .Include(x => x.BookingPayments)
                     .Where(a => a.Id == vm.Id).FirstOrDefault();

                    return Mapper.Map<Booking, BookingViewModel>(updatedBooking);
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