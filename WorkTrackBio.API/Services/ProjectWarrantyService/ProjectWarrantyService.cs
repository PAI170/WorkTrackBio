using AutoMapper;
using WorkTrackBio.API.Common;
using WorkTrackBio.API.Data.Models;
using WorkTrackBio.API.DataTransferObjects.ProjectWarranty;
using WorkTrackBio.API.Repositories.ProjectWarrantyRepository;
using WorkTrackBio.API.Validators.ProjectWarrantyValidator;

namespace WorkTrackBio.API.Services.ProjectWarrantyService
{
    public class ProjectWarrantyService : IProjectWarrantyService
    {
        private readonly IProjectWarrantyRepository _projectWarrantyRepository;
        private readonly IProjectWarrantyValidator _projectWarrantyValidator;
        private readonly IMapper _mapper;

        public ProjectWarrantyService(
            IProjectWarrantyRepository projectWarrantyRepository,
            IProjectWarrantyValidator projectWarrantyValidator,
            IMapper mapper)
        {
            _projectWarrantyRepository = projectWarrantyRepository ?? throw new ArgumentNullException(nameof(projectWarrantyRepository));
            _projectWarrantyValidator = projectWarrantyValidator ?? throw new ArgumentNullException(nameof(projectWarrantyValidator));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<ApiResponse<IEnumerable<ProjectWarrantyDataTransferObject>>> GetAllProjectWarrantiesAsync()
        {
            try
            {
                var projectWarranties = await _projectWarrantyRepository.GetAllAsync();
                var projectWarrantyDtos = _mapper.Map<IEnumerable<ProjectWarrantyDataTransferObject>>(projectWarranties);

                return ApiResponse<IEnumerable<ProjectWarrantyDataTransferObject>>.SuccessResponse(
                    projectWarrantyDtos,
                    $"Se encontraron {projectWarrantyDtos.Count()} garantía(s) de proyecto");
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<ProjectWarrantyDataTransferObject>>.ErrorResponse(
                    $"Error al obtener las garantías de proyectos: {ex.Message}");
            }
        }

        public async Task<ApiResponse<ProjectWarrantyDataTransferObject>> GetProjectWarrantyByIdAsync(int id)
        {
            try
            {
                if (id <= 0)
                    return ApiResponse<ProjectWarrantyDataTransferObject>.ErrorResponse("El ID debe ser mayor que 0");

                var projectWarranty = await _projectWarrantyRepository.GetByIdAsync(id);
                if (projectWarranty == null)
                    return ApiResponse<ProjectWarrantyDataTransferObject>.ErrorResponse($"No se encontró una garantía de proyecto con ID {id}");

                var projectWarrantyDto = _mapper.Map<ProjectWarrantyDataTransferObject>(projectWarranty);

                return ApiResponse<ProjectWarrantyDataTransferObject>.SuccessResponse(
                    projectWarrantyDto,
                    "Garantía de proyecto encontrada exitosamente");
            }
            catch (Exception ex)
            {
                return ApiResponse<ProjectWarrantyDataTransferObject>.ErrorResponse(
                    $"Error al obtener la garantía de proyecto: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<ProjectWarrantyDataTransferObject>>> GetProjectWarrantiesByProjectAsync(int projectId)
        {
            try
            {
                if (projectId <= 0)
                    return ApiResponse<IEnumerable<ProjectWarrantyDataTransferObject>>.ErrorResponse("El ID del proyecto debe ser mayor que 0");

                var projectWarranties = await _projectWarrantyRepository.GetByProjectAsync(projectId);
                var projectWarrantyDtos = _mapper.Map<IEnumerable<ProjectWarrantyDataTransferObject>>(projectWarranties);

                return ApiResponse<IEnumerable<ProjectWarrantyDataTransferObject>>.SuccessResponse(
                    projectWarrantyDtos,
                    $"Se encontraron {projectWarrantyDtos.Count()} garantía(s) de proyecto para el proyecto {projectId}");
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<ProjectWarrantyDataTransferObject>>.ErrorResponse(
                    $"Error al obtener las garantías de proyecto: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<ProjectWarrantyDataTransferObject>>> GetProjectWarrantiesByEmployeeAsync(int employeeId)
        {
            try
            {
                if (employeeId <= 0)
                    return ApiResponse<IEnumerable<ProjectWarrantyDataTransferObject>>.ErrorResponse("El ID del empleado debe ser mayor que 0");

                var projectWarranties = await _projectWarrantyRepository.GetByEmployeeAsync(employeeId);
                var projectWarrantyDtos = _mapper.Map<IEnumerable<ProjectWarrantyDataTransferObject>>(projectWarranties);

                return ApiResponse<IEnumerable<ProjectWarrantyDataTransferObject>>.SuccessResponse(
                    projectWarrantyDtos,
                    $"Se encontraron {projectWarrantyDtos.Count()} garantía(s) de proyecto para el empleado {employeeId}");
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<ProjectWarrantyDataTransferObject>>.ErrorResponse(
                    $"Error al obtener las garantías de proyecto del empleado: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<ProjectWarrantyDataTransferObject>>> GetProjectWarrantiesByStateAsync(int stateId)
        {
            try
            {
                if (stateId <= 0)
                    return ApiResponse<IEnumerable<ProjectWarrantyDataTransferObject>>.ErrorResponse("El ID del estado debe ser mayor que 0");

                var projectWarranties = await _projectWarrantyRepository.GetByStateAsync(stateId);
                var projectWarrantyDtos = _mapper.Map<IEnumerable<ProjectWarrantyDataTransferObject>>(projectWarranties);

                return ApiResponse<IEnumerable<ProjectWarrantyDataTransferObject>>.SuccessResponse(
                    projectWarrantyDtos,
                    $"Se encontraron {projectWarrantyDtos.Count()} garantía(s) de proyecto para el estado {stateId}");
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<ProjectWarrantyDataTransferObject>>.ErrorResponse(
                    $"Error al obtener las garantías de proyecto del estado: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<ProjectWarrantyDataTransferObject>>> GetProjectWarrantiesByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            try
            {
                if (startDate > endDate)
                    return ApiResponse<IEnumerable<ProjectWarrantyDataTransferObject>>.ErrorResponse("La fecha de inicio no puede ser posterior a la fecha de fin");

                var projectWarranties = await _projectWarrantyRepository.GetByDateRangeAsync(startDate, endDate);
                var projectWarrantyDtos = _mapper.Map<IEnumerable<ProjectWarrantyDataTransferObject>>(projectWarranties);

                return ApiResponse<IEnumerable<ProjectWarrantyDataTransferObject>>.SuccessResponse(
                    projectWarrantyDtos,
                    $"Se encontraron {projectWarrantyDtos.Count()} garantía(s) de proyecto entre {startDate:dd/MM/yyyy} y {endDate:dd/MM/yyyy}");
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<ProjectWarrantyDataTransferObject>>.ErrorResponse(
                    $"Error al obtener las garantías de proyecto por rango de fechas: {ex.Message}");
            }
        }

        public async Task<ApiResponse<IEnumerable<ProjectWarrantyDataTransferObject>>> GetProjectWarrantiesByProjectAndStateAsync(int projectId, int stateId)
        {
            try
            {
                if (projectId <= 0)
                    return ApiResponse<IEnumerable<ProjectWarrantyDataTransferObject>>.ErrorResponse("El ID del proyecto debe ser mayor que 0");

                if (stateId <= 0)
                    return ApiResponse<IEnumerable<ProjectWarrantyDataTransferObject>>.ErrorResponse("El ID del estado debe ser mayor que 0");

                var projectWarranties = await _projectWarrantyRepository.GetByProjectAndStateAsync(projectId, stateId);
                var projectWarrantyDtos = _mapper.Map<IEnumerable<ProjectWarrantyDataTransferObject>>(projectWarranties);

                return ApiResponse<IEnumerable<ProjectWarrantyDataTransferObject>>.SuccessResponse(
                    projectWarrantyDtos,
                    $"Se encontraron {projectWarrantyDtos.Count()} garantía(s) de proyecto para el proyecto {projectId} y estado {stateId}");
            }
            catch (Exception ex)
            {
                return ApiResponse<IEnumerable<ProjectWarrantyDataTransferObject>>.ErrorResponse(
                    $"Error al obtener las garantías de proyecto por proyecto y estado: {ex.Message}");
            }
        }

        public async Task<ApiResponse<ProjectWarrantyDataTransferObject>> CreateProjectWarrantyAsync(CreateProjectWarrantyDataTransferObject createDto)
        {
            try
            {
                // Validar el DTO
                var validationResult = await _projectWarrantyValidator.ValidateCreateAsync(createDto);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                    return ApiResponse<ProjectWarrantyDataTransferObject>.ErrorResponse("Error de validación", 400, errors);
                }

                // Mapear DTO a modelo
                var projectWarranty = _mapper.Map<ProjectWarranty>(createDto);

                // Crear la garantía
                var createdProjectWarranty = await _projectWarrantyRepository.CreateAsync(projectWarranty);
                var projectWarrantyDto = _mapper.Map<ProjectWarrantyDataTransferObject>(createdProjectWarranty);

                return ApiResponse<ProjectWarrantyDataTransferObject>.SuccessResponse(
                    projectWarrantyDto,
                    "Garantía de proyecto creada exitosamente");
            }
            catch (Exception ex)
            {
                return ApiResponse<ProjectWarrantyDataTransferObject>.ErrorResponse(
                    $"Error al crear la garantía de proyecto: {ex.Message}");
            }
        }

        public async Task<ApiResponse<ProjectWarrantyDataTransferObject>> UpdateProjectWarrantyAsync(int id, UpdateProjectWarrantyDataTransferObject updateDto)
        {
            try
            {
                if (id <= 0)
                    return ApiResponse<ProjectWarrantyDataTransferObject>.ErrorResponse("El ID debe ser mayor que 0");

                // Obtener la garantía existente
                var existingProjectWarranty = await _projectWarrantyRepository.GetByIdAsync(id);
                if (existingProjectWarranty == null)
                    return ApiResponse<ProjectWarrantyDataTransferObject>.ErrorResponse($"No se encontró una garantía de proyecto con ID {id}");

                // Validar el DTO
                var validationResult = await _projectWarrantyValidator.ValidateUpdateAsync(id, updateDto);
                if (!validationResult.IsValid)
                {
                    var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                    return ApiResponse<ProjectWarrantyDataTransferObject>.ErrorResponse("Error de validación", 400, errors);
                }

                // Crear un HashSet para rastrear qué campos fueron enviados explícitamente
                var sentFields = new HashSet<string>();
                var updateDtoType = typeof(UpdateProjectWarrantyDataTransferObject);
                foreach (var property in updateDtoType.GetProperties())
                {
                    if (property.GetValue(updateDto) != null)
                    {
                        sentFields.Add(property.Name);
                    }
                }

                // Aplicar actualizaciones parciales
                bool hasChanges = false;

                if (sentFields.Contains("IdProject"))
                {
                    existingProjectWarranty.IdProject = updateDto.IdProject!.Value;
                    hasChanges = true;
                }

                if (sentFields.Contains("WarrantyDate"))
                {
                    existingProjectWarranty.WarrantyDate = updateDto.WarrantyDate!.Value;
                    hasChanges = true;
                }

                if (sentFields.Contains("WarrantyDescription"))
                {
                    existingProjectWarranty.WarrantyDescription = updateDto.WarrantyDescription ?? string.Empty;
                    hasChanges = true;
                }

                if (sentFields.Contains("MadeById"))
                {
                    existingProjectWarranty.MadeById = updateDto.MadeById!.Value;
                    hasChanges = true;
                }

                if (sentFields.Contains("WarrantyCost"))
                {
                    existingProjectWarranty.WarrantyCost = updateDto.WarrantyCost;
                    hasChanges = true;
                }

                if (sentFields.Contains("StateId"))
                {
                    existingProjectWarranty.StateId = updateDto.StateId!.Value;
                    hasChanges = true;
                }

                if (!hasChanges)
                {
                    return ApiResponse<ProjectWarrantyDataTransferObject>.ErrorResponse("No se detectaron cambios para actualizar");
                }

                // Actualizar la garantía
                var updatedProjectWarranty = await _projectWarrantyRepository.UpdateAsync(existingProjectWarranty);
                var projectWarrantyDto = _mapper.Map<ProjectWarrantyDataTransferObject>(updatedProjectWarranty);

                return ApiResponse<ProjectWarrantyDataTransferObject>.SuccessResponse(
                    projectWarrantyDto,
                    "Garantía de proyecto actualizada exitosamente");
            }
            catch (Exception ex)
            {
                return ApiResponse<ProjectWarrantyDataTransferObject>.ErrorResponse(
                    $"Error al actualizar la garantía de proyecto: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> DeleteProjectWarrantyAsync(int id)
        {
            try
            {
                if (id <= 0)
                    return ApiResponse<bool>.ErrorResponse("El ID debe ser mayor que 0");

                // Verificar que la garantía existe
                var existingProjectWarranty = await _projectWarrantyRepository.GetByIdAsync(id);
                if (existingProjectWarranty == null)
                    return ApiResponse<bool>.ErrorResponse($"No se encontró una garantía de proyecto con ID {id}");

                // Verificar dependencias
                var hasDependencies = await _projectWarrantyRepository.HasDependenciesAsync(id);
                if (hasDependencies)
                    return ApiResponse<bool>.ErrorResponse("No se puede eliminar la garantía de proyecto porque tiene dependencias (registros de auditoría)");

                // Eliminar la garantía
                var deleted = await _projectWarrantyRepository.DeleteAsync(id);
                if (!deleted)
                    return ApiResponse<bool>.ErrorResponse("Error al eliminar la garantía de proyecto");

                return ApiResponse<bool>.SuccessResponse(
                    true,
                    "Garantía de proyecto eliminada exitosamente");
            }
            catch (Exception ex)
            {
                return ApiResponse<bool>.ErrorResponse(
                    $"Error al eliminar la garantía de proyecto: {ex.Message}");
            }
        }
    }
}
