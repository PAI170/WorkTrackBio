using AutoMapper;
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

        public async Task<IEnumerable<ProjectDataTransferObject>> GetAllProjectsAsync()
        {
            var projects = await _projectRepository.GetAllAsync();
            return _mapper.Map<IEnumerable<ProjectDataTransferObject>>(projects);
        }

        public async Task<ProjectDataTransferObject?> GetProjectByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("El ID debe ser mayor que 0", nameof(id));

            var project = await _projectRepository.GetByIdAsync(id);
            return _mapper.Map<ProjectDataTransferObject>(project);
        }

        public async Task<ProjectDataTransferObject?> GetProjectByNameAsync(string projectName)
        {
            if (string.IsNullOrWhiteSpace(projectName))
                throw new ArgumentException("El nombre del proyecto no puede estar vacío", nameof(projectName));

            var project = await _projectRepository.GetByNameAsync(projectName);
            return _mapper.Map<ProjectDataTransferObject>(project);
        }

        public async Task<IEnumerable<ProjectDataTransferObject>> GetProjectsByStateAsync(int stateId)
        {
            if (stateId <= 0)
                throw new ArgumentException("El ID del estado debe ser mayor que 0", nameof(stateId));

            // Verificar que el estado exista antes de buscar proyectos
            var stateExists = await _stateRepository.GetByIdAsync(stateId);
            if (stateExists == null)
                throw new ArgumentException($"No existe un estado con ID {stateId}", nameof(stateId));

            var projects = await _projectRepository.GetByStateAsync(stateId);
            return _mapper.Map<IEnumerable<ProjectDataTransferObject>>(projects);
        }

        public async Task<bool> ProjectExistsAsync(int id)
        {
            if (id <= 0)
                return false;

            var project = await _projectRepository.GetByIdAsync(id);
            return project != null;
        }

        public async Task<bool> ProjectNameExistsAsync(string projectName)
        {
            if (string.IsNullOrWhiteSpace(projectName))
                return false;

            return await _projectRepository.ExistsByNameAsync(projectName);
        }

        public async Task<ProjectDataTransferObject> CreateProjectAsync(CreateProjectDataTransferObject createDto)
        {
            if (createDto == null)
                throw new ArgumentNullException(nameof(createDto));

            var existingProjects = await _projectRepository.GetAllAsync();
            if (existingProjects.Any(p => string.Equals(p.ProjectName, createDto.ProjectName, StringComparison.OrdinalIgnoreCase)))
                throw new InvalidOperationException($"Ya existe un proyecto con el nombre '{createDto.ProjectName}' (ignorando mayúsculas/minúsculas)");

            if (createDto.StartDate.HasValue && createDto.EndDate.HasValue)
            {
                if (createDto.EndDate.Value < createDto.StartDate.Value)
                    throw new InvalidOperationException("La fecha de finalización no puede ser anterior a la fecha de inicio");
            }

            var project = _mapper.Map<WorkTrackBio.API.Data.Models.Project>(createDto);

            var createdProject = await _projectRepository.CreateAsync(project);

            return _mapper.Map<ProjectDataTransferObject>(createdProject);
        }

        public async Task<ProjectDataTransferObject?> UpdateProjectAsync(UpdateProjectDataTransferObject updateDto)
        {
            if (updateDto == null)
                throw new ArgumentNullException(nameof(updateDto));

            if (updateDto.Id <= 0)
                throw new ArgumentException("El ID debe ser mayor que 0", nameof(updateDto.Id));

            var existingProject = await _projectRepository.GetByIdAsync(updateDto.Id);
            if (existingProject == null)
                return null;

            bool hasChanges = false;

            if (!string.IsNullOrWhiteSpace(updateDto.ProjectName))
            {
                var allProjects = await _projectRepository.GetAllAsync();
                if (allProjects.Any(p => string.Equals(p.ProjectName, updateDto.ProjectName, StringComparison.OrdinalIgnoreCase) &&
                    p.Id != updateDto.Id))
                    throw new InvalidOperationException($"Ya existe un proyecto con el nombre '{updateDto.ProjectName}' (ignorando mayúsculas/minúsculas)");

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
                    throw new InvalidOperationException("La fecha de finalización no puede ser anterior a la fecha de inicio");
            }

            if (!hasChanges)
            {
                return _mapper.Map<ProjectDataTransferObject>(existingProject);
            }

            var updatedProject = await _projectRepository.UpdateAsync(existingProject);

            return _mapper.Map<ProjectDataTransferObject>(updatedProject);
        }

        public async Task<bool> DeleteProjectAsync(int id)
        {
            if (id <= 0)
                return false;

            var project = await _projectRepository.GetByIdAsync(id);
            if (project == null)
                return false;

            var isProjectInUse = await IsProjectInUseAsync(id);
            if (isProjectInUse)
                throw new InvalidOperationException($"No se puede eliminar el proyecto '{project.ProjectName}' porque está siendo usado por otras entidades del sistema");

            return await _projectRepository.DeleteAsync(id);
        }

        private async Task<bool> IsProjectInUseAsync(int projectId)
        {

            var hasDependencies = await _projectRepository.HasDependenciesAsync(projectId);
            return hasDependencies;
        }
    }
}
