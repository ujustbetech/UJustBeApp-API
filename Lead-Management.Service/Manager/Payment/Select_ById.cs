using Lead_Management.Service.Models.Payment;
using UJBHelper.Common;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using Lead_Management.Service.Repositories.Payment;

namespace Lead_Management.Service.Manager.Payment
{
    public class Select_ById : IDisposable
    {
        public Get_Request _response;
        private string _PaymentId;
        private IAddPaymentService _addPaymentService;
        public HttpStatusCode _statusCode = HttpStatusCode.OK;
        public List<Message_Info> _messages = null;

        public Select_ById(string PaymentId, IAddPaymentService addPaymentService)
        {
            _PaymentId = PaymentId;
            _addPaymentService = addPaymentService;
            _messages = new List<Message_Info>();
        }

        public void Process()
        {
            Get_PaymentDetails();
        }

        private void Get_PaymentDetails()
        {
            try
            {
                _response = _addPaymentService.Get_PaymentDetails(_PaymentId);

                _messages.Add(new Message_Info { Message = "Payment Details", Type = Message_Type.SUCCESS.ToString() });

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

        public void Dispose()
        {
            _PaymentId = null;
            _addPaymentService = null;
            _response = null;
            _messages = null;
        }
    }
}
