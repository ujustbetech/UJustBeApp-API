using Auth.Service.Models.Registeration.EnrollPartner;
using Auth.Service.Respositories.Registeration;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using UJBHelper.Common;
using UJBHelper.DataModel;
using UJBHelper.DataModel.Common;

namespace Auth.Service.Services.Registeration
{
    public class EnrollPartnerService : IEnrollPartnerService
    {
        private readonly IMongoCollection<User> _users;
        private IConfiguration _iconfiguration;
        private readonly IMongoCollection<System_Default> _default;
        public EnrollPartnerService(IConfiguration config)
        {
            _iconfiguration = config;
            //  var client = new MongoClient(DbHelper.GetConnectionString());
            //  var database = client.GetDatabase(DbHelper.GetDatabaseName());
            var client = new MongoClient(_iconfiguration["ConnectionString"]);
            var database = client.GetDatabase(_iconfiguration["Database"]);
            _users = database.GetCollection<User>("Users");
            _default = database.GetCollection<System_Default>("System_Default");
        }

        public void Update_Enrollment(Post_Request request)
        {

            TimeSpan diff = DateTime.Now - DateTime.UtcNow;
            string[] timezone = DateTime.Now.ToString("zzz").Split(new char[] { '+', '-', ':' });

            if (request.Localities.Count < 3)
            {
                request.Localities.Add("");
                request.Localities.Add("");
            }

            //DateTime? dob = null;
            //if (!string.IsNullOrWhiteSpace(request.birthDate))
            //{
            //    dob = Convert.ToDateTime(request.birthDate);
            //}
            if (request.birthDate.HasValue)
            {
                DateTime date1 = ((DateTime)request.birthDate).Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0));
                request.birthDate = date1;
            }

            if (request.mentorCode == null || request.mentorCode == "")
            {
                request.mentorCode = "UJB10122000000001";
            }
            _users.FindOneAndUpdate(
                Builders<User>.Filter.Eq(x => x._id, request.userId),
                Builders<User>.Update.Set(x => x.gender, request.gender)
                .Set(x => x.birthDate, request.birthDate)
                .Set(x => x.address, request.addressInfo)
                .Set(x => x.knowledgeSource, request.knowledgeSource)
                .Set(x => x.mentorCode, request.mentorCode)
                 .Set(x => x.organisationType, request.organisationType)
                .Set(x => x.preferredLocations.location1, request.Localities[0])
                .Set(x => x.preferredLocations.location2, request.Localities[1])
                .Set(x => x.preferredLocations.location3, request.Localities[2])
                 .Set(x => x.userType, request.userType)
                .Set(x => x.passiveIncome, request.passiveIncome)
                .Set(x => x.latitude, request.latitude)
                .Set(x => x.longitude, request.longitude)
                .Set(x => x.countryId, request.countryId)
                .Set(x => x.stateId, request.stateId)
                // .Set(x => x.Role, "Partner")
                );

            if (_users.Find(x => x._id == request.userId && x.myMentorCode == null).CountDocuments() > 0)
            {
                request.myMentorCode = Generate_Mentor_Code(request.countryId, request.stateId);

                _users.FindOneAndUpdate(
                 Builders<User>.Filter.Eq(x => x._id, request.userId),
             Builders<User>.Update
             .Set(x => x.myMentorCode, request.myMentorCode)
             );
                Update_System_Default("UJBCode");
            }
            if (_users.Find(x => x._id == request.userId && x.Role == "Guest").CountDocuments() > 0)
            {
                _users.FindOneAndUpdate(
                 Builders<User>.Filter.Eq(x => x._id, request.userId),
             Builders<User>.Update
             .Set(x => x.Role, "Partner")
             );
            }


        }

        public void Update_EnrollmentDetails(Post_EnrollPartner request)
        {

            TimeSpan diff = DateTime.Now - DateTime.UtcNow;
            string[] timezone = DateTime.Now.ToString("zzz").Split(new char[] { '+', '-', ':' });

            if (request.Localities.Count < 3)
            {
                request.Localities.Add("");
                request.Localities.Add("");
            }
            if (request.mentorCode == null || request.mentorCode == "")
            {
                request.mentorCode = "UJB10122000000001";
            }
            if (request.birthDate.HasValue)
            {
                DateTime date1 = ((DateTime)request.birthDate).Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0));
                request.birthDate = date1;
            }
            _users.FindOneAndUpdate(
                Builders<User>.Filter.Eq(x => x._id, request.userId),
                Builders<User>.Update.Set(x => x.gender, request.gender)
                .Set(x => x.birthDate, request.birthDate)
                // .Set(x => x.middleName, request.MiddleName)
                .Set(x => x.address, request.addressInfo)
                .Set(x => x.knowledgeSource, request.knowledgeSource)
                .Set(x => x.mentorCode, request.mentorCode)
                .Set(x => x.organisationType, request.organisationType)
                .Set(x => x.preferredLocations.location1, request.Localities[0])
                .Set(x => x.preferredLocations.location2, request.Localities[1])
                .Set(x => x.preferredLocations.location3, request.Localities[2])
                 .Set(x => x.userType, request.userType)
                .Set(x => x.passiveIncome, request.passiveIncome)
                .Set(x => x.latitude, request.latitude)
                .Set(x => x.longitude, request.longitude)
                .Set(x => x.countryId, request.countryId)
                .Set(x => x.stateId, request.stateId)
                .Set(x => x.Role, request.Role)
                );

            if (_users.Find(x => x._id == request.userId && x.myMentorCode == null).CountDocuments() > 0)
            {
                request.myMentorCode = Generate_Mentor_Code(request.countryId, request.stateId);

                _users.FindOneAndUpdate(
                 Builders<User>.Filter.Eq(x => x._id, request.userId),
             Builders<User>.Update
             .Set(x => x.myMentorCode, request.myMentorCode)
             );
                Update_System_Default("UJBCode");
            }

            Update_System_Default("UJBCode");
        }

        public bool Check_If_User_Exists(string userId)
        {
            return _users.Find(x => x._id == userId).CountDocuments() > 0;
        }

        public string Get_Coordinates_From_Address(Address addressInfo)
        {
            var location = $"{addressInfo.flatWing.ToLower().Replace(" ", "+")}+{addressInfo.location.ToLower().Replace(" ", "+")}+{addressInfo.locality.ToLower().Replace(" ", "+")}";
            var google_Api_key = Otp_Generator.Get_Google_Api_Key();
            string url = $"https://maps.googleapis.com/maps/api/geocode/json?address={location}&key={google_Api_key}";

            WebRequest request = WebRequest.Create(url);

            WebResponse response = request.GetResponse();

            Stream data = response.GetResponseStream();

            StreamReader reader = new StreamReader(data);

            // json-formatted string from maps api
            string responseFromServer = reader.ReadToEnd();


            var google_response = new Google_Location_To_Coordinates_Response();

            google_response = JsonConvert.DeserializeObject<Google_Location_To_Coordinates_Response>(responseFromServer);

            response.Close();

            if (google_response.status == "OK")
            {
                return google_response.results[0].geometry.location.lat + "," + google_response.results[0].geometry.location.lng;
            }
            return "NONE";

        }

        public string Get_Current_Role(string userId)
        {
            return _users.Find(x => x._id == userId).Project(x => x.Role).FirstOrDefault();
        }

        public string Generate_Mentor_Code(int CountryCode, int StateCode)
        {
            TimeSpan diff = DateTime.Now - DateTime.UtcNow;
            string[] timezone = DateTime.Now.ToString("zzz").Split(new char[] { '+', '-', ':' });

            int UJBCodeNoLength = 0;

            var UJBNoCounter = "";
            var UJBCode = "";

            DateTime CurrentDate = DateTime.Today.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0));
            //CurrentDate = new DateTime(CurrentDate.Year, CurrentDate.Month, CurrentDate.Day);

            var filter = Builders<System_Default>.Filter.Lte(x => x.startDate, CurrentDate);
            filter = filter & Builders<System_Default>.Filter.Gte(x => x.endDate, CurrentDate);
            filter = filter & Builders<System_Default>.Filter.Eq(x => x.Default_Name, "UJBCode");


            UJBNoCounter = _default.Find(filter).FirstOrDefault().Default_Value;
            UJBCodeNoLength = UJBNoCounter.Length;
            UJBCode = "UJB" + CountryCode + StateCode;

            if (UJBCodeNoLength == 1)
            {
                UJBCode = UJBCode + "00000000" + int.Parse(UJBNoCounter);
            }
            else if (UJBCodeNoLength == 2)
            {
                UJBCode = UJBCode + "0000000" + int.Parse(UJBNoCounter);
            }
            else if (UJBCodeNoLength == 3)
            {
                UJBCode = UJBCode + "000000" + int.Parse(UJBNoCounter);
            }
            else if (UJBCodeNoLength == 4)
            {
                UJBCode = UJBCode + "00000" + int.Parse(UJBNoCounter);
            }
            else if (UJBCodeNoLength == 5)
            {
                UJBCode = UJBCode + "0000" + int.Parse(UJBNoCounter);
            }
            else if (UJBCodeNoLength == 6)
            {
                UJBCode = UJBCode + "000" + int.Parse(UJBNoCounter);
            }
            else if (UJBCodeNoLength == 7)
            {
                UJBCode = UJBCode + "00" + int.Parse(UJBNoCounter);
            }
            else if (UJBCodeNoLength == 8)
            {
                UJBCode = UJBCode + "0" + int.Parse(UJBNoCounter);
            }
            else
            {
                UJBCode = UJBCode + int.Parse(UJBNoCounter);
            }
            return UJBCode;
        }

        public void Update_System_Default(string Default_Name)
        {

            TimeSpan diff = DateTime.Now - DateTime.UtcNow;
            string[] timezone = DateTime.Now.ToString("zzz").Split(new char[] { '+', '-', ':' });

            DateTime CurrentDate = DateTime.Today.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0));
            // CurrentDate = new DateTime(CurrentDate.Year, CurrentDate.Month, CurrentDate.Day);

            var filter = Builders<System_Default>.Filter.Lte(x => x.startDate, CurrentDate);
            filter = filter & Builders<System_Default>.Filter.Gte(x => x.endDate, CurrentDate);
            filter = filter & Builders<System_Default>.Filter.Eq(x => x.Default_Name, Default_Name);

            int value = int.Parse(_default.Find(filter).FirstOrDefault().Default_Value);
            value = value + 1;
            _default.FindOneAndUpdate(
                Builders<System_Default>.Filter.Eq(x => x.Default_Name, Default_Name),
               Builders<System_Default>.Update
               .Set(x => x.Default_Value, value.ToString())
               );
        }


    }
}
