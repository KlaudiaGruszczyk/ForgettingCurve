using MediatR;
using ForgettingCurve.Application.Responses;

namespace ForgettingCurve.Application.Commands.Scope
{
    public class CreateScopeCommand : IRequest<ScopeResponse>
    {
        public string Name { get; set; }
        public Guid UserId { get; set; }
    }
} 