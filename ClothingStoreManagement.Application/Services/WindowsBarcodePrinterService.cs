
using ClothingStoreManagement.Application.DTO;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.Drawing.Text;

namespace ClothingStoreManagement.Application.Services
{
    public class WindowsBarcodePrinterService : IWindowsBarcodePrinterService
    {
        // Physical sticker size: 40 x 25 mm
        private const float LabelWidthMm = 40f;
        private const float LabelHeightMm = 25f;

        public IReadOnlyList<string> GetInstalledPrinters()
        {
            return PrinterSettings
                .InstalledPrinters
                .Cast<string>()
                .OrderBy(x => x)
                .ToList();
        }
        public void PrintVariant(
    BarcodeLabelDto label,
    int quantity,
    string printerName)
        {
            ArgumentNullException.ThrowIfNull(label);

            if (quantity is < 1 or > 500)
                throw new ArgumentOutOfRangeException(
                    nameof(quantity),
                    "عدد الاستيكرات يجب أن يكون من 1 إلى 500.");

            var request = new BarcodePrintRequestDto();

            for (var i = 0; i < quantity; i++)
            {
                request.Labels.Add(label);
            }

            Print(request, printerName);
        }
        public void Print(
            BarcodePrintRequestDto request,
            string printerName)
        {
            if (request.Labels == null || request.Labels.Count == 0)
                throw new InvalidOperationException(
                    "No barcode labels to print.");

            if (string.IsNullOrWhiteSpace(printerName))
                throw new ArgumentException(
                    "Printer name is required.",
                    nameof(printerName));

            using var document = new PrintDocument();

            document.PrinterSettings.PrinterName = printerName;

            if (!document.PrinterSettings.IsValid)
            {
                throw new InvalidOperationException(
                    $"Printer '{printerName}' is not available.");
            }

            var widthInHundredths =
                MmToHundredthsOfInch(LabelWidthMm);

            var heightInHundredths =
                MmToHundredthsOfInch(LabelHeightMm);

            var customPaperSize = new PaperSize(
                "BarcodeLabel40x25",
                widthInHundredths,
                heightInHundredths);

            document.DefaultPageSettings.PaperSize =
                customPaperSize;

            document.DefaultPageSettings.Margins =
                new Margins(0, 0, 0, 0);

            document.OriginAtMargins = false;

            var labelIndex = 0;

            document.PrintPage += (_, e) =>
            {
                ConfigureGraphics(e.Graphics);

                var bounds = new Rectangle(
                    0,
                    0,
                    widthInHundredths,
                    heightInHundredths);

                DrawLabel(
                    e.Graphics,
                    request.Labels[labelIndex],
                    bounds);

                labelIndex++;

                e.HasMorePages =
                    labelIndex < request.Labels.Count;
            };

            document.Print();
        }

        private static void ConfigureGraphics(Graphics graphics)
        {
            graphics.TextRenderingHint =
                TextRenderingHint.SingleBitPerPixelGridFit;

            graphics.SmoothingMode =
                SmoothingMode.None;

            graphics.PixelOffsetMode =
                PixelOffsetMode.Half;

            graphics.InterpolationMode =
                InterpolationMode.NearestNeighbor;

            graphics.CompositingMode =
                CompositingMode.SourceCopy;
        }

        private static int MmToHundredthsOfInch(float mm)
        {
            return (int)Math.Round(
                mm / 25.4f * 100f);
        }

        private static void DrawLabel(
            Graphics graphics,
            BarcodeLabelDto label,
            Rectangle bounds)
        {
            var y = 1;

            var centerX = bounds.Width / 2;

            using var titleFont = new Font(
                "Arial",
                6.5f,
                FontStyle.Bold,
                GraphicsUnit.Point);

            using var detailsFont = new Font(
                "Arial",
                5.5f,
                FontStyle.Regular,
                GraphicsUnit.Point);

            using var skuFont = new Font(
                "Arial",
                6f,
                FontStyle.Bold,
                GraphicsUnit.Point);

            using var priceFont = new Font(
                "Arial",
                6f,
                FontStyle.Bold,
                GraphicsUnit.Point);

            // 1. Product name
            DrawCenteredText(
                graphics,
                label.ProductName,
                titleFont,
                centerX,
                ref y);

            // 2. Color + Size
            var details =
                $"{label.Color ?? ""} {label.Size ?? ""}"
                    .Trim()
                    .ToUpperInvariant();

            if (!string.IsNullOrWhiteSpace(details))
            {
                DrawCenteredText(
                    graphics,
                    details,
                    detailsFont,
                    centerX,
                    ref y);
            }

            y += 1;

            // 3. Barcode
            if (label.BarcodeImage != null &&
                label.BarcodeImage.Length > 0)
            {
                using var barcodeStream =
                    new MemoryStream(label.BarcodeImage);

                using var barcode =
                    Image.FromStream(barcodeStream);

                var barcodeWidth =
                    (int)(bounds.Width * 0.85f);

                var barcodeHeight =
                    MmToHundredthsOfInch(8f);

                var barcodeX =
                    centerX - barcodeWidth / 2;

                graphics.DrawImage(
                    barcode,
                    new Rectangle(
                        barcodeX,
                        y,
                        barcodeWidth,
                        barcodeHeight));

                y += barcodeHeight + 1;
            }

            // 4. SKU
            var skuText =
                label.Sku?
                    .Trim()
                    .ToUpperInvariant()
                ?? string.Empty;

            DrawCenteredText(
                graphics,
                skuText,
                skuFont,
                centerX,
                ref y);

            // 5. Price
            var priceText = $"{label.Price:N2} EGP";

            DrawCenteredText(
                graphics,
                priceText,
                priceFont,
                centerX,
                ref y);
        }
        private static void DrawCenteredText(
            Graphics graphics,
            string text,
            Font font,
            int centerX,
            ref int y)
        {
            if (string.IsNullOrWhiteSpace(text))
                return;

            var size = graphics.MeasureString(
                text,
                font);

            var x =
                centerX - (int)(size.Width / 2f);

            if (x < 0)
                x = 0;

            graphics.DrawString(
                text,
                font,
                Brushes.Black,
                x,
                y);

            y += (int)size.Height;
        }
    }
}
