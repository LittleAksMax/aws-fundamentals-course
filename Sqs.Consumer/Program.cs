// See https://aka.ms/new-console-template for more information
using Amazon.SQS; // requires AWSSDK.SQS NuGet package
using Amazon.SQS.Model;

// we can provide new AWSCredentials() as a parameter with our credentials if we aren't
// already authenticated
var sqsClient = new AmazonSQSClient();

var cancellationTokenSource = new CancellationTokenSource();

// we need the queue URL to send messages (listed under URL header when you inspect the queue in AWS)
// but our code could be agnostic of things like region/account number, for which we can use the name
// of the queue only
// we can use the name to find the URL

const string queueName = "Customers"; // case-sensitive
var queueUrlResponse = await sqsClient.GetQueueUrlAsync(queueName);

var receiveMessageRequest = new ReceiveMessageRequest
{
    QueueUrl = queueUrlResponse.QueueUrl,
    // by default, the response message will not include any
    // attributes or details, as it makes the pulling down of
    // messages more efficient, we can specify the ones we want
    // like below
    // "All" means all attributes
    AttributeNames = new List<string>() { "All" },      // things like SenderId
    MessageAttributeNames= new List<string>() { "All" } // things like MessageType
};

while (!cancellationTokenSource.IsCancellationRequested)
{
    var response = await sqsClient.ReceiveMessageAsync(receiveMessageRequest, cancellationTokenSource.Token);
    
    // need to iterate as multiple messages may be polled between delays
    foreach (var message in response.Messages)
    {
        // can access Attributes, Body, etc. of each message
        Console.WriteLine($" Message ID : {message.MessageId}");
        Console.WriteLine($"Message Body: {message.Body}");

        // we now need to delete the message to avoid the 'processed' messages
        // being cycled through again and being reprocessed
        // we need to pass in the queue's URL and the receipt handle of the
        // message processed
        await sqsClient.DeleteMessageAsync(queueUrlResponse.QueueUrl, message.ReceiptHandle);
    }
    await Task.Delay(3000);
}