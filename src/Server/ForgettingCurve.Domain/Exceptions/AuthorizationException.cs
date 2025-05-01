using System;

namespace ForgettingCurve.Domain.Exceptions
{
    public class AuthorizationException : Exception
    {
        public AuthorizationException() : base("You are not authorized to access this resource") { }
        
        public AuthorizationException(string message) : base(message) { }
        
        public AuthorizationException(string message, Exception innerException) : base(message, innerException) { }
    }
} 