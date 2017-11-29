using HavaBusiness;
using System;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;

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

        #region GEt User by Id
        public AspNetUser GetUserById(int id)
        {
            try
            {
                return this.ObjContext.AspNetUsers.Where(u => u.Id == id).FirstOrDefault();
            }
            catch (Exception ex)
            {
                return null;
                throw;
            }
        }
        #endregion

        #region Send Mail

        public bool SendMailToRecepients(string[] toMails, string[] ccMails, string messageBody, string subject)
        {
            try
            {
                MailMessage mail = new MailMessage();
                SmtpClient SmtpServer = new SmtpClient(ConfigurationManager.AppSettings["SMTP_Client"].ToString());

                mail.From = new MailAddress(ConfigurationManager.AppSettings["From_Mail"].ToString());

                foreach (string tomail in toMails)
                {
                    mail.To.Add(tomail);
                }

                foreach (string ccmail in ccMails)
                {
                    mail.CC.Add(ccmail);
                }

                mail.Subject = subject;
                mail.Body = messageBody;

                SmtpServer.Send(mail);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        #endregion

        public bool SendMails(string toMail, string[] ccMails, string mailBody, string mailSubject)
        {
            //SmtpClient client = new SmtpClient("mail.pandarix.com");
            //If you need to authenticate
            string userName = ConfigurationManager.AppSettings["From_Mail"].ToString();
            ////string password = ConfigurationManager.AppSettings["From_PWD"].ToString();

            //////client.Credentials = new NetworkCredential(userName, password);

            ////using (MailMessage mm = new MailMessage(userName, toMail))
            ////{
            ////    mm.Subject = mailSubject;
            ////    mm.Body = mailBody;
            ////    mm.IsBodyHtml = true;
            ////    using (SmtpClient smtp = new SmtpClient())
            ////    {
            ////        smtp.Host = "pop.1and1.co.uk";
            ////        smtp.EnableSsl = true;
            ////        NetworkCredential NetworkCred = new NetworkCredential(userName, password);
            ////        smtp.UseDefaultCredentials = true;
            ////        smtp.Credentials = NetworkCred;
            ////        smtp.Port = 587;
            ////        smtp.Send(mm);
            ////    }
            ////}

            using (SmtpClient smtpClient = new SmtpClient("auth.smpt.1and1.co.uk", 587))
            {
                smtpClient.EnableSsl = true;
                smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtpClient.UseDefaultCredentials = true;
                smtpClient.Credentials = new NetworkCredential("hava@pandarix.com", "Hava1234567");

                smtpClient.Send("hava@pandarix.com", toMail, mailSubject, mailBody);
            }


            MailMessage mail = new MailMessage();
            SmtpClient client = new SmtpClient();
            client.Port = 587;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Host = "auth.smpt.1and1.co.uk"; 
            
            mail.To.Add(toMail);

            mail.From = new MailAddress(userName);
            mail.Subject = mailSubject;
            mail.Body = mailBody;
            mail.IsBodyHtml = true;
            mail.Priority = MailPriority.High;
            client.Send(mail);

            return true;

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