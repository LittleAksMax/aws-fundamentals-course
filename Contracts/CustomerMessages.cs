namespace Contracts;

public sealed class CustomerCreated : ISqsMessage
{
    public required Guid Id { get; init; }
    public required string FullName { get; init; }
    public required string Email { get; init; }
    public required string GitHubUserName { get; init; }
    public required DateTime DoB { get; init;}
}

public sealed class CustomerUpdated : ISqsMessage
{
    public required Guid Id { get; init; }
    public required string FullName { get; init; }
    public required string Email { get; init; }
    public required string GitHubUserName { get; init; }
    public required DateTime DoB { get; init;}
}

public sealed class CustomerDeleted : ISqsMessage
{
    public required Guid Id { get; init; }
}