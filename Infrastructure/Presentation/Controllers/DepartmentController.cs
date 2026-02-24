using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Services.Abstraction.Contracts;
using Shared.Dtos.DoctorModule.DepartmentDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Presentation.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DepartmentController(IServiceManager _serviceManager)  : ControllerBase
    {
        // POST /api/departments
        [HttpPost]
        [ProducesResponseType(typeof(DepartmentResultDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<DepartmentResultDto>> CreateDepartment([FromBody] CreateDepartmentDto dto)
        {
            var department = await _serviceManager.DepartmentService.CreateDepartmentAsync(dto);
            return CreatedAtAction(nameof(GetDepartmentById), new { id = department.Id }, department);
        }

        // GET /api/departments
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<DepartmentResultDto>), StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<DepartmentResultDto>>> GetAllDepartments()
            => Ok(await _serviceManager.DepartmentService.GetAllDepartmentAsync());

        // GET /api/departments/{id}
        [HttpGet("{id:int}")]
        [ProducesResponseType(typeof(DepartmentResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DepartmentResultDto>> GetDepartmentById(int id)
            => Ok(await _serviceManager.DepartmentService.GetDepartmentByIdAsync(id));

        // PUT /api/departments/{id}
        [HttpPut("{id:int}")]
        [ProducesResponseType(typeof(DepartmentResultDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DepartmentResultDto>> UpdateDepartment(
            int id, [FromBody] UpdateDepartmentDto dto)
            => Ok(await _serviceManager.DepartmentService.UpadateDepartmentAsync(id, dto));

    }
}
