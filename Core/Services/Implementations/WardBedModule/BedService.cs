using AutoMapper;
using Domain.Contracts;
using Domain.Models.Enums.WardBedEnums;
using Domain.Models.WardBedModule;
using Services.Abstraction.Contracts.WardBedService;
using Services.Exceptions;
using Services.Specifications.WardBedModule;
using Shared.Dtos.WardBedModule.BedDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Implementations.WardBedModule
{
    public class BedService(IUnitOfWork _unitOfWork, IMapper _mapper) : IBedService
    {
        public async Task<BedResultDto> AddBedToRoomAsync(int roomId, CreateBedDto dto)
        {
            var roomRepo = _unitOfWork.GetRepository<Room, int>();
            var room = await roomRepo.GetByIdAsync(roomId);
            if (room is null) throw new RoomNotFoundException(roomId);

            var bedRepo = _unitOfWork.GetRepository<Bed, int>();

            // BR: BedNumber must be unique within the room
            var existingBeds = await bedRepo.GetAllAsync(new BedsByRoomSpecification(roomId));
            if (existingBeds.Any(b => b.BedNumber == dto.BedNumber))
                throw new ConflictException($"Bed number '{dto.BedNumber}' already exists in this room.");

            var bed = _mapper.Map<Bed>(dto);
            bed.RoomId = roomId;
            bed.Status = BedStatus.Available;

            await bedRepo.AddAsync(bed);
            await _unitOfWork.SaveChangesAsync();

            var beds = await bedRepo.GetAllAsync(new BedsByRoomSpecification(roomId));
            var saved = beds.First(b => b.Id == bed.Id);
            return _mapper.Map<BedResultDto>(saved);
        }

        public async Task<IEnumerable<BedResultDto>> GetBedsInRoomAsync(int roomId)
        {
            var roomRepo = _unitOfWork.GetRepository<Room, int>();
            if (await roomRepo.GetByIdAsync(roomId) is null)
                throw new RoomNotFoundException(roomId);

            var bedRepo = _unitOfWork.GetRepository<Bed, int>();
            var beds = await bedRepo.GetAllAsync(new BedsByRoomSpecification(roomId));
            return _mapper.Map<IEnumerable<BedResultDto>>(beds);
        }

        public async Task<BedResultDto> UpdateBedStatusAsync(int bedId, UpdateBedStatusDto dto)
        {
            var bedRepo = _unitOfWork.GetRepository<Bed, int>();
            var bed = await bedRepo.GetByIdAsync(bedId);
            if (bed is null) throw new BedNotFoundException(bedId);

            // BR: Cannot manually set Occupied — only Admission does that
            if (dto.Status == BedStatus.Occupied)
                throw new BusinessRuleException("Bed status cannot be manually set to Occupied. Use Admission instead.");

            bed.Status = dto.Status;
            bed.Notes = dto.Notes ?? bed.Notes;

            bedRepo.Update(bed);
            await _unitOfWork.SaveChangesAsync();

            var beds = await bedRepo.GetAllAsync(new BedsByRoomSpecification(bed.RoomId));
            var updated = beds.First(b => b.Id == bedId);
            return _mapper.Map<BedResultDto>(updated);
        }

        public async Task<IEnumerable<BedAvailabilityResultDto>> GetAvailableBedsAsync(
            string? wardType = null, string? bedType = null)
        {
            WardType? parsedWardType = null;
            BedType? parsedBedType = null;

            if (!string.IsNullOrEmpty(wardType) &&
                Enum.TryParse<WardType>(wardType, true, out var wt))
                parsedWardType = wt;

            if (!string.IsNullOrEmpty(bedType) &&
                Enum.TryParse<BedType>(bedType, true, out var bt))
                parsedBedType = bt;

            var bedRepo = _unitOfWork.GetRepository<Bed, int>();
            var beds = await bedRepo.GetAllAsync(
                new AvailableBedSpecification(parsedWardType, parsedBedType));

            return _mapper.Map<IEnumerable<BedAvailabilityResultDto>>(beds);
        }
    }
}
