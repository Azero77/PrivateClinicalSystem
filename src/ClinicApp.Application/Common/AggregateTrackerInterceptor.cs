using Castle.DynamicProxy;
using ClinicApp.Domain.Common;
using System.Reflection;

namespace ClinicApp.Application.Common;
public class AggregateTrackerInterceptor : IInterceptor
{
    private readonly IUnitOfWork _unitOfWork;

    public AggregateTrackerInterceptor(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public void Intercept(IInvocation invocation)
    {
        if (invocation.Method.GetCustomAttribute<AsNoTrackingAttribute>() is not null)
            return;
        if (invocation.ReturnValue is Task task)
        {
            task.ContinueWith(t =>
            {
                if (t.IsCanceled || t.IsFaulted) return;

                var result = t.GetType().GetProperty("Result");
                if (result is null) return;
                TrackIfAggregate(result);return;
            });
        }
        TrackIfAggregate(invocation.ReturnValue);
        return;
    }

    private void TrackIfAggregate(object? result)
    {
        if (result is null)
            return;
        else if (result is AggregateRoot entity)
        {
            _unitOfWork.Track(entity);
        }
        else if (result is IEnumerable<AggregateRoot> entities)
        {
            foreach (var subentity in entities)
            {
                _unitOfWork.Track(subentity);
            }
        }
    }
}

[AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
public sealed class AsNoTrackingAttribute : Attribute;

