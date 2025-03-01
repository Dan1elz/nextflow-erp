using dotnet_api_erp.src.Domain.DTOs.SalesContext;
using dotnet_api_erp.src.Domain.Entities.Base;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace dotnet_api_erp.src.Domain.Entities.ProductContext
{
    public class Product(ProductDTO createProductDTO) : BaseEntity()
    {
        // Supplier não foi criado
        public Supplier SupplierId { get; protected set; }

        [StringLength(100, ErrorMessage = "O nome não pode exceder 100 caracteres")]
        [Required(ErrorMessage = "O nome deve ser obrigatório")]
        public string Name { get; protected set; }

        [Required(ErrorMessage = "Você deve inserir uma descrição")]
        public string Description { get; internal set; }

        public string Image { get; protected set; }

        public int Quantity { get; protected set; }

        public double Price { get; protected set; }
        [Required(ErrorMessage = "A validade deve ser obrigatória")]
        public DateTime Validity { get; protected set; }
        public DateTime CreateAt { get; protected set; }
        public DateTime UpdateAt { get; protected set; }
        public bool Active
        {
            get; protected set;
        }
    }

}