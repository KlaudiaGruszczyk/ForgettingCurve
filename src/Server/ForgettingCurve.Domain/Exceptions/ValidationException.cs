using System;
using System.Collections.Generic;

namespace ForgettingCurve.Domain.Exceptions
{
    public class ValidationException : Exception
    {
        public ValidationException() : base("One or more validation failures have occurred") 
        { 
            Errors = new Dictionary<string, string[]>();
        }
        
        public ValidationException(string message) : base(message) 
        { 
            Errors = new Dictionary<string, string[]>();
        }
        
        public ValidationException(string message, Exception innerException) : base(message, innerException) 
        { 
            Errors = new Dictionary<string, string[]>();
        }
        
        public ValidationException(IDictionary<string, string[]> errors) : base("One or more validation failures have occurred") 
        { 
            Errors = errors;
        }
        
        public IDictionary<string, string[]> Errors { get; }
    }
} 