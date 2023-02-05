using Prism.Events;

namespace TextBlaster.ProcessList;

internal class ProcessSelectedEvent : PubSubEvent<ProcessSelectedEventArgs>
{
}

internal class ProcessSelectedEventArgs
{
    public string? ProcessName { get; set; }
    public int? ProcessId { get; set; }
}
