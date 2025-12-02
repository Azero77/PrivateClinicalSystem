using ClinicApp.Domain.Common.Interfaces;
using ClinicApp.Domain.SessionAgg;
using ClinicApp.Infrastructure.Persistance;

namespace ClinicApp.Infrastructure.Services;

public class SessionEventAdderService : IEventAdderService<SessionDomainEvent>
{
    private readonly AppDbContext _context;
    private readonly IClock _clock;

    public SessionEventAdderService(AppDbContext context, IClock clock)
    {
        _context = context;
        _clock = clock;
    }

    public Task Add(SessionDomainEvent domainEvent)
    {
        var state = SessionState.From(
            Guid.NewGuid(),
            domainEvent.sessionId,
            domainEvent,
            _clock.UtcNow.DateTime
            );
        _context.SessionStates.Add(state);
        return Task.CompletedTask;
    }
}