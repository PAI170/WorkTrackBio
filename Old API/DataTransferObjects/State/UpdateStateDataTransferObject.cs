namespace WorkTrackBio.API.DataTransferObjects.State
{
    public class UpdateStateDataTransferObject
    {
        public int Id { get; set; }
        public string? StateName { get; set; }
        public string? StateType { get; set; }
        public string? Description { get; set; }
    }
}
