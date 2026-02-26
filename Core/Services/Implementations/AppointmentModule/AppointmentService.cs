using AutoMapper;
using Domain.Contracts;
using Domain.Models.AppointmentModule;
using Domain.Models.DoctorModule;
using Domain.Models.Enums.AppointmentEnums;
using Domain.Models.Enums.DoctorEnums;
using Domain.Models.Enums.PatientEnums;
using Domain.Models.PatientModule;
using Services.Abstraction.Contracts;
using Services.Exceptions;
using Services.Specifications.AppointmentModule;
using Services.Specifications.DoctorModule;
using Shared;
using Shared.Dtos.AppointmentModule;
using Shared.Parameters;

namespace Services.Implementations.AppointmentModule
{
    public class AppointmentService (IUnitOfWork _unitOfWork,IMapper _mapper) : IAppointmentService
    {
        public async Task<AppointmentResultDto> BookAppointmentAsync(CreateAppointmentDto dto)
        {
            // 1. Validate patient exists & is Active
            var patientRepo = _unitOfWork.GetRepository<Patient, int>();
            var patient = await patientRepo.GetByIdAsync(dto.PatientId);
            if (patient is null) throw new PatientNotFoundException(dto.PatientId);
            if (patient.Status != PatientStatus.Active)
                throw new BusinessRuleException("Only Active patients can book appointments.");

            // 2. Validate doctor exists & is Active
            var doctorRepo = _unitOfWork.GetRepository<Doctor, int>();
            var doctor = await doctorRepo.GetByIdAsync(
                new DoctorWithDetailsSpecification(dto.DoctorId));
            if (doctor is null) throw new DoctorNotFoundException(dto.DoctorId);
            if (doctor.Status != DoctorStatus.Active)
                throw new BusinessRuleException("Only Active doctors can receive bookings.");

            // 3. Date must be today or future
            if (dto.AppointmentDate < DateOnly.FromDateTime(DateTime.UtcNow))
                throw new ValidationException(
                    "Appointment date must be today or in the future.");

            // 4. StartTime must fall within a DoctorSchedule slot for that DayOfWeek
            var dayOfWeek = (System.DayOfWeek)((int)dto.AppointmentDate.DayOfWeek);
            // Convert to our custom enum (Monday=1 ... Sunday=7)
            var customDay = (Domain.Models.Enums.DoctorEnums.DayOfWeeks)
                ((int)dayOfWeek == 0 ? 7 : (int)dayOfWeek);
            var matchingSchedule = doctor.AvailabilitySchedules
                .FirstOrDefault(s => s.DayOfWeek == customDay &&
                                     s.IsAvailable &&
                                     dto.StartTime >= s.StartTime &&
                                     dto.StartTime < s.EndTime);
            if (matchingSchedule is null)
                throw new BusinessRuleException(
                    "The requested time is outside the doctor's available schedule.");

            // 5. Compute EndTime from slot duration
            var endTime = dto.StartTime.AddMinutes(
                matchingSchedule.SlotDurationMinutes);

            // 6. Check for slot conflicts
            var aptRepo = _unitOfWork.GetRepository<Appointment, int>();
            var conflictSpec = new ConflictCheckSpecification(
                dto.DoctorId, dto.AppointmentDate, dto.StartTime, endTime);
            var conflicts = await aptRepo.GetAllAsync(conflictSpec);
            if (conflicts.Any())
                throw new BusinessRuleException(
                    "The doctor already has a conflicting appointment in this time slot.");

            // 7. Generate unique ConfirmationNumber
            var confirmationNumber = await GenerateConfirmationNumberAsync(
                dto.AppointmentDate, aptRepo);

            // 8. Create and save
            var appointment = new Appointment
            {
                PatientId = dto.PatientId,
                DoctorId = dto.DoctorId,
                AppointmentDate = dto.AppointmentDate,
                StartTime = dto.StartTime,
                EndTime = endTime,
                Status = AppointmentStatus.Scheduled,
                Type = dto.Type,
                ReasonForVisit = dto.ReasonForVisit,
                ConfirmationNumber = confirmationNumber,
                BookedAt = DateTime.UtcNow
            };

            await aptRepo.AddAsync(appointment);
            await _unitOfWork.SaveChangesAsync();

            // 9. Reload with navigation props for mapping
            var saved = await aptRepo.GetByIdAsync(
                new AppointmentWithDetailsSpecification(appointment.Id));
            return _mapper.Map<AppointmentResultDto>(saved!);
        }

        public async Task<AppointmentResultDto> GetAppointmentByIdAsync(int id)
        {
            var aptRepo = _unitOfWork.GetRepository<Appointment, int>();
            var apt = await aptRepo.GetByIdAsync(
                new AppointmentWithDetailsSpecification(id));
            if (apt is null) throw new AppointmentNotFoundException(id);
            return _mapper.Map<AppointmentResultDto>(apt);
        }

        public async Task<AppointmentResultDto> UpdateAppointmentStatusAsync(
            int id, UpdateAppointmentStatusDto dto)
        {
            var aptRepo = _unitOfWork.GetRepository<Appointment, int>();
            var apt = await aptRepo.GetByIdAsync(
                new AppointmentWithDetailsSpecification(id));
            if (apt is null) throw new AppointmentNotFoundException(id);

            // Enforce state machine rules
            ValidateStateTransition(apt.Status, dto.NewStatus);

            apt.Status = dto.NewStatus;
            apt.UpdatedAt = DateTime.UtcNow;

            if (dto.NewStatus == AppointmentStatus.Cancelled)
            {
                if (string.IsNullOrEmpty(dto.CancellationReason))
                    throw new ValidationException(
                        "CancellationReason is required when cancelling.");
                apt.CancellationReason = dto.CancellationReason;
            }

            if (!string.IsNullOrEmpty(dto.Notes))
                apt.Notes = dto.Notes;

            aptRepo.Update(apt);
            await _unitOfWork.SaveChangesAsync();

            var updated = await aptRepo.GetByIdAsync(
                new AppointmentWithDetailsSpecification(id));
            return _mapper.Map<AppointmentResultDto>(updated!);
        }

        private static void ValidateStateTransition(
            AppointmentStatus current, AppointmentStatus next)
        {
            var valid = (current, next) switch
            {
                (AppointmentStatus.Scheduled, AppointmentStatus.Confirmed) => true,
                (AppointmentStatus.Scheduled, AppointmentStatus.Cancelled) => true,
                (AppointmentStatus.Confirmed, AppointmentStatus.Cancelled) => true,
                (AppointmentStatus.Confirmed, AppointmentStatus.Completed) => true,
                (AppointmentStatus.Confirmed, AppointmentStatus.NoShow) => true,
                _ => false
            };
            if (!valid)
                throw new BusinessRuleException(
                    $"Cannot transition from {current} to {next}.");
        }

        public async Task<bool> CancelAppointmentAsync(int id, string reason)
        {
            var aptRepo = _unitOfWork.GetRepository<Appointment, int>();
            var apt = await aptRepo.GetByIdAsync(id);
            if (apt is null) throw new AppointmentNotFoundException(id);

            if (apt.Status == AppointmentStatus.Cancelled)
                throw new BusinessRuleException(
                    "Appointment is already cancelled.");

            // Cancellation window: must be at least 2 hours before start
            var appointmentDateTime = apt.AppointmentDate.ToDateTime(apt.StartTime);
            if (DateTime.UtcNow > appointmentDateTime.AddHours(-2))
                throw new BusinessRuleException(
                    "Cannot cancel an appointment less than 2 hours before start time.");

            apt.Status = AppointmentStatus.Cancelled;
            apt.CancellationReason = reason;
            apt.UpdatedAt = DateTime.UtcNow;

            aptRepo.Update(apt);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<AvailableSlotDto>> GetAvailableSlotsAsync(
            int doctorId, DateOnly date)
        {
            var doctorRepo = _unitOfWork.GetRepository<Doctor, int>();
            var doctor = await doctorRepo.GetByIdAsync(
                new DoctorWithDetailsSpecification(doctorId));
            if (doctor is null) throw new DoctorNotFoundException(doctorId);

            // Get doctor's schedule for that day
            var systemDayOfWeek = (System.DayOfWeek)((int)date.DayOfWeek);
            var customDay = (Domain.Models.Enums.DoctorEnums.DayOfWeeks)
                ((int)systemDayOfWeek == 0 ? 7 : (int)systemDayOfWeek);
            var schedule = doctor.AvailabilitySchedules
                .FirstOrDefault(s => s.DayOfWeek == customDay && s.IsAvailable);

            if (schedule is null) return [];  // No schedule that day

            // Get all booked appointments for that doctor+date
            var aptRepo = _unitOfWork.GetRepository<Appointment, int>();
            var booked = await aptRepo.GetAllAsync(
                new DoctorDailyAppointmentsSpecification(doctorId, date));
            var bookedActive = booked.Where(a =>
                a.Status == AppointmentStatus.Scheduled ||
                a.Status == AppointmentStatus.Confirmed).ToList();

            // Generate all slots for the schedule
            var slots = new List<AvailableSlotDto>();
            var slotStart = schedule.StartTime;
            while (slotStart.AddMinutes(schedule.SlotDurationMinutes) <= schedule.EndTime)
            {
                var slotEnd = slotStart.AddMinutes(schedule.SlotDurationMinutes);
                var isTaken = bookedActive.Any(a =>
                    a.StartTime < slotEnd && a.EndTime > slotStart);
                slots.Add(new AvailableSlotDto
                {
                    DoctorId = doctorId,
                    Date = date,
                    StartTime = slotStart,
                    EndTime = slotEnd,
                    IsAvailable = !isTaken
                });
                slotStart = slotEnd;
            }
            return slots;
        }

        public async Task<PaginatedResult<AppointmentResultDto>> GetAllAppointmentsAsync(
            AppointmentSpecificationParameters parameters)
        {
            var aptRepo = _unitOfWork.GetRepository<Appointment, int>();
            var apts = await aptRepo.GetAllAsync(
                new AppointmentListSpecification(parameters));
            var count = await aptRepo.CountAsync(
                new AppointmentCountSpecification(parameters));
            return new PaginatedResult<AppointmentResultDto>(
                parameters.PageIndex, parameters.PageSize, count,
                _mapper.Map<IEnumerable<AppointmentResultDto>>(apts));
        }

        public async Task<IEnumerable<AppointmentResultDto>>
            GetPatientAppointmentsAsync(int patientId)
        {
            var patientRepo = _unitOfWork.GetRepository<Patient, int>();
            if (await patientRepo.GetByIdAsync(patientId) is null)
                throw new PatientNotFoundException(patientId);
            var aptRepo = _unitOfWork.GetRepository<Appointment, int>();
            var apts = await aptRepo.GetAllAsync(
                new PatientAppointmentsSpecification(patientId));
            return _mapper.Map<IEnumerable<AppointmentResultDto>>(apts);
        }

        public async Task<IEnumerable<AppointmentResultDto>>
            GetDoctorAppointmentsAsync(int doctorId, DateOnly date)
        {
            var doctorRepo = _unitOfWork.GetRepository<Doctor, int>();
            if (await doctorRepo.GetByIdAsync(doctorId) is null)
                throw new DoctorNotFoundException(doctorId);
            var aptRepo = _unitOfWork.GetRepository<Appointment, int>();
            var apts = await aptRepo.GetAllAsync(
                new DoctorDailyAppointmentsSpecification(doctorId, date));
            return _mapper.Map<IEnumerable<AppointmentResultDto>>(apts);
        }

        public async Task<AppointmentResultDto> ConfirmAppointmentAsync(int id)
        {
            var aptRepo = _unitOfWork.GetRepository<Appointment, int>();
            var apt = await aptRepo.GetByIdAsync(
                new AppointmentWithDetailsSpecification(id));
            if (apt is null) throw new AppointmentNotFoundException(id);
            if (apt.Status != AppointmentStatus.Scheduled)
                throw new BusinessRuleException(
                    "Only Scheduled appointments can be confirmed.");
            apt.Status = AppointmentStatus.Confirmed;
            apt.UpdatedAt = DateTime.UtcNow;
            aptRepo.Update(apt);
            await _unitOfWork.SaveChangesAsync();
            var updated = await aptRepo.GetByIdAsync(
                new AppointmentWithDetailsSpecification(id));
            return _mapper.Map<AppointmentResultDto>(updated!);
        }

        public async Task<AppointmentResultDto> CompleteAppointmentAsync(
            int id, string? notes)
        {
            var aptRepo = _unitOfWork.GetRepository<Appointment, int>();
            var apt = await aptRepo.GetByIdAsync(
                new AppointmentWithDetailsSpecification(id));
            if (apt is null) throw new AppointmentNotFoundException(id);
            if (apt.Status != AppointmentStatus.Confirmed)
                throw new BusinessRuleException(
                    "Only Confirmed appointments can be completed.");
            apt.Status = AppointmentStatus.Completed;
            apt.UpdatedAt = DateTime.UtcNow;
            if (!string.IsNullOrEmpty(notes)) apt.Notes = notes;
            aptRepo.Update(apt);
            await _unitOfWork.SaveChangesAsync();
            var updated = await aptRepo.GetByIdAsync(
                new AppointmentWithDetailsSpecification(id));
            return _mapper.Map<AppointmentResultDto>(updated!);
        }

        private async Task<string> GenerateConfirmationNumberAsync(
            DateOnly date,
            Domain.Contracts.IGenericRepository<Appointment, int> aptRepo)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var rng = new Random();
            string number;
            do
            {
                var suffix = new string(Enumerable.Repeat(chars, 6)
                    .Select(s => s[rng.Next(s.Length)]).ToArray());
                number = $"APT-{date:yyyyMMdd}-{suffix}";
                // Check uniqueness
                var existing = await aptRepo.GetAllAsync(asNoTracking: true);
                if (!existing.Any(a => a.ConfirmationNumber == number))
                    break;
            } while (true);
            return number;
        }
    }
}
