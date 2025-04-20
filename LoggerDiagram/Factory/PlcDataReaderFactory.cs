using LoggerDiagram.Services;
using NLog;

public class PlcDataReaderFactory : IPlcDataReaderFactory
{
    private readonly ILogger _logger;

    public PlcDataReaderFactory(ILogger logger)
    {
        _logger = logger;
    }

    public IPlcDataReader Create(string ip)
    {
        return new PlcDataReader(ip, _logger);
    }
}