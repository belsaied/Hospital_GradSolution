using Domain.Models.Enums.DoctorEnums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Shared.Dtos.DoctorModule.DoctorDtos
{
    public record UpdateDoctorDto
    {
        [MaxLength(100)]
        public string? FirstName { get; init; }

        [MaxLength(100)]
        public string? LastName { get; init; }

        [Phone]
        public string? Phone { get; init; }

        [EmailAddress]
        public string? Email { get; init; }

        [MaxLength(100)]
        public string? Specialization { get; init; }

        public int? DepartmentId { get; init; }

        [Range(0, 60)]
        public int? YearsOfExperience { get; init; }

        [Range(0, 100000)]
        public decimal? ConsultationFee { get; init; }

        [MaxLength(1000)]
        public string? Bio { get; init; }

        public string? PictureUrl { get; init; }

        public DoctorStatus? Status { get; init; }
    }
}
