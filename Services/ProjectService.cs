using Backend.Data;
using Backend.DTOs;
using Backend.Models;
using Microsoft.EntityFrameworkCore;

namespace Backend.Services
{
    public class ProjectService : IProjectService
    {
        private readonly AppDbContext _context;

        public ProjectService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ProjectResponseDto>> GetAllAsync(string? search, string? status, string? category, string? sort)
        {
            var query = _context.Projects.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim().ToLower();
                query = query.Where(p =>
                    p.ProjectName.ToLower().Contains(term) ||
                    (p.Description != null && p.Description.ToLower().Contains(term)) ||
                    (p.ClientName != null && p.ClientName.ToLower().Contains(term)));
            }

            if (!string.IsNullOrWhiteSpace(status))
            {
                query = query.Where(p => p.Status == status);
            }

            if (!string.IsNullOrWhiteSpace(category))
            {
                query = query.Where(p => p.Category == category);
            }

            query = sort switch
            {
                "oldest" => query.OrderBy(p => p.CreatedAt),
                _ => query.OrderByDescending(p => p.CreatedAt), // "newest" is the default
            };

            var projects = await query.ToListAsync();
            return projects.Select(MapToDto);
        }

        public async Task<ProjectResponseDto?> GetByIdAsync(Guid id)
        {
            var project = await _context.Projects.FindAsync(id);
            return project is null ? null : MapToDto(project);
        }
        private static DateTime? NormalizeDate(DateTime? value)
        {
            if (!value.HasValue)
                return null;

            return DateTime.SpecifyKind(value.Value, DateTimeKind.Utc);
        }
        public async Task<ProjectResponseDto> CreateAsync(CreateProjectDto dto)
        {
            var now = DateTime.UtcNow;

            var project = new Project
            {
                Id = Guid.NewGuid(),
                ProjectName = dto.ProjectName.Trim(),

                // Every optional field is copied as-is. If the client omitted the
                // property (or sent null), model binding leaves it null and that
                // null is written straight into the database — no placeholder text.
                Description = NullIfEmpty(dto.Description),
                ProjectLink = NullIfEmpty(dto.ProjectLink),
                VideoUrl = NullIfEmpty(dto.VideoUrl),
                ImageUrl = NullIfEmpty(dto.ImageUrl),
                Technologies = NullIfEmptyArray(dto.Technologies),
                GithubUrl = NullIfEmpty(dto.GithubUrl),
                LiveDemoUrl = NullIfEmpty(dto.LiveDemoUrl),
                Status = NullIfEmpty(dto.Status),
                StartDate = NormalizeDate(dto.StartDate),
                EndDate = NormalizeDate(dto.EndDate),
                Category = NullIfEmpty(dto.Category),
                ClientName = NullIfEmpty(dto.ClientName),
                Notes = NullIfEmpty(dto.Notes),

                CreatedAt = now,
                UpdatedAt = now
            };

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();

            return MapToDto(project);
        }

        public async Task<ProjectResponseDto?> UpdateAsync(Guid id, UpdateProjectDto dto)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project is null) return null;

            project.ProjectName = dto.ProjectName.Trim();

            // If the caller sends null (or omits the field) for an optional value,
            // that field is explicitly set to null here — clearing it in the DB.
            project.Description = NullIfEmpty(dto.Description);
            project.ProjectLink = NullIfEmpty(dto.ProjectLink);
            project.VideoUrl = NullIfEmpty(dto.VideoUrl);
            project.ImageUrl = NullIfEmpty(dto.ImageUrl);
            project.Technologies = NullIfEmptyArray(dto.Technologies);
            project.GithubUrl = NullIfEmpty(dto.GithubUrl);
            project.LiveDemoUrl = NullIfEmpty(dto.LiveDemoUrl);
            project.Status = NullIfEmpty(dto.Status);
            project.StartDate = NormalizeDate(dto.StartDate);
            project.EndDate = NormalizeDate(dto.EndDate);
            project.Category = NullIfEmpty(dto.Category);
            project.ClientName = NullIfEmpty(dto.ClientName);
            project.Notes = NullIfEmpty(dto.Notes);

            project.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return MapToDto(project);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var project = await _context.Projects.FindAsync(id);
            if (project is null) return false;

            _context.Projects.Remove(project);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<ProjectResponseDto>> GetByStatusAsync(string status)
        {
            var projects = await _context.Projects.Where(p => p.Status == status).ToListAsync();
            return projects.Select(MapToDto);
        }

        public async Task<IEnumerable<ProjectResponseDto>> GetByCategoryAsync(string category)
        {
            var projects = await _context.Projects.Where(p => p.Category == category).ToListAsync();
            return projects.Select(MapToDto);
        }

        public async Task<object> GetDashboardStatsAsync()
        {
            var projects = await _context.Projects.ToListAsync();

            return new
            {
                TotalProjects = projects.Count,
                CompletedProjects = projects.Count(p => p.Status == ProjectStatus.Completed),
                InProgressProjects = projects.Count(p => p.Status == ProjectStatus.InProgress),
                PlanningProjects = projects.Count(p => p.Status == ProjectStatus.Planning),
                OnHoldProjects = projects.Count(p => p.Status == ProjectStatus.OnHold),
                ProjectsMissingOptionalInfo = projects.Count(p =>
                    p.Description == null && p.ImageUrl == null && p.GithubUrl == null &&
                    p.LiveDemoUrl == null && p.Technologies == null && p.Status == null &&
                    p.Category == null),
                RecentProjects = projects
                    .OrderByDescending(p => p.CreatedAt)
                    .Take(5)
                    .Select(MapToDto)
            };
        }

        private static string? NullIfEmpty(string? value) =>
            string.IsNullOrWhiteSpace(value) ? null : value.Trim();

        private static string[]? NullIfEmptyArray(string[]? values)
        {
            if (values is null) return null;
            var cleaned = values.Where(v => !string.IsNullOrWhiteSpace(v)).Select(v => v.Trim()).ToArray();
            return cleaned.Length == 0 ? null : cleaned;
        }

        private static ProjectResponseDto MapToDto(Project p) => new()
        {
            Id = p.Id,
            ProjectName = p.ProjectName,
            Description = p.Description,
            ProjectLink = p.ProjectLink,
            VideoUrl = p.VideoUrl,
            ImageUrl = p.ImageUrl,
            Technologies = p.Technologies,
            GithubUrl = p.GithubUrl,
            LiveDemoUrl = p.LiveDemoUrl,
            Status = p.Status,
            StartDate = p.StartDate,
            EndDate = p.EndDate,
            Category = p.Category,
            ClientName = p.ClientName,
            Notes = p.Notes,
            CreatedAt = p.CreatedAt,
            UpdatedAt = p.UpdatedAt
        };
    }
}
