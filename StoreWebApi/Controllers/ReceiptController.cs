using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StoreService.Interfaces;
using StoreService.Services;

namespace StoreWebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReceiptController : ControllerBase
    {
        private readonly IReceiptService _receiptService;

        public ReceiptController(IReceiptService receiptService)
        {
            _receiptService = receiptService;
        }
        /// <summary>
        /// get All Receipts
        /// </summary>
        [HttpGet]
        //[Authorize(Roles = "Admin,Customer")]

        public async Task<IActionResult> GetAllReceipts()
        {
            var result = await _receiptService.GetAllReceipts();
            if (!result.Success)
            {
                return StatusCode(result.StatusCode, result.Error);
            }
            return StatusCode(result.StatusCode, result.Result);
        }
        /// <summary>
        /// get item by name
        /// </summary>
        [HttpGet("{orderId}")]
        //[Authorize(Roles = "Admin,Customer")]
        public async Task<IActionResult> GetReceipt(int orderId)
        {
            var result = await _receiptService.GetReceipt(orderId);
            if (!result.Success)
            {
                return StatusCode(result.StatusCode, result.Error);
            }
            return StatusCode(result.StatusCode, result.Result);
        }
    }
}
