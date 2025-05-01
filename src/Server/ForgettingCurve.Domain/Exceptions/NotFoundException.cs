using System;

namespace ForgettingCurve.Domain.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException() : base("The requested resource was not found") { }
        
        public NotFoundException(string message) : base(message) { }
        
        public NotFoundException(string name, object key) 
            : base($"Entity '{name}' with key '{key}' was not found") { }
        
        public NotFoundException(string message, Exception innerException) : base(message, innerException) { }
    }
} 