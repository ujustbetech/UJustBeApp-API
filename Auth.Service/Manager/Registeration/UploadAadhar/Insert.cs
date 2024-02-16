using Auth.Service.Models.Registeration.UploadAadhar;
using Auth.Service.Respositories.Registeration;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using UJBHelper.Common;
using UJBHelper.DataModel;
using System.Linq;
using System.Threading.Tasks;

namespace Auth.Service.Manager.Registeration.UploadAadhar
{
    public class Insert : IDisposable
    {
        private Post_Request request;

        public HttpStatusCode _statusCode = HttpStatusCode.OK;

        private IUploadAadharService _uploadAadharService;

        public List<Message_Info> _messages = null;
        private IConfiguration _iconfiguration;


        public Insert(Post_Request post_Request, IUploadAadharService UploadAadharService, IConfiguration iconfiguration)
        {
            _messages = new List<Message_Info>();

            request = post_Request;

            _uploadAadharService = UploadAadharService;
            _iconfiguration = iconfiguration;

        }

        public void UploadFront()
        {
            try
            {
                string FileDestination = System.IO.Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory());
                FileDestination = FileDestination + _iconfiguration["AadharcardPath"];
                string FileURL = _iconfiguration["AadharcardURL"];

                if (request.FrontImageURL == "")
                {
                    if (!string.IsNullOrEmpty(request.aadharFrontBase64) && !string.IsNullOrEmpty(request.FrontFileName))
                    {
                        Byte[] bytes = Convert.FromBase64String(request.aadharFrontBase64);
                        string fileType = Path.GetFileName(request.FrontFileName.Substring(request.FrontFileName.LastIndexOf('.') + 1));

                        string fileUniqueName = Utility.UploadFilebytes(bytes, request.FrontFileName, FileDestination);
                        FileURL = FileURL + fileUniqueName;
                        if (fileUniqueName != null)
                        {
                            request.FrontUniqueName = fileUniqueName;
                            request.FrontImageURL = FileURL;
                            //_uploadAadharService.Update_Pan_Details(request);
                        }
                    }
                    else
                    {
                        request.FrontUniqueName = "";
                        request.FrontImageURL = "";
                    }
                }
                else if (request.FrontImageURL != "")
                {
                    if (!string.IsNullOrEmpty(request.aadharFrontBase64) && !string.IsNullOrEmpty(request.FrontFileName))
                    {
                        if (!request.aadharFrontBase64.Contains("Content"))
                        {
                            string[] URL = request.FrontImageURL.Split('/');
                            request.FrontUniqueName = URL[3].ToString();
                            FileDestination = FileDestination + "\\" + request.FrontUniqueName;
                            System.IO.File.Delete(FileDestination);

                            FileDestination = System.IO.Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory());
                            FileDestination = FileDestination + _iconfiguration["AadharcardPath"];
                            FileURL = _iconfiguration["AadharcardURL"];

                            Byte[] bytes = Convert.FromBase64String(request.aadharFrontBase64);
                            string fileType = Path.GetFileName(request.FrontFileName.Substring(request.FrontFileName.LastIndexOf('.') + 1));

                            string fileUniqueName = Utility.UploadFilebytes(bytes, request.FrontFileName, FileDestination);
                            FileURL = FileURL + fileUniqueName;
                            if (fileUniqueName != null)
                            {
                                request.FrontUniqueName = fileUniqueName;
                                request.FrontImageURL = FileURL;
                                // _uploadAadharService.Update_Pan_Details(request);
                            }
                        }
                        else
                        {
                            string[] ImageURL = request.FrontImageURL.Split('/');
                            request.FrontUniqueName = ImageURL[3].ToString();
                        }
                    }
                    else if (!string.IsNullOrEmpty(request.FrontFileName))
                    {
                        string[] ImageURL = request.FrontImageURL.Split('/');
                        request.FrontUniqueName = ImageURL[3].ToString();
                        //_uploadAadharService.Update_Pan_Details(request);
                    }
                    else
                    {
                        string[] ImageURL = request.FrontImageURL.Split('/');
                        request.FrontUniqueName = ImageURL[3].ToString();
                        FileDestination = FileDestination + "\\" + request.FrontUniqueName;
                        System.IO.File.Delete(FileDestination);
                        request.FrontUniqueName = "";
                        request.FrontImageURL = "";
                        request.FrontFileName = "";
                        //_uploadAadharService.Update_Pan_Details(request);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(Assembly.GetCallingAssembly().GetName().Name + "\n\t" + ex.ToString());
            }
        }

        public void UploadBack()
        {
            try
            {
                string FileDestination = System.IO.Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory());
                FileDestination = FileDestination + _iconfiguration["AadharcardPath"];
                string FileURL = _iconfiguration["AadharcardURL"];

                if (request.BackImageURL == "")
                {
                    if (!string.IsNullOrEmpty(request.aadharBackBase64) && !string.IsNullOrEmpty(request.BackFileName))
                    {
                        Byte[] bytes = Convert.FromBase64String(request.aadharBackBase64);
                        string fileType = Path.GetFileName(request.BackFileName.Substring(request.BackFileName.LastIndexOf('.') + 1));

                        string fileUniqueName = Utility.UploadFilebytes(bytes, request.BackFileName, FileDestination);
                        FileURL = FileURL + fileUniqueName;
                        if (fileUniqueName != null)
                        {
                            request.BackUniqueName = fileUniqueName;
                            request.BackImageURL = FileURL;
                            //_uploadAadharService.Update_Pan_Details(request);
                        }
                    }
                    else
                    {
                        request.BackUniqueName = "";
                        request.BackImageURL = "";
                    }
                }
                else if (request.BackImageURL != "")
                {
                    if (!string.IsNullOrEmpty(request.aadharBackBase64) && !string.IsNullOrEmpty(request.BackFileName))
                    {
                        if (!request.aadharBackBase64.Contains("Content"))
                        {
                            string[] URL = request.BackImageURL.Split('/');
                            request.BackUniqueName = URL[3].ToString();
                            FileDestination = FileDestination + "\\" + request.BackUniqueName;
                            System.IO.File.Delete(FileDestination);

                            FileDestination = System.IO.Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory());
                            FileDestination = FileDestination + _iconfiguration["AadharcardPath"];
                            FileURL = _iconfiguration["AadharcardURL"];

                            Byte[] bytes = Convert.FromBase64String(request.aadharBackBase64);
                            string fileType = Path.GetFileName(request.BackFileName.Substring(request.BackFileName.LastIndexOf('.') + 1));

                            string fileUniqueName = Utility.UploadFilebytes(bytes, request.BackFileName, FileDestination);
                            FileURL = FileURL + fileUniqueName;
                            if (fileUniqueName != null)
                            {
                                request.BackUniqueName = fileUniqueName;
                                request.BackImageURL = FileURL;
                                // _uploadAadharService.Update_Pan_Details(request);
                            }
                        }
                        else
                        {
                            string[] ImageURL = request.BackImageURL.Split('/');
                            request.BackUniqueName = ImageURL[3].ToString();
                        }
                    }
                    else if (!string.IsNullOrEmpty(request.BackFileName))
                    {
                        string[] ImageURL = request.BackImageURL.Split('/');
                        request.BackUniqueName = ImageURL[3].ToString();
                        //_uploadAadharService.Update_Pan_Details(request);
                    }
                    else
                    {
                        string[] ImageURL = request.BackImageURL.Split('/');
                        request.BackUniqueName = ImageURL[3].ToString();
                        FileDestination = FileDestination + "\\" + request.BackUniqueName;
                        System.IO.File.Delete(FileDestination);
                        request.BackUniqueName = "";
                        request.BackImageURL = "";
                        request.BackFileName = "";
                        //_uploadAadharService.Update_Pan_Details(request);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(Assembly.GetCallingAssembly().GetName().Name + "\n\t" + ex.ToString());
            }
        }

        public void Process()
        {
            try
            {
                if (Check_If_User_Exists())
                {
                    if (request.aadharFrontBase64.Contains(";base64,"))
                    {
                        string[] a = request.aadharFrontBase64.Split(',');
                        request.aadharFrontBase64 = a[1];
                    }

                    if (request.aadharBackBase64.Contains(";base64,"))
                    {
                        string[] a = request.aadharBackBase64.Split(',');
                        request.aadharBackBase64 = a[1];
                    }

                    if (string.IsNullOrEmpty(request.aadharFrontBase64)
                       && string.IsNullOrEmpty(request.aadharBackBase64)
                       && string.IsNullOrEmpty(request.FrontFileName) && string.IsNullOrEmpty(request.BackFileName)
                       && string.IsNullOrEmpty(request.BackImageURL) && string.IsNullOrEmpty(request.FrontImageURL))
                    {
                        request.FrontFileName = "";
                        request.FrontImageURL = "";
                        request.FrontUniqueName = "";
                        request.BackFileName = "";
                        request.BackImageURL = "";
                        request.BackUniqueName = "";
                        // _uploadPanService.Update_Pan_Details(request);
                    }
                    else
                    {
                        UploadFront();
                        UploadBack();
                    }
                    _uploadAadharService.Update_Aadhar_Details(request);

                    if (Check_If_All_Docs_Uploaded())
                    {
                        //var nq = new Notification_Queue(_iconfiguration["ConnectionString"], _iconfiguration["Database"]);
                        //Notification notify_template = new Notification();
                        //notify_template = nq.Get_Notification_Template("KYC Approval Under Process");

                        //bool isaalowed = notify_template.Data.Where(x => x.Receiver == "User").Select(x => x.SMS.isSMSAllowed).FirstOrDefault();
                        //if (isaalowed)
                        //{
                        //    nq.Add_To_Queue(request.userId, "", "", "", "new", "KYC Approval Under Process", "", "SMS", "User", "");
                        //}
                        //    nq.Add_To_Queue(request.userId, "", "", "", "new", "KYC Approval Under Process", "", "Email", "User", "");
                        //   Add_To_Notification_Queue();
                        SendNotification(request.userId);
                    }

                    _messages.Add(new Message_Info
                    {
                        Message = "Partner's AADHAR details updated successfully",
                        Type = Message_Type.SUCCESS.ToString()
                    });

                    _statusCode = HttpStatusCode.OK;
                }
            }
            catch (Exception ex)
            {
                _messages.Add(new Message_Info
                {
                    Message = "Could not update Partner's AADHAR details",
                    Type = Message_Type.ERROR.ToString()
                });

                _statusCode = HttpStatusCode.InternalServerError;

                Logger.Log.Error(Assembly.GetCallingAssembly().GetName().Name + "\n\t" + ex.ToString());

                throw;
            }
        }

        private void Add_To_Notification_Queue()
        {
            MessageBody MB = new MessageBody();
            var nq = new Notification_Sender();
            //   nq.SendNotification("KYC Approval Under Process", MB, request.userId, "", "");
        }

        public void SendNotification(string UserId)
        {
            MessageBody MB = new MessageBody();

            var nq = new Notification_Sender();
            Task.Run(() => nq.SendNotificationAsync("KYC Approval Under Process", MB, UserId, "", ""));
            // nq.SendNotification("KYC Approval Under Process", MB, UserId, "", "");
            //return Task.CompletedTask;

        }
        private bool Check_If_All_Docs_Uploaded()
        {
            try
            {
                if (_uploadAadharService.Check_If_All_Docs_Uploaded(request.userId))
                {
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                Logger.Log.Error(Assembly.GetCallingAssembly().GetName().Name + "\n\t" + ex.ToString());
                return false;
            }
        }

        private bool Check_If_User_Exists()
        {
            try
            {
                if (_uploadAadharService.Check_If_User_Exists(request.userId))
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
            _messages = null;

            _uploadAadharService = null;

            _statusCode = HttpStatusCode.OK;

            request = null;
        }
    }
}
