using log4net.Config;
using System;
using System.Data;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Timers;
using SusbscriptionService.Services;
using UJBHelper.Common;
using UJBHelper.DataModel;


namespace Susbscription
{
    public partial class Service1 : ServiceBase
    {
        Timer timer = new Timer(); // name space(using System.Timers;)  
        static string _ScheduledRunningTime = "06:00 AM";
        private SubscriptionService _service;
        public Service1()
        {
            InitializeComponent();
            XmlConfigurator.Configure();
        }

        protected override void OnStart(string[] args)
        {
            Logger.Log.Info("Subscription Service Started at_ " + DateTime.Now);
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
                    Check_If_Subscription_Active();
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(Assembly.GetCallingAssembly().GetName().Name + "\n\t" + ex.ToString());
            }
        }

        internal void Check_If_Subscription_Active()
        {
            _service = new SubscriptionService();
            _service.Check_If_Subscription_Active();
        }

        protected override void OnStop()
        {
            Logger.Log.Info("Subscription Service Stopped at_ " + DateTime.Now);

        }
    }
}
