using Business.Service.Repositories.Company;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Newtonsoft.Json;
using System.IO;
using System.Net;
using UJBHelper.Common;
using UJBHelper.DataModel;
using UJBHelper.DataModel.Common;
using Post_Request = Business.Service.Models.Company.UpdateCompany.Post_Request;

namespace Business.Service.Services.Company
{
    public class UpdateCompanyService : IUpdateCompanyService
    {
        private readonly IMongoCollection<BusinessDetails> _businessDetails;
        private IConfiguration _iconfiguration;
        public UpdateCompanyService(IConfiguration config)
        {
            // var client = new MongoClient(DbHelper.GetConnectionString());
            // var database = client.GetDatabase(DbHelper.GetDatabaseName());
            _iconfiguration = config;
            var client = new MongoClient(_iconfiguration["ConnectionString"]);
            var database = client.GetDatabase(_iconfiguration["Database"]);
            _businessDetails = database.GetCollection<BusinessDetails>("BussinessDetails");
        }

        public bool Check_If_Business_Exists(string businessId)
        {
            return _businessDetails.Find(x => x.Id == businessId).CountDocuments() > 0;
        }

        public bool UpdateBusinessDescription(Post_Request request)
        {
            bool bussinessupdate = false;
            var _bussdeatails = _businessDetails.Find(x => x.Id == request.businessId).Project(x =>
                   new BusinessDetails
                   {
                       BusinessDescription = x.BusinessDescription,
                       
                   }).FirstOrDefault();

            if (_bussdeatails.BusinessDescription != request.value)
            {
                bussinessupdate = true;
            }
           
            _businessDetails.FindOneAndUpdate(
                Builders<BusinessDetails>.Filter.Eq(x => x.Id, request.businessId),
                Builders<BusinessDetails>.Update.Set(x => x.BusinessDescription, request.value));
            return bussinessupdate;
        }

        public void UpdateNameOfPartner(Post_Request request)
        {
            _businessDetails.FindOneAndUpdate(
                Builders<BusinessDetails>.Filter.Eq(x => x.Id, request.businessId),
                Builders<BusinessDetails>.Update.Set(x => x.NameOfPartner, request.value));
        }


        public void UpdateBusinessEmail(Post_Request request)
        {
            _businessDetails.FindOneAndUpdate(
                Builders<BusinessDetails>.Filter.Eq(x => x.Id, request.businessId),
                Builders<BusinessDetails>.Update.Set(x => x.BusinessEmail, request.value));
        }

        public void UpdateBusinessUrl(Post_Request request)
        {
            _businessDetails.FindOneAndUpdate(
                Builders<BusinessDetails>.Filter.Eq(x => x.Id, request.businessId),
                Builders<BusinessDetails>.Update.Set(x => x.WebsiteUrl, request.value));
        }

        public void UpdateCompanyName(Post_Request request)
        {
            _businessDetails.FindOneAndUpdate(
                Builders<BusinessDetails>.Filter.Eq(x => x.Id, request.businessId),
                Builders<BusinessDetails>.Update.Set(x => x.CompanyName, request.value));
        }

        public void UpdateBusinessGst(Post_Request request)
        {
            _businessDetails.FindOneAndUpdate(
               Builders<BusinessDetails>.Filter.Eq(x => x.Id, request.businessId),
               Builders<BusinessDetails>.Update.Set(x => x.GSTNumber, request.value));
        }

        public void UpdateCompanyLogo(Models.Company.UpdateCompanyLogo.Post_Request request)
        {
            _businessDetails.FindOneAndUpdate(
                Builders<BusinessDetails>.Filter.Eq(x => x.Id, request.businessId),
                Builders<BusinessDetails>.Update.Set(x => x.Logo, new Logo
                {
                    logoImageURL = request.logoImageURL,
                    logoImageName = request.logoImgName,
                    logoUniqueName = request.logoUniqueName
                }));
        }

        public void UpdateCompanyAddress(Models.Company.UpdateCompanyAddress.Post_Request request)
        {
            _businessDetails.FindOneAndUpdate(
               Builders<BusinessDetails>.Filter.Eq(x => x.Id, request.businessId),
               Builders<BusinessDetails>.Update.Set(x => x.BusinessAddress, new BusinessAddress
               {
                   Flat_Wing = request.flatWing,
                   Locality = request.locality,
                   Location = request.location
               })
               .Set(x=>x.latitude,request.latitude)
               .Set(x=>x.longitude,request.longitude));
        }

        public string Get_Coordinates_From_Address(string  flatWing, string companylocation, string locality)
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
    }
}
