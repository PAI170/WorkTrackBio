namespace WorkTrackBio.API.DataTransferObjects.Catalog
{
    public class RoleResponseDto
    {
        public int Id { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public int UserCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}