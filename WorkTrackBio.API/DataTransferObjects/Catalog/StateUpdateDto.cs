using WorkTrackBio.API.Data.Models;

namespace WorkTrackBio.API.DataTransferObjects.Catalog;

public class StateUpdateDto
{
    public int Id { get; set; }
    public string StateName { get; set; } = string.Empty;
    public StateType StateType { get; set; }
    public string? Description { get; set; }
}