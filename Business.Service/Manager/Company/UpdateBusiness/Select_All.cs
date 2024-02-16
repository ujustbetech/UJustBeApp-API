using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using Business.Service.Models.Company.UpdateBusiness;
using Business.Service.Repositories.Company;
using UJBHelper.Common;

namespace Business.Service.Manager.Company.UpdateBusiness
{
    public class Select_All : IDisposable
    {
        private IUpdateBusinessService _updateBusinessService;
        public HttpStatusCode _statusCode = HttpStatusCode.OK;
        public List<Message_Info> _messages = null;
        public List<Get_Request> _response = null;

        public Select_All(IUpdateBusinessService updateBusinessService)
        {
            _updateBusinessService = updateBusinessService;
            _messages = new List<Message_Info>();
        }

        public void Process()
        {
            Get_All_Business();
        }

        private void Get_All_Business()
        {
            try
            {
                _response = _updateBusinessService.Get_Business_List();

                _messages.Add(new Message_Info { Message = "Business created successfully", Type = Message_Type.SUCCESS.ToString() });

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
            _updateBusinessService = null;
            _messages = null;
            _response = null;
        }
    }
}
