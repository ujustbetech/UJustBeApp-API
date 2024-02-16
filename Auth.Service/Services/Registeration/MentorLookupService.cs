using Auth.Service.Models.Registeration.MentorList;
using Auth.Service.Respositories.Registeration;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using UJBHelper.DataModel;

namespace Auth.Service.Services.Registeration
{
    public class MentorLookupService : IMentorLookupService
    {
        private readonly IMongoCollection<User> _users;
        private readonly IMongoCollection<UserKYCDetails> _userKYCDetails;
        private readonly IMongoCollection<BusinessDetails> _businessDetails;
        private IConfiguration _iconfiguration;
        public MentorLookupService(IConfiguration config)
        {
            _iconfiguration = config;
            //  var client = new MongoClient(DbHelper.GetConnectionString());
            //  var database = client.GetDatabase(DbHelper.GetDatabaseName());
            var client = new MongoClient(_iconfiguration["ConnectionString"]);
            var database = client.GetDatabase(_iconfiguration["Database"]);
            _users = database.GetCollection<User>("Users");
            _userKYCDetails = database.GetCollection<UserKYCDetails>("UserKYCDetails");
            _businessDetails = database.GetCollection<BusinessDetails>("BussinessDetails");
        }

        public List<Get_Request> Get_Mentor_By_Search(string searchTerm)
        {
            searchTerm = searchTerm.ToLower();
            var response = new List<Get_Request>();
            var list = new List<Get_Request>();
            var userIdList = _userKYCDetails.Find(x => x.IsApproved.Flag == true).Project(x => x.UserId).ToList();

            var res = _users.Find(x => x.firstName.ToLower().Contains(searchTerm));

            response.AddRange(res.Project(y => new Get_Request
            {
                Base64_Image = y.ImageURL,
                FullName = y.firstName + " " + y.lastName + " " + y.myMentorCode,
                firstName = y.firstName,
                lastName = y.lastName,
                mentorCode = y.myMentorCode,
                userId = y._id,
                address = y.address
            }).ToList());

            res = _users.Find(x => x.lastName.ToLower().Contains(searchTerm));
            response.AddRange(res.Project(y => new Get_Request
            {
                Base64_Image = y.ImageURL,
                FullName = y.firstName + " " + y.lastName + " " + y.myMentorCode,
                firstName = y.firstName,
                lastName = y.lastName,
                mentorCode = y.myMentorCode,
                userId = y._id,
                address = y.address,
            }).ToList());

            var userIdLists = _businessDetails.Find(x => x.UserId != null).Project(x => x.UserId).ToList();
            var userFullnameList = _users.Find(x => userIdLists.Contains(x._id)).ToList();
            
            userIdLists = new List<string>();
            foreach (var f in userFullnameList)
            {
                if ((f.firstName + " " + f.lastName).ToLower().Contains(searchTerm))
                {
                    userIdLists.Add(f._id);
                }
            }

            res = _users.Find(x => userIdLists.Contains(x._id));

            response.AddRange(res.Project(y => new Get_Request
            {
                Base64_Image = y.ImageURL,
                FullName = y.firstName + " " + y.lastName + " " + y.myMentorCode,
                firstName = y.firstName,
                lastName = y.lastName,
                mentorCode = y.myMentorCode,
                userId = y._id,
                address = y.address,
            }).ToList());

            //res = _users.Find(x => x.myMentorCode.ToLower().Contains(searchTerm));
            //response.AddRange(res.Project(y => new Get_Request
            //{
            //    Base64_Image = y.ImageURL,
            //    FullName = y.firstName + " " + y.lastName + " " + y.myMentorCode,
            //    firstName = y.firstName,
            //    lastName = y.lastName,
            //    mentorCode = y.myMentorCode,
            //    userId = y._id,
            //    address = y.address
            //}).ToList());

            //return _users.Find(x => userIdList.Contains(x._id) && x.firstName.ToLower().Contains(searchTerm.ToLower()) || x.lastName.ToLower().Contains(searchTerm.ToLower()) || x.myMentorCode.ToLower().Contains(searchTerm.ToLower())).Project(y => new Get_Request
            // {
            //     Base64_Image = y.base64Image,
            //     firstName = y.firstName,
            //     lastName = y.lastName,
            //     mentorCode = y.myMentorCode,
            //     userId = y._id
            // }).ToList();

            response = response.GroupBy(x => x.userId).Select(y => y.FirstOrDefault()).ToList();
            foreach (var r in response)
            {
                if (userIdList.Contains(r.userId))
                {
                    r.NoOfConnects = int.Parse(_users.Find(user => (user.mentorCode == r.mentorCode)).CountDocuments().ToString());
                    list.Add(r);
                }
            }
            return list;
        }
    }
}
