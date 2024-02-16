using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UJBHelper.Data;
using UJBHelper.DataModel;
using UJBHelper.Services;

namespace UJBHelper.Common
{
    public class Notification_Sender
    {

        private readonly IMongoCollection<Notification> _notification;
        private NotificationService _service;
        private readonly IMongoCollection<NotificationQueue> _notificationQueue;
        private readonly IMongoCollection<User> _user;
        private Notification_Queue _notificationqueue;
        private Lead_Email_Details Lead_Details = new Lead_Email_Details();
        private readonly IMongoCollection<Leads> _lead;
        public Notification_Sender()
        {
            var client = new MongoClient(DbHelper.GetConnectionString());
            var database = client.GetDatabase(DbHelper.GetDatabaseName());
            _notification = database.GetCollection<Notification>("Notification");
            _notificationQueue = database.GetCollection<NotificationQueue>("NotificationQueue");
            _user = database.GetCollection<User>("User");
            _service = new NotificationService();
            _notificationqueue = new Notification_Queue();
            _lead = database.GetCollection<Leads>("Leads");

        }

        public Task SendNotificationAsync(string Event, MessageBody MessageBody, string UserId, string LeadId, string BussinessId)
        {
            bool issend = false;
            try
            {

                Notification notify_template = new Notification();
                notify_template = Get_Notification_Template(Event);
                if (notify_template.isActive)
                {
                    if (notify_template.isSystemGenerated)
                    {
                        issend = Check_If_notification_Send(UserId, Event, notify_template, LeadId);
                    }
                    else
                    {
                        issend = true;
                    }
                    if (issend)
                    {
                        if (notify_template != null)
                        {
                            var Notificationdata = notify_template.Data;
                            MessageBody = GetbodyData(Event, MessageBody, UserId, LeadId, BussinessId);

                            foreach (var notifydata in notify_template.Data)
                            {
                                string Body = "";
                                string subject = "";
                                string Reciever = notifydata.Receiver;

                                List<string> email_Id = new List<string>();
                                List<string> Mobile_Number = new List<string>();
                                if (notifydata.Receiver != "UJBAdmin" && notifydata.Receiver != "Referred")
                                {
                                    _notificationqueue.Message_To_Add_Notification_List(Event, notifydata.Receiver, UserId, LeadId, "");
                                }

                                switch (Reciever)
                                {
                                    case "UJBAdmin":
                                        email_Id = _service.Get_Admin_EmailIds();
                                        Mobile_Number = _service.Get_Admin_MobileNumbers();
                                        break;
                                    case "Referrer":
                                        UserId = Lead_Details.referredById;
                                        email_Id.Add(Lead_Details.referredByemailId);

                                        Mobile_Number.Add(_service.Get_Receiver_Mobile_Number(Reciever, UserId, LeadId));
                                        break;
                                    case "Referred":
                                        email_Id.Add(Lead_Details.referredToemailId);
                                        UserId = Lead_Details.clientPartnerId;
                                        Mobile_Number.Add(_service.Get_Receiver_Mobile_Number(Reciever, UserId, LeadId));
                                        break;
                                    case "Business":
                                        email_Id.Add(Lead_Details.clientPartneremailId);
                                        UserId = Lead_Details.clientPartnerId;
                                        Mobile_Number.Add(_service.Get_Receiver_Mobile_Number(Reciever, UserId, LeadId));

                                        break;
                                    default:
                                        email_Id.Add(_service.Get_Receiver_Email_Id(Reciever, UserId));
                                        Mobile_Number.Add(_service.Get_Receiver_Mobile_Number(Reciever, UserId, LeadId));
                                        break;
                                }

                                if (notifydata.Email.isEmailAllowed)
                                {

                                    Body = BindBody(notifydata.Email.Body, MessageBody, Reciever);
                                    subject = BindBody(notifydata.Email.Subject, MessageBody, Reciever);
                                    foreach (var EmailId in email_Id)
                                    {
                                        try
                                        {
                                            _notificationqueue.Add_To_Queue(UserId, LeadId, notify_template._id, "", "success", Event, EmailId, "Email", Reciever, "", Body);
                                            Email_Sms_Sender.Send_Email(EmailId, "", subject, Body);
                                            Update_Notification_Status(notify_template._id, "Email Sent Successfully", notify_template._id, "success", EmailId, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                                        }
                                        catch (Exception ex)
                                        {
                                            Update_Notification_Status(notify_template._id, ex.ToString(), notify_template._id, "failure", EmailId, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                                            //
                                        }
                                    }
                                }



                                if (notifydata.SMS.isSMSAllowed)
                                {

                                    Body = BindBody(notifydata.SMS.SMSBody, MessageBody, Reciever);
                                    foreach (var mobile_number in Mobile_Number)
                                    {
                                        try
                                        {
                                            _notificationqueue.Add_To_Queue(UserId, LeadId, notify_template._id, "", "success", Event, mobile_number, "SMS", Reciever, "", Body);
                                            Email_Sms_Sender.Send_Sms(Body, mobile_number);

                                            Update_Notification_Status(notify_template._id, "SMS Sent Successfully", notify_template._id, "success", mobile_number, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                                        }
                                        catch (Exception ex)
                                        {
                                            Update_Notification_Status(notify_template._id, ex.ToString(), notify_template._id, "failure", mobile_number, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                                        }
                                    }

                                }
                                if (notifydata.Push.isPushAllowed && Reciever != "UJBAdmin")
                                {

                                    Body = BindBody(notifydata.Push.MessageBody, MessageBody, Reciever);
                                    subject = BindBody(notifydata.Push.Title, MessageBody, Reciever);
                                    try
                                    {
                                        _notificationqueue.Add_To_Queue(UserId, LeadId, notify_template._id, "", "success", Event, "", "Push", Reciever, "", Body);
                                        // await Send_Push_Notification(Reciever, Event, UserId, Body, subject);
                                        Update_Notification_Status(notify_template._id, "Push Sent Successfully", notify_template._id, "success", "", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                                    }
                                    catch (Exception ex)
                                    {
                                        Update_Notification_Status(notify_template._id, ex.ToString(), notify_template._id, "failure", "", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                                    }
                                }


                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {

            }
            return null;
            //  return Task.CompletedTask();
        }

        public void SendNotification(string Event, MessageBody MessageBody, string UserId, string LeadId, string BussinessId)
        {
            bool issend = false;
            try
            {

                Notification notify_template = new Notification();
                notify_template = Get_Notification_Template(Event);
                if (notify_template.isActive)
                {
                    if (notify_template.isSystemGenerated)
                    {
                        issend = Check_If_notification_Send(UserId, Event, notify_template, LeadId);
                    }
                    else
                    {
                        issend = true;
                    }
                    if (issend)
                    {
                        if (notify_template != null)
                        {
                            var Notificationdata = notify_template.Data;
                            MessageBody = GetbodyData(Event, MessageBody, UserId, LeadId, BussinessId);

                            foreach (var notifydata in notify_template.Data)
                            {
                                string Body = "";
                                string subject = "";
                                string Reciever = notifydata.Receiver;

                                List<string> email_Id = new List<string>();
                                List<string> Mobile_Number = new List<string>();
                                if (notifydata.Receiver != "UJBAdmin" && notifydata.Receiver != "Referred")
                                {
                                    _notificationqueue.Message_To_Add_Notification_List(Event, notifydata.Receiver, UserId, LeadId, "");
                                }
                                string notifiyId = "";

                                switch (Reciever)
                                {
                                    case "UJBAdmin":
                                        email_Id = _service.Get_Admin_EmailIds();
                                        Mobile_Number = _service.Get_Admin_MobileNumbers();
                                        break;
                                    case "Referrer":
                                        UserId = Lead_Details.referredById;
                                        email_Id.Add(Lead_Details.referredByemailId);

                                        Mobile_Number.Add(_service.Get_Receiver_Mobile_Number(Reciever, UserId, LeadId));
                                        break;
                                    case "Referred":
                                        email_Id.Add(Lead_Details.referredToemailId);
                                        // UserId = Lead_Details.clientPartnerId;
                                        // Mobile_Number.Add(_service.Get_Receiver_Mobile_Number(Reciever, UserId, LeadId));
                                        break;
                                    case "Business":
                                        email_Id.Add(Lead_Details.clientPartneremailId);
                                        UserId = Lead_Details.clientPartnerId;
                                        Mobile_Number.Add(_service.Get_Receiver_Mobile_Number(Reciever, UserId, LeadId));

                                        break;
                                    default:
                                        email_Id.Add(_service.Get_Receiver_Email_Id(Reciever, UserId));
                                        Mobile_Number.Add(_service.Get_Receiver_Mobile_Number(Reciever, UserId, LeadId));
                                        break;
                                }

                                if (notifydata.Email.isEmailAllowed)
                                {

                                    Body = BindBody(notifydata.Email.Body, MessageBody, Reciever);
                                    subject = BindBody(notifydata.Email.Subject, MessageBody, Reciever);
                                    foreach (var EmailId in email_Id)
                                    {
                                        try
                                        {
                                            notifiyId = _notificationqueue.Add_To_Queue(UserId, LeadId, notify_template._id, "", "success", Event, EmailId, "Email", Reciever, "", Body);
                                            Email_Sms_Sender.Send_Email(EmailId, "", subject, Body);
                                            Update_Notification_Status(notifiyId, "Email Sent Successfully", notify_template._id, "success", EmailId, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                                        }
                                        catch (Exception ex)
                                        {
                                            Update_Notification_Status(notifiyId, ex.ToString(), notify_template._id, "failure", EmailId, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                                            //
                                        }
                                    }
                                }



                                if (notifydata.SMS.isSMSAllowed)
                                {

                                    Body = BindBody(notifydata.SMS.SMSBody, MessageBody, Reciever);
                                    foreach (var mobile_number in Mobile_Number)
                                    {
                                        try
                                        {
                                            notifiyId = _notificationqueue.Add_To_Queue(UserId, LeadId, notify_template._id, "", "success", Event, mobile_number, "SMS", Reciever, "", Body);
                                            Email_Sms_Sender.Send_Sms(Body, mobile_number);

                                            Update_Notification_Status(notifiyId, "SMS Sent Successfully", notify_template._id, "success", mobile_number, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                                        }
                                        catch (Exception ex)
                                        {
                                            Update_Notification_Status(notifiyId, ex.ToString(), notify_template._id, "failure", mobile_number, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                                        }
                                    }

                                }
                                if (notifydata.Push.isPushAllowed && Reciever != "UJBAdmin")
                                {

                                    Body = BindBody(notifydata.Push.MessageBody, MessageBody, Reciever);
                                    subject = BindBody(notifydata.Push.Title, MessageBody, Reciever);
                                    try
                                    {
                                        notifiyId = _notificationqueue.Add_To_Queue(UserId, LeadId, notify_template._id, "", "success", Event, "", "Push", Reciever, "", Body);
                                        Send_Push_Notification(Reciever, Event, UserId, Body, subject);
                                        Update_Notification_Status(notifiyId, "Push Sent Successfully", notify_template._id, "success", "", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                                    }
                                    catch (Exception ex)
                                    {
                                        Update_Notification_Status(notifiyId, ex.ToString(), notify_template._id, "failure", "", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));

                                    }
                                }


                            }
                        }
                    }
                }

            }
            catch (Exception ex)
            {

            }
            //    return issend;
        }
        private void Update_Notification_Status(string queueId, string message, string templateId, string status, string contactInfo, string date)
        {
            _service.Update_Notification_Queue(queueId, message, templateId, status, contactInfo, date);
            //  return true;
        }

        internal bool Check_If_notification_Send(string UserId, string Event, Notification notify_template, string LeadId)
        {
            DateTime notifyDate = new DateTime();
            bool issend = false;
            if (UserId != null && UserId != "")
            {
                if (_notificationQueue.Find(x => x.userId == UserId && x.Event == Event && (x.status != "success")).CountDocuments() != 0 && _notificationQueue.Find(x => x.userId == UserId && x.Event == Event && (x.status == "success")).CountDocuments() == 0)
                {
                    notifyDate = _notificationQueue.Find(x => x.userId == UserId && x.Event == Event && x.status != "success").SortByDescending(x => x.dateOfNotification).Project(x => x.dateOfNotification).FirstOrDefault();

                }
                else
                {
                    notifyDate = _user.Find(x => x._id == UserId).Project(x => x.Created.created_On).FirstOrDefault();
                }


            }
            else if (LeadId != null && LeadId != "")
            {

                if (Event == "Auto Rejection Reminder")
                {
                    var leadDate = _lead.Find(x => x.Id == LeadId).Project(x => x.Created.created_On).FirstOrDefault();
                    var notificationDate = _notificationQueue.Find(x => x.leadId == LeadId && x.Event == "Auto Rejection Reminder" && x.status == "success").SortByDescending(x => x.dateOfNotification).Project(x => x.dateOfNotification).FirstOrDefault();
                    if ((DateTime.Now - leadDate).TotalHours >= 48 && (DateTime.Now - notificationDate).TotalHours >= 24)
                    {
                        issend = true;
                    }
                    else if ((DateTime.Now - leadDate).TotalHours >= 72 && (DateTime.Now - notificationDate).TotalHours >= 48)
                    {
                        issend = true;
                    }
                }
                else
                {
                    int failcount = (int)_notificationQueue.Find(x => x.leadId == LeadId && x.Event == Event && (x.status != "success")).CountDocuments();
                    int succcount = (int)_notificationQueue.Find(x => x.leadId == LeadId && x.Event == Event && (x.status == "success")).CountDocuments();
                    if (succcount != 0)
                    {
                        notifyDate = _notificationQueue.Find(x => x.userId == UserId && x.Event == Event && x.status == "success").SortByDescending(x => x.dateOfNotification).Project(x => x.dateOfNotification).FirstOrDefault();

                    }
                    else
                    {
                        if (Event.ToLower().Contains("followup"))
                        {
                            notifyDate = _lead.Find(x => x.Id == LeadId).Project(x => x.refStatusUpdatedOn).FirstOrDefault();
                        }

                        else
                        {
                            notifyDate = _lead.Find(x => x.Id == LeadId).Project(x => x.Created.created_On).FirstOrDefault();
                        }
                    }
                }

            }

            if (Event != "Auto Rejection Reminder")
            {
                if (notifyDate != null)
                {
                    if (notify_template.frequencyType == "Day")
                    {
                        if ((DateTime.Now - notifyDate).TotalDays >= notify_template.frequency)
                        {
                            issend = true;
                        }
                    }
                    if (notify_template.frequencyType == "Hours")
                    {
                        if ((DateTime.Now - notifyDate).TotalHours >= notify_template.frequency)
                        {
                            issend = true;
                        }
                    }
                    if (notify_template.frequencyType == "Minutes")
                    {
                        if ((DateTime.Now - notifyDate).TotalMinutes >= notify_template.frequency)
                        {
                            issend = true;
                        }
                    }
                }
                else { issend = false; }
            }
            return issend;
        }

        public MessageBody GetbodyData(string Event, MessageBody _messageBody, string UserId, string LeadId, string bussinessID)
        {
            switch (Event)
            {
                case "Lead Created":
                case "Lead Acceptance":
                case "Lead Rejection":
                case "Lead Status Changed":
                case "Lead Auto Rejection":
                case "Auto Rejection Reminder":
                case "Not Connected FollowUp":
                case "Called But No Response FollowUp":
                case "Discussion In Progress FollowUp":
                case "Deal Won FollowUp":
                case "Received Part Payment & Transferred to UJustBe FollowUp":
                case "Work In Progress FollowUp":
                case "Work Completed FollowUp":
                case "Received Full And Final Payment FollowUp":
                case "Agreed Percentage Transferred FollowUp":
                    {
                        Lead_Details = _service.Get_Lead_Mailing_Details(LeadId, _messageBody.statusId);
                        _messageBody.referralCode = Lead_Details.referralCode;
                        _messageBody.referredByName = Lead_Details.referredByName;
                        _messageBody.clientPartnerName = Lead_Details.clientPartnerName;
                        _messageBody.currentStatus = Lead_Details.currentStatus;
                        _messageBody.productServiceName = Lead_Details.productServiceName;
                        _messageBody.referredToName = Lead_Details.referredToName;                    // 

                        var referredTo = Lead_Details.referredToName;
                        if (Lead_Details.isForSelf)
                        {
                            referredTo = "your self";
                        }
                        _messageBody.referredTo = referredTo;

                    }

                    break;
                case "Registeration":
                case "KYC Approval Under Process":
               
                case "Reject Partner":
                case "Partner Profile Updated":
                case "Partner Business Profile Updated":
                case "Guest Reminder To Upgrade":
                case "Email Changed":
                case "Mobile Number Changed":
                case "Skip KYC":
                case "Incomplete Profile":
                case "No Client Partner Products":
                case "Registeration Fee":
                case "Membership Fee":
                case "Meeting Fee":
                case "Registeration Fee Reminder":
                case "Membership Fee Reminder":
                case "Meeting Fee Reminder":
                    if (_messageBody.UserName == null || _messageBody.UserName == "")
                    {
                        _messageBody.UserName = _service.Get_Receiver_Name(UserId);
                    }
                    break;
                case "Approve Partner":
                    if (_messageBody.UserName == null || _messageBody.UserName == "")
                    {
                        _messageBody.UserName = _service.Get_Receiver_Name(UserId);
                    }
                    _messageBody.MentorName = _service.Get_Mentor_Name(UserId);
                    break;
                case "Product Details Updated":

                    string userId = _service.Get_bussiness_details(bussinessID);
                    _messageBody.clientPartnerName = _service.Get_bussiness_Name(bussinessID);
                    _messageBody.UserName = _service.Get_Receiver_Name(userId);
                    _messageBody.UJBCode = _service.Get_Receiver_UJBcode(userId);
                    break;

            }
            return _messageBody;
        }

        public async Task SendSMS(string Receiver, string message, string UserId, string leadId)
        {
            string mobile_number = "";
            if (Receiver == "UJBAdmin")
            {
                var adminMobileNumbers = _service.Get_Admin_MobileNumbers();
                foreach (var adminMobileNumber in adminMobileNumbers)
                {
                    Email_Sms_Sender.Send_Sms(message, adminMobileNumber);
                }
            }
            else
            {
                if (Receiver != "User")
                {
                    mobile_number = _service.Get_Receiver_Mobile_Number(Receiver, UserId, leadId);
                }
                else
                {
                    mobile_number = _service.Get_Receiver_Mobile_Number(Receiver, UserId, leadId);
                }


                Email_Sms_Sender.Send_Sms(message, mobile_number);
            }
        }

        private void Send_Push_Notification(string Receiver, string template_type, string UserId, string message, string title)
        {
            try
            {

                var eventFlag = "";
                var Reciverflag = "";
                switch (template_type)
                {
                    case "Lead Created":
                    case "Lead Acceptance":
                    case "Lead Rejection":
                    case "Lead Status Changed":
                    case "Lead Auto Rejection":
                    case "Auto Rejection Reminder":
                        switch (Receiver)
                        {

                            case "Referrer":
                                eventFlag = "Referral";
                                Reciverflag = "Partner";
                                break;

                            case "Business":
                                eventFlag = "Bussiness";
                                Reciverflag = "Listed Partner";
                                break;
                        }
                        break;

                    case "Not Connected FollowUp":
                    case "Called But No Response FollowUp":
                    case "Discussion In Progress FollowUp":
                    case "Deal Won FollowUp":
                    case "Received Part Payment & Transferred to UJustBe FollowUp":
                    case "Work In Progress FollowUp":
                    case "Work Completed FollowUp":
                    case "Received Full And Final Payment FollowUp":
                    case "Agreed Percentage Transferred FollowUp":

                        switch (Receiver)
                        {

                            case "Referrer":
                                eventFlag = "Followup";
                                Reciverflag = "Partner";
                                break;

                            case "Business":
                                eventFlag = "Followup";
                                Reciverflag = "Listed Partner";
                                break;
                        }
                        break;
                    case "Approve Partner":
                        eventFlag = "RegistrationApproved";
                        Reciverflag = "Partner";

                        break;
                    case "Reject Partner":
                        eventFlag = "RegistrationRejected";
                        Reciverflag = "Partner";

                        break;
                    case "Partner Profile Updated":
                        eventFlag = "Profileupdated";
                        Reciverflag = "Partner";

                        break;
                    case "Partner Business Profile Updated":
                        eventFlag = "Profileupdated";
                        Reciverflag = "Listed Partner";

                        break;
                    case "Guest Reminder To Upgrade":
                        eventFlag = "BecomePartner";
                        Reciverflag = "Guest";

                        break;

                    case "Incomplete Profile":
                        eventFlag = "IncompleteProfile";
                        Reciverflag = "Partner";

                        break;
                    case "No Client Partner Products":
                        eventFlag = "IncompleteProfile";
                        Reciverflag = "Listed Partner";

                        break;



                }
                var token = _service.Get_User_Token(UserId);

                var data = new Dictionary<string, string>();
                data.Add("Role", Reciverflag);
                data.Add("Flag", eventFlag);

                Email_Sms_Sender.Send_Push_Notification(token, title, message, data);
                //}
                //   Update_Notification_Status(notification._id, "Push Notification Sent Successfully", notify_template._id, "success", fcm_number, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            }
            catch (Exception ex)
            {
                Logger.Log.Error(Assembly.GetCallingAssembly().GetName().Name + "\n\t" + ex.ToString());
                //  Update_Notification_Status(notification._id, ex.ToString(), notify_template._id, "failure", fcm_number, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            }
        }

        private string BindBody(string Body, MessageBody _messageBody, string Reciever)
        {
            string message = "";
            try
            {
                if (_messageBody.referredTo == "your self")
                {
                    if (Reciever != "Referrer")
                    {
                        _messageBody.referredTo = "self";
                    }

                }

                message = Body
                 .Replace("@user", _messageBody.UserName)
                 .Replace("@new_password", _messageBody.new_password)
                 .Replace("@business", _messageBody.clientPartnerName)
                   .Replace("@productservice", _messageBody.productServiceName)
                   .Replace("@referredperson", _messageBody.referredTo)
                   .Replace("@referrer", _messageBody.referredByName)
                   .Replace("@status", _messageBody.currentStatus)
                   .Replace("@new_otp", _messageBody.new_otp)
                   .Replace("@ref_code", _messageBody.referralCode)
                  .Replace("@fields", _messageBody.details)
                 .Replace("@UJBCode", _messageBody.UJBCode)
                  .Replace("@Mentor", _messageBody.MentorName);

            }
            catch (Exception ex)
            {
                Logger.Log.Error(Assembly.GetCallingAssembly().GetName().Name + "\n\t" + ex.ToString());

            }
            return message;
        }


        public Notification Get_Notification_Template(string template_type)
        {
            Notification _notify = new Notification();
            if (_notification.Find(x => x.Event == template_type && x.isActive).CountDocuments() > 0)
            {
                _notify = _notification.Find(x => x.Event == template_type && x.isActive).FirstOrDefault();
            }
            return _notify;
        }
    }

    public class MessageBody
    {
        public string UserName { get; set; }
        public string password { get; set; }
        public string clientPartnerName { get; set; }
        public string productServiceName { get; set; }
        public string referredToName { get; set; }
        public string new_otp { get; set; }
        public string new_password { get; set; }
        public string referredTo { get; set; }
        public string referredByName { get; set; }
        public string currentStatus { get; set; }
        public string referralCode { get; set; }
        public int statusId { get; set; }
        public string details { get; set; }
        public string bussinessName { get; set; }
        public string UJBCode { get; set; }
        public string MentorName { get; set; }
    }
}
