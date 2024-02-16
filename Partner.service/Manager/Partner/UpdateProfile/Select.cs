using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using Partner.Service.Repositories.Partner;
using UJBHelper.Common;

namespace Partner.Service.Manager.Partner.UpdateProfile
{
    public class Select : IDisposable
    {
        public HttpStatusCode _statusCode = HttpStatusCode.OK;
        public List<Message_Info> _messages = null;
        public string _UserId = null;
        public string _MobileNo = null;
        public string _countryCode = null;
        public string new_otp=null;
        private IUpdatePartnerProfile _updatePartnerProfileService;


        public Select(string UserId,string MobileNo, string CountryCode,IUpdatePartnerProfile updatePartnerProfileService)
        {
            _UserId = UserId;
            _MobileNo = MobileNo;
            _countryCode = CountryCode;
            _updatePartnerProfileService = updatePartnerProfileService;
            _messages = new List<Message_Info>();
        }
        public void Process()
        {
            try
            {
                if (Verify_User())
                {
                    if (Verify_UserIsActive())
                    {
                        new_otp = Generate_Otp();
                        if (!string.IsNullOrEmpty(new_otp))
                        {
                            Send_Otp_Via_Sms();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(Assembly.GetCallingAssembly().GetName().Name + "\n\t" + ex.ToString());
            }
        }

        private string Generate_Otp()
        {
            string otp = Otp_Generator.Generate_Otp();
            _updatePartnerProfileService.Update_Otp(_UserId, otp);
            return otp;
        }

        private void Send_Otp_Via_Sms()
        {
            string fullName = "";//request.firstName + " " + request.lastName;
            try
            {
                var message = $" Hello { fullName },{ new_otp} is your OTP for UJUSTB App. use this OTP to continue.";
              //  var message = $"{new_otp} is your OTP for UJUSTB App. use this OTP to continue.";
                var mobilenumber = _countryCode.Substring(1) + _MobileNo;
                Email_Sms_Sender.Send_Sms(message, mobilenumber);               
                _messages.Add(new Message_Info { Message = "OTP sent successfully via SMS", Type = Message_Type.SUCCESS.ToString() });

                _statusCode = HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(Assembly.GetCallingAssembly().GetName().Name + "\n\t" + ex.ToString());

                _messages.Add(new Message_Info { Message = "Couldn't Send OTP Via SMS", Type = Message_Type.ERROR.ToString() });

                _statusCode = HttpStatusCode.BadRequest;
            }
        }

        

        private bool Verify_User()
        {
            try
            {
                if (_updatePartnerProfileService.Check_If_User_Exist(_UserId))
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
                if (_updatePartnerProfileService.Check_If_User_IsActive(_UserId))
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
            _UserId = null;

            _updatePartnerProfileService = null;

            _statusCode = HttpStatusCode.OK;

            _messages = null;
        }
    }
}
