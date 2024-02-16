using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Promotion.Service.Models.Promotions;
using Promotion.Service.Repositories.GetPromotionService;
using Promotion.Service.Repositories.PromotionService;
using UJBHelper.Common;
using UJBHelper.DataModel;

namespace Promotion.Service.Manager.PromotionService
{
    public class Insert : IDisposable
    {
        private Post_Request request;
        private IConfiguration _iconfiguration;
        private IAddPromotionService _addPromotionService;
        private IGetPromotionService _getPromotionService;
        public HttpStatusCode _statusCode = HttpStatusCode.OK;
        public List<Message_Info> _messages = null;

        public Insert(Post_Request request, IAddPromotionService addPromotionService, IConfiguration iconfiguration)
        {
            _iconfiguration = iconfiguration;
            this.request = request;
            _addPromotionService = addPromotionService;
            _messages = new List<Message_Info>();
        }

        public void Process()
        {
            try
            {

                if (string.IsNullOrWhiteSpace(request.PromotionId))
                {
                    Insert_New_Promotion();
                }
                else
                {
                    Update_Promotions_Details();

                }

            }
            catch (Exception ex)
            {
                Logger.Log.Error(Assembly.GetCallingAssembly().GetName().Name + "\n\t" + ex.ToString());
            }
        }

        private void Update_Promotions_Details()
        {
            try
            {
                
                string FileDestination = System.IO.Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory());
                FileDestination = FileDestination + _iconfiguration["PromotionPath"];
                foreach (var item in request.Media)
                {
                    if (item.FileUniqueName == "")
                    {
                        if (!string.IsNullOrEmpty(item.Base64string) && !string.IsNullOrEmpty(item.FileName))
                        {
                            Byte[] bytes = Convert.FromBase64String(item.Base64string);
                            string fileType = Path.GetFileName(item.FileName.Substring(item.FileName.LastIndexOf('.') + 1));
                            string FileDestination1 = System.IO.Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory());
                            FileDestination1 = FileDestination1 + _iconfiguration["PromotionPath"];
                            string fileUniqueName = Utility.UploadFilebytes(bytes, item.FileName, FileDestination1);
                            String FileURL = _iconfiguration["PromotionURL"] + fileUniqueName;
                            if (fileUniqueName != null)
                            {
                                PromotionMedia media = new PromotionMedia();
                                media.FileName = item.FileName;
                                media.UniqueName = fileUniqueName;
                                media.ImageURL = FileURL;
                                request.Media1.Add(media);
                            }
                        }
                    }
                    else if (item.Base64string != "")
                    {
                        FileDestination = FileDestination + "\\" + item.FileUniqueName;
                        System.IO.File.Delete(FileDestination);
                       // request.Media.Remove(item);
                        if (!string.IsNullOrEmpty(item.Base64string) && !string.IsNullOrEmpty(item.FileName))
                        {
                            Byte[] bytes = Convert.FromBase64String(item.Base64string);
                            string fileType = Path.GetFileName(item.FileName.Substring(item.FileName.LastIndexOf('.') + 1));
                            string FileDestination1 = System.IO.Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory());
                            FileDestination1 = FileDestination1 + _iconfiguration["PromotionPath"];
                            string fileUniqueName = Utility.UploadFilebytes(bytes, item.FileName, FileDestination1);
                            String FileURL = _iconfiguration["PromotionURL"] + fileUniqueName;
                            if (fileUniqueName != null)
                            {
                                PromotionMedia media = new PromotionMedia();
                                media.FileName = item.FileName;
                                media.UniqueName = fileUniqueName;
                                media.ImageURL = FileURL;
                                request.Media1.Add(media);
                            }
                        }
                    }
                    else
                    {
                        PromotionMedia media = new PromotionMedia();
                        media.FileName = item.FileName;
                        media.UniqueName = item.FileUniqueName;
                        media.ImageURL = item.FileURL;
                        request.Media1.Add(media);
                    }
                }
               
                _addPromotionService.Update_Promotions_Details(request);

                _messages.Add(new Message_Info
                {
                    Message = "Promotion Details Updated",
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

        private void Insert_New_Promotion()
        {
            try
            {
                foreach (var item in request.Media)
                {
                    if (!string.IsNullOrEmpty(item.Base64string) && !string.IsNullOrEmpty(item.FileName))
                    {
                        Byte[] bytes = Convert.FromBase64String(item.Base64string);
                        string fileType = Path.GetFileName(item.FileName.Substring(item.FileName.LastIndexOf('.') + 1));
                        string FileDestination = System.IO.Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory());
                        FileDestination = FileDestination + _iconfiguration["PromotionPath"];
                        string fileUniqueName = Utility.UploadFilebytes(bytes, item.FileName, FileDestination);
                        String FileURL = _iconfiguration["PromotionURL"] + fileUniqueName;
                        if (fileUniqueName != null)
                        {
                            PromotionMedia media = new PromotionMedia();
                            media.FileName = item.FileName;
                            media.UniqueName = fileUniqueName;
                            media.ImageURL = FileURL;
                            request.Media1.Add(media);
                        }
                    }
                }
                _addPromotionService.Insert_New_Promotion(request);
                _messages.Add(new Message_Info
                {
                    Message = "New Promotion Created",
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

        public void Dispose()
        {
            request = null;

            _addPromotionService = null;

            _statusCode = HttpStatusCode.OK;

            _messages = null;
        }
    }
}
