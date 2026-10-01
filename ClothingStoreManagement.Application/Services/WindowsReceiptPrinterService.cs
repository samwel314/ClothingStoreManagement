using ClothingStoreManagement.Application.DTO;
using ClothingStoreManagement.Ui;
using System.Drawing;
using System.Drawing.Printing;
using System.Drawing.Text;

namespace ClothingStoreManagement.Application.Services
{
    public class WindowsReceiptPrinterService : IReceiptPrinterService
    {
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

                // 80mm Standard Thermal Paper
                printDocument.DefaultPageSettings.PaperSize = new PaperSize("Receipt 80mm", 280, 1200);
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

        private static void DrawReceipt(Graphics graphics, InvoiceDTO invoice)
        {
            graphics.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;

            // بداية نقطة رسم الفريم العلوية
            float frameStartY = 3;

            const float startX = 5;
            const float printableWidth = 270;
            float y = 10;

            // الخطوط
            using var titleFont = new Font("Segoe UI", 12, FontStyle.Bold);
            using var headerFont = new Font("Segoe UI", 8.5f, FontStyle.Regular);
            using var boldFont = new Font("Segoe UI", 8.5f, FontStyle.Bold);
            using var normalFont = new Font("Segoe UI", 8.5f, FontStyle.Regular);
            using var smallFont = new Font("Segoe UI", 7.5f, FontStyle.Regular);
            using var miniFont = new Font("Segoe UI", 6.5f, FontStyle.Regular);
            using var grandTotalFont = new Font("Segoe UI", 11f, FontStyle.Bold);

            // تنسيقات محاذاة قياسية ثابتة وبسيطة (بدون Flags معقدة)
            using var formatCenter = new StringFormat { Alignment = StringAlignment.Center };
            using var formatRight = new StringFormat { Alignment = StringAlignment.Far };  // يمين
            using var formatLeft = new StringFormat { Alignment = StringAlignment.Near };  // يسار

            // =========================
            // Header
            // =========================
            graphics.DrawString(ClientBaseData.Name ?? "متجر الملابس", titleFont, Brushes.Black, new RectangleF(startX, y, printableWidth, 22), formatCenter);
            y += 22;

            graphics.DrawString($"العنوان: {ClientBaseData.Address}", headerFont, Brushes.Black, new RectangleF(startX, y, printableWidth, 16), formatCenter);
            y += 15;

            graphics.DrawString($"تليفون المحل: {ClientBaseData.Phone}", headerFont, Brushes.Black, new RectangleF(startX, y, printableWidth, 16), formatCenter);
            y += 18;

            DrawLine(graphics, startX, ref y, printableWidth);

            // =========================
            // Invoice Meta
            // =========================
            graphics.DrawString($" {invoice.SerialNumber} : رقم ", boldFont, Brushes.Black, new RectangleF(startX, y, printableWidth, 18), formatCenter);
            y += 17;

            graphics.DrawString($"التاريخ: {invoice.LastUpdate:HH:mm dd/MM/yyyy}", boldFont, Brushes.Black, new RectangleF(startX, y, printableWidth, 18), formatCenter);
            y += 20;

            DrawLine(graphics, startX, ref y, printableWidth);

            // =========================
            // Items Table Header
            // =========================
            // توزيع المساحة من الشمال لليمن: [إجمالي: 80px] [كمية: 50px] [الصنف: 140px]
            float colLeftX = startX;              // تبدأ من 5
            float colCenterX = startX + 80;       // تبدأ من 85
            float colRightX = startX + 130;       // تبدأ من 135
            float colRightWidth = 140;

            graphics.DrawString(" الصنف ", boldFont, Brushes.Black, new RectangleF(colRightX, y, colRightWidth, 18), formatRight);
            graphics.DrawString("كمية", boldFont, Brushes.Black, new RectangleF(colCenterX, y, 50, 18), formatCenter);
            graphics.DrawString("إجمالي", boldFont, Brushes.Black, new RectangleF(colLeftX, y, 80, 18), formatLeft);

            y += 18;
            DrawLine(graphics, startX, ref y, printableWidth);

            // =========================
            // Items Body
            // =========================
            foreach (var item in invoice.Items)
            {
                float rowStartY = y;

                // اسم المنتج (يمين)
                graphics.DrawString(item.ProductName, boldFont, Brushes.Black, new RectangleF(colRightX, y, colRightWidth, 18), formatRight);
                y += 15;

                // المقاس واللون (يمين)
                graphics.DrawString($" {item.Size} | {item.Color} ", smallFont, Brushes.Black, new RectangleF(colRightX, y, colRightWidth, 15), formatRight);
                y += 14;

                if (item.Discount > 0)
                {
                    graphics.DrawString($" خصم {item.Discount}% ", smallFont, Brushes.Black, new RectangleF(colRightX, y, colRightWidth, 15), formatRight);
                    y += 14;
                }

                // الكمية (وسط)
                graphics.DrawString(item.Quantity.ToString(), normalFont, Brushes.Black, new RectangleF(colCenterX, rowStartY, 50, 18), formatCenter);

                // الإجمالي (يسار)
                graphics.DrawString(item.Total.ToString("N2"), normalFont, Brushes.Black, new RectangleF(colLeftX, rowStartY, 80, 18), formatLeft);

                y += 4;
            }

            DrawLine(graphics, startX, ref y, printableWidth);

            // =========================
            // Summary Section
            // =========================
            // الإجمالي: النص لليمين والمبلغ لليسار
            DrawSummaryRow(graphics, " : الإجمالي ", invoice.TotalAmount.ToString("N2"), normalFont, startX, printableWidth, ref y, formatRight, formatLeft);

            if (invoice.TotalAmount > invoice.TotalAmountWithDiscount)
            {
                var discountAmount = invoice.TotalAmount - invoice.TotalAmountWithDiscount;
                DrawSummaryRow(graphics, " : إجمالي الخصومات ", $"-{discountAmount:N2}", normalFont, startX, printableWidth, ref y, formatRight, formatLeft);
            }

            DrawLine(graphics, startX, ref y, printableWidth);

            // الصافي النهائي
            DrawSummaryRow(graphics, " : الصافي النهائي ", invoice.TotalAmountWithDiscount.ToString("N2"), grandTotalFont, startX, printableWidth, ref y, formatRight, formatLeft);

            DrawLine(graphics, startX, ref y, printableWidth);

            // =========================
            // Footer
            // =========================
            y += 4;
            graphics.DrawString("شكراً لزيارتكم", normalFont, Brushes.Black, new RectangleF(startX, y, printableWidth, 18), formatCenter);
            y += 18;

            graphics.DrawString("الاسترجاع خلال 14 يوم", smallFont, Brushes.Black, new RectangleF(startX, y, printableWidth, 16), formatCenter);
            y += 18;

            graphics.DrawString("Smart POS | Eng. Samuel Marzouk - 01005415303", miniFont, Brushes.Gray, new RectangleF(startX, y, printableWidth, 14), formatCenter);
            y += 16;

            // =========================
            // رسم الفريم الخارجي
            // =========================
            using var framePen = new Pen(Color.Black, 1.2f);
            graphics.DrawRectangle(framePen, startX - 2, frameStartY, printableWidth + 4, y - frameStartY);
        }

        private static void DrawSummaryRow(Graphics graphics, string label, string value, Font font, float x, float totalWidth, ref float y, StringFormat formatRight, StringFormat formatLeft)
        {
            float halfWidth = totalWidth / 2;

            // النص ناحية اليمين (النص يوضع في الجزء الأيمن)
            graphics.DrawString(label, font, Brushes.Black, new RectangleF(x + halfWidth, y, halfWidth, 20), formatRight);

            // المبلغ ناحية اليسار (المبلغ يوضع في الجزء الأيسر)
            graphics.DrawString(value, font, Brushes.Black, new RectangleF(x, y, halfWidth, 20), formatLeft);

            y += 20;
        }

        private static void DrawLine(Graphics graphics, float x, ref float y, float width)
        {
            y += 2;
            graphics.DrawLine(Pens.Black, x, y, x + width, y);
            y += 6;
        }
    }
}