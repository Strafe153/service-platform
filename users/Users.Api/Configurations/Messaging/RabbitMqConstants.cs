namespace Users.Api.Configurations.Messaging;

public static class RabbitMqConstants
{
    public static class Exchanges
    {
        public const string User = "user";
    }

    public static class RoutingKeys
    {
        public const string UserCreated = "user.created";
        public const string UserDeleted = "user.deleted";
    }
}