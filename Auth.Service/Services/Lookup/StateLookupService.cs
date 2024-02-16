using Auth.Service.Models.Lookup.State;
using Auth.Service.Respositories.Lookup;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using UJBHelper.DataModel;

namespace Auth.Service.Services.Lookup
{
    public class StateLookupService : IStateService
    {
        private readonly IMongoCollection<StateInfo> _states;
        private IConfiguration _iconfiguration;
        public StateLookupService(IConfiguration config)
        {
            _iconfiguration = config;
            //  var client = new MongoClient(DbHelper.GetConnectionString());
            //  var database = client.GetDatabase(DbHelper.GetDatabaseName());
            var client = new MongoClient(_iconfiguration["ConnectionString"]);
            var database = client.GetDatabase(_iconfiguration["Database"]);
            _states = database.GetCollection<StateInfo>("States");  
        }

        public List<Get_Request> Get_State_Suggestion(int countryId, string searchTerm)
        {
            var res = new List<Get_Request>();
            var f2 = Builders<StateInfo>.Filter.Eq(x => x.countryId, countryId);
            if (searchTerm!="null")
            {
                f2 = f2 & Builders<StateInfo>.Filter.Where(x => x.stateName.ToLower().Contains(searchTerm.ToLower()));
            }

            res = _states.Find(f2).Project(x => new Get_Request
            {
                Id = x._id,
                CountryId = x.countryId,
                StateId = x.stateId,
                StateName = x.stateName
            }).ToList();
            return res;
        }
    }
}
