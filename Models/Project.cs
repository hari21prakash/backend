using System;

namespace Backend.Models
{
    /// <summary>
    /// Project entity. Only Id and ProjectName are required (NOT NULL).
    /// Every other field is nullable and stores a real NULL when not provided —
    /// never a placeholder string like "N/A" or "Unknown".
    /// </summary>
    public class Project
    {
        public Guid Id { get; set; } = Guid.NewGuid();

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

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

    /// <summary>Allowed values are advisory only — the column is a plain nullable string
    /// so the API never rejects a project for using a value outside this list.</summary>
    public static class ProjectStatus
    {
        public const string Planning = "Planning";
        public const string InProgress = "In Progress";
        public const string Completed = "Completed";
        public const string OnHold = "On Hold";

        public static readonly string[] All = { Planning, InProgress, Completed, OnHold };
    }

    public static class ProjectCategory
    {
        public const string WebDevelopment = "Web Development";
        public const string MobileApp = "Mobile App";
        public const string AI = "AI";
        public const string Desktop = "Desktop";
        public const string DataScience = "Data Science";
        public const string Other = "Other";

        public static readonly string[] All = { WebDevelopment, MobileApp, AI, Desktop, DataScience, Other };
    }
}
