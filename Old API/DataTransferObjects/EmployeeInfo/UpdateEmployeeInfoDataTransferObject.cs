namespace WorkTrackBio.API.DataTransferObjects.EmployeeInfo
{
    public class UpdateEmployeeInfoDataTransferObject
    {
        public int Id { get; set; }
        public string? DocumentNumber { get; set; }
        public int? DocumentTypeId { get; set; }
        public DateOnly? DocumentExpire { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? EmergencyContact { get; set; }
        public string? EmergencyContactPhoneNumber { get; set; }
        public DateOnly? Birthday { get; set; }
        public decimal? CostPerHour { get; set; }
        public int? StateId { get; set; }
        public string? Address { get; set; }
        public string? IBAN { get; set; }
    }
}
