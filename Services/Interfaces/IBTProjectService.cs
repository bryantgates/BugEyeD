using BugEyeD.Models;

namespace BugEyeD.Services.Interfaces
{
    public interface IBTProjectService
    {
       public Task AddProjectAsync(Project project);
       public Task<List<Project>> GetAllProjectsByCompanyIdAsync(int companyId);
       public Task<List<Project>> GetAllProjectsByPriorityAsync(int companyId, string priority);
       public Task<List<Project>> GetAllUserProjectsAsync(string userId);
       public Task<List<Project>> GetArchivedProjectsByCompanyIdAsync(int companyId);
       public Task<Project?> GetProjectByIdAsync(int projectId, int companyId);
       public Task<List<ProjectPriority>> GetProjectPrioritiesAsync();
       public Task ArchiveProjectAsync(Project project, int companyId);
       public Task RestoreProjectAsync(Project project, int companyId);
       public Task UpdateProjectAsync(Project project, int companyId);
       public Task<BTUser?> GetProjectManagerAsync(int projectId, int companyId);
       Task<bool> AddProjectManagerAsync(string userId, int projectId, int companyId);
       Task RemoveProjectManagerAsync(int projectId, int companyId);
    }
}
