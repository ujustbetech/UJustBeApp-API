using Auth.Service.Models.Registeration.Otp;
using Auth.Service.Respositories.Registeration;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using UJBHelper.Common;
using System.Threading.Tasks;

namespace Auth.Service.Manager.Registeration.Otp
{
    public class Update : IDisposable
    {
        private Put_Request request;

        public HttpStatusCode _statusCode = HttpStatusCode.OK;

        private IOtpService _otpService;
        private IRegisterService _registerService;
        public string new_otp = null;

        public List<Message_Info> _messages = null;

        private User_Contact_Details user_details = null;


        public Update(Put_Request request, IOtpService otpService)
        {
            _messages = new List<Message_Info>();

            _otpService = otpService;
                

            this.request = request;
        }

        public void Process()
        {
            if (Verify_User())
            {
                Get_User_Details();

                ReGenerate_Otp();
                 SendOTPNotification("", "", request.userId, new_otp);
                //var nq = new Notification_Queue();
                // nq.Add_To_Queue(request.userId, "", "", "", "new", "OTP Verification", "", "SMS", "User", "");
              //  Resend_Otp_Via_Sms();

                // Resend_Otp_Via_Email();
            }
        }

        public Task SendOTPNotification(string firstName, string lastName, string UserId, string new_otp)
        {
            MessageBody MB = new MessageBody();
           // MB.UserName = firstName + " " + lastName;
            MB.new_otp = new_otp;
            var nq = new Notification_Sender();
            nq.SendNotification("OTP Send", MB, UserId, "", "");
            return Task.CompletedTask;
        }

        //private void Resend_Otp_Via_Email()
        //{
        //    try
        //    {
        //        string fullName = user_details.FirstName + " " + user_details.LastName;

        //        string subject = "(UJustBe) New Password for " + fullName;

        //        string body = $"Hello {fullName}, Your New Password For UJustBe Login is :- {new_otp}";

        //        Email_Sms_Sender.Send_Email(user_details.EmailId, fullName, subject, body, new_otp);

        //        _messages.Add(new Message_Info { Message = "OTP sent successfully via Email", Type = Message_Type.SUCCESS.ToString() });

        //        _statusCode = HttpStatusCode.OK;
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Log.Error(Assembly.GetCallingAssembly().GetName().Name + "\n\t" + ex.ToString());

        //        _messages.Add(new Message_Info { Message = "Couldn't Send Email", Type = Message_Type.ERROR.ToString() });

        //        _statusCode = HttpStatusCode.BadRequest;
        //    }
        //}

        private void Get_User_Details()
        {
            try
            {
                user_details = _otpService.Get_User_Details(request.userId);
            }
            catch (Exception ex)
            {
                Logger.Log.Error(Assembly.GetCallingAssembly().GetName().Name + "\n\t" + ex.ToString());
            }
        }

        private bool Verify_User()
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

        private void Resend_Otp_Via_Sms()
        {
            try
            {
                string fullName = user_details.FirstName + " " + user_details.LastName;

                // var message = $" Hello { fullName },{ new_otp} is your OTP for UJUSTB App. use this OTP to continue.";
                var message = $" Hello { fullName }, { new_otp} is your OTP to verify yourself on UJustBe App. Please use this OTP to continue.";
                var mobilenumber = request.countryCode.Substring(1) + request.MobileNumber;

                if (!string.IsNullOrEmpty(request.MobileNumber))
                {
                    user_details.MobileNo = mobilenumber;
                }
                Email_Sms_Sender.Send_Sms(message, user_details.MobileNo);

                _messages.Add(new Message_Info { Message = "OTP sent successfully via SMS", Type = Message_Type.SUCCESS.ToString() });
               // _registerService.Add_To_Notification_Queue(request.userId, "", "", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "success", "OTP Verification", request.MobileNumber, "SMS", "User", "OTP Re-sent Successfully");

                _statusCode = HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(Assembly.GetCallingAssembly().GetName().Name + "\n\t" + ex.ToString());

                _messages.Add(new Message_Info { Message = "Couldn't Send OTP Via SMS", Type = Message_Type.ERROR.ToString() });

                _statusCode = HttpStatusCode.BadRequest;
            }
        }

        private void ReGenerate_Otp()
        {
            try
            {
                new_otp = Otp_Generator.Generate_Otp();

                _otpService.Update_Otp(request.userId, new_otp);

                _messages.Add(new Message_Info
                {
                    Message = "Otp Generated Successfully",
                    Type = Message_Type.SUCCESS.ToString()
                });

                _statusCode = HttpStatusCode.OK;

            }
            catch (Exception ex)
            {
                _messages.Add(new Message_Info
                {
                    Message = "Otp Could not be Generated ",
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

            new_otp = null;

            user_details = null;
        }
    }

    public class User_Contact_Details
    {
        public string MobileNo { get; set; }
        public string EmailId { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
