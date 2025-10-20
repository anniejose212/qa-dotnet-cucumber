using System;
using System.Collections.Generic;
using System.Net;   

namespace qa_dotnet_cucumber.Support
{
    public abstract class StepBase
    {
        private readonly List<string> _logBuffer = new();

        public IReadOnlyList<string> GetLogs() => _logBuffer;
        public void ClearLogs() => _logBuffer.Clear();

        public void Info(string message)
        {
            // Encode once at log origin — prevents all XSS in reports
            var safe = WebUtility.HtmlEncode(message ?? string.Empty);

            _logBuffer.Add(safe);
            Console.WriteLine($"[INFO] {message}"); // Keep raw in console for dev clarity
        }
    }
}
