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
    public class UserController : BaseController
    {
        private readonly IUserService _userService;

        public UserController(ILogger<UserController> logger, IUserService userService)
            : base(logger)
        {
            _userService = userService;
        }

        [HttpPost]
        public IActionResult Create([FromBody] UserDto userDto)
        {
            try
            {
                if (userDto == null)
                {
                    return BadRequest(new { message = "Invalid user data.", success = false });
                }
                _userService.CreateUser(userDto);
                return HandleResponse(new { message = "User created successfully.", success = true });
            }
            catch (Exception ex)
            {
                return HandleError(ex, "Failed to create user.");
            }
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                var users = _userService.GetAllUsers();
                return HandleResponse(users);
            }
            catch (Exception ex)
            {
                return HandleError(ex, "Failed to retrieve users.");
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            try
            {
                var user = _userService.GetUserById(id);
                return HandleResponse(user);
            }
            catch (Exception ex)
            {
                return HandleError(ex, $"Failed to retrieve user with ID {id}.");
            }
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] UserDto userDto)
        {
            try
            {
                if (userDto == null || userDto.Id != id)
                {
                    return BadRequest(new { message = "Invalid user data.", success = false });
                }
                _userService.UpdateUser(userDto);
                return HandleResponse(new { message = "User updated successfully.", success = true });
            }
            catch (Exception ex)
            {
                return HandleError(ex, $"Failed to update user with ID {id}.");
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                _userService.DeleteUser(id);
                return HandleResponse(new { message = "User deleted successfully.", success = true });
            }
            catch (Exception ex)
            {
                return HandleError(ex, $"Failed to delete user with ID {id}.");
            }
        }
    }
}
