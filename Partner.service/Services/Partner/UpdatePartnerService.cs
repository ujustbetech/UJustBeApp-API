using Partner.Service.Repositories.Partner;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using UJBHelper.DataModel;
using Post_Request = Partner.Service.Models.Partners.UpdateProfile.Post_Request;
using System;

namespace Partner.Service.Services.Partner
{
    public class UpdatePartnerService : IUpdatePartnerProfile
    {
        private readonly IMongoCollection<User> _userDetails;
        private readonly IMongoCollection<UserOtherDetails> _userOtherDetails;
        private IConfiguration _iconfiguration;
        public UpdatePartnerService(IConfiguration config)
        {
            _iconfiguration = config;
            //  var client = new MongoClient(DbHelper.GetConnectionString());
            //  var database = client.GetDatabase(DbHelper.GetDatabaseName());
            var client = new MongoClient(_iconfiguration["ConnectionString"]);
            var database = client.GetDatabase(_iconfiguration["Database"]);
            _userDetails = database.GetCollection<User>("Users");
            _userOtherDetails = database.GetCollection<UserOtherDetails>("UsersOtherDetails");
        }

        public void Update_Otp(string userId, string otp)
        {
            _userDetails.FindOneAndUpdate(Builders<User>.Filter.Eq(x => x._id, userId), Builders<User>.Update.Set(x => x.otpVerification.OTP, otp));
        }

        public bool Check_If_User_Exist(string UserId)
        {
            return _userDetails.Find(x => x._id == UserId).CountDocuments() > 0;
        }

        public bool Check_If_UserOtherDetails_Exist(string UserId)
        {
            return _userOtherDetails.Find(x => x.UserId == UserId).CountDocuments() > 0;
        }

        public bool Check_If_User_IsActive(string UserId)
        {
            return _userDetails.Find(x => x._id == UserId & x.isActive == true).CountDocuments() > 0;
        }

        public void UpdateGender(Post_Request request)
        {
            _userDetails.FindOneAndUpdate(
                Builders<User>.Filter.Eq(x => x._id, request.userId),
                Builders<User>.Update.Set(x => x.gender, request.value));
        }

        public void UpdateName(Post_Request request)
        {
            if (!string.IsNullOrEmpty(request.value))
            {
                string[] name = request.value.Split(",");
                _userDetails.FindOneAndUpdate(
               Builders<User>.Filter.Eq(x => x._id, request.userId),
               Builders<User>.Update.Set(x => x.firstName, name[0])
               .Set(x => x.lastName, name[1]));

            }
        }

        public void UpdateEmail(Post_Request request)
        {
            _userDetails.FindOneAndUpdate(
                Builders<User>.Filter.Eq(x => x._id, request.userId),
                Builders<User>.Update.Set(x => x.emailId, request.value));
        }

        public void UpdateKnowledgeSource(Post_Request request)
        {
            _userDetails.FindOneAndUpdate(
                Builders<User>.Filter.Eq(x => x._id, request.userId),
                Builders<User>.Update.Set(x => x.knowledgeSource, request.value));
        }

        public void UpdateMentorCode(Post_Request request)
        {
            _userDetails.FindOneAndUpdate(
                Builders<User>.Filter.Eq(x => x._id, request.userId),
                Builders<User>.Update.Set(x => x.mentorCode, request.value));
        }

        //public void UpdateOrganisationType(Post_Request request)
        //{
        //    _userDetails.FindOneAndUpdate(
        //        Builders<User>.Filter.Eq(x => x._id, request.userId),
        //        Builders<User>.Update.Set(x => x.organisationType, request.value));
        //}

        //public void UpdateUserType(Post_Request request)
        //{
        //    _userDetails.FindOneAndUpdate(
        //        Builders<User>.Filter.Eq(x => x._id, request.userId),
        //        Builders<User>.Update.Set(x => x.userType, int.Parse(request.value)));
        //}

        public void UpdateMobileNo(Post_Request request)
        {
            _userDetails.FindOneAndUpdate(
                Builders<User>.Filter.Eq(x => x._id, request.userId),
                Builders<User>.Update.Set(x => x.mobileNumber, request.value));
        }

        public void UpdatePassiveIncome(Post_Request request)
        {
            _userDetails.FindOneAndUpdate(
                Builders<User>.Filter.Eq(x => x._id, request.userId),
                Builders<User>.Update.Set(x => x.passiveIncome, request.value));
        }

        //public void UpdateBirthDate(Models.Partners.UpdateBirthDate.Post_Request request)
        //{
        //    TimeSpan diff = DateTime.Now - DateTime.UtcNow;
        //    string[] timezone = DateTime.Now.ToString("zzz").Split(new char[] { '+', '-', ':' });

        //    DateTime BirthDate = DateTime.Parse()

        //    if (request.value.HasValue)
        //    {
        //        DateTime date1 = ((DateTime)request.value).Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0));
        //        request.value = date1;
        //    }
        //    _userDetails.FindOneAndUpdate(
        //        Builders<User>.Filter.Eq(x => x._id, request.userId),
        //        Builders<User>.Update.Set(x => x.birthDate, request.value));
        //}

        public void UpdateBirthDate(Post_Request request)
        {
            TimeSpan diff = DateTime.Now - DateTime.UtcNow;
            string[] timezone = DateTime.Now.ToString("zzz").Split(new char[] { '+', '-', ':' });

            DateTime BirthDate = DateTime.Parse(request.value);
            BirthDate = BirthDate.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0));

            //if (request.value.HasValue)
            //{
            //    DateTime date1 = ((DateTime)request.value).Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0));
            //    request.value = date1;
            //}
            _userDetails.FindOneAndUpdate(
                Builders<User>.Filter.Eq(x => x._id, request.userId),
                Builders<User>.Update.Set(x => x.birthDate, BirthDate));
        }

        public void UpdateLanguage(Post_Request request)
        {
            _userDetails.FindOneAndUpdate(
                Builders<User>.Filter.Eq(x => x._id, request.userId),
                Builders<User>.Update.Set(x => x.language, request.value));
        }

        public void UpdateAboutMe(Post_Request request)
        {
            if (Check_If_UserOtherDetails_Exist(request.userId))
            {
                _userOtherDetails.FindOneAndUpdate(
                    Builders<UserOtherDetails>.Filter.Eq(x => x.UserId, request.userId),
                    Builders<UserOtherDetails>.Update.Set(x => x.aboutMe, request.value));
            }
            else
            {

                TimeSpan diff = DateTime.Now - DateTime.UtcNow;
                string[] timezone = DateTime.Now.ToString("zzz").Split(new char[] { '+', '-', ':' });

                var p = new UserOtherDetails
                {
                    UserId = request.userId,
                  //  father_Husband_Name = "",
                    maritalStatus = "",
                    nationality = "",
                    phoneNo = "",
                    Hobbies = "",
                    aboutMe = request.value,
                    areaOfInterest = "",
                    canImpartTraining = null,
                    Created = new Created
                    {
                        created_By = "",
                        created_On = DateTime.Now.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0))
                    },
                    Updated = new Updated
                    {
                        updated_By = "",
                        updated_On = DateTime.Now.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0))
                    }
                };
                _userOtherDetails.InsertOne(p);
            }
        }

        public void UpdateCanImpartTraining(Post_Request request)
        {
            TimeSpan diff = DateTime.Now - DateTime.UtcNow;
            string[] timezone = DateTime.Now.ToString("zzz").Split(new char[] { '+', '-', ':' });

            if (Check_If_UserOtherDetails_Exist(request.userId))
            {
                _userOtherDetails.FindOneAndUpdate(
                Builders<UserOtherDetails>.Filter.Eq(x => x.UserId, request.userId),
                Builders<UserOtherDetails>.Update.Set(x => x.canImpartTraining, bool.Parse(request.value)));
            }
            else
            {
                var p = new UserOtherDetails
                {
                    UserId = request.userId,
                    //father_Husband_Name = "",
                    maritalStatus = "",
                    nationality = "",
                    phoneNo = "",
                    Hobbies = "",
                    aboutMe = "",
                    areaOfInterest = "",
                    canImpartTraining =bool.Parse(request.value),
                    Created = new Created
                    {
                        created_By = "",
                        created_On = DateTime.Now.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0))
                    },
                    Updated = new Updated
                    {
                        updated_By = "",
                        updated_On = DateTime.Now.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0))
                    }
                };
                _userOtherDetails.InsertOne(p);
            }
        }

        public void UpdateMaritalStatus(Post_Request request)
        {

            TimeSpan diff = DateTime.Now - DateTime.UtcNow;
            string[] timezone = DateTime.Now.ToString("zzz").Split(new char[] { '+', '-', ':' });

            if (Check_If_UserOtherDetails_Exist(request.userId))
            {
                _userOtherDetails.FindOneAndUpdate(
                    Builders<UserOtherDetails>.Filter.Eq(x => x.UserId, request.userId),
                    Builders<UserOtherDetails>.Update.Set(x => x.maritalStatus, request.value));
            }
            else
            {
                var p = new UserOtherDetails
                {
                    UserId = request.userId,
                    //father_Husband_Name = "",
                    maritalStatus = request.value,
                    nationality = "",
                    phoneNo = "",
                    Hobbies = "",
                    aboutMe = "",
                    areaOfInterest = "",
                    canImpartTraining = null,
                    Created = new Created
                    {
                        created_By = "",
                        created_On = DateTime.Now.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0))
                    },
                    Updated = new Updated
                    {
                        updated_By = "",
                        updated_On = DateTime.Now.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0))
                    }
                };
                _userOtherDetails.InsertOne(p);
            }
        }

        public void UpdateHobbies(Post_Request request)
        {

            TimeSpan diff = DateTime.Now - DateTime.UtcNow;
            string[] timezone = DateTime.Now.ToString("zzz").Split(new char[] { '+', '-', ':' });

            if (Check_If_UserOtherDetails_Exist(request.userId))
            {
                _userOtherDetails.FindOneAndUpdate(
                    Builders<UserOtherDetails>.Filter.Eq(x => x.UserId, request.userId),
                    Builders<UserOtherDetails>.Update.Set(x => x.Hobbies, request.value));
            }
            else
            {
                var p = new UserOtherDetails
                {
                    UserId = request.userId,
                    //father_Husband_Name = "",
                    maritalStatus = "",
                    nationality = "",
                    phoneNo = "",
                    Hobbies = request.value,
                    aboutMe = "",
                    areaOfInterest = "",
                    canImpartTraining = null,
                    Created = new Created
                    {
                        created_By = "",
                        created_On = DateTime.Now.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0))
                    },
                    Updated = new Updated
                    {
                        updated_By = "",
                        updated_On = DateTime.Now.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0))
                    }
                };
                _userOtherDetails.InsertOne(p);
            }
        }

        public void UpdateAreaOfInterest(Post_Request request)
        {
            TimeSpan diff = DateTime.Now - DateTime.UtcNow;
            string[] timezone = DateTime.Now.ToString("zzz").Split(new char[] { '+', '-', ':' });

            if (Check_If_UserOtherDetails_Exist(request.userId))
            {
                _userOtherDetails.FindOneAndUpdate(
                    Builders<UserOtherDetails>.Filter.Eq(x => x.UserId, request.userId),
                    Builders<UserOtherDetails>.Update.Set(x => x.areaOfInterest, request.value));
            }
            else
            {
                var p = new UserOtherDetails
                {
                    UserId = request.userId,
                    //father_Husband_Name = "",
                    maritalStatus = "",
                    nationality = "",
                    phoneNo = "",
                    Hobbies = "",
                    aboutMe = "",
                    areaOfInterest = request.value,
                    canImpartTraining = null,
                    Created = new Created
                    {
                        created_By = "",
                        created_On = DateTime.Now.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0))
                    },
                    Updated = new Updated
                    {
                        updated_By = "",
                        updated_On = DateTime.Now.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0))
                    }
                };
                _userOtherDetails.InsertOne(p);
            }
        }

        public void UpdateProfileImage(Models.Partners.UpdateProfileImage.Post_Request request)
        {
            _userDetails.FindOneAndUpdate(
                Builders<User>.Filter.Eq(x => x._id, request.userId),
                Builders<User>.Update.Set(x => x.FileName, request.FileName)
                .Set(x => x.UniqueName, request.UniqueName)
                .Set(x => x.ImageURL, request.ImageURL)
               // .Set(x => x.imageType, request.ImageType)
                );
        }

        public void UpdatePartnerLocation(Models.Partners.UpdateLocation.Post_Request request)
        {
            _userDetails.FindOneAndUpdate(
                Builders<User>.Filter.Eq(x => x._id, request.UserId),
                Builders<User>.Update.Set(x => x.preferredLocations, new PreferredLocations
                {
                    location1 = request.locations.location1,
                    location2 = request.locations.location2,
                    location3 = request.locations.location3
                })
                );
        }

        public void UpdatePartnerAddress(Models.Partners.UpdateAddress.Post_Request request)
        {
            _userDetails.FindOneAndUpdate(
                Builders<User>.Filter.Eq(x => x._id, request.UserId),
                Builders<User>.Update.Set(x => x.address, new Address
                {
                    flatWing = request.address.flatWing,
                    locality = request.address.locality,
                    location = request.address.location
                   // stateId = request.address.stateId
                }).Set(x => x.stateId, request.stateId)
                .Set(x => x.countryId, request.countryId)
                );
        }


        public void Update_Partner_AppVersion(Models.Partners.UpdatePartnerAppVersion.Post_Request request)
        {
            _userDetails.FindOneAndUpdate(
                Builders<User>.Filter.Eq(x => x._id, request.userId),
                Builders<User>.Update.Set(x => x.currentAppVersion, request.versionCode));
        }
        public bool Check_If_User_AppVersion_Exist(string UserId, string CurrentVersion)
        {
            return _userDetails.Find(x => x._id == UserId && x.currentAppVersion == CurrentVersion).CountDocuments() > 0;
        }
    }
}
