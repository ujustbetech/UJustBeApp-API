using UJBHelper.Common;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using Lead_Management.Service.Repositories.Payment;
using Lead_Management.Service.Models.Payment;

namespace Lead_Management.Service.Manager.Payment
{
    public class Select_Balance:IDisposable
    {
        private Put_Request request;
        private IAddPaymentService _addPaymentService;
        public HttpStatusCode _statusCode = HttpStatusCode.OK;
        public List<Message_Info> _messages = null;
        public GetBalance _response;

        public Select_Balance(Put_Request request, IAddPaymentService addPaymentService)
        {
            this.request = request;
            _addPaymentService = addPaymentService;
            _messages = new List<Message_Info>();
        }

        public void Process()
        {
            if (Verify_Lead())
            {
                Calculate_Balance();
            }
           
        }

        public void Calculate_Balance()
        {
            try {
                _response = _addPaymentService.Calculate_Balance(request);

                _messages.Add(new Message_Info
                {
                    Message = "Payment Balance Details",
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

        public void Dispose()
        {
            _messages = null;
            _addPaymentService = null;
            _response = null;
            _statusCode = HttpStatusCode.OK;
            request = null;
        }

        private bool Verify_Lead()
        {
            try
            {
                if (_addPaymentService.Check_If_Lead_Exist(request.leadId))
                {
                    return true;
                }
                _messages.Add(new Message_Info
                {
                    Message = "No Lead Found",
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
                    Message = "No Referral Found",
                    Type = Message_Type.ERROR.ToString()
                });

                _statusCode = HttpStatusCode.NotFound;

                return false;
            }
        }
    }
}
