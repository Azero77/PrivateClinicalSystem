using ClinicApp.Domain.Common;
using ErrorOr;
using System.Linq.Expressions;
using System.Reflection;

namespace ClinicApp.Application.DataQueryHelpers;

public class OrderCommand<T> : IQueryCommand<T>
    where T : Entity
{

    private readonly string _propertyName;
    private readonly bool _ascending;

    private OrderCommand(string propertyName, bool ascending)
    {
        _propertyName = propertyName;
        _ascending = ascending;

        PropertyInfo? prop = typeof(T)
                .GetProperty(_propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
    }


    public static ErrorOr<OrderCommand<T>> Create(string propertyName, bool ascending)
    {
        PropertyInfo? prop = typeof(T)
               .GetProperty(propertyName, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);

        if (prop is null)
            return Error.Validation("Application.Validation",$"No property of name {propertyName} in {typeof(T).Name}");

        return new OrderCommand<T>(propertyName,ascending);
    }
    public IQueryable<T> Apply(IQueryable<T> query)
    {
        var param = Expression.Parameter(typeof(T), "x");
        var property = Expression.Property(param, _propertyName);
        var lambda = Expression.Lambda(property, param);

        var methodName = _ascending ? "OrderBy" : "OrderByDescending";

        var result = Expression.Call(
            typeof(Queryable),
            methodName,
            new[] { typeof(T), property.Type },
            query.Expression,
            Expression.Quote(lambda));

        return query.Provider.CreateQuery<T>(result);
    }
}
