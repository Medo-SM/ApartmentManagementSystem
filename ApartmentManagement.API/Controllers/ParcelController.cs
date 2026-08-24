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
    public class ParcelController : BaseController
    {
        private readonly IParcelService _parcelService;

        public ParcelController(ILogger<ParcelController> logger, IParcelService parcelService)
            : base(logger)
        {
            _parcelService = parcelService;
        }

        [HttpPost]
        public IActionResult Create([FromBody] ParcelDto parcelDto)
        {
            try
            {
                if (parcelDto == null)
                {
                    return BadRequest(new { message = "Invalid parcel data.", success = false });
                }
                _parcelService.CreateParcel(parcelDto);
                return HandleResponse(new { message = "Parcel created successfully.", success = true });
            }
            catch (Exception ex)
            {
                return HandleError(ex, "Failed to create parcel.");
            }
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                var parcels = _parcelService.GetAllParcels();
                return HandleResponse(parcels);
            }
            catch (Exception ex)
            {
                return HandleError(ex, "Failed to retrieve parcels.");
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            try
            {
                var parcel = _parcelService.GetParcelById(id);
                return HandleResponse(parcel);
            }
            catch (Exception ex)
            {
                return HandleError(ex, $"Failed to retrieve parcel with ID {id}.");
            }
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] ParcelDto parcelDto)
        {
            try
            {
                if (parcelDto == null || parcelDto.Id != id)
                {
                    return BadRequest(new { message = "Invalid parcel data.", success = false });
                }
                _parcelService.UpdateParcel(parcelDto);
                return HandleResponse(new { message = "Parcel updated successfully.", success = true });
            }
            catch (Exception ex)
            {
                return HandleError(ex, $"Failed to update parcel with ID {id}.");
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                _parcelService.DeleteParcel(id);
                return HandleResponse(new { message = "Parcel deleted successfully.", success = true });
            }
            catch (Exception ex)
            {
                return HandleError(ex, $"Failed to delete parcel with ID {id}.");
            }
        }
    }
}
