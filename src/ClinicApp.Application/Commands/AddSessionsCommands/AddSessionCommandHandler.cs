using ClinicApp.Domain.Common.Interfaces;
using ClinicApp.Domain.Repositories;
using ClinicApp.Domain.Services.Sessions;
using ClinicApp.Domain.SessionAgg;
using ErrorOr;
using MediatR;

namespace ClinicApp.Application.Commands.AddSessionsCommands;

public sealed class AddSessionCommandHandler : IRequestHandler<AddSessionCommand, ErrorOr<Session>>
{
    private readonly IDoctorRepository _doctorRepo;
    private readonly IScheduler _scheduler;
    private readonly IClock _clock;

    public AddSessionCommandHandler(IDoctorRepository doctorRepo, IScheduler scheduler, IClock clock)
    {
        _doctorRepo = doctorRepo;
        _scheduler = scheduler;
        _clock = clock;
    }

    public async Task<ErrorOr<Session>> Handle(AddSessionCommand request, CancellationToken cancellationToken)
    {
        var doctor = await _doctorRepo.GetByIdAsync(request.doctorId);

        if (doctor is null)
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

        var result = await _scheduler.CreateSession(session.Value, doctor);
        if (result.IsError)
            return result.Errors;
        return session;
    }
}