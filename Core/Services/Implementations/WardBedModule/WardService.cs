using AutoMapper;
using Domain.Contracts;
using Domain.Models.Enums.WardBedEnums;
using Domain.Models.WardBedModule;
using Services.Abstraction.Contracts.WardBedService;
using Services.Exceptions;
using Services.Specifications.WardBedModule;
using Shared.Dtos.WardBedModule.RoomsDtos;
using Shared.Dtos.WardBedModule.WardDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Implementations.WardBedModule
{
    public class WardService(IUnitOfWork _unitOfWork, IMapper _mapper) : IWardService
    {
        public async Task<WardResultDto> CreateWardAsync(CreateWardDto dto)
        {
            var repo = _unitOfWork.GetRepository<Ward, int>();

            // BR-16: Ward Name must be unique
            var all = await repo.GetAllAsync(asNoTracking: true);
            if (all.Any(w => w.Name.ToLower() == dto.Name.ToLower()))
                throw new ConflictException($"Ward with name '{dto.Name}' already exists.");

            var ward = _mapper.Map<Ward>(dto);
            ward.IsActive = true;
            await repo.AddAsync(ward);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<WardResultDto>(ward);
        }

        public async Task<IEnumerable<WardOccupancySummaryDto>> GetAllWardsWithOccupancyAsync()
        {
            var repo = _unitOfWork.GetRepository<Ward, int>();
            var wards = await repo.GetAllAsync(new AllWardsWithRoomsSpecification());

            return wards.Select(w =>
            {
                var allBeds = w.Rooms.SelectMany(r => r.Beds).ToList();
                var total = allBeds.Count;
                var occupied = allBeds.Count(b => b.Status == BedStatus.Occupied);
                var available = allBeds.Count(b => b.Status == BedStatus.Available);
                var maintenance = allBeds.Count(b => b.Status == BedStatus.Maintenance);
                return new WardOccupancySummaryDto
                {
                    WardId = w.Id,
                    WardName = w.Name,
                    WardType = w.WardType.ToString(),
                    TotalBeds = total,
                    OccupiedBeds = occupied,
                    AvailableBeds = available,
                    MaintenanceBeds = maintenance,
                   // OccupancyPercentage = total == 0 ? 0 : Math.Round((decimal)occupied / total * 100, 2)
                };
            });
        }

        public async Task<WardResultDto> GetWardByIdAsync(int id)
        {
            var repo = _unitOfWork.GetRepository<Ward, int>();
            var ward = await repo.GetByIdAsync(id);
            if (ward is null) throw new NotFoundException("Ward", id);
            return _mapper.Map<WardResultDto>(ward);
        }

        public async Task<WardResultDto> UpdateWardAsync(int id, UpdateWardDto dto)
        {
            var repo = _unitOfWork.GetRepository<Ward, int>();
            var ward = await repo.GetByIdAsync(id);
            if (ward is null) throw new NotFoundException("Ward", id);

            if (!string.IsNullOrEmpty(dto.Name)) ward.Name = dto.Name;
            if (dto.WardType.HasValue) ward.WardType = dto.WardType.Value;
            if (dto.Floor.HasValue) ward.Floor = dto.Floor.Value;
            if (!string.IsNullOrEmpty(dto.PhoneExtension)) ward.PhoneExtension = dto.PhoneExtension;
            if (!string.IsNullOrEmpty(dto.Description)) ward.Description = dto.Description;
            if (dto.IsActive.HasValue) ward.IsActive = dto.IsActive.Value;

            repo.Update(ward);
            await _unitOfWork.SaveChangesAsync();
            return _mapper.Map<WardResultDto>(ward);
        }

        public async Task<RoomResultDto> AddRoomToWardAsync(int wardId, CreateRoomDto dto)
        {
            var wardRepo = _unitOfWork.GetRepository<Ward, int>();
            if (await wardRepo.GetByIdAsync(wardId) is null)
                throw new NotFoundException("Ward", wardId);

            var room = _mapper.Map<Room>(dto);
            room.WardId = wardId;
            room.IsActive = true;
            var roomRepo = _unitOfWork.GetRepository<Room, int>();
            await roomRepo.AddAsync(room);
            await _unitOfWork.SaveChangesAsync();

            //  بعد SaveChanges، reload الـ room بـ Id عادي مع Ward navigation
            var spec = new RoomsByWardSpecification(wardId);
            var rooms = await roomRepo.GetAllAsync(spec);
            var loaded = rooms.First(r => r.Id == room.Id);
            return _mapper.Map<RoomResultDto>(loaded);
          
        }

        public async Task<IEnumerable<RoomResultDto>> GetRoomsInWardAsync(int wardId)
        {
            var wardRepo = _unitOfWork.GetRepository<Ward, int>();
            if (await wardRepo.GetByIdAsync(wardId) is null)
                throw new NotFoundException("Ward", wardId);

            var roomRepo = _unitOfWork.GetRepository<Room, int>();
            var rooms = await roomRepo.GetAllAsync(new RoomsByWardSpecification(wardId));
            return _mapper.Map<IEnumerable<RoomResultDto>>(rooms);
        }

        Task<WardWithDetailsResultDto> IWardService.GetWardByIdAsync(int wardId)
        {
            throw new NotImplementedException();
        }
    }

}
