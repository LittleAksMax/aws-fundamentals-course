using Contracts;
using Customers.Api.Domain;

namespace Customers.Api.Mapping;

public static class DomainToMessageMapper
{
    public static CustomerCreated ToCustomerCreatedMessage(this Customer customer)
    {
        return new CustomerCreated
        {
            Id = customer.Id,
            GitHubUserName = customer.GitHubUsername,
            Email = customer.Email,
            FullName = customer.FullName,
            DoB = customer.DateOfBirth
        };
    }

    public static CustomerUpdated ToCustomerUpdatedMessage(this Customer customer)
    {
        return new CustomerUpdated
        {
            Id = customer.Id,
            FullName = customer.FullName,
            Email = customer.Email,
            GitHubUserName = customer.GitHubUsername,
            DoB = customer.DateOfBirth
        };
    }
}