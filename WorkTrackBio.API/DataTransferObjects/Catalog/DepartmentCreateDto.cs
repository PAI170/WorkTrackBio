namespace WorkTrackBio.API.DataTransferObjects.Catalog
{
    public class DepartmentCreateDto
    {
        public string DepartmentName { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}