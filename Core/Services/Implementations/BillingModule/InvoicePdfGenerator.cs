using Domain.Models.BillingModule;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Services.Abstraction.Contracts.BillingService;

namespace Services.Implementations.BillingModule
{
    public sealed class InvoicePdfGenerator : IInvoicePdfGenerator
    {
        // QuestPDF requires the community license to be set once on startup.
        // Register QuestPDF.Infrastructure.QuestPDF.Settings.LicenseType = LicenseType.Community
        // in your DI registration or Program.cs before calling Generate().

        public byte[] Generate(Invoice invoice, string patientName, string patientEmail)
        {
            var document = new InvoicePdfDocument(invoice, patientName, patientEmail);
            return document.GeneratePdf();
        }
    }

    internal sealed class InvoicePdfDocument(Invoice invoice, string patientName, string patientEmail)
    : IDocument
    {
        // ── Colour tokens ──────────────────────────────────────────────────────
        private static readonly string _primaryColor = "#1E3A5F";   // Dark navy
        private static readonly string _accentColor = "#0078D4";    // Blue
        private static readonly string _lightGray = "#F5F5F5";
        private static readonly string _borderGray = "#D0D0D0";

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(40);

                page.Header().Element(ComposeHeader);
                page.Content().Element(ComposeContent);
                page.Footer().Element(ComposeFooter);
            });
        }

        // ── Header ─────────────────────────────────────────────────────────────

        private void ComposeHeader(IContainer container)
        {
            container.Column(col =>
            {
                col.Item().Row(row =>
                {
                    // Hospital branding
                    row.RelativeItem().Column(inner =>
                    {
                        inner.Item().Text("HMS — Hospital Management System")
                            .FontSize(18).Bold().FontColor(_primaryColor);
                        inner.Item().Text("Medical Invoice")
                            .FontSize(11).FontColor(_accentColor);
                    });

                    // Invoice meta
                    row.ConstantItem(180).Column(inner =>
                    {
                        inner.Item().AlignRight().Text(invoice.InvoiceNumber)
                            .FontSize(13).Bold().FontColor(_primaryColor);
                        inner.Item().AlignRight().Text($"Issued: {invoice.IssuedAt?.ToString("dd MMM yyyy") ?? "Draft"}")
                            .FontSize(9).FontColor(Colors.Grey.Medium);
                        if (invoice.DueDate.HasValue)
                            inner.Item().AlignRight().Text($"Due: {invoice.DueDate:dd MMM yyyy}")
                                .FontSize(9).FontColor(Colors.Red.Medium);
                    });
                });

                col.Item().PaddingVertical(4)
                   .LineHorizontal(1).LineColor(_borderGray);
            });
        }

        // ── Content ────────────────────────────────────────────────────────────

        private void ComposeContent(IContainer container)
        {
            container.Column(col =>
            {
                col.Spacing(12);

                // Patient info
                col.Item().Element(ComposePatientInfo);

                // Line items table
                col.Item().Element(ComposeLineItemsTable);

                // Totals
                col.Item().Element(ComposeTotals);

                // Payment status
                if (invoice.PaidAmount > 0)
                    col.Item().Element(ComposePaymentSummary);

                // Notes
                if (!string.IsNullOrWhiteSpace(invoice.Notes))
                {
                    col.Item().Background(_lightGray).Padding(8).Column(n =>
                    {
                        n.Item().Text("Notes").Bold().FontSize(9);
                        n.Item().Text(invoice.Notes).FontSize(9).FontColor(Colors.Grey.Darken1);
                    });
                }
            });
        }

        private void ComposePatientInfo(IContainer container)
        {
            container.Background(_lightGray).Padding(10).Row(row =>
            {
                row.RelativeItem().Column(col =>
                {
                    col.Item().Text("Bill To").Bold().FontSize(9).FontColor(_accentColor);
                    col.Item().Text(patientName).Bold().FontSize(11);
                    if (!string.IsNullOrWhiteSpace(patientEmail))
                        col.Item().Text(patientEmail).FontSize(9).FontColor(Colors.Grey.Darken1);
                    col.Item().Text($"Patient ID: {invoice.PatientId}").FontSize(9).FontColor(Colors.Grey.Medium);
                });

                row.RelativeItem().Column(col =>
                {
                    col.Item().Text("Invoice Status").Bold().FontSize(9).FontColor(_accentColor);
                    col.Item().Text(invoice.Status.ToString())
                        .Bold().FontSize(11)
                        .FontColor(invoice.Status == Domain.Models.Enums.BillingEnums.InvoiceStatus.Paid
                            ? Colors.Green.Darken2 : _primaryColor);
                });
            });
        }

        private void ComposeLineItemsTable(IContainer container)
        {
            container.Table(table =>
            {
                table.ColumnsDefinition(cols =>
                {
                    cols.RelativeColumn(4);   // Description
                    cols.RelativeColumn(2);   // Type
                    cols.ConstantColumn(50);  // Qty
                    cols.ConstantColumn(80);  // Unit Price
                    cols.ConstantColumn(80);  // Total
                });

                // Header
                table.Header(header =>
                {
                    foreach (var label in new[] { "Description", "Type", "Qty", "Unit Price", "Total" })
                    {
                        header.Cell().Background(_primaryColor).Padding(5)
                              .Text(label).Bold().FontSize(9).FontColor(Colors.White);
                    }
                });

                // Rows
                var isOdd = false;
                foreach (var item in invoice.LineItems)
                {
                    var bg = isOdd ? Colors.White : _lightGray;
                    isOdd = !isOdd;

                    table.Cell().Background(bg).Padding(5)
                         .Text(item.Description).FontSize(9);
                    table.Cell().Background(bg).Padding(5)
                         .Text(item.LineItemType.ToString()).FontSize(9);
                    table.Cell().Background(bg).Padding(5).AlignCenter()
                         .Text(item.Quantity.ToString()).FontSize(9);
                    table.Cell().Background(bg).Padding(5).AlignRight()
                         .Text($"{item.UnitPrice:N2}").FontSize(9);
                    table.Cell().Background(bg).Padding(5).AlignRight()
                         .Text($"{item.Total:N2}").FontSize(9).Bold();
                }
            });
        }

        private void ComposeTotals(IContainer container)
        {
            container.AlignRight().Table(table =>
            {
                table.ColumnsDefinition(cols =>
                {
                    cols.ConstantColumn(150);
                    cols.ConstantColumn(100);
                });

                void AddRow(string label, string value, bool bold = false, string? color = null)
                {
                    table.Cell().Padding(3).Text(label)
                        .FontSize(9).Bold(bold).FontColor(color ?? Colors.Black);
                    table.Cell().Padding(3).AlignRight().Text(value)
                        .FontSize(9).Bold(bold).FontColor(color ?? Colors.Black);
                }

                AddRow("Sub-Total", $"{invoice.SubTotal:N2}");

                if (invoice.DiscountAmount > 0 || invoice.DiscountPercent > 0)
                    AddRow("Discount", $"- {invoice.DiscountAmount + invoice.SubTotal * invoice.DiscountPercent / 100m:N2}",
                        color: Colors.Green.Darken2);

                if (invoice.TaxPercent > 0)
                    AddRow($"Tax ({invoice.TaxPercent:N1}%)", $"{invoice.TaxAmount:N2}");

                table.Cell().ColumnSpan(2).LineHorizontal(1).LineColor(_borderGray);

                AddRow("Total", $"{invoice.TotalAmount:N2}", bold: true, color: _primaryColor);
            });
        }

        private void ComposePaymentSummary(IContainer container)
        {
            container.Background(_lightGray).Padding(8).Row(row =>
            {
                row.RelativeItem().Column(col =>
                {
                    col.Item().Text("Payment Summary").Bold().FontSize(9).FontColor(_accentColor);
                    foreach (var payment in invoice.Payments
                                 .Where(p => p.Status == Domain.Models.Enums.BillingEnums.PaymentStatus.Succeeded))
                    {
                        col.Item().Text(
                            $"{payment.PaidAt?.ToString("dd MMM yyyy")} — {payment.PaymentMethod} — {payment.Amount:N2}")
                            .FontSize(9);
                    }
                });
                row.ConstantItem(140).Column(col =>
                {
                    col.Item().AlignRight().Text($"Paid: {invoice.PaidAmount:N2}").FontSize(9).FontColor(Colors.Green.Darken2);
                    col.Item().AlignRight().Text($"Outstanding: {invoice.OutstandingBalance:N2}")
                        .FontSize(10).Bold()
                        .FontColor(invoice.OutstandingBalance > 0 ? Colors.Red.Medium : Colors.Green.Darken2);
                });
            });
        }

        // ── Footer ─────────────────────────────────────────────────────────────

        private void ComposeFooter(IContainer container)
        {
            container.Column(col =>
            {
                col.Item().LineHorizontal(1).LineColor(_borderGray);
                col.Item().PaddingTop(4).Row(row =>
                {
                    row.RelativeItem().Text("Thank you for choosing HMS. Please retain this invoice for your records.")
                        .FontSize(8).FontColor(Colors.Grey.Medium);
                    row.ConstantItem(80).AlignRight()
                        .Text(text =>
                        {
                            text.Span("Page ").FontSize(8).FontColor(Colors.Grey.Medium);
                            text.CurrentPageNumber().FontSize(8).FontColor(Colors.Grey.Medium);
                            text.Span(" of ").FontSize(8).FontColor(Colors.Grey.Medium);
                            text.TotalPages().FontSize(8).FontColor(Colors.Grey.Medium);
                        });
                });
            });
        }
    }
}
