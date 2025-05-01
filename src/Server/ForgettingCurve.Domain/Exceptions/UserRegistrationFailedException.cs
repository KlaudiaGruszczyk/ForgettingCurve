using System;

namespace ForgettingCurve.Domain.Exceptions
{
    public class UserRegistrationFailedException : Exception
    {
        public string[] Errors { get; }
        
        public UserRegistrationFailedException(string[] errors) 
            : base("User registration failed")
        {
            Errors = errors;
        }
        
        public UserRegistrationFailedException(string message, string[] errors = null) 
            : base(message)
        {
            Errors = errors ?? Array.Empty<string>();
        }
        
        public UserRegistrationFailedException(string message, Exception innerException, string[] errors = null) 
            : base(message, innerException)
        {
            Errors = errors ?? Array.Empty<string>();
        }
    }
} 