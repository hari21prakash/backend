using Backend.DTOs;

namespace Backend.Services
{
    public interface IProjectService
    {
        Task<IEnumerable<ProjectResponseDto>> GetAllAsync(string? search, string? status, string? category, string? sort);
        Task<ProjectResponseDto?> GetByIdAsync(Guid id);
        Task<ProjectResponseDto> CreateAsync(CreateProjectDto dto);
        Task<ProjectResponseDto?> UpdateAsync(Guid id, UpdateProjectDto dto);
        Task<bool> DeleteAsync(Guid id);
        Task<IEnumerable<ProjectResponseDto>> GetByStatusAsync(string status);
        Task<IEnumerable<ProjectResponseDto>> GetByCategoryAsync(string category);
        Task<object> GetDashboardStatsAsync();
    }
}
