using dotnet_api_erp.src.Application.DTOs;
using dotnet_api_erp.src.Application.Exceptions;
using dotnet_api_erp.src.Application.Mappers;
using dotnet_api_erp.src.Application.Services.Base;
using dotnet_api_erp.src.Domain.Entities.ProductContext;
using dotnet_api_erp.src.Domain.Entities.UserContext;
using dotnet_api_erp.src.Domain.Enums;
using dotnet_api_erp.src.Domain.Interfaces.ProductContext;
using dotnet_api_erp.src.Infrastructure.Data;

#pragma warning disable CS9107

namespace dotnet_api_erp.src.Application.Services.ProductContext
{
    public class StockMovementService(IStockMovementRepository repository, IProductRepository productRepository, ApplicationDbContext context) : BaseService<StockMovement, IStockMovementRepository>(repository, context)
    {
        private readonly IProductRepository _productRepository = productRepository;
        
        public async Task<StockMovement> AddAsync(CreateStockMovementDto stockMovement, RefreshToken token, CancellationToken ct)
        {
            var product = await _productRepository.GetByIdAsync(stockMovement.ProductId, ct) ?? throw new NotFoundException("Produto não encontrado.");
            
            if (stockMovement.Quantity <= 0)
                throw new BadRequestException("Quantidade inválida. Valor deve ser maior que zero.");

            switch (stockMovement.MovementType)
            {
                case MovementType.Entry:
                case MovementType.Return:
                    product.SetMovementStock(product.Quantity + stockMovement.Quantity);
                    break;

                case MovementType.Exit:
                case MovementType.Sales:
                    if (stockMovement.Quantity > product.Quantity)
                        throw new BadRequestException("Quantidade inválida. Valor deve ser menor ou igual ao estoque atual.");
                    product.SetMovementStock(product.Quantity - stockMovement.Quantity);
                    break;

                case MovementType.Adjustment:
                    product.SetMovementStock(stockMovement.Quantity);
                    break;

                default:
                    throw new BadRequestException("Tipo de movimentação inválido.");
            }

            await _productRepository.Update(product, ct);

             var stockMovementEntity = new StockMovement(StockMovementMapper.ToCreateStockMovementDTO(stockMovement, product, token.UserId));
            stockMovementEntity.Validate();
            
            await _repository.AddAsync(stockMovementEntity, ct);
            return stockMovementEntity;
        }
        public async override Task DeleteAsync(Guid Id, CancellationToken ct)
        {
            StockMovement entity = await _repository.GetByIdAsync(Id, ct) 
                ?? throw new NotFoundException("Movimentação de Estoque não encontrada");
            
            if (entity.CreateAt.AddHours(24) <= DateTime.UtcNow)
                throw new BadRequestException("Não é possível excluir movimentações criadas há mais de 24 horas.");
               
            await _repository.Remove(entity, ct);
        }
        public async override Task DeleteRangeAsync(List<Guid> Ids, CancellationToken ct)
        {
            var tasks = Ids.Select(Id => DeleteAsync(Id, ct));
            await Task.WhenAll(tasks);
        }
    }
}