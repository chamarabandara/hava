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
            

            try
            {
                MailMessage mails = new MailMessage();
                //set the FROM address
                mails.From = new MailAddress("andrew.hava@pandarix.com");
                //set the RECIPIENTS
                mails.To.Add(toMail);
                mails.CC.Add("hava@pandarix.com");
                //enter a SUBJECT
                mails.Subject = mailSubject;
                //Enter the message BODY
                mails.Body = mailBody;
                mails.IsBodyHtml = true;
                //set the mail server (default should be auth.smtp.1and1.co.uk)
                SmtpClient smtp = new SmtpClient("auth.smtp.1and1.co.uk");
                //Enter your full e-mail address and password
                smtp.Credentials = new NetworkCredential("andrew.hava@pandarix.com", "Andrew123456");
                //send the message 
                smtp.Send(mails);
            }
            catch(System.Net.Mail.SmtpException ex)
            {
                //TODO: LOG EXCEPTION AND DISPLAY FRIENDLY MESSAGE!
            }
            

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