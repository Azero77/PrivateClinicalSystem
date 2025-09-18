using ClinicApp.Application.Common;
using ClinicApp.Domain.Repositories;
using ErrorOr;
using MediatR;

namespace ClinicApp.Application.Commands.DoctorAddCommands;

public record DeleteDoctorCommand(Guid Id) : IRequest<ErrorOr<Success>>;

public class DeleteDoctorCommandHandler : IRequestHandler<DeleteDoctorCommand, ErrorOr<Success>>
{
    private readonly IDoctorRepository _doctorRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteDoctorCommandHandler(IDoctorRepository doctorRepository, IUnitOfWork unitOfWork)
    {
        _doctorRepository = doctorRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<ErrorOr<Success>> Handle(DeleteDoctorCommand request, CancellationToken cancellationToken)
    {
        var deletedDoctor = await _doctorRepository.DeleteDoctor(request.Id);
        if (deletedDoctor is null)
        {
            return Errors.General.NotFound;
        }
        await _unitOfWork.SaveChangesAsync(cancellationToken, deletedDoctor);
        return Result.Success;
    }
}
