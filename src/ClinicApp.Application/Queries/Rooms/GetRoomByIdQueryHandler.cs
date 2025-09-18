using ClinicApp.Application.DTOs;
using ClinicApp.Domain.Repositories;
using ErrorOr;
using MediatR;

namespace ClinicApp.Application.Queries.Rooms;

public class GetRoomByIdQueryHandler : IRequestHandler<GetRoomByIdQuery, ErrorOr<RoomDTO>>
{
    private readonly IRoomRepository _roomRepository;

    public GetRoomByIdQueryHandler(IRoomRepository roomRepository)
    {
        _roomRepository = roomRepository;
    }

    public async Task<ErrorOr<RoomDTO>> Handle(GetRoomByIdQuery request, CancellationToken cancellationToken)
    {
        var room = await _roomRepository.GetById(request.Id);
        if (room is null)
            return Application.Common.Errors.General.NotFound;

        return new RoomDTO
        {
            Id = room.Id,
            Name = room.Name
        };
    }
}
