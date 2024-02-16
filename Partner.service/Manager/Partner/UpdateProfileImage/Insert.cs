using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Reflection;
using Microsoft.Extensions.Configuration;
using Partner.Service.Models.Partners.UpdateProfileImage;
using Partner.Service.Repositories.Partner;
using UJBHelper.Common;

namespace Partner.Service.Manager.Partner.UpdateProfileImage
{
    public class Insert : IDisposable
    {
        public HttpStatusCode _statusCode = HttpStatusCode.OK;
        public List<Message_Info> _messages = null;
        private Post_Request request;
        private IUpdatePartnerProfile _updateProfileImageService;
        private IConfiguration _iconfiguration;

        public Insert(Post_Request request, IUpdatePartnerProfile updateProfileImageService,IConfiguration iconfiguration)
        {
            this.request = request;
            _iconfiguration = iconfiguration;
            _updateProfileImageService = updateProfileImageService;
            _messages = new List<Message_Info>();
        }

        public void Process()
        {
            try
            {
                if (Verify_User())
                {
                    if (Verify_UserIsActive())
                    {
                        Update_Profile_Image_Details();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(Assembly.GetCallingAssembly().GetName().Name + "\n\t" + ex.ToString());
            }
        }

        private bool Verify_User()
        {
            try
            {
                if (_updateProfileImageService.Check_If_User_Exist(request.userId))
                {
                    return true;
                }
                _messages.Add(new Message_Info
                {
                    Message = "No Users Found",
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
                    Message = "No Users Found",
                    Type = Message_Type.ERROR.ToString()
                });

                _statusCode = HttpStatusCode.NotFound;

                return false;
            }
        }

        private bool Verify_UserIsActive()
        {
            try
            {
                if (_updateProfileImageService.Check_If_User_IsActive(request.userId))
                {
                    return true;
                }
                _messages.Add(new Message_Info
                {
                    Message = "User Is InActive",
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
                    Message = "User Is InActive",
                    Type = Message_Type.ERROR.ToString()
                });

                _statusCode = HttpStatusCode.NotFound;

                return false;
            }
        }
        
        private void UploadProfileImage()
        {
            if (request.ImageBase64.Contains(";base64,"))
            {
                string[] a = request.ImageBase64.Split(',');
                request.ImageBase64 = a[1];
            }
            String FileURL = "";
            string FileDestination = System.IO.Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory());
            FileDestination = FileDestination + _iconfiguration["ProfileImagePath"];
            FileURL = _iconfiguration["ProfileImageURL"];

            if (request.ImageURL == "")
            {
                if (!string.IsNullOrEmpty(request.ImageBase64) && !string.IsNullOrEmpty(request.FileName))
                {
                    Byte[] bytes = Convert.FromBase64String(request.ImageBase64);
                    string fileType = Path.GetFileName(request.FileName.Substring(request.FileName.LastIndexOf('.') + 1));

                    string fileUniqueName = Utility.UploadFilebytes(bytes, request.FileName, FileDestination);
                    FileURL = FileURL + fileUniqueName;
                    if (fileUniqueName != null)
                    {
                        request.UniqueName = fileUniqueName;
                        request.ImageURL = FileURL;
                      
                    }
                }
            }
            else if (request.ImageURL != "")
            {
                if (!string.IsNullOrEmpty(request.ImageBase64) && !string.IsNullOrEmpty(request.FileName))
                {
                    string[] URL = request.ImageURL.Split('/');
                    request.UniqueName = URL[3].ToString();
                    FileDestination = FileDestination + "\\" + request.UniqueName;
                    System.IO.File.Delete(FileDestination);

                    FileDestination = System.IO.Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory());
                    FileDestination = FileDestination + _iconfiguration["ProfileImagePath"];
                    FileURL = _iconfiguration["ProfileImageURL"];
                    

                    Byte[] bytes = Convert.FromBase64String(request.ImageBase64);
                    string fileType = Path.GetFileName(request.FileName.Substring(request.FileName.LastIndexOf('.') + 1));

                    string fileUniqueName = Utility.UploadFilebytes(bytes, request.FileName, FileDestination);
                    FileURL = FileURL + fileUniqueName;
                    if (fileUniqueName != null)
                    {
                        request.UniqueName = fileUniqueName;
                        request.ImageURL = FileURL;                       
                    }
                }
                else if (!string.IsNullOrEmpty(request.FileName))
                {
                    string[] ImageURL = request.ImageURL.Split('/');
                    request.UniqueName = ImageURL[3].ToString();
                   
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
                    
                }
            }
        }

        private void Update_Profile_Image_Details()
        {
            try
            {
                 UploadProfileImage();
                _updateProfileImageService.UpdateProfileImage(request);

                _messages.Add(new Message_Info { Message = "Profile Image updated successfully", Type = Message_Type.SUCCESS.ToString() });

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

            _updateProfileImageService = null;

            _statusCode = HttpStatusCode.OK;

            _messages = null;
        }
    }
}
