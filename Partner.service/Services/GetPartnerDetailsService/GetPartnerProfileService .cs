using MongoDB.Driver;
using System;
using Microsoft.Extensions.Configuration;
using Partner.Service.Models.PartnerDetails.PartnerProfile;
using MongoDB.Bson;
using UJBHelper.DataModel;
using Partner.Service.Repositories.PartnerDetails;
using static UJBHelper.Common.Common;
using UJBHelper.Common;

namespace Partner.Service.Services.GetPartnerProfileService
{
    public class GetPartnerProfileService : IGetPartnerProfile
    {
        private readonly IMongoCollection<User> _users;
        private readonly IMongoCollection<UserOtherDetails> _userOtherDetails;
        private readonly IMongoCollection<CountryInfo> _countryCode;
        private readonly IMongoCollection<StateInfo> _states;
        private IConfiguration _iconfiguration;
   
        public GetPartnerProfileService(IConfiguration config)
        {
            _iconfiguration = config;
            //  var client = new MongoClient(DbHelper.GetConnectionString());
            //  var database = client.GetDatabase(DbHelper.GetDatabaseName());
            var client = new MongoClient(_iconfiguration["ConnectionString"]);
            var database = client.GetDatabase(_iconfiguration["Database"]);
            _users = database.GetCollection<User>("Users");
            _userOtherDetails = database.GetCollection<UserOtherDetails>("UsersOtherDetails");
           
            _countryCode = database.GetCollection<CountryInfo>("CountryCode");
            _states = database.GetCollection<StateInfo>("States");
         
        }

      
        public bool Check_If_User_Exist(string UserId)
        {
            return _users.Find(x => x._id == UserId).CountDocuments() > 0;
        }

        public bool Check_If_User_IsActive(string UserId)
        {
            return _users.Find(x => x._id == UserId & x.isActive == true).CountDocuments() > 0;
        }

        public Get_Request GetPartnerProfile(String UserId)
        {
            var res = new Get_Request();
            var UserRole = _users.Find(x => x._id == UserId).FirstOrDefault().Role.ToString();
            var filter = Builders<User>.Filter.Eq(x => x._id, UserId);
          
            var filter2 = Builders<UserOtherDetails>.Filter.Eq(x => x.UserId, UserId);

            res.userInfo = _users.Find(filter).FirstOrDefault();
           
            res.userOtherDetails = _userOtherDetails.Find(filter2).FirstOrDefault();
            res.countryName = _countryCode.Find(x => x.countryId == res.userInfo.countryId).Project(x => x.countryName).FirstOrDefault();
            res.stateName = _states.Find(x => x.stateId == res.userInfo.stateId).Project(x => x.stateName).FirstOrDefault();
           
            return res;
        }

    }
}
