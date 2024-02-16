using Business.Service.Models.ProductService;
using Business.Service.Repositories.ProductService;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.IO;
using UJBHelper.Common;
using UJBHelper.DataModel;

namespace Business.Service.Repositories.ProductServices.AddProductService
{
    public class AddProductService : IAddProductService
    {
        private readonly IMongoCollection<DbProductService> _products;
        private readonly IMongoCollection<ProductServiceDetails> _productsDetails;
        private readonly IMongoCollection<BusinessDetails> _businessDetails;
        private readonly IMongoCollection<User> _users;
        private readonly IMongoCollection<Categories> _categories;
        private readonly IMongoCollection<Leads> _leads;
        private readonly IMongoCollection<DbProductServiceArchive> _productsArchive;
        private readonly IMongoCollection<ProductServiceDetailsArchive> _productsDetailsArchive;
        private readonly IMongoCollection<LeadProductServiceDetails> _leadProductsDetails;
        private IConfiguration _iconfiguration;

        public AddProductService(IConfiguration config)
        {
            //  var client = new MongoClient(DbHelper.GetConnectionString());
            //  var database = client.GetDatabase(DbHelper.GetDatabaseName());
            _iconfiguration = config;
            var client = new MongoClient(_iconfiguration["ConnectionString"]);
            var database = client.GetDatabase(_iconfiguration["Database"]);
            _businessDetails = database.GetCollection<BusinessDetails>("BussinessDetails");
            _products = database.GetCollection<DbProductService>("ProductsServices");
            _productsDetails = database.GetCollection<ProductServiceDetails>("ProductsServicesDetails");
            _productsDetailsArchive = database.GetCollection<ProductServiceDetailsArchive>("ProductServiceDetailsArchive");
            _productsArchive = database.GetCollection<DbProductServiceArchive>("ProductsServicesArchive");
            _users = database.GetCollection<User>("Users");
            _leads = database.GetCollection<Leads>("Leads");
            _categories = database.GetCollection<Categories>("Categories");
            _leadProductsDetails = database.GetCollection<LeadProductServiceDetails>("LeadProductServiceDetails");
        }

        public bool Check_If_Product_Exists(string productId)
        {
            return _products.Find(x => x.Id == productId).CountDocuments() > 0;
        }

        internal List<ProductImg> UploadProductImages_Update(List<UploadedProductImg> ProdImgs, List<ProductImg> DBImgs)
        {
            List<ProductImg> ProdImgList = new List<ProductImg>();
            foreach (var item in ProdImgs)
            {
                if (item.prodImgBase64.Contains(";base64,"))
                {
                    string[] a = item.prodImgBase64.Split(',');
                    item.prodImgBase64 = a[1];
                }
                String FileURL = "";
                string FileDestination = System.IO.Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory());
                FileDestination = FileDestination + _iconfiguration["ProductImgsPath"];
                FileURL = _iconfiguration["ProductImgsUrl"];
                if (item.ImageURL == "")
                {
                    if (!string.IsNullOrEmpty(item.prodImgBase64) && !string.IsNullOrEmpty(item.prodImgName))
                    {
                        Byte[] bytes = Convert.FromBase64String(item.prodImgBase64);
                        string fileType = Path.GetFileName(item.prodImgName.Substring(item.prodImgName.LastIndexOf('.') + 1));
                        string fileUniqueName = Utility.UploadFilebytes(bytes, item.prodImgName, FileDestination);
                        FileURL = FileURL + fileUniqueName;
                        ProductImg img = new ProductImg();
                        img.ImageURL = FileURL;
                        img.UniqueName = fileUniqueName;
                        img.prodImgName = item.prodImgName;
                        img.isDefaultImg = item.isDefaultImg;
                        ProdImgList.Add(img);
                    }
                }
                else if (item.ImageURL != "")
                {
                    if (!string.IsNullOrEmpty(item.prodImgBase64) && !string.IsNullOrEmpty(item.prodImgName))
                    {
                        Byte[] bytes = Convert.FromBase64String(item.prodImgBase64);
                        string fileType = Path.GetFileName(item.prodImgName.Substring(item.prodImgName.LastIndexOf('.') + 1));
                        string fileUniqueName = Utility.UploadFilebytes(bytes, item.prodImgName, FileDestination);
                        FileURL = FileURL + fileUniqueName;
                        ProductImg img = new ProductImg();
                        img.ImageURL = FileURL;
                        img.UniqueName = fileUniqueName;
                        img.prodImgName = item.prodImgName;
                        img.isDefaultImg = item.isDefaultImg;
                        ProdImgList.Add(img);
                    }
                    else
                    {
                        string extn = Path.GetFileName(DBImgs.Find(x => x.ImageURL == item.ImageURL).prodImgName.Substring(DBImgs.Find(x => x.ImageURL == item.ImageURL).prodImgName.LastIndexOf('.') + 1));
                        string fileName = string.Concat(DateTime.Now.ToString("ddMMyyyyHHmmssffff") + '.' + extn);
                        ProductImg img = new ProductImg();
                        try
                        {
                            string FileDestination1 = System.IO.Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory());
                            FileDestination1 = FileDestination1 + _iconfiguration["ProductImgsPath"] + "\\" + DBImgs.Find(x => x.ImageURL == item.ImageURL).UniqueName;
                            string DestinationFileName = System.IO.Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory()) + _iconfiguration["ProductImgsPath"] + "\\" + fileName;
                            System.IO.File.Copy(FileDestination1, DestinationFileName);
                        }
                        catch
                        {

                        }
                        img.ImageURL = FileURL = FileURL + fileName;
                        img.UniqueName = fileName;
                        img.prodImgName = DBImgs.Find(x => x.ImageURL == item.ImageURL).prodImgName;
                        img.isDefaultImg = item.isDefaultImg;//DBImgs.Find(x => x.ImageURL == item.ImageURL).isDefaultImg;
                        ProdImgList.Add(img);
                    }
                }
            }
            return ProdImgList;
        }
        internal List<ProductImg> UploadProductImages_Insert(List<UploadedProductImg> ProdImgs)
        {
            List<ProductImg> ProdImgList = new List<ProductImg>();
            foreach (var item in ProdImgs)
            {
                if (item.prodImgBase64.Contains(";base64,"))
                {
                    string[] a = item.prodImgBase64.Split(',');
                    item.prodImgBase64 = a[1];
                }
                String FileURL = "";
                string FileDestination = System.IO.Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory());
                FileDestination = FileDestination + _iconfiguration["ProductImgsPath"];
                FileURL = _iconfiguration["ProductImgsUrl"];
                if (item.ImageURL == "")
                {
                    if (!string.IsNullOrEmpty(item.prodImgBase64) && !string.IsNullOrEmpty(item.prodImgName))
                    {
                        Byte[] bytes = Convert.FromBase64String(item.prodImgBase64);
                        string fileType = Path.GetFileName(item.prodImgName.Substring(item.prodImgName.LastIndexOf('.') + 1));
                        string fileUniqueName = Utility.UploadFilebytes(bytes, item.prodImgName, FileDestination);
                        FileURL = FileURL + fileUniqueName;
                        ProductImg img = new ProductImg();
                        img.ImageURL = FileURL;
                        img.UniqueName = fileUniqueName;
                        img.prodImgName = item.prodImgName;
                        img.isDefaultImg = item.isDefaultImg;
                        ProdImgList.Add(img);
                    }
                }

            }
            return ProdImgList;
        }

        public void Insert_New_Product(Post_Request request)
        {
            List<ProductImg> imgs = new List<ProductImg>();
            imgs = UploadProductImages_Insert(request.productImages);
            TimeSpan diff = DateTime.Now - DateTime.UtcNow;
            string[] timezone = DateTime.Now.ToString("zzz").Split(new char[] { '+', '-', ':' });

            var p = new DbProductService
            {
                bussinessId = request.businessId,
                description = request.description,
                isActive = true,
                minimumDealValue = request.minimumDealValue,
                name = request.name,
                productPrice = request.productPrice,
                shareType = request.shareType,
                type = request.type,
                typeOf = request.typeOf,
                url = request.url,
                ProductImg = imgs,
                //ProductImg = request.productImages,
                Created = new Created
                {
                    created_By = request.createdBy,
                    created_On = DateTime.Now.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0))
                },
                Updated = new Updated()
            };

            _products.InsertOne(p);
            var productId = p.Id;



            foreach (var pr in request.productsOrServices)
            {
                if (string.IsNullOrWhiteSpace(pr.productDetailsId))
                {
                    Insert_Product_Service_Details(pr, p.Id, request.createdBy);
                }
                else
                {
                    var res = _productsDetails.Find(x => x.Id == pr.productDetailsId).Project(x => new ProductServiceDetails
                    {
                        from = x.from,
                        to = x.to,
                        type = x.type,
                        value = x.value,
                        productName = x.productName,
                        isActive = x.isActive
                    }).FirstOrDefault();
                    if (res.from != pr.from || res.to != pr.to || res.type != pr.type || res.productName != pr.productName || res.isActive != pr.isActive)
                    {
                        Update_Product_Service_Details(pr, request.createdBy);
                    }
                }
            }
        }

        public string Update_Product_Details(Post_Request request)
        {
            string fileds = "";
            Post_Request pre_request = new Post_Request();
            List<ProductImg> DBImgs = new List<ProductImg>();
            DBImgs = _products.Find(x => x.Id == request.productId).Project(x => x.ProductImg).FirstOrDefault();
            List<ProductImg> imgs = new List<ProductImg>();
            if (request.isActive)
            {
                if (request.productImages != null)
                {
                    List<UploadedProductImg> matches = request.productImages.FindAll(p => p.prodImgBase64 != null && p.prodImgBase64 != "");
                    if (matches.Count > 0)
                    {
                        fileds = "\"Product Images\", ";
                    }
                    imgs = UploadProductImages_Update(request.productImages, DBImgs);

                    foreach (var item in DBImgs)
                    {
                        string FileDestination = System.IO.Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory());
                        FileDestination = FileDestination + _iconfiguration["ProductImgsPath"];
                        FileDestination = FileDestination + "\\" + item.UniqueName;
                        System.IO.File.Delete(FileDestination);
                    }
                }
            }
            else
            {
                imgs = DBImgs;
            }

            TimeSpan diff = DateTime.Now - DateTime.UtcNow;
            string[] timezone = DateTime.Now.ToString("zzz").Split(new char[] { '+', '-', ':' });


            var prdres = _products.Find(x => x.Id == request.productId).Project(x => new DbProductService
            {
                productPrice = x.productPrice,
                minimumDealValue = x.minimumDealValue,
                description = x.description,
                name = x.name,
                url = x.url,
                typeOf = x.typeOf,
                shareType = x.shareType,
                type = x.type,
                isActive = x.isActive,
                Created = new Created
                {
                    created_On = x.Created.created_On,
                    created_By = x.Created.created_By,

                }
            }).FirstOrDefault();

            if (prdres.type != request.type || prdres.url != request.url || prdres.name != request.name || prdres.description != request.description || prdres.minimumDealValue != request.minimumDealValue || prdres.productPrice != request.productPrice || prdres.shareType != request.shareType || prdres.typeOf != request.typeOf)
            {
                //if (prdres.type != request.type)
                //{
                //    prdres.type = request.type;
                //}
                if (prdres.name != request.name)
                {
                    fileds += "\"Name\", ";
                }
                if (!string.IsNullOrEmpty(prdres.url) || !string.IsNullOrEmpty(request.url))
                {
                    if (prdres.url != request.url)
                    {
                        fileds += "\"Url\", ";
                    }
                }
                if (!string.IsNullOrEmpty(prdres.description) || !string.IsNullOrEmpty(request.description))
                {
                    if (prdres.description != request.description)
                    {
                        fileds += "\"Description\", ";
                    }
                }
                if (prdres.minimumDealValue != request.minimumDealValue)
                {
                    fileds += "\"Minimum Deal Value\", ";
                }
                if (prdres.productPrice != request.productPrice)
                {
                    fileds += "\"Price\", ";
                }
                if (prdres.shareType != request.shareType)
                {
                    fileds += "\"Share Type (Single/Multiple)\", ";
                }
                if (prdres.typeOf != request.typeOf)
                {
                    fileds += "\"Slab/Product\", ";
                }


                Insert_Product_Service_Archive(prdres, request.productId, request.createdBy, fileds, "Updated");
            }
            if (!request.isActive)
            {
                Insert_Product_Service_Archive(prdres, request.productId, request.createdBy, fileds, "Deleted");
            }



            _products.FindOneAndUpdate(
                Builders<DbProductService>.Filter.Eq(x => x.Id, request.productId),
                        Builders<DbProductService>.Update
                        .Set(x => x.description, request.description)
                        .Set(x => x.isActive, request.isActive)
                        .Set(x => x.minimumDealValue, request.minimumDealValue)
                        .Set(x => x.name, request.name)
                        .Set(x => x.productPrice, request.productPrice)
                        .Set(x => x.shareType, request.shareType)
                        .Set(x => x.type, request.type)
                        .Set(x => x.typeOf, request.typeOf)
                        .Set(x => x.url, request.url)
                        .Set(x => x.ProductImg, imgs)
                        .Set(x => x.Updated, new Updated
                        {
                            updated_By = request.createdBy,
                            updated_On = DateTime.Now.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0))
                        })
                        );

            if (prdres.shareType != request.shareType || prdres.typeOf != request.typeOf)
            {

                var res = _productsDetails.Find(x => x.prodservId == request.productId).Project(x => new ProductInfo
                {
                    productDetailsId = x.Id,
                    from = x.from,
                    to = x.to,
                    type = x.type,
                    value = x.value,
                    productName = x.productName,
                    isActive = x.isActive,
                    createdOn = x.created.created_On
                }).ToList();

                foreach (ProductInfo pr in res)
                {

                    pr.isActive = false;
                    Update_Product_Service_Details(pr, request.createdBy);
                    Insert_Product_Service_Details_Archive(pr, pr.productDetailsId, request.createdBy, request.businessId, "Deleted", "", request.productId);
                }
                //  _productsDetails.DeleteMany(x => x.prodservId == request.productId);
            }


            foreach (var pr in request.productsOrServices)
            {
                if (string.IsNullOrWhiteSpace(pr.productDetailsId))
                {
                    Insert_Product_Service_Details(pr, request.productId, request.createdBy);
                }
                else
                {
                    var res = _productsDetails.Find(x => x.Id == pr.productDetailsId && x.isActive).Project(x => new ProductInfo
                    {
                        productDetailsId = x.Id,
                        from = x.from,
                        to = x.to,
                        type = x.type,
                        value = x.value,
                        productName = x.productName,
                        isActive = x.isActive
                    }).FirstOrDefault();
                    if (res != null)
                    {
                        if (res.from != pr.from || res.value != pr.value || res.to != pr.to || res.type != pr.type || res.productName != pr.productName || res.isActive != pr.isActive)
                        {

                            if (res.from != pr.from || res.to != pr.to && !fileds.Contains("Slab Value"))
                            {
                                fileds += "\"Slab Value\", ";
                            }
                            if (res.value != pr.value || res.type != pr.type && !fileds.Contains("Shared Percentage/Amount"))
                            {
                                fileds += "\"Shared Percentage/Amount\", ";
                            }
                            //if (res.type != pr.type)
                            //{
                            //    fileds += " Product  share percentage/Amount,";
                            //}
                            if (!string.IsNullOrEmpty(pr.productName) || !string.IsNullOrEmpty(res.productName))
                            {
                                if (res.productName != pr.productName && !fileds.Contains("Sub Product"))
                                {
                                    fileds += "\"Sub Product\", ";
                                }
                            }

                            Update_Product_Service_Details(pr, request.createdBy);
                            Insert_Product_Service_Details_Archive(res, res.productDetailsId, request.createdBy, request.businessId, "Updated", fileds, request.productId);
                        }

                    }
                    else
                    {
                        Insert_Product_Service_Details(pr, request.productId, request.createdBy);
                    }
                }
            }
            var notupdtaedLead = _leads.Find(x => x.referredProductORServicesId == request.productId && x.typeOf == null).Project(x => x.Id).ToList();
            if (notupdtaedLead.Count > 0)
            {
                foreach (string leadId in notupdtaedLead)
                {
                    UpdatedLeadData(request.productId, leadId);
                }
            }

            if (fileds != "")
            {
                fileds = fileds.Trim();
                fileds = fileds.TrimEnd(',');

                fileds += " of " + request.type + " - \"" + request.name + "\"";
            }
            return fileds;
        }
        private void UpdatedLeadData(string ProductID, string leadId)
        {

            TimeSpan diff = DateTime.Now - DateTime.UtcNow;
            string[] timezone = DateTime.Now.ToString("zzz").Split(new char[] { '+', '-', ':' });
            var shareType = _products.Find(x => x.Id == ProductID).FirstOrDefault().shareType;
            var _typeof = _products.Find(x => x.Id == ProductID).FirstOrDefault().typeOf;
            var referredby = _leads.Find(x => x.Id == leadId).FirstOrDefault().referredBy.userId;

            _leads.FindOneAndUpdate(
                Builders<Leads>.Filter.Eq(x => x.Id, leadId),
                        Builders<Leads>.Update
                        .Set(x => x.typeOf, _typeof)
                        .Set(x => x.shareType, shareType));

            var productsOrServices = _productsDetails.Find(x => x.prodservId == ProductID && x.isActive).Project(x => new ProductServiceDetails
            {
                productName = x.productName,
                Id = x.Id,
                from = x.from,
                to = x.to,
                isActive = x.isActive,
                type = x.type,
                value = x.value,

            }).ToList();
            foreach (var productsde in productsOrServices)
            {
                var ps = new LeadProductServiceDetails
                {
                    prodservdetailId = productsde.Id,
                    LeadId = leadId,
                    from = productsde.from,
                    to = productsde.to,
                    prodservId = ProductID,
                    productName = productsde.productName,
                    type = (int)productsde.type,
                    value = (int)productsde.value,
                    isActive = true,
                    created = new Created
                    {
                        created_By = referredby,
                        created_On = DateTime.Now.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0))
                    },
                    updated = new Updated()
                };

                _leadProductsDetails.InsertOne(ps);
            }

        }

        private void Update_Product_Service_Details(ProductInfo pr, string updatedBy)
        {
            TimeSpan diff = DateTime.Now - DateTime.UtcNow;
            string[] timezone = DateTime.Now.ToString("zzz").Split(new char[] { '+', '-', ':' });

            _productsDetails.FindOneAndUpdate(
                Builders<ProductServiceDetails>.Filter.Eq(x => x.Id, pr.productDetailsId),
                Builders<ProductServiceDetails>.Update
                .Set(x => x.from, pr.from)
                .Set(x => x.to, pr.to)
                .Set(x => x.type, pr.type)
                .Set(x => x.value, pr.value)
                .Set(x => x.productName, pr.productName)
                .Set(x => x.isActive, pr.isActive)
                .Set(x => x.updated, new Updated
                {
                    updated_By = updatedBy,
                    updated_On = DateTime.Now.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0))

                })
                );
        }

        private void Insert_Product_Service_Details(ProductInfo pr, string id, string createdBy)
        {
            TimeSpan diff = DateTime.Now - DateTime.UtcNow;
            string[] timezone = DateTime.Now.ToString("zzz").Split(new char[] { '+', '-', ':' });

            var ps = new ProductServiceDetails
            {
                from = pr.from,
                to = pr.to,
                prodservId = id,
                productName = pr.productName,
                type = pr.type,
                value = pr.value,
                isActive = true,
                created = new Created
                {
                    created_By = createdBy,
                    created_On = DateTime.Now.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0))
                },
                updated = new Updated()
            };

            _productsDetails.InsertOne(ps);
        }

        private void Insert_Product_Service_Details_Archive(ProductInfo pr, string id, string createdBy, string businessId, string Action, string fileds, string productId)
        {
            TimeSpan diff = DateTime.Now - DateTime.UtcNow;
            string[] timezone = DateTime.Now.ToString("zzz").Split(new char[] { '+', '-', ':' });

            var ps = new ProductServiceDetailsArchive
            {
                prodservDdtailsId = pr.productDetailsId,

                from = pr.from,
                to = pr.to,
                prodservId = productId,
                BussinessId = businessId,
                productName = pr.productName,
                type = pr.type,
                value = pr.value,
                isActive = true,
                DetailCreatedDated = pr.createdOn,
                created = new Created
                {
                    created_By = createdBy,
                    created_On = DateTime.Now.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0))
                },
                Action = Action,
                UpdatedFields = fileds
            };

            _productsDetailsArchive.InsertOne(ps);
        }

        private void Insert_Product_Service_Archive(DbProductService prdres, string id, string createdBy, string fileds, string Action)
        {
            TimeSpan diff = DateTime.Now - DateTime.UtcNow;
            string[] timezone = DateTime.Now.ToString("zzz").Split(new char[] { '+', '-', ':' });

            var p = new DbProductServiceArchive
            {
                productId = prdres.Id,
                bussinessId = prdres.bussinessId,
                description = prdres.description,
                isActive = prdres.isActive,
                minimumDealValue = prdres.minimumDealValue,
                name = prdres.name,
                productPrice = prdres.productPrice,
                shareType = prdres.shareType,
                type = prdres.type,
                typeOf = prdres.typeOf,
                url = prdres.url,
                ProductImg = prdres.ProductImg,
                ProductCreatedOn = prdres.Created.created_On,
                Created = new Created
                {
                    created_By = createdBy,
                    created_On = DateTime.Now.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0))
                },
                UpdatedFields = fileds,
                Action = Action
            };

            _productsArchive.InsertOne(p);
        }

        public void Update_Product_Images(Put_Request request)
        {
            //TimeSpan diff = DateTime.Now - DateTime.UtcNow;
            //string[] timezone = DateTime.Now.ToString("zzz").Split(new char[] { '+', '-', ':' });

            //var imgs = new List<ProductImg>();

            //foreach (var i in request.ProductImages)
            //{
            //    imgs.Add(new ProductImg
            //    {
            //        prodImgBase64 = i.prodImgBase64,
            //        prodImgName = i.prodImgName,
            //        isDefaultImg = i.isDefaultImg
            //    });
            //}

            //_products.FindOneAndUpdate(
            //    Builders<DbProductService>.Filter.Eq(x => x.Id, request.productId),
            //    Builders<DbProductService>.Update
            //    .Set(x => x.ProductImg, imgs)
            //    .Set(x => x.Updated, new Updated
            //    {
            //        updated_By = request.updatedBy,
            //        updated_On = DateTime.Now.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0))

            //    })
            //    );
        }

        public bool Check_If_User_Exists(string userId)
        {
            return _users.Find(x => x._id == userId).CountDocuments() > 0;
        }

        public List<Get_Request> Get_Products_By_User(string userId)
        {
            var businessid = _businessDetails.Find(x => x.UserId == userId).FirstOrDefault().Id;
            var catids = _businessDetails.Find(x => x.UserId == userId).Project(x => x.Categories).FirstOrDefault();
            bool sharetype = false;
            if (catids != null && catids.Count != 0)
            {
                var categories = _categories.Find(x => catids.Contains(x.Id)).Project(x => x.PercentageShare).ToList();
                foreach (var category in categories)
                {
                    if (!category)
                    {
                        sharetype = category;
                    }
                }
            }

            var res = new List<Get_Request>();
            res = _products.Find(x => x.bussinessId == businessid && x.type == "Product" && x.isActive).Project(x => new Get_Request
            {
                productId = x.Id,
                name = x.name,
                type = x.type,
                url = x.url,
                productPrice = x.productPrice,
                minimumDealValue = x.minimumDealValue,
                isActive = x.isActive,
                description = x.description,
                productImages = x.ProductImg,
                shareType = x.shareType,
                typeOf = x.typeOf
            }).ToList();

            foreach (var r in res)
            {
                var refcnt = _leads.Find(x => x.referredProductORServicesId == r.productId && x.referralStatus != 2 && x.dealStatus != 3).CountDocuments();
                if (refcnt > 0)
                {
                    r.IsAllowDelete = false;
                }
                else
                {
                    r.IsAllowDelete = true;
                }
                r.productsOrServices = _productsDetails.Find(x => x.prodservId == r.productId && x.isActive).Project(x => new ProductInfo
                {
                    productName = x.productName,
                    productDetailsId = x.Id,
                    from = x.from,
                    to = x.to,
                    isActive = x.isActive,
                    type = x.type,
                    value = x.value,
                    createdOn = x.created.created_On,
                    updatedOn = x.updated.updated_On
                }).ToList();
            }

            return res;
        }

        public List<Get_Request> Get_Services_By_User(string userId)
        {
            var businessid = _businessDetails.Find(x => x.UserId == userId).FirstOrDefault().Id;


            var res = new List<Get_Request>();
            res = _products.Find(x => x.bussinessId == businessid && x.type == "Service" && x.isActive).Project(x => new Get_Request
            {
                productId = x.Id,
                name = x.name,
                type = x.type,
                url = x.url,
                productPrice = x.productPrice,
                minimumDealValue = x.minimumDealValue,
                isActive = x.isActive,
                description = x.description,
                productImages = x.ProductImg,
                shareType = x.shareType,
                typeOf = x.typeOf
            }).ToList();

            foreach (var r in res)
            {
                var refcnt = _leads.Find(x => x.referredProductORServicesId == r.productId && x.referralStatus != 2 && x.dealStatus != 3).CountDocuments();
                if (refcnt > 0)
                {
                    r.IsAllowDelete = false;
                }
                else
                {
                    r.IsAllowDelete = true;
                }
                r.productsOrServices = _productsDetails.Find(x => x.prodservId == r.productId && x.isActive).Project(x => new ProductInfo
                {
                    productName = x.productName,
                    productDetailsId = x.Id,
                    from = x.from,
                    to = x.to,
                    isActive = x.isActive,
                    type = x.type,
                    value = x.value,
                    createdOn = x.created.created_On,
                    updatedOn = x.updated.updated_On
                }).ToList();
            }

            return res;
        }

        public Get_Request Get_Product_Service_Details(string prodServiceId)
        {
            var res = new Get_Request();
            res = _products.Find(x => x.Id == prodServiceId).Project(x => new Get_Request
            {
                productId = x.Id,
                name = x.name,
                type = x.type,
                url = x.url,
                productPrice = x.productPrice,
                minimumDealValue = x.minimumDealValue,
                isActive = x.isActive,
                description = x.description,
                productImages = x.ProductImg,
                shareType = x.shareType,
                typeOf = x.typeOf,
                Created = x.Created,
                Updated = x.Updated
            }).FirstOrDefault();
            var refcnt = _leads.Find(x => x.referredProductORServicesId == prodServiceId && x.referralStatus != 2 && x.dealStatus != 3).CountDocuments();
            if (refcnt > 0)
            {
                res.IsAllowDelete = false;
            }
            else
            {
                res.IsAllowDelete = true;
            }
            res.productsOrServices = _productsDetails.Find(x => x.prodservId == res.productId && x.isActive).Project(x => new ProductInfo
            {
                productName = x.productName,
                productDetailsId = x.Id,
                from = x.from,
                to = x.to,
                isActive = x.isActive,
                type = x.type,
                value = x.value,
                createdOn = x.created.created_On,
                updatedOn = x.updated.updated_On
            }).ToList();

            return res;
        }

        public List<Get_Request> Get_Products_Services_By_User(string userId)
        {
            var businessid = _businessDetails.Find(x => x.UserId == userId).FirstOrDefault().Id;

            var res = new List<Get_Request>();
            res = _products.Find(x => x.bussinessId == businessid && x.isActive && (x.type == "Service" || x.type == "Product")).Project(x => new Get_Request
            {
                productId = x.Id,
                name = x.name,
                type = x.type,
                url = x.url,
                productPrice = x.productPrice,
                minimumDealValue = x.minimumDealValue,
                isActive = x.isActive,
                description = x.description,
                productImages = x.ProductImg,
                shareType = x.shareType,
                typeOf = x.typeOf
            }).ToList();

            foreach (var r in res)
            {
                var refcnt = _leads.Find(x => x.referredProductORServicesId == r.productId && x.referralStatus != 2 && x.dealStatus != 3).CountDocuments();
                if (refcnt > 0)
                {
                    r.IsAllowDelete = false;
                }
                else
                {
                    r.IsAllowDelete = true;
                }
                r.productsOrServices = _productsDetails.Find(x => x.prodservId == r.productId && x.isActive).Project(x => new ProductInfo
                {
                    productName = x.productName,
                    productDetailsId = x.Id,
                    from = x.from,
                    to = x.to,
                    isActive = x.isActive,
                    type = x.type,
                    value = x.value,
                    createdOn = x.created.created_On,
                    updatedOn = x.updated.updated_On
                }).ToList();
            }

            return res;
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

            //_productsDetails.FindOneAndUpdate(
            //    Builders<ProductServiceDetails>.Filter.Eq(x => x.Id, ProddeatislId),
            //    Builders<ProductServiceDetails>.Update

            //    .Set(x => x.isActive, false)

            //    );
        }

        public bool Check_If_Product_Image_Exists(Post_Request request)
        {
            // return _productsDetails.Find(x => x.Id == productDeatislId).CountDocuments() > 0;
            return false;
        }
        public bool Check_If_Referral_Exists(string productId)
        {
            return _leads.Find(x => x.referredProductORServicesId == productId && x.referralStatus != 2 && x.dealStatus != 3).CountDocuments() == 0;
        }
    }
}
