using Auth.Service.Models.MobileVersion;

namespace Auth.Service.Respositories.MobileVersion
{
    public  interface IGetVersionDetails
    {
        Get_Request GetVersionDetails(string Type);
    }
}
