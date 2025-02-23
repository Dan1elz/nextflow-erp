namespace boilerplate_backend.src.Application.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException() : base() {}
        public NotFoundException(string message) : base(message) {}
        public NotFoundException(string message, Exception exception) : base(message, exception) {}
    }
}