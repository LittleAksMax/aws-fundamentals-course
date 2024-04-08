using System.Text.Json;
using Amazon.SimpleNotificationService;
using Amazon.SimpleNotificationService.Model;
using Contracts;

var customer = new CustomerCreated
{
    Id = Guid.NewGuid(),
    Email = "test@example.com",
    FullName = "Test McTesterson",
    DoB = new DateTime(1999, 1, 1),
    GitHubUserName = "TestCoder"
};

var snsClient = new AmazonSimpleNotificationServiceClient();

var topicArnResponse = await snsClient.FindTopicAsync("Customers");

if (topicArnResponse is not null)
{
    var publishRequest = new PublishRequest
    {
        TopicArn = topicArnResponse.TopicArn,
        Message = JsonSerializer.Serialize(customer),
        MessageAttributes = new Dictionary<string, MessageAttributeValue>
        {
            {
                "MessageType",
                new MessageAttributeValue
                {
                    DataType = "String",
                    StringValue = nameof(CustomerCreated)
                }
            }
        }
    };

    var response = await snsClient.PublishAsync(publishRequest);
}
else
{
    Console.WriteLine("Topic not found!");
}