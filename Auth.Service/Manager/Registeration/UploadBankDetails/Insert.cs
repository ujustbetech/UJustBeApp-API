using Auth.Service.Models.Registeration.UploadBankDetails;
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

namespace Auth.Service.Manager.Registeration.UploadBankDetails
{
    public class Insert : IDisposable
    {
        private Post_Request request;

        public HttpStatusCode _statusCode = HttpStatusCode.OK;

        private IUploadBankDetailsService _uploadBankDetailsService;

        public List<Message_Info> _messages = null;
        private IConfiguration _iconfiguration;

        public Insert(Post_Request post_Request, IUploadBankDetailsService UploadBankDetailsService, IConfiguration iconfiguration)
        {
            _messages = new List<Message_Info>();

            request = post_Request;

            _uploadBankDetailsService = UploadBankDetailsService;
            _iconfiguration = iconfiguration;

        }

        public void Process()
        {
            try
            {
                if (Check_If_User_Exists())
                {
                    if (request.BankDetails.cancelChequebase64Img.Contains(";base64,"))
                    {
                        string[] a = request.BankDetails.cancelChequebase64Img.Split(',');
                        request.BankDetails.cancelChequebase64Img = a[1];
                    }
                    Update_Bank_Details();

                    if (Check_If_All_Docs_Uploaded())
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
                        //Add_To_Notification_Queue();
                        SendNotification(request.userId);
                    }
                }

            }
            catch (Exception ex)
            {
                Logger.Log.Error(Assembly.GetCallingAssembly().GetName().Name + "\n\t" + ex.ToString());
            }
        }
        public void SendNotification(string UserId)
        {
            MessageBody MB = new MessageBody();

            var nq = new Notification_Sender();
            Task.Run(() => nq.SendNotificationAsync("KYC Approval Under Process", MB, UserId, "", ""));
            // nq.SendNotification("KYC Approval Under Process", MB, UserId, "", "");
            //return Task.CompletedTask;

        }

        //private void Add_To_Notification_Queue()
        //{
        //    MessageBody MB = new MessageBody();
        //    var nq = new Notification_Sender();
        //    nq.SendNotification("KYC Approval Under Process", MB, request.userId, "", "");
        //}
        private bool Check_If_All_Docs_Uploaded()
        {
            try
            {
                if (_uploadBankDetailsService.Check_If_All_Docs_Uploaded(request.userId))
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
                if (_uploadBankDetailsService.Check_If_User_Exists(request.userId))
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

        public void UploadCheque()
        {
            try
            {
                string FileDestination = System.IO.Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory());
                FileDestination = FileDestination + _iconfiguration["BankDetailsPath"];
                string FileURL = _iconfiguration["BankDetailsURL"];

                if (request.BankDetails.ImageURL == "")
                {
                    if (!string.IsNullOrEmpty(request.BankDetails.cancelChequebase64Img) && !string.IsNullOrEmpty(request.BankDetails.FileName))
                    {
                        Byte[] bytes = Convert.FromBase64String(request.BankDetails.cancelChequebase64Img);
                        string fileType = Path.GetFileName(request.BankDetails.FileName.Substring(request.BankDetails.FileName.LastIndexOf('.') + 1));

                        string fileUniqueName = Utility.UploadFilebytes(bytes, request.BankDetails.FileName, FileDestination);
                        FileURL = FileURL + fileUniqueName;
                        if (fileUniqueName != null)
                        {
                            request.BankDetails.UniqueName = fileUniqueName;
                            request.BankDetails.ImageURL = FileURL;

                        }
                    }
                    else
                    {
                        request.BankDetails.UniqueName = "";
                        request.BankDetails.ImageURL = "";

                    }
                }
                else if (request.BankDetails.ImageURL != "")
                {
                    if (!string.IsNullOrEmpty(request.BankDetails.cancelChequebase64Img) && !string.IsNullOrEmpty(request.BankDetails.FileName))
                    {
                        if (!request.BankDetails.cancelChequebase64Img.Contains("Content"))
                        {
                            string[] URL = request.BankDetails.ImageURL.Split('/');
                            request.BankDetails.UniqueName = URL[3].ToString();
                            FileDestination = FileDestination + "\\" + request.BankDetails.UniqueName;
                            System.IO.File.Delete(FileDestination);

                            FileDestination = System.IO.Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory());
                            FileDestination = FileDestination + _iconfiguration["BankDetailsPath"];
                            FileURL = _iconfiguration["BankDetailsURL"];

                            Byte[] bytes = Convert.FromBase64String(request.BankDetails.cancelChequebase64Img);
                            string fileType = Path.GetFileName(request.BankDetails.FileName.Substring(request.BankDetails.FileName.LastIndexOf('.') + 1));

                            string fileUniqueName = Utility.UploadFilebytes(bytes, request.BankDetails.FileName, FileDestination);
                            FileURL = FileURL + fileUniqueName;
                            if (fileUniqueName != null)
                            {
                                request.BankDetails.UniqueName = fileUniqueName;
                                request.BankDetails.ImageURL = FileURL;
                                // _uploadAadharService.Update_Pan_Details(request);
                            }

                        }
                        else
                        {
                            string[] ImageURL = request.BankDetails.ImageURL.Split('/');
                            request.BankDetails.UniqueName = ImageURL[3].ToString();
                        }
                    }
                    else if (!string.IsNullOrEmpty(request.BankDetails.FileName))
                    {
                        string[] ImageURL = request.BankDetails.ImageURL.Split('/');
                        request.BankDetails.UniqueName = ImageURL[3].ToString();
                        //_uploadAadharService.Update_Pan_Details(request);
                    }
                    else
                    {
                        string[] ImageURL = request.BankDetails.ImageURL.Split('/');
                        request.BankDetails.UniqueName = ImageURL[3].ToString();
                        FileDestination = FileDestination + "\\" + request.BankDetails.UniqueName;
                        System.IO.File.Delete(FileDestination);
                        request.BankDetails.UniqueName = "";
                        request.BankDetails.ImageURL = "";
                        request.BankDetails.FileName = "";
                        //_uploadAadharService.Update_Pan_Details(request);
                    }
                }
                else if (string.IsNullOrEmpty(request.BankDetails.cancelChequebase64Img) && string.IsNullOrEmpty(request.BankDetails.FileName)
                    && string.IsNullOrEmpty(request.BankDetails.ImageURL))
                {
                    request.BankDetails.UniqueName = "";
                    request.BankDetails.ImageURL = "";
                    request.BankDetails.FileName = "";
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(Assembly.GetCallingAssembly().GetName().Name + "\n\t" + ex.ToString());
            }
        }

        private void Update_Bank_Details()
        {
            try
            {
                UploadCheque();
                _uploadBankDetailsService.Update_Bank_Details(request);

                _messages.Add(new Message_Info
                {
                    Message = "Partner's Bank details updated successfully",
                    Type = Message_Type.SUCCESS.ToString()
                });

                _statusCode = HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                _messages.Add(new Message_Info
                {
                    Message = "Could not update Partner's Bank details",
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

            _uploadBankDetailsService = null;

            _statusCode = HttpStatusCode.OK;

            request = null;
        }
    }
}
