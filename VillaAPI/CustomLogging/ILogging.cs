namespace VillaAPI.CustomLogging
{
    public enum LoggingType
    {
        Information,
        Error,
        Warning
    }
    public interface ILogging
    {
        void Log(LoggingType type, string message);
    }
}
