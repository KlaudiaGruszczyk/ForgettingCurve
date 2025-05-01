using System;
using System.Threading.Tasks;
using ForgettingCurve.Domain.Entities;

namespace ForgettingCurve.Application.Auth
{
    public interface IJwtTokenGenerator
    {
        Task<(string token, DateTime expiresAt)> GenerateTokenAsync(ApplicationUser user);
    }
} 