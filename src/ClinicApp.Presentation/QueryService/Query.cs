using ClinicApp.Application.Queries.Common;
using ClinicApp.Shared;
using ClinicApp.Shared.QueryTypes;
using HotChocolate.Authorization;
using MediatR;

namespace ClinicApp.Presentation.QueryService;

public class Query
{
    [UsePaging]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    [Authorize(Policy = PoliciesConstants.CanViewAllSessionsPolicy)]
    [Authorize(Policy = PoliciesConstants.CanViewOwnSessionsPolicy)]
    public async Task<IQueryable<SessionQueryType>> GetSessions(IMediator mediator)
    {
        return await mediator.Send(new QueryRequest<SessionQueryType>());
    }

    [UseProjection]
    [Authorize(Policy = PoliciesConstants.CanViewOwnSessionsPolicy)] // will add a middleware to check the id of the session and who is requesting it
    public async Task<SessionQueryType?> GetSession(Guid id,IMediator mediator)
    {
        return await mediator.Send(new QuerySingleRequest<SessionQueryType>(id));
    }



    [UseProjection]
    public async Task<DoctorQueryType?> GetDoctor(Guid doctorId, IMediator mediator)
    {
        return await mediator.Send(new QuerySingleRequest<DoctorQueryType>(doctorId));
    }
    [UseProjection]
    public async Task<PatientQueryType?> GetPatient(Guid patientId, IMediator mediator)
    {
        return await mediator.Send(new QuerySingleRequest<PatientQueryType>(patientId));
    }

}
