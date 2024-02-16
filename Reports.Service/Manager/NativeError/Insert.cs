using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Reports.Service.Models.NativeError;
using Reports.Service.Repositories.NativeError;
using UJBHelper.Common;

namespace Reports.Service.Manager.NativeError
{
    public class Insert : IDisposable
    {
        private Post_Request request;
        private INativeErrorService _nativeErrorService;
        public HttpStatusCode _statusCode = HttpStatusCode.OK;
        public List<Message_Info> _messages = null;
        private IConfiguration _iconfiguration;
        public Insert(Post_Request request, INativeErrorService nativeErrorService, IConfiguration iConfiguration)
        {
            this.request = request;
            _nativeErrorService = nativeErrorService;
            _messages = new List<Message_Info>();
            _iconfiguration = iConfiguration;
        }

        public void Process()
        {
           // Insert_Error_Log();
        }

        private void Insert_Error_Log()
        {
            try
            {
                //string FileDestination = System.IO.Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory());
                //FileDestination = FileDestination + _iconfiguration["LogPath"];
                //_nativeErrorService.Insert_Error_Log(request, FileDestination);
                //_messages.Add(new Message_Info
                //{
                //    Message = "Error Log Inserted Successfully",
                //    Type = Message_Type.SUCCESS.ToString()
                //});

                _statusCode = HttpStatusCode.OK;
                
            }
            catch (Exception ex)
            {
                Logger.Log.Error(Assembly.GetCallingAssembly().GetName().Name + "\n\t" + ex.ToString());
                _messages.Add(new Message_Info
                {
                    Message = "Error Inserting Log",
                    Type = Message_Type.ERROR.ToString()
                });

                _statusCode = HttpStatusCode.InternalServerError;
            }
        }

        public void Dispose()
        {
            request = null;
            _nativeErrorService = null;
            _statusCode = HttpStatusCode.OK;
            _messages = null;
        }
    }
}
