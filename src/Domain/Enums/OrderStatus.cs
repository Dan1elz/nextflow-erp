namespace dotnet_api_erp.src.Domain.Enums
{
   public enum OrderStatus : byte
    {
        PendingPayment = 1,   // Aguardando pagamento
        PaymentConfirmed = 2, // Pagamento confirmado
        Processing = 3,       // Pedido em processamento
        Shipped = 4,          // Pedido enviado
        Delivered = 5,        // Pedido entregue
        Canceled = 6,         // Pedido cancelado
        Refunded = 7          // Pedido reembolsado
    }
}