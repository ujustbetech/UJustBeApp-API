using Lead_Management.Service.Repositories.ReferralStatus;
using UJBHelper.Common;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using Lead_Management.Service.Models.ReferralStatus;

namespace Lead_Management.Service.Manager.ReferralStatus
{
    public class Select : IDisposable
    {
        private int StatusId;
        private IReferralStatusService _referralStatusService;
        public HttpStatusCode _statusCode = HttpStatusCode.OK;
        public List<Message_Info> _messages = null;
        public Get_Request _response = null;

        public Select(int StatusId, IReferralStatusService referralStatusService)
        {
            this.StatusId = StatusId;
            _referralStatusService = referralStatusService;
            _messages = new List<Message_Info>();
        }

        public void Process()
        {
            if (Check_If_Status_Exist())
            {
                Get_Dependent_Status_Details();
            }
        }

        private void Get_Dependent_Status_Details()
        {
            try
            {
                _response = _referralStatusService.Get_Dependent_Status_Details(StatusId);

                _messages.Add(new Message_Info
                {
                    Message = "Status Details",
                    Type = Message_Type.SUCCESS.ToString()
                });

                _statusCode = HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(Assembly.GetCallingAssembly().GetName().Name + "\n\t" + ex.ToString());

                _messages.Add(new Message_Info
                {
                    Message = "Exception Occured",
                    Type = Message_Type.ERROR.ToString()
                });

                _statusCode = HttpStatusCode.InternalServerError;
                throw;
            }
        }

        private bool Check_If_Status_Exist()
        {
            try
            {
                //string stringValue = Enum.GetName(typeof(ReferralStatusEnum), StatusId);
                if (_referralStatusService.Check_If_Status_Exist(StatusId))
                {
                    return true;
                }
                _messages.Add(new Message_Info
                {
                    Message = "No Dependent Status Found",
                    Type = Message_Type.ERROR.ToString()
                });

                _statusCode = HttpStatusCode.NotFound;

                return false;
            }
            catch (Exception ex)
            {
                _messages.Add(new Message_Info
                {
                    Message = "No Dependent Status Found",
                    Type = Message_Type.ERROR.ToString()
                });

                _statusCode = HttpStatusCode.NotFound;

                return false;
            }
        }

        public void Dispose()
        {
            _messages = null;
            _referralStatusService = null;
            _response = null;
            _statusCode = HttpStatusCode.OK;
            StatusId = 0;
        }

    }

    public class SelectDependent : IDisposable
    {
        private int StatusId;
        private IReferralStatusService _referralStatusService;
        public HttpStatusCode _statusCode = HttpStatusCode.OK;
        public List<Message_Info> _messages = null;
        public Get_Request _response = null;

        public SelectDependent(int StatusId, IReferralStatusService referralStatusService)
        {
            this.StatusId = StatusId;
            _referralStatusService = referralStatusService;
            _messages = new List<Message_Info>();
        }

        public void Process()
        {
            if (Check_If_Status_Exist())
            {
                Get_Dependent_Status();
            }
        }

        private void Get_Dependent_Status()
        {
            try
            {
                _response = _referralStatusService.Get_Dependent_Status(StatusId);

                _messages.Add(new Message_Info
                {
                    Message = "Status Details",
                    Type = Message_Type.SUCCESS.ToString()
                });

                _statusCode = HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(Assembly.GetCallingAssembly().GetName().Name + "\n\t" + ex.ToString());

                _messages.Add(new Message_Info
                {
                    Message = "Exception Occured",
                    Type = Message_Type.ERROR.ToString()
                });

                _statusCode = HttpStatusCode.InternalServerError;
                throw;
            }
        }

        private bool Check_If_Status_Exist()
        {
            try
            {
                //string stringValue = Enum.GetName(typeof(ReferralStatusEnum), StatusId);
                if (_referralStatusService.Check_If_Status_Exist(StatusId))
                {
                    return true;
                }
                _messages.Add(new Message_Info
                {
                    Message = "No Dependent Status Found",
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
                    Message = "No Dependent Status Found",
                    Type = Message_Type.ERROR.ToString()
                });

                _statusCode = HttpStatusCode.NotFound;

                return false;
            }
        }

        public void Dispose()
        {
            _messages = null;
            _referralStatusService = null;
            _response = null;
            _statusCode = HttpStatusCode.OK;
            StatusId = 0;
        }

    }

}
