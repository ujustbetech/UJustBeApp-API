using System.Collections.Generic;
using System.Linq;
using MongoDB.Driver.Linq;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Search.Service.Models.Dashboard;
using Search.Service.Repositories.Dashboard;
using UJBHelper.DataModel;
using UJBHelper.Common;
using UJBHelper.DataModel.Common;
using System;
using MongoDB.Bson;

namespace Search.Service.Services.Dashboard
{
    public class DashboardService : IDashboardService
    {
        private readonly IMongoCollection<User> _users;
        private readonly IMongoCollection<BusinessDetails> _businessDetails;
        private readonly IMongoCollection<DbProductService> _productService;
        private readonly IMongoCollection<ProductServiceDetails> _productServiceDetails;
        private readonly IMongoCollection<Categories> _categories;
        private IConfiguration _iconfiguration;

        public DashboardService(IConfiguration config)
        {
            _iconfiguration = config;
            //  var client = new MongoClient(DbHelper.GetConnectionString());
            //  var database = client.GetDatabase(DbHelper.GetDatabaseName());
            var client = new MongoClient(_iconfiguration["ConnectionString"]);
            var database = client.GetDatabase(_iconfiguration["Database"]);
            _users = database.GetCollection<User>("Users");
            _businessDetails = database.GetCollection<BusinessDetails>("BussinessDetails");
            _productService = database.GetCollection<DbProductService>("ProductsServices");
            _productServiceDetails = database.GetCollection<ProductServiceDetails>("ProductsServicesDetails");
            _categories = database.GetCollection<Categories>("Categories");

        }

        public Get_Request Get_Business_By_Search(Post_Request request)
        {
            var response = new Get_Request();
            /// request.searchTerm = "/"+request.searchTerm.ToLower()+"/";
            request.searchTerm = request.searchTerm.ToLower();

            var useridList = _users.Find(x => x.isMembershipAgreementAccepted == true && (x.firstName.ToLower().Contains(request.searchTerm) | x.lastName.ToLower().Contains(request.searchTerm))).Project(x => x._id).ToList();  // x._id != request.userId &&

            var prodBusinessIdList = _productService.Find(y => y.name.ToLower().Contains(request.searchTerm)).Project(x => x.bussinessId).ToList();
          //  var bussinessId = _businessDetails.Find(x => x.UserId != request.userId).Project(x => x.Id).FirstOrDefault();
            if (request.SearchType == SearchType.Dashboard)
            {
                response.businessList = _businessDetails.AsQueryable().Where(x => x.UserId != null  && x.isApproved.Flag == 1 && x.isSubscriptionActive && x.UserId != request.userId).Select(x => new Business_Info  //
                {
                    userId = x.UserId,
                    address = x.BusinessAddress,
                    businessDescription = x.BusinessDescription,
                    businessId = x.Id,
                    businessName = x.CompanyName,
                    businessUrl = x.WebsiteUrl,
                    categories = x.Categories,
                    rating = x.averageRating,
                    tagline = x.Tagline,
                    Logo = x.Logo
                }).ToList();

                response.businessList = response.businessList.GroupBy(x => x.businessId).Select(y => y.FirstOrDefault()).ToList();
                var blist1 = new List<Business_Info>();
                blist1.AddRange(response.businessList);
                response.businessList.Clear();
                foreach (var b in blist1)
                {
                    if (_productService.Find(x => x.bussinessId == b.businessId).CountDocuments() != 0 && _users.Find(x => x._id == b.userId && x.isActive && x.Role == "Listed Partner" && x.isMembershipAgreementAccepted == true).CountDocuments() != 0)
                    {
                        response.businessList.Add(b);
                    }
                }
                response.listCount = response.businessList.Count();
                response.businessList = Utility.ShuffleList(response.businessList);
                response.businessList = response.businessList.Skip(request.skipTotal).Take(5).OrderBy(x => x.businessName).ToList();


                foreach (var r in response.businessList)
                {
                    if (r.categories == null)
                    {
                        r.categories = new List<string>();
                    }
                    r.categories.Select(s => s ?? "").ToList();
                }

                //foreach (var r in response.businessList)
                //{
                //    r.categories = _categories.Find(x => r.categories.Contains(x.Id)).Project(x => x.categoryName).ToList();
                //}

                foreach (var biz in response.businessList)
                {
                    biz.categories_details = new List<Category_Details>();
                    var categories = _businessDetails.Find(x => x.Id == biz.businessId).Project(x => x.Categories).FirstOrDefault();
                    foreach (string cat in categories)
                    {
                        var cate = _categories.Find(x => x.Id == cat).Project(x => new Category_Details
                        {
                            categoryName = x.categoryName,
                            Id = x.Id,
                            PercentageShare = x.PercentageShare
                        }).FirstOrDefault();
                        biz.categories_details.Add(cate);
                    }
                    biz.categories = _categories.Find(x => biz.categories.Contains(x.Id)).Project(x => x.categoryName).ToList();
                    var prodIds = _productService.Find(x => x.bussinessId == biz.businessId).Project(x => x.Id).ToList();

                    var prodServicesDetails = _productServiceDetails.Find(x => prodIds.Contains(x.prodservId)).ToList();

                    biz.shareDetails = new List<Share_Details>();

                    if (prodServicesDetails.Count != 0 && prodServicesDetails != null)
                    {

                        foreach (var ps in prodServicesDetails)
                        {
                            switch (ps.type)
                            {
                                case 1:
                                    biz.shareDetails.Add(new Share_Details { shareType = "Percent", value = ps.value.ToString() });
                                    break;
                                case 2:
                                    biz.shareDetails.Add(new Share_Details { shareType = "Amount", value = ps.value.ToString() });
                                    break;
                            }
                        }
                    }
                    var userDetails = _users.Find(x => x._id == biz.userId).FirstOrDefault();
                    if (userDetails != null && string.IsNullOrWhiteSpace(biz.businessName))
                    {
                        biz.businessName = userDetails.firstName + " " + userDetails.lastName;
                        //switch (userDetails.organisationType)
                        //{
                        //    case "Individual":
                        //        b.businessName = userDetails.firstName + " " + userDetails.lastName;
                        //        break;
                        //    case "Other":
                        //        b.businessName = userDetails.firstName + " " + userDetails.lastName;
                        //        break;
                        //}
                    }
                }

                return response;
                //return response;
            }
            else if (request.SearchType == SearchType.Empty)
            {
                response.businessList = _businessDetails.AsQueryable().Where(x => x.UserId != null && x.isApproved.Flag == 1 && x.isSubscriptionActive). // && x.UserId != request.userId
                    Select(x => new Business_Info
                    {
                        userId = x.UserId,
                        address = x.BusinessAddress,
                        businessDescription = x.BusinessDescription,
                        businessId = x.Id,
                        businessName = x.CompanyName,
                        businessUrl = x.WebsiteUrl,
                        categories = x.Categories,
                        rating = x.averageRating,
                        tagline = x.Tagline,
                        Logo = x.Logo
                    }).ToList();

                //response.businessList = response.businessList.GroupBy(x => x.businessId).Select(y => y.FirstOrDefault()).ToList();
                //var blist1 = new List<Business_Info>();
                //blist1.AddRange(response.businessList);
                //response.businessList.Clear();
                //foreach (var b in blist1)
                //{
                //    if (_productService.Find(x => x.bussinessId == b.businessId).CountDocuments() != 0 && _users.Find(x => x._id == b.userId && x.isActive && x.Role == "Listed Partner").CountDocuments() != 0)
                //    {
                //        response.businessList.Add(b);
                //    }
                //}


                //foreach (var r in response.businessList)
                //{
                //    if (r.categories == null)
                //    {
                //        r.categories = new List<string>();
                //    }
                //    r.categories.Select(s => s ?? "").ToList();
                //}

                //foreach (var r in response.businessList)
                //{
                //    r.categories = _categories.Find(x => r.categories.Contains(x.Id)).Project(x => x.categoryName).ToList();
                //}

                //response.businessList = response.businessList.OrderBy(a => Guid.NewGuid()).ToList();

                //foreach (var biz in response.businessList)
                //{
                //    var prodIds = _productService.Find(x => x.bussinessId == biz.businessId).Project(x => x.Id).ToList();

                //    var prodServicesDetails = _productServiceDetails.Find(x => prodIds.Contains(x.prodservId)).ToList();

                //    biz.shareDetails = new List<Share_Details>();

                //    if (prodServicesDetails.Count != 0 && prodServicesDetails != null)
                //    {

                //        foreach (var ps in prodServicesDetails)
                //        {
                //            switch (ps.type)
                //            {
                //                case 1:
                //                    biz.shareDetails.Add(new Share_Details { shareType = "Percent", value = ps.value.ToString() });
                //                    break;
                //                case 2:
                //                    biz.shareDetails.Add(new Share_Details { shareType = "Amount", value = ps.value.ToString() });
                //                    break;
                //            }
                //        }
                //    }
                //}
                //foreach (var b in response.businessList)
                //{
                //    var userDetails = _users.Find(x => x._id == b.userId).FirstOrDefault();
                //    if (userDetails != null && string.IsNullOrWhiteSpace(b.businessName))
                //    {
                //        switch (userDetails.organisationType)
                //        {
                //            case "Individual":
                //                b.businessName = userDetails.firstName + " " + userDetails.lastName;
                //                break;
                //        }
                //    }
                //}
                //response.listCount = response.businessList.Count();
                //response.businessList = response.businessList.Skip(request.skipTotal).Take(20).OrderBy(x => x.businessName).ToList();

            }
            else if (request.SearchType == SearchType.Advanced)
            {
                if (string.IsNullOrWhiteSpace(request.searchTerm) && (request.categoryIds == null || request.categoryIds.Count == 0))
                {
                    response.businessList = _businessDetails.Find(x => x.isApproved.Flag == 1  && x.isSubscriptionActive).Project(x => new Business_Info //&& x.UserId != request.userId
                    {
                        userId = x.UserId,
                        address = x.BusinessAddress,
                        businessDescription = x.BusinessDescription,
                        businessId = x.Id,
                        businessName = x.CompanyName,
                        businessUrl = x.WebsiteUrl,
                        categories = x.Categories,
                        rating = x.averageRating,
                        tagline = x.Tagline,
                        Logo = x.Logo,
                        latitude = x.latitude,
                        longitude = x.longitude
                    }).ToList();
                }
                else if (request.categoryIds != null && request.categoryIds.Count != 0)
                {
                    var catfilter = Builders<BusinessDetails>.Filter.AnyIn(x => x.Categories, request.categoryIds)
                        & Builders<BusinessDetails>.Filter.Eq(x => x.isApproved.Flag, 1)
                        & Builders<BusinessDetails>.Filter.Eq(x => x.isSubscriptionActive, true);
                       // & Builders<BusinessDetails>.Filter.Ne(x => x.UserId, request.userId);

                    var res = _businessDetails.Find(catfilter);

                    var list = res.Project(x => new Business_Info
                    {
                        userId = x.UserId,
                        address = x.BusinessAddress,
                        businessDescription = x.BusinessDescription,
                        businessId = x.Id,
                        businessName = x.CompanyName,
                        businessUrl = x.WebsiteUrl,
                        categories = x.Categories,
                        rating = x.averageRating,
                        tagline = x.Tagline,
                        Logo = x.Logo,
                        latitude = x.latitude,
                        longitude = x.longitude
                    }).ToList();

                    response.businessList.AddRange(list.Where(x => x.businessName == null ? false : x.businessName.ToLower().Contains(request.searchTerm)).ToList());
                    response.businessList.AddRange(list.Where(x => x.tagline == null ? false : x.tagline.ToLower().Contains(request.searchTerm)).ToList());
                    response.businessList.AddRange(list.Where(x => x.address.Locality == null ? false : x.address.Locality.ToLower().Contains(request.searchTerm)).ToList());
                    response.businessList.AddRange(list.Where(x => x.address.Location == null ? false : x.address.Location.ToLower().Contains(request.searchTerm)).ToList());
                    //  response.businessList.AddRange(list.Where(x => x.address.Locality == null ? false : x.address.Locality.ToLower().Contains(request.searchTerm) || x.address.Location == null ? false : x.address.Location.ToLower().Contains(request.searchTerm)).ToList());
                    response.businessList.AddRange(list.Where(x => x.businessDescription == null ? false : x.businessDescription.ToLower().Contains(request.searchTerm)).ToList());

                    prodBusinessIdList = _productService.Find(y => y.name.ToLower().Contains(request.searchTerm.ToLower())).Project(x => x.bussinessId).ToList();  // y.bussinessId != bussinessId &&
                    prodBusinessIdList.AddRange(_productService.Find(x => x.description.ToLower().Contains(request.searchTerm.ToLower())).Project(x => x.bussinessId).ToList()); // x.bussinessId != bussinessId &&

                    prodBusinessIdList = prodBusinessIdList.GroupBy(x => x).Select(y => y.FirstOrDefault()).ToList();

                    var userIds = _users.Find(x => x.firstName.ToLower().Contains(request.searchTerm) && x.Role == "Listed Partner" && x.isMembershipAgreementAccepted == true ).Project(x => x._id).ToList(); //&& x._id != request.userId
                    userIds.AddRange(_users.Find(x => x.lastName.ToLower().Contains(request.searchTerm) && x.Role == "Listed Partner" && x.isMembershipAgreementAccepted == true ).Project(x => x._id).ToList()); //&& x._id != request.userId
                    userIds = userIds.GroupBy(x => x).Select(y => y.FirstOrDefault()).ToList();

                    response.businessList.AddRange(list.Where(x => userIds.Contains(x.userId)).ToList());
                    response.businessList.AddRange(list.Where(x => prodBusinessIdList.Contains(x.businessId)).ToList());

                    var catids = _categories.Find(x => x.categoryName.ToLower().Contains(request.searchTerm)).Project(x => x.Id).ToList();
                    foreach (var item in catids)
                    {
                        response.businessList.AddRange(list.Where(x => x.categories.Contains(item)).ToList());
                    }
                }
                else
                {
                    var res = _businessDetails.Find(x => x.isApproved.Flag == 1 && x.isSubscriptionActive ); //&& x.UserId != request.userId

                    if (_users.Find(x => x._id == request.userId).FirstOrDefault().Role != "Guest")
                    {
                        res = _businessDetails.Find(x => x.CompanyName.ToLower().Contains(request.searchTerm)  && x.isApproved.Flag == 1 && x.isSubscriptionActive); //&& x.UserId != request.userId

                        response.businessList.AddRange(res.Project(x => new Business_Info
                        {
                            userId = x.UserId,
                            address = x.BusinessAddress,
                            businessDescription = x.BusinessDescription,
                            businessId = x.Id,
                            businessName = x.CompanyName,
                            businessUrl = x.WebsiteUrl,
                            categories = x.Categories,
                            rating = x.averageRating,
                            tagline = x.Tagline,
                            Logo = x.Logo,
                            latitude = x.latitude,
                            longitude = x.longitude
                        }).ToList());
                    }

                    res = _businessDetails.Find(x => x.Tagline.ToLower().Contains(request.searchTerm)  && x.isApproved.Flag == 1 && x.isSubscriptionActive); //&& x.UserId != request.userId

                    response.businessList.AddRange(res.Project(x => new Business_Info
                    {
                        userId = x.UserId,
                        address = x.BusinessAddress,
                        businessDescription = x.BusinessDescription,
                        businessId = x.Id,
                        businessName = x.CompanyName,
                        businessUrl = x.WebsiteUrl,
                        categories = x.Categories,
                        rating = x.averageRating,
                        tagline = x.Tagline,
                        Logo = x.Logo,
                        latitude = x.latitude,
                        longitude = x.longitude
                    }).ToList());


                    res = _businessDetails.Find(x => x.BusinessDescription.ToLower().Contains(request.searchTerm)  && x.isApproved.Flag == 1 && x.isSubscriptionActive); //&& x.UserId != request.userId

                    response.businessList.AddRange(res.Project(x => new Business_Info
                    {
                        userId = x.UserId,
                        address = x.BusinessAddress,
                        businessDescription = x.BusinessDescription,
                        businessId = x.Id,
                        businessName = x.CompanyName,
                        businessUrl = x.WebsiteUrl,
                        categories = x.Categories,
                        rating = x.averageRating,
                        tagline = x.Tagline,
                        Logo = x.Logo,
                        latitude = x.latitude,
                        longitude = x.longitude
                    }).ToList());


                    res = _businessDetails.Find(x => (x.BusinessAddress.Locality.ToLower().Contains(request.searchTerm) ||
                        x.BusinessAddress.Location.ToLower().Contains(request.searchTerm)) && x.isApproved.Flag == 1 && x.isSubscriptionActive); // && x.UserId != request.userId 

                    response.businessList.AddRange(res.Project(x => new Business_Info
                    {
                        userId = x.UserId,
                        address = x.BusinessAddress,
                        businessDescription = x.BusinessDescription,
                        businessId = x.Id,
                        businessName = x.CompanyName,
                        businessUrl = x.WebsiteUrl,
                        categories = x.Categories,
                        rating = x.averageRating,
                        tagline = x.Tagline,
                        Logo = x.Logo,
                        latitude = x.latitude,
                        longitude = x.longitude
                    }).ToList());

                    var catids = _categories.Find(x => x.categoryName.ToLower().Contains(request.searchTerm)).Project(x => x.Id).ToList();
                    if (catids != null)
                    {
                        res = _businessDetails.Find(Builders<BusinessDetails>.Filter.AnyIn(x => x.Categories, catids)
                            & Builders<BusinessDetails>.Filter.Eq(x => x.isApproved.Flag, 1)
                        & Builders<BusinessDetails>.Filter.Eq(x => x.isSubscriptionActive, true)
                      //  & Builders<BusinessDetails>.Filter.Ne(x => x.UserId, request.userId)
                            );

                        response.businessList.AddRange(res.Project(x => new Business_Info
                        {
                            userId = x.UserId,
                            address = x.BusinessAddress,
                            businessDescription = x.BusinessDescription,
                            businessId = x.Id,
                            businessName = x.CompanyName,
                            businessUrl = x.WebsiteUrl,
                            categories = x.Categories,
                            rating = x.averageRating,
                            tagline = x.Tagline,
                            Logo = x.Logo,
                            latitude = x.latitude,
                            longitude = x.longitude
                        }).ToList());
                    }

                    if (_users.Find(x => x._id == request.userId).FirstOrDefault().Role != "Guest")
                    {
                        var userIds = _users.Find(x => x.firstName.ToLower().Contains(request.searchTerm) && x.Role == "Listed Partner" && x.isMembershipAgreementAccepted == true ).Project(x => x._id).ToList();//&& x._id != request.userId
                        userIds.AddRange(_users.Find(x => x.lastName.ToLower().Contains(request.searchTerm) && x.Role == "Listed Partner" && x.isMembershipAgreementAccepted == true).Project(x => x._id).ToList()); // && x._id != request.userId
                        userIds = userIds.GroupBy(x => x).Select(y => y.FirstOrDefault()).ToList();

                        res = _businessDetails.Find(x => userIds.Contains(x.UserId) && x.isApproved.Flag == 1 && x.isSubscriptionActive);



                        prodBusinessIdList = _productService.Find(y => y.name.ToLower().Contains(request.searchTerm.ToLower())).Project(x => x.bussinessId).ToList();
                        prodBusinessIdList.AddRange(_productService.Find(x => x.description.ToLower().Contains(request.searchTerm.ToLower())).Project(x => x.bussinessId).ToList());

                        prodBusinessIdList = prodBusinessIdList.GroupBy(x => x).Select(y => y.FirstOrDefault()).ToList();

                        var res1 = _businessDetails.Find(x => prodBusinessIdList.Contains(x.Id) && x.isApproved.Flag == 1 && x.isSubscriptionActive);
                        // response.businessList.AddRange(list.Where(x => prodBusinessIdList.Contains(x.businessId)).ToList());

                        response.businessList.AddRange(res.Project(x => new Business_Info
                        {
                            userId = x.UserId,
                            address = x.BusinessAddress,
                            businessDescription = x.BusinessDescription,
                            businessId = x.Id,
                            businessName = x.CompanyName,
                            businessUrl = x.WebsiteUrl,
                            categories = x.Categories,
                            rating = x.averageRating,
                            tagline = x.Tagline,
                            Logo = x.Logo,
                            latitude = x.latitude,
                            longitude = x.longitude
                        }).ToList());

                        response.businessList.AddRange(res1.Project(x => new Business_Info
                        {
                            userId = x.UserId,
                            address = x.BusinessAddress,
                            businessDescription = x.BusinessDescription,
                            businessId = x.Id,
                            businessName = x.CompanyName,
                            businessUrl = x.WebsiteUrl,
                            categories = x.Categories,
                            rating = x.averageRating,
                            tagline = x.Tagline,
                            Logo = x.Logo,
                            latitude = x.latitude,
                            longitude = x.longitude
                        }).ToList());
                    }
                }
            }

            response.businessList = response.businessList.GroupBy(x => x.businessId).Select(y => y.FirstOrDefault()).ToList();
            var blist = new List<Business_Info>();
            blist.AddRange(response.businessList);
            response.businessList.Clear();
            foreach (var b in blist)
            {
                if (_productService.Find(x => x.bussinessId == b.businessId).CountDocuments() != 0 && _users.Find(x => x._id == b.userId && x.isActive && x.Role == "Listed Partner" && x.isMembershipAgreementAccepted == true ).CountDocuments() != 0) //&& x._id != request.userId
                {
                    response.businessList.Add(b);
                }
            }

            response.listCount = response.businessList.Count();
            // response.businessList = response.businessList.Skip(request.skipTotal).Take(20).ToList();
            foreach (var r in response.businessList)
            {
                if (r.categories == null)
                {
                    r.categories = new List<string>();
                }
                r.categories.Select(s => s ?? "").ToList();
            }
            foreach (var r in response.businessList)
            {
                r.categories_details = new List<Category_Details>();
                var categories = _businessDetails.Find(x => x.Id == r.businessId).Project(x => x.Categories).FirstOrDefault();
                foreach (string cat in categories)
                {
                    var cate = _categories.Find(x => x.Id == cat).Project(x => new Category_Details
                    {
                        categoryName = x.categoryName,
                        Id = x.Id,
                        PercentageShare = x.PercentageShare
                    }).FirstOrDefault();
                    r.categories_details.Add(cate);
                }
                r.categories = _categories.Find(x => r.categories.Contains(x.Id)).Project(x => x.categoryName).ToList();

                var prodIds = _productService.Find(x => x.bussinessId == r.businessId).Project(x => x.Id).ToList();

                var prodServicesDetails = _productServiceDetails.Find(x => prodIds.Contains(x.prodservId)).ToList();

                r.shareDetails = new List<Share_Details>();

                if (prodServicesDetails.Count != 0 && prodServicesDetails != null)
                {

                    foreach (var ps in prodServicesDetails)
                    {
                        switch (ps.type)
                        {
                            case 1:
                                r.shareDetails.Add(new Share_Details { shareType = "Percent", value = ps.value.ToString() });
                                break;
                            case 2:
                                r.shareDetails.Add(new Share_Details { shareType = "Amount", value = ps.value.ToString() });
                                break;
                        }
                    }
                }

                var userDetails = _users.Find(x => x._id == r.userId).FirstOrDefault();
                if (userDetails != null && string.IsNullOrWhiteSpace(r.businessName))
                {
                    r.businessName = userDetails.firstName + " " + userDetails.lastName;
                    //switch (userDetails.organisationType)
                    //{

                    //case "Individual":
                    //    r.businessName = userDetails.firstName + " " + userDetails.lastName;
                    //    break;
                    //case "Other":
                    //    r.businessName = userDetails.firstName + " " + userDetails.lastName;
                    //    break;
                    //}
                }
            }
            double latitude = 0.0;
            double longitude = 0.0;
            if (request.sortValue == SortValue.NearToFar || request.sortValue == null)
            {
                if (request.latitude == 0 && request.longitude == 0)
                {
                    var role = _users.Find(x => x._id == request.userId).Project(x => x.Role).FirstOrDefault();

                    //var coord = new Coordinates()

                    switch (role)
                    {
                        case "Guest":
                            response.businessList = response.businessList.OrderByDescending(x => x.rating).ToList();
                            return response;
                        //break;
                        case "Partner":
                            latitude = _users.Find(x => x._id == request.userId).Project(x => x.latitude).FirstOrDefault();
                            longitude = _users.Find(x => x._id == request.userId).Project(x => x.longitude).FirstOrDefault();
                            break;
                        case "Listed Partner":
                            var busId = _businessDetails.Find(x => x.UserId == request.userId).Project(x => x.Id).FirstOrDefault();
                            latitude = _businessDetails.Find(x => x.Id == busId).Project(x => x.latitude).FirstOrDefault();
                            longitude = _businessDetails.Find(x => x.Id == busId).Project(x => x.longitude).FirstOrDefault();
                            break;
                    }
                }
                else
                {
                    latitude = request.latitude;
                    longitude = request.longitude;
                }

                foreach (var l in response.businessList)
                {
                    l.distance = new Coordinates(latitude, longitude)
                     .DistanceTo(new Coordinates(l.latitude, l.longitude), UnitOfLength.Kilometers);
                }
                response.businessList = response.businessList.OrderBy(x => x.distance).ToList();
            }
            else
            {
                switch (request.sortValue)
                {
                    case SortValue.Ascending:
                        response.businessList = response.businessList.OrderBy(x => x.businessName).ToList();
                        break;
                    case SortValue.Descending:
                        response.businessList = response.businessList.OrderByDescending(x => x.businessName).ToList();
                        break;
                    case SortValue.Rating:
                        response.businessList = response.businessList.OrderByDescending(x => x.rating).ToList();
                        break;
                }
            }
            response.businessList = response.businessList.Skip(request.skipTotal).Take(20).ToList();
            return response;
        }

        public List<Get_Suggestion> Get_Business_Suggestion(string query,string userId)
        {
            var response = new List<Get_Suggestion>();
            if (string.IsNullOrWhiteSpace(query))
            {
                query = "";
            }
            else
            {
                /// query = ""+query.ToLower()+"/";
                /// 


                 query = query.ToLower();

            }

            var res = _businessDetails.Find(x => true);
            var bussinessId = _businessDetails.Find(x => x.UserId == userId).Project(x => x.Id).FirstOrDefault();

            res = _businessDetails.Find(x=>x.CompanyName.ToLower().Contains(query) && x.isApproved.Flag == 1 && x.isSubscriptionActive ); //&& x.UserId!=userId 

            response.AddRange(res.Project(x => new Get_Suggestion
            {
                businessId = x.Id,
                businessName = x.CompanyName,
                userId = x.UserId
            }).ToList());

            res = _businessDetails.Find(x => x.Tagline.ToLower().Contains(query) && x.isApproved.Flag == 1 && x.isSubscriptionActive);// && x.UserId != userId

            response.AddRange(res.Project(x => new Get_Suggestion
            {
                businessId = x.Id,
                businessName = x.CompanyName,
                userId = x.UserId
            }).ToList());

            res = _businessDetails.Find(x => x.BusinessDescription.ToLower().Contains(query) && x.isApproved.Flag == 1 && x.isSubscriptionActive);// && x.UserId != userId

            response.AddRange(res.Project(x => new Get_Suggestion
            {
                businessId = x.Id,
                businessName = x.CompanyName,
                userId = x.UserId
            }).ToList());

            res = _businessDetails.Find(x => (x.BusinessAddress.Locality.ToLower().Contains(query) ||
                x.BusinessAddress.Location.ToLower().Contains(query)) && x.isApproved.Flag == 1 && x.isSubscriptionActive);// && x.UserId != userId

            response.AddRange(res.Project(x => new Get_Suggestion
            {
                businessId = x.Id,
                businessName = x.CompanyName,
                userId = x.UserId
            }).ToList());

            var catids = _categories.Find(x => x.categoryName.ToLower().Contains(query)).Project(x => x.Id).ToList();
            if (catids != null)
            {
                res = _businessDetails.Find(Builders<BusinessDetails>.Filter.AnyIn(x => x.Categories, catids)
                    & Builders<BusinessDetails>.Filter.Eq(x => x.isApproved.Flag, 1)
                & Builders<BusinessDetails>.Filter.Eq(x => x.isSubscriptionActive, true)
              //  & Builders<BusinessDetails>.Filter.Ne(x => x.UserId, userId)
                    );

                response.AddRange(res.Project(x => new Get_Suggestion
                {
                    businessId = x.Id,
                    businessName = x.CompanyName,
                    userId = x.UserId
                }).ToList());
            }

           var prodBusinessIdList = _productService.Find(y =>    y.name.ToLower().Contains(query.ToLower())).Project(x => x.bussinessId).ToList();//y.bussinessId != bussinessId
            prodBusinessIdList.AddRange(_productService.Find(x => x.bussinessId != bussinessId && x.description.ToLower().Contains(query.ToLower())).Project(x => x.bussinessId).ToList());



            prodBusinessIdList = prodBusinessIdList.GroupBy(x => x).Select(y => y.FirstOrDefault()).ToList();

            res = _businessDetails.Find(x => prodBusinessIdList.Contains(x.Id) && x.isApproved.Flag == 1 && x.isSubscriptionActive);
            // resp
            response.AddRange(res.Project(x => new Get_Suggestion
            {
                businessId = x.Id,
                businessName = x.CompanyName,
                userId = x.UserId
            }).ToList());


            //var MulprodServList = _productService.Find(y => y.bussinessId != bussinessId && y.name.ToLower().Contains(query.ToLower())).Project(x => x.bussinessId).ToList();
            //MulprodServList.AddRange(_productService.Find(x => x.bussinessId != bussinessId && x.description.ToLower().Contains(query.ToLower())).Project(x => x.bussinessId).ToList());



            //MulprodServList = MulprodServList.GroupBy(x => x).Select(y => y.FirstOrDefault()).ToList();

            //res = _businessDetails.Find(x => MulprodServList.Contains(x.Id) && x.isApproved.Flag == 1 && x.isSubscriptionActive);
            //// resp
            //response.AddRange(res.Project(x => new Get_Suggestion
            //{
            //    businessId = x.Id,
            //    businessName = x.CompanyName,
            //    userId = x.UserId
            //}).ToList());

            var userIds = _users.Find(x => x.firstName.ToLower().Contains(query) && x.Role == "Listed Partner").Project(x => x._id).ToList();
            userIds.AddRange(_users.Find(x => x.lastName.ToLower().Contains(query) && x.Role == "Listed Partner").Project(x => x._id).ToList());
            userIds = userIds.GroupBy(x => x).Select(y => y.FirstOrDefault()).ToList();

            res = _businessDetails.Find(x => userIds.Contains(x.UserId) && x.isApproved.Flag == 1 && x.isSubscriptionActive);// && x.UserId!=userId

            response.AddRange(res.Project(x => new Get_Suggestion
            {
                businessId = x.Id,
                businessName = x.CompanyName,
                userId = x.UserId
            }).ToList());


            //var useridList = _users.Find(x => x.firstName.ToLower().Contains(query) | x.lastName.ToLower().Contains(query)).Project(x => x._id).ToList();
            //var prodBusinessIdList = _productService.Find(y => y.name.ToLower().Contains(query)).Project(x => x.bussinessId).ToList();
            //var categoryIdList = _categories.Find(x => x.categoryName.ToLower().Contains(query)).Project(x => x.Id).ToList();

            //var f2 = Builders<BusinessDetails>.Filter.AnyIn(x => x.Categories, categoryIdList)
            //    | Builders<BusinessDetails>.Filter.Where(x => x.CompanyName.ToLower().Contains(query))
            //    | Builders<BusinessDetails>.Filter.Where(x => x.Tagline.ToLower().Contains(query))
            //    | Builders<BusinessDetails>.Filter.Where(x => x.BusinessAddress.Locality.ToLower().Contains(query))
            //    | Builders<BusinessDetails>.Filter.Where(x => x.BusinessAddress.Location.ToLower().Contains(query))
            //    | Builders<BusinessDetails>.Filter.In(x => x.UserId, useridList)
            //    | Builders<BusinessDetails>.Filter.In(x => x.Id, prodBusinessIdList);

            //res = _businessDetails.Find(f2).Project(x => new Get_Suggestion
            //{
            //    businessId = x.Id,
            //    businessName = x.CompanyName
            //}).ToList();
            response= response.GroupBy(x => x.businessId).Select(y => y.FirstOrDefault()).ToList();

          //  response = response.GroupBy(x => x.businessId).Select(y => y.FirstOrDefault()).ToList();
            var blist = new List<Get_Suggestion>();
            blist.AddRange(response);
            response.Clear();
            foreach (var b in blist)
            {
                if (_productService.Find(x => x.bussinessId == b.businessId).CountDocuments() != 0 && _users.Find(x => x._id == b.userId && x.isActive && x.Role == "Listed Partner" && x.isMembershipAgreementAccepted==true).CountDocuments() != 0)
                {
                    response.Add(b);
                }
            }

            foreach (var b in response)
            {
                var UserId = _businessDetails.Find(x => x.Id == b.businessId).Project(x => x.UserId).FirstOrDefault();
                var userDetails = _users.Find(x => x._id == UserId).FirstOrDefault();
                if (userDetails != null &&(string.IsNullOrWhiteSpace(b.businessName) || b.businessName==BsonNull.Value))
                {
                    b.businessName = userDetails.firstName + " " + userDetails.lastName;

                    //switch (userDetails.organisationType)
                    //{
                    //    case "Individual":
                    //        b.businessName = userDetails.firstName + " " + userDetails.lastName;
                    //        break;
                    //    case "Other":
                    //        b.businessName = userDetails.firstName + " " + userDetails.lastName;
                    //        break;
                    //}
                    b.mentorCode = userDetails.myMentorCode;
                }
            }
            response = response.Take(5).ToList();
            return response;
        }

        public bool Check_If_User_IsActive(string UserId)
        {
            return _users.Find(x => x._id == UserId & x.isActive == true).CountDocuments() > 0;
        }
    }

    public class FilteredUsers
    {
        public string id { get; set; }
        public double distance { get; set; }
    }
}
