using Business.Service.Models.Company.UpdateBusiness.UpdateBanner;
using Business.Service.Repositories.Company;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using UJBHelper.Common;

namespace Business.Service.Manager.Company.UpdateBusiness.UpdateBanner
{
    public class Update : IDisposable
    {
        private Put_Request request;
        private IConfiguration _iconfiguration;
        private IUpdateBusinessService _updateBusinessService;
        public HttpStatusCode _statusCode = HttpStatusCode.OK;
        public List<Message_Info> _messages = null;

        public Update(Put_Request request, IUpdateBusinessService updateBusinessService, IConfiguration iconfiguration)
        {
            this.request = request;
            _iconfiguration = iconfiguration;
            _updateBusinessService = updateBusinessService;
            _messages = new List<Message_Info>();
        }

        public void Process()
        {
            if (Verify_Business())
            {
                UpdateBanner();
            }
        }

        private bool Verify_Business()
        {
            try
            {
                if (_updateBusinessService.Check_If_Business_Exists(request.BusinessId, request.UserId))
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

        public void Dispose()
        {
            request = null;

            _updateBusinessService = null;

            _statusCode = HttpStatusCode.OK;

            _messages = null;
        }

        private void UpdateBanner()
        {
            try
            {
                if (request.Base64string.Contains(";base64,"))
                {
                    string[] a = request.Base64string.Split(',');
                    request.Base64string = a[1];
                }

                String FileURL = "";
                string FileDestination = System.IO.Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory());


                FileDestination = FileDestination + _iconfiguration["BannerPath"];
                FileURL = _iconfiguration["BannerURL"];

                if (request.URL == "")
                {
                    if (!string.IsNullOrEmpty(request.Base64string) && !string.IsNullOrEmpty(request.FileName))
                    {
                        Byte[] bytes = Convert.FromBase64String(request.Base64string);
                        string fileType = Path.GetFileName(request.FileName.Substring(request.FileName.LastIndexOf('.') + 1));

                        string fileUniqueName = Utility.UploadFilebytes(bytes, request.FileName, FileDestination);
                        FileURL = FileURL + fileUniqueName;
                        if (fileUniqueName != null)
                        {
                            request.UniqueFileName = fileUniqueName;
                            request.URL = FileURL;

                        }
                    }
                    else
                    {
                        request.UniqueFileName = "";
                        request.URL = "";
                        
                    }
                }
                else if (request.URL != "")
                {
                    if (!string.IsNullOrEmpty(request.Base64string) && !string.IsNullOrEmpty(request.FileName))
                    {
                        if (!request.Base64string.Contains("Content"))
                        {
                            if (request.URL != null && request.URL != "")
                            {
                                string[] URL = request.URL.Split('/');
                                request.UniqueFileName = URL[3].ToString();
                            }
                            if (request.UniqueFileName != null && request.UniqueFileName != "")
                            {
                                FileDestination = FileDestination + "\\" + request.UniqueFileName;
                                System.IO.File.Delete(FileDestination);
                            }
                            FileDestination = System.IO.Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory());


                            FileDestination = FileDestination + _iconfiguration["BannerPath"];
                            FileURL = _iconfiguration["BannerURL"];


                            Byte[] bytes = Convert.FromBase64String(request.Base64string);
                            string fileType = Path.GetFileName(request.FileName.Substring(request.FileName.LastIndexOf('.') + 1));

                            string fileUniqueName = Utility.UploadFilebytes(bytes, request.FileName, FileDestination);
                            FileURL = FileURL + fileUniqueName;
                            if (fileUniqueName != null)
                            {
                                request.UniqueFileName = fileUniqueName;
                                request.URL = FileURL;

                            }
                        }
                        else
                        {
                            string[] ImageURL = request.URL.Split('/');
                            request.UniqueFileName = ImageURL[3].ToString();
                           
                        }
                    }
                    else
                    if (!string.IsNullOrEmpty(request.FileName))
                    {
                        string[] ImageURL = request.URL.Split('/');
                        request.UniqueFileName = ImageURL[3].ToString();
                        
                    }
                    else
                    {
                        string[] ImageURL = request.URL.Split('/');
                        request.UniqueFileName = ImageURL[3].ToString();
                        FileDestination = FileDestination + "\\" + request.UniqueFileName;
                        System.IO.File.Delete(FileDestination);
                        request.UniqueFileName = "";
                        request.URL = "";
                        request.FileName = "";
                       
                    }
                }
                else if (string.IsNullOrEmpty(request.URL) && string.IsNullOrEmpty(request.Base64string) && string.IsNullOrEmpty(request.FileName))
                {
                    request.UniqueFileName = "";
                    request.URL = "";
                    request.FileName = "";
                    
                }


                //if (!string.IsNullOrEmpty(request.Base64string) && !string.IsNullOrEmpty(request.FileName))
                //{
                //    Byte[] bytes = Convert.FromBase64String(request.Base64string);
                //    string fileType = Path.GetFileName(request.FileName.Substring(request.FileName.LastIndexOf('.') + 1));
                //    string FileDestination = System.IO.Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory());
                //    // FileDestination = FileDestination + "\\UJBApiGateway\\" + "\\Content\\" + "\\Business\\" + "\\Banner\\";
                //    FileDestination = FileDestination + _iconfiguration["BannerPath"];
                //    string fileUniqueName = Utility.UploadFilebytes(bytes, request.FileName, FileDestination);
                //    //String FileURL = "/Business/Banner/" + fileUniqueName;
                //    String FileURL = _iconfiguration["BannerURL"] + fileUniqueName;
                //    if (fileUniqueName != null)
                //    {
                //        request.UniqueFileName = fileUniqueName;
                //        request.URL = FileURL;
                //        _updateBusinessService.UpdateBanner(request);
                //    }
                //}

                _updateBusinessService.UpdateBanner(request);

                _messages.Add(new Message_Info { Message = "Business Banner updated successfully", Type = Message_Type.SUCCESS.ToString() });

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
    }
}
