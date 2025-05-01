using System;

namespace ForgettingCurve.Domain.Identity.Exceptions
{
    public class AccountLockedException : Exception
    {
        public DateTimeOffset? LockoutEnd { get; }
        
        public AccountLockedException(string message, DateTimeOffset? lockoutEnd = null) : base(message)
        {
            LockoutEnd = lockoutEnd;
        }
    }
} 