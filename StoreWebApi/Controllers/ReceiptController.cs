using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StoreWebApi.Actions;
using StoreWebApi.Interfaces;
using StoreWebApi.Models;

namespace StoreWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReceiptController : ControllerBase
    {
        private readonly IPaymentGateWay _receiptService;

        public ReceiptController(IPaymentGateWay receiptService)
        {
            _receiptService = receiptService;
        }
        /// <summary>
        /// pay the order and create the receipt 
        /// </summary>
        [HttpPost]
        //[Idempotent]
        public async Task<IActionResult> createReceipt(int orderId)
        {
            return Ok(await _receiptService.payForOrder(orderId));
        }

    }
}
