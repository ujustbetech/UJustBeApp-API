using log4net;

namespace UJBHelper.Common
{
    public static class Logger
    {
        public static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        
    }
}
