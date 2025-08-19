using ClinicApp.Application.Converters;
using ClinicApp.Domain.DoctorAgg;
using ClinicApp.Domain.Repositories;
using ErrorOr;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClinicApp.Application.Commands.DoctorAddCommands;
public record DoctorAddCommand(
    Guid doctorId,
    Guid userId,
    Guid roomId,
    WorkingDays workingDays,
    TimeOnly workingHoursStartTime,
    TimeOnly workingHoursEndTime,
    string? Major) : IRequest<ErrorOr<Doctor>>;


public class DoctorAddCommandHandler : IRequestHandler<DoctorAddCommand, ErrorOr<Doctor>>
{
    private readonly IDoctorRepository _repo;

    public DoctorAddCommandHandler(IDoctorRepository repo)
    {
        _repo = repo;
    }

    public async Task<ErrorOr<Doctor>> Handle(DoctorAddCommand request, CancellationToken cancellationToken)
    {
        var doctor = await _repo.AddDoctor(request.ToDoctor());

        if (doctor is null)
            return Error.Validation("Doctor.Validation","Something Error Occured");

        return doctor;
    }
}