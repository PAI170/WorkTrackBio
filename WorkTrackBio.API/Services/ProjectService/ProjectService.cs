using AutoMapper;
using WorkTrackBio.API.Common;
using WorkTrackBio.API.Data.Models;
using WorkTrackBio.API.DataTransferObjects.Project;
using WorkTrackBio.API.Repositories.ProjectRepository;
using WorkTrackBio.API.Repositories.StateRepository;

namespace WorkTrackBio.API.Services.ProjectService
{
    public class ProjectService : IProjectService
    {
        private readonly IProjectRepository _projectRepository;
        private readonly IStateRepository _stateRepository;
        private readonly IMapper _mapper;

        public ProjectService(IProjectRepository projectRepository, IStateRepository stateRepository, IMapper mapper)
        {
            _projectRepository = projectRepository ?? throw new ArgumentNullException(nameof(projectRepository));
            _stateRepository = stateRepository ?? throw new ArgumentNullException(nameof(stateRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<ApiResponse<IEnumerable<ProjectDataTransferObject>>> GetAllProjectsAsync()
        {
            var projects = await _projectRepository.GetAllAsync();
            var projectDto = _mapper.Map<IEnumerable<ProjectDataTransferObject>>(projects);

            return ApiResponse<IEnumerable<ProjectDataTransferObject>>.SuccessResponse(projectDto,
                    $"Se encontraron {projectDto.Count()} proyectos");
        }

        public async Task<ApiResponse<ProjectDataTransferObject>> GetProjectByIdAsync(int id)
        {
            if (id <= 0)
                return ApiResponse<ProjectDataTransferObject>.ErrorResponse("El ID debe ser mayor que 0", 400);

            var project = await _projectRepository.GetByIdAsync(id);
           
            if (project == null)
                return ApiResponse<ProjectDataTransferObject>.ErrorResponse($"No se encontro el proyecto con el ID {id}", 404);

            var result = _mapper.Map<ProjectDataTransferObject>(project);
            return ApiResponse<ProjectDataTransferObject>.SuccessResponse(result, "Proyectos obtenido exitosamente");
        }

        public async Task<ApiResponse<ProjectDataTransferObject>> GetProjectByNameAsync(string projectName)
        {
            if (string.IsNullOrWhiteSpace(projectName))
                return ApiResponse<ProjectDataTransferObject>.ErrorResponse("El nombre del proyecto no puede ir vacío", 400);
            var project = await _projectRepository.GetByNameAsync(projectName);
            if (project == null)
                return ApiResponse<ProjectDataTransferObject>.ErrorResponse($"No se encontró el proyecto '{projectName}'", 404);

            var result = _mapper.Map<ProjectDataTransferObject>(project);
            return ApiResponse<ProjectDataTransferObject>.SuccessResponse(result, "Proyecto obtenido exitosamente");

        }

        public async Task<ApiResponse<IEnumerable<ProjectDataTransferObject>>> GetProjectsByStateAsync(int stateId)
        {
            if (stateId <= 0)
                return ApiResponse<IEnumerable<ProjectDataTransferObject>>.ErrorResponse("El ID debe ser mayor que 0", 400);

            var stateExists = await _stateRepository.GetByIdAsync(stateId);
            if (stateExists == null)
                return ApiResponse<IEnumerable<ProjectDataTransferObject>>.ErrorResponse($"No se encontro el proyecto con el ID {stateId}", 404);

            var projects = await _projectRepository.GetByStateAsync(stateId);
            var result = _mapper.Map<IEnumerable<ProjectDataTransferObject>>(projects);
            return ApiResponse<IEnumerable<ProjectDataTransferObject>>.SuccessResponse(result, "Proyectos obtenidos exitosamente");
        }

        public async Task<ApiResponse<ProjectDataTransferObject>> CreateProjectAsync(CreateProjectDataTransferObject createDto)
        {
            if (createDto == null)
                return ApiResponse<ProjectDataTransferObject>.ErrorResponse("Nombre del Proyecto no puede estar vacio", 400);

            if (await _projectRepository.ExistsByNameAsync(createDto.ProjectName))
                return ApiResponse<ProjectDataTransferObject>.ErrorResponse($"Ya existe un proyecto con el nombre", 409);

            if (createDto.StartDate.HasValue && createDto.EndDate.HasValue)
            {
                if (createDto.EndDate.Value < createDto.StartDate.Value)
                    return ApiResponse<ProjectDataTransferObject>.ErrorResponse("La fecha de finalización no puede ser anterior a la fecha de inicio", 400);
            }

            var project = _mapper.Map<WorkTrackBio.API.Data.Models.Project>(createDto);

            var createdProject = await _projectRepository.CreateAsync(project);

            var result = _mapper.Map<ProjectDataTransferObject>(createdProject);
            return ApiResponse<ProjectDataTransferObject>.SuccessResponse(result, "Proyecto creado exitosamente", 201);
        }

        public async Task<ApiResponse<ProjectDataTransferObject>> UpdateProjectAsync(UpdateProjectDataTransferObject updateDto)
        {
            if (updateDto == null)
                return ApiResponse<ProjectDataTransferObject>.ErrorResponse("Nombre del Proyecto no puede estar vacio", 400);

            if (updateDto.Id <= 0)
                return ApiResponse<ProjectDataTransferObject>.ErrorResponse("El ID tiene que ser mayor que 0", 400);

            var existingProject = await _projectRepository.GetByIdAsync(updateDto.Id);
            if (existingProject == null)
                return ApiResponse<ProjectDataTransferObject>.ErrorResponse($"No se encontró un proyecto con ID {updateDto.Id}", 404);

            bool hasChanges = false;

            if (!string.IsNullOrWhiteSpace(updateDto.ProjectName))
            {
                if (await _projectRepository.ExistsByNameAsync(updateDto.ProjectName) && existingProject.ProjectName.ToLower() != updateDto.ProjectName.ToLower())
                    return ApiResponse<ProjectDataTransferObject>.ErrorResponse($"Ya existe un proyecto con el nombre", 409);

                if (!string.Equals(existingProject.ProjectName, updateDto.ProjectName, StringComparison.OrdinalIgnoreCase))
                {
                    existingProject.ProjectName = updateDto.ProjectName;
                    hasChanges = true;
                }
            }

            if (updateDto.StartDate.HasValue)
            {
                if (existingProject.StartDate != updateDto.StartDate.Value)
                {
                    existingProject.StartDate = updateDto.StartDate.Value;
                    hasChanges = true;
                }
            }

            if (updateDto.EndDate.HasValue)
            {
                if (existingProject.EndDate != updateDto.EndDate.Value)
                {
                    existingProject.EndDate = updateDto.EndDate.Value;
                    hasChanges = true;
                }
            }

            if (updateDto.StateId.HasValue)
            {
                if (existingProject.StateId != updateDto.StateId.Value)
                {
                    existingProject.StateId = updateDto.StateId.Value;
                    hasChanges = true;
                }
            }

            if (existingProject.StartDate.HasValue && existingProject.EndDate.HasValue)
            {
                if (existingProject.EndDate.Value < existingProject.StartDate.Value)
                    return ApiResponse<ProjectDataTransferObject>.ErrorResponse("La fecha de finalización no puede ser anterior a la fecha de inicio", 400);
            }

            if (!hasChanges)
            {
                var unchanged = _mapper.Map<ProjectDataTransferObject>(existingProject);
                return ApiResponse<ProjectDataTransferObject>.SuccessResponse(unchanged, "No se detectaron cambios");
            }

            var updatedProject = await _projectRepository.UpdateAsync(existingProject);

            var result = _mapper.Map<ProjectDataTransferObject>(updatedProject);
            return ApiResponse<ProjectDataTransferObject>.SuccessResponse(result, "Proyecto actualizado exitosamente");
        }

        public async Task<ApiResponse<bool>> DeleteProjectAsync(int id)
        {
            if (id <= 0)
                return ApiResponse<bool>.ErrorResponse("El ID debe ser mayor que 0", 400);

            var project = await _projectRepository.GetByIdAsync(id);
            if (project == null)
                return ApiResponse<bool>.ErrorResponse($"No se encontró un proyecto con ID {id}", 404);

            var isProjectInUse = await IsProjectInUseAsync(id);
            if (isProjectInUse)
                return ApiResponse<bool>.ErrorResponse($"No se puede eliminar el proyecto porque está siendo usado por proyectos del sistema", 409);

            await _projectRepository.DeleteAsync(id);
            return ApiResponse<bool>.SuccessResponse(true, "Proyecto eliminado exitosamente");
        }

        private async Task<bool> IsProjectInUseAsync(int projectId)
        {
            return await _projectRepository.HasDependenciesAsync(projectId);
        }
    }
}
