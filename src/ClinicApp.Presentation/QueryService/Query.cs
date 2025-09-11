using ClinicApp.Application.Queries.Common;
using ClinicApp.Application.QueryTypes;
using MediatR;

namespace ClinicApp.Presentation.QueryService;

public class Query
{
    [UsePaging]
    [UseProjection]
    [UseFiltering]
    [UseSorting]
    public async Task<IQueryable<SessionQueryType>> GetSessions(IMediator mediator)
    {
        return await mediator.Send(new QueryRequest<SessionQueryType>());
    }

    [UseProjection]
    public async Task<SessionQueryType?> GetSession(Guid id,IMediator mediator)
    {
        return await mediator.Send(new QuerySingleRequest<SessionQueryType>(id));
    }



    [UseProjection]
    public async Task<DoctorQueryType?> GetDoctor(Guid doctorId, IMediator mediator)
    {
        return await mediator.Send(new QuerySingleRequest<DoctorQueryType>(doctorId));
    }
}
