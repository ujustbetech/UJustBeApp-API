using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UJBHelper.DataModel;
using UJBHelper.DataModel.Common;


namespace UJBHelper.Common
{
    public static class Email_Sms_Sender
    {
        public static void Send_Email(string EmailId, string Firstname, string subject, string message_body)
        {
            var nq = new Notification_Queue();
            System_Default Sd = new System_Default();
            Sd = nq.checkEmailSendDetails();
            MailMessage message = new MailMessage();

            string fromemail = Sd.fromEmailID;
            var fromAddress = new MailAddress(fromemail, "UJustBe Connect");

            foreach (var address in EmailId.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries))
            {
                message.To.Add(address);
            }

            //Password for AWS
            string fromPassword = Sd.password;
            string Host = Sd.host;
            int port = Sd.port;
            string userName = Sd.userName;


            var smtp = new SmtpClient
            {
                Host = Host,//"email-smtp.us-east-1.amazonaws.com",
                Port = port,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(userName, fromPassword)
            };


            message.Subject = subject;
            message.Body = message_body;
            message.From = fromAddress;
            message.IsBodyHtml = true;


            smtp.Send(message);

            var emailBody = $"\t\nEmail sent successfully to {EmailId} on {DateTime.Now} with message body {message_body}.\t\n";
           
            Logger.Log.Debug(emailBody);
        }


        public static void Send_Email_attachment(string EmailId, string Firstname, string subject, string message_body, System.IO.StreamReader attachURL = null, string FileName = null)
        {
            //var fromAddress = new MailAddress("connect@ujustbe.com", "UJustBe Connect");
            //var toAddress = new MailAddress(EmailId, Firstname);
            //const string fromPassword = "BMis7Ju09ruKE/jbTCYSiNzuU9gnu/bLPi4vk5uhZ0Y6";
            var nq = new Notification_Queue();
            System_Default Sd = new System_Default();
            Sd = nq.checkEmailSendDetails();
            string fromemail = Sd.fromEmailID;
            var fromAddress = new MailAddress(fromemail, "UJustBe Connect");
            var toAddress = new MailAddress(EmailId, Firstname);
            //Password for AWS
            string fromPassword = Sd.password; //"BMis7Ju09ruKE/jbTCYSiNzuU9gnu/bLPi4vk5uhZ0Y6";
            string Host = Sd.host;
            int port = Sd.port;
            string userName = Sd.userName;
            MailMessage message;

            var smtp = new SmtpClient
            {
                Host = Host,//"email-smtp.us-east-1.amazonaws.com",
                Port = port,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(userName, fromPassword)
            };

            message = new MailMessage(fromAddress, toAddress);
            message.Subject = subject;
            message.Body = message_body;

            message.Attachments.Add(new Attachment(attachURL.BaseStream, FileName, "application/pdf"));
            {
                smtp.Send(message);
            }
            var emailBody = $"\t\nEmail sent successfully to {EmailId} on {DateTime.Now} with message body {message_body}.\t\n";

            Logger.Log.Debug(emailBody);
        }
        public static  void Send_Sms(string message, string mobileno)
        {

            try
            {
                var nq = new Notification_Queue();
                if (nq.checkSMSSendFlag() == "true")
                {
                    System_Default Sd = new System_Default();
                    Sd = nq.checkSMSSendDetails();
                    string userName = Sd.userName;//"Ujust_sms";
                    string password = Sd.password;//"155497";
                    string senderId = Sd.senderId;//"UJUSTB";

                    // string encodedMessage = HttpUtility.UrlEncode(message.Replace("+", "%2B"));
                    string encodedMessage = Uri.EscapeDataString((message).Replace("+", "%2B"));
                    var url = $"http://hapi.smsapi.org/SendSMS.aspx?UserName={userName}&password={password}&MobileNo={mobileno}&SenderID={senderId}&CDMAHeader={senderId}&Message={encodedMessage}";
                    WebRequest req = WebRequest.Create(url);

                    req.Proxy = new WebProxy(); //true means no proxy
                    WebResponse resp = req.GetResponse();
                    StreamReader sr = new StreamReader(resp.GetResponseStream());
                    var smsBody = $"\t\n SMS sent successfully to {mobileno} on {DateTime.Now} with message body {message}. \t\n";

                    Logger.Log.Debug(smsBody);
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(ex.ToString());
                throw;
            }

        }

        public static void Send_Push_Notification(string token, string mtitle, string message, Dictionary<string, string> additionalData)
        {


            try
            {
                //var notifydetails = notify_template.Data.Where(x => x.Receiver == notification.Receiver).FirstOrDefault();
                var applicationID = FCMDetails.FCMApplicationId;

                var senderId = FCMDetails.FCMSenderId;

                string deviceId = token; //token

                WebRequest tRequest = WebRequest.Create("https://fcm.googleapis.com/fcm/send");

                tRequest.Method = "post";

                tRequest.ContentType = "application/json";

                var data = new

                {

                    to = deviceId,

                    notification = new

                    {

                        body = message,

                        title = mtitle,

                        icon = "fcm_push_icon",
                        color = "#00326C",
                        click_action = "FCM_PLUGIN_ACTIVITY"


                    },
                    data = additionalData

                };



                var json = JsonConvert.SerializeObject(data);

                byte[] byteArray = Encoding.UTF8.GetBytes(json);

                tRequest.Headers.Add(string.Format("Authorization: key={0}", applicationID));

                tRequest.Headers.Add(string.Format("Sender: id={0}", senderId));

                tRequest.ContentLength = byteArray.Length;


                using (Stream dataStream = tRequest.GetRequestStream())
                {

                    dataStream.Write(byteArray, 0, byteArray.Length);


                    using (WebResponse tResponse = tRequest.GetResponse())
                    {

                        using (Stream dataStreamResponse = tResponse.GetResponseStream())
                        {

                            using (StreamReader tReader = new StreamReader(dataStreamResponse))
                            {

                                string sResponseFromServer = tReader.ReadToEnd();

                                FCM_Response res = JsonConvert.DeserializeObject<FCM_Response>(sResponseFromServer);
                                if (res.failure == 1)
                                {
                                    throw new Exception(res.results[0].error);
                                }
                            }
                        }
                    }
                }
            }


            catch (Exception ex)

            {
                throw;
            }

            //Update_Notification_Status(notification._id, "Push Notification Sent Successfully", notify_template._id, "success", fcm_number, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));


        }
    }
}
