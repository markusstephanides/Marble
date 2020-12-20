using Marble.Core.Models;

namespace Marble.Core.Abstractions
{
    public interface IAppHostBuilderWithExposedModel : IAppHostBuilder
    {
        public AppHostBuildingModel BuildingModel { get; }
    }
}