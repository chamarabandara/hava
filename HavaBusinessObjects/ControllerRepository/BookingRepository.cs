using AutoMapper;
using HavaBusiness;
using HavaBusinessObjects.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web;

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
                    booking.CreatedBy = 1;
                    booking.CreatedDate = DateTime.UtcNow;
                    booking.ModifiedBy = 1;
                    booking.ModifiedDate = DateTime.UtcNow;

                    this.ObjContext.Bookings.Add(booking);
                    this.ObjContext.SaveChanges();

                    int bookingId = booking.Id;

                    BookingProduct bookingProduct = Mapper.Map<BookingProductsViewModel, BookingProduct>(vm.BookingProduct);
                    BookingOption bookingOption = Mapper.Map<BookingOptionViewModel, BookingOption>(vm.BookingOption);
                    BookingPayment bookingPayment = Mapper.Map<BookingPaymentViewModel, BookingPayment>(vm.BookingPayment);

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
                    return Mapper.Map<Booking, BookingViewModel>(booking);
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
            var booking = this.ObjContext.Bookings
                .Include(x => x.BookingOptions)
                .Include(x => x.BookingProducts)
                .Include(x => x.BookingPayments)
                .Where(a => a.Id == id).FirstOrDefault();


            return Mapper.Map<Booking, BookingViewModel>(booking);
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