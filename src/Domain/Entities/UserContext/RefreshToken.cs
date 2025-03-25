using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using dotnet_api_erp.src.Domain.Entities.Base;

namespace dotnet_api_erp.src.Domain.Entities.UserContext
{
    public class RefreshToken : BaseEntity 
    {
        [ForeignKey("User"), Required(ErrorMessage = "Por favor insira o Id do Usuario")]
        public Guid UserId {get; set;}
        public virtual User? User {get; set;}
        
        [Required(ErrorMessage = "Por favor insira o Token")]
        public string Value {get; set;} = string.Empty;

        private RefreshToken() { }

        public RefreshToken(Guid userId, string value)
        {
            UserId = userId;
            Value = value;
        }
    }
}
