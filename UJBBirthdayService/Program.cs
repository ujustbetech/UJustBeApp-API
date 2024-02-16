using System.ServiceProcess;

namespace UJBBirthdayService
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

           //// var s1 = new Service1();
          //  s1.Check_If_Birthday();
        }
    }
}
