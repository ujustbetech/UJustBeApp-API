using Auth.Service.Models.Registeration.MentorList;
using Auth.Service.Respositories.Registeration;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using UJBHelper.Common;

namespace Auth.Service.Manager.Registeration.MentorLookup
{
    public class Insert : IDisposable
    {
        private Post_Request request;

        public List<Get_Request> _response;

        public HttpStatusCode _statusCode = HttpStatusCode.OK;

        private IMentorLookupService _mentorLookupService;

        public List<Message_Info> _messages = null;

        public Insert(Post_Request post_Request, IMentorLookupService MentorLookupService)
        {
            _messages = new List<Message_Info>();

            request = post_Request;

            _mentorLookupService = MentorLookupService;
        }

        public void Process()
        {
            try
            {
                _response = _mentorLookupService.Get_Mentor_By_Search(request.searchTerm);

                if(_response.Count < 1)
                {
                    _messages.Add(new Message_Info
                    {
                        Message = "No mentors found",
                        Type = Message_Type.INFO.ToString()
                    });

                    _statusCode = HttpStatusCode.OK;
                }
                else
                {
                    _messages.Add(new Message_Info
                    {
                        Message = "Mentors List",
                        Type = Message_Type.SUCCESS.ToString()
                    });

                    _statusCode = HttpStatusCode.OK;

                }
            }
            catch (Exception ex)
            {
                _messages.Add(new Message_Info
                {
                    Message = "Could not get Mentors List",
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

            _mentorLookupService = null;

            _statusCode = HttpStatusCode.OK;

            request = null;

            _response = null;
        }
    }
}
