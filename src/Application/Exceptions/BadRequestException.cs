namespace dotnet_api_erp.src.Application.Exceptions
{
    public class BadRequestException : Exception
    {
        public BadRequestException() : base() {}
        public BadRequestException(string message) : base(message) {}
        public BadRequestException(string message, Exception exception) : base(message, exception) {}
    }
}