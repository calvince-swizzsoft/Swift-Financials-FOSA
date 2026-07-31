namespace Infrastructure.Crosscutting.Framework.Logging
{
    public class TraceSourceLogFactory : ILoggerFactory
    {
        public ILogger Create()
        {
            return new TraceSourceLog();
        }
    }
}
