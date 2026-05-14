using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StoreWebApi.Actions;
using StoreWebApi.DTO;
using StoreWebApi.Interfaces;
using StoreWebApi.Models;

namespace StoreWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "refreshTokenIsValid")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentGateWay _paymentService;

        public PaymentController(IPaymentGateWay paymentService)
        {
            _paymentService = paymentService;
        }

        /// <summary>
        /// pay the order and create the receipt 
        /// </summary>

        [HttpPost]
        [Idempotent]
        [Authorize(Roles ="Customer")]
        public async Task<IActionResult> ApplyPayment()
        {
            return Ok(await _paymentService.payForOrder());
        }

    }
}
