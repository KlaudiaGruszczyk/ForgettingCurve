using ForgettingCurve.Api.Filters;
using ForgettingCurve.Application.Abstractions;
using ForgettingCurve.Domain.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using MediatR;
using ForgettingCurve.Application.Commands.Scope;
using ForgettingCurve.Application.Requests;
using ForgettingCurve.Application.Responses;

namespace ForgettingCurve.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class ScopesController : ControllerBase
    {
        private readonly IScopeRepository _scopeRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMediator _mediator;

        public ScopesController(IScopeRepository scopeRepository, IUnitOfWork unitOfWork, IMediator mediator)
        {
            _scopeRepository = scopeRepository;
            _unitOfWork = unitOfWork;
            _mediator = mediator;
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
        [ProducesResponseType(typeof(ScopeResponse), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateScope([FromBody] CreateScopeRequest request)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
            {
                return Unauthorized();
            }

            var command = new CreateScopeCommand
            {
                Name = request.Name,
                UserId = userGuid
            };
            
            var result = await _mediator.Send(command);
            
            return CreatedAtAction(nameof(GetScope), new { id = result.ScopeId }, result);
        }

        [HttpPut("{id}")]
        [RequiresResourceOwnership("id", typeof(IScopeRepository))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateScope(Guid id, [FromBody] UpdateScopeRequest request)
        {
            var existingScope = await _scopeRepository.GetByIdAsync(id);
            
            if (existingScope == null)
            {
                return NotFound();
            }
            
            existingScope.UpdateName(request.Name);
            
            _scopeRepository.Update(existingScope);
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