using ClinicApp.Domain.Common;
using ClinicApp.Infrastructure.Persistance;
using ErrorOr;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Reflection;

namespace ClinicApp.Infrastructure.Repositories;
public interface IPaginatedRepository<T>
    where T : Entity
{
    Task<IReadOnlyCollection<T>> GetAll(int pageNumber, int itemsPerPage);
    Task<IReadOnlyCollection<T>> GetAll(int pageNumber);
    Task<ErrorOr<IReadOnlyCollection<T>>> GetItems(DataQueryOptions<T> opts);
}

public class PaginatedRepostiory<T> : IPaginatedRepository<T>
    where T : Entity
{
    private readonly AppDbContext _context;
    protected int ItemsPerPage { get; set; } = 5;
    public PaginatedRepostiory(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyCollection<T>> GetAll(int pageNumber, int itemsPerPage)
    {
        return await _context.Set<T>()
            .AsNoTracking()
            .Take(itemsPerPage)
            .Skip(itemsPerPage * pageNumber)
            .ToListAsync();
    }

    public Task<IReadOnlyCollection<T>> GetAll(int pageNumber)
    {
        return GetAll(pageNumber, ItemsPerPage);
    }

    public async Task<ErrorOr<IReadOnlyCollection<T>>> GetItems(DataQueryOptions<T> opts)
    {
        var query = _context.Set<T>().AsNoTracking();

        if (opts.filtering is not null)
        {
            query = query.Where(opts.filtering);
        }

        if (opts.orderbyItem is not null && opts.orderSchema is not null)
        {
            PropertyInfo? prop = typeof(T)
                .GetProperty(opts.orderbyItem, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
            if (prop is null)
                return Error.Validation("Infrastructure.Validation", $"No property of name {opts.orderbyItem} in {typeof(T).Name}");
            if (!Enum.TryParse<OrderSchema>(opts.orderSchema, true, out OrderSchema orderSchema))
                return Error.Validation("Infrastructure.Validation", "No Proper OrderSchema is configured");

            var parameter = Expression.Parameter(typeof(T), "x");
            var property = Expression.Property(parameter, prop.Name);
            var keySelector = Expression.Lambda(property, parameter);

            var methodName = orderSchema == OrderSchema.Asc ? "OrderBy" : "OrderByDescending";

            var orderByCall = Expression.Call(
                typeof(Queryable),
                methodName,
                new[] { typeof(T), prop.PropertyType },
                query.Expression,
                Expression.Quote(keySelector));

            query = query.Provider.CreateQuery<T>(orderByCall);
        }

        return await query
            .Skip(ItemsPerPage * opts.pageNumber)
            .Take(ItemsPerPage)
            .ToListAsync();
    }


    enum OrderSchema
    {
        Asc,
        Desc
    }

}

public record DataQueryOptions<T>(int pageNumber, string? orderbyItem = null, string? orderSchema = null, Expression<Func<T, bool>>? filtering = null);