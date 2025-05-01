using MediatR;
using ForgettingCurve.Application.Abstractions;
using ForgettingCurve.Application.Responses;

namespace ForgettingCurve.Application.Commands.Scope
{
    public class CreateScopeCommandHandler : IRequestHandler<CreateScopeCommand, ScopeResponse>
    {
        private readonly IScopeRepository _scopeRepository;
        private readonly IUnitOfWork _unitOfWork;

        public CreateScopeCommandHandler(IScopeRepository scopeRepository, IUnitOfWork unitOfWork)
        {
            _scopeRepository = scopeRepository;
            _unitOfWork = unitOfWork;
        }

        public async Task<ScopeResponse> Handle(CreateScopeCommand request, CancellationToken cancellationToken)
        {
            var scope = new ForgettingCurve.Domain.Entities.Scope(request.UserId, request.Name);
            
            await _scopeRepository.AddAsync(scope);
            await _unitOfWork.SaveChangesAsync();
            
            return new ScopeResponse
            {
                ScopeId = scope.Id,
                Name = scope.Name,
                CreationDate = scope.CreationDate
            };
        }
    }
} 