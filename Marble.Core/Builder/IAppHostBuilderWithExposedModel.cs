namespace Marble.Core.Builder
{
    public interface IAppHostBuilderWithExposedModel : IAppHostBuilder
    {
        public AppHostBuildingModel BuildingModel { get; }
    }
}