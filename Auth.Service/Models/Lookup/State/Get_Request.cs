namespace Auth.Service.Models.Lookup.State
{
    public class Get_Request
    {
        public string Id { get; set; }
        public string StateName { get; set; }
        public int StateId { get; set; }
        public int CountryId { get; set; }
    }
}
