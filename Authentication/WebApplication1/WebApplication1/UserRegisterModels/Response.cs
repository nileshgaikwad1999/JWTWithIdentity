namespace WebApplication1.UserRegisterModels
{
    public class Response
    {
        public Error Error { get; set; }

        public bool IsSucess { get; set; } 

        public dynamic Data { get; set; }
    }

    public class Error
    {
        public string ErrorMessage { get; set; }

        public string ErrorCode { get; set; }
    }
}
