namespace WorkTrackBio.API.DataTransferObjects.State
{
    public class UpdateStateDataTransferObject
    {
        public int Id { get; set; }
        public string StateName { get; set; } = string.Empty;
        public string StateType { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
