using Lead_Management.Service.Models.Payment;
using UJBHelper.Common;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using Lead_Management.Service.Repositories.Payment;
using UJBHelper.DataModel;

namespace Lead_Management.Service.Manager.Payment
{
    public class Insert:IDisposable
    {
        private Post_Request request;
        private IAddPaymentService _addPaymentService;
        public HttpStatusCode _statusCode = HttpStatusCode.OK;
        public List<Message_Info> _messages = null;
        public string TransactionId = "";

        public Insert(Post_Request request, IAddPaymentService addPaymentService)
        {
            this.request = request;
            _addPaymentService = addPaymentService;
            _messages = new List<Message_Info>();
        }

        private double calculatePartnerShare(double Amt, string LeadId)
        {
            double partnershare = 0;
            double UJBaggreedshare = 0;

            UJBaggreedshare = _addPaymentService.GetSharedPercentage(LeadId, 2);
            if (Amt != 0)
            {
                partnershare = ((Amt * UJBaggreedshare) / 100);               
                partnershare.ToString("00");
            }
            return partnershare;
        }

        private double calculateMentorShare(double Amt, string LeadId)
        {
            double mentorshare = 0;
            double UJBaggreedshare = 0;

            UJBaggreedshare = _addPaymentService.GetSharedPercentage(LeadId, 3);
            if (Amt != 0)
            {
                mentorshare = ((Amt * UJBaggreedshare) / 100);
              
              
                mentorshare.ToString("00");
            }
            return mentorshare;
        }
        public double calculateUJBShare(double Amt, string LeadId)
        {
            double UJBshare = 0;
            double UJBaggreedshare = 0;

            UJBaggreedshare = _addPaymentService.GetSharedPercentage(LeadId,1);
            if (Amt != 0)
            {
                UJBshare = ((Amt * UJBaggreedshare) / 100);
                UJBshare.ToString("00");
            }
            return UJBshare;
        }

        public double calculateLPMentorShare(double Amt,string LeadId)
        {
            //_addPaymentService.Update_System_Default("PaymentTransactionCode");
            double UJBshare = 0;
            double UJBaggreedshare = 0;

            UJBaggreedshare= _addPaymentService.GetSharedPercentage(LeadId,4);
            if (Amt != 0)
            {
                UJBshare = ((Amt * UJBaggreedshare) / 100);
                UJBshare.ToString("00");
            }
            return UJBshare;
        }
        public void Update_System_Default()
        {
            try
            {
                _addPaymentService.Update_System_Default("PaymentTransactionCode");

                _messages.Add(new Message_Info
                {
                    Message = "Payment Created Successfully",
                    Type = Message_Type.SUCCESS.ToString()
                });

                _statusCode = HttpStatusCode.OK;
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

        public void Process()
        {
            if (request.PayType == 1)
            {
                request.ShareRecvdByPartners = new ShareRecievedByPartners();
                request.ShareRecvdByPartners.RecievedByReferral = calculatePartnerShare(request.amount,request.leadId);
                request.ShareRecvdByPartners.RecievedByMentor = calculateMentorShare(request.amount, request.leadId);
                request.ShareRecvdByPartners.RecievedByUJB = calculateUJBShare(request.amount, request.leadId);
                request.ShareRecvdByPartners.RecievedByLPMentor = calculateLPMentorShare(request.amount, request.leadId);
            }
            else
            {
                request.ShareRecvdByPartners = new ShareRecievedByPartners();
            }

            if (string.IsNullOrWhiteSpace(request.PaymentId))
            {
                Insert_New_Payment();
                Update_System_Default();
            }

            else
            {
                if (Check_If_Payment_Exist())
                {
                    Update_Payment_Details();
                }

            }

        }

        private void Update_Payment_Details()
        {
            try
            {
                _addPaymentService.Update_Payment_Details(request);

                _messages.Add(new Message_Info
                {
                    Message = "Payment Details Updated",
                    Type = Message_Type.SUCCESS.ToString()
                });

                _statusCode = HttpStatusCode.OK;
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

        private bool Check_If_Payment_Exist()
        {
            try
            {
                if (_addPaymentService.Check_If_Payment_Exist(request.PaymentId))
                {
                    return true;
                }
                _messages.Add(new Message_Info
                {
                    Message = "No Payment Details Found",
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
                    Message = "No Payment Details Found",
                    Type = Message_Type.ERROR.ToString()
                });

                _statusCode = HttpStatusCode.NotFound;

                return false;
            }
        }

        private void Insert_New_Payment()
        {
            try
            {
                TransactionId = _addPaymentService.Insert_New_Payment(request);

                _messages.Add(new Message_Info
                {
                    Message = "Payment Details Added Successfully",
                    Type = Message_Type.SUCCESS.ToString()
                });

                _statusCode = HttpStatusCode.OK;
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
        public void Dispose()
        {
            request = null;

            _addPaymentService = null;

            _statusCode = HttpStatusCode.OK;

            _messages = null;
        }

    }
}
