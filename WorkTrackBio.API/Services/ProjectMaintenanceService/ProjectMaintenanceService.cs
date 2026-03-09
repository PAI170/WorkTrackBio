using AutoMapper;
using WorkTrackBio.API.DataTransferObjects.ProjectMaintenance;
using WorkTrackBio.API.Data.Models;
using WorkTrackBio.API.Repositories.ProjectMaintenanceRepository;
using WorkTrackBio.API.Repositories.ProjectRepository;
using WorkTrackBio.API.Repositories.EmployeeInfoRepository;
using WorkTrackBio.API.Repositories.StateRepository;
using WorkTrackBio.API.Common;

namespace WorkTrackBio.API.Services.ProjectMaintenanceService
{
    public class ProjectMaintenanceService : IProjectMaintenanceService
    {
        private readonly IProjectMaintenanceRepository _projectMaintenanceRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IEmployeeInfoRepository _employeeInfoRepository;
        private readonly IStateRepository _stateRepository;
        private readonly IMapper _mapper;

        public ProjectMaintenanceService(
            IProjectMaintenanceRepository projectMaintenanceRepository,
            IProjectRepository projectRepository,
            IEmployeeInfoRepository employeeInfoRepository,
            IStateRepository stateRepository,
            IMapper mapper)
        {
            _projectMaintenanceRepository = projectMaintenanceRepository ?? throw new ArgumentNullException(nameof(projectMaintenanceRepository));
            _projectRepository = projectRepository ?? throw new ArgumentNullException(nameof(projectRepository));
            _employeeInfoRepository = employeeInfoRepository ?? throw new ArgumentNullException(nameof(employeeInfoRepository));
            _stateRepository = stateRepository ?? throw new ArgumentNullException(nameof(stateRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<ApiResponse<IEnumerable<ProjectMaintenanceDataTransferObject>>> GetAllProjectMaintenancesAsync()
        {
            var maintenances = await _projectMaintenanceRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<ProjectMaintenanceDataTransferObject>>(maintenances);
        }

        public async Task<ApiResponse<ProjectMaintenanceDataTransferObject>> GetProjectMaintenanceByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("El ID debe ser mayor que 0", nameof(id));

            var maintenance = await _projectMaintenanceRepository.GetByIdAsync(id);
            return _mapper.Map<ProjectMaintenanceDataTransferObject>(maintenance);
        }

        public async Task<ApiResponse<IEnumerable<ProjectMaintenanceDataTransferObject>>> GetProjectMaintenancesByProjectAsync(int projectId)
        {
            if (projectId <= 0)
                throw new ArgumentException("El ID del proyecto debe ser mayor que 0", nameof(projectId));

            // Verificar que el proyecto exista
            var projectExists = await _projectRepository.GetByIdAsync(projectId);
            if (projectExists == null)
                throw new ArgumentException($"No existe un proyecto con ID {projectId}", nameof(projectId));

            var maintenances = await _projectMaintenanceRepository.GetByProjectAsync(projectId);
            return _mapper.Map<IEnumerable<ProjectMaintenanceDataTransferObject>>(maintenances);
        }

        public async Task<ApiResponse<IEnumerable<ProjectMaintenanceDataTransferObject>>> GetProjectMaintenancesByEmployeeAsync(int employeeId)
        {
            if (employeeId <= 0)
                throw new ArgumentException("El ID del empleado debe ser mayor que 0", nameof(employeeId));

            // Verificar que el empleado exista
            var employeeExists = await _employeeInfoRepository.GetByIdAsync(employeeId);
            if (employeeExists == null)
                throw new ArgumentException($"No existe un empleado con ID {employeeId}", nameof(employeeId));

            var maintenances = await _projectMaintenanceRepository.GetByEmployeeAsync(employeeId);
            return _mapper.Map<IEnumerable<ProjectMaintenanceDataTransferObject>>(maintenances);
        }

        public async Task<ApiResponse<IEnumerable<ProjectMaintenanceDataTransferObject>>> GetProjectMaintenancesByStateAsync(int stateId)
        {
            if (stateId <= 0)
                throw new ArgumentException("El ID del estado debe ser mayor que 0", nameof(stateId));

            // Verificar que el estado exista
            var stateExists = await _stateRepository.GetByIdAsync(stateId);
            if (stateExists == null)
                throw new ArgumentException($"No existe un estado con ID {stateId}", nameof(stateId));

            var maintenances = await _projectMaintenanceRepository.GetByStateAsync(stateId);
            return _mapper.Map<IEnumerable<ProjectMaintenanceDataTransferObject>>(maintenances);
        }

        public async Task<ApiResponse<ProjectMaintenanceDataTransferObject>> CreateProjectMaintenanceAsync(CreateProjectMaintenanceDataTransferObject createDto)
        {
            if (createDto == null)
                return ApiResponse<ProjectMaintenanceDataTransferObject>.ErrorResponse("Mantenimiento realizado requerido", 400);

            // Verificar que el proyecto exista
            var projectExists = await _projectRepository.GetByIdAsync(createDto.IdProject);
            if (projectExists == null)
                return ApiResponse<ProjectMaintenanceDataTransferObject>.ErrorResponse("No existe el proyecto indicado", 400);

            // Verificar que el empleado exista
            var employeeExists = await _employeeInfoRepository.GetByIdAsync(createDto.MadeById);
            if (employeeExists == null)
                return ApiResponse<ProjectMaintenanceDataTransferObject>.ErrorResponse("No existe el usuario indicado", 400);

            // Verificar que el estado exista
            var stateExists = await _stateRepository.GetByIdAsync(createDto.StateId);
            if (stateExists == null)
                return ApiResponse<ProjectMaintenanceDataTransferObject>.ErrorResponse("El estado indicado no existe", 400);

            var maintenance = _mapper.Map<ProjectMaintenance>(createDto);
            maintenance.MaintenanceDate = DateTime.UtcNow;

            var createdMaintenance = await _projectMaintenanceRepository.CreateAsync(maintenance);
            var result = _mapper.Map<ProjectMaintenanceDataTransferObject>(createdMaintenance);
        }

        public async Task<ApiResponse<ProjectMaintenanceDataTransferObject>> UpdateProjectMaintenanceAsync(UpdateProjectMaintenanceDataTransferObject updateDto)
        {
            if (updateDto == null)
                return ApiResponse<ProjectMaintenanceDataTransferObject>.ErrorResponse("No existe el proyecto seleccionado", 400);

            // Verificar que el mantenimiento exista
            var existingMaintenance = await _projectMaintenanceRepository.GetByIdAsync(updateDto.Id);
            if (existingMaintenance == null)
                return null;

            // Verificar que el proyecto exista si se está actualizando
            if (updateDto.IdProject.HasValue)
            {
                var projectExists = await _projectRepository.GetByIdAsync(updateDto.IdProject.Value);
                if (projectExists == null)
                    return ApiResponse<ProjectMaintenanceDataTransferObject>.ErrorResponse($"No se encontró proyecto con ID {updateDto}");
            }

            // Verificar que el empleado exista si se está actualizando
            if (updateDto.MadeById.HasValue)
            {
                var employeeExists = await _employeeInfoRepository.GetByIdAsync(updateDto.MadeById.Value);
                if (employeeExists == null)
                    return ApiResponse<ProjectMaintenanceDataTransferObject>.ErrorResponse($"No se encontró empleado con ID {updateDto}");
            }

            // Verificar que el estado exista si se está actualizando
            if (updateDto.StateId.HasValue)
            {
                var stateExists = await _stateRepository.GetByIdAsync(updateDto.StateId.Value);
                if (stateExists == null)
                    throw new ArgumentException($"No existe un estado con ID {updateDto.StateId.Value}", nameof(updateDto.StateId));
            }

            var maintenance = _mapper.Map<ProjectMaintenance>(updateDto);
            var updatedMaintenance = await _projectMaintenanceRepository.UpdateAsync(maintenance);
            
            return updatedMaintenance != null ? _mapper.Map<ProjectMaintenanceDataTransferObject>(updatedMaintenance) : null;
        }

        public async Task<ApiResponse<bool>> DeleteProjectMaintenanceAsync(int id)
        {
            if (id <= 0)
                return false;

            // Verificar que no tenga dependencias
            var hasDependencies = await _projectMaintenanceRepository.HasDependenciesAsync(id);
            if (hasDependencies)
                throw new InvalidOperationException("No se puede eliminar el mantenimiento porque tiene dependencias");

            return await _projectMaintenanceRepository.DeleteAsync(id);
        }

        public async Task<ApiResponse<bool>> ProjectMaintenanceExistsAsync(int id)
        {
            if (id <= 0)
                return false;

            return await _projectMaintenanceRepository.ExistsByIdAsync(id);
        }
    }
}
