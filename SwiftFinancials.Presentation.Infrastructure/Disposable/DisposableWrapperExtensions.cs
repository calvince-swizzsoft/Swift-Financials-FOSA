using System;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace SwiftFinancials.Presentation.Infrastructure.Disposable
{
    public static class DisposableWrapperExtensions
    {
        public static IDisposableWrapper<T> Wrap<T>(this T baseObject)
            where T : class, IDisposable
        {
            return baseObject as IDisposableWrapper<T> ?? new DisposableWrapper<T>(baseObject);
        }

        public static IDisposableWrapper<TProxy> Wrap<TProxy, TService>(this TProxy proxy, double timeoutMinutes)
            where TProxy : ClientBase<TService>
            where TService : class
        {
            return new ClientWrapper<TProxy, TService>(proxy, null, timeoutMinutes);
        }

        public static IDisposableWrapper<TProxy> Wrap<TProxy, TService>(this TProxy proxy, IEndpointBehavior behavior, double timeoutMinutes)
            where TProxy : ClientBase<TService>
            where TService : class
        {
            return new ClientWrapper<TProxy, TService>(proxy, behavior, timeoutMinutes);
        }
    }
}
