namespace WTB.API.Models.DTOs.Device
{
    /// <summary>
    /// DTO para mostrar detalles completos de un dispositivo
    /// </summary>
    public record DeviceDetailsDto(
        int Id,
        string DeviceId,
        string DeviceName,
        int ProjectId,
        string? Location,
        string DeviceType,
        bool IsActive,
        DateTime CreatedDate,
        DateTime? ModifiedDate,
        DateTime? LastSeen,
        string? Notes,
        string? IpAddress,
        string? SerialNumber,
        string? FirmwareVersion,
        
        // Información del proyecto relacionado
        string ProjectName,
        string ProjectState,
        
        // Información de auditoría
        int? CreatedById,
        int? ModifiedById,
        string? CreatedBy,
        string? ModifiedBy
    );
}
