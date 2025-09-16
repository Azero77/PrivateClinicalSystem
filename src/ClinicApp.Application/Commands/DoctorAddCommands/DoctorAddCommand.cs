using ClinicApp.Application.Common;
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
    string firstName,
    string lastName,
    WorkingDays workingDays,
    TimeOnly workingHoursStartTime,
    TimeOnly workingHoursEndTime,
    string? Major) : IRequest<ErrorOr<Doctor>>;


public class DoctorAddCommandHandler : IRequestHandler<DoctorAddCommand, ErrorOr<Doctor>>
{
    private readonly IDoctorRepository _repo;
    private readonly IUnitOfWork _unitOfWork;
    public DoctorAddCommandHandler(IDoctorRepository repo, IUnitOfWork unitOfWork)
    {
        _repo = repo;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Doctor>> Handle(DoctorAddCommand request, CancellationToken cancellationToken)
    {
        var doctor = await _repo.AddDoctor(request.ToDoctor());
        var num = await _unitOfWork.SaveChangesAsync(cancellationToken,doctor);
        if (num != 1)
            return Errors.Doctor.CreateFailed;
        return doctor;
    }
}