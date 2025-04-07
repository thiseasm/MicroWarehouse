namespace MicroWarehouse.Data.Abstractions.Interfaces
{
    public interface ICounterRepository
    {
        Task<int> IncrementCounterAsync(string entityName, CancellationToken cancellationToken = default);
    }
}
