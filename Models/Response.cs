namespace UMA.Models
{
    public class Response
    {
        public bool IsError { get; set; }
        public string Message { get; set; }
        public int StatusCode { get; set; }
        public object Data { get; set; }
        public string Exception { get; set; }
        public Response(string message)
        {
            Message = message;
            IsError = true;
        }
        public Response()
        {
            
        }
    }
}
