namespace Customers.Api.Messaging;

public class TopicSettings
{
    // the key is the key of the key-value pairing under
    // this section of the appsettings.json file
    public const string Key = "Topic";
    public required string Name { get; init; }
}