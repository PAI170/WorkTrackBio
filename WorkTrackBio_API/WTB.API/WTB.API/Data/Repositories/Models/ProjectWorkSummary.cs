namespace WTB.API.Data.Repositories.Models
{
    /// <summary>
    /// Resumen de trabajo por proyecto
    /// </summary>
    public class ProjectWorkSummary
    {
        public int ProjectId { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public string ClientName { get; set; } = string.Empty;
        public decimal TotalHours { get; set; }
        public int TotalEmployees { get; set; }
        public decimal TotalCost { get; set; }
        public decimal ProgressPercentage { get; set; }
        public DateTime? EstimatedEndDate { get; set; }
    }

    /// <summary>
    /// Estadísticas de proyectos
    /// </summary>
    public class ProjectStatistics
    {
        public int TotalProjects { get; set; }
        public int ActiveProjects { get; set; }
        public int CompletedProjects { get; set; }
        public int PendingProjects { get; set; }
        public decimal TotalBudget { get; set; }
        public decimal TotalSpent { get; set; }
        public decimal AverageProjectDuration { get; set; }
        public int TotalEmployees { get; set; }
    }

    /// <summary>
    /// Estadísticas de asistencia
    /// </summary>
    public class AssistanceStatistics
    {
        public int TotalAssistances { get; set; }
        public int CompletedAssistances { get; set; }
        public int PendingAssistances { get; set; }
        public decimal TotalHours { get; set; }
        public decimal AverageHoursPerDay { get; set; }
        public int TotalEmployees { get; set; }
        public int TotalProjects { get; set; }
        public decimal TotalCost { get; set; }
    }

    /// <summary>
    /// Resumen de trabajadores con mayor tiempo
    /// </summary>
    public class TopWorkerSummary
    {
        public int EmployeeId { get; set; }
        public string EmployeeName { get; set; } = string.Empty;
        public decimal TotalHours { get; set; }
        public int TotalDays { get; set; }
        public decimal AverageHoursPerDay { get; set; }
        public decimal TotalCost { get; set; }
    }
}
