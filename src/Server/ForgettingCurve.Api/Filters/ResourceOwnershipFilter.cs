using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;

namespace ForgettingCurve.Api.Filters
{
    public class ResourceOwnershipFilter : IAsyncActionFilter
    {
        private readonly IServiceProvider _serviceProvider;

        public ResourceOwnershipFilter(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var requiresOwnershipAttribute = context.ActionDescriptor.EndpointMetadata
                .OfType<RequiresResourceOwnershipAttribute>()
                .FirstOrDefault();

            if (requiresOwnershipAttribute == null)
            {
                await next();
                return;
            }

            var userId = context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            if (!context.ActionArguments.TryGetValue(requiresOwnershipAttribute.ResourceIdParameter, out var resourceIdObj))
            {
                context.Result = new BadRequestResult();
                return;
            }

            if (!Guid.TryParse(resourceIdObj?.ToString(), out var resourceId))
            {
                context.Result = new BadRequestResult();
                return;
            }

            var repositoryType = requiresOwnershipAttribute.RepositoryType;
            var repository = _serviceProvider.GetService(repositoryType);
            if (repository == null)
            {
                throw new InvalidOperationException($"Repository of type {repositoryType.Name} not registered.");
            }

            var isOwnerMethod = repositoryType.GetMethod("IsOwner");
            if (isOwnerMethod == null)
            {
                throw new InvalidOperationException($"Method IsOwner not found on repository type {repositoryType.Name}.");
            }

            var isOwner = (bool)(isOwnerMethod.Invoke(repository, new object[] { resourceId, Guid.Parse(userId) }) 
                ?? throw new InvalidOperationException("IsOwner method returned null."));

            if (!isOwner)
            {
                context.Result = new NotFoundResult();
                return;
            }

            await next();
        }
    }
} 