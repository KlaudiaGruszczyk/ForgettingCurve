using System;

namespace ForgettingCurve.Domain.Identity.Exceptions
{
    public class AccountNotVerifiedException : Exception
    {
        public AccountNotVerifiedException(string message) : base(message) { }
    }
} 