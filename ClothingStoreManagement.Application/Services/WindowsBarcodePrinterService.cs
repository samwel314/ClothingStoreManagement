using ClothingStoreManagement.Application.DTO;
using System.Drawing;
using System.Drawing.Printing;

namespace ClothingStoreManagement.Application.Services
{
    public class WindowsBarcodePrinterService : IWindowsBarcodePrinterService
    {
        public IReadOnlyList<string> GetInstalledPrinters()
        {
            return PrinterSettings
                .InstalledPrinters
                .Cast<string>()
                .OrderBy(x => x)
                .ToList();
        }

        public void Print(
            BarcodePrintRequestDto request,
            string printerName)
        {
            if (request.Labels.Count == 0)
                throw new InvalidOperationException("No barcode labels to print.");

            if (string.IsNullOrWhiteSpace(printerName))
                throw new ArgumentException(
                    "Printer name is required.",
                    nameof(printerName));

            using var document = new PrintDocument();

            document.PrinterSettings.PrinterName = printerName;

            if (!document.PrinterSettings.IsValid)
                throw new InvalidOperationException(
                    $"Printer '{printerName}' is not available.");

            var labelIndex = 0;

            document.PrintPage += (_, e) =>
            {
                const int columns = 3;
                const int rows = 8;

                const int horizontalMargin = 20;
                const int verticalMargin = 20;
                const int horizontalSpacing = 8;
                const int verticalSpacing = 8;

                var usableWidth =
                    e.PageBounds.Width
                    - (horizontalMargin * 2)
                    - (horizontalSpacing * (columns - 1));

                var usableHeight =
                    e.PageBounds.Height
                    - (verticalMargin * 2)
                    - (verticalSpacing * (rows - 1));

                var labelWidth = usableWidth / columns;
                var labelHeight = usableHeight / rows;

                for (var row = 0; row < rows; row++)
                {
                    for (var column = 0; column < columns; column++)
                    {
                        if (labelIndex >= request.Labels.Count)
                            break;

                        var x =
                            horizontalMargin
                            + column * (labelWidth + horizontalSpacing);

                        var y =
                            verticalMargin
                            + row * (labelHeight + verticalSpacing);

                        var bounds = new Rectangle(
                            x,
                            y,
                            labelWidth,
                            labelHeight);

                        DrawLabel(
                            e.Graphics,
                            request.Labels[labelIndex],
                            bounds);

                        labelIndex++;
                    }
                }

                e.HasMorePages =
                    labelIndex < request.Labels.Count;
            };

            document.Print();
        }

        private static void DrawLabel(
            Graphics graphics,
            BarcodeLabelDto label,
            Rectangle bounds)
        {
            const int padding = 6;

            using var titleFont =
                new Font("Arial", 9, FontStyle.Bold);

            using var detailsFont =
                new Font("Arial", 7);

            using var skuFont =
                new Font("Arial", 7, FontStyle.Bold);

            var centerX = bounds.Left + bounds.Width / 2;

            var y = bounds.Top + padding;

            DrawCenteredText(
                graphics,
                label.ProductName,
                titleFont,
                centerX,
                ref y,
                bounds.Right - padding);

            var details =
                $"{label.Color ?? ""} {label.Size ?? ""}".Trim();

            if (!string.IsNullOrWhiteSpace(details))
            {
                DrawCenteredText(
                    graphics,
                    details,
                    detailsFont,
                    centerX,
                    ref y,
                    bounds.Right - padding);
            }

            using var barcodeStream =
                new MemoryStream(label.BarcodeImage);

            using var barcode =
                Image.FromStream(barcodeStream);

            var barcodeWidth =
                Math.Min(
                    bounds.Width - (padding * 2),
                    barcode.Width);

            var barcodeHeight =
                (int)((double)barcode.Height / barcode.Width * barcodeWidth);

            var barcodeX =
                centerX - barcodeWidth / 2;

            graphics.DrawImage(
                barcode,
                barcodeX,
                y,
                barcodeWidth,
                barcodeHeight);

            y += barcodeHeight + 3;

            DrawCenteredText(
                graphics,
                label.Sku,
                skuFont,
                centerX,
                ref y,
                bounds.Right - padding);
        }

        private static void DrawCenteredText(
            Graphics graphics,
            string text,
            Font font,
            int centerX,
            ref int y,
            int rightLimit)
        {
            if (string.IsNullOrWhiteSpace(text))
                return;

            var size = graphics.MeasureString(text, font);

            var x = centerX - size.Width / 2;

            if (x < 0)
                x = 0;

            if (x + size.Width > rightLimit)
                x = Math.Max(0, rightLimit - (int)size.Width);

            graphics.DrawString(
                text,
                font,
                Brushes.Black,
                x,
                y);

            y += (int)size.Height + 2;
        }
    }
}
