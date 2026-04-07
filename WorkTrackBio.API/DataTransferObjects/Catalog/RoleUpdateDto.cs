namespace WorkTrackBio.API.DataTransferObjects.Catalog
{
    public class RoleUpdateDto
    {
        public string RoleName { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}