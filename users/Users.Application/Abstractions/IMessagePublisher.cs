namespace Users.Application.Abstractions;

public interface IMessagePublisher
{
    Task Publish<T>(T message, string routingKey, CancellationToken cancellationToken)
        where T : notnull;
}