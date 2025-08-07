@echo off
echo Creating LoginDTO.cs...
(
echo using System.ComponentModel.DataAnnotations;
echo.
echo namespace WTB.API.Models.DTOs.Auth
echo {
echo     public class LoginDTO
echo     {
echo         [Required]
echo         [EmailAddress]
echo         [StringLength(100^)]
echo         public string Email { get; set; } = string.Empty;
echo.
echo         [Required]
echo         [StringLength(255^)]
echo         public string Password { get; set; } = string.Empty;
echo     }
echo.
echo     public class LoginResponseDTO
echo     {
echo         public string Token { get; set; } = string.Empty;
echo         public string RefreshToken { get; set; } = string.Empty;
echo         public DateTime ExpiresAt { get; set; }
echo         public UserInfoDTO User { get; set; } = new();
echo     }
echo.
echo     public class UserInfoDTO
echo     {
echo         public int Id { get; set; }
echo         public string Email { get; set; } = string.Empty;
echo         public string FirstName { get; set; } = string.Empty;
echo         public string LastName { get; set; } = string.Empty;
echo         public string RoleName { get; set; } = string.Empty;
echo         public DateTime LastLogin { get; set; }
echo     }
echo }
) > "Models\DTOs\Auth\LoginDTO.cs"

echo Creating ReportsDto.cs...
(
echo using System.ComponentModel.DataAnnotations;
echo using WTB.API.Models.DTOs.Common;
echo.
echo namespace WTB.API.Models.DTOs.Reports
echo {
echo     public class ReportFilterDTO
echo     {
echo         public DateTime? StartDate { get; set; }
echo         public DateTime? EndDate { get; set; }
echo         public int? EmployeeId { get; set; }
echo         public int? ProjectId { get; set; }
echo         public int? StateId { get; set; }
echo         public string? EmployeeName { get; set; }
echo         public string? ProjectName { get; set; }
echo     }
echo.
echo     public class EmployeeReportDTO
echo     {
echo         public int EmployeeId { get; set; }
echo         public string EmployeeName { get; set; } = string.Empty;
echo         public string DocumentNumber { get; set; } = string.Empty;
echo         public string DocumentTypeName { get; set; } = string.Empty;
echo         public DateTime Birthday { get; set; }
echo         public int Age =^> DateTime.Today.Year - Birthday.Year;
echo         public string PhoneNumber { get; set; } = string.Empty;
echo         public string Address { get; set; } = string.Empty;
echo         public decimal? CostPerHour { get; set; }
echo         public string StateName { get; set; } = string.Empty;
echo         public DateTime RegisterDate { get; set; }
echo         public int TotalProjects { get; set; }
echo         public int TotalAssistances { get; set; }
echo         public decimal TotalHours { get; set; }
echo         public decimal TotalCost { get; set; }
echo         public List^<EmployeeProjectSummaryDTO^> ProjectSummaries { get; set; } = new();
echo     }
echo }
) > "Models\DTOs\Reports\ReportsDto.cs"

echo Files created successfully!
pause
