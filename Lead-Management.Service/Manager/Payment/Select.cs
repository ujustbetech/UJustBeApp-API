using Lead_Management.Service.Models.Payment;
using UJBHelper.Common;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using Lead_Management.Service.Repositories.Payment;

namespace Lead_Management.Service.Manager.Payment
{
    public class Select:IDisposable
    {
        private string LeadId;
        private IAddPaymentService _addPaymentService;
        public HttpStatusCode _statusCode = HttpStatusCode.OK;
        public List<Message_Info> _messages = null;
        public Get_Request _response = null;

        public Select(string LeadId, IAddPaymentService addPaymentService)
        {
            this.LeadId = LeadId;
            _addPaymentService = addPaymentService;
            _messages = new List<Message_Info>();
        }

        public void Process()
        {
            if (Verify_Lead())
            {
                Get_Payment_List();
            }
        }

        private void Get_Payment_List()
        {
            try
            {
                _response = _addPaymentService.Get_Payment_List(LeadId);

                _messages.Add(new Message_Info
                {
                    Message = "Referral Details",
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

        private bool Verify_Lead()
        {
            try
            {
                if (_addPaymentService.Check_If_Lead_Exist(LeadId))
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

        public void Dispose()
        {
            _messages = null;
            _addPaymentService = null;
            _response = null;
            _statusCode = HttpStatusCode.OK;
            LeadId = null;
        }
    }
}
