namespace WorkTrackBio.API.DataTransferObjects.Employee
{
    public class EmployeeListDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
        public string StateName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public string? PhotoUrl { get; set; }
    }
}