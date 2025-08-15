
using Bogus;
using ClinicApp.Domain.DoctorAgg;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Net.Http.Json;

namespace ClinicApp.Presentation.Tests.IntegrationTest.DoctorsControllerTests;

[Collection(ApiFactory.ApiCollectionTests)]
public class GetDoctorWithSessionQueriesTest
    : IAsyncLifetime
{
    private readonly HttpClient _client;
    private readonly Faker<Doctor> faker = new Faker<Doctor>()
        .RuleFor(x => x.FirstName, x => x.Person.FirstName)
        .RuleFor(x => x.LastName, x => x.Person.LastName)
        .RuleFor(x => x.Id, new Guid())
        .RuleFor(x => x.Major, x => x.Person.UserName)
        .RuleFor(x => x.UserId, new Guid())
        .RuleFor(x => x.WorkingTime, 
            //Time From 10:30 am to 5:30 am in wed and thu
            x => WorkingTime.Create(new TimeOnly(10,30),new TimeOnly(17,30),WorkingDays.Tuesday | WorkingDays.Wednesday));

    private List<Guid> _createdUserIds = new();


    public GetDoctorWithSessionQueriesTest(ApiFactory api)
    {
        _client = api.CreateClient();
    }

    public Task InitializeAsync()
    {
        return Task.CompletedTask;
    }

    public Task DisposeAsync()
    {
        return Task.CompletedTask;
    }

}
