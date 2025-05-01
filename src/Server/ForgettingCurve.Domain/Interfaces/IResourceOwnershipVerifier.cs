namespace ForgettingCurve.Domain.Interfaces
{
    public interface IResourceOwnershipVerifier
    {
        Task<bool> IsOwnerAsync(Guid resourceId, Guid userId);
    }
} 