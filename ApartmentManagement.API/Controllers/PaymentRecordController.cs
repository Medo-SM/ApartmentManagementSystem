using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Application.DTOs;
using Application.Interfaces;

namespace ApartmentManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentRecordController : BaseController
    {
        private readonly IPaymentRecordService _paymentRecordService;

        public PaymentRecordController(ILogger<PaymentRecordController> logger, IPaymentRecordService paymentRecordService)
            : base(logger)
        {
            _paymentRecordService = paymentRecordService;
        }

        [HttpPost]
        public IActionResult Create([FromBody] PaymentRecordDto paymentRecordDto)
        {
            try
            {
                if (paymentRecordDto == null)
                {
                    return BadRequest(new { message = "Invalid payment record data.", success = false });
                }
                _paymentRecordService.CreatePaymentRecord(paymentRecordDto);
                return HandleResponse(new { message = "Payment record created successfully.", success = true });
            }
            catch (Exception ex)
            {
                return HandleError(ex, "Failed to create payment record.");
            }
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                var paymentRecords = _paymentRecordService.GetAllPaymentRecords();
                return HandleResponse(paymentRecords);
            }
            catch (Exception ex)
            {
                return HandleError(ex, "Failed to retrieve payment records.");
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            try
            {
                var paymentRecord = _paymentRecordService.GetPaymentRecordById(id);
                return HandleResponse(paymentRecord);
            }
            catch (Exception ex)
            {
                return HandleError(ex, $"Failed to retrieve payment record with ID {id}.");
            }
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] PaymentRecordDto paymentRecordDto)
        {
            try
            {
                if (paymentRecordDto == null || paymentRecordDto.Id != id)
                {
                    return BadRequest(new { message = "Invalid payment record data.", success = false });
                }
                _paymentRecordService.UpdatePaymentRecord(paymentRecordDto);
                return HandleResponse(new { message = "Payment record updated successfully.", success = true });
            }
            catch (Exception ex)
            {
                return HandleError(ex, $"Failed to update payment record with ID {id}.");
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                _paymentRecordService.DeletePaymentRecord(id);
                return HandleResponse(new { message = "Payment record deleted successfully.", success = true });
            }
            catch (Exception ex)
            {
                return HandleError(ex, $"Failed to delete payment record with ID {id}.");
            }
        }
    }
}
