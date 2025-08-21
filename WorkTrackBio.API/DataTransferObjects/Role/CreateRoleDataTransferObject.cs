namespace WorkTrackBio.API.DataTransferObjects.Role
{
    public class CreateRoleDataTransferObject
    {
        public string RoleName { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
