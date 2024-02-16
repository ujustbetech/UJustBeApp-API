using log4net.Config;
using System;
using System.Data;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Timers;
using UJBBirthdayService.Services;
using UJBHelper.Common;
using UJBHelper.DataModel;

namespace UJBBirthdayService
{
    public partial class Service1 : ServiceBase
    {
        Timer timer = new Timer(); // name space(using System.Timers;)  
        static string _ScheduledRunningTime = "11:00 AM";
        private BirthdayService _service;
        private Notification notify_template;
        private string email_Id;
        private string mobile_number;
        private string user_name;
        private string user_Id;

        public Service1()
        {
            InitializeComponent();
            XmlConfigurator.Configure();
        }

        protected override void OnStart(string[] args)
        {
            Logger.Log.Info("Birthday Service Started at_ " + DateTime.Now);
            timer.Elapsed += new ElapsedEventHandler(OnElapsedTime);
            timer.Enabled = true;
            timer.Interval = TimeSpan.FromMinutes(1).TotalMilliseconds;//Every one minute
        }

        private void OnElapsedTime(object sender, ElapsedEventArgs e)
        {
            try
            {
                string _CurrentTime = DateTime.Now.ToString("hh:mm tt");//string.Format("{0:t}", DateTime.Now);
                if (_CurrentTime == _ScheduledRunningTime)
                {
                    Check_If_Birthday();
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(Assembly.GetCallingAssembly().GetName().Name + "\n\t" + ex.ToString());
            }
        }

        internal void Check_If_Birthday()
        {
            _service = new BirthdayService();

            if (_service.Check_If_Birthday())
            {
                Logger.Log.Info("Found New Birthday at_ " + DateTime.Now);
                //ProcessNotification();
            }
        }

        protected override void OnStop()
        {
            Logger.Log.Info("Birthday Service Stopped at_ " + DateTime.Now);

        }
    }
}
