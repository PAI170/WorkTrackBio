namespace WorkTrackBio.API.DataTransferObjects.Project
{

    public class UpdateProjectDataTransferObject
    {
        public int Id { get; set; }
        public string? ProjectName { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public int? StateId { get; set; }
    }
}
