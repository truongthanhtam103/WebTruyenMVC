namespace WebTruyenMVC.Entity
{
    public class MessagesResponse
    {
        public int Code { get; set; } = 200;
        public string Message { get; set; } = "Successful";
        public object Data { get; set; } = new object();
    }
}
