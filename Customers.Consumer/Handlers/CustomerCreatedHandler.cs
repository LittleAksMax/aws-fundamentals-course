using System.Globalization;
using Contracts;
using MediatR;

namespace Customers.Consumer.Handlers;

public class CustomerCreatedHandler : IRequestHandler<CustomerCreated>
{
    private readonly ILogger<CustomerCreatedHandler> _logger;

    public CustomerCreatedHandler(ILogger<CustomerCreatedHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(CustomerCreated request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Created with ID: {request.Id}", request);
        return Task.CompletedTask;
    }
}