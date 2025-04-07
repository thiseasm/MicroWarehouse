using MicroWarehouse.Core.Abstractions.Interfaces;
using MicroWarehouse.Data.Abstractions.Interfaces;


namespace MicroWarehouse.Core.Services
{
    public class IdGeneratorService(ICounterRepository counterRepository) : IIdGeneratorService
    {
        public async Task<int> GetNextSequenceValueAsync(string entityName, CancellationToken cancellationToken)
            => await counterRepository.IncrementCounterAsync(entityName, cancellationToken);
    }

}
