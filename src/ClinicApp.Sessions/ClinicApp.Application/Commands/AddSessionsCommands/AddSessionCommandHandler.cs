using ClinicApp.Application.Commands.Common;
using ClinicApp.Application.Common;
using ClinicApp.Application.Converters;
using ClinicApp.Application.DTOs;
using ClinicApp.Domain.Common.Interfaces;
using ClinicApp.Domain.Common.ValueObjects;
using ClinicApp.Domain.Repositories;
using ClinicApp.Domain.Services.Sessions;
using ClinicApp.Domain.SessionAgg;
using ErrorOr;
using FluentValidation;
using MediatR;

namespace ClinicApp.Application.Commands.AddSessionsCommands;

public sealed class AddSessionCommandHandler : ValidatedCommandHandler<AddSessionCommand,SessionDTO>
{
    private readonly IDoctorRepository _doctorRepo;
    private readonly IScheduler _scheduler;
    private readonly IClock _clock;
    private readonly IUnitOfWork _unitOfWork;


    public AddSessionCommandHandler(IDoctorRepository doctorRepo, IScheduler scheduler, IClock clock, IUnitOfWork unitOfWork, IValidator<AddSessionCommand> validator)
        : base(validator)
    {
        _doctorRepo = doctorRepo;
        _scheduler = scheduler;
        _clock = clock;
        _unitOfWork = unitOfWork;
    }

    public override async Task<ErrorOr<SessionDTO>> GetResponse(AddSessionCommand request, CancellationToken cancellationToken)
    {
        var doctor = await _doctorRepo.GetById(request.doctorId);

        if (doctor is null)
            return Errors.General.NotFound;
        var timerange = TimeRange.Create(request.StartTime, request.EndTime);
        if (timerange.IsError)
            return timerange.Errors;
        ErrorOr<Session> session =  await _scheduler.CreateSession(Guid.NewGuid(),
            timerange.Value,
            new SessionDescription(request.SessionDescriptionContent),
            request.roomId,
            request.patientId,
            request.doctorId,
            _clock,
            request.role,
            doctor
            );
        if (session.IsError)
            return session.Errors;
        await _unitOfWork.SaveChangesAsync(cancellationToken,session.Value);
        return session.Value.FromSessionToDTO();
    }
}