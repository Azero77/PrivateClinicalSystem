using ClinicApp.Domain.Common.Interfaces;
using ClinicApp.Domain.DoctorAgg;
using ClinicApp.Domain.Repositories;
using ClinicApp.Domain.SessionAgg;
using ClinicApp.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;

namespace ClinicApp.Infrastructure.Repositories;

public class DbSessionRepository : ISessionRepository
{
    private readonly AppDbContext _context;
    private readonly IClock _clock;

    public DbSessionRepository(AppDbContext context, IClock clock)
    {
        _context = context;
        _clock = clock;
    }

    public async Task<Session> AddSession(Session session)
    {
        await _context.Sessions.AddAsync(session);
        await _context.SaveChangesAsync();
        return session;
    }

    public async Task<Session?> DeleteSession(Guid sessionId)
    {

        Session? session = await _context.Sessions.FirstOrDefaultAsync(d => d.Id == doctorId);
        if (session is not null)
        {
            _context.Remove(session);
            await _context.SaveChangesAsync();
        }
        return session;
    }

    public async Task<IReadOnlyCollection<Session>> GetAllSessionsForDoctor(Doctor doctor)
    {
        List<Session> sessions = await _context.Sessions.Where(s => s.DoctorId == doctor.Id).ToListAsync();

        return sessions.AsReadOnly();
    }

    public async Task<IReadOnlyCollection<Session>> GetFutureSessionsDoctor(Doctor doctor)
    {
        var sessions = await _context.Sessions.Where(s => (s.DoctorId == doctor.Id) && (s.SessionDate.StartTime > _clock.UtcNow)).ToListAsync();
        return sessions.AsReadOnly();
    }

    public Task<Session?> GetSessionById(Guid sessionId)
    {
        return _context.Sessions.FirstOrDefaultAsync(s => s.Id == sessionId);
    }

    public Task<IReadOnlyCollection<SessionState>> GetSessionHistory(Guid sessionId)
    {
        throw new NotImplementedException();
    }

    public async Task<IReadOnlyCollection<Session>> GetSessionsForDay(DateOnly date)
    {
        var sessions = await _context.Sessions.Where(s => DateOnly.FromDateTime(s.SessionDate.StartTime) == date)
            .ToListAsync();
        return sessions.AsReadOnly();
    }

    public async Task<IReadOnlyCollection<Session>> GetSessionsForDoctorToday(Guid doctorid)
    {
        var sessions = await _context.Sessions.Where(s => s.DoctorId == doctorid && s.SessionDate.StartTime.Date == _clock.UtcNow.Date).ToListAsync();
        return sessions.AsReadOnly();
    }

    public async Task<IReadOnlyCollection<Session>> GetSessionsForToday()
    {

        var sessions = await _context.Sessions.Where(s.SessionDate.StartTime.Date == _clock.UtcNow.Date).ToListAsync();
        return sessions.AsReadOnly();
    }


    public async Task<Session> UpdateSession(Session session)
    {
        _context.Update(session);
        await _context.SaveChangesAsync();
        return session;
    }
}
