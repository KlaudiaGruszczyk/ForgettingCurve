using System;

namespace ForgettingCurve.Domain.Exceptions
{
    public class InvalidVerificationTokenException : Exception
    {
        public InvalidVerificationTokenException() 
            : base("The verification token is invalid or has expired")
        {
        }
        
        public InvalidVerificationTokenException(string message) 
            : base(message)
        {
        }
        
        public InvalidVerificationTokenException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }
    }
} 