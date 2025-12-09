using MediatR;
using Users.Domain.Aggregates.User;

namespace Users.Api.Application.Commands.UpdateAddress;

public class UpdateAddressCommandHandler
    : IRequestHandler<IdentifiedCommand<Ulid, UpdateAddressCommand>>
{
    private readonly IUsersRepository _usersRepository;

    public UpdateAddressCommandHandler(IUsersRepository usersRepository)
    {
        _usersRepository = usersRepository;
    }

    public async Task Handle(
        IdentifiedCommand<Ulid, UpdateAddressCommand> request,
        CancellationToken cancellationToken)
    {
        var user = await _usersRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NullReferenceException($"User with id {request.Id} not found.");

        user.UpdateAddress(
            request.Command.Country,
            request.Command.State,
            request.Command.City,
            request.Command.ZipCode,
            request.Command.Street);

        _usersRepository.Update(user);
        await _usersRepository.SaveChangesAsync(cancellationToken);
    }
}