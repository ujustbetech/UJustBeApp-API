using System.ServiceProcess;

namespace UJBNotification
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new Service1()
            };
            ServiceBase.Run(ServicesToRun);

         //   var s1 = new Service1();
            //s1.Check_If_KYC_Pending();
          //  s1.Check_if_Referral_Below_72_Hours();
            //s1.Check_If_Guest_Reminder();
        }
    }
}
