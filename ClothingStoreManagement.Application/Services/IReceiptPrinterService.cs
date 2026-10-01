using ClothingStoreManagement.Application.DTO;

namespace ClothingStoreManagement.Application.Services
{
    public interface IReceiptPrinterService
    {
        Task<bool> PrintAsync(InvoiceDTO invoice);
    }
}
