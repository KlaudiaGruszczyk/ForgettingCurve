using ForgettingCurve.Api.Filters;
using ForgettingCurve.Application.Abstractions;
using ForgettingCurve.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace ForgettingCurve.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ScopesController : ControllerBase
    {
        private readonly IScopeRepository _scopeRepository;
        private readonly IUnitOfWork _unitOfWork;

        public ScopesController(IScopeRepository scopeRepository, IUnitOfWork unitOfWork)
        {
            _scopeRepository = scopeRepository;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Scope>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllScopes()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
            {
                return Unauthorized();
            }

            var scopes = await _scopeRepository.GetScopesByUserIdAsync(userGuid);
            return Ok(scopes);
        }

        [HttpGet("{id}")]
        [RequiresResourceOwnership("id", typeof(IScopeRepository))]
        [ProducesResponseType(typeof(Scope), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetScope(Guid id)
        {
            var scope = await _scopeRepository.GetByIdAsync(id);
            
            if (scope == null)
            {
                return NotFound();
            }
            
            return Ok(scope);
        }

        [HttpPost]
        [ProducesResponseType(typeof(Scope), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateScope([FromBody] Scope scope)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
            {
                return Unauthorized();
            }

            scope.OwnerUserId = userGuid;
            scope.CreationDate = DateTime.UtcNow;
            
            await _scopeRepository.AddAsync(scope);
            await _unitOfWork.SaveChangesAsync();
            
            return CreatedAtAction(nameof(GetScope), new { id = scope.Id }, scope);
        }

        [HttpPut("{id}")]
        [RequiresResourceOwnership("id", typeof(IScopeRepository))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateScope(Guid id, [FromBody] Scope scope)
        {
            if (id != scope.Id)
            {
                return BadRequest();
            }

            var existingScope = await _scopeRepository.GetByIdAsync(id);
            
            if (existingScope == null)
            {
                return NotFound();
            }
            
            // Zachowanie OwnerUserId - nie pozwalamy na zmianę właściciela
            scope.OwnerUserId = existingScope.OwnerUserId;
            scope.CreationDate = existingScope.CreationDate;
            scope.LastModifiedDate = DateTime.UtcNow;
            
            _scopeRepository.Update(scope);
            await _unitOfWork.SaveChangesAsync();
            
            return NoContent();
        }

        [HttpDelete("{id}")]
        [RequiresResourceOwnership("id", typeof(IScopeRepository))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteScope(Guid id)
        {
            var scope = await _scopeRepository.GetByIdAsync(id);
            
            if (scope == null)
            {
                return NotFound();
            }
            
            _scopeRepository.Remove(scope);
            await _unitOfWork.SaveChangesAsync();
            
            return NoContent();
        }
    }
} 