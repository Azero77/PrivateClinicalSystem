using ClinicApp.Application.Queries.Sessions;
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
        return await mediator.Send(new GetSessionsQuery());
    }

    public async Task<IQueryable<SessionQueryType>> GetSession(Guid id,IMediator mediator)
    {
        return await mediator.Send(new GetSessionQuery(id));
    }
}
