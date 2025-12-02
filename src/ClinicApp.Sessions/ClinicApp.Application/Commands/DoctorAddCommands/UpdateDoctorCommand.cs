using ClinicApp.Application.Common;
using ClinicApp.Application.Converters;
using ClinicApp.Application.DTOs;
using ClinicApp.Domain.DoctorAgg;
using ClinicApp.Domain.Repositories;
using ErrorOr;
using MediatR;

namespace ClinicApp.Application.Commands.DoctorAddCommands;

public record UpdateDoctorCommand(
    Guid Id,
    string? FirstName,
    string? LastName,
    Guid? RoomId,
    WorkingDays? WorkingDays,
    TimeOnly? WorkingHoursStartTime,
    TimeOnly? WorkingHoursEndTime,
    string? Major) : IRequest<ErrorOr<DoctorDTO>>;

public class UpdateDoctorCommandHandler : IRequestHandler<UpdateDoctorCommand, ErrorOr<DoctorDTO>>
{
    private readonly IDoctorRepository _doctorRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateDoctorCommandHandler(IDoctorRepository doctorRepository, IUnitOfWork unitOfWork)
    {
        _doctorRepository = doctorRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<DoctorDTO>> Handle(UpdateDoctorCommand request, CancellationToken cancellationToken)
    {
        var doctor = await _doctorRepository.GetById(request.Id);
        if (doctor is null)
        {
            return Errors.General.NotFound;
        }

        if (request.FirstName is not null)
        {
            doctor.FirstName = request.FirstName;
        }

        if (request.LastName is not null)
        {
            doctor.LastName = request.LastName;
        }

        if (request.RoomId.HasValue)
        {
            doctor.UpdateRoom(request.RoomId.Value);
        }

        if (request.Major is not null)
        {
            doctor.UpdateMajor(request.Major);
        }

        if (request.WorkingDays.HasValue || request.WorkingHoursStartTime.HasValue || request.WorkingHoursEndTime.HasValue)
        {
            var workingDays = request.WorkingDays ?? doctor.WorkingTime.WorkingDays;
            var startTime = request.WorkingHoursStartTime ?? doctor.WorkingTime.WorkingHours.StartTime;
            var endTime = request.WorkingHoursEndTime ?? doctor.WorkingTime.WorkingHours.EndTime;
            var workingTime = WorkingTime.Create(startTime, endTime, workingDays, doctor.WorkingTime.WorkingHours.TimeZoneId).Value;
            doctor.UpdateWorkingTime(workingTime);
        }

        var updatedDoctor = await _doctorRepository.UpdateDoctor(doctor);
        await _unitOfWork.SaveChangesAsync(cancellationToken, updatedDoctor);
        return updatedDoctor.FromDoctorToDTO();
    }
}
