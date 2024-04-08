// See https://aka.ms/new-console-template for more information
using Amazon.SQS; // requires AWSSDK.SQS NuGet package
using Amazon.SQS.Model;
using Contracts;
using System.Text.Json;


// we can provide new AWSCredentials() as a parameter with our credentials if we aren't
// already authenticated
var sqsClient = new AmazonSQSClient();

var customer = new CustomerCreated
{
    Id = Guid.NewGuid(),
    Email = "example@gmail.com",
    FullName = "Example McExample",
    DoB = new DateTime(1999, 1, 1),
    GitHubUserName = "ExampleCodes"
};

// we need the queue URL to send messages (listed under URL header when you inspect the queue in AWS)
// but our code could be agnostic of things like region/account number, for which we can use the name
// of the queue only
// we can use the name to find the URL

const string queueName = "Customers"; // case-sensitive
var queueUrlResponse = await sqsClient.GetQueueUrlAsync(queueName);

var sendMessageRequest = new SendMessageRequest
{
    QueueUrl = queueUrlResponse.QueueUrl,
    MessageBody = JsonSerializer.Serialize(customer), // the contents of our message in JSON
    

    // DelaySeconds = 5, // to delay message (could add some jitter)

    // MessageDeduplicationId // refers to FIFO queues only
    // MessageGroupId         // refers to FIFO queues only

    // MessageSystemAttributes // dictionary referring to details under 'Details' tab of messages

    // these attributes of the message will show up under the 'Attributes' tab of the messages
    MessageAttributes = new Dictionary<string, MessageAttributeValue>
    {
        {
            // in any case where the type of the message is important (it is for most CRUD)
            "MessageType",
            new MessageAttributeValue
            {
                DataType = "String", // must be String/Number/Binary/define own custom one (cringe)
                StringValue = nameof (CustomerCreated)
            }
        }
    }
};

// break-point + inspect the below
// ReSharper disable once UnusedVariable
var response = await sqsClient.SendMessageAsync(sendMessageRequest);

// Console.ReadLine();