using System.Text.Json;
using Amazon.SQS;
using Amazon.SQS.Model;
using Contracts;
using Customers.Api.Messaging;
using MediatR;
using Microsoft.Extensions.Options;

namespace Customers.Consumer;

public class QueueConsumerService : BackgroundService
{
    private readonly ILogger<QueueConsumerService> _logger;
    private readonly IAmazonSQS _sqs;
    private readonly IOptions<QueueSettings> _queueSettings;
    private string? _queueUrl; // null by default
    
    // used to find the appropriate handler based on the type of the
    // received message with the MediatR library
    private readonly IMediator _mediator; 

    public QueueConsumerService(IAmazonSQS sqs,
        IOptions<QueueSettings> queueSettings,
        IMediator mediator,
        ILogger<QueueConsumerService> logger)
    {
        _sqs = sqs;
        _queueSettings = queueSettings;
        _mediator = mediator;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var receiveMessageRequest = new ReceiveMessageRequest
        {
            QueueUrl = await GetQueueUrl(stoppingToken),
            AttributeNames = new List<string> { "All" },
            MessageAttributeNames= new List<string> { "All" },
            
            MaxNumberOfMessages = 1, // might want to increase this for batch-processing
        };

        while (!stoppingToken.IsCancellationRequested)
        {
            var response = await _sqs.ReceiveMessageAsync(receiveMessageRequest, stoppingToken);

            foreach (var message in response.Messages)
            {
                // get the type of the message as that may determine how I deal with the message
                // can be CustomerCreated, CustomerUpdated, or CustomerDeleted
                var messageType = message.MessageAttributes["MessageType"].StringValue;
            
                // we can handle the messages effectively and cleanly with the mediator pattern
                // for which we can use the MediatR library
                // load type during run-time, make sure to include namespaces AND the assembly (Contracts)
                var type = Type.GetType($"Contracts.{messageType}, Contracts");

                if (type is null)
                {
                    _logger.LogWarning("Unknown message type: {MessageType}", messageType);
                    continue; // so we don't delete the message in this case
                }

                // ! to suppress invalid nullability warning
                // cast it to the empty ISqsMessage interface so it can be handled by the SqsMessenger
                var typedMessage = (ISqsMessage)JsonSerializer.Deserialize(message.Body, type)!;

                try
                {
                    await _mediator.Send(typedMessage, stoppingToken);
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "Message failed during processing.");
                    continue; // so we don't delete the message in this case
                }
                await _sqs.DeleteMessageAsync(await GetQueueUrl(stoppingToken),
                    message.ReceiptHandle, stoppingToken);
            }
            
            await Task.Delay(1000, stoppingToken);
        }
    }
    
    private async Task<string> GetQueueUrl(CancellationToken cts)
    {
        if (_queueUrl is not null) return _queueUrl;
        
        var queueUrlResponse = await _sqs.GetQueueUrlAsync(_queueSettings.Value.Name, cts);
        _queueUrl = queueUrlResponse.QueueUrl;
        
        return _queueUrl;
    }
}