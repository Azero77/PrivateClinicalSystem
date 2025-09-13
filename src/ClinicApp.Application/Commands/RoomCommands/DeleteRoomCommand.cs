using ClinicApp.Application.Common;
using ClinicApp.Domain.Repositories;
using ErrorOr;
using MediatR;

namespace ClinicApp.Application.Commands.RoomCommands;

public record DeleteRoomCommand(Guid Id) : IRequest<ErrorOr<Success>>;

public class DeleteRoomCommandHandler : IRequestHandler<DeleteRoomCommand, ErrorOr<Success>>
{
    private readonly IRoomRepository _roomRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteRoomCommandHandler(IRoomRepository roomRepository, IUnitOfWork unitOfWork)
    {
        _roomRepository = roomRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Success>> Handle(DeleteRoomCommand request, CancellationToken cancellationToken)
    {
        var deletedRoom = await _roomRepository.DeleteRoom(request.Id);
        if (deletedRoom is null)
        {
             return Error.NotFound("Room.NotFound", "Room not found.");
        }
        await _unitOfWork.SaveChangesAsync(cancellationToken, deletedRoom);
        return Result.Success;
    }
}
