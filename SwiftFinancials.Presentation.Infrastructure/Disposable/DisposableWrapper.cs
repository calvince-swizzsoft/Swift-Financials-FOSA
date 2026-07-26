using System;

namespace SwiftFinancials.Presentation.Infrastructure.Disposable
{
    public class DisposableWrapper<T> : IDisposableWrapper<T> where T : class, IDisposable
    {
        public T BaseObject { get; private set; }

        public DisposableWrapper(T baseObject)
        {
            BaseObject = baseObject;
        }

        protected virtual void OnDispose()
        {
            BaseObject.Dispose();
        }

        public void Dispose()
        {
            if (BaseObject != null)
            {
                try
                {
                    OnDispose();
                }
                catch { } // swallow...
            }

            BaseObject = null;
        }
    }
}
