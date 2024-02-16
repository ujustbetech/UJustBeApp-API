using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using Business.Service.Models.Company.UpdateCompanyLogo;
using Business.Service.Repositories.Company;
using Microsoft.Extensions.Configuration;
using UJBHelper.Common;

namespace Business.Service.Manager.Company.UpdateCompanyLogo
{
    public class Insert : IDisposable
    {
        public HttpStatusCode _statusCode = HttpStatusCode.OK;
        public List<Message_Info> _messages = null;
        private Post_Request request;
        private IUpdateCompanyService _updateCompanyService;
        private IConfiguration _iconfiguration;

        public Insert(Post_Request request, IUpdateCompanyService updateCompanyService, IConfiguration iconfiguration)
        {
            this.request = request;
            _updateCompanyService = updateCompanyService;
            _messages = new List<Message_Info>();
            _iconfiguration = iconfiguration;
        }

        public void Process()
        {
            if (Verify_Business())
            {
                Update_Company_Logo();
            }
        }

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

        private void Update_Company_Logo()
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
                    if (!string.IsNullOrEmpty(request.logoBase64) && !string.IsNullOrEmpty(request.logoImgName))
                    {
                        Byte[] bytes = Convert.FromBase64String(request.logoBase64);
                        string fileType = Path.GetFileName(request.logoImgName.Substring(request.logoImgName.LastIndexOf('.') + 1));

                        string fileUniqueName = Utility.UploadFilebytes(bytes, request.logoImgName, FileDestination);
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
                    if (!string.IsNullOrEmpty(request.logoBase64) && !string.IsNullOrEmpty(request.logoImgName))
                    {
                        if (!request.logoBase64.Contains("Content"))
                        {
                            if (request.logoImageURL != null && request.logoImageURL != "")
                            {
                                string[] URL = request.logoImageURL.Split('/');
                                request.logoUniqueName = URL[3].ToString();
                            }
                            if (request.logoUniqueName != null && request.logoUniqueName != "")
                            {
                                FileDestination = FileDestination + "\\" + request.logoUniqueName;
                                System.IO.File.Delete(FileDestination);
                            }
                            FileDestination = System.IO.Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory());

                          
                                FileDestination = FileDestination + _iconfiguration["LogoPath"];
                                FileURL = _iconfiguration["LogoURL"];
                           

                            Byte[] bytes = Convert.FromBase64String(request.logoBase64);
                            string fileType = Path.GetFileName(request.logoImgName.Substring(request.logoImgName.LastIndexOf('.') + 1));

                            string fileUniqueName = Utility.UploadFilebytes(bytes, request.logoImgName, FileDestination);
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
                    if (!string.IsNullOrEmpty(request.logoImgName))
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
                        request.logoImgName = "";
                       // _uploadPanService.Update_Pan_Details(request);
                    }
                }
                else if (string.IsNullOrEmpty(request.logoImageURL) && string.IsNullOrEmpty(request.logoBase64) && string.IsNullOrEmpty(request.logoImgName))
                {
                    request.logoUniqueName = "";
                    request.logoImageURL = "";
                    request.logoImgName = "";
                 //   _uploadPanService.Update_Pan_Details(request);
                }

                _updateCompanyService.UpdateCompanyLogo(request);

                _messages.Add(new Message_Info { Message = "Business Logo updated successfully", Type = Message_Type.SUCCESS.ToString() });

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
            request = null;

            _updateCompanyService = null;

            _statusCode = HttpStatusCode.OK;

            _messages = null;
        }
    }
}
