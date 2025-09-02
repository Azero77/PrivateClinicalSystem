using ClinicApp.Domain.Common;
using ClinicApp.Domain.PatientAgg;
using ClinicApp.Infrastructure.Persistance.DataModels;
using System.Reflection;

namespace ClinicApp.Infrastructure.Mappers;

public static class PatientMapper
{
    public static PatientDataModel ToDataModel(this Patient patient)
    {
        return new PatientDataModel
        {
            Id = patient.Id,
            UserId = patient.UserId,
            FirstName = patient.FirstName,
            LastName = patient.LastName
        };
    }

    public static Patient ToDomain(this PatientDataModel patientData)
    {
        return new Patient(
            patientData.Id,
            patientData.UserId,
            patientData.FirstName,
            patientData.LastName
        );
    }
}
