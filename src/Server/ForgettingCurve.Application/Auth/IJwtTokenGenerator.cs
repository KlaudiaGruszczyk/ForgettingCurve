using ForgettingCurve.Domain.Entities;
using System;
using System.Threading.Tasks;

namespace ForgettingCurve.Application.Auth
{
    public interface IJwtTokenGenerator
    {
        Task<(string token, DateTime expiresAt)> GenerateTokenAsync(ApplicationUser user);
    }
} 