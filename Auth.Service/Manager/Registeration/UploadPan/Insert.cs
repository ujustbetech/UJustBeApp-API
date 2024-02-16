using Auth.Service.Models.Registeration.UploadPan;
using Auth.Service.Respositories.Registeration;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using UJBHelper.Common;
using UJBHelper.DataModel;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Auth.Service.Manager.Registeration.UploadPan
{
    public class Insert : IDisposable
    {
        private Post_Request request;

        public HttpStatusCode _statusCode = HttpStatusCode.OK;

        private IUploadPanService _uploadPanService;

        public List<Message_Info> _messages = null;
        private IConfiguration _iconfiguration;
        public Insert(Post_Request post_Request, IUploadPanService UploadPanService, IConfiguration iconfiguration)
        {
            _messages = new List<Message_Info>();
            _iconfiguration = iconfiguration;
            request = post_Request;

            _uploadPanService = UploadPanService;
        }

        public void Process()
        {
            try
            {
                if (Check_If_User_Exists())
                {
                    if (request.panImgBase64.Contains(";base64,"))
                    {
                        string[] a = request.panImgBase64.Split(',');
                        request.panImgBase64 = a[1];
                    }

                    String FileURL = "";
                    string FileDestination = System.IO.Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory());

                    if (request.panType == "Individual")
                    {
                        FileDestination = FileDestination + _iconfiguration["PancardPath"];
                        FileURL = _iconfiguration["PancardURL"];
                    }
                    else
                    {
                        FileDestination = FileDestination + _iconfiguration["BussinessPanPath"];
                        FileURL = _iconfiguration["BussinessPanURL"];
                    }

                    if (request.ImageURL == "")
                    {
                        if (!string.IsNullOrEmpty(request.panImgBase64) && !string.IsNullOrEmpty(request.FileName))
                        {
                            Byte[] bytes = Convert.FromBase64String(request.panImgBase64);
                            string fileType = Path.GetFileName(request.FileName.Substring(request.FileName.LastIndexOf('.') + 1));

                            string fileUniqueName = Utility.UploadFilebytes(bytes, request.FileName, FileDestination);
                            FileURL = FileURL + fileUniqueName;
                            if (fileUniqueName != null)
                            {
                                request.UniqueName = fileUniqueName;
                                request.ImageURL = FileURL;
                                _uploadPanService.Update_Pan_Details(request);
                            }
                        }
                        else
                        {
                            request.UniqueName = "";
                            request.ImageURL = "";
                            _uploadPanService.Update_Pan_Details(request);
                        }
                    }
                    else if (request.ImageURL != "")
                    {
                        if (!string.IsNullOrEmpty(request.panImgBase64) && !string.IsNullOrEmpty(request.FileName))
                        {
                            if (!request.panImgBase64.Contains("Content"))
                            {
                                string[] URL = request.ImageURL.Split('/');
                                request.UniqueName = URL[3].ToString();
                                FileDestination = FileDestination + "\\" + request.UniqueName;
                                System.IO.File.Delete(FileDestination);

                                FileDestination = System.IO.Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory());

                                if (request.panType == "Individual")
                                {
                                    FileDestination = FileDestination + _iconfiguration["PancardPath"];
                                    FileURL = _iconfiguration["PancardURL"];
                                }
                                else
                                {
                                    FileDestination = FileDestination + _iconfiguration["BussinessPanPath"];
                                    FileURL = _iconfiguration["BussinessPanURL"];
                                }

                                Byte[] bytes = Convert.FromBase64String(request.panImgBase64);
                                string fileType = Path.GetFileName(request.FileName.Substring(request.FileName.LastIndexOf('.') + 1));

                                string fileUniqueName = Utility.UploadFilebytes(bytes, request.FileName, FileDestination);
                                FileURL = FileURL + fileUniqueName;
                                if (fileUniqueName != null)
                                {
                                    request.UniqueName = fileUniqueName;
                                    request.ImageURL = FileURL;
                                    _uploadPanService.Update_Pan_Details(request);
                                }
                            }
                            else
                            {
                                string[] ImageURL = request.ImageURL.Split('/');
                                request.UniqueName = ImageURL[3].ToString();
                                // request.ImageURL = FileURL + request.UniqueName;
                                _uploadPanService.Update_Pan_Details(request);
                            }
                                
                        }
                        else
                        if (!string.IsNullOrEmpty(request.FileName))
                        {
                            string[] ImageURL = request.ImageURL.Split('/');
                            request.UniqueName = ImageURL[3].ToString();
                            // request.ImageURL = FileURL + request.UniqueName;
                            _uploadPanService.Update_Pan_Details(request);
                        }
                        else
                        {
                            string[] ImageURL = request.ImageURL.Split('/');
                            request.UniqueName = ImageURL[3].ToString();
                            FileDestination = FileDestination + "\\" + request.UniqueName;
                            System.IO.File.Delete(FileDestination);
                            request.UniqueName = "";
                            request.ImageURL = "";
                            request.FileName = "";
                            _uploadPanService.Update_Pan_Details(request);
                        }
                    }
                    else if (string.IsNullOrEmpty(request.ImageURL) && string.IsNullOrEmpty(request.panImgBase64) && string.IsNullOrEmpty(request.FileName ))
                    {
                        request.UniqueName = "";
                        request.ImageURL = "";
                        request.FileName = "";
                        _uploadPanService.Update_Pan_Details(request);
                    }
                    _messages.Add(new Message_Info
                    {
                        Message = "Partner's PAN details updated successfully",
                        Type = Message_Type.SUCCESS.ToString()
                    });

                    if (request.panType == "Individual" && Check_If_All_Docs_Uploaded())
                    {
                        //var nq = new Notification_Queue(_iconfiguration["ConnectionString"], _iconfiguration["Database"]);
                        //nq.Add_To_Queue(request.userId, "", "", "", "new", "KYC Approval Under Process", "", "Email", "User", "");
                        //Notification notify_template = new Notification();
                        //notify_template = nq.Get_Notification_Template("KYC Approval Under Process");

                        //bool isaalowed = notify_template.Data.Where(x => x.Receiver == "User").Select(x => x.SMS.isSMSAllowed).FirstOrDefault();
                        //if (isaalowed)
                        //{
                        //    nq.Add_To_Queue(request.userId, "", "", "", "new", "KYC Approval Under Process", "", "SMS", "User", "");
                        //}
                        //  Add_To_Notification_Queue();
                        // Task.Run(() => SendNotification(request.userId));
                        SendNotification(request.userId);
                    }

                    _statusCode = HttpStatusCode.OK;

                }
            }
            catch (Exception ex)
            {
                _messages.Add(new Message_Info
                {
                    Message = "Could not update Partner's PAN details",
                    Type = Message_Type.ERROR.ToString()
                });

                _statusCode = HttpStatusCode.InternalServerError;

                Logger.Log.Error(Assembly.GetCallingAssembly().GetName().Name + "\n\t" + ex.ToString());

                throw;
            }
        }
        //if (!string.IsNullOrEmpty(request.panImgBase64))
        //        {
        //            string fileUniqueName = "";
        //            String FileURL = "";
        //            string FileName = "";
        //            if (request.panImgBase64.StartsWith("http"))
        //            {
        //                var arrfile = request.ImageURL.Split('.');
        //                var arrfile1 = request.ImageURL.Split('/');
        //                //if (request.panType == "Individual")
        //                //{
        //                    FileName = "panimg."+ arrfile[1];
        //                    fileUniqueName = arrfile1[arrfile1.Length-1];
        //                    FileURL = request.ImageURL;
        //                //}
        //            }
        //            else
        //            {
        //                var arrbaseimgs = request.panImgBase64.Split(',');
        //                Byte[] bytes = Convert.FromBase64String(arrbaseimgs[1]);
        //                var filetype = arrbaseimgs[0].Split('/')[1];
        //                 FileName = "panimg." + filetype.ToString().Replace(";base64", "");
        //                string fileType = request.panImgType;
        //                string FileDestination = System.IO.Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory());
        //                if (request.panType == "Individual")
        //                {
        //                    FileDestination = FileDestination + _iconfiguration["PancardPath"];
        //                    fileUniqueName = Utility.UploadFilebytes(bytes, FileName, FileDestination);
        //                    FileURL = "Content" + _iconfiguration["PancardURL"] + fileUniqueName;
        //                }
        //                else if (request.panType == "Business")
        //                {
        //                    FileDestination = FileDestination + _iconfiguration["BussinessPanPath"];
        //                    fileUniqueName = Utility.UploadFilebytes(bytes, FileName, FileDestination);
        //                    FileURL = "Content" + _iconfiguration["BussinessPanURL"] + fileUniqueName;
        //                }

        //            }
        //            if (fileUniqueName != null)
        //            {

        //                request.FileName = FileName;
        //                request.UniqueName = fileUniqueName;
        //                request.ImageURL = FileURL;
        //            }
        //            _uploadPanService.Update_Pan_Details(request);


        //private void Add_To_Notification_Queue()
        //{
        //    MessageBody MB = new MessageBody();
        //    var nq = new Notification_Sender();
        //    nq.SendNotification("KYC Approval Under Process", MB, request.userId, "", "");
        //}

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
                if (_uploadPanService.Check_If_All_Docs_Uploaded(request.userId))
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
                if (_uploadPanService.Check_If_User_Exists(request.userId))
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

            _uploadPanService = null;

            _statusCode = HttpStatusCode.OK;

            request = null;
        }
    }
}
