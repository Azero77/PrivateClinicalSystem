using ClinicApp.Application.DTOs;
using ClinicApp.Domain.Repositories;
using MediatR;

namespace ClinicApp.Application.Queries.Rooms;

public class GetAllRoomsQueryHandler : IRequestHandler<GetAllRoomsQuery, IEnumerable<RoomDTO>>
{
    private readonly IRoomRepository _roomRepository;

    public GetAllRoomsQueryHandler(IRoomRepository roomRepository)
    {
        _roomRepository = roomRepository;
    }

    public async Task<IEnumerable<RoomDTO>> Handle(GetAllRoomsQuery request, CancellationToken cancellationToken)
    {
        var rooms = await _roomRepository.GetAllRooms();
        return rooms.Select(r => new RoomDTO
        {
            Id = r.Id,
            Name = r.Name
        });
    }
}
