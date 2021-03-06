﻿using AutoMapper;
using HavaBusiness;
using HavaBusinessObjects.Utilities;
using HavaBusinessObjects.ViewModels;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Net.Mail;
using System.Web.UI.WebControls;

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
            HavaBusinessObjects.Utility _utility = new HavaBusinessObjects.Utility();

            Booking booking = Mapper.Map<BookingViewModel, Booking>(vm);

            using (var dbContextTransaction = this.ObjContext.Database.BeginTransaction())
            {
                try
                {
                    var partner = this.ObjContext.Partners.Where(a => a.Id == vm.Partner.id).FirstOrDefault();
                   string prefix = partner.Name.Substring(0, 3);
                    var lastBook = this.ObjContext.Bookings.Where(a => a.PartnerId == vm.Partner.id).ToList();


                    booking.BookingNo = prefix + "-" + (lastBook.Count() + 1).ToString();
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

                    //if (vm.BookingProducts != null && vm.BookingProducts.Count() > 0)
                    //{
                    //    BookingProduct bookingProduct = Mapper.Map<BookingProductsViewModel, BookingProduct>(vm.BookingProducts.FirstOrDefault());
                    //    bookingProduct.BookingId = bookingId;

                    //    this.ObjContext.BookingProducts.Add(bookingProduct);
                    //    this.ObjContext.SaveChanges();
                    //}

                    //if (vm.BookingOptions != null && vm.BookingOptions.Count() > 0)
                    //{
                    //    BookingOption bookingOption = Mapper.Map<BookingOptionViewModel, BookingOption>(vm.BookingOptions.FirstOrDefault());

                    //    bookingOption.CreatedDate = DateTime.UtcNow;
                    //    bookingOption.BookingId = bookingId;

                    //    this.ObjContext.BookingOptions.Add(bookingOption);
                    //    this.ObjContext.SaveChanges();
                    //}

                    //if (vm.BookingPayments != null && vm.BookingPayments.Count() > 0)
                    //{
                    //    BookingPayment bookingPayment = Mapper.Map<BookingPaymentViewModel, BookingPayment>(vm.BookingPayments.FirstOrDefault());

                    //    bookingPayment.CreatedDate = DateTime.UtcNow;
                    //    bookingPayment.BookingId = bookingId;

                    //    this.ObjContext.BookingPayments.Add(bookingPayment);
                    //    this.ObjContext.SaveChanges();
                    //}

                    if (vm.BookingPassenger != null && vm.BookingPassenger.Count() > 0)
                    {
                        List<BookingPassenger> bookingPassengers = Mapper.Map<List<BookingPassengerViewModel>, List<BookingPassenger>>(vm.BookingPassenger.ToList());

                        bookingPassengers.ForEach(a => a.BookingId = bookingId);

                        this.ObjContext.BookingPassengers.AddRange(bookingPassengers);
                        this.ObjContext.SaveChanges();
                    }

                    dbContextTransaction.Commit();

                    //var newBooking = this.ObjContext.Bookings
                    // .Include(x => x.Partner)
                    // .Include(x => x.BookingStatu)
                    // .Include(x => x.BookingType)
                    // .Include(x => x.BookingSubProducts.Select(y => y.Product))
                    // .Include(x => x.BookingOptions)
                    // .Include(x => x.BookingProducts.Select(y => y.Product))
                    // .Include(x => x.BookingPayments)
                    // .Include(x => x.BookingPassengers)
                    // .Where(a => a.Id == booking.Id).FirstOrDefault();


                    //string htmlBody = this.BookingConfirmation(newBooking);
                    //Utility uty = new Utility();
                    //string[] ccMails = new string[1];
                    //ccMails[0] = "udayangana@alliontechnologies.com";

                    //uty.SendMails("udayangana1986@hotmail.com", ccMails, htmlBody, "Booking Confirmation");

                    var newBooking = this.ObjContext.Bookings
                     .Include(x => x.Partner)
                     .Include(x => x.LocationDetail)
                     .Include(x => x.BookingStatu)
                     .Include(x => x.BookingType)
                     .Include(x => x.BookingSubProducts.Select(a => a.Product))
                     .Include(x => x.BookingOptions)
                     .Include(x => x.BookingProducts.Select(a => a.Product))
                     .Include(x => x.BookingPayments)
                     .Include(x => x.BookingPassengers)
                     .Where(a => a.Id == booking.Id).FirstOrDefault();

                    var msgBody = this.BookingConfirmation(newBooking);

                    string[] tomail = new string[1];
                    tomail[0] = newBooking.BookingPassengers.FirstOrDefault().Email;

                    string[] ccmail = new string[1];

                    bool isSend = _utility.SendMails(newBooking.BookingPassengers.FirstOrDefault().Email, ccmail,msgBody, "Booking Confirmation - " + newBooking.BookingNo);


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
            try
            {
                var booking = this.ObjContext.Bookings
                     .Include(x => x.LocationDetail)
                     .Include(x => x.Partner)
                     .Include(x => x.BookingStatu)
                     .Include(x => x.BookingType)
                     .Include(x => x.BookingType)
                     .Include(x => x.BookingOptions)
                     .Include(x => x.BookingPayments.Select(a => a.Common))
                     .Include(x => x.BookingPassengers)
                     .Include(x => x.BookingProducts)
                     .Include(x => x.BookingSubProducts)
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
                foreach (Booking booking in bookings.OrderByDescending(a => a.CreatedDate))
                {
                    JObject bk = new JObject();
                    bk.Add("id", booking.Id);
                    bk.Add("refNo", booking.BookingNo);
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
                List<Booking> bookings = this.ObjContext.Bookings.Where(a => a.UserId == userId).ToList();
                bookings = bookings.OrderByDescending(x => x.CreatedDate).ToList();
                JArray returnArr = new JArray();
                foreach (Booking booking in bookings)
                {
                    JObject bk = new JObject();
                    bk.Add("id", booking.Id);
                    bk.Add("refNo", booking.BookingNo);
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
                foreach (BookingStatu item in status.OrderBy(a => a.Name))
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
                    booking.BookingTypeId = vm.BookingType.Id;
                    booking.PickupDate = vm.PickupDate;
                    booking.PickupTime = vm.PickupTime;
                    booking.PickupLocation = vm.PickupLocation;
                    booking.ReturnTime = vm.ReturnTime;
                    booking.ReturnPickupLocation = vm.ReturnPickupLocation;
                    booking.DropLocation = vm.DropLocation.Id;
                    booking.ReturnDate = vm.ReturnDate;
                    booking.RefNo = vm.RefNo;
                    booking.ModifiedBy = 1;
                    booking.ModifiedDate = DateTime.UtcNow;
                    booking.PartnerId = vm.Partner.id;
                    booking.NumberOfDays = vm.NumberOfDays;
                    booking.BookingStatusId = vm.BookingStatu.Id;
                    booking.TSPId = vm.TSPId;
                    booking.PaymentTypeId = vm.PaymentTypeId;
                    booking.PromotionId = vm.PromotionId;
                    booking.HasPromotions = vm.HasPromotions;
                    booking.UserConfirmed = vm.UserConfirmed;
                    booking.UserId = vm.UserId;
                    booking.IsReturn = vm.IsReturn;
                    booking.IsAirportTransfer = vm.IsAirportTransfer;

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

        public string BookingConfirmation(Booking booking)
        {
            string reportType = Constants.Booking_Confirmation.ToString();
            var reportTemplate = this.ObjContext.ReportTemplates.Where(a => a.Code == reportType).FirstOrDefault();

            ListDictionary replacements = new ListDictionary();

            // Delivery Address - Start
            replacements.Add("{Title}", "Booking Confirmation " + DateTime.UtcNow.ToString("dd/MM/yyyy"));
            if (booking.IsChaffeur)
            {
                replacements.Add("{subTitle}", "Your Journey Start from " + booking.PickupDate.Value.ToString("dd/MM/yyyy") + ". to  " + booking.ReturnDate.Value.ToString("dd/MM/yyyy"));
            }
            else
            {
                replacements.Add("{subTitle}", "Your Journey Start from " + booking.PickupLocation + ". to " + booking.LocationDetail.ToLocation);
            }
            replacements.Add("{bookingNo}", booking.BookingNo);
            replacements.Add("{totalAmount}", booking.TotalCost.ToString("#,##0.00"));
            replacements.Add("{bookingRoute}", booking.PickupLocation + " - " + booking.LocationDetail.ToLocation);

            replacements.Add("{bookingDate}", (booking.CreatedDate.Value.ToString("dd/MM/yyyy")) + " - " + booking.PickupTime.Value.ToString());
            replacements.Add("{pickupAddress}", booking.PickupLocation);
            replacements.Add("{mainProduct}", booking.BookingProducts.FirstOrDefault().Product.Name);
            replacements.Add("{mainProductPrice}", booking.BookingProducts.FirstOrDefault().Price.Value.ToString("#,##0.00"));

            string information = "";

            foreach (var orderRow in booking.BookingSubProducts.Where(a => a.Quantity > 0))
            {
                information += "<tr>";

                information += "<td bgcolor='#eeeeee' style='padding: 2px 5px 0 5px; background-color: #eeeeee; font-size: 12px;'>" + orderRow.Product.Name + "</td>";
                information += "<td bgcolor='#eeeeee' style='padding: 2px 5px 0 5px; background-color: #eeeeee; font-size: 12px;'>" + orderRow.MarketPrice.Value.ToString("#,##0.00") + "</td>";
                information += "<td bgcolor='#eeeeee' style='padding: 2px 5px 0 5px; background-color: #eeeeee; font-size: 12px;'>" + orderRow.Quantity.Value.ToString() + "</td>";
                information += "<td bgcolor='#eeeeee' style='padding: 2px 5px 0 5px; background-color: #eeeeee; font-size: 12px; white-space: nowrap;'>" +  (orderRow.MarketPrice.Value * orderRow.Quantity.Value).ToString("#,##0.00") + "</td>";

                information += "</tr>";
            }

            replacements.Add("{information}", information);

            MailDefinition md = new MailDefinition();
            md.From = "test@domain.com";
            MailMessage msg = md.CreateMailMessage("test@domain.com", replacements, reportTemplate.HTMLBody, new System.Web.UI.Control());

            return msg.Body;
        }

        public List<BookingViewModel> ExportBooking(int? partnerId, int? bookingStatus)
        {
            try
            {
                if (partnerId == 0)
                {
                    partnerId = null;
                }

                if (bookingStatus == 0)
                {
                    bookingStatus = null;
                }

                var booking = this.ObjContext.Bookings
                     .Include(x => x.LocationDetail)
                     .Include(x => x.Partner)
                     .Include(x => x.BookingStatu)
                     .Include(x => x.BookingType)
                     .Include(x => x.BookingType)
                     .Include(x => x.BookingOptions)
                     .Include(x => x.BookingPayments.Select(a => a.Common))
                     .Include(x => x.BookingPassengers)
                     .Include(x => x.BookingProducts)
                     .Include(x => x.BookingSubProducts)
                     .Where(a => //(EntityFunctions.TruncateTime(a.PickupDate) >= EntityFunctions.TruncateTime(filters.FromDate)) 
                     //&& (EntityFunctions.TruncateTime(a.PickupDate) <= EntityFunctions.TruncateTime(filters.ToDate))
                     //
                     (!bookingStatus.HasValue || bookingStatus.Value == a.BookingStatusId) &&
                     (!partnerId.HasValue || partnerId.Value == a.PartnerId))
                     .ToList();


                return Mapper.Map<List<Booking>, List<BookingViewModel>>(booking);
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