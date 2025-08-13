namespace WTB.API.Data.Repositories.Models
{
    /// <summary>
    /// Resumen de trabajo por empleado
    /// </summary>
    public class EmployeeWorkSummary
    {
        public int EmployeeId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public decimal TotalHours { get; set; }
        public int TotalProjects { get; set; }
        public decimal TotalCost { get; set; }
    }

    /// <summary>
    /// Estadísticas de empleados
    /// </summary>
    public class EmployeeStatistics
    {
        public int TotalEmployees { get; set; }
        public int ActiveEmployees { get; set; }
        public int InactiveEmployees { get; set; }
        public int EmployeesWithFingerPrint { get; set; }
        public decimal AverageCostPerHour { get; set; }
        public int AverageAge { get; set; }
    }
}
