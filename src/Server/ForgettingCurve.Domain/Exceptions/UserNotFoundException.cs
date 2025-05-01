using System;

namespace ForgettingCurve.Domain.Exceptions
{
    public class UserNotFoundException : Exception
    {
        public string Email { get; }
        
        public UserNotFoundException(string email) 
            : base($"User with email {email} was not found")
        {
            Email = email;
        }
        
        public UserNotFoundException(string message, string email) 
            : base(message)
        {
            Email = email;
        }
        
        public UserNotFoundException(string message, Exception innerException, string email) 
            : base(message, innerException)
        {
            Email = email;
        }
    }
} 