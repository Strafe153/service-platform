using MediatR;

namespace Users.Api.Application.Commands.UpdateAddress;

public sealed record UpdateAddressCommand(
    string Country,
    string State,
    string City,
    string ZipCode,
    string? Street) : IRequest;