using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using Business.Service.Models.Company.UpdateCompanyAddress;
using Business.Service.Repositories.Company;
using UJBHelper.Common;

namespace Business.Service.Manager.Company.UpdateCompanyAddress
{
    public class Insert : IDisposable
    {
        public HttpStatusCode _statusCode = HttpStatusCode.OK;
        public List<Message_Info> _messages = null;
        private Post_Request request;
        private IUpdateCompanyService _updateCompanyService;
        private string new_latitude = null;
        private string new_longitude = null;

        public Insert(Post_Request request, IUpdateCompanyService updateCompanyService)
        {
            this.request = request;
            _updateCompanyService = updateCompanyService;
            _messages = new List<Message_Info>();
        }

        public void Process()
        {
            if (Verify_Business())
            {
                //Get_Coordinates_From_Address();

                Update_Company_Address();
            }
        }

        //private void Get_Coordinates_From_Address()
        //{
        //    try
        //    {
        //        var res = _updateCompanyService.Get_Coordinates_From_Address(request);

        //        if (res != "NONE")
        //        {
        //            new_latitude = res.Split(",")[0];
        //            new_longitude = res.Split(",")[1];

        //            request.latitude = double.Parse(new_latitude);
        //            request.longitude = double.Parse(new_longitude);

        //            _messages.Add(new Message_Info
        //            {
        //                Message = "Address Converted to Coordinates Successfully",
        //                Type = Message_Type.SUCCESS.ToString()
        //            });

        //            _statusCode = HttpStatusCode.OK;
        //        }
        //        else
        //        {
                   
        //            _messages.Add(new Message_Info
        //            {
        //                Message = "Couldn't Convert Address to Coordinates",
        //                Type = Message_Type.ERROR.ToString()
        //            });

        //            _statusCode = HttpStatusCode.InternalServerError;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.Log.Error(Assembly.GetCallingAssembly().GetName().Name + "\n\t" + ex.ToString());

        //        _messages.Add(new Message_Info
        //        {
        //            Message = "Couldn't Convert Address to Coordinates (Exception)",
        //            Type = Message_Type.ERROR.ToString()
        //        });

        //        _statusCode = HttpStatusCode.InternalServerError;

        //    }
        //}

        private bool Verify_Business()
        {
            try
            {
                if (_updateCompanyService.Check_If_Business_Exists(request.businessId))
                {
                    return true;
                }
                _messages.Add(new Message_Info
                {
                    Message = "No Business Found",
                    Type = Message_Type.ERROR.ToString()
                });

                _statusCode = HttpStatusCode.NotFound;

                return false;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(Assembly.GetCallingAssembly().GetName().Name + "\n\t" + ex.ToString());
                _messages.Add(new Message_Info
                {
                    Message = "No Business Found",
                    Type = Message_Type.ERROR.ToString()
                });

                _statusCode = HttpStatusCode.NotFound;

                return false;
            }
        }

        private void Update_Company_Address()
        {
            try
            {
                _updateCompanyService.UpdateCompanyAddress(request);

                _messages.Add(new Message_Info { Message = "Company's Address updated successfully", Type = Message_Type.SUCCESS.ToString() });

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
            new_latitude = null;

            new_longitude = null;

            request = null;

            _updateCompanyService = null;

            _statusCode = HttpStatusCode.OK;

            _messages = null;
        }
    }
}
