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
    public class RoleController : BaseController
    {
        private readonly IRoleService _roleService;

        public RoleController(ILogger<RoleController> logger, IRoleService roleService)
            : base(logger)
        {
            _roleService = roleService;
        }

        [HttpPost]
        public IActionResult Create([FromBody] RoleDto roleDto)
        {
            try
            {
                if (roleDto == null)
                {
                    return BadRequest(new { message = "Invalid role data.", success = false });
                }
                _roleService.CreateRole(roleDto);
                return HandleResponse(new { message = "Role created successfully.", success = true });
            }
            catch (Exception ex)
            {
                return HandleError(ex, "Failed to create role.");
            }
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                var roles = _roleService.GetAllRoles();
                return HandleResponse(roles);
            }
            catch (Exception ex)
            {
                return HandleError(ex, "Failed to retrieve roles.");
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            try
            {
                var role = _roleService.GetRoleById(id);
                return HandleResponse(role);
            }
            catch (Exception ex)
            {
                return HandleError(ex, $"Failed to retrieve role with ID {id}.");
            }
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] RoleDto roleDto)
        {
            try
            {
                if (roleDto == null || roleDto.Id != id)
                {
                    return BadRequest(new { message = "Invalid role data.", success = false });
                }
                _roleService.UpdateRole(roleDto);
                return HandleResponse(new { message = "Role updated successfully.", success = true });
            }
            catch (Exception ex)
            {
                return HandleError(ex, $"Failed to update role with ID {id}.");
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                _roleService.DeleteRole(id);
                return HandleResponse(new { message = "Role deleted successfully.", success = true });
            }
            catch (Exception ex)
            {
                return HandleError(ex, $"Failed to delete role with ID {id}.");
            }
        }
    }
}
