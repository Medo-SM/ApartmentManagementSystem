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
    public class TenantController : BaseController
    {
        private readonly ITenantService _tenantService;

        public TenantController(ILogger<TenantController> logger, ITenantService tenantService)
            : base(logger)
        {
            _tenantService = tenantService;
        }

        [HttpPost]
        public IActionResult Create([FromBody] TenantDto tenantDto)
        {
            try
            {
                if (tenantDto == null)
                {
                    return BadRequest(new { message = "Invalid tenant data.", success = false });
                }
                _tenantService.CreateTenant(tenantDto);
                return HandleResponse(new { message = "Tenant created successfully.", success = true });
            }
            catch (Exception ex)
            {
                return HandleError(ex, "Failed to create tenant.");
            }
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                var tenants = _tenantService.GetAllTenants();
                return HandleResponse(tenants);
            }
            catch (Exception ex)
            {
                return HandleError(ex, "Failed to retrieve tenants.");
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            try
            {
                var tenant = _tenantService.GetTenantById(id);
                return HandleResponse(tenant);
            }
            catch (Exception ex)
            {
                return HandleError(ex, $"Failed to retrieve tenant with ID {id}.");
            }
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] TenantDto tenantDto)
        {
            try
            {
                if (tenantDto == null || tenantDto.Id != id)
                {
                    return BadRequest(new { message = "Invalid tenant data.", success = false });
                }
                _tenantService.UpdateTenant(tenantDto);
                return HandleResponse(new { message = "Tenant updated successfully.", success = true });
            }
            catch (Exception ex)
            {
                return HandleError(ex, $"Failed to update tenant with ID {id}.");
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                _tenantService.DeleteTenant(id);
                return HandleResponse(new { message = "Tenant deleted successfully.", success = true });
            }
            catch (Exception ex)
            {
                return HandleError(ex, $"Failed to delete tenant with ID {id}.");
            }
        }
    }
}
