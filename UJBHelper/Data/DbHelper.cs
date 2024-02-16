namespace UJBHelper.Data
{
    public class DbHelper
    {
        public static string GetConnectionString()
        {

        // return "mongodb://192.168.0.210:27017"; // local ais2

         return "mongodb://localhost:27017";

          
        }

        public static string GetDatabaseName()
        {
            return "UJB";
        }

        //case "AIS_SERVER":
        //           connectionString = "mongodb://192.168.0.101:27017";
        //           "UJB_DIT"
        //           break;
        //       case "YASH_LOCAL":
        //           connectionString = "";
        //           break;
        //       case "UJB_UAT":
        //           connectionString = "mongodb://192.169.201.117:27017";
        //           "UJB_UAT"
        //           break;
        //       case "UJB_PROD":
        //           connectionString = "";
        //           break;
    }
}
