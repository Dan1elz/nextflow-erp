using System.Linq.Expressions;
using dotnet_api_erp.src.Application.Exceptions;
using dotnet_api_erp.src.Application.Mappers;
using dotnet_api_erp.src.Application.Services.Base;
using dotnet_api_erp.src.Domain.Entities.SalesContext;
using dotnet_api_erp.src.Domain.Entities.UserContext;
using dotnet_api_erp.src.Domain.Enums;
using dotnet_api_erp.src.Domain.Interfaces.SalesContext;
using dotnet_api_erp.src.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using static dotnet_api_erp.src.Application.DTOs.SaleDTO;
using static dotnet_api_erp.src.Application.DTOs.SharedDto;

#pragma warning disable CS9107

namespace dotnet_api_erp.src.Application.Services.SalesContext
{
    public class SaleService(ISaleRepository repository,  IOrderRepository orderRepository,  IPaymentRepository paymentRepository, ApplicationDbContext context) : BaseService<Sale, ISaleRepository>(repository, context)
    {
        private readonly IOrderRepository _orderRepository = orderRepository;
        private readonly IPaymentRepository _paymentRepository = paymentRepository;
        
        public async Task<Sale> AddAsync(RefreshToken token, CreateSaleDto sale, CancellationToken ct)
        {
            
            var order = await _orderRepository.GetByIdAsync(sale.OrderId, ct) ?? throw new NotFoundException("Ordem não encontrada");
            
            const decimal tolerance = 0.01m;
            if (Math.Abs((double)(order.TotalAmount - sale.Payments.Sum(item => item.Amount))) > (double)tolerance)
                throw new BadRequestException("Os valores dos pagamentos não condizem com o valor da ordem.");

            order.Update(OrderStatus.PaymentConfirmed);
            await _orderRepository.Update(order, ct);


            var SaleEntity = new Sale(SaleMapper.ToCreateSaleDTO(token.UserId, sale));
            SaleEntity.Validate();
            await _repository.AddAsync(SaleEntity, ct);

            var paymentItems = SaleMapper.ToCreatePaymentDTO(SaleEntity.Id, sale.Payments)
            .Select(item => new Payment(item))
            .ToList();

            foreach(var payment in paymentItems)
            {
                payment.Validate();
                await _paymentRepository.AddAsync(payment, ct);
            }

            return SaleEntity;
        }
        public override async Task<Sale> GetByIdAsync(Guid Id, CancellationToken ct) //GET{}
        {
            return await _repository.GetByIdAsync(Id, ct, includeExpression: x => x.Include(u => u.Payments)) ?? throw new NotFoundException("Entity not found");
        }
        public override async Task<Sale> GetAsync(Expression<Func<Sale, bool>> predicate, CancellationToken ct) //GET
        {
            return await _repository.GetAsync(predicate, ct, includeExpression: x => x.Include(u => u.Payments)) ?? throw new NotFoundException("Nenhumm dado encontrado.");
        }
        public override async Task<ApiResponseTable<Sale>> GetAllAsync(Expression<Func<Sale, bool>> predicate, int offset, int limit, CancellationToken ct) //GET
        {
            var data = await _repository.GetAllAsync(predicate, offset, limit, ct, includeExpression: x => x.Include(u => u.Payments)) ?? throw new NotFoundException("Nenhum dado encontrado.");
            var totalItems = await _repository.CountAsync(predicate, ct);

            return new ApiResponseTable<Sale>
            {
                Data = [.. data],
                TotalItems = totalItems
            };
        }
    }
}