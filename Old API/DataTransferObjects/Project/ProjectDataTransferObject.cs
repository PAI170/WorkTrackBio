namespace WorkTrackBio.API.DataTransferObjects.Project
{
    public class ProjectDataTransferObject
    {
        public int Id { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
        public int StateId { get; set; }
        public string StateName { get; set; } = string.Empty;
    }
}
