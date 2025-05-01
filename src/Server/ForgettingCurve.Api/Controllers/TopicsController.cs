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
    [Route("api/scopes/{scopeId}/topics")]
    public class TopicsController : ControllerBase
    {
        private readonly ITopicRepository _topicRepository;
        private readonly IScopeRepository _scopeRepository;
        private readonly IUnitOfWork _unitOfWork;

        public TopicsController(
            ITopicRepository topicRepository, 
            IScopeRepository scopeRepository, 
            IUnitOfWork unitOfWork)
        {
            _topicRepository = topicRepository;
            _scopeRepository = scopeRepository;
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        [RequiresResourceOwnership("scopeId", typeof(IScopeRepository))]
        [ProducesResponseType(typeof(IEnumerable<Topic>), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetTopics(Guid scopeId)
        {
            var topics = await _topicRepository.GetTopicsByScopeIdAsync(scopeId);
            return Ok(topics);
        }

        [HttpGet("{id}")]
        [RequiresResourceOwnership("id", typeof(ITopicRepository))]
        [ProducesResponseType(typeof(Topic), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetTopic(Guid scopeId, Guid id)
        {
            var topic = await _topicRepository.GetByIdAsync(id);
            
            if (topic == null)
            {
                return NotFound();
            }
            
            return Ok(topic);
        }

        [HttpPost]
        [RequiresResourceOwnership("scopeId", typeof(IScopeRepository))]
        [ProducesResponseType(typeof(Topic), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CreateTopic(Guid scopeId, [FromBody] Topic topic)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            
            if (string.IsNullOrEmpty(userId) || !Guid.TryParse(userId, out var userGuid))
            {
                return Unauthorized();
            }

            var scope = await _scopeRepository.GetByIdAsync(scopeId);
            if (scope == null || scope.OwnerUserId != userGuid)
            {
                return NotFound();
            }

            topic.ScopeId = scopeId;
            topic.OwnerUserId = userGuid;
            topic.CreationDate = DateTime.UtcNow;
            
            await _topicRepository.AddAsync(topic);
            await _unitOfWork.SaveChangesAsync();
            
            return CreatedAtAction(nameof(GetTopic), new { scopeId = scopeId, id = topic.Id }, topic);
        }

        [HttpPut("{id}")]
        [RequiresResourceOwnership("id", typeof(ITopicRepository))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdateTopic(Guid scopeId, Guid id, [FromBody] Topic topic)
        {
            if (id != topic.Id)
            {
                return BadRequest();
            }

            var existingTopic = await _topicRepository.GetByIdAsync(id);
            if (existingTopic == null)
            {
                return NotFound();
            }

            topic.OwnerUserId = existingTopic.OwnerUserId;
            topic.ScopeId = scopeId;
            topic.CreationDate = existingTopic.CreationDate;
            topic.LastModifiedDate = DateTime.UtcNow;
            
            _topicRepository.Update(topic);
            await _unitOfWork.SaveChangesAsync();
            
            return NoContent();
        }

        [HttpDelete("{id}")]
        [RequiresResourceOwnership("id", typeof(ITopicRepository))]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteTopic(Guid scopeId, Guid id)
        {
            var topic = await _topicRepository.GetByIdAsync(id);
            if (topic == null)
            {
                return NotFound();
            }
            
            _topicRepository.Remove(topic);
            await _unitOfWork.SaveChangesAsync();
            
            return NoContent();
        }
    }
} 