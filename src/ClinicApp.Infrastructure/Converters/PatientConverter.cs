using ClinicApp.Infrastructure.Persistance.DataModels;
using ClinicApp.Domain.PatientAgg;

namespace ClinicApp.Infrastructure.Converters;

public class PatientConverter : IConverter<Patient, PatientDataModel>
{
    public Patient MapToEntity(PatientDataModel model)
    {
        return new Patient(
            model.Id,
            model.UserId,
            model.FirstName,
            model.LastName
        );
    }

    public PatientDataModel MapToData(Patient entity)
    {
        return new PatientDataModel
        {
            Id = entity.Id,
            UserId = entity.UserId,
            FirstName = entity.FirstName,
            LastName = entity.LastName
        };
    }
}