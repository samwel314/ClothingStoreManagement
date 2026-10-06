using ClothingStoreManagement.Application.DTO;

namespace ClothingStoreManagement.Application.Services
{
    public interface IWindowsBarcodePrinterService
    {
        IReadOnlyList<string> GetInstalledPrinters();

        void Print(
            BarcodePrintRequestDto request,
            string printerName);
    }
}
