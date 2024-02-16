using Auth.Service.Models.Registeration.MentorList;
using System.Collections.Generic;

namespace Auth.Service.Respositories.Registeration
{
    public interface IMentorLookupService
    {
        List<Get_Request> Get_Mentor_By_Search(string searchTerm);
    }
}
