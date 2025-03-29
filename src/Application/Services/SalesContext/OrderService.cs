using dotnet_api_erp.src.Application.DTOs;
using dotnet_api_erp.src.Application.Exceptions;
using dotnet_api_erp.src.Application.Mappers;
using dotnet_api_erp.src.Application.Services.Base;
using dotnet_api_erp.src.Application.Services.ProductContext;
using dotnet_api_erp.src.Domain.Entities.SalesContext;
using dotnet_api_erp.src.Domain.Entities.UserContext;
using dotnet_api_erp.src.Domain.Enums;
using dotnet_api_erp.src.Domain.Interfaces.ProductContext;
using dotnet_api_erp.src.Domain.Interfaces.SalesContext;
using dotnet_api_erp.src.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using static dotnet_api_erp.src.Application.DTOs.OrderDto;

#pragma warning disable CS9107

namespace dotnet_api_erp.src.Application.Services.SalesContext
{
    public class OrderService(IOrderRepository repository, IOrderItemRepository orderItemRepository, IProductRepository productRepository, StockMovementService stockMovementService, ApplicationDbContext context) : BaseService<Order, IOrderRepository>(repository, context)
    {
        private readonly IOrderItemRepository _orderItemRepository = orderItemRepository;
        private readonly IProductRepository _productRepository = productRepository;
        private readonly StockMovementService _stockMovementService = stockMovementService;

        public async Task<Order> AddAsync(CreateOrderDto order, RefreshToken token, CancellationToken ct)
        {
            var productIds = order.Items.Select(item => item.ProductId).Distinct().ToList();
            var products = await _productRepository.GetAllAsync(p => productIds.Contains(p.Id), 0, int.MaxValue, ct);

            var productDictionary = products.ToDictionary(p => p.Id, p => p);
            
            var totalValue = order.Items.Sum(item => item.Price * item.Quantity);
            if (!Equals(totalValue, order.TotalAmount))
                throw new BadRequestException("O valor total não corresponde à soma dos itens.");

            foreach (var item in order.Items)
            {
                if (!productDictionary.TryGetValue(item.ProductId, out var product))
                    throw new BadRequestException($"Produto com ID {item.ProductId} não encontrado.");

                if (item.Quantity == 0 || Math.Round((decimal)(item.Price / item.Quantity), 2) != Math.Round((decimal)product.Price, 2))
                    throw new BadRequestException($"O preço do item com ID {item.ProductId} não corresponde ao preço do produto.");

                if (item.Quantity > product.Quantity)
                    throw new BadRequestException($"A quantidade do item com ID {item.ProductId} é maior do que a disponível em estoque.");
            }

            var orderEntity = new Order(OrderMapper.ToCreateOrderDTO(order));
            orderEntity.Validate();
            await _repository.AddAsync(orderEntity, ct);

            var orderItems = OrderMapper.ToCreateOrderItemDTOs(orderEntity.Id, order.Items)
            .Select(item => new OrderItem(item))
            .ToList();

            foreach (var orderItem in orderItems)
            {
                if(productDictionary.TryGetValue(orderItem.ProductId, out var product))
                {
                    orderItem.Validate();
                    await _orderItemRepository.AddAsync(orderItem, ct);


                     await RegisterStockMovement(
                        product.Id,
                        $"Venda do produto {product.Name}, Quantidade {orderItem.Quantity}",
                        MovementType.Sales, 
                        orderItem.Quantity, 
                        token, 
                        ct
                    );
                }
            }

            return orderEntity;
        }
        public async Task DeleteAsync(Guid Id, RefreshToken token, CancellationToken ct)
        {
            Order Order = await repository.GetByIdAsync(Id,  ct, includeExpression: x => x.Include(u => u.OrderItems)) 
                ?? throw new NotFoundException("Ordem não encontrada");
            
            if (Order.OrderStatus == OrderStatus.PaymentConfirmed)
                throw new BadRequestException("Não é possível excluir a Ordem, pois ela já foi confirmada.");

            if (Order.OrderStatus == OrderStatus.Canceled)
                throw new BadRequestException("A Ordem já está cancelada.");

            Order.Delete();
            await repository.Update(Order, ct);
            
            var productIds = Order.OrderItems.Select(item => item.ProductId).Distinct().ToList();
            var products = await _productRepository.GetAllAsync(p => productIds.Contains(p.Id), 0, int.MaxValue, ct);

            var productDictionary = products.ToDictionary(p => p.Id, p => p);

            foreach (var orderItem in Order.OrderItems)
            {
                if(productDictionary.TryGetValue(orderItem.ProductId, out var product))
                {
                    await _orderItemRepository.Remove(orderItem, ct); 
                    
                    await RegisterStockMovement(
                        product.Id,
                        $"Retorno do produto {product.Name}, Quantidade {orderItem.Quantity}",
                        MovementType.Return, 
                        orderItem.Quantity, 
                        token, 
                        ct
                    );
                }
            }
        }
        public async Task DeleteRangeAsync(List<Guid> Ids, RefreshToken token, CancellationToken ct)
        {
            var tasks = Ids.Select(Id => DeleteAsync(Id, token, ct));
            await Task.WhenAll(tasks);
        }
        private async Task RegisterStockMovement(Guid productId, string descricao, MovementType tipo, int quantidade, RefreshToken token, CancellationToken ct)
        {
            CreateStockMovementDto movementDto = new()
            {
                ProductId = productId,
                Description = descricao,
                MovementType = tipo,
                Quantity = quantidade
            };
            await _stockMovementService.AddAsync(movementDto, token, ct);
        }
    }
}