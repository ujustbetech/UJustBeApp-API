using System;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using log4net.Config;
using System.Timers;
using UJBHelper.Common;
using UJBHelper.DataModel;
using System.Collections.Generic;
using UJBNotification_Email.Services;

namespace UJBNotification_Email
{
    partial class EmailSend : ServiceBase
    {
        private Timer _schedulertimer;
       
        private NotificationService _service;
        private List<NotificationQueue> notifications;
        private NotificationQueue notification;
      
        private Notification notify_template;
        private Lead_Email_Details email_Details = new Lead_Email_Details();
      
        private string template_type;
       
        private string email_Id;
       
        private string user_name;
        private string user_id = "";
        private string new_password = "";
        private string new_otp = "";
      
        public EmailSend()
        {
            InitializeComponent();
            _service = new NotificationService();
            XmlConfigurator.Configure();



        }

        protected override void OnStart(string[] args)
        {

            _schedulertimer = new Timer();
            _schedulertimer.Enabled = true;
            _schedulertimer.Interval = TimeSpan.FromMinutes(3).TotalMilliseconds; ;
            _schedulertimer.Elapsed += this.Process;

        }

        protected override void OnStop()
        {
            // TODO: Add code here to perform any tear-down necessary to stop your service.
        }

        public void Process(object sender, ElapsedEventArgs e)
        {
             _schedulertimer.Enabled = false;
            method1();
            _schedulertimer.Enabled = true;
        }
        public void method2()
        {
        }
            public void method1()
        {
            try
            {
                Logger.Log.Info(" Process Service Started at_ " + DateTime.Now);
                //_schedulertimer.Enabled = false;
                _service = new NotificationService();
                notifications = _service.Get_Notification();
                if (notifications != null)
                {
                    foreach (NotificationQueue notify in notifications)
                    {
                        if (!string.IsNullOrWhiteSpace(notify.userId))
                        {
                            user_id = notify.userId;
                        }
                        template_type = notify.Event;

                        notify_template = _service.Get_Notification_Template(template_type);
                        notification = notify;
                        Send_Notification();
                    }
                }

                //Check_If_Notification_Inserted();
                Logger.Log.Info(" Process Service Ended at_ " + DateTime.Now);

            }
            catch (Exception ex)
            {

                Logger.Log.Error(Assembly.GetCallingAssembly().GetName().Name + "\n\t" + ex.ToString());



            }
            
        }

     
       

        #region OnSpotNotify
      
      
        public void Send_Notification()
        {
            switch (template_type)
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
                    email_Details = _service.Get_Lead_Mailing_Details(notification.leadId);
                    switch (notification.Receiver)
                    {
                        case "UJBAdmin":
                            //switch (notification.Type)
                            //{
                            //    case "Push":
                            //        Send_Push_Notification();
                            //        break;
                            //    case "Email":
                                    Send_Email_To_UJB_Admin();
                            //        break;
                            //    case "SMS":
                            //        Send_SMS_To_Receiver();
                            //        break;
                            //}
                            break;
                        case "Referrer":
                            user_id = email_Details.referredById;
                            //switch (notification.Type)
                            //{
                            //    case "Push":
                            //        Send_Push_Notification();
                            //        break;
                            //    case "Email":
                                    Send_Email_To_Referrer();
                            //        break;
                            //    case "SMS":
                            //        Send_SMS_To_Receiver();
                            //        break;
                            //}
                            break;
                        case "Referred":
                            user_id = email_Details.referredById;
                            //switch (notification.Type)
                            //{
                            //    case "Email":
                           //         Send_Email_To_Referred_Person();
                            //        break;
                            //    case "SMS":
                            //        Send_SMS_To_Receiver();
                            //        break;
                            //}
                            break;
                        case "Business":
                            user_id = email_Details.clientPartnerId;
                            //switch (notification.Type)
                            //{
                            //    case "Push":
                            //        Send_Push_Notification();
                            //        break;
                           //     case "Email":
                                    Send_Email_To_Client_Partner();
                            //        break;
                            //    case "SMS":
                            //        Send_SMS_To_Receiver();
                            //        break;
                            //}
                            break;
                    }
                    break;
                case "Registeration":
                    user_name = _service.Get_Receiver_Name(notification.userId);
                    new_password = SecurePasswordHasherHelper.Decrypt(_service.Get_User_Password(notification.Receiver, notification.userId));
                    //switch (notification.Type)
                    //{
                    //    case "Email":
                            Send_Email_To_Receiver();
                        //    break;
                        //case "SMS":
                        //    Send_SMS_To_Receiver();
                        //    break;
                   // }
                    break;
                case "Forgot Password":
                case "Change Password":
                    user_name = _service.Get_Receiver_Name(notification.userId);
                    new_password = SecurePasswordHasherHelper.Decrypt(_service.Get_User_Password(notification.Receiver, notification.userId));
                    //switch (notification.Type)
                    //{
                    //    case "Email":
                            Send_Email_To_Receiver();
                    //        break;
                    //    case "SMS":
                    //        Send_SMS_To_Receiver();
                    //        break;
                    //}
                    break;
                case "KYC Approval Under Process":
                case "OTP Verification":
                case "Approve Partner":
                case "Reject Partner":
                case "Partner Profile Updated":
                case "Partner Business Profile Updated":
                case "Guest Reminder To Upgrade":
                    user_name = _service.Get_Receiver_Name(notification.userId);
                    new_otp = _service.Get_User_OTP(notification.userId);
                    //switch (notification.Type)
                    //{
                    //    case "Push":
                    //        Send_Push_Notification();
                    //        break;
                    //    case "Email":
                            Send_Email_To_Receiver();
                    //        break;
                    //    case "SMS":
                    //        Send_SMS_To_Receiver();
                    //        break;
                    //}
                    break;
                case "Email Changed":
                case "Mobile Number Changed":
                case "Skip KYC":
                    user_name = _service.Get_Receiver_Name(notification.userId);
                    //switch (notification.Type)
                    //{
                    //    case "Email":
                            Send_Email_To_Receiver();
                    //        break;
                    //    case "SMS":
                    //        Send_SMS_To_Receiver();
                    //        break;
                    //}
                    break;
                case "Incomplete Profile":
                case "No Client Partner Products":
                    user_name = _service.Get_Receiver_Name(notification.userId);
                    //new_otp = _service.Get_User_OTP(notification.userId);
                    //switch (notification.Type)
                    //{
                    //    case "Push":
                    //        Send_Push_Notification();
                    //        break;
                    //    case "Email":
                    Send_Email_To_Receiver();
                    //        break;
                    //    case "SMS":
                    //        Send_SMS_To_Receiver();
                    //        break;
                    //}
                    break;
                case "Registeration Fee":
                    //switch (notification.Type)
                    //{
                    //    case "Email":
                            Send_Email_To_Receiver();
                    //        break;
                    //    case "SMS":
                    //        Send_SMS_To_Receiver();
                    //        break;
                    //}
                    break;
                case "Membership Fee":
                    //switch (notification.Type)
                    //{
                    //    case "Email":
                            Send_Email_To_Receiver();
                    //        break;
                    //    case "SMS":
                    //        Send_SMS_To_Receiver();
                    //        break;
                    //}
                    break;
                case "Meeting Fee":
                    //switch (notification.Type)
                    //{
                    //    case "Email":
                            Send_Email_To_Receiver();
                    //        break;
                    //    case "SMS":
                    //        Send_SMS_To_Receiver();
                    //        break;
                    //}
                    break;
                case "Registeration Fee Reminder":
                    //switch (notification.Type)
                    //{
                    //    case "Email":
                            Send_Email_To_Receiver();
                    //        break;
                    //    case "SMS":
                    //        Send_SMS_To_Receiver();
                    //        break;
                    //}
                    break;
                case "Membership Fee Reminder":
                    //switch (notification.Type)
                    //{
                    //    case "Email":
                            Send_Email_To_Receiver();
                    //        break;
                    //    case "SMS":
                    //        Send_SMS_To_Receiver();
                    //        break;
                    //}
                    break;
                case "Meeting Fee Reminder":
                    //switch (notification.Type)
                    //{
                    //    case "Email":
                          Send_Email_To_Receiver();
                    //        break;
                    //    case "SMS":
                    //        Send_SMS_To_Receiver();
                    //        break;
                    //}
                    break;
            }
        }

        private void Send_Email_To_Receiver()
        {
            try
            {
                email_Id = _service.Get_Receiver_Email_Id(notification.Receiver, notification.userId);
                var notificationData = notify_template.Data.Where(x => x.Receiver == notification.Receiver).FirstOrDefault();

                var subject = notificationData.Email.Subject
                    .Replace("@user", user_name)
                    .Replace("@new_password", new_password);

                var message_body = notificationData.Email.Body
                   .Replace("@user", user_name)
                   .Replace("@new_password", new_password);


                //if (NotifyList.NotificationListHolder.Contains(notification.Event))
                //{
                //    var message = notificationData.Push.MessageBody
                //  .Replace("@user", user_name)
                //  .Replace("@new_password", new_password)
                //  .Replace("@business", email_Details.clientPartnerName)
                //    .Replace("@productservice", email_Details.productServiceName)
                //    .Replace("@referredperson", email_Details.referredToName)
                //    .Replace("@referrer", email_Details.referredByName)
                //    .Replace("@status", email_Details.currentStatus);

                //    if (notification.status == "new")
                //    {
                //        var ns = new Notification_Queue();
                //        ns.Add_To_Notification_List(user_id, DateTime.Now, message, notification.Event, false, notify_template.isSystemGenerated, notification.leadId, false);
                //    }
                //}

                Email_Sms_Sender.Send_Email(email_Id, user_name, subject, message_body);

                Update_Notification_Status(notification._id, "Email Sent Successfully", notify_template._id, "success", email_Id, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            }
            catch (Exception ex)
            {
                Update_Notification_Status(notification._id, ex.ToString(), notify_template._id, "failure", email_Id, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                Logger.Log.Error(Assembly.GetCallingAssembly().GetName().Name + "\n\t" + ex.ToString());
            }
        }

        private void Send_Email_To_UJB_Admin()
        {
            try
            {
                var notificationData = notify_template.Data.Where(x => x.Receiver == "UJBAdmin").FirstOrDefault();

                var subject = notificationData.Email.Subject
                    .Replace("@business", email_Details.clientPartnerName)
                    .Replace("@productservice", email_Details.productServiceName)
                    .Replace("@referredperson", email_Details.referredToName)
                    .Replace("@referrer", email_Details.referredByName)
                    .Replace("@status", email_Details.currentStatus);

                var message_body = notificationData.Email.Body
                    .Replace("@business", email_Details.clientPartnerName)
                    .Replace("@productservice", email_Details.productServiceName)
                    .Replace("@referredperson", email_Details.referredToName)
                    .Replace("@referrer", email_Details.referredByName)
                    .Replace("@status", email_Details.currentStatus)
                    .Replace("@ref_code", email_Details.referralCode);

                if (notification.Receiver == "UJBAdmin")
                {
                    var adminEmailIds = _service.Get_Admin_EmailIds();
                    foreach (var adminEmailId in adminEmailIds)
                    {
                        Email_Sms_Sender.Send_Email(adminEmailId, "UJB Admin", subject, message_body);
                    }
                }

                Update_Notification_Status(notification._id, "Email Sent Successfully", notify_template._id, "success", email_Details.ujbAdminEmailId, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            }
            catch (Exception ex)
            {
                Logger.Log.Error(Assembly.GetCallingAssembly().GetName().Name + "\n\t" + ex.ToString());

                Update_Notification_Status(notification._id, ex.ToString(), notify_template._id, "failure", email_Details.ujbAdminEmailId, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            }
        }

        private void Update_Notification_Status(string queueId, string message, string templateId, string status, string contactInfo, string date)
        {
            _service.Update_Notification_Queue(queueId, message, templateId, status, contactInfo, date);
        }

        private void Send_Email_To_Referrer()
        {
            try
            {
                var notificationData = notify_template.Data.Where(x => x.Receiver == "Referrer").FirstOrDefault();

                var subject = notificationData.Email.Subject
                    .Replace("@business", email_Details.clientPartnerName)
                    .Replace("@productservice", email_Details.productServiceName)
                    .Replace("@referredperson", email_Details.referredToName)
                    .Replace("@referrer", email_Details.referredByName)
                    .Replace("@status", email_Details.currentStatus)
                    .Replace("@ref_code", email_Details.referralCode);
                var referredTo = email_Details.referredToName;
                if (email_Details.isForSelf)
                {
                    referredTo = "Your Self";
                }
                var message_body = notificationData.Email.Body
                    .Replace("@business", email_Details.clientPartnerName)
                    .Replace("@productservice", email_Details.productServiceName)
                    .Replace("@referredperson", referredTo)
                    .Replace("@referrer", email_Details.referredByName)
                    .Replace("@status", email_Details.currentStatus)
                    .Replace("@ref_code", email_Details.referralCode);
                //if (NotifyList.NotificationListHolder.Contains(notification.Event))
                //{
                //    var message = notificationData.Push.MessageBody
                //  .Replace("@user", user_name)
                //  .Replace("@new_password", new_password)
                //  .Replace("@business", email_Details.clientPartnerName)
                //    .Replace("@productservice", email_Details.productServiceName)
                //    .Replace("@referredperson", referredTo)
                //    .Replace("@referrer", email_Details.referredByName)
                //    .Replace("@status", email_Details.currentStatus)
                //    .Replace("@ref_code", email_Details.referralCode);

                //    if (notification.status == "new")
                //    {
                //        var ns = new Notification_Queue();
                //        ns.Add_To_Notification_List(user_id, DateTime.Now, message, notification.Event, false, notify_template.isSystemGenerated, notification.leadId, true);
                //    }
                //}
                Email_Sms_Sender.Send_Email(email_Details.referredByemailId, email_Details.referredByName, subject, message_body);

                Update_Notification_Status(notification._id, "Email Sent Successfully", notify_template._id, "success", email_Details.referredByemailId, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            }
            catch (Exception ex)
            {
                Logger.Log.Error(Assembly.GetCallingAssembly().GetName().Name + "\n\t" + ex.ToString());

                Update_Notification_Status(notification._id, ex.ToString(), notify_template._id, "failure", email_Details.referredByemailId, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            }
        }

        private void Send_Email_To_Client_Partner()
        {
            try
            {
                var notificationData = notify_template.Data.Where(x => x.Receiver == "Business").FirstOrDefault();

                var subject = notificationData.Email.Subject
                    .Replace("@business", email_Details.clientPartnerName)
                    .Replace("@productservice", email_Details.productServiceName)
                    .Replace("@referredperson", email_Details.referredToName)
                    .Replace("@referrer", email_Details.referredByName)
                    .Replace("@status", email_Details.currentStatus);

                if (email_Details.isForSelf)
                {
                    email_Details.referredToName = " Self";
                }
              
                var message_body = notificationData.Email.Body
                    .Replace("@business", email_Details.clientPartnerName)
                    .Replace("@productservice", email_Details.productServiceName)
                    .Replace("@referredperson", email_Details.referredToName)
                    .Replace("@referrer", email_Details.referredByName)
                    .Replace("@status", email_Details.currentStatus)
                    .Replace("@ref_code", email_Details.referralCode);

                //if (NotifyList.NotificationListHolder.Contains(notification.Event))
                //{
                //    var message = notificationData.Push.MessageBody
                //  .Replace("@user", user_name)
                //  .Replace("@new_password", new_password)
                //  .Replace("@business", email_Details.clientPartnerName)
                //    .Replace("@productservice", email_Details.productServiceName)
                //    .Replace("@referredperson", email_Details.referredToName)
                //    .Replace("@referrer", email_Details.referredByName)
                //    .Replace("@status", email_Details.currentStatus)
                //    .Replace("@ref_code", email_Details.referralCode);


                //    if (notification.status == "new")
                //    {
                //        var ns = new Notification_Queue();
                //        ns.Add_To_Notification_List(user_id, DateTime.Now, message, notification.Event, false, notify_template.isSystemGenerated, notification.leadId, false);
                //    }
                //}
                Email_Sms_Sender.Send_Email(email_Details.clientPartneremailId, email_Details.clientPartnerName, subject, message_body);

                Update_Notification_Status(notification._id, "Email Sent Successfully", notify_template._id, "success", email_Details.clientPartneremailId, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            }
            catch (Exception ex)
            {
                Logger.Log.Error(Assembly.GetCallingAssembly().GetName().Name + "\n\t" + ex.ToString());

                Update_Notification_Status(notification._id, ex.ToString(), notify_template._id, "failure", email_Details.clientPartneremailId, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            }
        }

        private void Send_Email_To_Referred_Person()
        {
            try
            {
                var notificationData = notify_template.Data.Where(x => x.Receiver == "Referred").FirstOrDefault();

                var subject = notificationData.Email.Subject
                    .Replace("@business", email_Details.clientPartnerName)
                    .Replace("@productservice", email_Details.productServiceName)
                    .Replace("@referredperson", email_Details.referredToName)
                    .Replace("@referrer", email_Details.referredByName)
                    .Replace("@status", email_Details.currentStatus)
                    .Replace("@ref_code", email_Details.referralCode);

                var referredBy = email_Details.referredByName;
                if (email_Details.isForSelf)
                {
                    referredBy = "Your Self";
                }
                var message_body = notificationData.Email.Body
                    .Replace("@business", email_Details.clientPartnerName)
                    .Replace("@productservice", email_Details.productServiceName)
                    .Replace("@referredperson", email_Details.referredToName)
                    .Replace("@referrer", referredBy)
                    .Replace("@status", email_Details.currentStatus)
                    .Replace("@ref_code", email_Details.referralCode);

                Email_Sms_Sender.Send_Email(email_Details.referredToemailId, email_Details.referredToName, subject, message_body);

                Update_Notification_Status(notification._id, "Email Sent Successfully", notify_template._id, "success", email_Details.referredToemailId, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            }
            catch (Exception ex)
            {
                Logger.Log.Error(Assembly.GetCallingAssembly().GetName().Name + "\n\t" + ex.ToString());

                Update_Notification_Status(notification._id, ex.ToString(), notify_template._id, "failure", email_Details.referredToemailId, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            }
        }

    
        public void Get_Notification()
        {
            if (_service.Check_If_Notification_Inserted())
            {
                Logger.Log.Info("Found New Notification at_ " + DateTime.Now);
                _service = new NotificationService();
                notifications = _service.Get_Notification();
                if (notification != null)
                {
                    foreach (NotificationQueue notify in notifications)
                    { 
                        if (!string.IsNullOrWhiteSpace(notify.userId))
                        {
                            user_id = notify.userId;
                        }
                    template_type = notify.Event;

                    notify_template = _service.Get_Notification_Template(template_type);
                    Send_Notification();
                }
                }
                //else
                //{
                //    System.Threading.Thread.Sleep(1000);
                //}
            }
            //}
        }
        #endregion


    }
}

public class Lead_Email_Details
{
    public string ujbAdminEmailId { get; set; }

    public string referredById { get; set; }
    public string referredByName { get; set; }
    public string referredByemailId { get; set; }

    public string referredToName { get; set; }
    public string referredToemailId { get; set; }

    public string clientPartnerId { get; set; }
    public string clientPartnerName { get; set; }
    public string clientPartneremailId { get; set; }

    public string productServiceName { get; set; }
    public string dealStatus { get; set; }
    public string currentStatus { get; set; }
    public string referralCode { get; set; }

    public bool isForSelf { get; set; }
}

internal class LeadRejection
{
    public string dealId { get; set; }
    public string userId { get; set; }
    public string rejectionReason { get; set; }
    public bool isAccepted { get; set; }
}

public class RejectionReminder
{
    public string leadId { get; set; }
    public DateTime dateOfLead { get; set; }
    public string type { get; set; }
}
