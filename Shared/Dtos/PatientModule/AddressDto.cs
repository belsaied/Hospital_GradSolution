namespace Shared.Dtos.PatientModule
{
    public record AddressDto
    {
        public string Street { get; init; } = string.Empty;
        public string City { get; init; } = string.Empty;
        public string Country { get; init; } = string.Empty;
        public string PostalCode { get; init; } = string.Empty;
    }
}
