using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using dotnet_api_erp.src.Domain.Entities.Base;

namespace dotnet_api_erp.src.Domain.Entities.UserContext
{
    public class RefreshToken(Guid userId, string value) : BaseEntity 
    {
        [ForeignKey("User"), Required(ErrorMessage = "Por favor insira o Id do Usuario")]
        public Guid UserId {get; private set;} = userId;
        public virtual User? User {get; set;}
        
        [Required(ErrorMessage = "Por favor insira o Token")]
        public string Value {get; private set;} = value;
    }
}
