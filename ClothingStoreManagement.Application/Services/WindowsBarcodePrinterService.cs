using ClothingStoreManagement.Application.DTO;
using System.Drawing;
using System.Drawing.Printing;

namespace ClothingStoreManagement.Application.Services
{
    public class WindowsBarcodePrinterService : IWindowsBarcodePrinterService
    {
        // XP-246B Specs: Max width 48mm
        private const float LabelWidthMm = 48f;
        private const float LabelHeightMm = 30f;

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
            if (request.Labels == null || request.Labels.Count == 0)
                throw new InvalidOperationException("No barcode labels to print.");

            if (string.IsNullOrWhiteSpace(printerName))
                throw new ArgumentException("Printer name is required.", nameof(printerName));

            using var document = new PrintDocument();

            document.PrinterSettings.PrinterName = printerName;

            if (!document.PrinterSettings.IsValid)
                throw new InvalidOperationException($"Printer '{printerName}' is not available.");

            // 1. تحديد مقاس الورقة بوضوح بالطول والعرض بالـ Hundredths of an Inch
            var widthInHundredths = MmToHundredthsOfInch(LabelWidthMm);
            var heightInHundredths = MmToHundredthsOfInch(LabelHeightMm);

            var customPaperSize = new PaperSize("CustomBarcodeLabel", widthInHundredths, heightInHundredths);

            document.DefaultPageSettings.PaperSize = customPaperSize;
            document.DefaultPageSettings.Margins = new Margins(0, 0, 0, 0);

            var labelIndex = 0;

            document.PrintPage += (_, e) =>
            {
                // إيقاف التنعيم (AntiAlias) لجعل الباركوود والنصوص حادة وواضحة للمستشعر الضوئي
                e.Graphics.TextRenderingHint = System.Drawing.Text.TextRenderingHint.SingleBitPerPixelGridFit;
                e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.NearestNeighbor;

                var bounds = new Rectangle(0, 0, widthInHundredths, heightInHundredths);

                DrawLabel(e.Graphics, request.Labels[labelIndex], bounds);

                labelIndex++;

                // الشرط ده هو اللي بيخلي PrintDocument يكرر حدث PrintPage لكل عنصر في الـ List
                e.HasMorePages = labelIndex < request.Labels.Count;
            };

            document.Print();
        }

        private static int MmToHundredthsOfInch(float mm)
        {
            return (int)Math.Round(mm / 25.4f * 100f);
        }

        private static void DrawLabel(
            Graphics graphics,
            BarcodeLabelDto label,
            Rectangle bounds)
        {
            const int padding = 3;

            using var titleFont = new Font("Arial", 7, FontStyle.Bold);
            using var detailsFont = new Font("Arial", 6, FontStyle.Regular);
            using var skuFont = new Font("Arial", 6, FontStyle.Bold);

            var centerX = bounds.Left + bounds.Width / 2;
            var y = bounds.Top + padding;

            // 1. اسم المنتج
            DrawCenteredText(graphics, label.ProductName, titleFont, centerX, ref y, bounds.Right - padding);

            // 2. التفاصيل (اللون / المقاس)
            var details = $"{label.Color ?? ""} {label.Size ?? ""}".Trim();
            if (!string.IsNullOrWhiteSpace(details))
            {
                DrawCenteredText(graphics, details, detailsFont, centerX, ref y, bounds.Right - padding);
            }

            // 3. صورة الباركوود (مع التخلص الآمن من الـ Memory Stream)
            if (label.BarcodeImage != null && label.BarcodeImage.Length > 0)
            {
                using var barcodeStream = new MemoryStream(label.BarcodeImage);
                using var barcode = Image.FromStream(barcodeStream);

                var maxBarcodeWidth = bounds.Width - (padding * 2);
                var barcodeWidth = Math.Min(maxBarcodeWidth, barcode.Width);

                // الحفاظ على النسبة والتناسب للباركوود
                var barcodeHeight = (int)((double)barcode.Height / barcode.Width * barcodeWidth);

                // تقييم ارتفاع الباركوود عشان ما يخرجش برة الاستيكر
                barcodeHeight = Math.Min(barcodeHeight, 40);

                var barcodeX = centerX - barcodeWidth / 2;

                graphics.DrawImage(barcode, barcodeX, y, barcodeWidth, barcodeHeight);

                y += barcodeHeight + 2;
            }

            // 4. كود الـ SKU / الباركوود النصي
            DrawCenteredText(graphics, label.Sku, skuFont, centerX, ref y, bounds.Right - padding);
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
            var x = centerX - (int)(size.Width / 2);

            if (x < 0) x = 0;

            graphics.DrawString(text, font, Brushes.Black, x, y);

            y += (int)size.Height;
        }
    }
}