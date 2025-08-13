using Microsoft.Extensions.Logging;
using WTB.API.Data.Repositories;
using WTB.API.Data.Repositories.Models;
using WTB.API.Models.DTOs.Project;
using WTB.API.Models.Entities;
using WTB.API.Services.Interfaces;

namespace WTB.API.Services.Implementations
{
    /// <summary>
    /// Implementación del servicio de proyectos que encapsula la lógica de negocio
    /// </summary>
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IEmployeeRepository _employeeRepository;
        private readonly IAssistanceRepository _assistanceRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ProjectService> _logger;

        public ProjectService(
            IProjectRepository projectRepository,
            IEmployeeRepository employeeRepository,
            IAssistanceRepository assistanceRepository,
            IUnitOfWork unitOfWork,
            ILogger<ProjectService> logger)
        {
            _projectRepository = projectRepository;
            _employeeRepository = employeeRepository;
            _assistanceRepository = assistanceRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        #region Operaciones CRUD Básicas

        public async Task<Projects?> GetByIdAsync(int id)
        {
            try
            {
                _logger.LogInformation("Obteniendo proyecto con ID: {ProjectId}", id);
                return await _projectRepository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener proyecto con ID: {ProjectId}", id);
                throw;
            }
        }

        public async Task<Projects?> GetByIdWithIncludesAsync(int id)
        {
            try
            {
                _logger.LogInformation("Obteniendo proyecto con ID: {ProjectId} e includes", id);
                return await _projectRepository.GetByIdWithIncludesAsync(id,
                    p => p.State,
                    p => p.Assistances,
                    p => p.ProjectsAssigns);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener proyecto con ID: {ProjectId} e includes", id);
                throw;
            }
        }

        public async Task<IEnumerable<Projects>> GetAllAsync()
        {
            try
            {
                _logger.LogInformation("Obteniendo todos los proyectos");
                return await _projectRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener todos los proyectos");
                throw;
            }
        }

        public async Task<Projects> CreateAsync(CreateProjectDto createDto)
        {
            try
            {
                _logger.LogInformation("Creando nuevo proyecto: {ProjectName}", createDto.ProjectName);

                // Validar que no exista un proyecto con el mismo nombre
                if (await _projectRepository.ExistsByNameAsync(createDto.ProjectName))
                {
                    throw new InvalidOperationException($"Ya existe un proyecto con el nombre: {createDto.ProjectName}");
                }

                // Crear la entidad
                var project = new Projects
                {
                    ProjectName = createDto.ProjectName,
                    StartDate = createDto.StartDate,
                    EndDate = createDto.EndDate,
                    StateId = createDto.StateId
                };

                var createdProject = await _projectRepository.AddAsync(project);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Proyecto creado exitosamente con ID: {ProjectId}", createdProject.Id);
                return createdProject;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear proyecto: {ProjectName}", createDto.ProjectName);
                throw;
            }
        }

        public async Task<Projects> UpdateAsync(int id, UpdateProjectDto updateDto)
        {
            try
            {
                _logger.LogInformation("Actualizando proyecto con ID: {ProjectId}", id);

                var existingProject = await _projectRepository.GetByIdAsync(id);
                if (existingProject == null)
                {
                    throw new InvalidOperationException($"No se encontró el proyecto con ID: {id}");
                }

                // Validar que el nuevo nombre no esté en uso por otro proyecto
                if (updateDto.ProjectName != existingProject.ProjectName &&
                    await _projectRepository.ExistsByNameAsync(updateDto.ProjectName))
                {
                    throw new InvalidOperationException($"Ya existe otro proyecto con el nombre: {updateDto.ProjectName}");
                }

                // Actualizar propiedades
                existingProject.ProjectName = updateDto.ProjectName;
                existingProject.StartDate = updateDto.StartDate;
                existingProject.EndDate = updateDto.EndDate;
                existingProject.StateId = updateDto.StateId;

                var updatedProject = await _projectRepository.UpdateAsync(existingProject);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Proyecto actualizado exitosamente con ID: {ProjectId}", id);
                return updatedProject;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar proyecto con ID: {ProjectId}", id);
                throw;
            }
        }

        public async Task DeleteAsync(int id)
        {
            try
            {
                _logger.LogInformation("Eliminando proyecto con ID: {ProjectId}", id);

                var project = await _projectRepository.GetByIdAsync(id);
                if (project == null)
                {
                    throw new InvalidOperationException($"No se encontró el proyecto con ID: {id}");
                }

                // Validar que el proyecto pueda ser eliminado
                var (isValid, errors) = await ValidateForDeletionAsync(id);
                if (!isValid)
                {
                    throw new InvalidOperationException($"No se puede eliminar el proyecto: {string.Join(", ", errors)}");
                }

                await _projectRepository.DeleteAsync(project);
                await _unitOfWork.SaveChangesAsync();

                _logger.LogInformation("Proyecto eliminado exitosamente con ID: {ProjectId}", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al eliminar proyecto con ID: {ProjectId}", id);
                throw;
            }
        }

        #endregion

        #region Operaciones de Búsqueda y Filtrado

        public async Task<(IEnumerable<Projects> Items, int TotalCount)> GetFilteredAsync(ProjectFilterDto filter)
        {
            try
            {
                _logger.LogInformation("Obteniendo proyectos filtrados");
                return await _projectRepository.GetFilteredAsync(filter);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener proyectos filtrados");
                throw;
            }
        }

        public async Task<IEnumerable<Projects>> SearchAsync(string searchTerm)
        {
            try
            {
                _logger.LogInformation("Buscando proyectos con término: {SearchTerm}", searchTerm);
                return await _projectRepository.SearchProjectsAsync(searchTerm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar proyectos con término: {SearchTerm}", searchTerm);
                throw;
            }
        }

        public async Task<IEnumerable<Projects>> GetByStateAsync(int stateId)
        {
            try
            {
                _logger.LogInformation("Obteniendo proyectos por estado: {StateId}", stateId);
                return await _projectRepository.GetByStateAsync(stateId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener proyectos por estado: {StateId}", stateId);
                throw;
            }
        }

        public async Task<IEnumerable<Projects>> GetActiveAsync()
        {
            try
            {
                _logger.LogInformation("Obteniendo proyectos activos");
                return await _projectRepository.GetActiveProjectsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener proyectos activos");
                throw;
            }
        }

        public async Task<IEnumerable<Projects>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate)
        {
            try
            {
                _logger.LogInformation("Obteniendo proyectos por rango de fechas: {FromDate} - {ToDate}", fromDate, toDate);
                return await _projectRepository.GetByDateRangeAsync(fromDate, toDate);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener proyectos por rango de fechas: {FromDate} - {ToDate}", fromDate, toDate);
                throw;
            }
        }

        #endregion

        #region Operaciones Específicas del Negocio

        public async Task<Projects?> GetByNameAsync(string projectName)
        {
            try
            {
                _logger.LogInformation("Obteniendo proyecto por nombre: {ProjectName}", projectName);
                return await _projectRepository.GetByNameAsync(projectName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener proyecto por nombre: {ProjectName}", projectName);
                throw;
            }
        }

        public async Task<bool> ExistsByNameAsync(string projectName)
        {
            try
            {
                _logger.LogInformation("Verificando existencia de proyecto con nombre: {ProjectName}", projectName);
                return await _projectRepository.ExistsByNameAsync(projectName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al verificar existencia de proyecto con nombre: {ProjectName}", projectName);
                throw;
            }
        }

        public async Task<IEnumerable<Projects>> GetNeedingAttentionAsync(int daysThreshold = 7)
        {
            try
            {
                _logger.LogInformation("Obteniendo proyectos que requieren atención (umbral: {DaysThreshold} días)", daysThreshold);
                return await _projectRepository.GetProjectsNeedingAttentionAsync(daysThreshold);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener proyectos que requieren atención (umbral: {DaysThreshold} días)", daysThreshold);
                throw;
            }
        }

        public async Task<IEnumerable<Projects>> GetByEmployeeAsync(int employeeId)
        {
            try
            {
                _logger.LogInformation("Obteniendo proyectos del empleado: {EmployeeId}", employeeId);
                return await _projectRepository.GetProjectsByEmployeeAsync(employeeId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener proyectos del empleado: {EmployeeId}", employeeId);
                throw;
            }
        }

        public async Task<IEnumerable<Projects>> GetByHoursWorkedRangeAsync(decimal minHours, decimal maxHours)
        {
            try
            {
                _logger.LogInformation("Obteniendo proyectos por rango de horas trabajadas: {MinHours} - {MaxHours}", minHours, maxHours);
                // Esta implementación requeriría lógica adicional en el repositorio
                // Por ahora retornamos una lista vacía
                return await Task.FromResult(Enumerable.Empty<Projects>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener proyectos por rango de horas trabajadas: {MinHours} - {MaxHours}", minHours, maxHours);
                throw;
            }
        }

        #endregion

        #region Operaciones de Asignación de Empleados

        public async Task AssignEmployeeAsync(int projectId, int employeeId, DateTime startDate, DateTime? endDate = null)
        {
            try
            {
                _logger.LogInformation("Asignando empleado {EmployeeId} al proyecto {ProjectId}", employeeId, projectId);

                // Validar que el proyecto exista
                var project = await _projectRepository.GetByIdAsync(projectId);
                if (project == null)
                {
                    throw new InvalidOperationException($"No se encontró el proyecto con ID: {projectId}");
                }

                // Validar que el empleado exista
                var employee = await _employeeRepository.GetByIdAsync(employeeId);
                if (employee == null)
                {
                    throw new InvalidOperationException($"No se encontró el empleado con ID: {employeeId}");
                }

                // Validar que la asignación sea válida
                var (isValid, errors) = await ValidateEmployeeAssignmentAsync(projectId, employeeId);
                if (!isValid)
                {
                    throw new InvalidOperationException($"No se puede asignar el empleado: {string.Join(", ", errors)}");
                }

                // Aquí implementarías la lógica para crear la asignación
                // Por ahora solo registramos la operación
                _logger.LogInformation("Empleado {EmployeeId} asignado exitosamente al proyecto {ProjectId}", employeeId, projectId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al asignar empleado {EmployeeId} al proyecto {ProjectId}", employeeId, projectId);
                throw;
            }
        }

        public Task UnassignEmployeeAsync(int projectId, int employeeId)
        {
            try
            {
                _logger.LogInformation("Desasignando empleado {EmployeeId} del proyecto {ProjectId}", employeeId, projectId);

                // Aquí implementarías la lógica para desasignar al empleado
                // Por ahora solo registramos la operación
                _logger.LogInformation("Empleado {EmployeeId} desasignado exitosamente del proyecto {ProjectId}", employeeId, projectId);
                return Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al desasignar empleado {EmployeeId} del proyecto {ProjectId}", employeeId, projectId);
                throw;
            }
        }

        public async Task<IEnumerable<EmployeeInfo>> GetAssignedEmployeesAsync(int projectId)
        {
            try
            {
                _logger.LogInformation("Obteniendo empleados asignados al proyecto: {ProjectId}", projectId);
                // Esta implementación requeriría lógica adicional en el repositorio
                // Por ahora retornamos una lista vacía
                return await Task.FromResult(Enumerable.Empty<EmployeeInfo>());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener empleados asignados al proyecto: {ProjectId}", projectId);
                throw;
            }
        }

        public async Task<IEnumerable<Projects>> GetProjectsByEmployeeAsync(int employeeId)
        {
            try
            {
                _logger.LogInformation("Obteniendo proyectos del empleado: {EmployeeId}", employeeId);
                return await _projectRepository.GetProjectsByEmployeeAsync(employeeId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener proyectos del empleado: {EmployeeId}", employeeId);
                throw;
            }
        }

        #endregion

        #region Operaciones de Reportes y Estadísticas

        public async Task<IEnumerable<ProjectWorkSummary>> GetTopWorkingAsync(DateTime fromDate, DateTime toDate, int top = 10)
        {
            try
            {
                _logger.LogInformation("Obteniendo top {Top} proyectos trabajadores del {FromDate} al {ToDate}", top, fromDate, toDate);
                return await _projectRepository.GetTopWorkingProjectsAsync(fromDate, toDate, top);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener top {Top} proyectos trabajadores del {FromDate} al {ToDate}", top, fromDate, toDate);
                throw;
            }
        }

        public async Task<ProjectStatistics> GetStatisticsAsync()
        {
            try
            {
                _logger.LogInformation("Obteniendo estadísticas de proyectos");
                return await _projectRepository.GetProjectStatisticsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener estadísticas de proyectos");
                throw;
            }
        }

        public async Task<decimal> GetProgressAsync(int projectId)
        {
            try
            {
                _logger.LogInformation("Obteniendo progreso del proyecto: {ProjectId}", projectId);
                
                // Esta implementación requeriría lógica adicional para calcular el progreso
                // Por ahora retornamos 0
                return await Task.FromResult(0m);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener progreso del proyecto: {ProjectId}", projectId);
                throw;
            }
        }

        public async Task<DateTime?> GetEstimatedEndDateAsync(int projectId)
        {
            try
            {
                _logger.LogInformation("Obteniendo fecha estimada de finalización del proyecto: {ProjectId}", projectId);
                
                var project = await _projectRepository.GetByIdAsync(projectId);
                return project?.EndDate;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener fecha estimada de finalización del proyecto: {ProjectId}", projectId);
                throw;
            }
        }

        #endregion

        #region Operaciones de Validación

        public async Task<(bool IsValid, List<string> Errors)> ValidateForCreationAsync(CreateProjectDto createDto)
        {
            var errors = new List<string>();

            try
            {
                // Validar que el nombre no esté en uso
                if (await _projectRepository.ExistsByNameAsync(createDto.ProjectName))
                {
                    errors.Add($"Ya existe un proyecto con el nombre: {createDto.ProjectName}");
                }

                // Validar que la fecha de inicio no sea en el futuro (opcional)
                if (createDto.StartDate.HasValue && createDto.StartDate.Value > DateTime.Today)
                {
                    errors.Add("La fecha de inicio no puede ser en el futuro");
                }

                // Validar que la fecha de fin sea posterior a la de inicio
                if (createDto.StartDate.HasValue && createDto.EndDate.HasValue && 
                    createDto.EndDate.Value <= createDto.StartDate.Value)
                {
                    errors.Add("La fecha de fin debe ser posterior a la fecha de inicio");
                }

                return (errors.Count == 0, errors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al validar proyecto para creación");
                errors.Add("Error interno durante la validación");
                return (false, errors);
            }
        }

        public async Task<(bool IsValid, List<string> Errors)> ValidateForUpdateAsync(int id, UpdateProjectDto updateDto)
        {
            var errors = new List<string>();

            try
            {
                // Validar que el proyecto exista
                var existingProject = await _projectRepository.GetByIdAsync(id);
                if (existingProject == null)
                {
                    errors.Add($"No se encontró el proyecto con ID: {id}");
                    return (false, errors);
                }

                // Validar que el nuevo nombre no esté en uso por otro proyecto
                if (updateDto.ProjectName != existingProject.ProjectName &&
                    await _projectRepository.ExistsByNameAsync(updateDto.ProjectName))
                {
                    errors.Add($"Ya existe otro proyecto con el nombre: {updateDto.ProjectName}");
                }

                // Validar que la fecha de inicio no sea en el futuro
                if (updateDto.StartDate.HasValue && updateDto.StartDate.Value > DateTime.Today)
                {
                    errors.Add("La fecha de inicio no puede ser en el futuro");
                }

                // Validar que la fecha de fin sea posterior a la de inicio
                if (updateDto.StartDate.HasValue && updateDto.EndDate.HasValue && 
                    updateDto.EndDate.Value <= updateDto.StartDate.Value)
                {
                    errors.Add("La fecha de fin debe ser posterior a la fecha de inicio");
                }

                return (errors.Count == 0, errors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al validar proyecto para actualización con ID: {ProjectId}", id);
                errors.Add("Error interno durante la validación");
                return (false, errors);
            }
        }

        public async Task<(bool IsValid, List<string> Errors)> ValidateForDeletionAsync(int id)
        {
            var errors = new List<string>();

            try
            {
                // Validar que el proyecto exista
                var project = await _projectRepository.GetByIdAsync(id);
                if (project == null)
                {
                    errors.Add($"No se encontró el proyecto con ID: {id}");
                    return (false, errors);
                }

                // Aquí podrías agregar más validaciones de negocio
                // Por ejemplo, verificar que no tenga asistencias activas
                // o que no esté en un estado que impida la eliminación

                return (errors.Count == 0, errors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al validar proyecto para eliminación con ID: {ProjectId}", id);
                errors.Add("Error interno durante la validación");
                return (false, errors);
            }
        }

        public async Task<(bool IsValid, List<string> Errors)> ValidateEmployeeAssignmentAsync(int projectId, int employeeId)
        {
            var errors = new List<string>();

            try
            {
                // Validar que el proyecto exista
                var project = await _projectRepository.GetByIdAsync(projectId);
                if (project == null)
                {
                    errors.Add($"No se encontró el proyecto con ID: {projectId}");
                }

                // Validar que el empleado exista
                var employee = await _employeeRepository.GetByIdAsync(employeeId);
                if (employee == null)
                {
                    errors.Add($"No se encontró el empleado con ID: {employeeId}");
                }

                // Validar que el proyecto esté activo
                if (project != null && project.StateId != 1) // Asumiendo que 1 = Activo
                {
                    errors.Add("Solo se pueden asignar empleados a proyectos activos");
                }

                // Aquí podrías agregar más validaciones de negocio
                // Por ejemplo, verificar que el empleado no esté ya asignado al proyecto
                // o que no tenga conflictos de horario

                return (errors.Count == 0, errors);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al validar asignación de empleado {EmployeeId} al proyecto {ProjectId}", employeeId, projectId);
                errors.Add("Error interno durante la validación");
                return (false, errors);
            }
        }

        #endregion
    }
}
