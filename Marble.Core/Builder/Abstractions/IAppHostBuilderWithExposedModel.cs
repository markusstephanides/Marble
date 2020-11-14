using Marble.Core.Builder.Models;

namespace Marble.Core.Builder.Abstractions
{
    public interface IAppHostBuilderWithExposedModel : IAppHostBuilder
    {
        public AppHostBuildingModel BuildingModel { get; }
    }
}