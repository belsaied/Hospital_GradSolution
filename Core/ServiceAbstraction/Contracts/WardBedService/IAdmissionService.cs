using Shared.Dtos.WardBedModule.AdmissionDtos;
using System;
using System.Collections.Generic;
using System.Text;

namespace Services.Abstraction.Contracts.WardBedService
{
    public interface IAdmissionService
    {
        Task<AdmissionResultDto> AdmitPatientAsync(CreateAdmissionDto dto);
        Task<AdmissionResultDto> GetAdmissionByIdAsync(int admissionId);
        Task<IEnumerable<AdmissionResultDto>> GetPatientAdmissionHistoryAsync(int patientId);
        Task<IEnumerable<AdmissionResultDto>> GetActiveAdmissionsAsync();
        Task<AdmissionResultDto> DischargePatientAsync(int admissionId, DischargeDto dto);
        Task<AdmissionResultDto> TransferPatientAsync(int admissionId, TransferBedDto dto);

    }
}
