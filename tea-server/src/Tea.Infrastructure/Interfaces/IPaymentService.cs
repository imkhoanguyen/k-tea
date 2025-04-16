using Microsoft.AspNetCore.Http;
using Tea.Application.DTOs.Orders;
using Tea.Application.DTOs.Payments;

namespace Tea.Infrastructure.Interfaces
{
    public interface IPaymentService
    {
        string CreatePaymentUrl(PaymentRequest request, HttpContext context);
        Task<OrderResponse?> HandlePayment(PaymentReturnRequest request);
    }
}