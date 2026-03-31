namespace WorkTrackBio.API.DataTransferObjects.Project
{
    public class CreateProjectDataTransferObject
    {
        public string ProjectName { get; set; } = string.Empty;
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public int StateId { get; set; }
    }
}
