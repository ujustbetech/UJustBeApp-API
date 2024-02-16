using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Business.Service.Models.ApproveBusinessKYC;
using Business.Service.Repositories.ApproveBusinessKYC;
using UJBHelper.Common;

namespace Business.Service.Manager.ApproveBusinessKYC
{
    public class Update : IDisposable
    {
        private Put_Request request;
        private IApproveBusinessKYCService _approveBusinessKYCService;
        public HttpStatusCode _statusCode = HttpStatusCode.OK;
        public List<Message_Info> _messages = null;

        public Update(Put_Request request, IApproveBusinessKYCService approveBusinessKYCService)
        {
            this.request = request;
            _approveBusinessKYCService = approveBusinessKYCService;
            _messages = new List<Message_Info>();

        }

        internal void Process()
        {
            if (Verify_Business())
            {
                if (request.isApproved == 1)
                {
                    if (Check_If_SusbscriptionPaymentDone())
                    {
                        AddSubscriptionDetails();
                        request.isSubscriptionActive = true;
                        Update_KYC_Details();
                   }
                }
                else
                {
                    request.isSubscriptionActive = false;
                    Update_KYC_Details();
                }
               
            }
        }  

        private bool Check_If_SusbscriptionPaymentDone()
        {
            try
            {
                if (_approveBusinessKYCService.Check_If_SusbscriptionPaymentDone(request.businessId))
                {
                    return true;
                }
                _messages.Add(new Message_Info
                {
                    Message = "Membership amount is pending",
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
                    Message = "Membership amount is pending",
                    Type = Message_Type.ERROR.ToString()
                });

                _statusCode = HttpStatusCode.NotFound;

                return false;
            }
        }

        private void AddSubscriptionDetails()
        {
            try
            {
                _approveBusinessKYCService.AddSubscriptionDetails(request.businessId);

                _messages.Add(new Message_Info
                {
                    Message = "Business KYC Status Updated",
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
            }
        }

        private void Update_KYC_Details()
        {
            try
            {
                _approveBusinessKYCService.Update_KYC_Details(request);

                _messages.Add(new Message_Info
                {
                    Message = "Business KYC Status Updated",
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
            }
        }

        private bool Verify_Business()
        {
            try
            {
                if (_approveBusinessKYCService.Check_If_Business_Exists(request.businessId))
                {
                    return true;
                }
                _messages.Add(new Message_Info
                {
                    Message = "No Business Found",
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
                    Message = "No Business Found",
                    Type = Message_Type.ERROR.ToString()
                });

                _statusCode = HttpStatusCode.NotFound;

                return false;
            }
        }

        public void Dispose()
        {
            request = null;

            _approveBusinessKYCService = null;

            _statusCode = HttpStatusCode.OK;

            _messages = null;
        }
    }
}
