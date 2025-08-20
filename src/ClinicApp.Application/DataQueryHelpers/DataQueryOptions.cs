using ClinicApp.Domain.Common;

namespace ClinicApp.Application.DataQueryHelpers;

public class DataQueryOptions<T>
    where T : Entity
{
    private readonly List<IQueryCommand<T>> _commands;

    public DataQueryOptions(List<IQueryCommand<T>> commands)
    {
        _commands = commands;
    }
    public IQueryable<T> Apply(IQueryable<T> query)
    {
        foreach (var command in _commands)
        {
            query = command.Apply(query);
        }

        return query;
    }
}
