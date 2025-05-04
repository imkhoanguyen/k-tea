using Tea.Application.DTOs.Items;

namespace Tea.Infrastructure.Interfaces
{
    public interface IExcelService
    {
        Task<byte[]> Export<T>(List<T> dataItems) where T : class, new();
        Task<byte[]> ExportTemplateUpdateItemAsync(List<int> ids);
        Task<ImportResult> ImportUpdateItemsFromExcelAsync(Stream fileStream);
    }
}