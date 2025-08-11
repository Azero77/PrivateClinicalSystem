using ClinicApp.Application.Converters;
using ClinicApp.Application.QueryServices;
using ClinicApp.Domain.Common;
using ClinicApp.Domain.Common.Interfaces;
using ClinicApp.Domain.Common.ValueObjects;
using ClinicApp.Domain.Services.Sessions;
using ClinicApp.Domain.SessionAgg;
using ErrorOr;
using MediatR;

namespace ClinicApp.Application.Commands.AddSessionsCommands;
public record AddSessionCommand(
                        Guid sessionId,
                       TimeRange sessionDate,
                       SessionDescription sessionDescription,
                       Guid roomId,
                       Guid patientId,
                       Guid doctorId,
                       UserRole role) : IRequest<ErrorOr<Session>>;

public sealed class AddSessionCommandHandler : IRequestHandler<AddSessionCommand, ErrorOr<Session>>
{
    private readonly IDoctorQueryService _doctorQueryService;
    private readonly IScheduler _scheduler;
    private readonly IClock _clock;

    public AddSessionCommandHandler(IDoctorQueryService doctorQueryService, IScheduler scheduler, IClock clock)
    {
        _doctorQueryService = doctorQueryService;
        _scheduler = scheduler;
        _clock = clock;
    }

    public async Task<ErrorOr<Session>> Handle(AddSessionCommand request, CancellationToken cancellationToken)
    {
        var doctorDto = await _doctorQueryService.GetDoctorWithSessions(request.doctorId);

        if (doctorDto is null)
            return Error.NotFound("Application.Doctor.NotFound","Doctor with id not found");

        ErrorOr<Session> session = Session.Schedule(request.sessionId,
            request.sessionDate,
            request.sessionDescription,
            request.roomId,
            request.patientId,
            request.doctorId,
            _clock,
            request.role
            );
        if (session.IsError)
            return session.Errors;

        var result = await _scheduler.CreateSession(session.Value, doctorDto.ToDoctor());
        if (result.IsError)
            return result.Errors;
        return session;
    }
}