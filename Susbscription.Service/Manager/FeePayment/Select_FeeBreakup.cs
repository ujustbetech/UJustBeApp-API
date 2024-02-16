using Susbscription.Service.Models.FeePayment;
using Susbscription.Service.Repositories.FeePayment;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using UJBHelper.Common;
namespace Susbscription.Service.Manager.FeePayment
{
    public class Select_FeeBreakup:IDisposable
    {
        public Get_FeeBreakup _response;
        private string _userId;
        private string _feeType;

        private IFeePaymentService _getFeeTypeService;
        public HttpStatusCode _statusCode = HttpStatusCode.OK;
        public List<Message_Info> _messages = null;

        public Select_FeeBreakup(string UserId, string FeeType,IFeePaymentService getFeeTypeService)
        {
            _userId = UserId;
            _feeType = FeeType;
            _getFeeTypeService = getFeeTypeService;
            _messages = new List<Message_Info>();
        }
        public void Process()
        {
            Get_FeeBreakup();
        }
        private void Get_FeeBreakup()
        {
            try
            {
                _response = _getFeeTypeService.Get_FeeBreakup(_userId, _feeType);
                if (_response._messages.Count > 0)
                {
                    _messages.AddRange(_response._messages);
                }
                _messages.Add(new Message_Info { Message = "Fee BreakUp List", Type = Message_Type.SUCCESS.ToString() });

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
            _getFeeTypeService = null;
            _userId = null;
            _response = null;
            _messages = null;
        }

    }
}
