using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using Auth.Service.Models.Registeration.Register;
using Auth.Service.Respositories.Registeration;
using Microsoft.Extensions.Configuration;
using UJBHelper.Common;
using UJBHelper.DataModel;
using System.Linq;
using System.Threading.Tasks;

namespace Auth.Service.Manager.Registeration.Register
{
    public class Insert : IDisposable
    {
        private Post_Request request;

        public Get_Request _response;

        public HttpStatusCode _statusCode = HttpStatusCode.OK;

        private IRegisterService _registerService;

        public List<Message_Info> _messages = null;

        private string new_otp = "";

        private IConfiguration _iconfiguration;
        private Notification notify_template;

        public Insert(Post_Request post_Request, IRegisterService registerService, IConfiguration iconfiguration)
        {
            _messages = new List<Message_Info>();

            request = post_Request;

            _registerService = registerService;

            _iconfiguration = iconfiguration;
            notify_template = new Notification();

        }

        public void Process()
        {
            try
            {
                request.password = SecurePasswordHasherHelper.Generate_HashPasssword(request.password);
                if (string.IsNullOrWhiteSpace(request.Id))
                {
                    Generate_Otp();

                    if (Check_If_User_Email_Exists())
                    {
                        if (Check_If_User_PhoneNo_Exists())
                        {
                            Insert_New_User();

                            SendOTPNotification(request.firstName, request.lastName, _response.UserId, new_otp);

                            SendNotification(request.firstName, request.lastName, _response.UserId);

                            // Task.Run(() => SendNotification(request.firstName, request.lastName, _response.UserId));

                        }
                    }

                }
                else
                {
                    if (Check_If_User_Email_Exists())
                    {
                        if (Check_If_User_PhoneNo_Exists())
                        {
                            Update_Existing_User();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(Assembly.GetCallingAssembly().GetName().Name + "\n\t" + ex.ToString());
                _messages.Add(new Message_Info { Message = "Error While Registering User", Type = Message_Type.ERROR.ToString() });

                _statusCode = HttpStatusCode.InternalServerError;

                throw;
            }
        }

        private bool Check_If_User_PhoneNo_Exists()
        {
            try
            {
                if (_registerService.Check_If_User_PhoneNo_Exists(request.mobileNumber))
                {
                    return true;
                }
                _messages.Add(new Message_Info
                {
                    Message = "Mobile Number Already Exist",
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

        private bool Check_If_User_Email_Exists()
        {
            try
            {
                if (_registerService.Check_If_User_Email_Exists(request.email))
                {
                    return true;
                }
                _messages.Add(new Message_Info
                {
                    Message = "Email Already Exist",
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
        //private void Add_To_Notification_Queue()
        //{
        //    MessageBody MB = new MessageBody();
        //    MB.UserName = request.firstName + " " + request.lastName;
        //    var nq = new Notification_Sender();
        //    nq.SendNotification("Registeration", MB, _response.UserId, "", "");

        //}
        public void SendNotification(string firstName, string lastName, string UserId)
        {
            MessageBody MB = new MessageBody();
            MB.UserName = firstName + " " + lastName;
            var nq = new Notification_Sender();
            Task.Run(() => nq.SendNotificationAsync("Registeration", MB, UserId, "", ""));
            //  nq.SendNotification("Registeration", MB, UserId, "", "");
            //return Task.CompletedTask;

        }

        public void SendOTPNotification(string firstName, string lastName, string UserId, string new_otp)
        {
            MessageBody MB = new MessageBody();
            MB.UserName = firstName + " " + lastName;
            MB.new_otp = new_otp;
            var nq = new Notification_Sender();
            Task.Run(() => nq.SendNotificationAsync("OTP Send", MB, UserId, "", ""));
            //    nq.SendNotification("OTP Send", MB, UserId, "", "");
            //   return Task.CompletedTask;
        }
        private void Add_To_Notification_Queue_old()
        {
            var nq = new Notification_Queue(_iconfiguration["ConnectionString"], _iconfiguration["Database"]);

            notify_template = nq.Get_Notification_Template("Registeration");
            //   nq.Add_To_Queue(_response.UserId,"","","","new","Registeration","","Email","User","");


            bool isaalowed = notify_template.Data.Where(x => x.Receiver == "User").Select(x => x.SMS.isSMSAllowed).FirstOrDefault();
            if (isaalowed)
            {
                //    nq.Add_To_Queue(_response.UserId, "", "", "", "new", "Registeration", "", "SMS", "User", "");
            }
        }

        private void Send_Otp_Via_Sms()
        {
            _response.otp = new_otp;
            _response.otp = new_otp;
            string fullName = request.firstName + " " + request.lastName;

            try
            {
                // Hello @user,{ new_otp} is your OTP for UJUSTB App. use this OTP to continue.
                var message = $" Hello { fullName }, { new_otp} is your OTP to verify yourself on UJustBe App. Please use this OTP to continue.";
                var mobilenumber = request.countryCode.Substring(1) + request.mobileNumber;
                Email_Sms_Sender.Send_Sms(message, mobilenumber);

                _response.otp = new_otp;
                //  _response.otp = new_otp;
                _messages.Add(new Message_Info { Message = "OTP sent successfully via SMS", Type = Message_Type.SUCCESS.ToString() });
                _registerService.Add_To_Notification_Queue(_response.UserId, "", "", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "success", "OTP Verification", request.mobileNumber, "SMS", "User", "OTP Sent Successfully");

                _statusCode = HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(Assembly.GetCallingAssembly().GetName().Name + "\n\t" + ex.ToString());

                _messages.Add(new Message_Info { Message = "Couldn't Send OTP Via SMS", Type = Message_Type.ERROR.ToString() });

                _statusCode = HttpStatusCode.BadRequest;
            }
        }

        private void Generate_Otp()
        {
            new_otp = Otp_Generator.Generate_Otp();
        }

        private void Update_Existing_User()
        {
            try
            {
                _response = _registerService.Update_User(request);

                _messages.Add(new Message_Info { Message = "User Updated Successfully", Type = Message_Type.SUCCESS.ToString() });

                _statusCode = HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(Assembly.GetCallingAssembly().GetName().Name + "\n\t" + ex.ToString());
            }
        }

        private void Insert_New_User()
        {
            try
            {
                _response = _registerService.Insert_User(request, new_otp);

                _messages.Add(new Message_Info { Message = "User Created Successfully", Type = Message_Type.SUCCESS.ToString() });

                _statusCode = HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(Assembly.GetCallingAssembly().GetName().Name + "\n\t" + ex.ToString());
            }
        }

        public void Dispose()
        {
            request = null;

            _response = null;

            _registerService = null;

            _messages = null;

            new_otp = null;
        }
    }
}
