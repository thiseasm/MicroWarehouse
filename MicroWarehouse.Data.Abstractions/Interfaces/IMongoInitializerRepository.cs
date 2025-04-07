namespace MicroWarehouse.Data.Abstractions.Interfaces
{
    public interface IMongoInitializerRepository
    {
        Task InitializeAsync(CancellationToken cancellationToken = default);
    }
}
