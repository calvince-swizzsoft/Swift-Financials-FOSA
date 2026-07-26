using System;

namespace SwiftFinancials.Presentation.Infrastructure.Disposable
{
    public interface IDisposableWrapper<T> : IDisposable
    {
        T BaseObject { get; }
    }
}
