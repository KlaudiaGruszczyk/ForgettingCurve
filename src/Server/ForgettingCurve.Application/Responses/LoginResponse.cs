using System;

namespace ForgettingCurve.Application.Responses
{
    public class LoginResponse
    {
        public string? Token { get; set; }
        public DateTime ExpiresAt { get; set; }
        public Guid UserId { get; set; }
    }
} 