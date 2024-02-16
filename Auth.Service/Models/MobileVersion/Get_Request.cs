namespace Auth.Service.Models.MobileVersion
{
    public class Get_Request
    {
        public AndroidInfo Android { get; set; }
        public IOSInfo IOS { get; set; }
    }
    public class AndroidInfo
    {
        public string androidVersion { get; set; }
        public bool isAndroidForce { get; set; }
    }

    public class IOSInfo
    {
        public string iosVersion { get; set; }
        public bool isIOSForce { get; set; }
    }
}
