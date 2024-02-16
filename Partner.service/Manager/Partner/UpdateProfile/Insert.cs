using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Partner.Service.Models.Partners.UpdateProfile;
using Partner.Service.Repositories.Partner;
using UJBHelper.Common;
using UJBHelper.DataModel;
using System.Linq;
using System.Threading.Tasks;

namespace Partner.Service.Manager.Partner.UpdateProfile
{
    public class Insert : IDisposable
    {
        public HttpStatusCode _statusCode = HttpStatusCode.OK;
        public List<Message_Info> _messages = null;
        private Post_Request request;
        private  IUpdatePartnerProfile _updatePartnerProfileService;
        private IConfiguration _iconfiguration;
        private Notification notify_template;



        public Insert(Post_Request request, IUpdatePartnerProfile updatePartnerProfileService, IConfiguration iconfiguration
)
        {
            this.request = request;
            _updatePartnerProfileService = updatePartnerProfileService;
            _messages = new List<Message_Info>();
            _iconfiguration = iconfiguration;
            notify_template = new Notification();

        }

        public void Process()
        {
            if (Verify_User())
            {
                if (Verify_UserIsActive())
                {
                    Update_Partner_Profile_Details();
                }
            }
        }

        private void Update_Partner_Profile_Details()
        {
            try
            {
                MessageBody MB = new MessageBody();
                var nq = new Notification_Sender();
                // var nq = new Notification_Queue(_iconfiguration["ConnectionString"], _iconfiguration["Database"]);
                switch (request.type)
                {
                    case "Gender":
                        _updatePartnerProfileService.UpdateGender(request);
                        break;
                    case "Name":
                        _updatePartnerProfileService.UpdateName(request);
                        break;
                    case "Email":
                        _updatePartnerProfileService.UpdateEmail(request);
                        //nq.Add_To_Queue(request.userId, "", "", "", "new", "Email Changed", "", "Email", "User", "");
                        //notify_template = nq.Get_Notification_Template("Email Changed");
                        //bool isallowed = notify_template.Data.Where(x => x.Receiver == "User").Select(x => x.SMS.isSMSAllowed).FirstOrDefault();
                        //if (isallowed)
                        //{
                        //    nq.Add_To_Queue(request.userId, "", "", "", "new", "Email Changed", "", "SMS", "User", "");
                        //}

                        var sendNotification = SendNotification("Email Changed", request.userId);
                        break;
                    case "Mobile":
                        _updatePartnerProfileService.UpdateMobileNo(request);
                        break;
                    case "AboutMe":
                        _updatePartnerProfileService.UpdateAboutMe(request);
                        break;
                    case "Hobbies":
                        _updatePartnerProfileService.UpdateHobbies(request);
                        break;
                    case "AreaOfInterest":
                        _updatePartnerProfileService.UpdateAreaOfInterest(request);
                        break;
                    case "PassiveIncome":
                        _updatePartnerProfileService.UpdatePassiveIncome(request);
                        break;
                    case "BirthDate":
                        _updatePartnerProfileService.UpdateBirthDate(request);
                        break;
                    case "CanImpartTraining":
                        _updatePartnerProfileService.UpdateCanImpartTraining(request);
                        break;
                    case "Language":
                        _updatePartnerProfileService.UpdateLanguage(request);
                        break;
                    case "MaritalStatus":
                        _updatePartnerProfileService.UpdateMaritalStatus(request);
                        break;
                    case "KnowledgeSource":
                        _updatePartnerProfileService.UpdateKnowledgeSource(request);
                        break;
                    case "MentorCode":
                        _updatePartnerProfileService.UpdateMentorCode(request);
                        break;
                    //case "OrganisationType":
                    //    _updatePartnerProfileService.UpdateOrganisationType(request);
                    //    break;
                    //case "UserType":
                    //    _updatePartnerProfileService.UpdateUserType(request);
                    //    break;
                    default:                        
                        break;

                }

                _messages.Add(new Message_Info { Message = $"Partners {request.type} updated successfully", Type = Message_Type.SUCCESS.ToString() });

                _statusCode = HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(Assembly.GetCallingAssembly().GetName().Name + "\n\t" + ex.ToString());

                _messages.Add(new Message_Info { Message = "Exception Occured", Type = Message_Type.ERROR.ToString() });

                _statusCode = HttpStatusCode.InternalServerError;

                throw;
            }
        }
        public Task SendNotification(string Event, string UserId)
        {
            MessageBody MB = new MessageBody();

            var nq = new Notification_Sender();
             nq.SendNotification(Event, MB, UserId, "", "");
            return Task.CompletedTask;

        }
        private bool Verify_User()
        {
            try
            {
                if (_updatePartnerProfileService.Check_If_User_Exist(request.userId))
                {
                    return true;
                }
                _messages.Add(new Message_Info
                {
                    Message = "No Users Found",
                    Type = Message_Type.ERROR.ToString()
                });

                _statusCode = HttpStatusCode.NotFound;

                return false;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(Assembly.GetCallingAssembly().GetName().Name + "\n\t" + ex.ToString());
                _messages.Add(new Message_Info
                {
                    Message = "No Users Found",
                    Type = Message_Type.ERROR.ToString()
                });

                _statusCode = HttpStatusCode.NotFound;

                return false;
            }
        }


        private bool Verify_UserIsActive()
        {
            try
            {
                if (_updatePartnerProfileService.Check_If_User_IsActive(request.userId))
                {
                    return true;
                }
                _messages.Add(new Message_Info
                {
                    Message = "User Is InActive",
                    Type = Message_Type.ERROR.ToString()
                });

                _statusCode = HttpStatusCode.NotFound;

                return false;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(Assembly.GetCallingAssembly().GetName().Name + "\n\t" + ex.ToString());
                _messages.Add(new Message_Info
                {
                    Message = "User Is InActive",
                    Type = Message_Type.ERROR.ToString()
                });

                _statusCode = HttpStatusCode.NotFound;

                return false;
            }
        }


        public void Dispose()
        {
            request = null;

            _updatePartnerProfileService = null;

            _statusCode = HttpStatusCode.OK;

            _messages = null;
            notify_template = null;
        }
    }
}
