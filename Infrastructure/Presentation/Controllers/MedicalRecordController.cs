using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Abstraction.Contracts;
using Shared.Dtos.MedicalRecordsDto;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/medical-records")]
    public class MedicalRecordController(IServiceManager _serviceManager) : ControllerBase
    {
        // POST /api/medical-records
        [HttpPost]
        [ProducesResponseType(typeof(MedicalRecordResultDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult<MedicalRecordResultDto>> CreateMedicalRecord(
            [FromBody] CreateMedicalRecordDto dto)
        {
            var record = await _serviceManager.MedicalRecordService.CreateMedicalRecordAsync(dto);
            return CreatedAtAction(nameof(GetMedicalRecordById), new { id = record.Id }, record);
        }

        // GET /api/medical-records/{id}
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(MedicalRecordResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<MedicalRecordResultDto>> GetMedicalRecordById(int id)
            => Ok(await _serviceManager.MedicalRecordService.GetMedicalRecordByIdAsync(id));

        // PUT /api/medical-records/{id}?requestingDoctorId=1
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(MedicalRecordResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult<MedicalRecordResultDto>> UpdateMedicalRecord(
            int id,
            [FromBody] UpdateMedicalRecordDto dto,
            [FromQuery] int requestingDoctorId)
            => Ok(await _serviceManager.MedicalRecordService
                    .UpdateMedicalRecordAsync(id, dto, requestingDoctorId));

        // GET /api/medical-records/patient/{patientId}
        [HttpGet("patient/{patientId:int}")]
        [ProducesResponseType(typeof(IEnumerable<MedicalRecordResultDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<MedicalRecordResultDto>>> GetPatientMedicalRecords(
            int patientId)
            => Ok(await _serviceManager.MedicalRecordService.GetPatientMedicalRecordsAsync(patientId));

        // GET /api/medical-records/doctor/{doctorId}
        [HttpGet("doctor/{doctorId:int}")]
        [ProducesResponseType(typeof(IEnumerable<MedicalRecordResultDto>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IEnumerable<MedicalRecordResultDto>>> GetDoctorMedicalRecords(
            int doctorId)
            => Ok(await _serviceManager.MedicalRecordService.GetDoctorMedicalRecordsAsync(doctorId));

        // POST /api/medical-records/{id}/vitals
        [HttpPost("{id:int}/vitals")]
        [ProducesResponseType(typeof(VitalSignResultDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<VitalSignResultDto>> AddVitalSign(
            int id, [FromBody] CreateVitalSignDto dto)
        {
            var vital = await _serviceManager.VitalSignService.AddVitalSignAsync(id, dto);
            return CreatedAtAction(nameof(GetMedicalRecordById), new { id }, vital);
        }

        // POST /api/medical-records/{id}/prescriptions
        [HttpPost("{id:int}/prescriptions")]
        [ProducesResponseType(typeof(PrescriptionResultDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult<PrescriptionResultDto>> AddPrescription(
            int id, [FromBody] CreatePrescriptionDto dto)
        {
            var prescription = await _serviceManager.PrescriptionService.AddPrescriptionAsync(id, dto);
            return CreatedAtAction(nameof(GetMedicalRecordById), new { id }, prescription);
        }

        // DELETE /api/medical-records/{recordId}/prescriptions/{presId}
        [HttpDelete("{recordId:int}/prescriptions/{presId:int}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<IActionResult> CancelPrescription(int recordId, int presId)
        {
            await _serviceManager.PrescriptionService.CancelPrescriptionAsync(recordId, presId);
            return NoContent();
        }

        // POST /api/medical-records/{id}/lab-orders
        [HttpPost("{id:int}/lab-orders")]
        [ProducesResponseType(typeof(LabOrderResultDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
        public async Task<ActionResult<LabOrderResultDto>> CreateLabOrder(
            int id, [FromBody] CreateLabOrderDto dto)
        {
            var order = await _serviceManager.LabOrderService.CreateLabOrderAsync(id, dto);
            return CreatedAtAction(nameof(GetMedicalRecordById), new { id }, order);
        }
    }
}
