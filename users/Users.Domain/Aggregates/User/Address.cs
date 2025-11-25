
namespace Users.Domain.Aggregates.User;

public sealed class Address : ValueObject
{
    public string Country { get; private set; }

    public string State { get; private set; }

    public string City { get; private set; }

    public string ZipCode { get; private set; }

    public string? Street { get; private set; }

    public Address(
        string country,
        string state,
        string city,
        string zipCode,
        string? street = null)
    {
        Country = country;
        State = state;
        City = city;
        ZipCode = zipCode;
        Street = street;
    }

    public override IEnumerable<object> GetEqualityComponents()
    {
        yield return Country;
        yield return State;
        yield return City;
        yield return ZipCode;
        yield return Street ?? string.Empty;
    }
}