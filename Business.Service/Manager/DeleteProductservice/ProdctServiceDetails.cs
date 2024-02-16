using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using Business.Service.Models.ProductService;
using Business.Service.Repositories.DeleteProductService;
using UJBHelper.Common;

namespace Business.Service.Manager.DeleteProductservice
{
    public class DeleteProductServiceDetails : IDisposable
    {
        private string ProdServicedetailId;
       // private string type;
        
        private IDeleteProductService _deleteProductService;
        public HttpStatusCode _statusCode = HttpStatusCode.OK;
        public List<Message_Info> _messages = null;
        public DeleteProductServiceDetails(string ProdServiceId,  IDeleteProductService deleteProductService)
        {
            this.ProdServicedetailId = ProdServiceId;
            //  this.type = type;
            _deleteProductService = deleteProductService;
            _messages = new List<Message_Info>();
        }
        public void Process()
        {
            if (Check_If_ProductDetails_Exists())
            {
                Delete_ProductServiceDetails();
            }
        }

       

     
        private void Delete_ProductServiceDetails()
        {
            try
            {

                _deleteProductService.Delete_ProductsDetails(ProdServicedetailId);
                //if (_response.Count == 0)
                //{
                //    _messages.Add(new Message_Info { Message = "No Products Found", Type = Message_Type.INFO.ToString() });

                //    _statusCode = HttpStatusCode.NotFound;
                //}
                //else
                //{
                    _messages.Add(new Message_Info { Message = "Slab Product Deleted successfully", Type = Message_Type.SUCCESS.ToString() });

                    _statusCode = HttpStatusCode.OK;
                //}

               // _statusCode = HttpStatusCode.InternalServerError;

            }

            catch (Exception ex)
            {
                Logger.Log.Error(Assembly.GetCallingAssembly().GetName().Name + "\n\t" + ex.ToString());

                _messages.Add(new Message_Info { Message = "Exception Occured", Type = Message_Type.ERROR.ToString() });

                _statusCode = HttpStatusCode.InternalServerError;
                throw;
            }
        }

        private bool Check_If_ProductDetails_Exists()
        {
            try
            {
                if (_deleteProductService.Check_If_ProductDetails_Exists(ProdServicedetailId))
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

        public void Dispose()
        {
            ProdServicedetailId = null;

           // type = null;

            _deleteProductService = null;

            _statusCode = HttpStatusCode.OK;

            _messages = null;
        }

    }

 
   
}
