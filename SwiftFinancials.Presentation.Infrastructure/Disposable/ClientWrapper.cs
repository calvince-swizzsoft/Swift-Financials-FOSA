using System;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace SwiftFinancials.Presentation.Infrastructure.Disposable
{
    public class ClientWrapper<TProxy, TService> : DisposableWrapper<TProxy>
        where TProxy : ClientBase<TService>
        where TService : class
    {
        public ClientWrapper(TProxy proxy, IEndpointBehavior behavior, double timeoutMinutes)
            : base(proxy)
        {
            if (behavior != null)
                proxy.Endpoint.EndpointBehaviors.Add(behavior);

            proxy.InnerChannel.OperationTimeout = TimeSpan.FromMinutes(timeoutMinutes);
        }

        protected override void OnDispose()
        {
            try
            {
                if ((this.BaseObject as ICommunicationObject).State != CommunicationState.Faulted)
                {
                    (this.BaseObject as ICommunicationObject).Close();
                }
            }
            finally
            {
                if ((this.BaseObject as ICommunicationObject).State != CommunicationState.Closed)
                {
                    (this.BaseObject as ICommunicationObject).Abort();
                }
            }
        }
    }
}
