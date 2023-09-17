using Serilog;


namespace PianoRollMIDIConverter.Models.Utils
{
public sealed class LoggerSingleton
{
    private static LoggerSingleton _instance = null;
    private static readonly object lockObject = new();
    private ILogger _logger;

    private LoggerSingleton()
    {
        _logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .WriteTo.File("log.txt", rollingInterval: RollingInterval.Day)
            .CreateLogger();
    }

    public static LoggerSingleton Instance
    {
        get
        {
            lock (lockObject)
            {
                _instance ??= new LoggerSingleton();
                return _instance;
            }
        }
    }

    public ILogger Logger => _logger;
}
}