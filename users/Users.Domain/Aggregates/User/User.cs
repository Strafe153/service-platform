namespace Users.Domain.Aggregates.User;

public sealed class User : Entity<Guid>, IAggregateRoot
{
    public string Email { get; private set; }

    public string FirstName { get; private set; }

    public string LastName { get; private set; }

    public string PhoneNumber { get; private set; }

    public DateOnly BirthDate { get; private set; }

    public string AuthProviderId { get; private set; }

    public Address Address { get; private set; }

    // This constructor is needed for EF to run migrations,
    // however the warnings appear due to properties not being set in here
#pragma warning disable CS8618
    private User() : base(Guid.NewGuid())
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
        Address address) : base(Guid.NewGuid())
    {
        Email = email;
        FirstName = firstName;
        LastName = lastName;
        PhoneNumber = phoneNumber;
        BirthDate = birthDate;
        AuthProviderId = authProviderId;
        Address = address;
    }

    public void Update(
        string firstName,
        string lastName,
        string phoneNumber,
        DateOnly birthDate)
    {
        FirstName = firstName;
        LastName = lastName;
        PhoneNumber = phoneNumber;
        BirthDate = birthDate;
    }

    public void UpdateAddress(
        string country,
        string state,
        string city,
        string zipCode,
        string? street) =>
            Address = new(country, state, city, zipCode, street);
}