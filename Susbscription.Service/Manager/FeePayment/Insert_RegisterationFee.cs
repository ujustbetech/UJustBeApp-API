using Susbscription.Service.Models.FeePayment;
using Susbscription.Service.Repositories.FeePayment;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using UJBHelper.Common;

namespace Susbscription.Service.Manager.FeePayment
{
    public class Insert_RegisterationFee:IDisposable
    {
        private Post_RegisterationFee request;
        private IFeePaymentService _addFeePaymentService;
        public HttpStatusCode _statusCode = HttpStatusCode.OK;
        public List<Message_Info> _messages = null;

        public Insert_RegisterationFee(Post_RegisterationFee request, IFeePaymentService addFeePaymentService)
        {
            this.request = request;
            _addFeePaymentService = addFeePaymentService;
            _messages = new List<Message_Info>();
        }

        public void Process()
        {
            Adjust_Partner_RegisterationFee();   
        }

        private void Adjust_Partner_RegisterationFee()
        {
            try
            {
                _addFeePaymentService.Adjust_Partner_RegisterationFee(request);

                _messages.Add(new Message_Info
                {
                    Message = "New Fee Payment Created",
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
            request = null;

            _addFeePaymentService = null;

            _statusCode = HttpStatusCode.OK;

            _messages = null;
        }
    }
}
