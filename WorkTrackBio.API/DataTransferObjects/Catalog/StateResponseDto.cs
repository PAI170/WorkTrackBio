namespace WorkTrackBio.API.DataTransferObjects.Catalog
{
    public class StateResponseDto
    {
        public int Id { get; set; }
        public string StateName { get; set; } = string.Empty;
        public string StateType { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
