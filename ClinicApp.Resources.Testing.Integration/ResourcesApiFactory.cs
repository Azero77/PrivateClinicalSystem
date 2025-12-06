using API;
using Microsoft.AspNetCore.Mvc.Testing;

namespace ClinicApp.Resources.Testing.Integration;
public class ResourcesApiFactory : WebApplicationFactory<IResourceApiMarker>, IAsyncLifetime

{
    //starting localstack container

    public Task InitializeAsync()
    {
        //configure s3 bucket
        throw new NotImplementedException();
    }

    public new Task DisposeAsync()
    {
        throw new NotImplementedException();
    }
}
