using ClinicApp.Application.Common;
using ClinicApp.Application.DTOs;
using ClinicApp.Application.QueryServices;
using ClinicApp.Application.Services;
using ClinicApp.Domain.Common;
using ClinicApp.Domain.Repositories;
using ClinicApp.Domain.SessionAgg;
using ClinicApp.Shared.QueryTypes;
using ErrorOr;
using MediatR;
using System.Text.Json;

namespace ClinicApp.Application.Queries.Sessions.SessionById;
public record GetSessionByIdQuery(Guid sessionId,UserRole role) : IRequest<ErrorOr<SessionQueryType>>;

public sealed class GetSessionByIdQueryHandler(IQueryService<SessionQueryType> queryService,IContentManagementService contentManagementService) : IRequestHandler<GetSessionByIdQuery, ErrorOr<SessionQueryType>>
{
    public async Task<ErrorOr<SessionQueryType>> Handle(GetSessionByIdQuery request, CancellationToken cancellationToken)
    {
        var session = await queryService.GetItemById(request.sessionId);

        if (session is null)
            return Errors.General.NotFound;

        ///now we need to configure the s3 urls for the content json elemen
        JsonElement? content = null;
        if (session.Content is not null)
        {
            content = await contentManagementService.FromServerAsync(session.Content.Value);
        }
        return session;
    }
}

