using Auth.Service.Models.Registeration.EnrollPartner;
using Auth.Service.Respositories.Registeration;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using UJBHelper.Common;
using MongoDB.Driver;
using System.Linq;
using UJBHelper.DataModel;
using UJBHelper.Services;
using System.Threading.Tasks;

namespace Auth.Service.Manager.Registeration.EnrollPartner
{
    public class Insert : IDisposable
    {
        private Post_Request request;

        public HttpStatusCode _statusCode = HttpStatusCode.OK;

        private IEnrollPartnerService _enrollPartnerService;

        public List<Message_Info> _messages = null;

        private string new_latitude = null;

        private string new_longitude = null;

        public string new_role = null;

        private IConfiguration _iconfiguration;
        private NotificationService _notificationservice;


        public Insert(Post_Request post_Request, IEnrollPartnerService EnrollPartnerService,IConfiguration iconfiguration)
        {
            _messages = new List<Message_Info>();

            request = post_Request;

            _enrollPartnerService = EnrollPartnerService;

            _iconfiguration = iconfiguration;
            _notificationservice = new NotificationService();

        }

        public void Process()
        {
            try
            {
                if (Check_If_User_Exists())
                {
                    //if (string.IsNullOrWhiteSpace(request.latitude) && string.IsNullOrWhiteSpace(request.longitude))
                    //{
                    Get_Coordinates_From_Address();
                    //}
                    // Generate_Mentor_Code();

                    Update_Enrollment();

                    // Add_To_Notification_Queue();
                    //Task.Run(() => SendNotification("", "", request.userId));
                    SendNotification("", "", request.userId);
                    Get_Current_Role();
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(Assembly.GetCallingAssembly().GetName().Name + "\n\t" + ex.ToString());
            }
        }

        //private void Add_To_Notification_Queue()
        //{
        //    MessageBody MB = new MessageBody();                   
        //    var nq = new Notification_Sender();
        //    nq.SendNotification("Registeration", MB, request.userId, "", "");

        //}

        
        public void SendNotification(string firstName, string lastName, string UserId)
        {
            MessageBody MB = new MessageBody();
            MB.UserName = firstName + " " + lastName;
            var nq = new Notification_Sender();
            Task.Run(() => nq.SendNotification("Registeration", MB, UserId, "", ""));
          //  return Task.CompletedTask;

        }

        private void Add_To_Notification_Queue_old()
        {
            var nq = new Notification_Queue(_iconfiguration["ConnectionString"], _iconfiguration["Database"]);
            nq.Add_To_Queue(request.userId, "", "", "", "new", "Registeration", "", "Email", "Partner", "");
            Notification notify_template = new Notification();
            notify_template = nq.Get_Notification_Template("Registeration");

            bool isaalowed = notify_template.Data.Where(x => x.Receiver == "Partner").Select(x => x.SMS.isSMSAllowed).FirstOrDefault();
            if (isaalowed)
            {
                nq.Add_To_Queue(request.userId, "", "", "", "new", "Registeration", "", "SMS", "Partner", "");
            }
        }

        private void Generate_Mentor_Code_old()
        {
            request.myMentorCode = "UJB" + request.countryId + request.stateId + Otp_Generator.GetNumber(9);
        }


       
        private void Get_Current_Role()
        {
            new_role = _enrollPartnerService.Get_Current_Role(request.userId);
        }

        private void Get_Coordinates_From_Address()
        {
            try
            {
                var res = _enrollPartnerService.Get_Coordinates_From_Address(request.addressInfo);

                if (res != "NONE")
                {
                    new_latitude = res.Split(",")[0];
                    new_longitude = res.Split(",")[1];

                    request.latitude = double.Parse(new_latitude);
                    request.longitude = double.Parse(new_longitude);

                    _messages.Add(new Message_Info
                    {
                        Message = "Address Converted to Coordinates Successfully",
                        Type = Message_Type.SUCCESS.ToString()
                    });

                    _statusCode = HttpStatusCode.OK;
                }
                else
                {
                    _messages.Add(new Message_Info
                    {
                        Message = "Couldn't Convert Address to Coordinates",
                        Type = Message_Type.ERROR.ToString()
                    });

                    _statusCode = HttpStatusCode.InternalServerError;
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(Assembly.GetCallingAssembly().GetName().Name + "\n\t" + ex.ToString());

                _messages.Add(new Message_Info
                {
                    Message = "Couldn't Convert Address to Coordinates (Exception)",
                    Type = Message_Type.ERROR.ToString()
                });

                _statusCode = HttpStatusCode.InternalServerError;

            }
        }

        private bool Check_If_User_Exists()
        {
            try
            {
                if (_enrollPartnerService.Check_If_User_Exists(request.userId))
                {
                    return true;
                }

                _messages.Add(new Message_Info
                {
                    Message = "No User Found",
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
                    Message = "No User Found",
                    Type = Message_Type.ERROR.ToString()
                });

                _statusCode = HttpStatusCode.NotFound;

                return false;
            }
        }

     
        private void Update_Enrollment()
        {
            try
            {
                _enrollPartnerService.Update_Enrollment(request);

                _messages.Add(new Message_Info
                {
                    Message = "Partner's enrollement updated successfully",
                    Type = Message_Type.SUCCESS.ToString()
                });

                _statusCode = HttpStatusCode.OK;

            }
            catch (Exception ex)
            {
                _messages.Add(new Message_Info
                {
                    Message = "Could not update Partner's enrollment details",
                    Type = Message_Type.ERROR.ToString()
                });

                _statusCode = HttpStatusCode.InternalServerError;

                Logger.Log.Error(Assembly.GetCallingAssembly().GetName().Name + "\n\t" + ex.ToString());

            }
        }

        public void Dispose()
        {
            new_latitude = null;

            new_longitude = null;

            _messages = null;

            _enrollPartnerService = null;

            _statusCode = HttpStatusCode.OK;

            request = null;

            new_role = null;
        }

    }
}
