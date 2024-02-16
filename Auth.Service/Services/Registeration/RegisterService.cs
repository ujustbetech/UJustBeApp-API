using Auth.Service.Models.Registeration.Register;
using Auth.Service.Respositories.Registeration;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using UJBHelper.DataModel;

namespace Auth.Service.Services.Registeration
{
    public class RegisterService : IRegisterService
    {
        private readonly IMongoCollection<User> _users;
        private readonly IMongoCollection<AdminUser> _adminUsers;
        private readonly IMongoCollection<NotificationQueue> _notificationQueue;

        private IConfiguration _iconfiguration;
        public RegisterService(IConfiguration config)
        {
            _iconfiguration = config;
            //  var client = new MongoClient(DbHelper.GetConnectionString());
            //  var database = client.GetDatabase(DbHelper.GetDatabaseName());
            var client = new MongoClient(_iconfiguration["ConnectionString"]);
            var database = client.GetDatabase(_iconfiguration["Database"]);
            _users = database.GetCollection<User>("Users");
            _adminUsers = database.GetCollection<AdminUser>("AdminUsers");
            _notificationQueue = database.GetCollection<NotificationQueue>("NotificationQueue");


        }

        public Get_Request Insert_User(Post_Request request, string new_otp)
        {
            TimeSpan diff = DateTime.Now - DateTime.UtcNow;
            string[] timezone = DateTime.Now.ToString("zzz").Split(new char[] { '+', '-', ':' });

            var u = new User
            {
                firstName = request.firstName,
                lastName = request.lastName,
                emailId = request.email,
                mobileNumber = request.mobileNumber,
                password = request.password,
                Role = request.userRole,
                isActive = true,
                Created = new Created { created_By = request.createdBy, created_On = DateTime.Now.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0)) },
                Updated = new Updated { updated_By = "", updated_On = null },
                socialLogin = new SocialLogin { Is_Social_Login = !string.IsNullOrWhiteSpace(request.socialMediaType), Social_Site = request.socialMediaType, Social_Code = request.socialMediaId },
                countryCode = request.countryCode,
                otpVerification = new OtpVerification { OTP = new_otp, OTP_Verified = false },
                address = new Address(),
                preferredLocations = new PreferredLocations(),
                latitude = 0.0,
                longitude = 0.0,
                FileName = "",
                ImageURL = "",
                UniqueName = "",
                isMembershipAgreementAccepted = false,
                isPartnerAgreementAccepted = false
            };

            _users.InsertOne(u);

            return new Get_Request { UserId = u._id, UserRole = u.Role, otp = new_otp };
        }

        public Get_Request Update_User(Post_Request request)
        {
            TimeSpan diff = DateTime.Now - DateTime.UtcNow;
            string[] timezone = DateTime.Now.ToString("zzz").Split(new char[] { '+', '-', ':' });

            var filter = Builders<User>.Filter.Eq("_id", request.Id);
            _users.FindOneAndUpdate(filter, Builders<User>.Update
                 .Set(x => x.firstName, request.firstName)
                 .Set(x => x.lastName, request.lastName)
                 .Set(x => x.emailId, request.email)
                 .Set(x => x.mobileNumber, request.mobileNumber)
                 .Set(x => x.countryCode, request.countryCode)
                 //.Set(x => x.password, request.password)
                 .Set(x => x.Role, request.userRole)
                 .Set(x => x.socialLogin.Is_Social_Login, !string.IsNullOrWhiteSpace(request.socialMediaType))
                 .Set(x => x.socialLogin.Social_Code, request.socialMediaId)
                 .Set(x => x.socialLogin.Social_Site, request.socialMediaType)
                 .Set(x => x.Updated, new Updated { updated_By = request.createdBy, updated_On = DateTime.Now.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0)) }));

            return new Get_Request { UserId = request.Id, UserRole = request.userRole };
        }

        public Get_Request Insert_Admin_User(Post_Request request, string new_otp)
        {

            TimeSpan diff = DateTime.Now - DateTime.UtcNow;
            string[] timezone = DateTime.Now.ToString("zzz").Split(new char[] { '+', '-', ':' });

            var u = new AdminUser
            {
                firstName = request.firstName,
                lastName = request.lastName,
                emailId = request.email,
                mobileNumber = request.mobileNumber,
                password = request.password,
                Role = request.userRole,
                countryCode = request.countryCode,
                isActive = true,
                Created = new Created { created_By = request.createdBy, created_On = DateTime.Now.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0)) },
                Updated = new Updated { updated_By = "", updated_On = null }
            };

            _adminUsers.InsertOne(u);

            return new Get_Request { UserId = u._id, UserRole = u.Role };
        }

        public Get_Request Update_Admin_User(Post_Request request)
        {
            TimeSpan diff = DateTime.Now - DateTime.UtcNow;
            string[] timezone = DateTime.Now.ToString("zzz").Split(new char[] { '+', '-', ':' });

            var filter = Builders<AdminUser>.Filter.Eq("_id", request.Id);
            _adminUsers.FindOneAndUpdate(filter, Builders<AdminUser>.Update
                 .Set(x => x.firstName, request.firstName)
                 .Set(x => x.lastName, request.lastName)
                 .Set(x => x.emailId, request.email)
                 .Set(x => x.mobileNumber, request.mobileNumber)
                 .Set(x => x.password, request.password)
                 .Set(x => x.Role, request.userRole)
                 .Set(x => x.countryCode, request.countryCode)
                 .Set(x => x.isActive, request.isActive)
                 .Set(x => x.Updated, new Updated { updated_By = request.createdBy, updated_On = DateTime.Now.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0)) }));

            return new Get_Request { UserId = request.Id, UserRole = request.userRole };
        }

        public string Verify_Admin_User(string userId, string oldPassword)
        {
            return _adminUsers.Find(x => x._id == userId && x.password == oldPassword).FirstOrDefault().firstName;
        }

        public bool Check_If_User_Email_Exists(string EmailId)
        {
            return _users.Find(x => x.emailId == EmailId).CountDocuments() == 0;
        }

        public bool Check_If_User_PhoneNo_Exists(string MobileNo)
        {
            return _users.Find(x => x.mobileNumber == MobileNo).CountDocuments() == 0;
        }

        public void Create_New_Password(string user_id, string new_password)
        {
            _adminUsers.FindOneAndUpdate(
               Builders<AdminUser>.Filter.Eq(x => x._id, user_id),
               Builders<AdminUser>.Update.Set(x => x.password, new_password)
                );
        }

        public Get_Request Insert_AdminPartner(Post_Request request, string new_otp)
        {

            TimeSpan diff = DateTime.Now - DateTime.UtcNow;
            string[] timezone = DateTime.Now.ToString("zzz").Split(new char[] { '+', '-', ':' });

            var u = new User
            {
                firstName = request.firstName,
                lastName = request.lastName,
                emailId = request.email,
                mobileNumber = request.mobileNumber,
                alternateMobileNumber = request.alternateMobileNumber,
                password = request.password,
                Role = request.userRole,
                isActive = request.isActive,
                Created = new Created { created_By = request.createdBy, created_On = DateTime.Now.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0)) },
                Updated = new Updated { updated_By = "", updated_On = null },
                socialLogin = new SocialLogin { Is_Social_Login = !string.IsNullOrWhiteSpace(request.socialMediaType), Social_Site = request.socialMediaType, Social_Code = request.socialMediaId },
                countryCode = request.countryCode,
                alternateCountryCode = request.alternateCountryCode,
                isActiveComment = request.isActiveComment,
                otpVerification = new OtpVerification { OTP = new_otp, OTP_Verified = false },
                address = new Address(),
                preferredLocations = new PreferredLocations(),
                middleName = request.middleName,
                latitude = 0.0,
                longitude = 0.0,
                FileName = "",
                UniqueName = "",
                ImageURL = "",
                isMembershipAgreementAccepted = false,
                isPartnerAgreementAccepted = false
            };

            _users.InsertOne(u);

            return new Get_Request { UserId = u._id, UserRole = u.Role };
        }

        public Get_Request Update_AdminPartner(Post_Request request)
        {

            TimeSpan diff = DateTime.Now - DateTime.UtcNow;
            string[] timezone = DateTime.Now.ToString("zzz").Split(new char[] { '+', '-', ':' });

            var filter = Builders<User>.Filter.Eq("_id", request.Id);
            _users.FindOneAndUpdate(filter, Builders<User>.Update
                 .Set(x => x.firstName, request.firstName)
                 .Set(x => x.lastName, request.lastName)
                 .Set(x => x.emailId, request.email)
                 .Set(x => x.mobileNumber, request.mobileNumber)
                 .Set(x => x.countryCode, request.countryCode)
                 //.Set(x => x.password, request.password)
                 .Set(x => x.Role, request.userRole)
                 .Set(x => x.isActive, request.isActive)
                 .Set(x => x.socialLogin.Is_Social_Login, !string.IsNullOrWhiteSpace(request.socialMediaType))
                 .Set(x => x.socialLogin.Social_Code, request.socialMediaId)
                 .Set(x => x.socialLogin.Social_Site, request.socialMediaType)
                 .Set(x => x.middleName, request.middleName)
                 .Set(x => x.alternateCountryCode, request.alternateCountryCode)
                 .Set(x => x.alternateMobileNumber, request.alternateMobileNumber)
                 .Set(x => x.isActiveComment, request.isActiveComment)
                 .Set(x => x.middleName, request.middleName)
                 .Set(x => x.Updated, new Updated { updated_By = request.createdBy, updated_On = DateTime.Now.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0)) }));

            return new Get_Request { UserId = request.Id, UserRole = request.userRole };
        }

        public void Add_To_Notification_Queue(string userId, string leadId, string notificationId, string date, string status, string eventName, string contactInfo, string type, string receiver, string message)
        {
            var newDate = DateTime.Parse(date);

            var INDIAN_ZONE = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            var dd = DateTime.Parse(date);
            newDate = TimeZoneInfo.ConvertTimeFromUtc(dd, INDIAN_ZONE);
            var nq = new NotificationQueue
            {
                userId = userId,
                leadId = leadId,
                notificationId = notificationId,
                dateOfNotification = newDate,
                status = status,
                Event = eventName,
                ContactInfo = contactInfo,
                Type = type,
                Receiver = receiver,
                Message = message

            };

            _notificationQueue.InsertOne(nq);
        }

        public bool Check_If_User_Email_Exists(string emailId, string UserId)
        {
            string email = _users.Find(x => x._id == UserId).Project(x => x.emailId).FirstOrDefault();
            if (email.ToLower() == emailId.ToLower())
            {
                return true;
            }
            else
            {
                return _users.Find(x => x.emailId.ToLower() == emailId.ToLower()).CountDocuments() > 0 ? false : true;
            }
            return true;
        }

        public bool Check_If_User_PhoneNo_Exists(string mobileNo, string UserId)
        {
            string mobileNumber = _users.Find(x => x._id == UserId).Project(x => x.mobileNumber).FirstOrDefault();
            if (mobileNo.ToLower() == mobileNumber.ToLower())
            {
                return true;
            }
            else
            {
                return _users.Find(x => x.mobileNumber == mobileNo).CountDocuments() > 0 ? false : true;
            }
            return true;

        }
    }
}
