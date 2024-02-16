using System.Collections.Generic;

namespace UJBHelper.Common
{
    public class Common
    {
        public enum DealStatusEnum
        {
            ReferralCreated = 0,
            NotConnected = 1,
            CalledButNoResponse = 2,
            DealNotClosed = 3,
            DiscussionInProgress = 4,
            DealClosed = 5,
            ReceivedPartPayment = 6,
            WorkInProgress = 7,
            WorkCompleted = 8,
            ReceivedFullAndFinalPayment = 9,
            AgreedPercentageTransferredToUJB = 10
        }

        //public enum UserType
        //{
        //    Homemaker=1,
        //    Employee=2,
        //    Student=3,
        //    Retired=4,
        //    Other=5,
        //    Freelancer=6,
        //    Consultant=7,
        //    CompanyOwner=8,
        //    Others=9
        //}

        public enum UserType
        {
            Individual_Proprietor = 1,
            PartnerShipFirm = 2,
            LLP = 3,
            Company = 4,
           // Proprietor=5
        }

        public enum PaymentType
        {
            Cheque=1,
            Cash=2,
            NEFT=3,
            Google_Pay=4,
            PhonePe=5,
            Other=6
        }

        public enum PayType
        {
            Inward = 1,
            Outward = 2
        }

        public enum PaymentFor
        {
            Subscription = 1,
            Membership = 2,
            Referred = 3
        }

        public enum ReferralStatusEnum
        {
            Pending = 0,
            Accepted = 1,
            Rejected = 2
        }

        public enum BusinessApproval
        {
            Pending = 0,
            Accepted = 1,
            Rejected = 2
        }

        public enum RejectReasons
        {
            PanCard_Rejected = 1,
            AadharCard_Rejected = 2,
            PanCard_AadharCard_Rejected = 3,
            PanCard_BankDetails_Rejected =5,
            BankDetails_Rejected = 4,
            AadharCard_BankDetails_Rejected = 6,
            PanCard_AadharCard_BankDetails_Rejected = 7
        }

        public enum NotificationReciver
        {
            UJBAdmin = 1,
            User = 2,
            Guest=3,
            Partner = 4,          
            Listed_Partner =5,
            Partner_Mentor = 6,
            Listed_Partner_Mentor=7,
            Referral = 8,
            Referred = 9
        }
    }

    public static class FCMDetails
    {
        public static string FCMApplicationId = "AAAA_3yfMkg:APA91bGPji6IdqaxgvQZZCK6XHOSQo6w8IjKBA1EF0P2Zossc-Sz4TU31rAuZkmCmX6rA6btuC0X1Td7SFHcieIr6UoPe9WM5w1uBCnCMw85YvrJ5eMZ1E8xroKkew7MHBo_6leYqfTb";
        public static string FCMSenderId = "1097307468360";
    }

    public static class NotifyList
    {
        public static List<string> NotificationListHolder = new List<string> {
            "Skip KYC",
            "KYC Approval Under Process",
            "Approve Partner",
            "Reject Partner",
            "Lead Created",
            "Lead Acceptance",
            "Lead Rejection",
            "Lead Status Changed",
            "Lead Auto Rejection",
            "Auto Rejection Reminder",
            "Incomplete Profile",
            "No Client Partner Products",
        };
    }

 
}

    
