using LoggerDiagram.Services;

public interface IPlcDataReaderFactory
{
    IPlcDataReader Create(string ip);
}