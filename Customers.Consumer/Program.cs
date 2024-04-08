using System.Reflection;
using Amazon.SQS;
using Customers.Api.Messaging;
using Customers.Consumer;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<QueueSettings>(builder.Configuration.GetSection(QueueSettings.Key));
builder.Services.AddSingleton<IAmazonSQS, AmazonSQSClient>();

// this is the background service that can continuously run
// to poll for new queue messages
builder.Services.AddHostedService<QueueConsumerService>();

// register MediatR with the 'marker' as the current assembly.
builder.Services.AddMediatR(config =>
{
    config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
});

var app = builder.Build();

app.Run();