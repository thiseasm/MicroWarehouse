
namespace MicroWarehouse.Core.Abstractions.Interfaces
{
    public interface IMongoInitializationService
    {
        Task InitializeAsync(CancellationToken cancellationToken = default);
    }
}
