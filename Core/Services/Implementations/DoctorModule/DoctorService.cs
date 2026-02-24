using AutoMapper;
using Domain.Contracts;
using Domain.Models.DoctorModule;
using Domain.Models.Enums.DoctorEnums;
using Services.Abstraction.Contracts;
using Services.Exceptions;
using Services.Specifications.DoctorModule;
using Shared;
using Shared.Dtos.DoctorModule.DoctorDtos;
using Shared.Parameters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Implementations.DoctorModule
{
    public class DoctorService(IUnitOfWork _unitOfWork , IMapper _mapper) : IDoctorService
    {
        public async Task<DoctorResultDto> RegisterDoctorAsync(CreateDoctorDto dto)
        {
            var doctorRepo = _unitOfWork.GetRepository<Doctor, int>();
            var deptRepo = _unitOfWork.GetRepository<Department, int>();

            // Step 1 :=> بيتشك الاول واشوف القسم موجود  ولا لا 
            var department = await deptRepo.GetByIdAsync(dto.DepartmentId);
            if (department is null)
                throw new DepartmentNotFoundException(dto.DepartmentId);

            //Step 2 :=> تأكد إن LicenseNumber مش مكرر
            var allDoctors = await doctorRepo.GetAllAsync(asNoTracking: true);
            if (allDoctors.Any(d => d.LicenseNumber == dto.LicenseNumber))
                throw new DuplicateLicenseNumberException(dto.LicenseNumber);

            //Step 3:=>  تأكد إن Email مش مكرر
            if (allDoctors.Any(d => d.Email.ToLower() == dto.Email.ToLower()))
                throw new DuplicateDoctorEmailException(dto.Email);

            //Step 4: => Mapping
            var doctor = _mapper.Map<Doctor>(dto);
            doctor.JoinDate = DateTime.UtcNow;
            doctor.Status = DoctorStatus.Active;

            await doctorRepo.AddAsync(doctor);
            await _unitOfWork.SaveChangesAsync();

            // Reload with Department لأن الـ mapper محتاج DepartmentName
            var saved = await doctorRepo.GetByIdAsync(new DoctorWithDetailsSpecification(doctor.Id));
            return _mapper.Map<DoctorResultDto>(saved);

        }
        public async Task<DoctorResultDto> GetDoctorByIdAsync(int id)
        {
            var doctorRepo = _unitOfWork.GetRepository<Doctor, int>();
            var doctor = await doctorRepo.GetByIdAsync(new DoctorWithDetailsSpecification(id));

            if (doctor is null)
                throw new DoctorNotFoundException(id);

            return _mapper.Map<DoctorResultDto>(doctor);

        }
        public async Task<DoctorResultDto> UpdateDoctorAsync(int id, UpdateDoctorDto dto)
        {
            var doctorRepo = _unitOfWork.GetRepository<Doctor, int>();
            var doctor = await doctorRepo.GetByIdAsync(new DoctorWithDetailsSpecification(id));
            
            if(doctor is null)
                throw new DoctorNotFoundException(id);

            if (!string.IsNullOrEmpty(dto.FirstName)) doctor.FirstName = dto.FirstName;
            if (!string.IsNullOrEmpty(dto.LastName)) doctor.LastName = dto.LastName;
            if (!string.IsNullOrEmpty(dto.Phone)) doctor.Phone = dto.Phone;
            if (!string.IsNullOrEmpty(dto.Email)) doctor.Email = dto.Email;
            if (!string.IsNullOrEmpty(dto.Specialization)) doctor.Specialization = dto.Specialization;
            if (!string.IsNullOrEmpty(dto.Bio)) doctor.Bio = dto.Bio;
            if (!string.IsNullOrEmpty(dto.PictureUrl)) doctor.PictureUrl = dto.PictureUrl;
            if (dto.DepartmentId.HasValue) doctor.DepartmentId = dto.DepartmentId.Value;
            if (dto.YearsOfExperience.HasValue) doctor.YearsOfExperience = dto.YearsOfExperience.Value;
            if (dto.ConsultationFee.HasValue) doctor.ConsultationFee = dto.ConsultationFee.Value;
            if (dto.Status.HasValue) doctor.Status = dto.Status.Value;

            doctorRepo.Update(doctor);
            await _unitOfWork.SaveChangesAsync();

            var updated = await doctorRepo.GetByIdAsync(new DoctorWithDetailsSpecification(id));
            return _mapper.Map<DoctorResultDto>(updated!);

        }
        public async Task<bool> DeactiveDoctorAsync(int id)
        {
            var doctorRepo = _unitOfWork.GetRepository<Doctor, int>();
            var doctor = await doctorRepo.GetByIdAsync(id);
            if (doctor is null)
                throw new DoctorNotFoundException(id);
            if (doctor.Status == DoctorStatus.Inactive)
                throw new BusinessRuleException("Doctor is already inactive.");

            doctor.Status =DoctorStatus.Inactive;
            doctorRepo.Update(doctor);
            await _unitOfWork.SaveChangesAsync();

            return true; 
        }
        public async Task<PaginatedResult<DoctorResultDto>> GetAllDoctorsAsync(DoctorSpecificationParameters parameters)
        {
            var doctorRepo = _unitOfWork.GetRepository<Doctor, int>();

            var doctors = await doctorRepo.GetAllAsync(new DoctorListSpecification(parameters));
            var totalCount = await doctorRepo.CountAsync(new DoctorCountSpecification(parameters));
            var doctorResult = _mapper.Map<IEnumerable<DoctorResultDto>>(doctors);

            return new PaginatedResult<DoctorResultDto>(
                parameters.PageIndex,
                parameters.PageSize,
                totalCount,
                doctorResult );

        }
        public async Task<DoctorWithDetailsResultDto> GetDoctorWithDetailsAsync(int id)
        {
            var doctorRepo = _unitOfWork.GetRepository<Doctor, int>();
            var doctor = await doctorRepo.GetByIdAsync(new DoctorWithDetailsSpecification(id));

            if (doctor is null)
                throw new DoctorNotFoundException(id);

            return _mapper.Map<DoctorWithDetailsResultDto>(doctor);

        }
        public async Task<QualificationResultDto> AddQualificationAsync(int doctorId, CreateQualificationDto dto)
        {

            var doctorRepo = _unitOfWork.GetRepository<Doctor, int>();
            var doctor = await doctorRepo.GetByIdAsync(doctorId);

            if (doctor is null)
                throw new DoctorNotFoundException(doctorId);

            var qualification = _mapper.Map<DoctorQualification>(dto);
            qualification.DoctorId = doctorId;

            var qualRepo = _unitOfWork.GetRepository<DoctorQualification, int>();
            await qualRepo.AddAsync(qualification);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<QualificationResultDto>(qualification);

        }
        public async Task<bool> RemoveQualificationAsync(int doctorId, int qualId)
        {

            var qualRepo = _unitOfWork.GetRepository<DoctorQualification, int>();
            var qualification = await qualRepo.GetByIdAsync(qualId);

            if(qualification is null)
                throw new QualificationNotFoundException(qualId);

            if (qualification.DoctorId != doctorId)
                throw new BusinessRuleException($"Qualification with ID {qualId} does not belong to doctor {doctorId}");


            qualRepo.Delete(qualification);
            await _unitOfWork.SaveChangesAsync();
            return true;

        }
        public async Task<ScheduleResultDto> SetScheduleAsync(int doctorId, CreateScheduleDto dto)
        {
            var doctorRepo = _unitOfWork.GetRepository<Doctor, int>();
            var doctor =await doctorRepo.GetByIdAsync(doctorId);

            if (doctor is null)
                throw new DoctorNotFoundException(doctorId);

            // Business Rule 1: EndTime > StartTime
            if (dto.EndTime <= dto.StartTime)
                throw new InvalidScheduleTimeException();

            var scheduleRepo = _unitOfWork.GetRepository<DoctorSchedule, int>();
            var existing = await scheduleRepo.GetAllAsync(new DoctorScheduleSpecification (doctorId));


            //مفيش جدولين لنفس اليوم

            if (existing.Any(s => s.DayOfWeek == dto.DayOfWeek))
                throw new OverlappingScheduleException(dto.DayOfWeek.ToString());

            var schedule = _mapper.Map<DoctorSchedule>(dto);
            schedule.DoctorId = doctorId;


            await scheduleRepo.AddAsync(schedule);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<ScheduleResultDto>(schedule);

        }
        public async Task<IEnumerable<ScheduleResultDto>> GetScheduleAsync(int doctorId)
        {
            var doctorRepo = _unitOfWork.GetRepository<Doctor, int>();
            if (await doctorRepo.GetByIdAsync(doctorId) is null)
                throw new DoctorNotFoundException(doctorId);


            var scheduleRepo = _unitOfWork.GetRepository<DoctorSchedule, int>();
            var schedules = await scheduleRepo.GetAllAsync(new DoctorScheduleSpecification(doctorId));

            return _mapper.Map<IEnumerable<ScheduleResultDto>>(schedules);

        }
        public async Task<IEnumerable<DoctorResultDto>> GetDoctorByDepartmentAsync(int departmentId)
        {
            var deptRepo = _unitOfWork.GetRepository<Department, int>();
            if(await deptRepo.GetByIdAsync(departmentId)is null)
                throw new DepartmentNotFoundException(departmentId);

            var doctorRepo = _unitOfWork.GetRepository<Doctor, int>();
            var doctors = await doctorRepo.GetAllAsync(new DoctorByDepartmentSpecification(departmentId));

            return _mapper.Map<IEnumerable<DoctorResultDto>>(doctors);

        }
        public async Task<IEnumerable<DoctorResultDto>> GetAvailableDoctorAsync(DateTime date)
        {
            var doctorRepo = _unitOfWork.GetRepository<Doctor, int>();
            var doctors = await doctorRepo.GetAllAsync(new DoctorAvailableOnDateSpecification(date));

            return _mapper.Map<IEnumerable<DoctorResultDto>>(doctors);

        }
       
    }
}
