using Serilog;
using System.Collections.Concurrent;

namespace BuildingBlock.Extensions
{
    public static class LoggerExtension
    {
        private static readonly ConcurrentDictionary<string, ILogger> _loggerCache = new();

        public static ILogger GetLogger(string connectionString, string databaseName, string collectionName)
        {
            var logger = _loggerCache.GetOrAdd(collectionName, name =>
            new LoggerConfiguration()
               .Enrich.With(new IgnorePropertiesEnricher("MessageTemplate", "RenderedMessage", "Timestamp", "UtcTimestamp"))
               .WriteTo.MongoDB(databaseUrl: $"{connectionString}/{databaseName}", collectionName: name)
               .CreateLogger());

            return logger;
        }
    }
}
