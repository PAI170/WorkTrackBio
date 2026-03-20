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
                var projectWarranties = await _projectWarrantyRepository.GetAllAsync();
                var projectWarrantyDtos = _mapper.Map<IEnumerable<ProjectWarrantyDataTransferObject>>(projectWarranties);

                return ApiResponse<IEnumerable<ProjectWarrantyDataTransferObject>>.SuccessResponse(projectWarrantyDtos, $"Se encontraron {projectWarrantyDtos.Count()} garantía(s) de proyecto");
        }

        public async Task<ApiResponse<ProjectWarrantyDataTransferObject>> GetProjectWarrantyByIdAsync(int id)
        {
                if (id <= 0)
                    return ApiResponse<ProjectWarrantyDataTransferObject>.ErrorResponse("El ID debe ser mayor que 0", 400);

                var projectWarranty = await _projectWarrantyRepository.GetByIdAsync(id);
                if (projectWarranty == null)
                    return ApiResponse<ProjectWarrantyDataTransferObject>.ErrorResponse($"No se encontró una garantía de proyecto con ID {id}", 404);

                var projectWarrantyDto = _mapper.Map<ProjectWarrantyDataTransferObject>(projectWarranty);

                return ApiResponse<ProjectWarrantyDataTransferObject>.SuccessResponse(projectWarrantyDto,"Garantía de proyecto encontrada exitosamente");
        }

        public async Task<ApiResponse<IEnumerable<ProjectWarrantyDataTransferObject>>> GetProjectWarrantiesByProjectAsync(int projectId)
        {
                if (projectId <= 0)
                    return ApiResponse<IEnumerable<ProjectWarrantyDataTransferObject>>.ErrorResponse("El ID del proyecto debe ser mayor que 0", 400);

                var projectWarranties = await _projectWarrantyRepository.GetByProjectAsync(projectId);
                var projectWarrantyDtos = _mapper.Map<IEnumerable<ProjectWarrantyDataTransferObject>>(projectWarranties);

                return ApiResponse<IEnumerable<ProjectWarrantyDataTransferObject>>.SuccessResponse(projectWarrantyDtos, $"Se encontraron {projectWarrantyDtos.Count()} garantía(s) de proyecto para el proyecto {projectId}");
        }

        public async Task<ApiResponse<IEnumerable<ProjectWarrantyDataTransferObject>>> GetProjectWarrantiesByEmployeeAsync(int employeeId)
        {
                if (employeeId <= 0)
                    return ApiResponse<IEnumerable<ProjectWarrantyDataTransferObject>>.ErrorResponse("El ID del empleado debe ser mayor que 0", 400);

                var projectWarranties = await _projectWarrantyRepository.GetByEmployeeAsync(employeeId);
                var projectWarrantyDtos = _mapper.Map<IEnumerable<ProjectWarrantyDataTransferObject>>(projectWarranties);

                return ApiResponse<IEnumerable<ProjectWarrantyDataTransferObject>>.SuccessResponse(projectWarrantyDtos, $"Se encontraron {projectWarrantyDtos.Count()} garantía(s) de proyecto para el empleado {employeeId}");
        }

        public async Task<ApiResponse<IEnumerable<ProjectWarrantyDataTransferObject>>> GetProjectWarrantiesByStateAsync(int stateId)
        {
                if (stateId <= 0)
                    return ApiResponse<IEnumerable<ProjectWarrantyDataTransferObject>>.ErrorResponse("El ID del estado debe ser mayor que 0", 400);

                var projectWarranties = await _projectWarrantyRepository.GetByStateAsync(stateId);
                var projectWarrantyDtos = _mapper.Map<IEnumerable<ProjectWarrantyDataTransferObject>>(projectWarranties);

                return ApiResponse<IEnumerable<ProjectWarrantyDataTransferObject>>.SuccessResponse(projectWarrantyDtos, $"Se encontraron {projectWarrantyDtos.Count()} garantía(s) de proyecto para el estado {stateId}");
        }

        public async Task<ApiResponse<IEnumerable<ProjectWarrantyDataTransferObject>>> GetProjectWarrantiesByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
                if (startDate > endDate)
                    return ApiResponse<IEnumerable<ProjectWarrantyDataTransferObject>>.ErrorResponse("La fecha de inicio no puede ser posterior a la fecha de fin", 400);

                var projectWarranties = await _projectWarrantyRepository.GetByDateRangeAsync(startDate, endDate);
                var projectWarrantyDtos = _mapper.Map<IEnumerable<ProjectWarrantyDataTransferObject>>(projectWarranties);

                return ApiResponse<IEnumerable<ProjectWarrantyDataTransferObject>>.SuccessResponse(projectWarrantyDtos, $"Se encontraron {projectWarrantyDtos.Count()} garantía(s) de proyecto entre {startDate:dd/MM/yyyy} y {endDate:dd/MM/yyyy}");
        }

        public async Task<ApiResponse<IEnumerable<ProjectWarrantyDataTransferObject>>> GetProjectWarrantiesByProjectAndStateAsync(int projectId, int stateId)
        {
                if (projectId <= 0)
                    return ApiResponse<IEnumerable<ProjectWarrantyDataTransferObject>>.ErrorResponse("El ID del proyecto debe ser mayor que 0", 400);

                if (stateId <= 0)
                    return ApiResponse<IEnumerable<ProjectWarrantyDataTransferObject>>.ErrorResponse("El ID del estado debe ser mayor que 0", 400);

                var projectWarranties = await _projectWarrantyRepository.GetByProjectAndStateAsync(projectId, stateId);
                var projectWarrantyDtos = _mapper.Map<IEnumerable<ProjectWarrantyDataTransferObject>>(projectWarranties);

                return ApiResponse<IEnumerable<ProjectWarrantyDataTransferObject>>.SuccessResponse(projectWarrantyDtos, $"Se encontraron {projectWarrantyDtos.Count()} garantía(s) de proyecto para el proyecto {projectId} y estado {stateId}");
        }

        public async Task<ApiResponse<ProjectWarrantyDataTransferObject>> CreateProjectWarrantyAsync(CreateProjectWarrantyDataTransferObject createDto)
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

                return ApiResponse<ProjectWarrantyDataTransferObject>.SuccessResponse(projectWarrantyDto,"Garantía de proyecto creada exitosamente");
        }

        public async Task<ApiResponse<ProjectWarrantyDataTransferObject>> UpdateProjectWarrantyAsync(int id, UpdateProjectWarrantyDataTransferObject updateDto)
        {
                if (id <= 0)
                    return ApiResponse<ProjectWarrantyDataTransferObject>.ErrorResponse("El ID debe ser mayor que 0", 400);

                // Obtener la garantía existente
                var existingProjectWarranty = await _projectWarrantyRepository.GetByIdAsync(id);
                if (existingProjectWarranty == null)
                    return ApiResponse<ProjectWarrantyDataTransferObject>.ErrorResponse($"No se encontró una garantía de proyecto con ID {id}", 404);

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
                    return ApiResponse<ProjectWarrantyDataTransferObject>.ErrorResponse("No se detectaron cambios para actualizar", 400);
                }

                // Actualizar la garantía
                var updatedProjectWarranty = await _projectWarrantyRepository.UpdateAsync(existingProjectWarranty);
                var projectWarrantyDto = _mapper.Map<ProjectWarrantyDataTransferObject>(updatedProjectWarranty);

                return ApiResponse<ProjectWarrantyDataTransferObject>.SuccessResponse(projectWarrantyDto, "Garantía de proyecto actualizada exitosamente");
        }

        public async Task<ApiResponse<bool>> DeleteProjectWarrantyAsync(int id)
        {
                if (id <= 0)
                    return ApiResponse<bool>.ErrorResponse("El ID debe ser mayor que 0", 400);

                // Verificar que la garantía existe
                var existingProjectWarranty = await _projectWarrantyRepository.GetByIdAsync(id);
                if (existingProjectWarranty == null)
                    return ApiResponse<bool>.ErrorResponse($"No se encontró una garantía de proyecto con ID {id}", 404);

                // Verificar dependencias
                var hasDependencies = await _projectWarrantyRepository.HasDependenciesAsync(id);
                if (hasDependencies)
                    return ApiResponse<bool>.ErrorResponse("No se puede eliminar la garantía de proyecto porque tiene dependencias (registros de auditoría)", 409);

                // Eliminar la garantía
                var deleted = await _projectWarrantyRepository.DeleteAsync(id);
                if (!deleted)
                    return ApiResponse<bool>.ErrorResponse("Error al eliminar la garantía de proyecto", 500);

                return ApiResponse<bool>.SuccessResponse(true,"Garantía de proyecto eliminada exitosamente");
        }
    }
}
