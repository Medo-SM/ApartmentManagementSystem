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
    public class IssueController : BaseController
    {
        private readonly IIssueService _issueService;

        public IssueController(ILogger<IssueController> logger, IIssueService issueService)
            : base(logger)
        {
            _issueService = issueService;
        }

        [HttpPost]
        public IActionResult Create([FromBody] IssueDto issueDto)
        {
            try
            {
                if (issueDto == null)
                {
                    return BadRequest(new { message = "Invalid issue data.", success = false });
                }
                _issueService.CreateIssue(issueDto);
                return HandleResponse(new { message = "Issue created successfully.", success = true });
            }
            catch (Exception ex)
            {
                return HandleError(ex, "Failed to create issue.");
            }
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            try
            {
                var issues = _issueService.GetAllIssues();
                return HandleResponse(issues);
            }
            catch (Exception ex)
            {
                return HandleError(ex, "Failed to retrieve issues.");
            }
        }

        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            try
            {
                var issue = _issueService.GetIssueById(id);
                return HandleResponse(issue);
            }
            catch (Exception ex)
            {
                return HandleError(ex, $"Failed to retrieve issue with ID {id}.");
            }
        }

        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] IssueDto issueDto)
        {
            try
            {
                if (issueDto == null || issueDto.Id != id)
                {
                    return BadRequest(new { message = "Invalid issue data.", success = false });
                }
                _issueService.UpdateIssue(issueDto);
                return HandleResponse(new { message = "Issue updated successfully.", success = true });
            }
            catch (Exception ex)
            {
                return HandleError(ex, $"Failed to update issue with ID {id}.");
            }
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            try
            {
                _issueService.DeleteIssue(id);
                return HandleResponse(new { message = "Issue deleted successfully.", success = true });
            }
            catch (Exception ex)
            {
                return HandleError(ex, $"Failed to delete issue with ID {id}.");
            }
        }
    }
}
