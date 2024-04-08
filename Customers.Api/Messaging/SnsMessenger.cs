using System.Text.Json;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Contracts;
using Microsoft.Extensions.Options;

namespace Customers.Api.Messaging;

public class SnsMessenger : ISnsMessenger
{
    private readonly IAmazonSimpleNotificationService _sns;
    private readonly IOptions<TopicSettings> _topicSettings;
    private string? _topicArn; // null by default


    public SnsMessenger(IAmazonSimpleNotificationService sns, IOptions<TopicSettings> topicSettings)
    {
        _sns           = sns;
        _topicSettings = topicSettings;
    }

    public async Task<PublishResponse> PublishMessageAsync<T>(T message) where T : ISqsMessage
    {
        var publishMessageRequest = new PublishRequest
        {
            TopicArn = await GetTopicArnAsync(),
            Message = JsonSerializer.Serialize(message),
            MessageAttributes = new Dictionary<string, MessageAttributeValue>
            {
                {
                    "MessageType",
                    new MessageAttributeValue
                    {
                        DataType = "String",
                        StringValue = typeof(T).Name // needs to be typeof not nameof as nameof is compile time,
                                                     // and we don't know T at compile time
                    }
                }
            }
        };

        return await _sns.PublishAsync(publishMessageRequest);
    }
    
    // ValueTask is more memory efficient since we are actually
    // caching the response
    private async ValueTask<string> GetTopicArnAsync()
    {
        if (_topicArn is not null) return _topicArn;
        
        var topicArnResponse = await _sns.FindTopicAsync(_topicSettings.Value.Name);
        _topicArn = topicArnResponse.TopicArn;
        
        return _topicArn;
    }
}