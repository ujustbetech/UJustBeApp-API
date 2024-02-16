using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Business.Service.Models.Company.UpdateBusiness;
using Business.Service.Repositories.Company;
using UJBHelper.Common;

namespace Business.Service.Manager.Company.UpdateBusiness
{
    public class Insert : IDisposable
    {
        public string _businessId = null;
        public HttpStatusCode _statusCode = HttpStatusCode.OK;
        public List<Message_Info> _messages = null;
        private Post_Request request;
        private IUpdateBusinessService _updateBusinessService;
        private IUpdateCompanyService _updateCompanyService;
        private string new_latitude = null;
        private string new_longitude = null;


        public Insert(Post_Request request1, IUpdateBusinessService updateBusinessService)
        {
            request = request1;
            _updateBusinessService = updateBusinessService;

            _messages = new List<Message_Info>();
        }

        public void Process()
        {
            request.businessId = Check_If_User_Bussiness_Exists();
            if (string.IsNullOrWhiteSpace(request.businessId))
            {
                Get_Coordinates_From_Address();
                Insert_New_Business();
            }
            else
            {
                if (Verify_Business())
                {
                    Get_Coordinates_From_Address();
                    string details = Update_Business_Details();
                    if (details != "")
                    {
                        var sendnotifiation = SendNotification(details, request.businessId);
                    }
                }
            }

        }
        private void Get_Coordinates_From_Address()
        {
            try
            {
                var res = _updateBusinessService.Get_Coordinates_From_Address(request.flatWing, request.location, request.locality);

                if (res != "NONE")
                {
                    new_latitude = res.Split(",")[0];
                    new_longitude = res.Split(",")[1];

                    request.latitude = double.Parse(new_latitude);
                    request.longitude = double.Parse(new_longitude);

                    _messages.Add(new Message_Info
                    {
                        Message = "Address Converted to Coordinates Successfully",
                        Type = Message_Type.SUCCESS.ToString()
                    });

                    _statusCode = HttpStatusCode.OK;
                }
                else
                {

                    _messages.Add(new Message_Info
                    {
                        Message = "Couldn't Convert Address to Coordinates",
                        Type = Message_Type.ERROR.ToString()
                    });

                    _statusCode = HttpStatusCode.InternalServerError;
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(Assembly.GetCallingAssembly().GetName().Name + "\n\t" + ex.ToString());

                _messages.Add(new Message_Info
                {
                    Message = "Couldn't Convert Address to Coordinates (Exception)",
                    Type = Message_Type.ERROR.ToString()
                });

                _statusCode = HttpStatusCode.InternalServerError;

            }
        }

        private void Insert_New_Business()
        {
            try
            {
                _businessId = _updateBusinessService.Insert_Business_Categories(request);

                _messages.Add(new Message_Info { Message = "Business created successfully", Type = Message_Type.SUCCESS.ToString() });

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

        private bool Verify_Business()
        {
            try
            {
                if (_updateBusinessService.Check_If_Business_Exists(request.businessId, request.userId))
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

        private string Check_If_User_Bussiness_Exists()
        {
            try
            {
                return _updateBusinessService.Check_If_User_Bussiness_Exists(request.userId);
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

                return "";
            }
        }

        private string Update_Business_Details()
        {
            try
            {
                Post_Request _postresult = new Post_Request();
                string details = "";
                _postresult = _updateBusinessService.Update_Business_Categories(request);
                //if (!string.IsNullOrWhiteSpace(_postresult.flatWing) || !string.IsNullOrWhiteSpace(_postresult.locality) || !string.IsNullOrWhiteSpace(_postresult.location))
                //{
                //    details = "the bussiness address details changes";
                //}
                if (!string.IsNullOrWhiteSpace(_postresult.flatWing))
                {
                    details += "\"Flat Wing\", ";
                }
                if (!string.IsNullOrWhiteSpace(_postresult.locality))
                {
                    details += "\"Locality\", ";
                }
                if (!string.IsNullOrWhiteSpace(_postresult.location))
                {
                    details += "\"Location\", ";
                }
                if (!string.IsNullOrWhiteSpace(_postresult.BusinessDescription))
                {
                    details += "\"Bussiness Description\", ";
                }

                if (details != "")
                {
                    details = details.Trim();
                    details = details.TrimEnd(',');

                }
                _businessId = request.businessId;
                _messages.Add(new Message_Info { Message = "Business details updated successfully", Type = Message_Type.SUCCESS.ToString() });

                _statusCode = HttpStatusCode.OK;
                return details;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(Assembly.GetCallingAssembly().GetName().Name + "\n\t" + ex.ToString());

                _messages.Add(new Message_Info { Message = "Exception Occured", Type = Message_Type.ERROR.ToString() });

                _statusCode = HttpStatusCode.InternalServerError;
                throw;
            }

        }

        public Task SendNotification(string details, string bussinessId)
        {
            MessageBody MB = new MessageBody();
            MB.details = details;

            var nq = new Notification_Sender();

            nq.SendNotification("Product Details Updated", MB, "", "", bussinessId);
            return Task.CompletedTask;

        }


        public void Dispose()
        {
            request = null;

            _businessId = null;

            _updateBusinessService = null;

            _statusCode = HttpStatusCode.OK;

            _messages = null;
        }
    }
}
