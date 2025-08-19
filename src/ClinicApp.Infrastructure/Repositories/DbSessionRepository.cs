﻿using ClinicApp.Domain.Common.Interfaces;
using ClinicApp.Domain.DoctorAgg;
using ClinicApp.Domain.Repositories;
using ClinicApp.Domain.SessionAgg;
using ClinicApp.Infrastructure.Persistance;
using Microsoft.EntityFrameworkCore;

namespace ClinicApp.Infrastructure.Repositories;

public class DbSessionRepository : PaginatedRepostiory<Session>,ISessionRepository
{
    private readonly AppDbContext _context;
    private IQueryable<Session> _sessionsNotTracked => _context.Sessions.AsNoTracking();
    private readonly IClock _clock;

    public DbSessionRepository(AppDbContext context, IClock clock)
        : base(context)
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

        Session? session = await _context.Sessions.FirstOrDefaultAsync(d => d.Id == sessionId);
        if (session is not null)
        {
            _context.Remove(session);
            await _context.SaveChangesAsync();
        }
        return session;
    }

    public async Task<IReadOnlyCollection<Session>> GetAllSessionsForDoctor(Doctor doctor)
    {
        List<Session> sessions = await _sessionsNotTracked.Where(s => s.DoctorId == doctor.Id).ToListAsync();

        return sessions.AsReadOnly();
    }

    public async Task<IReadOnlyCollection<Session>> GetFutureSessionsDoctor(Doctor doctor)
    {
        var sessions = await _sessionsNotTracked.Where(s => (s.DoctorId == doctor.Id) && (s.SessionDate.StartTime > _clock.UtcNow)).ToListAsync();
        return sessions.AsReadOnly();
    }

    public Task<Session?> GetSessionById(Guid sessionId)
    {
        return _sessionsNotTracked.FirstOrDefaultAsync(s => s.Id == sessionId);
    }

    public Task<IReadOnlyCollection<SessionState>> GetSessionHistory(Guid sessionId)
    {
        throw new NotImplementedException();
    }

    public async Task<IReadOnlyCollection<Session>> GetSessionsForDay(DateTime date)
    {
        date = date.Date;
        var sessions = await _sessionsNotTracked.Where(s => s.SessionDate.StartTime.Date == date)
            .ToListAsync();
        return sessions.AsReadOnly();
    }

    public async Task<IReadOnlyCollection<Session>> GetSessionsForDoctorToday(Guid doctorid)
    {
        var sessions = await _sessionsNotTracked.Where(s => s.DoctorId == doctorid && s.SessionDate.StartTime.Date == _clock.UtcNow.Date).ToListAsync();
        return sessions.AsReadOnly();
    }

    public async Task<IReadOnlyCollection<Session>> GetSessionsForToday()
    {

        var sessions = await _sessionsNotTracked.Where(s => s.SessionDate.StartTime.Date == _clock.UtcNow.Date).ToListAsync();
        return sessions.AsReadOnly();
    }


    public async Task<Session> UpdateSession(Session session)
    {
        _context.Update(session);
        await _context.SaveChangesAsync();
        return session;
    }

    public async Task<IReadOnlyCollection<Session>> GetSessionsForDoctorOnDay(Guid doctorid, DateTime date)
    {
        date = date.Date;
        List<Session> sessions = await _context.Sessions.AsNoTracking().Where(s => s.DoctorId == doctorid && s.SessionDate.StartTime.Date == date)
            .ToListAsync();
        return sessions.AsReadOnly();
    }

    public async Task<IReadOnlyCollection<Session>> GetSesssionsForDoctorOnDayAndAfter(Guid doctorid, DateTime date)
    {
        var day = date.Date;
        var dayAfter = day.AddDays(1);
        IReadOnlyCollection<Session> sessions = await _context.Sessions.AsNoTracking().Where(s => s.DoctorId == doctorid 
        && (s.SessionDate.StartTime.Date == date && s.SessionDate.StartTime.Date == dayAfter))
            .ToListAsync();
        return sessions;
    }
}
