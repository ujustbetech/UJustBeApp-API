using Auth.Service.Models.MobileVersion;
using Auth.Service.Respositories.MobileVersion;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using UJBHelper.Common;

namespace Auth.Service.Manager.MobileVersion
{
    public class Select : IDisposable
    {
        public Get_Request _response;

        private string _Type;

        public HttpStatusCode _statusCode = HttpStatusCode.OK;

        private IGetVersionDetails _versionService;

        public List<Message_Info> _messages = null;

        public Select(string type, IGetVersionDetails VersionService)
        {
            _Type = type;

            _messages = new List<Message_Info>();

            _versionService = VersionService;
        }

        public void Process()
        {
            Get_Version_Details();
        }
        private void Get_Version_Details()
        {
            try
            {
                _response = _versionService.GetVersionDetails(_Type);

                _messages.Add(new Message_Info
                {
                    Message = "Mobile Version details found Successfully",
                    Type = Message_Type.SUCCESS.ToString()
                });

                _statusCode = HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                _messages.Add(new Message_Info
                {
                    Message = "Could not get Mobile Version details",
                    Type = Message_Type.ERROR.ToString()
                });

                _statusCode = HttpStatusCode.InternalServerError;

                Logger.Log.Error(Assembly.GetCallingAssembly().GetName().Name + "\n\t" + ex.ToString());

                throw;
            }

        }

        public void Dispose()
        {
            _messages = null;

            _versionService = null;

            _statusCode = HttpStatusCode.OK;

            _response = null;

            _Type = null;
        }

        
    }
}
