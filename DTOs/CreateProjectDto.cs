using System.ComponentModel.DataAnnotations;

namespace Backend.DTOs
{
    /// <summary>
    /// Every property besides ProjectName is nullable and has no [Required] attribute,
    /// so omitting a property in the JSON body (or sending it as null) is valid.
    /// Validation attributes like [Url] only run when a value is actually present.
    /// </summary>
    public class CreateProjectDto
    {
        [Required(ErrorMessage = "Project name is required.")]
        [MinLength(2, ErrorMessage = "Project name must be at least 2 characters.")]
        [MaxLength(200, ErrorMessage = "Project name cannot exceed 200 characters.")]
        public string ProjectName { get; set; } = string.Empty;

        [MaxLength(5000, ErrorMessage = "Description cannot exceed 5000 characters.")]
        public string? Description { get; set; }

        [Url(ErrorMessage = "Project link must be a valid URL.")]
        public string? ProjectLink { get; set; }

        [Url(ErrorMessage = "Video URL must be a valid URL.")]
        public string? VideoUrl { get; set; }

        [Url(ErrorMessage = "Image URL must be a valid URL.")]
        public string? ImageUrl { get; set; }

        public string[]? Technologies { get; set; }

        [Url(ErrorMessage = "GitHub URL must be a valid URL.")]
        public string? GithubUrl { get; set; }

        [Url(ErrorMessage = "Live demo URL must be a valid URL.")]
        public string? LiveDemoUrl { get; set; }

        public string? Status { get; set; }

        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public string? Category { get; set; }

        [MaxLength(200)]
        public string? ClientName { get; set; }

        public string? Notes { get; set; }
    }
}
