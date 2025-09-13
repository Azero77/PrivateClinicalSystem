using ClinicApp.Application.Common;
using ClinicApp.Domain.Common.Entities;
using ClinicApp.Domain.Repositories;
using ErrorOr;
using MediatR;

namespace ClinicApp.Application.Commands.RoomCommands;

public record CreateRoomCommand(string Name) : IRequest<ErrorOr<Room>>;

public class CreateRoomCommandHandler : IRequestHandler<CreateRoomCommand, ErrorOr<Room>>
{
    private readonly IRoomRepository _roomRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateRoomCommandHandler(IRoomRepository roomRepository, IUnitOfWork unitOfWork)
    {
        _roomRepository = roomRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Room>> Handle(CreateRoomCommand request, CancellationToken cancellationToken)
    {
        var room = new Room(Guid.NewGuid(), request.Name);
        var createdRoom = await _roomRepository.AddRoom(room);
        if (createdRoom is null)
        {
            return Error.Failure("Room.NotCreated", "Failed to create room.");
        }
        await _unitOfWork.SaveChangesAsync(cancellationToken, createdRoom);
        return createdRoom;
    }
}
