using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tea.Application.DTOs.Payments;
using Tea.Infrastructure.Interfaces;

namespace Tea.Api.Controllers
{
    [Authorize]
    public class PaymentsController : BaseApiController
    {
        private readonly IPaymentService _paymentService;

        public PaymentsController(IPaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost]
        public IActionResult CreatePaymentUrl(PaymentRequest request)
        {
            var url = _paymentService.CreatePaymentUrl(request, HttpContext);

            return new JsonResult(url);
        }

        [HttpPost("return")]
        public async Task<IActionResult> PaymentReturn(PaymentReturnRequest request)
        {
            var response = await _paymentService.HandlePayment(request);

            return Ok(response);
        }
    }
}
