namespace dotnet_api_erp.src.Application.DTOs
{
    public class FileDto
    {
        public Guid Id { get; set; }
        public required string FileName { get; set; }
        public required string FileType { get; set; }
        public required string Base64 { get; set; }
    }
}
