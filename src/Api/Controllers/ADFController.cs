using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Api.Contracts.Requests;
using Api.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Management.DataFactory.Models;

namespace Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ADFController : ControllerBase
    {
        private readonly IADFService _adfService;

        public ADFController(IADFService adfService)
        {
            _adfService = adfService;
        }

        [HttpPost("pipeline")]
        public async Task<IActionResult> TriggerPipelineAsync(TriggerPipelineRequest request, CancellationToken cancellationToken = default)
        {
            var response = await _adfService.RunPipelineAsync(request.PipelineName, request.Parameters, cancellationToken);

            if (response != null)
            {
                return CreatedAtAction(nameof(GetPipelineStatus), new { id = response.RunId }, response);
            }

            return NotFound();

        }

        [HttpGet("pipeline/{id}")]
        public async Task<IActionResult> GetPipelineStatus(Guid id, CancellationToken cancellationToken = default)
        {
            var response = await _adfService.GetPipelineStatusAsync(id.ToString(), cancellationToken);

            if (response != null)
            {
                return Ok(response);
            }

            return NotFound();
        }
    }
}
