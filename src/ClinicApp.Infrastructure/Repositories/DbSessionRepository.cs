using ClinicApp.Application.DataQueryHelpers;
using ClinicApp.Domain.Common.Interfaces;
using ClinicApp.Domain.DoctorAgg;
using ClinicApp.Domain.Repositories;
using ClinicApp.Domain.SessionAgg;
using ClinicApp.Infrastructure.Mappers;
using ClinicApp.Infrastructure.Persistance;
using ClinicApp.Infrastructure.Persistance.DataModels;
using Microsoft.EntityFrameworkCore;

namespace ClinicApp.Infrastructure.Repositories;

public class DbSessionRepository : PaginatedRepository<Session>, ISessionRepository
{
    private readonly AppDbContext _context;
    private IQueryable<SessionDataModel> _sessionsNotTracked => _context.Sessions.AsNoTracking();
    private readonly IClock _clock;

    public DbSessionRepository(AppDbContext context, IClock clock)
        : base(context)
    {
        _context = context;
        _clock = clock;
    }

    public async Task<Session> AddSession(Session session)
    {
        await _context.Sessions.AddAsync(session.ToDataModel());
        return session;
    }

    public async Task<Session?> DeleteSession(Guid sessionId)
    {
        var sessionData = await _context.Sessions.FirstOrDefaultAsync(d => d.Id == sessionId);
        if (sessionData is not null)
        {
            _context.Remove(sessionData);
            return sessionData.ToDomain();
        }
        return null;
    }

    public async Task<IReadOnlyCollection<Session>> GetAllSessionsForDoctor(Guid doctorId)
    {
        List<Session> sessions = await _sessionsNotTracked.Where(s => s.DoctorId == doctorId).Select(s => s.ToDomain()).ToListAsync();

        return sessions.AsReadOnly();
    }

    public async Task<IReadOnlyCollection<Session>> GetFutureSessionsDoctor(Guid doctorId)
    {
        var sessions = await _sessionsNotTracked.Where(s => (s.DoctorId == doctorId) && (s.SessionDate.StartTime > _clock.UtcNow)).Select(s => s.ToDomain()).ToListAsync();
        return sessions.AsReadOnly();
    }

    public async Task<Session?> GetSessionById(Guid sessionId)
    {
        return (await _sessionsNotTracked.FirstOrDefaultAsync(s => s.Id == sessionId))?.ToDomain();
    }

    public Task<IReadOnlyCollection<SessionState>> GetSessionHistory(Guid sessionId)
    {
        throw new NotImplementedException();
    }

    public async Task<IReadOnlyCollection<Session>> GetSessionsForDay(DateTimeOffset date)
    {
        var targetDate = date.Date;
        var sessions = await _sessionsNotTracked.Where(s => s.SessionDate.StartTime.Date == targetDate)
            .ToListAsync();
        return sessions.Select(s => s.ToDomain()).ToList().AsReadOnly();
    }

    public async Task<IReadOnlyCollection<Session>> GetSessionsForDoctorToday(Guid doctorid)
    {
        var sessions = await _sessionsNotTracked.Where(s => s.DoctorId == doctorid && s.SessionDate.StartTime.Date == _clock.UtcNow.Date).ToListAsync();
        return sessions.Select(s => s.ToDomain()).ToList().AsReadOnly();
    }

    public async Task<IReadOnlyCollection<Session>> GetSessionsForToday()
    {

        var sessions = await _sessionsNotTracked.Where(s => s.SessionDate.StartTime.Date == _clock.UtcNow.Date).ToListAsync();
        return sessions.Select(s => s.ToDomain()).ToList().AsReadOnly();
    }


    public Task<Session> UpdateSession(Session session)
    {
        _context.Update(session.ToDataModel());
        return Task.FromResult(session);
    }

    public async Task<IReadOnlyCollection<Session>> GetSessionsForDoctorOnDay(Guid doctorid, DateTimeOffset date)
    {
        var targetDate = date.Date;
        var sessionsData = await _context.Sessions.AsNoTracking().Where(s => s.DoctorId == doctorid && s.SessionDate.StartTime.Date == targetDate)
            .ToListAsync();
        return sessionsData.Select(s => s.ToDomain()).ToList().AsReadOnly();
    }

    public async Task<IReadOnlyCollection<Session>> GetSesssionsForDoctorOnDayAndAfter(Guid doctorid, DateTimeOffset date)
    {
        var day = date.Date;
        var dayAfter = day.AddDays(1);
        var sessionsData = await _context.Sessions.AsNoTracking().Where(s => s.DoctorId == doctorid 
        && (s.SessionDate.StartTime.Date == day || s.SessionDate.StartTime.Date == dayAfter))
            .ToListAsync();
        return sessionsData.Select(s => s.ToDomain()).ToList();
    }
}
