using VillaAPI.CustomLogging;

namespace VillaAPI.CustomLogging
{
    public class Logging : ILogging
    {
        public void Log(LoggingType type, string message)
        {
            Console.WriteLine($"{type.ToString()}: {message}");
            //Console.BackgroundColor = ConsoleColor.Red;
        }
    }
}
