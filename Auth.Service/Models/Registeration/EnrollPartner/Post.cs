﻿using System;
using System.Collections.Generic;
using UJBHelper.DataModel;

namespace Auth.Service.Models.Registeration.EnrollPartner
{
    public class Post_Request
    {
        public string userId { get; set; }
        public string gender { get; set; }
        public DateTime? birthDate { get; set; }
        public Address addressInfo { get; set; }
        public string knowledgeSource { get; set; }
        public string mentorCode { get; set; }
        public string organisationType { get; set; }
        public int userType { get; set; }
        public List<string> Localities { get; set; }
        public string passiveIncome { get; set; }
        public double latitude { get; set; }
        public double longitude { get; set; }
        public int countryId { get; set; }
        public int stateId { get; set; }
        public string myMentorCode { get;set; }
    }

    //public class Address
    //{
    //    public string Location { get; set; }
    //    public string Flat_Wing { get; set; }
    //    public string Locality { get; set; }
    //}
}
