namespace UJBHelper.Common
{
    public class Message_Info
    {
        public string Message { get; set; }
        public string Type { get; set; }
    }

   public enum Message_Type
    {
        SUCCESS,
        INFO,
        ERROR
    }
}
