namespace Auth.Service.Models.Registeration.User
{
    public class Put_Request
    {
       
        public string UserId { get; set; }
        //public string father_Husband_Name { get; set; }
        public string maritalStatus { get; set; }
        public string nationality { get; set; }
        public string phoneNo { get; set; }
        public string Hobbies { get; set; }
        public string aboutMe { get; set; }
        public string areaOfInterest { get; set; }
        public bool canImpartTraining { get; set; }
        public string  created_By {get;set;}
        public string updated_By { get; set; }
        
    }
}
