using Serilog.Core;
using Serilog.Events;

namespace BuildingBlock.Extensions
{
    public class IgnorePropertiesEnricher : ILogEventEnricher
    {
        private readonly string[] _propertiesToIgnore;

        public IgnorePropertiesEnricher(params string[] propertiesToIgnore)
        {
            _propertiesToIgnore = propertiesToIgnore;
        }

        public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
        {
            foreach (var prop in _propertiesToIgnore)
            {
                // Remove unwanted properties
                logEvent.RemovePropertyIfPresent(prop);
            }
        }
    }
}
