using AutoMapper;
using Domain.Contracts;
using Domain.Models.DoctorModule;
using Services.Abstraction.Contracts;
using Services.Exceptions;
using Services.Specifications.DoctorModule;
using Shared.Dtos.DoctorModule.DepartmentDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Implementations.DoctorModule
{
    public class DepartmentService(IUnitOfWork _unitOfWork , IMapper _mapper) : IDepartmentService
    {
        public async Task<DepartmentResultDto> CreateDepartmentAsync(CreateDepartmentDto dto)
        {
            var deptRepo = _unitOfWork.GetRepository<Department, int>();
            
            var department = _mapper.Map<Department>(dto);
            await deptRepo.AddAsync(department);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<DepartmentResultDto>(department);

        }

        public async Task<IEnumerable<DepartmentResultDto>> GetAllDepartmentAsync()
        {

            var deptRepo = _unitOfWork.GetRepository<Department, int>();
            var departments = await deptRepo.GetAllAsync(new AllDepartmentsSpecification());
           
            
            return _mapper.Map<IEnumerable<DepartmentResultDto>>(departments);

        }

        public async Task<DepartmentResultDto> GetDepartmentByIdAsync(int id)
        {
            var deptRepo = _unitOfWork.GetRepository<Department, int>();
            var departments = await deptRepo.GetByIdAsync(new DepartmentWithHeadSpecification(id));

            if (departments is null)
                throw new DepartmentNotFoundException(id);

            return _mapper.Map<DepartmentResultDto>(departments);

        }

        public async Task<DepartmentResultDto> UpadateDepartmentAsync(int id, UpdateDepartmentDto dto)
        {

            var deptRepo = _unitOfWork.GetRepository<Department, int>();
            var department = await deptRepo.GetByIdAsync(id);

            if (department is null)
                throw new DepartmentNotFoundException(id);


            if (!string.IsNullOrEmpty(dto.Name)) department.Name = dto.Name;
            if (!string.IsNullOrEmpty(dto.Description)) department.Description = dto.Description;
            if (!string.IsNullOrEmpty(dto.PhoneExtension)) department.PhoneExtension = dto.PhoneExtension;
            if (dto.HeadDoctorId.HasValue) department.HeadDoctorId = dto.HeadDoctorId.Value;

            deptRepo.Update(department);
            await _unitOfWork.SaveChangesAsync();

            var updated = await deptRepo.GetByIdAsync(new DepartmentWithHeadSpecification(id));
            return _mapper.Map<DepartmentResultDto>(updated!);


        }
    }
}
