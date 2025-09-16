using ClinicApp.Domain.DoctorAgg;
using ClinicApp.Domain.Repositories;
using ClinicApp.Presentation.Requests;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace ClinicApp.Presentation.Authorization.Handlers;

public abstract class DoctorCanModifySession<TRequirement, TReqeust> : CanModifySession<TRequirement, TReqeust>
   where TRequirement : CanModifySessionRequirement,new()
{
    private readonly IDoctorRepository _repo;

    public DoctorCanModifySession(IDoctorRepository repo)
    {
        _repo = repo;
    }

    protected override async Task<bool> Authorize(AuthorizationHandlerContext context, TRequirement requirement, TReqeust resource, Guid userId)
    {
        var doctor = await _repo.GetDoctorByUsedId(userId);
        return CanDoAction(resource, doctor);
    }

    protected abstract bool CanDoAction(TReqeust resource, Doctor? doctor);
}
public class DoctorCanAddSession(IDoctorRepository repo) : DoctorCanModifySession<CanAddSessionRequirement, AddSessionRequest>(repo)
{
    protected override bool CanDoAction(AddSessionRequest resource, Doctor? doctor)
    {
        return resource.DoctorId == doctor?.Id;
    }
}
public class DoctorCanUpdateSession(IDoctorRepository repo) : DoctorCanModifySession<CanUpdateSessionRequirement, ModifySessionRequest>(repo)
{
    protected override bool CanDoAction(ModifySessionRequest resource, Doctor? doctor)
    {
        return resource.id == doctor?.Id;
    }
}

