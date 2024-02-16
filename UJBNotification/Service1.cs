using log4net.Config;
using System;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Timers;
using UJBHelper.Common;
using UJBHelper.DataModel;
using UJBNotification.Services;
using System.Collections.Generic;

namespace UJBNotification
{
    public partial class Service1 : ServiceBase
    {
        Timer timer = new Timer(); // name space(using System.Timers;)  
        Timer AutoDeclineTimer = new Timer();
        Timer GuestReminder = new Timer();
        Timer DeclineReminder = new Timer();
        Timer SkipKYCReminder = new Timer();      
        Timer IncompleteProfileReminder = new Timer();

        static string _ScheduledRunningTime = "08:30 AM";
        static string _ScheduledGuestReminder = "08:00 AM";
        static string _ScheduledKYCReminder = "09:00 AM";
        static string _ScheduledIncompleteReminder = "10:00 AM";
        Timer CPProductReminder = new Timer();

        private NotificationService _service;
      
      
        public Service1()
        {
            InitializeComponent();
            _service = new NotificationService();
            XmlConfigurator.Configure();           
            
        }

        protected override void OnStart(string[] args)
        {
             AutoDeclineTimer.Elapsed += new ElapsedEventHandler(OnElapsedAutoDeclineTime);
            AutoDeclineTimer.Interval = TimeSpan.FromMinutes(1).TotalMilliseconds;
            AutoDeclineTimer.Enabled = true;

            GuestReminder.Elapsed += new ElapsedEventHandler(OnElapsedGuestReminder);
            GuestReminder.Interval = TimeSpan.FromMinutes(1).TotalMilliseconds;
            GuestReminder.Enabled = true;

            DeclineReminder.Elapsed += new ElapsedEventHandler(OnElapsedDeclineReminder);
            DeclineReminder.Interval = TimeSpan.FromMinutes(1).TotalMilliseconds;
            DeclineReminder.Enabled = true;

            SkipKYCReminder.Elapsed += new ElapsedEventHandler(OnElapsedSkipKYCReminder);
            SkipKYCReminder.Interval = TimeSpan.FromMinutes(1).TotalMilliseconds;
            SkipKYCReminder.Enabled = true;

            CPProductReminder.Elapsed += new ElapsedEventHandler(OnElapsedCPProductReminder);
            CPProductReminder.Interval = TimeSpan.FromMinutes(1).TotalMilliseconds;
            CPProductReminder.Enabled = true;

            IncompleteProfileReminder.Elapsed += new ElapsedEventHandler(OnElapsedIncompleteProfileReminder);
            IncompleteProfileReminder.Interval = TimeSpan.FromMinutes(1).TotalMilliseconds;
            IncompleteProfileReminder.Enabled = true;           

            //referral auto decline timer
        }

        private void OnElapsedIncompleteProfileReminder(object sender, ElapsedEventArgs e)
        {
            try
            {
               
                string _CurrentTime = DateTime.Now.ToString("hh:mm tt");//string.Format("{0:t}", DateTime.Now);
                if (_CurrentTime == _ScheduledIncompleteReminder)
                {
                    CheckIfIncompleteProfile();
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(Assembly.GetCallingAssembly().GetName().Name + "\n\t" + ex.ToString());
            }
        }

        public void CheckIfIncompleteProfile()
        {
            Logger.Log.Info("start Incomplete profile at_ " + DateTime.Now);
            if (_service.Check_If_Profile_Incomplete())
            {
                Logger.Log.Info("Found New Incomplete Profile Reminder at_ " + DateTime.Now);
                _service = new NotificationService();

                //ProcessIncompleteProfileReminder();
            }
        }

    

        #region SubscriptionFee Reminder
        public void Check_If_SubscriptionFee_Paid()
        {
            if (_service.Check_If_Susbscription_Pending_48Hrs())
            {
                Logger.Log.Info("Found New Subscription Fee Reminder at_ " + DateTime.Now);
                _service = new NotificationService();               
            }
        }
        #endregion

        

        #region Meeting Reminder
        public void Check_If_MeetingFee_Paid()
        {
            if (_service.Check_If_Meeting_Pending_48Hrs())
            {
                Logger.Log.Info("Found New Meeting Fee Reminder at_ " + DateTime.Now);
                _service = new NotificationService();               
            }
        }
        #endregion

        private void OnElapsedCPProductReminder(object sender, ElapsedEventArgs e)
        {
            try
            {
                string _CurrentTime = DateTime.Now.ToString("hh:mm tt");//string.Format("{0:t}", DateTime.Now);
                if (_CurrentTime == _ScheduledRunningTime)
                {
                    Check_If_No_Products();
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(Assembly.GetCallingAssembly().GetName().Name + "\n\t" + ex.ToString());
            }
        }

        #region NoProductService
        private void Check_If_No_Products()
        {
            Logger.Log.Info("start New Add Product/Service Reminder at_ " + DateTime.Now);
            if (_service.Check_If_No_Products())
            {
                Logger.Log.Info("Found New Add Product/Service Reminder at_ " + DateTime.Now);
                _service = new NotificationService();

                //ProcessNoProductReminder();
            }
        }

       
        #endregion

        private void OnElapsedSkipKYCReminder(object sender, ElapsedEventArgs e)
        {
            try
            {
                string _CurrentTime = DateTime.Now.ToString("hh:mm tt");//string.Format("{0:t}", DateTime.Now);
                if (_CurrentTime == _ScheduledKYCReminder)
                {
                    Check_If_KYC_Pending();
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(Assembly.GetCallingAssembly().GetName().Name + "\n\t" + ex.ToString());
            }
        }

        #region KYCPending
        public void Check_If_KYC_Pending()
        {
            Logger.Log.Info("start New KYC Pending Reminder at_ " + DateTime.Now);
            if (_service.Check_If_KYC_Pending())
            {
                Logger.Log.Info("Found New KYC Pending Reminder at_ " + DateTime.Now);
                _service = new NotificationService();

                //ProcessKYCPendingReminder();
            }
        }

    
        #endregion

        private void OnElapsedDeclineReminder(object sender, ElapsedEventArgs e)
        {
            try
            {
                Check_if_Referral_Below_72_Hours();
            }
            catch (Exception ex)
            {
                Logger.Log.Error(Assembly.GetCallingAssembly().GetName().Name + "\n\t" + ex.ToString());
            }
        }

        #region RejectionReminder
        public void Check_if_Referral_Below_72_Hours()
        {
            if (_service.Check_if_Referral_Below_72_Hours())
            {
                Logger.Log.Info("Found New Referral Rejection Reminder at_ " + DateTime.Now);
              
            }
        }

      
        #endregion

        private void OnElapsedGuestReminder(object sender, ElapsedEventArgs e)
        {
            try
            {
                   string _CurrentTime = DateTime.Now.ToString("hh:mm tt");//string.Format("{0:t}", DateTime.Now);
                if (_CurrentTime == _ScheduledGuestReminder)
                {
                    Check_If_Guest_Reminder();
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(Assembly.GetCallingAssembly().GetName().Name + "\n\t" + ex.ToString());
            }
        }

        #region GuestReminder
        public void Check_If_Guest_Reminder()
        {
            Logger.Log.Info("start New Guest Reminder at_ " + DateTime.Now);
            if (_service.Check_If_Guest_Reminder())
            {
                Logger.Log.Info("Found New Guest Reminder at_ " + DateTime.Now);
                _service = new NotificationService();

                //ProcessGuestReminder();
            }
        }

       
        #endregion

        private void OnElapsedAutoDeclineTime(object sender, ElapsedEventArgs e)
        {
            try
            {
                Check_If_Referral_Crossed_72_Hours();
            }
            catch (Exception ex)
            {
                Logger.Log.Error(Assembly.GetCallingAssembly().GetName().Name + "\n\t" + ex.ToString());
            }
        }

        #region AutoRejection
        public void Check_If_Referral_Crossed_72_Hours()
        {
            if (_service.Check_If_Referral_Crossed_72_Hours())
            {
                Logger.Log.Info("Found New Referral OverDue at_ " + DateTime.Now);
                _service = new NotificationService();

                //ProcessAutoRejection();
            }
        }

        #endregion

      

        

        protected override void OnStop()
        {
            Logger.Log.Info("Service Stopped at_ " + DateTime.Now);
        }
    }

}
