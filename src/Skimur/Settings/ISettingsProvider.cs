namespace Skimur.Settings
{
    public interface ISettingsProvider<out T> where T: ISettings, new()
    {
        T Settings { get; }
    }
}
