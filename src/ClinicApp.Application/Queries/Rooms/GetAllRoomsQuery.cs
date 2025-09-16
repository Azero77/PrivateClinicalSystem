using ClinicApp.Application.DTOs;
using MediatR;

namespace ClinicApp.Application.Queries.Rooms;

public record GetAllRoomsQuery() : IRequest<IEnumerable<RoomDTO>>;
