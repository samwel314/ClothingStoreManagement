using System.Drawing;
using System.Drawing.Imaging;
using ZXing;
using ZXing.Common;
using ZXing.Windows.Compatibility;

namespace ClothingStoreManagement.Application.Services
{
    public class BarcodeService : IBarcodeService
    {
        public byte[] GenerateCode128(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Barcode value cannot be empty.", nameof(value));

            var writer = new BarcodeWriter<Bitmap>
            {
                Format = BarcodeFormat.CODE_128,
                Options = new EncodingOptions
                {
                    Width = 400,
                    Height = 120,
                    Margin = 10,
                    PureBarcode = true
                },
                Renderer = new BitmapRenderer()
            };

            using var bitmap = writer.Write(value);
            using var stream = new MemoryStream();

            bitmap.Save(stream, ImageFormat.Png);

            return stream.ToArray();
        }
    }
}
