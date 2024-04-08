using Contracts;
using Customers.Api.Domain;
using Customers.Api.Mapping;
using Customers.Api.Messaging;
using Customers.Api.Repositories;
using FluentValidation;
using FluentValidation.Results;

namespace Customers.Api.Services;

public class CustomerService : ICustomerService
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IGitHubService _gitHubService;
    private readonly ISnsMessenger _snsMessenger; // injected messenger into service

    public CustomerService(ICustomerRepository customerRepository, 
        IGitHubService gitHubService, ISnsMessenger snsMessenger)
    {
        _customerRepository = customerRepository;
        _gitHubService = gitHubService;
        _snsMessenger = snsMessenger;
    }

    public async Task<bool> CreateAsync(Customer customer)
    {
        var existingUser = await _customerRepository.GetAsync(customer.Id);
        if (existingUser is not null)
        {
            var message = $"A user with id {customer.Id} already exists";
            throw new ValidationException(message, GenerateValidationError(nameof(Customer), message));
        }

        var isValidGitHubUser = await _gitHubService.IsValidGitHubUser(customer.GitHubUsername);
        if (!isValidGitHubUser)
        {
            var message = $"There is no GitHub user with username {customer.GitHubUsername}";
            throw new ValidationException(message, GenerateValidationError(nameof(customer.GitHubUsername), message));
        }
        
        var customerDto = customer.ToCustomerDto();
        var success =  await _customerRepository.CreateAsync(customerDto);
        if (success)
        {
            await _snsMessenger.PublishMessageAsync(customer.ToCustomerCreatedMessage());
        }

        return success;
    }

    public async Task<Customer?> GetAsync(Guid id)
    {
        var customerDto = await _customerRepository.GetAsync(id);
        return customerDto?.ToCustomer();
    }

    public async Task<IEnumerable<Customer>> GetAllAsync()
    {
        var customerDtos = await _customerRepository.GetAllAsync();
        return customerDtos.Select(x => x.ToCustomer());
    }

    public async Task<bool> UpdateAsync(Customer customer)
    {
        var customerDto = customer.ToCustomerDto();
        
        var isValidGitHubUser = await _gitHubService.IsValidGitHubUser(customer.GitHubUsername);
        if (!isValidGitHubUser)
        {
            var message = $"There is no GitHub user with username {customer.GitHubUsername}";
            throw new ValidationException(message, GenerateValidationError(nameof(customer.GitHubUsername), message));
        }
        
        var success =  await _customerRepository.UpdateAsync(customerDto);
        if (success)
        {
            await _snsMessenger.PublishMessageAsync(customer.ToCustomerUpdatedMessage());
        }

        return success;
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var success = await _customerRepository.DeleteAsync(id);
        if (success)
        {
            await _snsMessenger.PublishMessageAsync(new CustomerDeleted
            {
                Id = id
            });
        }

        return success;
    }

    private static ValidationFailure[] GenerateValidationError(string paramName, string message)
    {
        return new []
        {
            new ValidationFailure(paramName, message)
        };
    }
}
