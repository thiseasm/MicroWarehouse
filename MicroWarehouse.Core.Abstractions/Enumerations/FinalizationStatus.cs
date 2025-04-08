namespace MicroWarehouse.Core.Abstractions.Enumerations
{
    public enum FinalizationStatus
    {
        Success,
        NotFound,
        AlreadyProcessed,
        StockReleaseFailed,
        UnknownError
    }
}
