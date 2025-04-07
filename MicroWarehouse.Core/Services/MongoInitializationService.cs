using MicroWarehouse.Core.Abstractions.Interfaces;
using MicroWarehouse.Data.Abstractions.Interfaces;

namespace MicroWarehouse.Core.Services
{
    public class MongoInitializationService(IMongoInitializerRepository initializerRepository) : IMongoInitializationService
    {
        public async Task InitializeAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                await initializerRepository.InitializeAsync(cancellationToken);
            }
            catch(Exception ex)
            {
                Console.WriteLine($"An error occurred during MongoDB initialization: {ex.Message}");
            }
        }
    }

}
