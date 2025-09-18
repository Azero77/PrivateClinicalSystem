using ClinicApp.Infrastructure.Persistance.DataModels;
using ClinicApp.Domain.SecretaryAgg;

namespace ClinicApp.Infrastructure.Converters;

public class SecretaryConverter : IConverter<Secretary, SecretaryDataModel>
{
    public Secretary MapToEntity(SecretaryDataModel model)
    {
        return new Secretary(model.Id, model.UserId, model.FirstName, model.LastName);
    }

    public SecretaryDataModel MapToData(Secretary entity)
    {
        return new SecretaryDataModel
        {
            Id = entity.Id,
            UserId = entity.UserId,
            FirstName = entity.FirstName,
            LastName = entity.LastName
        };
    }
}
