namespace Users.Domain.Aggregates.User;

public sealed class User : Entity<Ulid>, IAggregateRoot
{
    public string Email { get; private set; }

    public string FirstName { get; private set; }

    public string LastName { get; private set; }

    public string PhoneNumber { get; private set; }

    public DateOnly BirthDate { get; private set; }

    public string AuthProviderId { get; private set; }

    public Address Address { get; private set; }

#pragma warning disable CS8618
    private User() : base(Ulid.NewUlid())
    {
    }
#pragma warning restore CS8618 

    public User(
        string email,
        string firstName, 
        string lastName,
        string phoneNumber,
        DateOnly birthDate,
        string authProviderId,
        Address address) : base(Ulid.NewUlid())
    {
        Email = email;
        FirstName = firstName;
        LastName = lastName;
        PhoneNumber = phoneNumber;
        BirthDate = birthDate;
        AuthProviderId = authProviderId;
        Address = address;
    }
}