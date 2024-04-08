using Amazon.SimpleNotificationService.Model;
using Contracts;

namespace Customers.Api.Messaging;

public interface ISnsMessenger
{
    Task<PublishResponse> PublishMessageAsync<T>(T message) where T : ISqsMessage;
}