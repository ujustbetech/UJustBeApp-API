using FollowupService.Services;
using log4net.Config;
using System;
using System.Reflection;
using System.ServiceProcess;
using System.Timers;
using UJBHelper.Common;

namespace FollowupService
{
    public partial class Service1 : ServiceBase
    {
        Timer oneDayTimer = new Timer();
        Timer twoDayTimer = new Timer();
        Timer fiveDayTimer = new Timer();
        Timer sevenDayTimer = new Timer();

        static string _ScheduledConnectedFollowup = "05:00 PM";
        static string _ScheduledDiscussionFollowup = "05:30 PM";
        static string _ScheduledDealFollowup = "06:00 PM";
        static string _ScheduledFinalFollowup = "06:30 PM";
        Notification_Queue nq;
        private NotificationService _service;

        public Service1()
        {
            InitializeComponent();
            _service = new NotificationService();
            XmlConfigurator.Configure();
        }

        protected override void OnStart(string[] args)
        {
            Logger.Log.Info("Service Started at_ " + DateTime.Now);
            oneDayTimer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
            oneDayTimer.Interval = TimeSpan.FromMinutes(1).TotalMilliseconds;
            oneDayTimer.Enabled = true;
        }


        private void OnElapsedTime(object sender, ElapsedEventArgs e)
        {
            try
            {
                string _CurrentTime = DateTime.Now.ToString("hh:mm tt");//string.Format("{0:t}", DateTime.Now);
                if (_CurrentTime == _ScheduledFinalFollowup)
                {
                    SendFollowupNotification();
                }
                //24 hours include (Not Connected, Called But No Response)
                //Check_For_24_Hours_Followup();
                //Check_For_48_Hours_Followup();
                //Check_For_7_Days_Followup();
                //Check_For_5_Days_Followup();
                //   }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(Assembly.GetCallingAssembly().GetName().Name + "\n\t" + ex.ToString());
            }
        }

        public void SendFollowupNotification()
        {

            _service.SendFollowupNotification();


        }

        private void OnElapsedsevenDayTimer(object sender, ElapsedEventArgs e)
        {
            try
            {

                //7 Days include (Work Completed, Received Full & Final Payment, Agreed Percentage Transferred to UJB)
                Check_For_7_Days_Followup();

            }
            catch (Exception ex)
            {
                Logger.Log.Error(Assembly.GetCallingAssembly().GetName().Name + "\n\t" + ex.ToString());
            }
        }

        #region 7DaysFollowup
        public void Check_For_7_Days_Followup()
        {
            string _CurrentTime = DateTime.Now.ToString("hh:mm tt");//string.Format("{0:t}", DateTime.Now);
            if (_CurrentTime == _ScheduledFinalFollowup)
            {
                Logger.Log.Info("start New 7 Days (Work Completed, Received Full & Final Payment, Agreed Percentage Transferred to UJB) Followup at_ " + DateTime.Now);
                if (_service.Check_For_7_Days_Followup())
                {
                    Logger.Log.Info("Found New 7 Days Followup at_ " + DateTime.Now);
                    _service = new NotificationService();

                    Process_7_Days_Notification();
                }
            }
        }

        private void Process_7_Days_Notification()
        {
            var referral = _service.Get_7_Days_Followup();
            nq = new Notification_Queue();
            nq.Add_To_Queue("", referral.referralId, "", "", "new", referral.referralStatus, "", "Email", "UJBAdmin", "");
            //nq.Add_To_Queue("", referral.referralId, "", "", "new", referral.referralStatus, "", "SMS", "UJBAdmin", "");

            nq.Add_To_Queue("", referral.referralId, "", "", "new", referral.referralStatus, "", "Push", "Business", "");
            nq.Add_To_Queue("", referral.referralId, "", "", "new", referral.referralStatus, "", "Email", "Business", "");
            //nq.Add_To_Queue("", referral.referralId, "", "", "new", referral.referralStatus, "", "SMS", "Business", "");

            nq.Add_To_Queue("", referral.referralId, "", "", "new", referral.referralStatus, "", "Push", "Referrer", "");
            nq.Add_To_Queue("", referral.referralId, "", "", "new", referral.referralStatus, "", "Email", "Referrer", "");
            //nq.Add_To_Queue("", referral.referralId, "", "", "new", referral.referralStatus, "", "SMS", "Referrer", "");
            if (!referral.isForSelf)
            {
                nq.Add_To_Queue("", referral.referralId, "", "", "new", referral.referralStatus, "", "Email", "Referred", "");
                //nq.Add_To_Queue("", referral.referralId, "", "", "new", referral.referralStatus, "", "SMS", "Referrerd", "");
            }
        }
        #endregion

        private void OnElapsedfiveDayTimer(object sender, ElapsedEventArgs e)
        {
            try
            {
                //string _CurrentTime = string.Format("{0:t}", DateTime.Now);
                //if (_CurrentTime == _ScheduledDealFollowup)
                //{
                //5 Days include (Deal Won,Received Part Payment & Transferred to UJustBe, Work In Progress)
                Check_For_5_Days_Followup();
                //   }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(Assembly.GetCallingAssembly().GetName().Name + "\n\t" + ex.ToString());
            }
        }

        #region 5DaysFollowup

        private void Check_For_5_Days_Followup()
        {
            string _CurrentTime = DateTime.Now.ToString("hh:mm tt"); //string.Format("{0:t}", DateTime.Now);
            if (_CurrentTime == _ScheduledDealFollowup)
            {
                Logger.Log.Info("start (Deal Closed, Received Part Payment, Work In Progress) 5 Days Followup at_ " + DateTime.Now);
                if (_service.Check_For_5_Days_Followup())
                {
                    Logger.Log.Info("Found New 5 Days Followup at_ " + DateTime.Now);
                    _service = new NotificationService();

                    Process_5_Days_Notification();
                }
            }
        }

        private void Process_5_Days_Notification()
        {
            var referral = _service.Get_5_Days_Followup();
            nq = new Notification_Queue();
            nq.Add_To_Queue("", referral.referralId, "", "", "new", referral.referralStatus, "", "Email", "UJBAdmin", "");
            //nq.Add_To_Queue("", referral.referralId, "", "", "new", referral.referralStatus, "", "SMS", "UJBAdmin", "");

            nq.Add_To_Queue("", referral.referralId, "", "", "new", referral.referralStatus, "", "Push", "Business", "");
            nq.Add_To_Queue("", referral.referralId, "", "", "new", referral.referralStatus, "", "Email", "Business", "");
            //nq.Add_To_Queue("", referral.referralId, "", "", "new", referral.referralStatus, "", "SMS", "Business", "");

            nq.Add_To_Queue("", referral.referralId, "", "", "new", referral.referralStatus, "", "Push", "Referrer", "");
            nq.Add_To_Queue("", referral.referralId, "", "", "new", referral.referralStatus, "", "Email", "Referrer", "");
            //nq.Add_To_Queue("", referral.referralId, "", "", "new", referral.referralStatus, "", "SMS", "Referrer", "");

            if (!referral.isForSelf)
            {
                nq.Add_To_Queue("", referral.referralId, "", "", "new", referral.referralStatus, "", "Email", "Referred", "");
                //nq.Add_To_Queue("", referral.referralId, "", "", "new", referral.referralStatus, "", "SMS", "Referrerd", "");
            }
        }

        #endregion

        private void OnElapsedtwoDayTimer(object sender, ElapsedEventArgs e)
        {
            try
            {
                //string _CurrentTime = string.Format("{0:t}", DateTime.Now);
                //if (_CurrentTime == _ScheduledDiscussionFollowup)
                //{
                //48 hours include (Discussion in Progress)
                Check_For_48_Hours_Followup();
                // }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(Assembly.GetCallingAssembly().GetName().Name + "\n\t" + ex.ToString());
            }
        }

        #region 48HoursRegion

        public void Check_For_48_Hours_Followup()
        {
            string _CurrentTime = DateTime.Now.ToString("hh:mm tt"); //string.Format("{0:t}", DateTime.Now);
            if (_CurrentTime == _ScheduledDiscussionFollowup)
            {
                Logger.Log.Info("start New discussion in progress  48 Hours Followup at_ " + DateTime.Now);
                if (_service.Check_For_48_Hours_Followup())
                {
                    Logger.Log.Info("Found New 48 Hours Followup at_ " + DateTime.Now);
                    _service = new NotificationService();

                    Process_48_Hours_Notification();
                }
            }
        }

        private void Process_48_Hours_Notification()
        {
            var referral = _service.Get_48_Hours_Followup();
            nq = new Notification_Queue();
            nq.Add_To_Queue("", referral.referralId, "", "", "new", referral.referralStatus, "", "Email", "UJBAdmin", "");
            //nq.Add_To_Queue("", referral.referralId, "", "", "new", referral.referralStatus, "", "SMS", "UJBAdmin", "");

            nq.Add_To_Queue("", referral.referralId, "", "", "new", referral.referralStatus, "", "Push", "Business", "");
            nq.Add_To_Queue("", referral.referralId, "", "", "new", referral.referralStatus, "", "Email", "Business", "");
            //nq.Add_To_Queue("", referral.referralId, "", "", "new", referral.referralStatus, "", "SMS", "Business", "");

            nq.Add_To_Queue("", referral.referralId, "", "", "new", referral.referralStatus, "", "Push", "Referrer", "");
            nq.Add_To_Queue("", referral.referralId, "", "", "new", referral.referralStatus, "", "Email", "Referrer", "");
            //nq.Add_To_Queue("", referral.referralId, "", "", "new", referral.referralStatus, "", "SMS", "Referrer", "");
            if (!referral.isForSelf)
            {
                nq.Add_To_Queue("", referral.referralId, "", "", "new", referral.referralStatus, "", "Email", "Referred", "");
                //nq.Add_To_Queue("", referral.referralId, "", "", "new", referral.referralStatus, "", "SMS", "Referrerd", "");
            }
        }
        #endregion



        #region 24HoursFollowup

        private void Check_For_24_Hours_Followup()
        {
            string _CurrentTime = DateTime.Now.ToString("hh:mm tt");//string.Format("{0:t}", DateTime.Now);
            if (_CurrentTime == _ScheduledConnectedFollowup)
            {
                Logger.Log.Info("start New  Not Connected, Called But No Response 24 Hours Followup at_ " + DateTime.Now);
                if (_service.Check_For_24_Hours_Followup())
                {
                    Logger.Log.Info("Found New 24 Hours Followup at_ " + DateTime.Now);
                    _service = new NotificationService();

                    //ProcessNotification();
                }

            }
        }

        private void ProcessNotification()
        {
            try
            {
                var referral = _service.Get_24_Hours_Followup();
                nq = new Notification_Queue();
                nq.Add_To_Queue("", referral.referralId, "", "", "new", referral.referralStatus, "", "Email", "UJBAdmin", "");
                //nq.Add_To_Queue("", referral.referralId, "", "", "new", referral.referralStatus, "", "SMS", "UJBAdmin", "");

                nq.Add_To_Queue("", referral.referralId, "", "", "new", referral.referralStatus, "", "Push", "Business", "");
                nq.Add_To_Queue("", referral.referralId, "", "", "new", referral.referralStatus, "", "Email", "Business", "");
                //nq.Add_To_Queue("", referral.referralId, "", "", "new", referral.referralStatus, "", "SMS", "Business", "");

                nq.Add_To_Queue("", referral.referralId, "", "", "new", referral.referralStatus, "", "Push", "Referrer", "");
                nq.Add_To_Queue("", referral.referralId, "", "", "new", referral.referralStatus, "", "Email", "Referrer", "");
                //nq.Add_To_Queue("", referral.referralId, "", "", "new", referral.referralStatus, "", "SMS", "Referrer", "");
                if (!referral.isForSelf)
                {
                    nq.Add_To_Queue("", referral.referralId, "", "", "new", referral.referralStatus, "", "Email", "Referred", "");
                    //nq.Add_To_Queue("", referral.referralId, "", "", "new", referral.referralStatus, "", "SMS", "Referrerd", "");
                }

            }
            catch (Exception ex)
            {
                Logger.Log.Error(Assembly.GetCallingAssembly().GetName().Name + "\n\t" + ex.ToString());
            }
        }
        #endregion

        protected override void OnStop()
        {
            Logger.Log.Info("Service Stopped at_ " + DateTime.Now);
        }
    }

    public class ReferralDetails
    {
        public string referralId { get; set; }
        public string referralStatus { get; set; }
        public bool isForSelf { get; set; }
    }
}
