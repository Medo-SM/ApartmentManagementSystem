using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace ApartmentManagement.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class BaseController : ControllerBase
    {
        protected readonly ILogger<BaseController> Logger;

        protected BaseController(ILogger<BaseController> logger)
        {
            Logger = logger;
        }

        protected IActionResult HandleResponse<T>(T result, string message = null)
        {
            if (result == null)
            {
                Logger.LogWarning("Requested resource not found.");
                return NotFound(new
                {
                    message = "Resource not found.",
                    success = false
                });
            }

            var msgProp = result.GetType().GetProperty("Message");
            if (msgProp != null && message != null)
            {
                var currentValue = msgProp.GetValue(result);
                if (currentValue == null)
                    msgProp.SetValue(result, message);
            }

            return Ok(result);
        }

        protected IActionResult HandleError(Exception ex, string message = "An unexpected error occurred.")
        {
            Logger.LogError(ex, message);
            return StatusCode(500, new
            {
                message = message,
                success = false,
                error = ex.Message
            });
        }
    }
}
