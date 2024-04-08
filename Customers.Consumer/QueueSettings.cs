namespace Customers.Api.Messaging;

public class QueueSettings
{
    // the key is the key of the key-value pairing under
    // this section of the appsettings.json file
    public const string Key = "Queue";
    public required string Name { get; init; }
}