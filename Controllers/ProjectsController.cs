using Backend.DTOs;
using Backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Backend.Controllers
{
    [ApiController]
    [Route("api/projects")]
    [Authorize]
    public class ProjectsController : ControllerBase
    {
        private readonly IProjectService _service;
        private readonly ILogger<ProjectsController> _logger;

        public ProjectsController(IProjectService service, ILogger<ProjectsController> logger)
        {
            _service = service;
            _logger = logger;
        }

        // GET /api/projects?search=&status=&category=&sort=newest|oldest
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] string? search,
            [FromQuery] string? status,
            [FromQuery] string? category,
            [FromQuery] string? sort)
        {
            try
            {
                var projects = await _service.GetAllAsync(search, status, category, sort);
                return Ok(projects);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch projects.");
                return Problem("Something went wrong while loading projects. Please try again.", statusCode: 500);
            }
        }

        // GET /api/projects/dashboard
        [HttpGet("dashboard")]
        public async Task<IActionResult> GetDashboardStats()
        {
            try
            {
                var stats = await _service.GetDashboardStatsAsync();
                return Ok(stats);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to compute dashboard stats.");
                return Problem("Something went wrong while loading the dashboard. Please try again.", statusCode: 500);
            }
        }

        // GET /api/projects/search?q=
        [HttpGet("search")]
        public async Task<IActionResult> Search([FromQuery] string? q)
        {
            try
            {
                var projects = await _service.GetAllAsync(q, null, null, null);
                return Ok(projects);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to search projects.");
                return Problem("Something went wrong while searching. Please try again.", statusCode: 500);
            }
        }

        // GET /api/projects/status/{status}
        [HttpGet("status/{status}")]
        public async Task<IActionResult> GetByStatus(string status)
        {
            try
            {
                var projects = await _service.GetByStatusAsync(status);
                return Ok(projects);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch projects by status.");
                return Problem("Something went wrong while filtering by status. Please try again.", statusCode: 500);
            }
        }

        // GET /api/projects/category/{category}
        [HttpGet("category/{category}")]
        public async Task<IActionResult> GetByCategory(string category)
        {
            try
            {
                var projects = await _service.GetByCategoryAsync(category);
                return Ok(projects);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch projects by category.");
                return Problem("Something went wrong while filtering by category. Please try again.", statusCode: 500);
            }
        }

        // GET /api/projects/{id}
        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            try
            {
                var project = await _service.GetByIdAsync(id);
                if (project is null)
                    return NotFound(new { message = "Project not found." });

                return Ok(project);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to fetch project {Id}.", id);
                return Problem("Something went wrong while loading this project. Please try again.", statusCode: 500);
            }
        }

        // POST /api/projects
        // Only ProjectName is required. Any omitted or null optional field is
        // stored as a real NULL — {"projectName": "Test Project"} is a valid body.
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateProjectDto dto)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            try
            {
                var created = await _service.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create project.");
                return Problem("Something went wrong while creating the project. Please try again.", statusCode: 500);
            }
        }

        // PUT /api/projects/{id}
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProjectDto dto)
        {
            if (!ModelState.IsValid)
                return ValidationProblem(ModelState);

            try
            {
                var updated = await _service.UpdateAsync(id, dto);
                if (updated is null)
                    return NotFound(new { message = "Project not found." });

                return Ok(updated);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update project {Id}.", id);
                return Problem("Something went wrong while updating the project. Please try again.", statusCode: 500);
            }
        }

        // DELETE /api/projects/{id}
        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var deleted = await _service.DeleteAsync(id);
                if (!deleted)
                    return NotFound(new { message = "Project not found." });

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete project {Id}.", id);
                return Problem("Something went wrong while deleting the project. Please try again.", statusCode: 500);
            }
        }
    }
}
