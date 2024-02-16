using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using Business.Service.Models.DeleteProductService;
using Business.Service.Repositories.DeleteProductService;
using Microsoft.Extensions.Configuration;
using UJBHelper.Common;

namespace Business.Service.Manager.DeleteProductservice
{
    public class DeleteProductImages : IDisposable
    {

        private IDeleteProductService _deleteProductService;
        public HttpStatusCode _statusCode = HttpStatusCode.OK;
        public List<Message_Info> _messages = null;
        private Post_Request request;
        private IConfiguration _iconfiguration;

        public DeleteProductImages(Post_Request request, IDeleteProductService deleteProductService,IConfiguration iconfiguration)
        {
            this.request = request;
            _deleteProductService = deleteProductService;
            _messages = new List<Message_Info>();
            _iconfiguration = iconfiguration;
        }
        public void Process()
        {
            if (Check_If_Product_Exists())
            {
                if (Check_If_Product_Image_Exists())
                {
                    Delete_Product_Service_Images();
                }
            }
        }



        private void Delete_Product_Service_Images()
        {
            try
            {
                if (!string.IsNullOrEmpty(request.ImgUniquename))
                {
                    String FileURL = "";
                    string FileDestination = System.IO.Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory());

                    FileDestination = FileDestination + _iconfiguration["ProductImgsPath"];
                    FileURL = _iconfiguration["ProductImgsUrl"];
                    FileDestination = FileDestination + "\\" + request.ImgUniquename;
                    if (System.IO.File.Exists(FileDestination))
                    {
                        System.IO.File.Delete(FileDestination);
                    }

                    _deleteProductService.Delete_Products_service_Images(request);

                    _messages.Add(new Message_Info { Message = "Product Service Image Deleted successfully", Type = Message_Type.SUCCESS.ToString() });

                    _statusCode = HttpStatusCode.OK;
                }
            }

            catch (Exception ex)
            {
                Logger.Log.Error(Assembly.GetCallingAssembly().GetName().Name + "\n\t" + ex.ToString());

                _messages.Add(new Message_Info { Message = "Exception Occured", Type = Message_Type.ERROR.ToString() });

                _statusCode = HttpStatusCode.InternalServerError;
                throw;
            }
        }


        private bool Check_If_Product_Exists()
        {
            try
            {
                if (_deleteProductService.Check_If_Product_Exists(request.ProductId))
                {
                    return true;
                }
                _messages.Add(new Message_Info
                {
                    Message = "No product Found",
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
                    Message = "No product Found",
                    Type = Message_Type.ERROR.ToString()
                });

                _statusCode = HttpStatusCode.NotFound;

                return false;
            }
        }

        private bool Check_If_Product_Image_Exists()
        {
            try
            {
                if (_deleteProductService.Check_If_Product_Image_Exists(request))
                {
                    return true;
                }
                _messages.Add(new Message_Info
                {
                    Message = "No images found for given values",
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
                    Message = "No images found for given values",
                    Type = Message_Type.ERROR.ToString()
                });

                _statusCode = HttpStatusCode.NotFound;

                return false;
            }
        }

        public void Dispose()
        {
            request = null;

            // type = null;

            _deleteProductService = null;

            _statusCode = HttpStatusCode.OK;

            _messages = null;
        }

    }
}
