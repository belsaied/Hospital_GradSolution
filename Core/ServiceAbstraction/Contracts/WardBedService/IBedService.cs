using Domain.Models.Enums.WardBedEnums;
using Shared.Dtos.WardBedModule.BedDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Abstraction.Contracts.WardBedService
{
    public interface IBedService
    {
        Task<BedResultDto> AddBedToRoomAsync(int roomId, CreateBedDto dto);
        Task<IEnumerable<BedResultDto>> GetBedsInRoomAsync(int roomId);
        Task<BedResultDto> UpdateBedStatusAsync(int bedId, UpdateBedStatusDto dto);
        Task<IEnumerable<BedAvailabilityResultDto>> GetAvailableBedsAsync(string? wardType = null, string? bedType = null);

    }
}
