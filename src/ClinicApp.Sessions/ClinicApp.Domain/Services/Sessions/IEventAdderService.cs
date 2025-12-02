
using ClinicApp.Domain.Common;

/// <summary>
/// Service For publishing event Services
/// </summary>
public interface IEventAdderService<T>
    where T : IDomainEvent
{
    Task Add(T domainEvent);
}
