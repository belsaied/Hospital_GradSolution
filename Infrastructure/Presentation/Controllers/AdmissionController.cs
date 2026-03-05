using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Abstraction.Contracts;
using Shared.Dtos.WardBedModule.AdmissionDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/admissions")]
    public class AdmissionController(IServiceManager _serviceManager) : ControllerBase
    {// POST api/admissions
        [HttpPost]
        [ProducesResponseType(typeof(AdmissionResultDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult<AdmissionResultDto>> AdmitPatient(
            [FromBody] CreateAdmissionDto dto)
        {
            var admission = await _serviceManager.AdmissionService.AdmitPatientAsync(dto);
            return CreatedAtAction(nameof(GetAdmissionById), new { id = admission.Id }, admission);
        }

        // GET api/admissions/5
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(AdmissionResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AdmissionResultDto>> GetAdmissionById(int id)
            => Ok(await _serviceManager.AdmissionService.GetAdmissionByIdAsync(id));

        // GET api/admissions/active
        [HttpGet("active")]
        [ProducesResponseType(typeof(IEnumerable<AdmissionResultDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<AdmissionResultDto>>> GetActiveAdmissions()
            => Ok(await _serviceManager.AdmissionService.GetActiveAdmissionsAsync());

        // GET api/admissions/patient/3
        [HttpGet("patient/{patientId:int}")]
        [ProducesResponseType(typeof(IEnumerable<AdmissionResultDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<AdmissionResultDto>>> GetPatientAdmissionHistory(
            int patientId)
            => Ok(await _serviceManager.AdmissionService.GetPatientAdmissionHistoryAsync(patientId));

        // PUT api/admissions/5/discharge
        [HttpPut("{id:int}/discharge")]
        [ProducesResponseType(typeof(AdmissionResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult<AdmissionResultDto>> DischargePatient(
            int id, [FromBody] DischargeDto dto)
            => Ok(await _serviceManager.AdmissionService.DischargePatientAsync(id, dto));

        // POST api/admissions/5/transfer
        [HttpPost("{id:int}/transfer")]
        [ProducesResponseType(typeof(AdmissionResultDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult<AdmissionResultDto>> TransferPatient(
            int id, [FromBody] TransferBedDto dto)
        {
            var result = await _serviceManager.AdmissionService.TransferPatientAsync(id, dto);
            return StatusCode(StatusCodes.Status201Created, result);
        }
    }
}
