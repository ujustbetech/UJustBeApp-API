using Business.Service.Models.Company.UpdateBusiness;
using Business.Service.Repositories.Company;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using UJBHelper.DataModel;
using static UJBHelper.Common.Common;
using System.IO;
using System.Net;
using UJBHelper.Common;
using UJBHelper.DataModel.Common;
using Newtonsoft.Json;

namespace Business.Service.Services.Company
{
    public class UpdateBusinessService : IUpdateBusinessService
    {
        private readonly IMongoCollection<BusinessDetails> _businessDetails;
        private readonly IMongoCollection<User> _users;
        private IConfiguration _iconfiguration;
        public UpdateBusinessService(IConfiguration config)
        {
            // var client = new MongoClient(DbHelper.GetConnectionString());
            //var database = client.GetDatabase(DbHelper.GetDatabaseName());
            _iconfiguration = config;
            var client = new MongoClient(_iconfiguration["ConnectionString"]);
            var database = client.GetDatabase(_iconfiguration["Database"]);
            _businessDetails = database.GetCollection<BusinessDetails>("BussinessDetails");
            _users = database.GetCollection<User>("Users");
        }

        public bool Check_If_Business_Exists(string businessId, string userId)
        {
            return _businessDetails.Find(x => x.Id == businessId && x.UserId == userId).CountDocuments() > 0;
        }

        public bool Check_If_User_Exists(string userId)
        {
            return _users.Find(x => x._id == userId).CountDocuments() > 0;
        }
        public string Check_If_User_Bussiness_Exists(string userId)
        {
            if (_businessDetails.Find(x => x.UserId == userId).CountDocuments() > 0)
            {
                return _businessDetails.Find(x => x.UserId == userId).Project(x => x.Id).FirstOrDefault();
            }
            else
            {
                return "";
            }
        }

        public List<Get_Request> Get_Business_List()
        {
            List<Get_Request> _bsnsDetails = new List<Get_Request>();
            _bsnsDetails = _businessDetails.Find(x => true).Project(x => new Get_Request
            {
                BusinessDescription = x.BusinessDescription,
                BusinessEmail = x.BusinessEmail,
                businessId = x.Id,
                categories = x.Categories,
                companyName = x.CompanyName,
                createdBy = x.Created.created_By,
                userId = x.UserId,
                GSTNumber = x.GSTNumber,
                Logo = x.Logo,
                Address = x.BusinessAddress,
                Pan = x.BusinessPan,
                tagLine = x.Tagline,
                WebsiteUrl = x.WebsiteUrl,
                UserTypeId = x.UserType,
                NameOfPartner = x.NameOfPartner
            }).ToList();

            foreach (var item in _bsnsDetails)
            {
                item.UserType = Enum.GetName(typeof(UserType), item.UserTypeId).Replace("_", "/");
            }

            return _bsnsDetails;
        }

        public void Insert_Business_Admin(Put_Request request)
        {
            TimeSpan diff = DateTime.Now - DateTime.UtcNow;
            string[] timezone = DateTime.Now.ToString("zzz").Split(new char[] { '+', '-', ':' });

            var b = new BusinessDetails
            {
                BusinessAddress = new BusinessAddress
                {
                    Flat_Wing = request.Flat_Wing,
                    Locality = request.Locality,
                    Location = request.Location
                },
                BusinessDescription = request.BusinessDescription,
                BusinessEmail = request.BusinessEmail,
                BusinessPan = new BusinessPan
                {
                    FileName = request.FileName,
                    ImageURL = request.ImageURL,
                    UniqueName = request.UniqueName,
                    //PanImageType = request.PanImageType,
                    PanNumber = request.PanNumber
                },
                Categories = request.categories,
                CompanyName = request.companyName,
                Created = new Created
                {
                    created_By = request.createdBy,
                    created_On = DateTime.Now.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0))
                },
                GSTNumber = request.GSTNumber,
                Logo = new Logo
                {
                    logoImageURL = request.logoImageURL,
                    logoImageName = request.logoImageName,
                    logoUniqueName = request.logoUniqueName

                },
                Tagline = request.tagLine,
                Updated = new Updated(),
                UserId = request.userId,
                WebsiteUrl = request.WebsiteUrl,
                latitude = 0.0,
                longitude = 0.0,
                averageRating = 3.0,
                isApproved = new Approved(),
                isSubscriptionActive = false,
                UserType = request.UserType,
                NameOfPartner = request.NameofPartner
            };

            _businessDetails.InsertOne(b);
        }

        public string Insert_Business_Categories(Post_Request request)
        {
            TimeSpan diff = DateTime.Now - DateTime.UtcNow;
            string[] timezone = DateTime.Now.ToString("zzz").Split(new char[] { '+', '-', ':' });

            var bd = new BusinessDetails
            {
                UserId = request.userId,
                Tagline = request.tagline,
                Categories = request.categories,
                BusinessAddress = new BusinessAddress
                {
                    Flat_Wing = request.flatWing,
                    Locality = request.locality,
                    Location = request.location
                },
                BusinessPan = new BusinessPan(),
                Created = new Created { created_On = DateTime.Now.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0)), created_By = request.userId },
                Updated = new Updated(),
                Logo = new Logo(),
                latitude = request.latitude,
                longitude = request.longitude,
                averageRating = 3.0,
                isApproved = new Approved(),
                isSubscriptionActive = false,
                UserType = request.UserType,
                NameOfPartner = request.NameofPartner,
                CompanyName = request.CompanyName,
                BusinessDescription=request.BusinessDescription
            };

            _businessDetails.InsertOne(bd);


            _users.FindOneAndUpdate(
                Builders<User>.Filter.Eq(x => x._id, request.userId),
                Builders<User>.Update.Set(x => x.Role, "Listed Partner")
                );

            return bd.Id;
        }

        public void Update_Business_Admin(Put_Request request)
        {
            TimeSpan diff = DateTime.Now - DateTime.UtcNow;
            string[] timezone = DateTime.Now.ToString("zzz").Split(new char[] { '+', '-', ':' });

            var filter = Builders<BusinessDetails>.Filter.Eq(x => x.Id, request.businessId) & Builders<BusinessDetails>.Filter.Eq(x => x.UserId, request.userId);
            _businessDetails.FindOneAndUpdate(
            filter,
                Builders<BusinessDetails>.Update
                .Set(x => x.BusinessAddress, new BusinessAddress
                {
                    Flat_Wing = request.Flat_Wing,
                    Locality = request.Locality,
                    Location = request.Location
                })
                .Set(x => x.BusinessDescription, request.BusinessDescription)
                .Set(x => x.BusinessEmail, request.BusinessEmail)
                .Set(x => x.BusinessPan, new BusinessPan
                {
                    //PanBase64 = request.PanBase64,
                    //PanImageType = request.PanImageType,
                    FileName = request.FileName,
                    ImageURL = request.ImageURL,
                    UniqueName = request.UniqueName,
                    PanNumber = request.PanNumber
                })
                .Set(x => x.Categories, request.categories)
                .Set(x => x.CompanyName, request.companyName)
                .Set(x => x.Updated, new Updated { updated_By = request.createdBy, updated_On = DateTime.Now.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0)) })
                .Set(x => x.GSTNumber, request.GSTNumber)
                .Set(x => x.Logo, new Logo
                {
                    logoImageURL = request.logoImageURL,
                    logoImageName = request.logoImageName,
                    logoUniqueName = request.logoUniqueName
                })
                 .Set(x => x.Tagline, request.tagLine)
                 .Set(x => x.UserType, request.UserType)
                 .Set(x => x.NameOfPartner, request.NameofPartner)
                 //.Set(x => x.UserId, request.userId)
                 .Set(x => x.WebsiteUrl, request.WebsiteUrl)
                );
        }

        public Post_Request Update_Business_Categories(Post_Request request)
        {
            Post_Request _postresult = new Post_Request();
            var _bussdeatails = _businessDetails.Find(x => x.Id == request.businessId).Project(x =>
                     new BusinessDetails
                     {
                         Categories = x.Categories,
                         CompanyName = x.CompanyName,
                         UserType = x.UserType,
                         NameOfPartner = x.NameOfPartner,
                         Tagline = x.Tagline,
                         BusinessDescription = x.BusinessDescription,
                         BusinessAddress = new BusinessAddress
                         {
                             Flat_Wing = x.BusinessAddress.Flat_Wing,
                             Locality = x.BusinessAddress.Locality,
                             Location = x.BusinessAddress.Location
                         }
                     }).FirstOrDefault();


            if (_bussdeatails.BusinessAddress.Flat_Wing != request.flatWing)
            {
                _postresult.flatWing = request.flatWing;
            }
            if (_bussdeatails.BusinessAddress.Locality != request.locality)
            {
                _postresult.locality = request.locality;
            }
            if (_bussdeatails.BusinessAddress.Location != request.location)
            {
                _postresult.location = request.location;
            }
            if (_bussdeatails.BusinessAddress.Location != request.location)
            {
                _postresult.location = request.location;
            }
            if (_bussdeatails.BusinessDescription != request.BusinessDescription)
            {
                _postresult.BusinessDescription = request.BusinessDescription;
            }
            _postresult.businessId = request.businessId;

            _businessDetails.FindOneAndUpdate(Builders<BusinessDetails>.Filter.Eq(x => x.Id, request.businessId),
                Builders<BusinessDetails>.Update.Set(x => x.UserId, request.userId)
                .Set(x => x.Categories, request.categories)
                 .Set(x => x.CompanyName, request.CompanyName)
                  .Set(x => x.UserType, request.UserType)
                   .Set(x => x.NameOfPartner, request.NameofPartner)
                .Set(x => x.Tagline, request.tagline)
                .Set(x => x.BusinessAddress, new BusinessAddress
                {
                    Flat_Wing = request.flatWing,
                    Locality = request.locality,
                    Location = request.location
                })
               .Set(x => x.latitude, request.latitude)
               .Set(x => x.longitude, request.longitude)
                .Set(x => x.BusinessDescription, request.BusinessDescription));

            if (_users.Find(x => x.Role == "Partner" && x._id == request.userId).CountDocuments() > 0)
            {
                _users.FindOneAndUpdate(
                 Builders<User>.Filter.Eq(x => x._id, request.userId),
                 Builders<User>.Update.Set(x => x.Role, "Listed Partner")
                 );
            }
            //  return request.businessId;
            return _postresult;
        }

        public string Get_Coordinates_From_Address(string flatWing, string companylocation, string locality)
        {

            var location = $"{flatWing.ToLower().Replace(" ", "+")}+{companylocation.ToLower().Replace(" ", "+")}+{locality.ToLower().Replace(" ", "+")}";
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
        public string UpdateBanner(Business.Service.Models.Company.UpdateBusiness.UpdateBanner.Put_Request request)
        {
            _businessDetails.FindOneAndUpdate(Builders<BusinessDetails>.Filter.Eq(x => x.Id, request.BusinessId),
                Builders<BusinessDetails>.Update.Set(x => x.BannerDetails, new Banner
                {
                    ImageName = request.FileName,
                    ImageURL = request.URL,
                    UniqueName = request.UniqueFileName
                }));

            return request.UniqueFileName;
        }
    }
}
