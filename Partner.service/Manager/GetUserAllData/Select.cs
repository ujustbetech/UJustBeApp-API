using Partner.Service.Models.Partners.GetAllDetails;
using Partner.Service.Repositories.GetPartnerService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using UJBHelper.Common;

namespace Partner.Service.Manager.GetUserAllData
{
    public class Select : IDisposable
    {
        public Get_Details_Excel _response;
        public string _UserId;
        private IGetPartnerService _PartnerDetailsService;
        public HttpStatusCode _statusCode = HttpStatusCode.OK;
        public List<Message_Info> _messages = null;

        public Select(IGetPartnerService PartnerDetailsService)
        {
           
            _PartnerDetailsService = PartnerDetailsService;
            _messages = new List<Message_Info>();
        }

        public void Process()
        {
            GetUserAllDetails();
        }

        private void GetUserAllDetails()
        {
            try
            {
                _response = _PartnerDetailsService.GetUserAllDetails();

                //_messages.Add(new Message_Info { Message = " List", Type = Message_Type.SUCCESS.ToString() });

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
            _PartnerDetailsService = null;
            _response = null;
            _messages = null;
        }
    }
}
