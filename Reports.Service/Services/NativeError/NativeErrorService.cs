using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using Reports.Service.Models.NativeError;
using Reports.Service.Repositories.NativeError;
using System;
using System.IO;
using System.Reflection;
using UJBHelper.Common;
using UJBHelper.DataModel;

namespace Reports.Service.Services.NativeError
{
    public class NativeErrorService : INativeErrorService
    {
        private readonly IMongoCollection<ErrorLogs> _errorLog;
        private IConfiguration _iconfiguration;
        public string abc = "";
        public NativeErrorService(IConfiguration config)
        {
            _iconfiguration = config;
            //  var client = new MongoClient(DbHelper.GetConnectionString());
            //  var database = client.GetDatabase(DbHelper.GetDatabaseName());
            var client = new MongoClient(_iconfiguration["ConnectionString"]);
            var database = client.GetDatabase(_iconfiguration["Database"]);
            _errorLog = database.GetCollection<ErrorLogs>("ErrorLogs");
        }

        public void Insert_Error_Log(Post_Request request, string FileDestination)
        {

          //  string abc = "";
            // Set a variable to the Documents path.
            string docPath ="\\"+ DateTime.Now.ToString("yyyy-MM-dd") + ".txt";
            Logger.Log.Error("docPath :" + "\n\t" + docPath);
            string path = FileDestination;
            Logger.Log.Error("Path :" + "\n\t" + path);
            // Write the string array to a new file named "WriteLines.txt".
            if (!File.Exists(path + docPath))
            {
                Logger.Log.Error("file not exist :");
                try
                {
                    using (FileStream fs = new FileStream(path + docPath
                                      , FileMode.OpenOrCreate
                                      , FileAccess.ReadWrite))
                    {
                        StreamWriter tw = new StreamWriter(fs);
                        Log(request, tw) ;
                        tw.Flush();
                    }
                }
                catch (Exception ex)
                {
                    abc = ex.Message.ToString();
                }
               
            }
            else
            {
                Logger.Log.Error("file  exist :");
                try { 
                using (StreamWriter w = File.AppendText(path+docPath))
                {
                    abc = "file  exist :";
                    Log(request, w);
                        w.Flush();
                }
            }
                catch (Exception ex)
            {
                abc = ex.Message.ToString();
            }
        }
            //var el = new ErrorLogs
            //{
            //    UserId = request.UserId,
            //    Url = request.Url,
            //    screen = path,
            //    Method =docPath,
            //    message = abc,
            //    error = request.error,
            //    date = request.date,
            //    source = request.source,
            //    Created = new Created
            //    {
            //        created_By = request.createdBy,
            //        created_On = DateTime.Now
            //    },
            //    Updated = new Updated()
            //};

            //_errorLog.InsertOne(el);

        }
        public void Log(Post_Request request, TextWriter w)
        {
            try { 
    Logger.Log.Error("Log Entry :");
            w.Write("\r\nLog Entry : ");
            w.WriteLine($"{DateTime.Now.ToLongTimeString()} {DateTime.Now.ToLongDateString()}");
            w.WriteLine("  :");
            w.WriteLine($"  UserId = {request.UserId}");
            w.WriteLine($"  Source = {request.source}");
            w.WriteLine($"  Screen = {request.screen}");
            w.WriteLine($"  Url = {request.Url}");
            w.WriteLine($"  Method = {request.method}");
            w.WriteLine($"  Message = {request.message}");
            w.WriteLine($"  Error = {request.error}");
            w.WriteLine($"  Date = {request.date}");         
            w.WriteLine("-------------------------------");
                w.Close();
        }
                catch (Exception ex)
                {
                    abc = ex.Message.ToString();
                }
}

       
    }
}


