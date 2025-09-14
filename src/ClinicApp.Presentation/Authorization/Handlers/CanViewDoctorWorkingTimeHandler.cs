using ClinicApp.Domain.Common;
using ClinicApp.Domain.Repositories;
using ClinicApp.Shared.QueryTypes;
using HotChocolate.Resolvers;
using Microsoft.AspNetCore.Authorization;
using System.Threading.Tasks;

namespace ClinicApp.Presentation.Authorization.Handlers;

public class CanViewDoctorWorkingTimeHandler(IHttpContextAccessor accessor, IDoctorRepository repo) : CanViewBaseHandler<CanViewDoctorWorkingTime,
    DoctorQueryType>(accessor)
{
    public override UserRole ScopedRole => throw new NotImplementedException();

    protected override void CheckOwnedGraphQL(AuthorizationHandlerContext context, CanViewDoctorWorkingTime requirement, UserRole role, Guid domainId, IMiddlewareContext ctx)
    {
        if (ctx.Result is WorkingTimeQueryType workingTime)
        {
            var parent = ctx.Parent<DoctorQueryType>();
            if (parent is not null && IsOwned(parent,domainId,ScopedRole))
            {
                context.Succeed(requirement);
                return;
            }else
            {
                context.Fail();
                return;
            }
        }
    }

    protected override async Task<Guid> GetOwnedEntityIdAsync(Guid userId, UserRole role)
    {
        var doctor = await repo.GetDoctorByUsedId(userId);
        return doctor?.Id ?? Guid.Empty;
    }

    protected override bool IsOwned(DoctorQueryType resource, Guid? ownerId, UserRole role)
    {
        return resource.Id == ownerId;
    }
}
