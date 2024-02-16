using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using Partner.Service.Models.Partners;
using Partner.Service.Repositories.GetPartnerService;
using UJBHelper.Common;

namespace Partner.Service.Manager.GetPartnerService
{
    public class Select : IDisposable
    {
        public Get_Request _response;
        private int size;
        private int page;
        private string _firstname,_lastname,_mobile,_role;
        private IGetPartnerService _getPartnerService;
        public HttpStatusCode _statusCode = HttpStatusCode.OK;
        public List<Message_Info> _messages = null;

        public Select(int size, int page, IGetPartnerService getPartnerService)
        {
            this.size = size;
            this.page = page;
            //_firstname = FirstName;
            //_lastname = LastName;
            //_mobile = Mobile;
            //_role = Role;
            _getPartnerService = getPartnerService;
            _messages = new List<Message_Info>();
        }

        public void Process()
        {
            Get_Partners();
        }

        private void Get_Partners()
        {
            try
            {
                _response = _getPartnerService.Get_PartnerList(size, page);

                _messages.Add(new Message_Info { Message = "Partners List", Type = Message_Type.SUCCESS.ToString() });

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
            size = 0;
            page = 0;
            _firstname = null;
            _lastname = null;
            _mobile = null;
            _role = null;
            _getPartnerService = null;
            _response = null;
            _messages = null;
        }
    }
}
