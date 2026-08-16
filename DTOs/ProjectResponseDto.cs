using System;

namespace Backend.DTOs
{
    public class ProjectResponseDto
    {
        public Guid Id { get; set; }
        public string ProjectName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? ProjectLink { get; set; }
        public string? VideoUrl { get; set; }
        public string? ImageUrl { get; set; }
        public string[]? Technologies { get; set; }
        public string? GithubUrl { get; set; }
        public string? LiveDemoUrl { get; set; }
        public string? Status { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Category { get; set; }
        public string? ClientName { get; set; }
        public string? Notes { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
