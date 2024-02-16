using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using ExcelUpload.Service.Model.PartnerUpload;
using ExcelUpload.Service.Repositories.PartnerUpload;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using UJBHelper.Common;


namespace ExcelUpload.Service.Manager.PartnerUpload
{
    public class Upload : IDisposable
    {
        private Post_Request request;
        private IUploadPartnerDetails uploadPartner;
        public HttpStatusCode _statusCode = HttpStatusCode.OK;
        public List<Message_Info> _messages = null;
        public string JSONValue = null;
        public string fileLocation;
      
        public Upload(Post_Request request, IUploadPartnerDetails _uploadPartnerDetails)
        {
            this.request = request;
            uploadPartner = _uploadPartnerDetails;
            _messages = new List<Message_Info>();
            
        }
        public void Process()
        {
            if (request.FileType=="PartnerDetails")
            {
                fileLocation = @"C:\Users\ais\Desktop\UJB\" + request.FileName;
            }
            else if (request.FileType == "PartnerBusinessDetails")
            {
                fileLocation = @"C:\Users\ais\Desktop\UJB\" + request.FileName;
            }
            else if (request.FileType == "ReferralDetails")
            {
                fileLocation = @"C:\Users\ais\Desktop\UJB_Excels\" + request.FileName;
            }

            ReadExcelasJSON(fileLocation);
        }

        public  string ReadExcelasJSON(string FileLocation)
        {
            DataSet ds = new DataSet();
            try
            {
                DataTable dtTable = new DataTable();   
                using (SpreadsheetDocument doc = SpreadsheetDocument.Open(fileLocation, false))
                {
                    
                    WorkbookPart workbookPart = doc.WorkbookPart;
                    Sheets thesheetcollection = workbookPart.Workbook.GetFirstChild<Sheets>();                   
                    foreach (Sheet thesheet in thesheetcollection.OfType<Sheet>())
                    {                        
                        Worksheet theWorksheet = ((WorksheetPart)workbookPart.GetPartById(thesheet.Id)).Worksheet;
                        SheetData thesheetdata = theWorksheet.GetFirstChild<SheetData>();
                        for (int rCnt = 0; rCnt < thesheetdata.ChildElements.Count(); rCnt++)
                        {
                            List<string> rowList = new List<string>();
                            for (int rCnt1 = 0; rCnt1 < thesheetdata.ElementAt(rCnt).ChildElements.Count(); rCnt1++)
                            {
                                Cell thecurrentcell = (Cell)thesheetdata.ElementAt(rCnt).ChildElements.ElementAt(rCnt1);                               
                                string currentcellvalue = string.Empty;
                                if (thecurrentcell.DataType != null)
                                {
                                    if (thecurrentcell.DataType == CellValues.SharedString)
                                    {
                                        int id;
                                        if (Int32.TryParse(thecurrentcell.InnerText, out id))
                                        {
                                            SharedStringItem item = workbookPart.SharedStringTablePart.SharedStringTable.Elements<SharedStringItem>().ElementAt(id);
                                            if (item.Text != null)
                                            {
                                                if (rCnt == 0)
                                                {
                                                    dtTable.Columns.Add(item.Text.Text);
                                                }
                                                else
                                                {
                                                    rowList.Add(item.Text.Text);
                                                }
                                            }
                                            else if (item.InnerText != null)
                                            {
                                                currentcellvalue = item.InnerText;
                                            }
                                            else if (item.InnerXml != null)
                                            {
                                                currentcellvalue = item.InnerXml;
                                            }
                                        }



                                    }
                                    else
                                    {
                                        if (rCnt != 0)
                                        {
                                            rowList.Add(thecurrentcell.InnerText);
                                        }
                                    }


                                }
                                else
                                {
                                    if (rCnt != 0)
                                    {
                                        rowList.Add(thecurrentcell.InnerText);
                                    }
                                }
                            }

                            if (rCnt != 0)
                                dtTable.Rows.Add(rowList.ToArray());
                        }
                    }

                    if (dtTable.Rows.Count > 0)
                    {
                        DataTable error = new DataTable();
                        string ErrorFileName = "";
                        if (request.FileType == "PartnerDetails")
                        {
                            ds = Validate_Partner_Details(dtTable);
                            ErrorFileName = "Partner Upload Error File.xlsx";
                        }
                        else if (request.FileType == "PartnerBusinessDetails")
                        {
                            ds = Validate_Partner_Business_Details(dtTable);
                            ErrorFileName = "Partner Business Upload Error File.xlsx";
                        }
                        else if (request.FileType == "ReferralDetails")
                        {
                            ds = Validate_Referral_Details(dtTable);
                            ErrorFileName = " Referral Upload Error File.xlsx";
                        }
                        error = ds.Tables[1];
                        WriteExcelFile(error, ErrorFileName);
                    }
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        uploadPartner.Bulk_Partner_Upload(ds.Tables[0], request.FileType);
                        _messages.Add(new Message_Info
                        {
                            Message = "File Uploaded Sucessfully.. Please Check the download file for error",
                            Type = Message_Type.SUCCESS.ToString()
                        });

                        _statusCode = HttpStatusCode.OK;
                    }

                    JSONValue = JsonConvert.SerializeObject(dtTable);
                    return JSONValue;
                   
                }
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

        //public static string GetCellValue(SpreadsheetDocument document, Cell cell)
        //{
        //    string value = string.Empty;
        //    SharedStringTablePart stringTablePart = document.WorkbookPart.SharedStringTablePart;
        //    if (cell.CellValue != null)
        //    {
        //        value = cell.CellValue.InnerXml;
        //    }

        //    if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString)
        //    {
        //        return stringTablePart.SharedStringTable.ChildElements[Int32.Parse(value)].InnerText;
        //    }
        //    else
        //    {
        //        return value;
        //    }
        //}

        private DataSet Validate_Referral_Details(DataTable exceldt)
        {
            DataSet ds = new DataSet();
            try { 
                //DataSet ds = new DataSet();
                DataTable Errordt = new DataTable();
                Errordt = exceldt.Clone();
                //Errordt.Columns.Add("errordetails", typeof(string));
                //exceldt.Columns.Add("errordetails", typeof(string));

                string errormsg = string.Empty;
                string customError = string.Empty;

                if (exceldt != null && exceldt.Rows.Count > 0)
                {
                    foreach (DataRow dr in exceldt.Rows)
                    {
                        try
                        {
                            errormsg = "";
                            //if (string.IsNullOrWhiteSpace(dr["Referral Date"].ToString().Trim()))
                            //{
                            //    errormsg = errormsg + " Referral Date" + ",";
                            //}
                            //if (string.IsNullOrWhiteSpace(dr["Referred By Code"].ToString().Trim()))
                            //{
                            //    errormsg = errormsg + " Referred By Code" + ",";
                            //}
                            //if (string.IsNullOrWhiteSpace(dr["Referred By"].ToString().Trim()))
                            //{
                            //    errormsg = errormsg + " Referred By" + ",";
                            //}
                            //if (string.IsNullOrWhiteSpace(dr["Referred To Code"].ToString().Trim()))
                            //{
                            //    errormsg = errormsg + " Referred To Code" + ",";
                            //}
                            //if (string.IsNullOrWhiteSpace(dr["Referred To"].ToString().Trim()))
                            //{
                            //    errormsg = errormsg + " Referred To" + ",";
                            //}
                            //if (string.IsNullOrWhiteSpace(dr["Self Or Other"].ToString().Trim()))
                            //{
                            //    errormsg = errormsg + " Self Or Other" + ",";
                            //}
                            //if (string.IsNullOrWhiteSpace(dr["Name of Referral"].ToString().Trim()))
                            //{
                            //    errormsg = errormsg + " Name of Referral" + ",";
                            //}
                            //if (string.IsNullOrWhiteSpace(dr["Referred Product OR Services"].ToString().Trim()))
                            //{
                            //    errormsg = errormsg + " Referred Product OR Services" + ",";
                            //}
                            //if (dr["Self Or Other"].ToString().Trim() == "Other" & string.IsNullOrWhiteSpace(dr["Mobile No of the referral"].ToString().Trim()))
                            //{
                            //    errormsg = errormsg + " Mobile No of the referral" + ",";
                            //}
                            //if (string.IsNullOrWhiteSpace(dr["Status"].ToString().Trim()))
                            //{
                            //    errormsg = errormsg + " Status" + ",";
                            //}

                            //if (!string.IsNullOrEmpty(errormsg))
                            //{
                            //    errormsg = errormsg.TrimEnd(',');
                            //    errormsg = errormsg + " cannot be null";
                            //}

                            //if (!string.IsNullOrEmpty(dr["Referred By Code"].ToString()))
                            //{
                            //    if (!uploadPartner.Check_If_User_IsExist(dr["Referred By Code"].ToString().Trim()))
                            //    {
                            //        if (errormsg != null)
                            //        {
                            //            errormsg = errormsg + " , User does not exist. Please Check Details";
                            //        }
                            //        else
                            //        {
                            //            errormsg = errormsg + " User does not exist. Please Check Details";
                            //        }
                            //    }
                            //}
                            //if (!string.IsNullOrEmpty(dr["Referred To Code"].ToString()))
                            //{

                            //    if (!uploadPartner.Check_If_User_IsExist(dr["Referred To Code"].ToString().Trim()))
                            //    {
                            //        if (errormsg != null)
                            //        {
                            //            errormsg = errormsg + " , User does not exist. Please Check Details";
                            //        }
                            //        else
                            //        {
                            //            errormsg = errormsg + " User does not exist. Please Check Details";
                            //        }
                            //    }
                            //}

                            //if (!string.IsNullOrEmpty(dr["Deal Value"].ToString().Trim()))
                            //{
                            //    if (!(IsNumeric(dr["Deal Value"].ToString().Trim())))
                            //    {
                            //        if (errormsg != null)
                            //        {
                            //            errormsg = errormsg + " , Deal Value is incorrect.";
                            //        }
                            //        else
                            //        {
                            //            errormsg = errormsg + " Deal Value is incorrect." + ",";
                            //        }
                            //    }
                            //}
                            //if (!string.IsNullOrEmpty(errormsg))
                            //{
                            //    DataRow drNew = Errordt.NewRow();
                            //    drNew.ItemArray = dr.ItemArray;
                            //    drNew["errordetails"] = errormsg;
                            //    Errordt.Rows.Add(drNew);
                            //    dr["errordetails"] = errormsg;
                            //}
                        }
                        catch (Exception ex)
                        {
                            throw ex;
                        }
                    }
                }
                ds.Tables.Add(exceldt);
                ds.Tables.Add(Errordt);

            }
            catch (Exception ex)
            {
                Logger.Log.Error(Assembly.GetCallingAssembly().GetName().Name + "\n\t" + ex.ToString());
            }
            return ds;
        }

        private DataSet Validate_Partner_Business_Details(DataTable exceldt)
        {
            DataSet ds = new DataSet();
            try
            {
                
                DataTable Errordt = new DataTable();
                Errordt = exceldt.Clone();
                Errordt.Columns.Add("errordetails", typeof(string));
                exceldt.Columns.Add("errordetails", typeof(string));

                string errormsg = string.Empty;
                string customError = string.Empty;

                if (exceldt != null && exceldt.Rows.Count > 0)
                {
                    foreach (DataRow dr in exceldt.Rows)
                    {
                        try
                        {
                            errormsg = "";
                            //if (string.IsNullOrWhiteSpace(dr["firstName"].ToString().Trim()))
                            //{
                            //    errormsg = errormsg + " FirstName" + ",";
                            //}
                            //if (string.IsNullOrWhiteSpace(dr["lastname"].ToString().Trim()))
                            //{
                            //    errormsg = errormsg + " LastName" + ",";
                            //}
                            //if (string.IsNullOrWhiteSpace(dr["UJBCode"].ToString().Trim()))
                            //{
                            //    errormsg = errormsg + " UJBCode" + ",";
                            //}
                            //if (string.IsNullOrWhiteSpace(dr["Category"].ToString().Trim()))
                            //{
                            //    errormsg = errormsg + " Category" + ",";
                            //}
                            //if (string.IsNullOrWhiteSpace(dr["Business Type"].ToString().Trim()))
                            //{
                            //    errormsg = errormsg + " Business Type" + ",";
                            //}
                            //if (string.IsNullOrWhiteSpace(dr["Product Service Name"].ToString().Trim()))
                            //{
                            //    errormsg = errormsg + " Product Service Name" + ",";
                            //}
                            //if (string.IsNullOrWhiteSpace(dr["SingleOrMultiple"].ToString().Trim()))
                            //{
                            //    errormsg = errormsg + " SingleOrMultiple" + ",";
                            //}
                            //else
                            //{
                            //    if(dr["SingleOrMultiple"].ToString().Trim() == "Multiple" & string.IsNullOrEmpty(dr["SlabOrProduct"].ToString().Trim()))
                            //    {
                            //        errormsg = errormsg + " SlabOrProduct" + ",";
                            //    }
                            //}
                            //if (string.IsNullOrWhiteSpace(dr["PercentOrRupeeValue"].ToString().Trim()))
                            //{
                            //    errormsg = errormsg + " PercentOrRupeeValue" + ",";
                            //}
                            //if (string.IsNullOrWhiteSpace(dr["PercentOrRupee"].ToString().Trim()))
                            //{
                            //    errormsg = errormsg + " PercentOrRupee" + ",";
                            //}

                            //if(dr["SlabOrProduct"].ToString().Trim() == "Product" && string.IsNullOrEmpty(dr["ProductName"].ToString().Trim()))
                            //{
                            //    errormsg = errormsg + " ProductName" + ",";
                            //}

                            //if(dr["SlabOrProduct"].ToString().Trim() == "Slab" && (string.IsNullOrEmpty(dr["From Range"].ToString().Trim()) || string.IsNullOrEmpty(dr["To Range"].ToString().Trim())))
                            //{
                            //    errormsg = errormsg + " From and To Range" + ",";
                            //}


                            //if (!string.IsNullOrEmpty(errormsg))
                            //{
                            //    errormsg = errormsg.TrimEnd(',');
                            //    errormsg = errormsg + " cannot be null";
                            //}

                            //if (string.IsNullOrEmpty(errormsg))
                            //{
                            //    if (!uploadPartner.Check_If_User_IsExist(dr["firstName"].ToString().Trim(), dr["lastName"].ToString().Trim(), dr["myMentorCode"].ToString().Trim()))
                            //    {
                            //        if (errormsg != null)
                            //        {
                            //            errormsg = errormsg + " , User does not exist. Please Check Details";
                            //        }
                            //        else
                            //        {
                            //            errormsg = errormsg + " User does not exist. Please Check Details";
                            //        }
                            //    }
                            //}

                            //if (!string.IsNullOrEmpty(dr["PercentOrRupeeValue"].ToString().Trim()))
                            //{
                            //    if (!(IsNumeric(dr["PercentOrRupeeValue"].ToString().Trim())))
                            //    {
                            //        if (errormsg != null)
                            //        {
                            //            errormsg = errormsg + " , PercentOrRupee value is incorrect.";
                            //        }
                            //        else
                            //        {
                            //            errormsg = errormsg + " PercentOrRupee value is incorrect." + ",";
                            //        }
                            //    }
                            //}

                            //if (!string.IsNullOrEmpty(dr["Min. Deal Value (If Any)"].ToString().Trim()))
                            //{
                            //    if (!(IsNumeric(dr["Min. Deal Value (If Any)"].ToString().Trim())))
                            //    {
                            //        if (errormsg != null)
                            //        {
                            //            errormsg = errormsg + " , Min. Deal Value (If Any) value is incorrect.";
                            //        }
                            //        else
                            //        {
                            //            errormsg = errormsg + " Min. Deal Value (If Any) value is incorrect." + ",";
                            //        }
                            //    }
                            //}
                            //if (!string.IsNullOrEmpty(dr["From Range"].ToString().Trim()))
                            //{

                            //    if (!(IsNumeric(dr["From Range"].ToString().Trim())))
                            //    {
                            //        if (errormsg != null)
                            //        {
                            //            errormsg = errormsg + " , From Range value is incorrect.";
                            //        }
                            //        else
                            //        {
                            //            errormsg = errormsg + " From Range value is incorrect." + ",";
                            //        }
                            //    }
                            //}
                            //if (!string.IsNullOrEmpty(dr["To Range"].ToString().Trim()))
                            //{
                            //    if (!(IsNumeric(dr["To Range"].ToString().Trim())))
                            //    {
                            //        if (errormsg != null)
                            //        {
                            //            errormsg = errormsg + " , To Range value is incorrect.";
                            //        }
                            //        else
                            //        {
                            //            errormsg = errormsg + " To Range value is incorrect." + ",";
                            //        }
                            //    }
                            //}

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
                            throw ex;
                        }
                    }
                }
                ds.Tables.Add(exceldt);
                ds.Tables.Add(Errordt);
               
            }
            catch (Exception ex)
            {
                Logger.Log.Error(Assembly.GetCallingAssembly().GetName().Name + "\n\t" + ex.ToString());
            }
            return ds;
        }

        private DataSet Validate_Partner_Details(DataTable exceldt)
        {
            DataSet ds = new DataSet();
            try
            {
                int i = 1;
               
                DataTable Errordt = new DataTable();
                Errordt = exceldt.Clone();
                Errordt.Columns.Add("errordetails", typeof(string));
                exceldt.Columns.Add("errordetails", typeof(string));

                string errormsg = string.Empty;
                if (exceldt != null && exceldt.Rows.Count > 0)
                {
                    foreach (DataRow dr in exceldt.Rows)
                    {
                        try
                        {
                            errormsg = "";
                            //if (string.IsNullOrWhiteSpace(dr["firstName"].ToString()))
                            //{
                            //    errormsg = errormsg + " FirstName" + ",";
                            //}
                            //if (string.IsNullOrWhiteSpace(dr["lastname"].ToString()))
                            //{
                            //    errormsg = errormsg + " LastName" + ",";
                            //}
                            //if (string.IsNullOrWhiteSpace(dr["Role"].ToString()))
                            //{
                            //    errormsg = errormsg + " Role" + ",";
                            //}
                            //if (string.IsNullOrWhiteSpace(dr["mentorCode"].ToString()))
                            //{
                            //    errormsg = errormsg + " MentorCode" + ",";
                            //}
                            //if (string.IsNullOrWhiteSpace(dr["startDate"].ToString()))
                            //{
                            //    errormsg = errormsg + " Start Date" + ",";
                            //}
                            //if (string.IsNullOrWhiteSpace(dr["PanCard"].ToString()))
                            //{
                            //    errormsg = errormsg + " Pan Card" + ",";
                            //}
                            //if (string.IsNullOrWhiteSpace(dr["AdharCard"].ToString()))
                            //{
                            //    errormsg = errormsg + " Aadhar Card" + ",";
                            //}
                            //if (string.IsNullOrWhiteSpace(dr["emailId"].ToString()))
                            //{
                            //    errormsg = errormsg + " Email Id" + ",";
                            //}
                            //if (string.IsNullOrWhiteSpace(dr["mobileNumber"].ToString()))
                            //{
                            //    errormsg = errormsg + " Mobile Number" + ",";
                            //}
                            //if (string.IsNullOrWhiteSpace(dr["BankName"].ToString()))
                            //{
                            //    errormsg = errormsg + " Bank Name" + ",";
                            //}
                            //if (string.IsNullOrWhiteSpace(dr["AccountNumber"].ToString()))
                            //{
                            //    errormsg = errormsg + " Account Number" + ",";
                            //}
                            //if (string.IsNullOrWhiteSpace(dr["IFSCCode"].ToString()))
                            //{
                            //    errormsg = errormsg + " IFSC Code" + ",";
                            //}
                            //if (!string.IsNullOrEmpty(errormsg))
                            //{
                            //    errormsg = errormsg.TrimEnd(',');
                            //    errormsg = errormsg + " cannot be null";
                            //}

                            //if (!string.IsNullOrWhiteSpace(dr["mentorCode"].ToString()))
                            //{

                            //    if (!uploadPartner.Check_If_MentorCode_Exist(dr["mentorCode"].ToString()))
                            //    {
                            //        if (errormsg != null)
                            //        {
                            //            errormsg = errormsg + " , mentor code does not exist";
                            //        }
                            //        else
                            //        {
                            //            errormsg = errormsg + " mentor code does not exist";
                            //        }
                            //    }
                            //}
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
                            throw ex;
                        }
                    }
                }
                ds.Tables.Add(exceldt);
                ds.Tables.Add(Errordt);
                
            }
            catch (Exception ex)
            {
                Logger.Log.Error(Assembly.GetCallingAssembly().GetName().Name + "\n\t" + ex.ToString());
            }
            return ds;
        }

       
        public void Dispose()
        {
            //request = null;

            uploadPartner = null;

            _statusCode = HttpStatusCode.OK;

            _messages = null;
        }
    }
}


        
        

        
             
       
