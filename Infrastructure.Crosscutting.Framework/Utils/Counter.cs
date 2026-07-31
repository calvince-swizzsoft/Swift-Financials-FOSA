namespace Infrastructure.Crosscutting.Framework.Utils
{
    public class Counter
    {
        private readonly object SyncRoot = new object();
        private int value;

        public int Value
        {
            get
            {
                lock (this.SyncRoot)
                {
                    return value;
                }
            }
        }

        public int Decrement()
        {
            lock (this.SyncRoot)
            {
                return --value;
            }
        }

        public int Increment()
        {
            lock (this.SyncRoot)
            {
                return ++value;
            }
        }
    }
}
