using LegalAgreement.Service.Models.AgreementStatus;
using LegalAgreement.Service.Repositories.AgreementStatus;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UJBHelper.Common;
using UJBHelper.DataModel;

namespace LegalAgreement.Service.Services.AgreementStatus
{
    public class AgreementStatusService : IAgreementStatusService
    {
        private readonly IMongoCollection<User> _users;
        private readonly IMongoCollection<BusinessDetails> _bsnsDetails;
        private readonly IMongoCollection<AdminUser> _adminUsers;
        private readonly IMongoCollection<Notification> _notification;
        private readonly IMongoCollection<AgreementDetails> _agreementDetails;
        private IConfiguration _iconfiguration;
        private Notification notify_template;
        public string username = "";
        public string URL = "";


        public AgreementStatusService(IConfiguration config)
        {
            _iconfiguration = config;
            var client = new MongoClient(_iconfiguration["ConnectionString"]);
            var database = client.GetDatabase(_iconfiguration["Database"]);
            _users = database.GetCollection<User>("Users");
            _bsnsDetails = database.GetCollection<BusinessDetails>("BussinessDetails");
            _adminUsers = database.GetCollection<AdminUser>("AdminUsers");
            _agreementDetails = database.GetCollection<AgreementDetails>("AgreementDetails");
            _notification = database.GetCollection<Notification>("Notification");
        }

        public void Update_Partner_Agreement_Status(Put_Request request)
        {
            User userdetails = _users.Find(x => x._id == request.userId).FirstOrDefault();
            username = userdetails.firstName + " " + userdetails.lastName;
            TimeSpan diff = DateTime.Now - DateTime.UtcNow;
            string[] timezone = DateTime.Now.ToString("zzz").Split(new char[] { '+', '-', ':' });

            _users.FindOneAndUpdate(
                Builders<User>.Filter.Eq(x => x._id, request.userId),
                Builders<User>.Update
                .Set(x => x.isPartnerAgreementAccepted, request.statusId)

                );

            AgreementDetails agreement = _agreementDetails.Find(x => x.UserId == request.userId && x.type == "Partner Agreement").SortByDescending(x => x.created.created_On).FirstOrDefault();

            string AgreementId = agreement._id;
            URL = _iconfiguration["PartnerAgreementImageURL"] + agreement.PdfURL;

            _agreementDetails.FindOneAndUpdate(
                 Builders<AgreementDetails>.Filter.Eq(x => x._id, AgreementId),
                Builders<AgreementDetails>.Update
               .Set(x => x.accepted, new Accepted
               {
                   accepted_By = request.updatedBy,
                   accepted_On = DateTime.Now.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0)),
                   isAccepted = request.statusId
               })
                );
        }

        public void Update_Listed_Partner_Agreement_Status(Put_Request request)
        {
            User userdetails = _users.Find(x => x._id == request.userId).FirstOrDefault();
            BusinessDetails bsns = _bsnsDetails.Find(x => x.UserId == request.userId).FirstOrDefault();
            username = userdetails.firstName + " " + userdetails.lastName;
            TimeSpan diff = DateTime.Now - DateTime.UtcNow;
            string[] timezone = DateTime.Now.ToString("zzz").Split(new char[] { '+', '-', ':' });

            _users.FindOneAndUpdate(
                Builders<User>.Filter.Eq(x => x._id, request.userId),
                Builders<User>.Update
                .Set(x => x.isMembershipAgreementAccepted, request.statusId)
                );

            AgreementDetails agreement = _agreementDetails.Find(x => x.UserId == request.userId && x.BusinessId == bsns.Id && x.type == "Listed Partner Agreement").SortByDescending(x => x.created.created_On).FirstOrDefault();

            string AgreementId = agreement._id;
            URL = _iconfiguration["PartnerAgreementImageURL"] + agreement.PdfURL;

            _agreementDetails.FindOneAndUpdate(
                 Builders<AgreementDetails>.Filter.Eq(x => x._id, AgreementId),
                Builders<AgreementDetails>.Update
               .Set(x => x.accepted, new Accepted
               {
                   accepted_By = request.updatedBy,
                   accepted_On = DateTime.Now.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0)),
                   isAccepted = request.statusId
               })
                );
        }

        internal Notification Get_Notification_Template(string template_type)
        {
            return _notification.Find(x => x.Event == template_type).FirstOrDefault();
        }

        internal List<string> Get_Admin_EmailIds()
        {
            return _adminUsers.Find(x => x.isActive && x.allowNotifications).Project(x => x.emailId).ToList();
        }

        internal string Get_Receiver_Email_Id(string userId)
        {

            return _users.Find(x => x._id == userId).Project(x => x.emailId).FirstOrDefault();

        }

        internal void Send_Email_To_Receiver(string UserId, bool status, string FileName)
        {
            try
            {
                string email_Id = Get_Receiver_Email_Id(UserId);
                var notificationData = notify_template.Data.Where(x => x.Receiver == "User").FirstOrDefault();
                var subject = notificationData.Email.Subject;
                var message_body = notificationData.Email.Body
                   .Replace("@user", username);
                if (status == true)
                {
                    System.Net.WebRequest WR = System.Net.WebRequest.Create(URL);
                    WR.Method = "GET";
                    System.Net.WebResponse Resp = WR.GetResponse();
                    Email_Sms_Sender.Send_Email_attachment(email_Id, username, subject, message_body, (new System.IO.StreamReader(Resp.GetResponseStream(), System.Text.Encoding.UTF8)), FileName);
                }
                else
                {
                    Email_Sms_Sender.Send_Email(email_Id, username, subject, message_body);

                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(Assembly.GetCallingAssembly().GetName().Name + "\n\t" + ex.ToString());
            }
        }

        internal void Send_Email_To_UJB_Admin(bool Status, string FileName)
        {
            try
            {
                var notificationData = notify_template.Data.Where(x => x.Receiver == "UJBAdmin").FirstOrDefault();
                var subject = notificationData.Email.Subject;
                var message_body = notificationData.Email.Body
                    .Replace("@user", username);
                var adminEmailIds = Get_Admin_EmailIds();
                if (Status == true)
                {
                    foreach (var adminEmailId in adminEmailIds)
                    {
                        /// URL = "https://api.ujustbe.com:443/Content/User/Agreement/Partner/160320201421267604.pdf";
                        System.Net.WebRequest WR = System.Net.WebRequest.Create(URL);
                        WR.Method = "GET";
                        System.Net.WebResponse Resp = WR.GetResponse();

                        //var attachURL = URL.Replace("/", @"\"); 

                        Email_Sms_Sender.Send_Email_attachment(adminEmailId, "UJB Admin", subject, message_body, (new System.IO.StreamReader(Resp.GetResponseStream(), System.Text.Encoding.UTF8)), FileName);
                    }
                }
                else
                {
                    foreach (var adminEmailId in adminEmailIds)
                    {
                        Email_Sms_Sender.Send_Email(adminEmailId, "UJB Admin", subject, message_body);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(Assembly.GetCallingAssembly().GetName().Name + "\n\t" + ex.ToString());

            }
        }

        public void Send_Agreement_Via_Email(string UserId, bool status)
        {
            try
            {
                if (status == true)
                {
                    notify_template = Get_Notification_Template("Partner Agreement Accepted");
                }
                else
                {
                    notify_template = Get_Notification_Template("Partner Agreement Declined");
                }

                Send_Email_To_UJB_Admin(status, "Partner Agreement.Pdf");
                Send_Email_To_Receiver(UserId, status, "Partner Agreement.Pdf");


            }
            catch (Exception ex)
            {
                Logger.Log.Error(Assembly.GetCallingAssembly().GetName().Name + "\n\t" + ex.ToString());

            }
        }


        public void Send_ListedPartner_Agreement_Via_Email(string UserId, bool status)
        {
            try
            {
                if (status == true)
                {
                    notify_template = Get_Notification_Template("Listed Partner Agreement Accepted");
                }
                else
                {
                    notify_template = Get_Notification_Template("Listed Partner Agreement Declined");
                }

                Send_Email_To_UJB_Admin(status, "Listed Partner Agreement.Pdf");
                Send_Email_To_Receiver(UserId, status, "Listed Partner Agreement.Pdf");
            }
            catch (Exception ex)
            {
                Logger.Log.Error(Assembly.GetCallingAssembly().GetName().Name + "\n\t" + ex.ToString());

            }
        }

    }
}
