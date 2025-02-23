using System.ComponentModel.DataAnnotations;

namespace dotnet_api_erp.src.Domain.Entities.Base
{
    public class BaseEntity
    {
        [Key]
        public Guid Id {get; private init;}
        public DateTime CreateAt {get; private set;}
        public DateTime UpdateAt {get; private set;}
        public bool Active {get; private set;}
        protected BaseEntity() {
            Id = Guid.NewGuid();
            CreateAt = DateTime.UtcNow;
            UpdateAt = DateTime.UtcNow;
            Active = true;
        }

        public void Update()
        {
            UpdateAt = DateTime.UtcNow;
        }

        public void Delete()
        {
            UpdateAt = DateTime.UtcNow;
            Active = false;
        }

        public void Reactive()
        {
            if(Active == false)
            {
                UpdateAt = DateTime.UtcNow;
                Active = true;
            }
        }

        public void Validate()
        {
            var validationResults = new List<ValidationResult>();
            var validationContext = new ValidationContext(this);

            if (!Validator.TryValidateObject(this, validationContext, validationResults, true))
            {
            var errors = validationResults
                .Where(vr => vr.ErrorMessage != null)
                .Select(vr => vr.ErrorMessage);

            throw new ValidationException(string.Join("; ", errors));
            }
        }

    }
}