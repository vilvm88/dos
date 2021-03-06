using System;

namespace DoOrSave.Core
{
    public class JobConsoleLogger : IJobLogger
    {
        public void Verbose(string message)
        {
            Console.WriteLine($"[Verbose] {message}");
        }

        public void Debug(string message)
        {
            Console.WriteLine(message);
        }

        public void Information(string message)
        {
            Console.WriteLine($"[Information] {message}");
        }

        public void Warning(string message)
        {
            Console.WriteLine($"[Warning] {message}");
        }

        public void Error(Exception exception)
        {
            Console.WriteLine($"[Error] {exception}");
        }
    }
}
