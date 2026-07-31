namespace Infrastructure.Crosscutting.Framework.Logging
{
    public interface ILoggerFactory
    {
        ILogger Create();
    }
}
