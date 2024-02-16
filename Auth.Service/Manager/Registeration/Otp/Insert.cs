using Auth.Service.Models.Registeration.Otp;
using Auth.Service.Respositories.Registeration;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using UJBHelper.Common;
using UJBHelper.DataModel;
using System.Linq;
using System.Threading.Tasks;

namespace Auth.Service.Manager.Registeration.Otp
{
    public class Insert : IDisposable
    {
        private Post_Request request;

        public HttpStatusCode _statusCode = HttpStatusCode.OK;

        private IOtpService _otpService;

        public List<Message_Info> _messages = null;

        private IConfiguration _iconfiguration;


        public Insert(Post_Request request, IOtpService otpService, IConfiguration iconfiguration
)
        {
            _messages = new List<Message_Info>();

            _otpService = otpService;

            this.request = request;

            _iconfiguration = iconfiguration;

        }

        public void Process()
        {
            try
            {
                if (request.type == "Registeration" || request.type == "Update")
                {
                    if (Check_If_User_Exists())
                    {
                        Update_Otp_Status();

                        if (request.type == "Update")
                        {
                            Update_MobileNo();
                            //  Add_To_Notification_Queue();
                            //  var sendNotification = SendNotification("", "", request.userId);
                            SendNotification("", "", request.userId);
                        }


                    }
                }
                else
                {
                    _messages.Add(new Message_Info
                    {
                        Message = "Invalid Type Specified",
                        Type = Message_Type.ERROR.ToString()
                    });

                    _statusCode = HttpStatusCode.BadRequest;
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(Assembly.GetCallingAssembly().GetName().Name + "\n\t" + ex.ToString());
            }
        }

        public void SendNotification(string firstName, string lastName, string UserId)
        {
            MessageBody MB = new MessageBody();
            MB.UserName = firstName + " " + lastName;
            var nq = new Notification_Sender();
            // nq.SendNotification("Mobile Number Changed", MB, UserId, "", "");
            Task.Run(() => nq.SendNotificationAsync("Mobile Number Changed", MB, UserId, "", ""));
            //  return Task.CompletedTask;

        }


        private void Add_To_Notification_Queue()
        {
            var nq = new Notification_Queue(_iconfiguration["ConnectionString"], _iconfiguration["Database"]);
            //switch (request.type)
            //{
            //case "Registeration":
            //    nq.Add_To_Queue(request.userId, "", "", "", "new", "Registeration", "", "Email", "User", "");
            //    nq.Add_To_Queue(request.userId, "", "", "", "new", "Registeration", "", "SMS", "User", "");
            //    break;
            //case "Update":
            nq.Add_To_Queue(request.userId, "", "", "", "new", "Mobile Number Changed", "", "Email", "User", "");
            Notification notify_template = new Notification();
            notify_template = nq.Get_Notification_Template("Mobile Number Changed");

            bool isaalowed = notify_template.Data.Where(x => x.Receiver == "User").Select(x => x.SMS.isSMSAllowed).FirstOrDefault();
            if (isaalowed)
            {
                nq.Add_To_Queue(request.userId, "", "", "", "new", "Mobile Number Changed", "", "SMS", "User", "");
            }
            // break;
            //  }
        }

        private bool Check_If_User_Exists()
        {
            try
            {
                if (_otpService.Check_If_User_Exists(request.userId))
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

        private void Update_Otp_Status()
        {
            try
            {
                _otpService.Update_Otp_Flag(request);

                _messages.Add(new Message_Info
                {
                    Message = "Otp Validated Successfully",
                    Type = Message_Type.SUCCESS.ToString()
                });

                _statusCode = HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                _messages.Add(new Message_Info
                {
                    Message = "Otp Could not be Validated ",
                    Type = Message_Type.ERROR.ToString()
                });

                _statusCode = HttpStatusCode.InternalServerError;

                Logger.Log.Error(Assembly.GetCallingAssembly().GetName().Name + "\n\t" + ex.ToString());

                throw;
            }

        }

        private void Update_MobileNo()
        {
            try
            {
                _otpService.UpdateMobileNo(request.userId, request.MobileNumber, request.countryCode);

                _messages.Add(new Message_Info
                {
                    Message = "Mobile No Updated Successfully",
                    Type = Message_Type.SUCCESS.ToString()
                });

                _statusCode = HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                _messages.Add(new Message_Info
                {
                    Message = "Mobile No Could not be Updated",
                    Type = Message_Type.ERROR.ToString()
                });

                _statusCode = HttpStatusCode.InternalServerError;

                Logger.Log.Error(Assembly.GetCallingAssembly().GetName().Name + "\n\t" + ex.ToString());

                throw;
            }

        }

        public void Dispose()
        {
            _messages = null;

            _statusCode = HttpStatusCode.OK;

            request = null;

            _otpService = null;

        }
    }
}
