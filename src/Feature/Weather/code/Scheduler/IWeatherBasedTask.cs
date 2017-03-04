namespace Sitecore.Feature.Weather.Scheduler
{
    public interface IWeatherBasedTask
    {
        void Execute(dynamic data);
    }
}