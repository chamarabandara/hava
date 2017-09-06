using HavaBusiness;
using System;
using System.Configuration;
using System.Linq;
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