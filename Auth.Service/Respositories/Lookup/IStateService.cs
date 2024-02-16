using Auth.Service.Models.Lookup.State;
using System.Collections.Generic;

namespace Auth.Service.Respositories.Lookup
{
    public interface IStateService
    {
        List<Get_Request> Get_State_Suggestion(int countryId,string query);
    }
}
