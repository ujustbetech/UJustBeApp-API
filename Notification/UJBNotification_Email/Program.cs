using System;
using System.ServiceProcess;


namespace UJBNotification_Email
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[]
            {
                new EmailSend()
            };
            ServiceBase.Run(ServicesToRun);

           // var s1 = new EmailSend();
           // s1.method1();
            // s1.Check_if_Referral_Below_72_Hours();
            // s1.Check_If_Guest_Reminder();
        }
    }
}
