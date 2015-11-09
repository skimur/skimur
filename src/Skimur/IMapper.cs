namespace Skimur
{
    /// <summary>
    /// Handles mapping one type to another
    /// </summary>
    public interface IMapper
    {
        void Map<TSource, TDestination>(TSource source, TDestination destination);
        TDestination Map<TSource, TDestination>(TSource source) where TDestination : class, new();
    }
}
