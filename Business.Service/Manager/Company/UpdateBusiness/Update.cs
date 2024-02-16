using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using Business.Service.Models.Company.UpdateBusiness;
using Business.Service.Repositories.Company;
using Microsoft.Extensions.Configuration;
using UJBHelper.Common;

namespace Business.Service.Manager.Company.UpdateBusiness
{
    public class Update : IDisposable
    {
        private Put_Request request;
        private IUpdateBusinessService _updateBusinessService;
        public HttpStatusCode _statusCode = HttpStatusCode.OK;
        public List<Message_Info> _messages = null;
        private IConfiguration _iconfiguration;
        public Update(Put_Request request, IUpdateBusinessService updateBusinessService, IConfiguration iconfiguration)
        {
            this.request = request;
            _updateBusinessService = updateBusinessService;
            _messages = new List<Message_Info>();
            _iconfiguration = iconfiguration;
        }

        public void Process()
        {
            if (Verify_User())
            {
                if (string.IsNullOrWhiteSpace(request.businessId))
                {
                    Insert_New_Business();
                }
                else
                {
                    if (Verify_Business())
                    {
                        Update_Business_Details();
                    }
                }
            }
        }

        private void Update_Business_Details()
        {
            try
            {
                _updateBusinessService.Update_Business_Admin(request);

                _messages.Add(new Message_Info
                {
                    Message = "Business Details Updated",
                    Type = Message_Type.SUCCESS.ToString()
                });

                _statusCode = HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(Assembly.GetCallingAssembly().GetName().Name + "\n\t" + ex.ToString());

                _messages.Add(new Message_Info
                {
                    Message = "Exception Occured",
                    Type = Message_Type.ERROR.ToString()
                });

                _statusCode = HttpStatusCode.InternalServerError;
                throw;
            }
        }

        private bool Verify_Business()
        {
            try
            {
                if (_updateBusinessService.Check_If_Business_Exists(request.businessId,request.userId))
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

        private void Insert_New_Business()
        {
            try
            {

                if (request.logoBase64.Contains(";base64,"))
                {
                    string[] a = request.logoBase64.Split(',');
                    request.logoBase64 = a[1];
                }

                String FileURL = "";
                string FileDestination = System.IO.Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory());


                FileDestination = FileDestination + _iconfiguration["LogoPath"];
                FileURL = _iconfiguration["LogoURL"];



                if (request.logoImageURL == "")
                {
                    if (!string.IsNullOrEmpty(request.logoBase64) && !string.IsNullOrEmpty(request.logoImageName))
                    {
                        Byte[] bytes = Convert.FromBase64String(request.logoBase64);
                        string fileType = Path.GetFileName(request.logoImageName.Substring(request.logoImageName.LastIndexOf('.') + 1));

                        string fileUniqueName = Utility.UploadFilebytes(bytes, request.logoImageName, FileDestination);
                        FileURL = FileURL + fileUniqueName;
                        if (fileUniqueName != null)
                        {
                            request.logoUniqueName = fileUniqueName;
                            request.logoImageURL = FileURL;

                        }
                    }
                    else
                    {
                        request.logoUniqueName = "";
                        request.logoImageURL = "";
                        //_uploadPanService.Update_Pan_Details(request);
                    }
                }
                else if (request.logoImageURL != "")
                {
                    if (!string.IsNullOrEmpty(request.logoBase64) && !string.IsNullOrEmpty(request.logoImageName))
                    {
                        if (!request.logoBase64.Contains("Content"))
                        {
                            string[] URL = request.logoImageURL.Split('/');
                            request.logoUniqueName = URL[3].ToString();
                            FileDestination = FileDestination + "\\" + request.logoUniqueName;
                            System.IO.File.Delete(FileDestination);

                            FileDestination = System.IO.Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory());


                            FileDestination = FileDestination + _iconfiguration["LogoPath"];
                            FileURL = _iconfiguration["LogoURL"];


                            Byte[] bytes = Convert.FromBase64String(request.logoBase64);
                            string fileType = Path.GetFileName(request.logoImageName.Substring(request.logoImageName.LastIndexOf('.') + 1));

                            string fileUniqueName = Utility.UploadFilebytes(bytes, request.logoImageName, FileDestination);
                            FileURL = FileURL + fileUniqueName;
                            if (fileUniqueName != null)
                            {
                                request.logoUniqueName = fileUniqueName;
                                request.logoImageURL = FileURL;

                            }
                        }
                        else
                        {
                            string[] ImageURL = request.logoImageURL.Split('/');
                            request.logoUniqueName = ImageURL[3].ToString();
                            // request.ImageURL = FileURL + request.logoUniqueName;
                            // _uploadPanService.Update_Pan_Details(request);
                        }

                    }
                    else
                    if (!string.IsNullOrEmpty(request.logoImageName))
                    {
                        string[] ImageURL = request.logoImageURL.Split('/');
                        request.logoUniqueName = ImageURL[3].ToString();
                        // request.ImageURL = FileURL + request.logoUniqueName;
                        //  _uploadPanService.Update_Pan_Details(request);
                    }
                    else
                    {
                        string[] ImageURL = request.logoImageURL.Split('/');
                        request.logoUniqueName = ImageURL[3].ToString();
                        FileDestination = FileDestination + "\\" + request.logoUniqueName;
                        System.IO.File.Delete(FileDestination);
                        request.logoUniqueName = "";
                        request.logoImageURL = "";
                        request.logoImageName = "";
                        // _uploadPanService.Update_Pan_Details(request);
                    }
                }
                else if (string.IsNullOrEmpty(request.logoImageURL) && string.IsNullOrEmpty(request.logoBase64) && string.IsNullOrEmpty(request.logoImageName))
                {
                    request.logoUniqueName = "";
                    request.logoImageURL = "";
                    request.logoImageName = "";
                    //   _uploadPanService.Update_Pan_Details(request);
                }

                _updateBusinessService.Insert_Business_Admin(request);

                _messages.Add(new Message_Info
                {
                    Message = "New Business Created",
                    Type = Message_Type.SUCCESS.ToString()
                });

                _statusCode = HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(Assembly.GetCallingAssembly().GetName().Name + "\n\t" + ex.ToString());

                _messages.Add(new Message_Info
                {
                    Message = "Exception Occured",
                    Type = Message_Type.ERROR.ToString()
                });

                _statusCode = HttpStatusCode.InternalServerError;
                throw;
            }
        }

        private bool Verify_User()
        {
            try
            {
                if (_updateBusinessService.Check_If_User_Exists(request.userId))
                {
                    return true;
                }
                _messages.Add(new Message_Info
                {
                    Message = "No User Found",
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
                    Message = "No User Found",
                    Type = Message_Type.ERROR.ToString()
                });

                _statusCode = HttpStatusCode.NotFound;

                return false;
            }
        }

        public void Dispose()
        {
            request = null;

            _updateBusinessService = null;

            _statusCode = HttpStatusCode.OK;

            _messages = null;
        }
    }
}
