
using Susbscription.Service.Models.UserDetails;
using Susbscription.Service.Repositories.FeePayment;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using UJBHelper.Common;

namespace Susbscription.Service.Manager.UserDetails
{
    public class Select : IDisposable
    {
        public List<Get_Suggestion> _response = null;
        private string query;
        private IFeePaymentService _addPaymentService;
        public HttpStatusCode _statusCode = HttpStatusCode.OK;
        public List<Message_Info> _messages = null;

        public Select(string query, IFeePaymentService addPaymentService)
        {
            this.query = query;
            _addPaymentService = addPaymentService;
            _messages = new List<Message_Info>();
        }
        internal void Process()
        {
            try
            {
                _response = _addPaymentService.Get_Users_Suggestion(query);

                if (_response.Count > 0)
                {
                    _messages.Add(new Message_Info { Message = "Users list", Type = Message_Type.SUCCESS.ToString() });

                    _statusCode = HttpStatusCode.OK;
                }
                else
                {
                    _messages.Add(new Message_Info { Message = "No User Found", Type = Message_Type.INFO.ToString() });

                    _statusCode = HttpStatusCode.NotFound;
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(Assembly.GetCallingAssembly().GetName().Name + "\n\t" + ex.ToString());

                _messages.Add(new Message_Info { Message = "Exception Occured", Type = Message_Type.ERROR.ToString() });

                _statusCode = HttpStatusCode.InternalServerError;
            }

        }

        public void Dispose()
        {
            _addPaymentService = null;
            query = null;
            _messages = null;
            _statusCode = HttpStatusCode.OK;
            _response = null;
        }
    }
}
