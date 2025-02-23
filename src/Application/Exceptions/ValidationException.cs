namespace dotnet_api_erp.src.Application.Exceptions
{
    public class ValidationException : Exception
    {
        public IReadOnlyList<string> Errors { get; }

        public ValidationException(IEnumerable<string> errors)
            : base("Ocorreram erros de validação.")
        {
            Errors = new List<string>(errors).AsReadOnly();
        }
    }
}