using ClinicApp.Domain.Common;
using ClinicApp.Domain.Common.Interfaces;

namespace ClinicApp.Infrastructure.Converters;
public interface IConverter<TEntity, TData>
    where TEntity : Entity
    where TData : class
{
    TEntity MapToEntity(TData model);
    TData MapToData(TEntity model);
}
