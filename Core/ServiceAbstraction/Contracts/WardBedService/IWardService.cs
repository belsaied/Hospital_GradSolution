using Shared.Dtos.WardBedModule.RoomsDtos;
using Shared.Dtos.WardBedModule.WardDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Abstraction.Contracts.WardBedService
{
    public interface IWardService
    {
        Task<WardResultDto> CreateWardAsync(CreateWardDto dto);
        Task<IEnumerable<WardOccupancySummaryDto>> GetAllWardsWithOccupancyAsync();
        Task<WardWithDetailsResultDto> GetWardByIdAsync(int wardId);
        Task<WardResultDto> UpdateWardAsync(int wardId, UpdateWardDto dto);
        Task<RoomResultDto> AddRoomToWardAsync(int wardId, CreateRoomDto dto);
        Task<IEnumerable<RoomResultDto>> GetRoomsInWardAsync(int wardId);

    }
}
