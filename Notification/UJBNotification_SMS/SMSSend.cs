using System;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using log4net.Config;
using System.Timers;
using UJBHelper.Common;
using UJBHelper.DataModel;
using System.Collections.Generic;
using UJBNotification_SMS.Services;

namespace UJBNotification_SMS
{
    partial class SMSSend : ServiceBase
    {
        private Timer _schedulertimer;
        //private Timer _schedulertimer1;
        //private Timer _schedulertimer2;
        //private Timer _schedulertimer3;
        //private Timer _schedulertimer4;
        //private Timer _schedulertimer5;
        //private Timer _schedulertimer6;
        object message = null;

        string subject = null;

        string body = null;
        private NotificationService _service;
        private List<NotificationQueue> notifications;
        private NotificationQueue notification;
        private Notification Referralnotify_template;
        private Notification notify_template;
        private Lead_Email_Details email_Details = new Lead_Email_Details();
        private string Referraltemplate_type;
        private string template_type;
        private string mobile_number;
        private string email_Id;
        private string fcm_number;
        private string user_name;
        private string user_id = "";
        private string new_password = "";
        private string new_otp = "";
        private string user_token = "";

        static string _ScheduledRunningTime = "6:00 AM";
        public SMSSend()
        {
            InitializeComponent();
            _service = new NotificationService();
            _schedulertimer = new Timer();
            _schedulertimer.Enabled = true;
            _schedulertimer.Interval = Convert.ToDouble(5000);
            _schedulertimer.Elapsed += this.Process;
            //_schedulertimer1 = new Timer();
            //_schedulertimer1.Enabled = true;
            //_schedulertimer1.Interval = Convert.ToDouble(5000);
            //_schedulertimer1.Elapsed += this.OnElapsedAutoDeclineTime;
            //_schedulertimer2 = new Timer();
            //_schedulertimer2.Enabled = true;
            //_schedulertimer2.Interval = Convert.ToDouble(5000);
            //_schedulertimer2.Elapsed += this.OnElapsedGuestReminder;
            //_schedulertimer3 = new Timer();
            //_schedulertimer3.Enabled = true;
            //_schedulertimer3.Interval = Convert.ToDouble(5000);
            //_schedulertimer3.Elapsed += this.OnElapsedDeclineReminder;
            //_schedulertimer4 = new Timer();
            //_schedulertimer4.Enabled = true;
            //_schedulertimer4.Interval = Convert.ToDouble(5000);
            //_schedulertimer4.Elapsed += this.OnElapsedSkipKYCReminder;
            //_schedulertimer5 = new Timer();
            //_schedulertimer5.Enabled = true;
            //_schedulertimer5.Interval = Convert.ToDouble(5000);
            //_schedulertimer5.Elapsed += this.OnElapsedCPProductReminder;
            //_schedulertimer6 = new Timer();
            //_schedulertimer6.Enabled = true;
            //_schedulertimer6.Interval = Convert.ToDouble(5000);
            //_schedulertimer6.Elapsed += this.OnElapsedIncompleteProfileReminder;
           

        }

        protected override void OnStart(string[] args)
        {
            

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
                        bool isaalowed = notify_template.Data.Where(x => x.Receiver == notification.Receiver).Select(x=>x.SMS.isSMSAllowed).FirstOrDefault();
                        if(isaalowed)
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
                            //        Send_Email_To_UJB_Admin();
                            //        break;
                            //    case "SMS":
                                  Send_SMS_To_Receiver();
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
                            //        Send_Email_To_Referrer();
                            //        break;
                            //    case "SMS":
                                   Send_SMS_To_Receiver();
                            //        break;
                            //}
                            break;
                        case "Referred":
                            user_id = email_Details.referredById;
                            //switch (notification.Type)
                            //{
                            //    case "Email":
                            //        Send_Email_To_Referred_Person();
                            //        break;
                            //    case "SMS":
                                   Send_SMS_To_Receiver();
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
                            //    case "Email":
                            //        Send_Email_To_Client_Partner();
                            //        break;
                            //    case "SMS":
                                    Send_SMS_To_Receiver();
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
                    //        Send_Email_To_Receiver();
                    //        break;
                    //    case "SMS":
                          Send_SMS_To_Receiver();
                    //        break;
                    //}
                    break;
                case "Forgot Password":
                case "Change Password":
                    user_name = _service.Get_Receiver_Name(notification.userId);
                    new_password = SecurePasswordHasherHelper.Decrypt(_service.Get_User_Password(notification.Receiver, notification.userId));
                    //switch (notification.Type)
                    //{
                    //    case "Email":
                    //        Send_Email_To_Receiver();
                    //        break;
                    //    case "SMS":
                           Send_SMS_To_Receiver();
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
                    //        Send_Email_To_Receiver();
                    //        break;
                    //    case "SMS":
                          Send_SMS_To_Receiver();
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
                    //        Send_Email_To_Receiver();
                    //        break;
                    //    case "SMS":
                           Send_SMS_To_Receiver();
                    //        break;
                    //}
                    break;
                case "Incomplete Profile":
                case "No Client Partner Products":
                    user_name = _service.Get_Receiver_Name(notification.userId);
                    new_otp = _service.Get_User_OTP(notification.userId);
                    //switch (notification.Type)
                    //{
                    //    case "Push":
                    //        Send_Push_Notification();
                    //        break;
                    //    case "Email":
                    //        Send_Email_To_Receiver();
                    //        break;
                    //    case "SMS":
                    Send_SMS_To_Receiver();
                    //        break;
                    //}
                    break;
                case "Registeration Fee":
                    //switch (notification.Type)
                    //{
                    //    case "Email":
                    //        Send_Email_To_Receiver();
                    //        break;
                    //    case "SMS":
                           Send_SMS_To_Receiver();
                    //        break;
                    //}
                    break;
                case "Membership Fee":
                    //switch (notification.Type)
                    //{
                    //    case "Email":
                    //        Send_Email_To_Receiver();
                    //        break;
                    //    case "SMS":
                           Send_SMS_To_Receiver();
                    //        break;
                    //}
                    break;
                case "Meeting Fee":
                    //switch (notification.Type)
                    //{
                    //    case "Email":
                    //        Send_Email_To_Receiver();
                    //        break;
                    //    case "SMS":
                           Send_SMS_To_Receiver();
                    //        break;
                    //}
                    break;
                case "Registeration Fee Reminder":
                    //switch (notification.Type)
                    //{
                    //    case "Email":
                    //        Send_Email_To_Receiver();
                    //        break;
                    //    case "SMS":
                          Send_SMS_To_Receiver();
                    //        break;
                    //}
                    break;
                case "Membership Fee Reminder":
                    //switch (notification.Type)
                    //{
                    //    case "Email":
                    //        Send_Email_To_Receiver();
                    //        break;
                    //    case "SMS":
                           Send_SMS_To_Receiver();
                    //        break;
                    //}
                    break;
                case "Meeting Fee Reminder":
                    //switch (notification.Type)
                    //{
                    //    case "Email":
                    //        Send_Email_To_Receiver();
                    //        break;
                    //    case "SMS":
                            Send_SMS_To_Receiver();
                    //        break;
                    //}
                    break;
            }
        }

        private void Send_SMS_To_Receiver()
        {
            try
            {

                var notificationData = notify_template.Data.Where(x => x.Receiver == notification.Receiver).FirstOrDefault();
                var referredTo = email_Details.referredToName;
                if (email_Details.isForSelf)
                {
                    referredTo = "Your Self";
                }
                var message = notificationData.SMS.SMSBody
                  .Replace("@user", user_name)
                  .Replace("@new_password", new_password)
                  .Replace("@business", email_Details.clientPartnerName)
                    .Replace("@productservice", email_Details.productServiceName)
                    .Replace("@referredperson", referredTo)
                    .Replace("@referrer", email_Details.referredByName)
                    .Replace("@status", email_Details.currentStatus)
                    .Replace("@new_otp", new_otp)
                    .Replace("@ref_code", email_Details.referralCode);

                if (notification.Receiver == "UJBAdmin")
                {
                    var adminMobileNumbers = _service.Get_Admin_MobileNumbers();
                    foreach (var adminMobileNumber in adminMobileNumbers)
                    {
                        Email_Sms_Sender.Send_Sms(message, adminMobileNumber);
                    }
                }
                else
                {
                    if (notification.Receiver != "User")
                    {
                        mobile_number = _service.Get_Receiver_Mobile_Number(notification.Receiver, user_id,notification.leadId);
                    }
                    else
                    {
                        mobile_number = _service.Get_Receiver_Mobile_Number(notification.Receiver, notification.userId, notification.leadId);
                    }


                    Email_Sms_Sender.Send_Sms(message, mobile_number);
                }
                Update_Notification_Status(notification._id, "SMS Sent Successfully", notify_template._id, "success", mobile_number, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            }
            catch (Exception ex)
            {
                Logger.Log.Error(Assembly.GetCallingAssembly().GetName().Name + "\n\t" + ex.ToString());
                Update_Notification_Status(notification._id, ex.ToString(), notify_template._id, "failure", mobile_number, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
            }
        }

    
        private void Update_Notification_Status(string queueId, string message, string templateId, string status, string contactInfo, string date)
        {
            _service.Update_Notification_Queue(queueId, message, templateId, status, contactInfo, date);
        }

      

        //public void Get_ReferralNotification()
        //{
        //    while (true)
        //    {
        //        if (_service.Check_If_Notification_Inserted())
        //        {
        //            Logger.Log.Info("Found New Notification at_ " + DateTime.Now);
        //            _service = new NotificationService();

        //            notification = _service.Get_ReferralNotification(EventArray);
        //            if (notification != null)
        //            {
        //                if (!string.IsNullOrWhiteSpace(notification.userId))
        //                {
        //                    user_id = notification.userId;
        //                }
        //                //template_type = notification.Event;
        //                Referraltemplate_type = notification.Event;

        //                // notify_template = _service.Get_Notification_Template(template_type);
        //                Referralnotify_template = _service.Get_Notification_Template(Referraltemplate_type);
        //                Send_ReferralNotification();
        //            }
        //            else
        //            {
        //                System.Threading.Thread.Sleep(1000);
        //            }
        //        }
        //    }
        //}

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
