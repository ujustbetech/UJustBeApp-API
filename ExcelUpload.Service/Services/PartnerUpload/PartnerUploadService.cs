using MongoDB.Driver;
using System;
using System.Data;
using UJBHelper.Data;
using Microsoft.Extensions.Configuration;
using UJBHelper.DataModel;
using ExcelUpload.Service.Repositories.PartnerUpload;
using System.Collections.Generic;
using System.Linq;
using UJBHelper.Common;
using System.Net;
using System.IO;
using Newtonsoft.Json;
using UJBHelper.DataModel.Common;
using Wkhtmltopdf.NetCore;

using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;

using System.Reflection;
using System.Text.RegularExpressions;

namespace ExcelUpload.Service.Services.PartnerUpload
{
    public class PartnerUploadService : IUploadPartnerDetails
    {
        private readonly IMongoCollection<User> _userDetails;
        private readonly IMongoCollection<UserOtherDetails> _userOtherDetails;
        private readonly IMongoCollection<UserKYCDetails> _userKycDetails;
        private readonly IMongoCollection<BusinessDetails> _businessDetails;
        private readonly IMongoCollection<Categories> _categoryDetails;
        private readonly IMongoCollection<DbProductService> _prdctSrvc;
        private readonly IMongoCollection<ProductServiceDetails> _prdctSrvcDetais;
        private readonly IMongoCollection<Leads> _leads;
        private readonly IMongoCollection<StateInfo> _state;
        private readonly IMongoCollection<LeadsStatusHistory> _leadStatus;
        private readonly IMongoCollection<LeadFollowUp> _followUpDetails;
        public HttpStatusCode _statusCode = HttpStatusCode.OK;
        private readonly IMongoCollection<Agreement> _agreement;
        private readonly IMongoCollection<AgreementDetails> _agreementDetails;
        private readonly IMongoCollection<DealStatus> _dealstatus;
        public List<Message_Info> _messages = null;
        readonly IGeneratePdf _generatePdf;
        private string new_latitude = null;

        private string new_longitude = null;

        public string new_role = null;

        private readonly IMongoCollection<DealStatus> _dStatus;

        private IConfiguration _iconfiguration;
        private List<User> _UserList;
        private readonly IMongoCollection<System_Default> _default;
        public PartnerUploadService(IConfiguration config, IGeneratePdf generatePdf)
        {
            var client = new MongoClient(DbHelper.GetConnectionString());

            var database = client.GetDatabase(DbHelper.GetDatabaseName());
            _followUpDetails = database.GetCollection<LeadFollowUp>("LeadFollowUp");
            _userDetails = database.GetCollection<User>("Users");
            _userOtherDetails = database.GetCollection<UserOtherDetails>("UsersOtherDetails");
            _userKycDetails = database.GetCollection<UserKYCDetails>("UserKYCDetails");
            _businessDetails = database.GetCollection<BusinessDetails>("BussinessDetails");
            _categoryDetails = database.GetCollection<Categories>("Categories");
            _prdctSrvc = database.GetCollection<DbProductService>("ProductsServices");
            _prdctSrvcDetais = database.GetCollection<ProductServiceDetails>("ProductsServicesDetails");
            _leads = database.GetCollection<Leads>("Leads");
            _leadStatus = database.GetCollection<LeadsStatusHistory>("LeadsStatusHistory");
            _agreement = database.GetCollection<Agreement>("Agreement");
            _agreementDetails = database.GetCollection<AgreementDetails>("AgreementDetails");
            _state = database.GetCollection<StateInfo>("StateInfo");
           // _state = database.GetCollection<StateInfo>("StateInfo");
            _iconfiguration = config;
            _generatePdf = generatePdf;
            _dStatus = database.GetCollection<DealStatus>("DealStatus");
            _default = database.GetCollection<System_Default>("System_Default");

        }

        public bool Check_If_MentorCode_Exist(string MentorCode)
        {
            return _userDetails.Find(x => x.myMentorCode == MentorCode).CountDocuments() > 0;
        }

        public bool Check_If_User_IsExist(string firstName, string lastName, string myMentorCode)
        {
            return _userDetails.Find(x => x.firstName == firstName & x.lastName == lastName & x.myMentorCode == myMentorCode).CountDocuments() > 0;
        }

        public bool Check_If_User_IsExist(string myMentorCode)
        {
            return _userDetails.Find(x => x.myMentorCode == myMentorCode).CountDocuments() > 0;
        }

        public bool Check_If_status_IsExist(string status)
        {
            return _dStatus.Find(x => x.StatusName == status).CountDocuments() > 0;
        }

        public string GetUserId(string firstName, string LastName, string MyMentorCode)
        {
            string Id = "";
            Id = _userDetails.Find(x => x.firstName == firstName
                           & x.lastName == LastName & x.myMentorCode == MyMentorCode).FirstOrDefault()._id;
            return Id;
        }

        public string GetUserId(string MyMentorCode)
        {
            string Id = "";
            Id = _userDetails.Find(x => x.myMentorCode == MyMentorCode).FirstOrDefault()._id;
            return Id;
        }

        public string GetBusinessUserId(string userId)
        {
            string Id = "";
            Id = _businessDetails.Find(x => x.UserId == userId).FirstOrDefault().Id;
            return Id;
        }

        public string GetCategoryId(string CategoryName)
        {
            string Id = "";
            Id = _categoryDetails.Find(x => x.categoryName == CategoryName).FirstOrDefault().Id;
            return Id;
        }
        public void Bulk_Partner_Upload(DataTable dt, string FileType)
        {
            var client = new MongoClient(DbHelper.GetConnectionString());
            var database = client.GetDatabase(DbHelper.GetDatabaseName());
            var session = client.StartSession();
            session.StartTransaction(new TransactionOptions(
            readConcern: ReadConcern.Snapshot,
            writeConcern: WriteConcern.WMajority));
            TimeSpan diff = DateTime.Now - DateTime.UtcNow;
            string[] timezone = DateTime.Now.ToString("zzz").Split(new char[] { '+', '-', ':' });
            DataTable Errordt = new DataTable();
            Errordt = dt.Clone();
            Errordt.Columns.Add("errordetails", typeof(string));
            dt.Columns.Add("errordetails", typeof(string));

            
            string customError = string.Empty;
            try
            {
                if (FileType == "ReferralDetails")
                {
                    DataSet ds = new DataSet();
                    foreach (DataRow dr in dt.Rows)
                    {
                        string errormsg = string.Empty;
                        try
                        {
                            
                            if (string.IsNullOrWhiteSpace(dr["Referral Date"].ToString().Trim()))
                            {
                                errormsg = errormsg + " Referral Date" + ",";
                            }
                            if (string.IsNullOrWhiteSpace(dr["Referred By Code"].ToString().Trim()))
                            {
                                errormsg = errormsg + " Referred By Code" + ",";
                            }
                            if (string.IsNullOrWhiteSpace(dr["Referred By"].ToString().Trim()))
                            {
                                errormsg = errormsg + " Referred By" + ",";
                            }
                            if (string.IsNullOrWhiteSpace(dr["Referred To Code"].ToString().Trim()))
                            {
                                errormsg = errormsg + " Referred To Code" + ",";
                            }
                            if (string.IsNullOrWhiteSpace(dr["Referred To"].ToString().Trim()))
                            {
                                errormsg = errormsg + " Referred To" + ",";
                            }
                            if (string.IsNullOrWhiteSpace(dr["Self Or Other"].ToString().Trim()))
                            {
                                errormsg = errormsg + " Self Or Other" + ",";
                            }
                            //if (string.IsNullOrWhiteSpace(dr["Name of Referral"].ToString().Trim()) && dr["Self Or Other"].ToString().Trim() == "Other")
                            //{
                            //    errormsg = errormsg + " Name of Referral" + ",";
                            //}
                            if (string.IsNullOrWhiteSpace(dr["Referred Product OR Services"].ToString().Trim()))
                            {
                                errormsg = errormsg + " Referred Product OR Services" + ",";
                            }
                            //if (dr["Self Or Other"].ToString().Trim() == "Other" & string.IsNullOrWhiteSpace(dr["Mobile No of the referral"].ToString().Trim()))
                            //{
                            //    errormsg = errormsg + " Mobile No of the referral" + ",";
                            //}
                            if (string.IsNullOrWhiteSpace(dr["Status"].ToString().Trim()))
                            {
                                errormsg = errormsg + " Status" + ",";

                              
                            }
                          

                            if (!string.IsNullOrEmpty(errormsg))
                            {
                                errormsg = errormsg.TrimEnd(',');
                                errormsg = errormsg + " cannot be null";
                            }
                            if (!string.IsNullOrWhiteSpace(dr["Status"].ToString().Trim()))
                            {

                                if (!Check_If_status_IsExist(dr["Status"].ToString().Trim()))
                                {
                                    if (errormsg != null)
                                    {
                                        errormsg = errormsg + " , Status does not exist. Please Check Details";
                                    }
                                    else
                                    {
                                        errormsg = errormsg + " Status does not exist. Please Check Details";
                                    }
                                }
                            }
                            string BusinessId = "";
                            string RefrdPdctServId = "";
                            if (!string.IsNullOrEmpty(dr["Referred By Code"].ToString()))
                            {
                                if (!Check_If_User_IsExist(dr["Referred By Code"].ToString().Trim()))
                                {
                                    if (errormsg != null)
                                    {
                                        errormsg = errormsg + " , User does not exist. Please Check Details";
                                    }
                                    else
                                    {
                                        errormsg = errormsg + " User does not exist. Please Check Details";
                                    }
                                }
                            }
                            if (!string.IsNullOrEmpty(dr["Referred To Code"].ToString()))
                            {

                                if (!Check_If_User_IsExist(dr["Referred To Code"].ToString().Trim()))
                                {
                                    if (errormsg != null)
                                    {
                                        errormsg = errormsg + " , User does not exist. Please Check Details";
                                    }
                                    else
                                    {
                                        errormsg = errormsg + " User does not exist. Please Check Details";
                                    }


                                }else
                                {
                                    string ReferredToUserId = GetUserId(dr["Referred To Code"].ToString().Trim());
                                    
                                    try
                                    {
                                        BusinessId = _businessDetails.Find(x => x.UserId == ReferredToUserId).FirstOrDefault().Id;
                                    }
                                    catch
                                    {
                                        errormsg = errormsg + " bussiness not availbale" + ",";
                                    }
                                   
                                    try
                                    {
                                        if (!string.IsNullOrWhiteSpace(dr["Referred Product OR Services"].ToString().Trim()))
                                        {
                                            RefrdPdctServId = _prdctSrvc.Find(x => x.bussinessId == BusinessId & x.name == dr["Referred Product OR Services"].ToString().Trim()).FirstOrDefault().Id;
                                        }
                                    }
                                    catch
                                    {
                                        errormsg = errormsg + " product not availbale" + ",";
                                    }
                                }
                            }

                            if (!string.IsNullOrEmpty(dr["Deal Value"].ToString().Trim()))
                            {
                                if (!(IsNumeric(dr["Deal Value"].ToString().Trim())))
                                {
                                    if (errormsg != null)
                                    {
                                        errormsg = errormsg + " , Deal Value is incorrect.";
                                    }
                                    else
                                    {
                                        errormsg = errormsg + " Deal Value is incorrect." + ",";
                                    }
                                }
                            }
                            var ShareRecievedUJB = 0.0;

                            if (!string.IsNullOrEmpty(dr["Share Received By UJB"].ToString().Trim()))
                            {
                                if (!(IsNumeric(dr["Share Received By UJB"].ToString().Trim())))
                                {
                                    if (errormsg != null)
                                    {
                                        errormsg = errormsg + " , Share Received By UJB Value is incorrect.";
                                    }
                                    else
                                    {
                                        errormsg = errormsg + " Share Received By UJB Value is incorrect." + ",";
                                    }
                                }
                                else
                                {
                                    ShareRecievedUJB = double.Parse(dr["Share Received By UJB"].ToString());
                                }
                            }

                            if (string.IsNullOrEmpty(errormsg))
                            {
                                // int status = (int)(DealStatusEnum)Enum.Parse(typeof(DealStatusEnum), dr["Status"].ToString().Trim());
                                int status = _dStatus.Find(x => x.StatusName == dr["Status"].ToString().Trim()).FirstOrDefault().StatusId;
                                string UserId = GetUserId(dr["Referred By Code"].ToString().Trim());
                                string referralCode = GenerateReferralCode();
                                string ReferralDate = (dr["Referral Date"].ToString().Trim());
                                string[] RefDate = ReferralDate.Split('.');
                                DateTime RefDate1 = DateTime.Parse(RefDate[1] + "/" + RefDate[0] + "/" + RefDate[2]);

                                //double d = double.Parse(dr["Referral Date"].ToString().Trim());                       
                                //DateTime ReferralDate1 =Date DateTime.FromOADate(d).ToString("MM/dd/yyyy");
                                RefDate1 = RefDate1.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0));
                                DateTime dealClosedDate1 = new DateTime();
                                if (dr["Deal Closed Date"].ToString().Trim() != "")
                                {
                                    string DealClosedDate = (dr["Deal Closed Date"].ToString().Trim());
                                    string[] dealClosedDate = DealClosedDate.Split('.');
                                    dealClosedDate1 = DateTime.Parse(dealClosedDate[1] + "/" + dealClosedDate[0] + "/" + dealClosedDate[2]);
                                    dealClosedDate1 = dealClosedDate1.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0));
                                    // double d1 = double.Parse(dr["Deal Closed Date"].ToString().Trim());
                                    // DealClosedDate = DateTime.FromOADate(d1);

                                    //DealClosedDate = DealClosedDate.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0));
                                }

                                var isSelf = false;
                                if (dr["Self Or Other"].ToString().Trim() == "Self")
                                {
                                    isSelf = true;
                                }

                                int ShareType = 0;
                                if (dr["Percent Or Amount"].ToString() == "%")
                                {
                                    ShareType = 1;
                                }
                                else if (dr["Percent Or Amount"].ToString() == "Rs")
                                {
                                    ShareType = 2;

                                }
                                var dealValue = 0.0;

                                if (dr["Deal Value"].ToString() != "")
                                {
                                    dealValue = Double.Parse(dr["Deal Value"].ToString());
                                }
                                else
                                {
                                    dealValue = 0.0;

                                }
                                var lead = new Leads
                                {
                                    referralDate = RefDate1,//DateTime.FromOADate(Double.Parse(dr["Referral Date"].ToString())),
                                    referredBy = new ReferredBy
                                    {
                                        userId = UserId,
                                        name = dr["Referred By"].ToString().Trim()
                                    },

                                    isForSelf = isSelf,
                                    referredTo = new ReferredTo
                                    {
                                        name = dr["Name of Referral"].ToString().Trim(),
                                        mobileNumber = dr["Mobile No of the referral"].ToString().Trim(),
                                        emailId = dr["Email Id of the referral"].ToString().Trim()
                                    },
                                    ReferralCode = referralCode

                                            ,

                                    referralDescription = dr["Referral Details"].ToString().Trim(),
                                    referredProductORServices = dr["Referred Product OR Services"].ToString().Trim(),
                                    referredProductORServicesId = RefrdPdctServId,
                                    referredBusinessId = BusinessId,
                                    referralStatus = 1,
                                    dealStatus = status,
                                    dealValue = dealValue,

                                    shareRecievedByPartners = new ShareRecievedByPartners
                                    {
                                        partnerID = null,
                                        RecievedByReferral = 0.0,
                                        mentorID = null,
                                        RecievedByMentor = 0.0,
                                        RecievedByUJB = 0.0
                                    },
                                    shareReceivedByUJB = new ShareReceivedByUJB
                                    {
                                        percOrAmount = ShareType,
                                        value = ShareRecievedUJB
                                    },
                                    refStatusUpdatedOn = RefDate1,
                                    Created = new Created(),
                                    Updated = new Updated(),
                                };

                                _leads.InsertOne(lead);
                                var LeadId = lead.Id.Trim();
                                Update_System_Default("ReferralCode");

                                if (dr["Deal Closed Date"].ToString().Trim() != "")
                                {
                                    var _leadstatus = new LeadsStatusHistory
                                    {
                                        leadId = LeadId,

                                        statusId = status,
                                        Updated = new Updated
                                        {
                                            updated_On = dealClosedDate1,

                                            updated_By = null
                                        },



                                    };

                                    _leadStatus.InsertOne(_leadstatus);
                                }

                                errormsg = referralCode +" inserted successfully";
                               
                            }
                            if (!string.IsNullOrEmpty(errormsg))
                            {
                                DataRow drNew = Errordt.NewRow();
                                drNew.ItemArray = dr.ItemArray;
                                drNew["errordetails"] = errormsg;
                                Errordt.Rows.Add(drNew);
                                dr["errordetails"] = errormsg;
                            }
                        }
                        catch (Exception ex)
                        {
                            errormsg = ex.Message;
                            if (!string.IsNullOrEmpty(ex.Message))
                            {
                                DataRow drNew = Errordt.NewRow();
                                drNew.ItemArray = dr.ItemArray;
                                drNew["errordetails"] = errormsg;
                                Errordt.Rows.Add(drNew);
                                dr["errordetails"] = errormsg;
                            }
                        }

                        //var followUp = new LeadFollowUp
                        //{
                        //    leadId = LeadId,
                        //    followUpDate1 = dr["Follow up 1 Date"].ToString() == "" ? DateTime.MinValue : DateTime.FromOADate(Double.Parse(dr["Follow up 1 Date"].ToString())),
                        //    followUpDate2 = dr["Follow up 2 Date"].ToString() == "" ? DateTime.MinValue : DateTime.FromOADate(Double.Parse(dr["Follow up 2 Date"].ToString())),
                        //    followUpDate3 = dr["Follow up 3 Date"].ToString() == "" ? DateTime.MinValue : DateTime.FromOADate(Double.Parse(dr["Follow up 3 Date"].ToString())),
                        //    followUpDetails = dr["Follow up 1"].ToString().Trim(),
                        //    followUpDetails2 = dr["Follow up 2"].ToString().Trim(),
                        //    followUpDetails3 = dr["Follow up 3"].ToString().Trim()
                        //};

                        //_followUpDetails.InsertOne(followUp);
                    }
                   // ds.Tables.Add(dt);
                   // ds.Tables.Add(Errordt);
                    //DataTable error = new DataTable();
                    //error = ds.Tables[1];
                    WriteExcelFile(Errordt, "Referral File uploaded status.xlsx");
                
                }

                if (FileType == "PartnerBusinessDetails")
                {
                    DataView userProductView = new DataView(dt);
                    DataTable distinctUJBProducts = userProductView.ToTable(true, "UJBCode", "Business Type", "Product Service Name", "Description", "Product URL", "Price", "Min. Deal Value", "SingleOrMultiple", "SlabOrProduct");
                    DataView distinctUserView = new DataView(distinctUJBProducts);
                    DataTable distinctUJBCodes = distinctUserView.ToTable(true, "UJBCode");

                    foreach (DataRow drUser in distinctUJBCodes.Rows)
                    {
                        string userId = GetUserId(drUser["UJBCode"].ToString().Trim());
                        var businessId = GetBusinessUserId(userId);

                        DataRow[] userProducts = distinctUJBProducts.Select("UJBCode = '" + drUser["UJBCode"].ToString() + "'").Distinct().ToArray();

                        foreach (DataRow drprd in userProducts)
                        {
                            int RowtypeOf = 0; int RowShareType = 0;
                            if (drprd["SingleOrMultiple"].ToString().Trim() == "Single")
                            {
                                RowtypeOf = 1;
                            }
                            else if (drprd["SingleOrMultiple"].ToString().Trim() == "Multiple")
                            {
                                RowtypeOf = 2;

                            }

                            if (drprd["SlabOrProduct"].ToString().Trim() == "Slab")
                            {
                                RowShareType = 1;
                            }
                            else if (drprd["SlabOrProduct"].ToString().Trim() == "Product")
                            {
                                RowShareType = 2;

                            }
                            var pdctsrc = new DbProductService
                            {
                                bussinessId = businessId,
                                type = drprd["Business Type"].ToString().Trim(),
                                name = drprd["Product Service Name"].ToString().Trim(),
                                url = drprd["Product URL"].ToString().Trim(),
                                minimumDealValue = double.Parse(drprd["Min. Deal Value"].ToString().Trim() == "" ? "0" : drprd["Min. Deal Value"].ToString()),
                                typeOf = RowtypeOf,
                                shareType = RowShareType,
                                description = drprd["Description"].ToString().Trim(),
                                isActive=true
                            };
                            _prdctSrvc.InsertOne(pdctsrc);
                            var pdcSerId = pdctsrc.Id;

                            DataRow[] userProductDetails = dt.Select("UJBCode = '" + drUser["UJBCode"].ToString() + "' AND [Product Service Name]='" + drprd["Product Service Name"].ToString().Trim() + "'").Distinct().ToArray();

                            if (userProductDetails != null && userProductDetails.Count() > 0)
                            {
                                foreach (DataRow drprddt in userProductDetails)
                                {
                                    int RowType = 0;
                                    if (drprddt["PercentOrRupee"].ToString() == "%")
                                    {
                                        RowType = 1;
                                    }
                                    else if (drprddt["PercentOrRupee"].ToString() == "Rupee")
                                    {
                                        RowType = 2;

                                    }
                                    var pdctsrcDetails = new ProductServiceDetails
                                    {
                                        prodservId = pdcSerId,
                                        productName = drprddt["ProductName"].ToString().Trim(),
                                        from = int.Parse(drprddt["From Range"].ToString() == "" ? "0" : drprddt["From Range"].ToString()),
                                        to = int.Parse(drprddt["To Range"].ToString() == "" ? "0" : drprddt["To Range"].ToString()),
                                        type = RowType,
                                        value = double.Parse(drprddt["PercentOrRupeeValue"].ToString().Trim()),
                                        isActive=true

                                    };
                                    _prdctSrvcDetais.InsertOne(pdctsrcDetails);
                                }
                            }



                        }
                    }

                }

                else if (FileType == "PartnerDetails")
                {
                    //foreach (DataRow dr in dt.Rows)
                    //{
                    //    if (string.IsNullOrEmpty(dr["errordetails"].ToString()))
                    //    {
                    //        string PanUniqueName = "";
                    //        string BankUniqueName = "";
                    //        string FrontUniqueName = "";
                    //        string BackUniqueName = "";
                    //        string BusPanUniqueName = "";
                    //        var address = new Address
                    //        {
                    //            location = dr["location"].ToString(),
                    //            locality = dr["locality"].ToString(),
                    //            flatWing = dr["flatWing"].ToString()
                    //        };
                    //        string res = Get_Coordinates_Address(address);
                    //        var latitude = 0.0;
                    //        var longitude = 0.0;
                    //        if (res!="NONE")
                    //        { 
                    //        new_latitude = res.Split(",")[0];
                    //        new_longitude = res.Split(",")[1];
                    //            latitude = double.Parse(new_latitude);
                    //            longitude = double.Parse(new_longitude);
                    //        }
                          
                            

                    //        var BusinessAddress = new Address
                    //        {
                    //            locality = dr["Locality"].ToString(),
                    //            location = dr["Location"].ToString(),
                    //            flatWing = dr["Flat_Wing"].ToString(),
                    //        };

                    //        string busres = Get_Coordinates_Address(BusinessAddress);
                    //        var busilatitude = 0.0;
                    //        var buslongitude = 0.0;
                    //        if (busres != "NONE")
                    //        {
                    //            new_latitude = res.Split(",")[0];
                    //            new_longitude = res.Split(",")[1];

                    //             busilatitude = double.Parse(new_latitude);
                    //             buslongitude = double.Parse(new_longitude);
                    //        }
                    //        if (!string.IsNullOrWhiteSpace(dr["PancardImageName"].ToString().Trim()))
                    //        {

                    //            PanUniqueName = saveFile(dr["PancardImageName"].ToString().Trim(), "PancardPath");
                    //        }
                    //        if (!string.IsNullOrWhiteSpace(dr["CanceledChequeImageName"].ToString().Trim()))
                    //        {
                    //            BankUniqueName = saveFile(dr["CanceledChequeImageName"].ToString().Trim(), "BankDetailsPath");
                    //        }
                    //        if (!string.IsNullOrWhiteSpace(dr["AdharCardFrontImageName"].ToString().Trim()))
                    //        {
                    //            FrontUniqueName = saveFile(dr["AdharCardFrontImageName"].ToString().Trim(), "AadharcardPath");
                    //        }
                    //        if (!string.IsNullOrWhiteSpace(dr["AdharCardBackImageName"].ToString().Trim()))
                    //        {
                    //            BackUniqueName = saveFile(dr["AdharCardBackImageName"].ToString().Trim(), "AadharcardPath");
                    //        }
                    //        if (!string.IsNullOrWhiteSpace(dr["CompanyPanImage"].ToString().Trim()))
                    //        {
                    //            BusPanUniqueName = saveFile(dr["CompanyPanImage"].ToString().Trim(), "BussinessPanPath");
                    //        }
                    //        double d = double.Parse(dr["birthDate"].ToString().Trim());

                    //        DateTime Birthdate = DateTime.FromOADate(d);
                    //        double d1 = double.Parse(dr["startDate"].ToString().Trim());

                    //        DateTime startDate = DateTime.FromOADate(d1);
                    //        string password = dr["firstName"].ToString().Trim().ToLower() + "@123";

                    //        var p = new User
                    //        {

                    //            firstName = dr["firstName"].ToString().Trim(),
                    //            lastName = dr["lastName"].ToString().Trim(),
                    //            emailId = dr["emailId"].ToString().Trim(),
                    //            mobileNumber = dr["mobileNumber"].ToString().Trim(),
                    //            countryCode = "+91",
                    //            password = SecurePasswordHasherHelper.Generate_HashPasssword(password),
                    //            language = null,
                    //            gender = dr["gender"].ToString().Trim(),
                    //            birthDate = Birthdate,// DateTime.Parse(dr["birthDate"].ToString().Trim()),
                    //            preferredLocations = new PreferredLocations
                    //            {
                    //                location1 = dr["location1"].ToString().Trim(),
                    //                location2 = dr["location2"].ToString().Trim(),
                    //                location3 = dr["location3"].ToString().Trim()
                    //            },
                    //            FileName = "",
                    //            UniqueName = "",
                    //            ImageURL = "",
                    //            // base64Image = null,
                    //            imageType = null,
                    //            knowledgeSource = dr["knowledgeSource"].ToString().Trim(),
                    //            passiveIncome = dr["passiveIncome"].ToString().Trim(),
                    //            //organisationType = dr["organisationType"].ToString(),
                    //            // userType =int.Parse(dr["userType"].ToString()),
                    //            address = address,
                    //            mentorCode = dr["mentorCode"].ToString().Trim(),
                    //            otpVerification = new OtpVerification
                    //            {
                    //                OTP_Verified = false,
                    //                OTP = null
                    //            },
                    //            myMentorCode = dr["UJBCode"].ToString().Trim(),
                    //            Role = dr["Role"].ToString().Trim(),
                    //            socialLogin = new SocialLogin
                    //            {
                    //                Is_Social_Login = false,
                    //                Social_Code = null,
                    //                Social_Site = null
                    //            },
                    //            Created = new Created
                    //            {
                    //                created_By = null,
                    //                created_On = startDate// DateTime.Parse(dr["startDate"].ToString().Trim()),

                    //            },
                    //            latitude = latitude,
                    //            longitude = longitude,
                    //            middleName = dr["middle Name"].ToString(),
                    //            countryId = 101,
                    //            stateId = 22,//_state.Find(x => x.stateName == dr["StateName"].ToString()).FirstOrDefault().stateId,
                    //           isActive = true

                    //        };
                    //        _userDetails.InsertOne(p);
                    //        string RecvdUserId = p._id;
                    //        if (RecvdUserId != null)
                    //        {
                    //            var q = new UserOtherDetails
                    //            {
                    //                UserId = RecvdUserId,
                    //                maritalStatus = dr["maritalStatus"].ToString(),
                    //                nationality = dr["nationality"].ToString(),
                    //                areaOfInterest = dr["AreaOfInterest"].ToString(),
                    //                phoneNo = dr["phoneNo"].ToString(),
                    //                Hobbies = dr["Hobbies"].ToString()
                    //            };
                    //            _userOtherDetails.InsertOne(q);

                    //            var k = new UserKYCDetails
                    //            {
                    //                UserId = RecvdUserId,
                    //                PanCard = new PanCard
                    //                {
                    //                    PanNumber = dr["PanCard"].ToString(),
                    //                    UniqueName = PanUniqueName,
                    //                    FileName = dr["PancardImageName"].ToString(),
                    //                    ImageURL = _iconfiguration["PancardURL"].ToString() + PanUniqueName,
                    //                },
                    //                AdharCard = new AdharCard
                    //                {
                    //                    AdharNumber = dr["AdharCard"].ToString(),
                    //                    FrontFileName = dr["AdharCardFrontImageName"].ToString(),
                    //                    FrontImageURL = _iconfiguration["AadharcardURL"].ToString() + FrontUniqueName,
                    //                    FrontUniqueName = FrontUniqueName,
                    //                    BackFileName = dr["AdharCardBackImageName"].ToString(),
                    //                    BackImageURL = _iconfiguration["AadharcardURL"].ToString() + BackUniqueName,
                    //                    BackUniqueName = BackUniqueName,
                    //                },
                    //                BankDetails = new BankDetails
                    //                {
                    //                    BankName = dr["BankName"].ToString(),
                    //                    AccountHolderName = dr["AccountHolderName"].ToString(),
                    //                    IFSCCode = dr["IFSCCode"].ToString(),
                    //                    UniqueName = BankUniqueName,
                    //                    ImageURL = _iconfiguration["BankDetailsURL"].ToString() + BankUniqueName,
                    //                    FileName = dr["CanceledChequeImageName"].ToString(),
                    //                    AccountNumber=dr["AccountNumber"].ToString()


                    //                },
                    //                IsApproved = new IsApproved
                    //                {
                    //                    Flag = true,
                    //                    ApprovedOn = startDate,//DateTime.Parse(dr["startDate"].ToString().Trim()),//DateTime.Now.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0)),
                    //                    ApprovedBy = "",
                    //                    Reason = "",
                    //                    ReasonId = 0,

                    //                }

                    //            };

                    //            _userKycDetails.InsertOne(k);
                    //            //SaveAgreementDetails(RecvdUserId, "Admin");

                    //            if (dr["Role"].ToString().Trim() == "Listed Partner")
                    //            {
                    //                List<string> category = new List<string>();
                    //                // string UserId = GetUserId(dr["firstName"].ToString().Trim(), dr["lastName"].ToString().Trim(), dr["myMentorCode"].ToString().Trim());
                    //                category.Add(GetCategoryId(dr["Category"].ToString().Trim()));
                    //                //   UserType UserType = (UserType)Enum.Parse(typeof(UserType), dr["AreYou"].ToString());
                    //                var Bsns = new BusinessDetails
                    //                {
                    //                    UserId = RecvdUserId,
                    //                    CompanyName = dr["CompanyName"].ToString().Trim(),
                    //                    Categories = category,

                    //                    BusinessDescription = dr["Description"].ToString().Trim(),
                    //                    WebsiteUrl = dr["CorporateURL"].ToString().Trim(),
                    //                    Tagline = dr["Tagline"].ToString().Trim(),
                    //                    BusinessEmail = dr["BusinessEmailId"].ToString(),
                    //                    UserType = (int)(UJBHelper.Common.Common.UserType)Enum.Parse(typeof(UJBHelper.Common.Common.UserType), dr["AreYou"].ToString().Trim()),
                    //                    NameOfPartner = dr["PartnerName"].ToString().Trim(),
                    //                    //(int)enum.UserType.dr["AreYou"]// 0,//Enum.GetUnderlyingType(enum.UserType(dr["AreYou"].ToString())),
                    //                    BusinessAddress = new BusinessAddress
                    //                    {
                    //                        Locality = dr["Locality"].ToString(),
                    //                        Location = dr["Location"].ToString(),
                    //                        Flat_Wing = dr["Flat_Wing"].ToString(),
                    //                    },

                    //                    latitude = busilatitude,
                    //                    longitude = busilatitude,
                    //                    averageRating = 3.0,
                    //                    GSTNumber = dr["GSTIN No."].ToString().Trim(),
                    //                    BusinessPan = new BusinessPan
                    //                    {
                    //                        PanNumber = dr["CompanyPanCard"].ToString().Trim(),
                    //                        UniqueName = BusPanUniqueName,
                    //                        FileName = dr["CompanyPanImage"].ToString(),
                    //                        ImageURL = _iconfiguration["BussinessPanURL"].ToString() + BusPanUniqueName,
                    //                    },
                    //                    isApproved = new Approved
                    //                    {
                    //                        ApprovedBy = "",
                    //                        ApprovedOn = DateTime.Now.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0)),
                    //                        Flag = 1,
                    //                        Reason = ""
                    //                    },
                    //                    isSubscriptionActive = true
                    //                };
                    //                _businessDetails.InsertOne(Bsns);
                    //                // BusSaveAgreementDetails(Bsns.Id, "Admin");

                    //            }
                    //        }

                    //    }
                    //}

                    ////Save Agreements
                    foreach (DataRow dr in dt.Rows)
                    {
                        string userId = GetUserId(dr["UJBCode"].ToString().Trim());
                        SaveAgreementDetails(userId, "Admin");

                        if (dr["Role"].ToString().Trim() == "Listed Partner")
                        {
                            string businessUserId = GetBusinessUserId(userId);
                            BusSaveAgreementDetails(businessUserId, "Admin");
                        }
                    }
                }
            }
            catch (Exception err)
            {
                session.AbortTransaction();
            }

            session.CommitTransaction();
            session.Dispose();

        }

        public string GenerateReferralCode()
        {
            TimeSpan diff = DateTime.Now - DateTime.UtcNow;
            string[] timezone = DateTime.Now.ToString("zzz").Split(new char[] { '+', '-', ':' });

            int ReferralNoLength = 0;
            int CurrYear = 0;
            int CurrentYear = 0;
            var NextYear = 0;
            var ReferralNoCounter = "";
            var ReferralCode = "";

            DateTime CurrentDate = DateTime.Today.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0));
            //CurrentDate = new DateTime(CurrentDate.Year, CurrentDate.Month, CurrentDate.Day);

            var filter = Builders<System_Default>.Filter.Lte(x => x.startDate, CurrentDate);
            filter = filter & Builders<System_Default>.Filter.Gte(x => x.endDate, CurrentDate);
            filter = filter & Builders<System_Default>.Filter.Eq(x => x.Default_Name, "ReferralCode");

            CurrentYear = DateTime.Now.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0)).Year;
            DateTime dt = _default.Find(filter).FirstOrDefault().startDate;
            CurrYear = dt.Year;
            NextYear = CurrYear + 1;
            ReferralNoCounter = _default.Find(filter).FirstOrDefault().Default_Value;
            ReferralNoLength = ReferralNoCounter.Length;
            ReferralCode = "Ref/" + CurrYear.ToString().Substring(CurrYear.ToString().Length - 2) + "-" + NextYear.ToString().Substring(NextYear.ToString().Length - 2) + "/";

            if (ReferralNoLength == 1)
            {
                ReferralCode = ReferralCode + "0000000" + int.Parse(ReferralNoCounter);
            }
            else if (ReferralNoLength == 2)
            {
                ReferralCode = ReferralCode + "000000" + int.Parse(ReferralNoCounter);
            }
            else if (ReferralNoLength == 3)
            {
                ReferralCode = ReferralCode + "00000" + int.Parse(ReferralNoCounter);
            }
            else if (ReferralNoLength == 4)
            {
                ReferralCode = ReferralCode + "0000" + int.Parse(ReferralNoCounter);
            }
            else if (ReferralNoLength == 5)
            {
                ReferralCode = ReferralCode + "000" + int.Parse(ReferralNoCounter);
            }
            else if (ReferralNoLength == 6)
            {
                ReferralCode = ReferralCode + "00" + int.Parse(ReferralNoCounter);
            }
            else if (ReferralNoLength == 7)
            {
                ReferralCode = ReferralCode + "0" + int.Parse(ReferralNoCounter);
            }
            else
            {
                ReferralCode = ReferralCode + int.Parse(ReferralNoCounter);
            }
            return ReferralCode;
        }

       
        public void Update_System_Default(string Default_Name)
        {

            TimeSpan diff = DateTime.Now - DateTime.UtcNow;
            string[] timezone = DateTime.Now.ToString("zzz").Split(new char[] { '+', '-', ':' });

            DateTime CurrentDate = DateTime.Today.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0));
            // CurrentDate = new DateTime(CurrentDate.Year, CurrentDate.Month, CurrentDate.Day);

            var filter = Builders<System_Default>.Filter.Lte(x => x.startDate, CurrentDate);
            filter = filter & Builders<System_Default>.Filter.Gte(x => x.endDate, CurrentDate);
            filter = filter & Builders<System_Default>.Filter.Eq(x => x.Default_Name, Default_Name);

            int value = int.Parse(_default.Find(filter).FirstOrDefault().Default_Value);
            value = value + 1;
            _default.FindOneAndUpdate(
                Builders<System_Default>.Filter.Eq(x => x.Default_Name, Default_Name),
               Builders<System_Default>.Update
               .Set(x => x.Default_Value, value.ToString())
               );
        }

        public string Get_Coordinates_Address(Address addressInfo)
        {
            var location = $"{addressInfo.flatWing.ToLower().Replace(" ", "+")}+{addressInfo.location.ToLower().Replace(" ", "+")}+{addressInfo.locality.ToLower().Replace(" ", "+")}";
            var google_Api_key = Otp_Generator.Get_Google_Api_Key();
            string url = $"https://maps.googleapis.com/maps/api/geocode/json?address={location}&key={google_Api_key}";

            WebRequest request = WebRequest.Create(url);

            WebResponse response = request.GetResponse();

            Stream data = response.GetResponseStream();

            StreamReader reader = new StreamReader(data);

            // json-formatted string from maps api
            string responseFromServer = reader.ReadToEnd();


            var google_response = new Google_Location_To_Coordinates_Response();

            google_response = JsonConvert.DeserializeObject<Google_Location_To_Coordinates_Response>(responseFromServer);

            response.Close();

            if (google_response.status == "OK")
            {
                return google_response.results[0].geometry.location.lat + "," + google_response.results[0].geometry.location.lng;
            }
            return "NONE";

        }
        public void SaveAgreementDetails(string UserId, string UpdatedBy)
        {
            string FileURL = GeneratePDF(UserId);
            TimeSpan diff = DateTime.Now - DateTime.UtcNow;
            string[] timezone = DateTime.Now.ToString("zzz").Split(new char[] { '+', '-', ':' });

            var p = new AgreementDetails
            {
                UserId = UserId,
                BusinessId = null,
                type = "Partner Agreement",
                Version = 1.0,
                PdfURL = FileURL,
                created = new Created
                {
                    created_By = UpdatedBy,
                    created_On = DateTime.Now.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0))
                },
                accepted = new Accepted()
            };
            _agreementDetails.InsertOne(p);

        }

        public string GeneratePDF(string UserId)
        {
            // Get UserDetails
            User userdetails = _userDetails.Find(x => x._id == UserId).FirstOrDefault();
            string MentorName = _userDetails.Find(x => x.myMentorCode == userdetails.mentorCode).FirstOrDefault().firstName + " " + _userDetails.Find(x => x.myMentorCode == userdetails.mentorCode).FirstOrDefault().lastName;
            string HTMlString = "";
            HTMlString =  _agreement.Find(x => x.Type == "Partner Agreement").FirstOrDefault().AgreementContent;
            HTMlString = HTMlString.Replace("@UserName", userdetails.firstName + " " + userdetails.lastName);
            HTMlString = HTMlString.Replace("@MentorName", MentorName);
            HTMlString = HTMlString.Replace("@Address", userdetails.address.flatWing + " , " + userdetails.address.location);

            String FileURL = "";
            string FileDestination = System.IO.Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory());
            FileDestination = FileDestination + _iconfiguration["PartnerAgreementImagePath"];
            FileURL = _iconfiguration["PartnerAgreementImageURL"];
            Byte[] bytes = _generatePdf.GetPDF(HTMlString);
            string fileUniqueName = Utility.UploadFilebytes(bytes, "PartnerAgreement.pdf", FileDestination);
            FileURL = FileURL + fileUniqueName;
            return FileURL;
        }

        public string saveFile(string fileName, string imgtype)
        {
            var uniqueName = "";

            string sourcePath = @"F:\Sheetal\UJB\web-app\UJBApiGateway\Content\sourcePath"; //+ _iconfiguration["SourcePath"];
            string targetPath = @"F:\Sheetal\UJB\web-app" +_iconfiguration[imgtype];
            var extn = Path.GetFileName(fileName.Substring(fileName.LastIndexOf('.') + 1));
            uniqueName = string.Concat(DateTime.Now.ToString("ddMMyyyyHHmmssffff") + '.' + extn);
            // Use Path class to manipulate file and directory paths.
            string sourceFile = System.IO.Path.Combine(sourcePath, fileName);
            string destFile = System.IO.Path.Combine(targetPath, uniqueName);

            // To copy a folder's contents to a new location:
            // Create a new target folder. 
            // If the directory already exists, this method does not create a new directory.
            System.IO.Directory.CreateDirectory(targetPath);

            // To copy a file to another location and 
            // overwrite the destination file if it already exists.



            if (System.IO.Directory.Exists(sourcePath))
            {
                System.IO.File.Copy(sourceFile, destFile, true);
            }
            else
            {
                Console.WriteLine("Source path does not exist!");
            }

            return uniqueName;
        }


        public string BusGeneratePDF(BusinessDetails bsns)
        {
            // Get UserDetails
            User userdetails = _userDetails.Find(x => x._id == bsns.UserId).FirstOrDefault();

            string HTMlString = "";


            if (bsns.UserType == (int)UJBHelper.Common.Common.UserType.Individual_Proprietor)
            {
                HTMlString = _agreement.Find(x => x.Type == "Freelancer Listed Partner Agreement").FirstOrDefault().AgreementContent;
                HTMlString = HTMlString.Replace("@UserName", userdetails.firstName + " " + userdetails.lastName);
                HTMlString = HTMlString.Replace("@BusinessAddress", bsns.BusinessAddress.Flat_Wing + ", " + bsns.BusinessAddress.Location);
            }

            else if (bsns.UserType == (int)UJBHelper.Common.Common.UserType.LLP)
            {
                HTMlString = _agreement.Find(x => x.Type == "LLP Listed Partner Agreement").FirstOrDefault().AgreementContent;
                HTMlString = HTMlString.Replace("@CompanyName", bsns.CompanyName);
                HTMlString = HTMlString.Replace("@NameofPartner", bsns.NameOfPartner);
                HTMlString = HTMlString.Replace("@BusinessAddress", bsns.BusinessAddress.Flat_Wing + ", " + bsns.BusinessAddress.Location);
            }
            else if (bsns.UserType == (int)UJBHelper.Common.Common.UserType.PartnerShipFirm)
            {
                HTMlString = _agreement.Find(x => x.Type == "PartnerShipFirm Listed Partner Agreement").FirstOrDefault().AgreementContent;
                HTMlString = HTMlString.Replace("@CompanyName", bsns.CompanyName);
                HTMlString = HTMlString.Replace("@NameofPartner", bsns.NameOfPartner);
                HTMlString = HTMlString.Replace("@BusinessAddress", bsns.BusinessAddress.Flat_Wing + ", " + bsns.BusinessAddress.Location);
            }
            else if (bsns.UserType == (int)UJBHelper.Common.Common.UserType.Company)
            {
                HTMlString = _agreement.Find(x => x.Type == "Company Listed Partner Agreement").FirstOrDefault().AgreementContent;
                HTMlString = HTMlString.Replace("@CompanyName", bsns.CompanyName);
                HTMlString = HTMlString.Replace("@BusinessAddress", bsns.BusinessAddress.Flat_Wing + ", " + bsns.BusinessAddress.Location);
            }

            String FileURL = "";
            string FileDestination = System.IO.Path.GetDirectoryName(System.IO.Directory.GetCurrentDirectory());
            FileDestination = FileDestination + _iconfiguration["ListedPartnerAgreementImagePath"];
            FileURL = _iconfiguration["ListedPartnerAgreementImageURL"];
            Byte[] bytes = _generatePdf.GetPDF(HTMlString);
            string fileUniqueName = Utility.UploadFilebytes(bytes, "ListedPartnerAgreement.pdf", FileDestination);
            FileURL = FileURL + fileUniqueName;
            return FileURL;
        }

        public void BusSaveAgreementDetails(string BusinessId, string UpdatedBy)
        {

            BusinessDetails bsns = _businessDetails.Find(x => x.Id == BusinessId).FirstOrDefault();

            string FileURL = BusGeneratePDF(bsns);

            TimeSpan diff = DateTime.Now - DateTime.UtcNow;
            string[] timezone = DateTime.Now.ToString("zzz").Split(new char[] { '+', '-', ':' });

            var p = new AgreementDetails
            {
                UserId = bsns.UserId,
                BusinessId = bsns.Id,
                type = "Listed Partner Agreement",
                Version = 1.0,
                PdfURL = FileURL,
                created = new Created
                {
                    created_By = UpdatedBy,
                    created_On = DateTime.Now.Add(new TimeSpan(Convert.ToInt32(timezone[1]), Convert.ToInt32(timezone[2]), 0))
                },
                accepted = new Accepted()
            };
            _agreementDetails.InsertOne(p);
        }

        private bool IsNumeric(string s)
        {
            Regex r = new Regex(@"^\d+\.?\d*$");
            return r.IsMatch(s);
        }
        static void WriteExcelFile(DataTable dt, string FileName)
        {
            try
            {
                // List<userdetails> persons = new List<userdetails>()
                // {
                //     new UserDetails() {ID="1001", Name="ABCD", City ="City1", Country="USA"},
                //     new UserDetails() {ID="1002", Name="PQRS", City ="City2", Country="INDIA"},
                //     new UserDetails() {ID="1003", Name="XYZZ", City ="City3", Country="CHINA"},
                //     new UserDetails() {ID="1004", Name="LMNO", City ="City4", Country="UK"},
                //};

                // // Lets converts our object data to Datatable for a simplified logic.
                // // Datatable is most easy way to deal with complex datatypes for easy reading and formatting. 
                // DataTable table = (DataTable)JsonConvert.DeserializeObject(JsonConvert.SerializeObject(persons), (typeof(DataTable)));

                using (SpreadsheetDocument document = SpreadsheetDocument.Create(FileName, SpreadsheetDocumentType.Workbook))
                {
                    WorkbookPart workbookPart = document.AddWorkbookPart();
                    workbookPart.Workbook = new Workbook();

                    WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                    var sheetData = new SheetData();
                    worksheetPart.Worksheet = new Worksheet(sheetData);

                    Sheets sheets = workbookPart.Workbook.AppendChild(new Sheets());
                    Sheet sheet = new Sheet() { Id = workbookPart.GetIdOfPart(worksheetPart), SheetId = 1, Name = "Sheet1" };

                    sheets.Append(sheet);

                    Row headerRow = new Row();

                    List<string> columns = new List<string>();
                    foreach (System.Data.DataColumn column in dt.Columns)
                    {
                        columns.Add(column.ColumnName);

                        Cell cell = new Cell();
                        cell.DataType = CellValues.String;
                        cell.CellValue = new CellValue(column.ColumnName);
                        headerRow.AppendChild(cell);
                    }

                    sheetData.AppendChild(headerRow);

                    foreach (DataRow dsrow in dt.Rows)
                    {
                        Row newRow = new Row();
                        foreach (String col in columns)
                        {
                            Cell cell = new Cell();
                            cell.DataType = CellValues.String;
                            cell.CellValue = new CellValue(dsrow[col].ToString());
                            newRow.AppendChild(cell);
                        }

                        sheetData.AppendChild(newRow);
                    }

                    workbookPart.Workbook.Save();
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(Assembly.GetCallingAssembly().GetName().Name + "\n\t" + ex.ToString());
            }
        }

    }

}
//public async Task Bulk_Partner_Upload(DataTable dt)
//{
//    var client = new MongoClient(DbHelper.GetConnectionString());
//    var database = client.GetDatabase(DbHelper.GetDatabaseName());
//    var collection = database.GetCollection<BsonDocument>("Users");

//    List<BsonDocument> batch = new List<BsonDocument>();
//    foreach (DataRow dr in dt.Rows)
//    {
//        var dictionary = dr.Table.Columns.Cast<DataColumn>().ToDictionary(col => col.ColumnName, col => dr[col.ColumnName]);
//        batch.Add(new BsonDocument(dictionary));
//    }

//    await collection.InsertManyAsync(batch.AsEnumerable());
//}

