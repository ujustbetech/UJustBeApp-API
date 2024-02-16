using Lead_Management.Service.Models.Referral;
using Lead_Management.Service.Repositories.Referral;
using System;
using System.Collections.Generic;
using System.Net;
using System.Reflection;
using UJBHelper.Common;
using UJBHelper.DataModel;

namespace Lead_Management.Service.Manager.Referral
{
    public class UpdateDealValue:IDisposable
    {
        public DealValue_Get _response;
        private DealValue_Put request;
        private IReferralService _referralService;
        public HttpStatusCode _statusCode = HttpStatusCode.OK;
        public List<Message_Info> _messages = null;

        public UpdateDealValue(DealValue_Put request, IReferralService referralService)
        {
            this.request = request;
            _referralService = referralService;
            _messages = new List<Message_Info>();
        }

        public void Process()
        {
            try
            {
                if (Verify_lead())
                {
                    Update_DealValue();
                }
            }
            catch (Exception ex)
            {
                Logger.Log.Error(Assembly.GetCallingAssembly().GetName().Name + "\n\t" + ex.ToString());
            }
        }
                   
        

        private double calculatePartnerShare(double Amt)
        {
            double partnershare = 0;
            if (Amt != 0)
            {
                partnershare = ((Amt *50)/100);
                partnershare.ToString("00");
            }
            return partnershare;
        }

        private double calculateMentorShare(double Amt)
        {
            double mentorshare = 0;
            if (Amt != 0)
            {
                mentorshare = ((Amt * 30) / 100);
                mentorshare.ToString("00");
            }
            return mentorshare;
        }
        public double calculateUJBShare(double Amt)
        {
            double UJBshare = 0;
            if (Amt != 0)
            {
                UJBshare = ((Amt * 20) / 100);
                UJBshare.ToString("00");
            }
            return UJBshare;
        }

        
        private bool Verify_lead()
        {
            try
            {
                if (_referralService.Check_If_Lead_Exists(request.leadId))
                {
                    return true;
                }
                _messages.Add(new Message_Info
                {
                    Message = "Lead Not Found",
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
                    Message = "Lead Not Found",
                    Type = Message_Type.ERROR.ToString()
                });

                _statusCode = HttpStatusCode.NotFound;

                return false;
            }
        }
        private double calculatePartnerShare(double Amt, string LeadId)
        {
            double partnershare = 0;
            double UJBaggreedshare = 0;

            UJBaggreedshare = _referralService.GetSharedPercentage(LeadId, 2);
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

            UJBaggreedshare = _referralService.GetSharedPercentage(LeadId, 3);
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

            UJBaggreedshare = _referralService.GetSharedPercentage(LeadId, 1);
            if (Amt != 0)
            {
                UJBshare = ((Amt * UJBaggreedshare) / 100);
                UJBshare.ToString("00");
            }
            return UJBshare;
        }

        public double calculateLPMentorShare(double Amt, string LeadId)
        {
            //_addPaymentService.Update_System_Default("PaymentTransactionCode");
            double UJBshare = 0;
            double UJBaggreedshare = 0;

            UJBaggreedshare = _referralService.GetSharedPercentage(LeadId, 4);
            if (Amt != 0)
            {
                UJBshare = ((Amt * UJBaggreedshare) / 100);
                UJBshare.ToString("00");
            }
            return UJBshare;
        }
        private void Update_DealValue()
        {
            request.shareReceivedByPartner = new ShareRecievedByPartners();
            try
            {
                double PerValue = 0;
                // if its amount
                if (request.PercentOrAmt == 2)
                {
                    PerValue = request.Value;
                }
                else
                {
                    PerValue = ((request.dealValue * request.Value) / 100);
                }

              //request.shareReceivedByPartner.RecievedByReferral = calculatePartnerShare(PerValue);
              //request.shareReceivedByPartner.RecievedByMentor = calculateMentorShare(PerValue);
              //request.shareReceivedByPartner.RecievedByUJB = calculateUJBShare(PerValue);

               
                request.shareReceivedByPartner.RecievedByReferral = calculatePartnerShare(PerValue, request.leadId);
                request.shareReceivedByPartner.RecievedByMentor = calculateMentorShare(PerValue, request.leadId);
                request.shareReceivedByPartner.RecievedByUJB = calculateUJBShare(PerValue, request.leadId);
                request.shareReceivedByPartner.RecievedByLPMentor = calculateLPMentorShare(PerValue, request.leadId);

                _response =_referralService.Update_DealValue(request);

                _messages.Add(new Message_Info
                {
                    Message = "Deal Value Updated",
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
            _referralService = null;
            _messages = null;
           
        }
    }
}
