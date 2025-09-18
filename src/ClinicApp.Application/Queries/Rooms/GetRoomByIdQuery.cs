using ClinicApp.Application.DTOs;
using ErrorOr;
using MediatR;

namespace ClinicApp.Application.Queries.Rooms;

public record GetRoomByIdQuery(Guid Id) : IRequest<ErrorOr<RoomDTO>>;
