using Susbscription.Service.Models.UserDetails;
using Susbscription.Service.Repositories.FeePayment;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using UJBHelper.Common;

namespace Susbscription.Service.Manager.UserDetails
{
    public class Select_OtherDetails : IDisposable
    {
        public GetUserOtherDetails _response = null;
        private string _UserId;
        private IFeePaymentService _addPaymentService;
        public HttpStatusCode _statusCode = HttpStatusCode.OK;
        public List<Message_Info> _messages = null;

        public Select_OtherDetails(string userId, IFeePaymentService addPaymentService)
        {
            _UserId = userId;
            _addPaymentService = addPaymentService;
            _messages = new List<Message_Info>();
        }
        public void Process()
        {
            GetUserOtherDetails();
        }
        internal void GetUserOtherDetails()
        {
            try
            {
                _response = _addPaymentService.Get_Users_OtherDetails(_UserId);

                _messages.Add(new Message_Info { Message = "Users Details", Type = Message_Type.SUCCESS.ToString() });

                _statusCode = HttpStatusCode.OK;
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
            _UserId = null;
            _messages = null;
            _statusCode = HttpStatusCode.OK;
            _response = null;
        }
    }
}
