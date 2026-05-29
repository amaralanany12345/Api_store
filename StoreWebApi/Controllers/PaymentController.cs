using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StoreWebApi.Actions;
using StoreDomain.Models;
using StoreService.Interfaces;
using StoreService.DTO;

namespace StoreWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Policy = "refreshTokenIsValid")]
    public class PaymentController : ControllerBase
    {
        private readonly IPaymentGateWayService _paymentService;

        public PaymentController(IPaymentGateWayService paymentService)
        {
            _paymentService = paymentService;
        }

        /// <summary>
        /// pay the order and create the receipt 
        /// </summary>

        [HttpPost]
        //[Idempotent]
        [Authorize(Roles ="Customer")]
        public async Task<IActionResult> ApplyPayment()
        {
            var result=await _paymentService.PayForOrder();
            if (!result.Success)
            {
                return StatusCode(result.StatusCode, result.Error);
            }
            return StatusCode(result.StatusCode,result.Result);
        }

    }
}
