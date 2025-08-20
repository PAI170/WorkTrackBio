namespace WorkTrackBio.API.DataTransferObjects.State
{
    /// <summary>
    /// DTO para crear un nuevo State
    /// </summary>
    public class CreateStateDataTransferObject
    {
        public string StateName { get; set; } = string.Empty;
        public string StateType { get; set; } = string.Empty;
        public string? Description { get; set; }
    }
}
