namespace Tea.Infrastructure.Interfaces
{
    public interface IExcelService
    {
        Task<byte[]> Export<T>(List<T> dataItems) where T : class, new();
    }
}