using Reports.Service.Models.NativeError;

namespace Reports.Service.Repositories.NativeError
{
    public interface INativeErrorService
    {
        void Insert_Error_Log(Post_Request request,string FileDestination);
    }
}
