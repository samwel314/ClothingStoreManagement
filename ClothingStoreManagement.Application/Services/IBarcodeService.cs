namespace ClothingStoreManagement.Application.Services
{
    public interface IBarcodeService
    {
        byte[] GenerateCode128(string value);
    }
}
