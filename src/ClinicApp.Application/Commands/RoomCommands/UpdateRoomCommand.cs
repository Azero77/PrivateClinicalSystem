using ClinicApp.Application.Common;
using ClinicApp.Domain.Common.Entities;
using ClinicApp.Domain.Repositories;
using ErrorOr;
using MediatR;

namespace ClinicApp.Application.Commands.RoomCommands;

public record UpdateRoomCommand(Guid Id, string Name) : IRequest<ErrorOr<Room>>;

public class UpdateRoomCommandHandler : IRequestHandler<UpdateRoomCommand, ErrorOr<Room>>
{
    private readonly IRoomRepository _roomRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateRoomCommandHandler(IRoomRepository roomRepository, IUnitOfWork unitOfWork)
    {
        _roomRepository = roomRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Room>> Handle(UpdateRoomCommand request, CancellationToken cancellationToken)
    {
        var room = await _roomRepository.GetById(request.Id);
        if (room is null)
        {
            return Errors.General.NotFound;
        }

        room.UpdateName(request.Name);

        var updatedRoom = await _roomRepository.UpdateRoom(room);
        await _unitOfWork.SaveChangesAsync(cancellationToken, updatedRoom);
        return updatedRoom;
    }
}
