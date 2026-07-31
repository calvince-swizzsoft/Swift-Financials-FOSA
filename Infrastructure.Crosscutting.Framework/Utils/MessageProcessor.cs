using System;
using System.Linq;
using System.Messaging;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Crosscutting.Framework.Utils
{
    public abstract class MessageProcessor<TMessage>
    {
        private readonly MessageQueue[] Receivers;
        private readonly Counter ProcessingCounter = new Counter();
        private bool IsClosing;

        public MessageProcessor()
        { }

        public MessageProcessor(string path)
            : this(path, 1)
        { }

        public MessageProcessor(string path, int count)
            : base()
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentNullException("path");

            if (!MessageQueue.Exists(path))
                MessageQueue.Create(path, true);

            this.Receivers = Enumerable.Range(0, (count <= 0) ? 1 : count)
                .Select(i =>
                {
                    var queue = new MessageQueue(path, QueueAccessMode.Receive)
                    {
                        Formatter = new BinaryMessageFormatter()
                    };

                    queue.MessageReadPropertyFilter.SetAll();

                    return queue;

                }).ToArray();
        }

        public void Close()
        {
            this.IsClosing = true;

            this.OnClosing();

            foreach (var queue in this.Receivers)
            {
                queue.PeekCompleted -= Queue_PeekCompleted;
                queue.Close();
            }

            while (this.IsProcessing)
                Thread.Sleep(100);

            this.IsClosing = this.IsOpen = false;
            this.OnClosed();
        }

        public bool IsOpen { get; private set; }

        protected bool IsProcessing
        {
            get { return this.ProcessingCounter.Value > 0; }
        }

        protected virtual void OnClosing() { }
        protected virtual void OnClosed() { }
        protected virtual void OnOpening() { }
        protected virtual void OnOpened() { }

        public void Open()
        {
            if (this.IsOpen)
                throw new Exception("This processor is already open.");

            this.OnOpening();

            foreach (var queue in this.Receivers)
            {
                queue.PeekCompleted += Queue_PeekCompleted;
                queue.BeginPeek();
            }

            this.IsOpen = true;
            this.OnOpened();
        }

        protected abstract Task Process(TMessage @object, int appSpecific);
        protected abstract void LogError(Exception exception);

        private void Handle(Message message)
        {
            if (message == null) return;

            this.ProcessingCounter.Increment();
            try
            {
                var workerTask = this.Process((TMessage)message.Body, message.AppSpecific);

                Task.WaitAll(workerTask);
            }
            finally
            {
                this.ProcessingCounter.Decrement();
            }
        }

        private void Queue_PeekCompleted(object sender, PeekCompletedEventArgs e)
        {
            var queue = (MessageQueue)sender;

            var transaction = new MessageQueueTransaction();
            transaction.Begin();
            try
            {
                // if the queue closes after the transaction begins,
                // but before the call to Receive, then an exception
                // will be thrown and the transaction will be aborted
                // leaving the message to be processed next time
                this.Handle(queue.Receive(transaction));
                transaction.Commit();
            }
            catch (Exception ex)
            {
                this.LogError(ex);
                transaction.Abort();
            }
            finally
            {
                if (!this.IsClosing)
                    queue.BeginPeek();
            }
        }
    }
}
