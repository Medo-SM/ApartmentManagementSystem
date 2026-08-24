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
    public class ApartmentController : BaseController
    {
        private readonly IApartmentService _apartmentService;

        public ApartmentController(ILogger<ApartmentController> logger, IApartmentService apartmentService)
            : base(logger)
        {
            _apartmentService = apartmentService;
        }

        [HttpPost]
        public IActionResult Create([FromBody] ApartmentDto apartmentDto)
        {
            try
            {
                if (apartmentDto == null)
                {
                    return BadRequest(new { message = "Invalid apartment data.", success = false });
                }
                _apartmentService.CreateApartment(apartmentDto);
                return HandleResponse(new { message = "Apartment created successfully.", success = true });
            }
            catch (Exception ex)
            {
                return HandleError(ex, "Failed to create apartment.");
            }
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                var apartments = _apartmentService.GetAllApartments();
                return HandleResponse(apartments);
            }
            catch (Exception ex)
            {
                return HandleError(ex, "Failed to retrieve apartments.");
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            try
            {
                var apartment = _apartmentService.GetApartmentById(id);
                return HandleResponse(apartment);
            }
            catch (Exception ex)
            {
                return HandleError(ex, $"Failed to retrieve apartment with ID {id}.");
            }
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] ApartmentDto apartmentDto)
        {
            try
            {
                if (apartmentDto == null || apartmentDto.Id != id)
                {
                    return BadRequest(new { message = "Invalid apartment data.", success = false });
                }
                _apartmentService.UpdateApartment(apartmentDto);
                return HandleResponse(new { message = "Apartment updated successfully.", success = true });
            }
            catch (Exception ex)
            {
                return HandleError(ex, $"Failed to update apartment with ID {id}.");
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                _apartmentService.DeleteApartment(id);
                return HandleResponse(new { message = "Apartment deleted successfully.", success = true });
            }
            catch (Exception ex)
            {
                return HandleError(ex, $"Failed to delete apartment with ID {id}.");
            }
        }
    }
}
