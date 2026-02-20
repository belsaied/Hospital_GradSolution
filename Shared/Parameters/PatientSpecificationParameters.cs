using Domain.Models.Enums;

namespace Shared.Parameters
{
    public class PatientSpecificationParameters
    {
        private const int DefaultPageSize = 5;
        private const int MaxPageSize = 20;

        public string? Search { get; set; }
        public PatientStatus? Status { get; set; }
        public int PageIndex { get; set; } = 1;

        private int _pageSize = DefaultPageSize;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
        }
    }
}
