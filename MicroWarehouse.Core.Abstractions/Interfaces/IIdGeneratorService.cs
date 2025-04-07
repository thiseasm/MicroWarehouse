namespace MicroWarehouse.Core.Abstractions.Interfaces
{
    public interface IIdGeneratorService
    {
        Task<int> GetNextSequenceValueAsync(string entityName, CancellationToken cancellationToken = default);
    }
}
