using ErrorOr;

namespace ClinicApp.Infrastructure.Repositories;

[Serializable]
public class ErrorException : Exception
{
    public ErrorException(Error error)
    {
        Error = error;
    }
    protected ErrorException(
      System.Runtime.Serialization.SerializationInfo info,
      System.Runtime.Serialization.StreamingContext context) : base(info, context) { }

    public Error Error { get; }
}