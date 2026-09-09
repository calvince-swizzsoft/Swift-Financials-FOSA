using System;

namespace Application.Seedwork
{
    // A deliberate business rejection whose message is safe for the caller.
    public sealed class TransactionAuthorityException : InvalidOperationException
    {
        public TransactionAuthorityException(string message) : base(message) { }
    }
}
