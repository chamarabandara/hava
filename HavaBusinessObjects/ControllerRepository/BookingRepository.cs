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
            Booking booking = Mapper.Map<BookingViewModel , Booking>(vm);

            using (var dbContextTransaction = this.ObjContext.Database.BeginTransaction())
            {
                try
                {
                    booking.CreatedBy = 1;
                    booking.CreatedDate = DateTime.UtcNow;
                    booking.ModifiedBy = 1;
                    booking.ModifiedDate = DateTime.UtcNow;

                    this.ObjContext.Bookings.Add(booking);
                    this.ObjContext.SaveChanges();

                    int bookingId = booking.Id;

                    BookingProduct bookingProduct = Mapper.Map<BookingProductsViewModel , BookingProduct>(vm.BookingProducts.FirstOrDefault());
                    BookingOption bookingOption = Mapper.Map<BookingOptionViewModel , BookingOption>(vm.BookingOptions.FirstOrDefault());
                    BookingPayment bookingPayment = Mapper.Map<BookingPaymentViewModel , BookingPayment>(vm.BookingPayments.FirstOrDefault());

                    bookingOption.CreatedDate = DateTime.UtcNow;
                    bookingOption.BookingId = bookingId;

                    this.ObjContext.BookingOptions.Add(bookingOption);
                    this.ObjContext.SaveChanges();


                    bookingProduct.BookingId = bookingId;

                    this.ObjContext.BookingProducts.Add(bookingProduct);
                    this.ObjContext.SaveChanges();


                    bookingPayment.CreatedDate = DateTime.UtcNow;
                    bookingPayment.BookingId = bookingId;

                    this.ObjContext.BookingPayments.Add(bookingPayment);
                    this.ObjContext.SaveChanges();


                    dbContextTransaction.Commit();
                    return Mapper.Map<Booking , BookingViewModel>(booking);
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
                     .Include(x => x.Partner)
                     .Include(x => x.BookingStatu)
                     .Include(x => x.BookingType)
                     .Include(x => x.BookingType)
                     .Include(x => x.BookingOptions)
                     .Include(x => x.BookingProducts)
                     .Include(x => x.BookingPayments)
                     .Where(a => a.Id == id).FirstOrDefault();

                return Mapper.Map<Booking , BookingViewModel>(booking);
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
                    bk.Add("id" , booking.Id);
                    bk.Add("refNo" , booking.RefNo);
                    bk.Add("partner" , booking.PartnerId != null ? booking.Partner.Name : string.Empty);
                    bk.Add("bookingType" , booking.BookingTypeId != null ? booking.BookingType.type : string.Empty);
                    bk.Add("pickupDate" , booking.PickupDate != null ? booking.PickupDate.Value.ToString("yyyy-MMM-dd") : string.Empty);
                    bk.Add("pickupTime" , booking.PickupTime != null ? booking.PickupTime.Value.ToString(@"hh\:mm") : string.Empty);
                    bk.Add("pickupLocation" , booking.PickupLocation);
                    bk.Add("returnDate" , booking.ReturnDate != null ? booking.ReturnDate.Value.ToString("yyyy-MMM-dd") : string.Empty);
                    bk.Add("returnTime" , booking.ReturnTime != null ? booking.ReturnTime.Value.ToString(@"hh\:mm") : string.Empty);
                    bk.Add("dropLocation" , booking.DropLocation);
                    bk.Add("bookingStatus" , booking.BookingStatusId != null ? booking.BookingStatu.Name : string.Empty);

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