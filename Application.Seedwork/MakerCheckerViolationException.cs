using System;

namespace Application.Seedwork
{
    public sealed class MakerCheckerViolationException : InvalidOperationException
    {
        public MakerCheckerViolationException()
            : base("The user who initiated or most recently approved this sequential process cannot approve the next step. A different authorized user must complete it.")
        {
        }
    }
}
