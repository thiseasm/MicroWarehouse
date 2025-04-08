namespace MicroWarehouse.Infrastructure.Abstractions.Interfaces
{
    public interface IMongoInitializerRepository
    {
        Task InitializeAsync(CancellationToken cancellationToken = default);
    }
}
