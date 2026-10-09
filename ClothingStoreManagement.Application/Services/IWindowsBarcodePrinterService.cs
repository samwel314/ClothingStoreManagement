using ClothingStoreManagement.Application.DTO;

namespace ClothingStoreManagement.Application.Services
{
    public interface IWindowsBarcodePrinterService
    {
        IReadOnlyList<string> GetInstalledPrinters();

        void Print(
            BarcodePrintRequestDto request,
            string printerName);
        void PrintVariant(
    BarcodeLabelDto label,
    int quantity,
    string printerName);
    }
}
