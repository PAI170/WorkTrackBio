namespace WorkTrackBio.API.DataTransferObjects.Role
{
    public class RoleDataTransferObject
    {
        public int Id { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
