using ClinicApp.Domain.DoctorAgg;
using ClinicApp.Domain.Repositories;
using ClinicApp.Infrastructure.Converters;
using ClinicApp.Infrastructure.Persistance;
using ClinicApp.Infrastructure.Persistance.DataModels;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace ClinicApp.Infrastructure.Repositories;

public class CachedDbDoctorRepository : Repository<Doctor, DoctorDataModel>, IDoctorRepository
{
    private readonly DbDoctorRepository _repo;
    private IDistributedCache _cache;

    public CachedDbDoctorRepository(AppDbContext context,
                                    IConverter<Doctor, DoctorDataModel> converter,
                                    DbDoctorRepository repo,
                                    IDistributedCache cache) : base(context, converter)
    {
        _repo = repo;
        _cache = cache;
    }

    public async Task<Doctor> AddDoctor(Doctor doctor)
    {
        var newDoctor = await _repo.AddDoctor(doctor);
        string key = $"doctor-{newDoctor.Id}";
        await _cache.SetStringAsync(key,JsonConvert.SerializeObject(_converter.MapToData(newDoctor)));
        await _cache.RemoveAsync("doctors");
        return newDoctor;

    }

    public Task<Doctor?> DeleteDoctor(Guid doctorId)
    {
        string key = $"doctor-{doctorId}";
        _cache.RemoveAsync(key); //no need to await it
        _cache.RemoveAsync("doctors");
        return _repo.DeleteDoctor(doctorId);
    }

    public Task<Doctor?> GetDoctorByRoom(Guid roomId)
    {
        return _repo.GetDoctorByRoom(roomId);
    }

    public async Task<Doctor?> GetDoctorByUsedId(Guid userId)
    {
        string key = $"doctor-userId-{userId}";

        var json = await _cache.GetStringAsync(key);
        Doctor? result;
        if (string.IsNullOrEmpty(json))
        {
            result = await _repo.GetDoctorByUsedId(userId);
            if (result is null)
                return result;
            await _cache.SetStringAsync(key, JsonConvert.SerializeObject(_converter.MapToData(result)));
            return result;
        }
        result = _converter.MapToEntity(JsonConvert.DeserializeObject<DoctorDataModel>(json)!);
        return result;
    }

    public async Task<IReadOnlyCollection<Doctor>> GetDoctors()
    {
        string key = $"doctors";

        var json = await _cache.GetStringAsync(key);
        IReadOnlyCollection<Doctor>? result;
        if (string.IsNullOrEmpty(json))
        {
            result = await _repo.GetDoctors();
            await _cache.SetStringAsync(key, JsonConvert.SerializeObject(result.Select(d => _converter.MapToData(d))));
            return result;
        }
        var datamodels = JsonConvert.DeserializeObject<List<DoctorDataModel>>(json!,
            new JsonSerializerSettings()
            {
                ConstructorHandling = ConstructorHandling.AllowNonPublicDefaultConstructor
            });
        if (datamodels is null)
            throw new Exception();
        return datamodels.Select(_converter.MapToEntity).ToList().AsReadOnly();
    }

    public Task<Doctor> UpdateDoctor(Doctor doctor)
    {
        string key = $"doctor-{doctor.Id}";
        _cache.RemoveAsync(key);
        _cache.RemoveAsync("doctors"); // no need to await it
        return _repo.UpdateDoctor(doctor);
    }
}