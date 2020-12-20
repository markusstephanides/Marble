using Marble.Core.Abstractions;
using Microsoft.Extensions.Logging;

namespace Marble.Sandbox
{
    public class MathEntryService : IEntryService
    {
        private readonly ILogger<MathEntryService> logger;

        public MathEntryService(ILogger<MathEntryService> logger)
        {
            this.logger = logger;
        }

        public void OnAppStarted()
        {
            this.logger.LogInformation("MathEntryService initialized");
        }
    }
}