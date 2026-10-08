using ClothingStoreManagement.Application.DTO;
using ClothingStoreManagement.Ui;
using System;
using System.Drawing;
using System.Drawing.Printing;
using System.Drawing.Text;
using QRCoder;

namespace ClothingStoreManagement.Application.Services
{
    public class WindowsReceiptPrinterService : IReceiptPrinterService
    {
        // XP-246B Max Printable Width is 48mm (~189 hundredths of an inch)
        private const float LabelWidthMm = 48f;

        public Task<bool> PrintAsync(InvoiceDTO invoice)
        {
            try
            {
                var printerSettings = new PrinterSettings();

                if (string.IsNullOrWhiteSpace(printerSettings.PrinterName) || !printerSettings.IsValid)
                    return Task.FromResult(false);

                using var printDocument = new PrintDocument
                {
                    PrinterSettings = printerSettings
                };

                // تحديد مقاس الورق بوضوح للطابعة
                var widthInHundredths = (int)Math.Round(LabelWidthMm / 25.4f * 100f);

                // الارتفاع هنا ديناميكي لورق الفواتير الحراري المستمر
                printDocument.DefaultPageSettings.PaperSize = new PaperSize("Receipt48mm", widthInHundredths, 1000);
                printDocument.DefaultPageSettings.Margins = new Margins(0, 0, 0, 0);

                printDocument.PrintPage += (_, e) =>
                {
                    DrawReceipt(e.Graphics, invoice);
                };

                printDocument.Print();
                return Task.FromResult(true);
            }
            catch
            {
                return Task.FromResult(false);
            }
        }

        private static float DrawReceipt(Graphics graphics, InvoiceDTO invoice)
        {
            graphics.TextRenderingHint = TextRenderingHint.SingleBitPerPixelGridFit;

            float frameStartY = 2;
            const float startX = 3;
            const float printableWidth = 175; // تناسب 44mm داخل الـ 48mm لمنع قطع الأطراف
            float y = 5;

            // الخطوط
            using var titleFont = new Font("Segoe UI", 9.5f, FontStyle.Bold);
            using var headerFont = new Font("Segoe UI", 7f, FontStyle.Regular);
            using var boldFont = new Font("Segoe UI", 7f, FontStyle.Bold);
            using var normalFont = new Font("Segoe UI", 7f, FontStyle.Regular);
            using var smallFont = new Font("Segoe UI", 6f, FontStyle.Regular);
            using var miniFont = new Font("Segoe UI", 5.5f, FontStyle.Regular);
            using var grandTotalFont = new Font("Segoe UI", 8.5f, FontStyle.Bold);

            using var formatCenter = new StringFormat { Alignment = StringAlignment.Center, FormatFlags = StringFormatFlags.DirectionRightToLeft };
            using var formatRight = new StringFormat { Alignment = StringAlignment.Near, FormatFlags = StringFormatFlags.DirectionRightToLeft };
            using var formatLeft = new StringFormat { Alignment = StringAlignment.Far, FormatFlags = StringFormatFlags.DirectionRightToLeft };

            // =========================
            // Header
            // =========================
            graphics.DrawString(ClientBaseData.Name ?? "متجر الملابس", titleFont, Brushes.Black, new RectangleF(startX, y, printableWidth, 18), formatCenter);
            y += 16;

            if (!string.IsNullOrWhiteSpace(ClientBaseData.Address))
            {
                graphics.DrawString($"العنوان: {ClientBaseData.Address}", headerFont, Brushes.Black, new RectangleF(startX, y, printableWidth, 14), formatCenter);
                y += 12;
            }

            if (!string.IsNullOrWhiteSpace(ClientBaseData.Phone))
            {
                graphics.DrawString($"تليفون: {ClientBaseData.Phone}", headerFont, Brushes.Black, new RectangleF(startX, y, printableWidth, 14), formatCenter);
                y += 12;
            }

            DrawLine(graphics, startX, ref y, printableWidth);

            // =========================
            // Invoice Meta
            // =========================
            graphics.DrawString($"رقم الفاتورة: {invoice.SerialNumber}", boldFont, Brushes.Black, new RectangleF(startX, y, printableWidth, 14), formatCenter);
            y += 13;

            graphics.DrawString($"التاريخ: {invoice.LastUpdate:dd/MM/yyyy HH:mm}", boldFont, Brushes.Black, new RectangleF(startX, y, printableWidth, 14), formatCenter);
            y += 14;

            DrawLine(graphics, startX, ref y, printableWidth);

            // =========================
            // Items Table Header
            // =========================
            float colLeftX = startX;              // الإجمالي (اليسار)
            float colCenterX = startX + 50;        // الكمية (الوسط)
            float colRightX = startX + 80;         // الصنف (اليمين)
            float colRightWidth = 95;

            graphics.DrawString("الصنف", boldFont, Brushes.Black, new RectangleF(colRightX, y, colRightWidth, 14), formatRight);
            graphics.DrawString("ك", boldFont, Brushes.Black, new RectangleF(colCenterX, y, 30, 14), formatCenter);
            graphics.DrawString("الإجمالي", boldFont, Brushes.Black, new RectangleF(colLeftX, y, 50, 14), formatLeft);

            y += 13;
            DrawLine(graphics, startX, ref y, printableWidth);

            // =========================
            // Items Body
            // =========================
            foreach (var item in invoice.Items)
            {
                float rowStartY = y;

                SizeF nameSize = graphics.MeasureString(item.ProductName, boldFont, (int)colRightWidth);
                float nameHeight = Math.Max(13, nameSize.Height);

                graphics.DrawString(item.ProductName, boldFont, Brushes.Black, new RectangleF(colRightX, y, colRightWidth, nameHeight), formatRight);
                y += nameHeight;

                string details = $"{item.Size} | {item.Color}".Trim(' ', '|');
                if (!string.IsNullOrWhiteSpace(details))
                {
                    graphics.DrawString(details, smallFont, Brushes.Black, new RectangleF(colRightX, y, colRightWidth, 11), formatRight);
                    y += 11;
                }

                if (item.Discount > 0)
                {
                    graphics.DrawString($"خصم {item.Discount}%", smallFont, Brushes.Black, new RectangleF(colRightX, y, colRightWidth, 11), formatRight);
                    y += 11;
                }

                graphics.DrawString(item.Quantity.ToString(), normalFont, Brushes.Black, new RectangleF(colCenterX, rowStartY, 30, 14), formatCenter);
                graphics.DrawString(item.Total.ToString("N2"), normalFont, Brushes.Black, new RectangleF(colLeftX, rowStartY, 50, 14), formatLeft);

                y += 2;
            }

            DrawLine(graphics, startX, ref y, printableWidth);

            // =========================
            // Summary Section
            // =========================
            DrawSummaryRow(graphics, "الإجمالي:", invoice.TotalAmount.ToString("N2"), normalFont, startX, printableWidth, ref y, formatRight, formatLeft);

            if (invoice.TotalAmount > invoice.TotalAmountWithDiscount)
            {
                var discountAmount = invoice.TotalAmount - invoice.TotalAmountWithDiscount;
                DrawSummaryRow(graphics, "الخصم:", $"-{discountAmount:N2}", normalFont, startX, printableWidth, ref y, formatRight, formatLeft);
            }

            DrawLine(graphics, startX, ref y, printableWidth);

            // الصافي النهائي
            DrawSummaryRow(graphics, "الصافي:", invoice.TotalAmountWithDiscount.ToString("N2"), grandTotalFont, startX, printableWidth, ref y, formatRight, formatLeft);

            DrawLine(graphics, startX, ref y, printableWidth);

            // =========================
            // Footer & QR Code
            // =========================
            y += 2;
            graphics.DrawString("شكراً لزيارتكم - الاسترجاع خلال 14 يوم", smallFont, Brushes.Black, new RectangleF(startX, y, printableWidth, 12), formatCenter);
            y += 12;

            string facebookUrl = ClientBaseData.FacebookUrl;
            if (!string.IsNullOrWhiteSpace(facebookUrl))
            {
                DrawQRCode(graphics, facebookUrl, startX, printableWidth, ref y, qrSize: 38);
            }

            graphics.DrawString("Powered by POS System", miniFont, Brushes.Gray, new RectangleF(startX, y, printableWidth, 10), formatCenter);
            y += 11;

            // رسم الفريم الخارجي
            using var framePen = new Pen(Color.Black, 1.0f);
            graphics.DrawRectangle(framePen, startX - 1, frameStartY, printableWidth + 2, y - frameStartY);

            return y;
        }

        private static void DrawQRCode(Graphics graphics, string text, float startX, float totalWidth, ref float y, int qrSize = 38)
        {
            try
            {
                using var qrGenerator = new QRCodeGenerator();
                using var qrData = qrGenerator.CreateQrCode(text, QRCodeGenerator.ECCLevel.M);
                using var qrCode = new QRCode(qrData);
                using Bitmap qrImage = qrCode.GetGraphic(3);

                float qrX = startX + (totalWidth - qrSize) / 2;

                graphics.DrawImage(qrImage, qrX, y, qrSize, qrSize);
                y += qrSize + 3;
            }
            catch
            {
                // تجاوز الخطأ لضمان استمرار الطباعة
            }
        }

        private static void DrawSummaryRow(Graphics graphics, string label, string value, Font font, float x, float totalWidth, ref float y, StringFormat formatRight, StringFormat formatLeft)
        {
            float halfWidth = totalWidth / 2;
            graphics.DrawString(label, font, Brushes.Black, new RectangleF(x + halfWidth, y, halfWidth, 14), formatRight);
            graphics.DrawString(value, font, Brushes.Black, new RectangleF(x, y, halfWidth, 14), formatLeft);
            y += 14;
        }

        private static void DrawLine(Graphics graphics, float x, ref float y, float width)
        {
            y += 1;
            graphics.DrawLine(Pens.Black, x, y, x + width, y);
            y += 3;
        }
    }
}