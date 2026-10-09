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
                throw new ArgumentException(
                    "Barcode value cannot be empty.",
                    nameof(value));

            var formattedValue = value
                .Trim()
                .ToUpperInvariant();

            var writer = new BarcodeWriter<Bitmap>
            {
                Format = BarcodeFormat.CODE_128,

                Options = new EncodingOptions
                {
                    Width = 260,
                    Height = 80,
                    Margin = 0,
                    PureBarcode = true
                },

                Renderer = new BitmapRenderer()
            };

            using var bitmap = writer.Write(formattedValue);
            using var stream = new MemoryStream();

            bitmap.Save(stream, ImageFormat.Png);

            return stream.ToArray();
        }
    }
}