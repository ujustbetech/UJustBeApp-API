using Auth.Service.Models.Lookup.Country;
using Auth.Service.Respositories.Lookup;
using Microsoft.Extensions.Configuration;
using MongoDB.Driver;
using System;
using System.Linq;
using UJBHelper.DataModel;

namespace Auth.Service.Services.Lookup
{
    public class CountryLookupService : ICountryService
    {
        private readonly IMongoCollection<CountryInfo> _countries;
        private IConfiguration _iconfiguration;
        public CountryLookupService(IConfiguration config)
        {
            _iconfiguration = config;
            //  var client = new MongoClient(DbHelper.GetConnectionString());
            //  var database = client.GetDatabase(DbHelper.GetDatabaseName());
            var client = new MongoClient(_iconfiguration["ConnectionString"]);
            var database = client.GetDatabase(_iconfiguration["Database"]);
            _countries = database.GetCollection<CountryInfo>("CountryCode");
        }

        public Get_Request GetCountries()
        {
            var res = new Get_Request();
            var filter = Builders<CountryInfo>.Filter.Empty;
            res.countries = _countries.Find(filter).Project(x =>
                     new CountryInfo  {_id = x._id, countryName = x.countryName, code = x.code, countryId = x.countryId, }).ToList();

            res.totalCount = Convert.ToInt32(res.countries.Count().ToString());

            return res;
        }
    }
}
