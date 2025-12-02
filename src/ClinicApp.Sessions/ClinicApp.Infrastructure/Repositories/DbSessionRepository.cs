using ClinicApp.Domain.Common.Interfaces;
using ClinicApp.Domain.DoctorAgg;
using ClinicApp.Domain.Repositories;
using ClinicApp.Domain.SessionAgg;
using ClinicApp.Infrastructure.Converters;
using ClinicApp.Infrastructure.Persistance;
using ClinicApp.Infrastructure.Persistance.DataModels;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace ClinicApp.Infrastructure.Repositories;

public class DbSessionRepository : Repository<Session, SessionDataModel>, ISessionRepository
{
    private readonly IClock _clock;
    private IQueryable<SessionDataModel> _sessionsNotTracked => _context.Sessions.AsNoTracking();

    public DbSessionRepository(AppDbContext context, IConverter<Session, SessionDataModel> converter, IClock clock)
        : base(context,converter)
    {
        _clock = clock;
    }
    public async Task<Session> AddSession(Session session)
    {
        var sessionData = _converter.MapToData(session);
        await _context.Sessions.AddAsync(sessionData);
        return session;
    }

    public async Task<Session?> DeleteSession(Guid sessionId)
    {
        var sessionData = await _context.Sessions.FirstOrDefaultAsync(d => d.Id == sessionId);
        if (sessionData is not null)
        {
            _context.Entry(sessionData).Property<bool>("IsDeleted").CurrentValue = true;
            return _converter.MapToEntity(sessionData);
        }
        return null;
    }

    public async Task<IReadOnlyCollection<Session>> GetAllSessionsForDoctor(Doctor doctor)
    {
        var sessionsData = await _sessionsNotTracked.Where(s => s.DoctorId == doctor.Id).ToListAsync();
        return sessionsData.Select(_converter.MapToEntity).ToList().AsReadOnly();
    }

    public async Task<IReadOnlyCollection<Session>> GetFutureSessionsDoctor(Doctor doctor)
    {
        var sessionsData = await _sessionsNotTracked
            .Where(s => s.DoctorId == doctor.Id && s.StartTime > _clock.UtcNow)
            .ToListAsync();
        return sessionsData.Select(_converter.MapToEntity).ToList().AsReadOnly();
    }

    public async Task<IReadOnlyCollection<SessionState>> GetSessionHistory(Guid sessionId)
    {
        var states = await _context.SessionStates.AsNoTracking()
            .Where(s => s.SessionId == sessionId)
            .ToListAsync();

        return states.AsReadOnly();
    }

    public async Task<IReadOnlyCollection<Session>> GetSessionsForDay(DateTimeOffset date)
    {
        var targetDate = date.Date;
        var sessionsData = await _sessionsNotTracked
            .Where(s => s.StartTime.Date == targetDate)
            .ToListAsync();
        return sessionsData.Select(_converter.MapToEntity).ToList().AsReadOnly();
    }

    public async Task<IReadOnlyCollection<Session>> GetSessionsForDoctorToday(Guid doctorid)
    {
        var sessionsData = await _sessionsNotTracked
            .Where(s => s.DoctorId == doctorid && s.StartTime.Date == _clock.UtcNow.Date)
            .ToListAsync();
        return sessionsData.Select(_converter.MapToEntity).ToList().AsReadOnly();
    }

    public async Task<IReadOnlyCollection<Session>> GetSessionsForToday()
    {
        var sessionsData = await _sessionsNotTracked
            .Where(s => s.StartTime.Date == _clock.UtcNow.Date)
            .ToListAsync();
        return sessionsData.Select(_converter.MapToEntity).ToList().AsReadOnly();
    }

    public Task<Session> UpdateSession(Session session)
    {
        var sessionData = _converter.MapToData(session);
        _context.Update(sessionData);
        return Task.FromResult(session);
    }

    public async Task<IReadOnlyCollection<Session>> GetSessionsForDoctorOnDay(Guid doctorid, DateTimeOffset date)
    {
        var targetDate = date.Date;
        var sessionsData = await _context.Sessions.AsNoTracking()
            .Where(s => s.DoctorId == doctorid && s.StartTime.Date == targetDate)
            .ToListAsync();
        return sessionsData.Select(_converter.MapToEntity).ToList().AsReadOnly();
    }

    public async Task<IReadOnlyCollection<Session>> GetSesssionsForDoctorOnDayAndAfter(Guid doctorid, DateTimeOffset date)
    {
        var day = date.Date;
        var dayAfter = day.AddDays(1);
        var sessionsData = await _context.Sessions.AsNoTracking()
            .Where(s => s.DoctorId == doctorid && (s.StartTime.Date == day || s.StartTime.Date == dayAfter))
            .ToListAsync();
        return sessionsData.Select(_converter.MapToEntity).ToList().AsReadOnly();
    }
}