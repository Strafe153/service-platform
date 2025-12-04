using MediatR;
using Users.Api.Application.Commands.Update;
using Users.Domain.Aggregates.User;

namespace Users.Api.Application.Commands.Create;

public class UpdateUserCommandHandler : IRequestHandler<IdentifiedCommand<Ulid, UpdateUserCommand>>
{
    private readonly IUsersRepository _usersRepository;

    public UpdateUserCommandHandler(IUsersRepository usersRepository)
    {
        _usersRepository = usersRepository;
    }

    public async Task Handle(
        IdentifiedCommand<Ulid, UpdateUserCommand> request,
        CancellationToken cancellationToken)
    {
        var user = await _usersRepository.GetByIdAsync(request.Id)
            ?? throw new NullReferenceException($"User with id {request.Id} not found.");

        user.Update(
            request.Command.FirstName,
            request.Command.LastName,
            request.Command.PhoneNumber,
            request.Command.BirthDate);

        _usersRepository.Update(user);
        await _usersRepository.SaveChangesAsync(cancellationToken);
    }
}