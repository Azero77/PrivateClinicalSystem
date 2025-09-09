using ClinicApp.Domain.SessionAgg;

namespace ClinicApp.Presentation.Requests;

public record GetSessionsRequest(string? DoctorId,
                              DateTimeOffset? FromDatetime,
                              DateTimeOffset? ToDateTime,
                              string? RoomId,
                              string? PatientId,
                              SessionStatus? Status,
                              int pageNumber,
                              int pageSize,
                              string[] sortOptions //would look something like this ?sortOptions=startTime:ASC
                              );
