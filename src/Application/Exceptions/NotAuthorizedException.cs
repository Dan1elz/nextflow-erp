namespace dotnet_api_erp.src.Application.Exceptions
{
    public class NotAuthorizedException : Exception
    {
        public NotAuthorizedException() : base() {}
        public NotAuthorizedException(string message) : base(message) {}
        public NotAuthorizedException(string message, Exception exception) : base(message, exception) {}
    }
}