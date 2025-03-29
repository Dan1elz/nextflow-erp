namespace dotnet_api_erp.src.Domain.DTOs.ProducContext
{
    public class SuppliersDTO
    {
        public record CreateSupplierDTO(
            string Name, 
            string Address, 
            string ZipCode, 
            string CNPJ, 
            string Phone, 
            string Email 
        );
        public record UpdateSupplierDTO(string Name, 
            string Address, 
            string ZipCode, 
            string CNPJ, 
            string Phone, 
            string Email
        );
    }
}