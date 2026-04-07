namespace WorkTrackBio.API.DataTransferObjects.Catalog
{
    public class RoleCreateDto
    {
        public string RoleName { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}