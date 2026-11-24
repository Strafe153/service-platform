using MassTransit;
using Users.Application.Abstractions;

namespace Users.Infrastructure.Messaging;

public class MessagePublisher(IPublishEndpoint publishEndpoint) : IMessagePublisher
{
    public Task Publish<T>(T message, string routingKey, CancellationToken cancellationToken) where T : notnull
    {
        var publishTask = publishEndpoint.Publish(
            message,
            c =>
            {
                c.CorrelationId = Guid.NewGuid();
                c.SetRoutingKey(routingKey);
            },
            cancellationToken);

        return publishTask;
    }
}