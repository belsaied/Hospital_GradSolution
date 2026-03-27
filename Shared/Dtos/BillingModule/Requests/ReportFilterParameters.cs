namespace Shared.Dtos.BillingModule.Requests
{
    public record ReportFilterParameters
    {
        public DateOnly? StartDate { get; init; }
        public DateOnly? EndDate { get; init; }
        public int? DepartmentId { get; init; }
        public int? DoctorId { get; init; }
    }
}
