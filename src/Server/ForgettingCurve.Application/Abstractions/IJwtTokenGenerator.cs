namespace ForgettingCurve.Application.Abstractions
{
    public interface IJwtTokenGenerator
    {
        string GenerateToken(Guid userId, string email);
    }
} 