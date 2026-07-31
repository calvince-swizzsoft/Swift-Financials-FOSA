namespace Infrastructure.Crosscutting.Framework.Logging
{
    public class SerilogLoggerFactory : ILoggerFactory
    {
        private readonly ILogger _logger;

        public SerilogLoggerFactory(ILogger logger)
        {
            _logger = logger;
        }

        public ILogger Create()
        {
            return _logger;
        }
    }
}
