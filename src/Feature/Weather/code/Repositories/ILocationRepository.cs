namespace Sitecore.Feature.Weather.Repositories
{
    public interface ILocationRepository
    {
        string GetLocationFromIp(string ipAddress, string apiUrl);
    }
}