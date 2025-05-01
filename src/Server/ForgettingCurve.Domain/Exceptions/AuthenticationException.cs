using System;

namespace ForgettingCurve.Domain.Identity.Exceptions
{
    public class AuthenticationException : Exception
    {
        public AuthenticationException(string message) : base(message) { }
    }
} 