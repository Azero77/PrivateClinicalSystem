using ClinicApp.Application.Queries.Common;
using ClinicApp.Application.QueryTypes;
using ClinicApp.Presentation.Authorization.Policies;
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
    public async Task<IQueryable<SessionQueryType>> GetSessions(IMediator mediator)
    {
        return await mediator.Send(new QueryRequest<SessionQueryType>());
    }

    [UseProjection]
    [Authorize()] // will add a middleware to check the id of the session and who is requesting it
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
