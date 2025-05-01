using System;

namespace ForgettingCurve.Domain.Exceptions
{
    public class UserAlreadyExistsException : Exception
    {
        public UserAlreadyExistsException() 
            : base("User with the provided email already exists")
        {
        }
        
        public UserAlreadyExistsException(string message) 
            : base(message)
        {
        }
        
        public UserAlreadyExistsException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }
    }
} 