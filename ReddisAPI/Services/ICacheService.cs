namespace ReddisAPI.Services
{
    public interface ICacheService
    {
        T GetData<T>(string key);   
        bool SetData<T>(string key , T data , DateTimeOffset expiration);
        object RemoveData(string key);
    }
}
