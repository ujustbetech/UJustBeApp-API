using Business.Service.Models.DeleteProductService;
using Business.Service.Repositories.DeleteProductService;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using UJBHelper.Common;
using UJBHelper.DataModel;

namespace Business.Service.Services.ProductServices
{
    public class DeleteProductService : IDeleteProductService
    {
        private readonly IMongoCollection<DbProductService> _products;
        private readonly IMongoCollection<ProductServiceDetails> _productsDetails;
        private readonly IMongoCollection<BusinessDetails> _businessDetails;
        private readonly IMongoCollection<User> _users;
        private readonly IMongoCollection<Leads> _leads;
        private IConfiguration _iconfiguration;

        public DeleteProductService(IConfiguration config)
        {
            //  var client = new MongoClient(DbHelper.GetConnectionString());
            //  var database = client.GetDatabase(DbHelper.GetDatabaseName());
            _iconfiguration = config;
            var client = new MongoClient(_iconfiguration["ConnectionString"]);
            var database = client.GetDatabase(_iconfiguration["Database"]);
            _businessDetails = database.GetCollection<BusinessDetails>("BussinessDetails");
            _products = database.GetCollection<DbProductService>("ProductsServices");
            _productsDetails = database.GetCollection<ProductServiceDetails>("ProductsServicesDetails");
            _users = database.GetCollection<User>("Users");
            _leads = database.GetCollection<Leads>("Leads");
        }

        public bool Check_If_Product_Exists(string productId)
        {
            return _products.Find(x => x.Id == productId).CountDocuments() > 0;
        }

        public bool Check_If_Referral_Exists(string productId)
        {
            return _leads.Find(x => x.referredProductORServicesId == productId && x.referralStatus != 2 && x.dealStatus != 3).CountDocuments() == 0;
        }

        public void Delete_Products_service(string ProdServiceId)
        {
            TimeSpan diff = DateTime.Now - DateTime.UtcNow;
            string[] timezone = DateTime.Now.ToString("zzz").Split(new char[] { '+', '-', ':' });

            _products.FindOneAndUpdate(
                Builders<DbProductService>.Filter.Eq(x => x.Id, ProdServiceId),
                Builders<DbProductService>.Update
                .Set(x => x.isActive, false)
                );

        }


        public void Delete_ProductsDetails(string ProddeatislId)
        {
            TimeSpan diff = DateTime.Now - DateTime.UtcNow;
            string[] timezone = DateTime.Now.ToString("zzz").Split(new char[] { '+', '-', ':' });

            _productsDetails.FindOneAndUpdate(
                Builders<ProductServiceDetails>.Filter.Eq(x => x.Id, ProddeatislId),
                Builders<ProductServiceDetails>.Update

                .Set(x => x.isActive, false)

                );
        }

        public bool Check_If_ProductDetails_Exists(string productDeatislId)
        {
            return _productsDetails.Find(x => x.Id == productDeatislId).CountDocuments() > 0;
        }

        public void Delete_Products_service_Images(Post_Request request)
        {
            TimeSpan diff = DateTime.Now - DateTime.UtcNow;
            string[] timezone = DateTime.Now.ToString("zzz").Split(new char[] { '+', '-', ':' });

            var update = Builders<DbProductService>.Update.PullFilter(p => p.ProductImg,
                                                f => f.UniqueName == request.ImgUniquename);
            _products.FindOneAndUpdate(Builders<DbProductService>.Filter.Eq(x => x.Id, request.ProductId), update);
        }

        public bool Check_If_Product_Image_Exists(Post_Request request)
        {
            var filter = Builders<DbProductService>.Filter.Eq(x => x.Id, request.ProductId);
            filter = filter & Builders<DbProductService>.Filter.ElemMatch(z => z.ProductImg, a => a.UniqueName == request.ImgUniquename);

            return _products.Find(filter).CountDocuments() > 0;

        }
    }
}
