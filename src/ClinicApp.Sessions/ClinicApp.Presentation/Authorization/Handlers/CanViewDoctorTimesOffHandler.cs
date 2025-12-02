using ClinicApp.Domain.Common;
using ClinicApp.Domain.Repositories;
using ClinicApp.Shared.QueryTypes;
using HotChocolate.Resolvers;
using Microsoft.AspNetCore.Authorization;

namespace ClinicApp.Presentation.Authorization.Handlers;

public class CanViewDoctorTimesOffHandler
    : CanViewBaseHandler<CanViewDoctorTimesOff, DoctorQueryType>
{
    private readonly IDoctorRepository _repo;

    public CanViewDoctorTimesOffHandler(IHttpContextAccessor accessor, IDoctorRepository repo)
        : base(accessor)
    {
        _repo = repo;
    }

    public override UserRole ScopedRole => UserRole.Doctor;

    protected override void CheckOwnedGraphQL(
        AuthorizationHandlerContext context,
        CanViewDoctorTimesOff requirement,
        UserRole role,
        Guid domainId,
        IMiddlewareContext ctx)
    {
        // ctx.Result is a single TimeOffQueryType (or could be a list)
        switch (ctx.Result)
        {
            case TimeOffQueryType timeOff:
                var parentDoctor = ctx.Parent<DoctorQueryType>();
                if (parentDoctor != null && IsOwned(parentDoctor, domainId, role))
                    context.Succeed(requirement);
                else
                    context.Fail();
                break;

            case IEnumerable<TimeOffQueryType> timeOffs:
                var parent = ctx.Parent<DoctorQueryType>();
                if (parent != null && IsOwned(parent, domainId, role))
                    context.Succeed(requirement);
                else
                    context.Fail();
                break;

            default:
                context.Fail();
                break;
        }
    }

    protected override async Task<Guid> GetOwnedEntityIdAsync(Guid userId, UserRole role)
    {
        var doctor = await _repo.GetDoctorByUsedId(userId);
        return doctor?.Id ?? Guid.Empty;
    }

    protected override bool IsOwned(DoctorQueryType resource, Guid? ownerId, UserRole role)
    {
        return resource.Id == ownerId;
    }
}
