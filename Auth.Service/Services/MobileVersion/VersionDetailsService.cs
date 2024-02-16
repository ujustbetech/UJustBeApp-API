using Auth.Service.Models.MobileVersion;
using Auth.Service.Respositories.MobileVersion;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System.Linq;
using UJBHelper.DataModel;

namespace Auth.Service.Services.MobileVersion
{
    public class VersionDetailsService: IGetVersionDetails
    {
        private readonly IMongoCollection<MobileAppUpdates> _versionDetails;
        private IConfiguration _iconfiguration;
        public VersionDetailsService(IConfiguration config)
        {
            _iconfiguration = config;
            //  var client = new MongoClient(DbHelper.GetConnectionString());
            //  var database = client.GetDatabase(DbHelper.GetDatabaseName());
            var client = new MongoClient(_iconfiguration["ConnectionString"]);
            var database = client.GetDatabase(_iconfiguration["Database"]);
            _versionDetails = database.GetCollection<MobileAppUpdates>("MobileAppUpdates");
        }

        public Get_Request GetVersionDetails(string Type)
        {
            var res = new Get_Request();
            var filter = Builders<MobileAppUpdates>.Filter.Empty;
            if (Type == "Android")
            {
                res.Android = _versionDetails.Find(filter).Project(x =>
                     new AndroidInfo { androidVersion = x.androidVersion, isAndroidForce = x.isAndroidForce }).FirstOrDefault();
            }
            else
            {
                res.IOS = _versionDetails.Find(filter).Project(x =>
                     new IOSInfo { iosVersion = x.iosVersion, isIOSForce = x.isIOSForce }).FirstOrDefault();
            }           
           
            return res;
        }
    }
}
