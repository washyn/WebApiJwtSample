using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace QuestPDF.Invoice
{
    public sealed class InvoiceTextResources
    {
        public string CultureName { get; set; } = "es-PE";
        public string CurrencyName { get; set; } = "Soles";

        public string LabelCustomer { get; set; } = "Cliente: ";
        public string LabelPaymentMethod { get; set; } = "Método de Pago: ";
        public string LabelIssueDate { get; set; } = "Fecha de Operación: ";
        public string LabelPhone { get; set; } = "Celular (Bipay): ";
        public string LabelTransactionId { get; set; } = "ID de Transacción Bipay: ";
        public string LabelDocumentNumber { get; set; } = "Nro. de Comprobante: ";
        public string LabelRequestId { get; set; } = "ID de Solicitud UNAJ: ";

        public string LabelAutenticidadTitle { get; set; } = "AUTENTICIDAD DEL DOCUMENTO";
        public string LabelAutenticidadMessage { get; set; } = "Este comprobante fue generado automáticamente tras confirmar el pago en Bipay. Para verificar su autenticidad, utilice el código de verificación o escanee el código QR.";
        public string LabelVerificationCode { get; set; } = "Código de Verificación: ";
        public string LabelVerificationUrl { get; set; } = "URL de Verificación: ";

        public string LabelFooterText { get; set; } = "Emitido electrónicamente por la UNAJ — ";

        public string LabelTableHeaderCode { get; set; } = "Código";
        public string LabelTableHeaderConcept { get; set; } = "Concepto";
        public string LabelTableHeaderAmount { get; set; } = "Importe";

        public string LabelAmountInWordsPrefix { get; set; } = "Son: ";
        public string LabelTotalAmount { get; set; } = "Importe total: ";

        public string Message { get; set; } = "Documento emitido con fines de constancia de pago por servicios académicos y administrativos. No sustituye a una factura ni boleta de venta, salvo que el concepto así lo requiera. Cualquier alteración o enmendadura invalida este comprobante. En caso de dudas, verifique la validez del documento en el portal oficial de la universidad utilizando el código de verificación correspondiente.";
        public string MessageWarning { get; set; } = "La reproducción no autorizada o falsificación de este documento constituye una infracción penalizada conforme a las normas vigentes.";
    }

    public sealed class InvoiceLabelValueLine
    {
        public InvoiceLabelValueLine(string label, string value)
        {
            Label = label ?? string.Empty;
            Value = value ?? string.Empty;
        }

        public string Label { get; }
        public string Value { get; }
    }

    public sealed class InvoiceTableItemViewModel
    {
        public InvoiceTableItemViewModel(string index, string description, string total)
        {
            Index = index ?? string.Empty;
            Description = description ?? string.Empty;
            Total = total ?? string.Empty;
        }

        public string Index { get; }
        public string Description { get; }
        public string Total { get; }
    }

    public sealed class InvoiceDocumentViewModel
    {
        public string SellerName { get; init; } = string.Empty;
        public string SellerSubtext1 { get; init; } = string.Empty;
        public string SellerSubtext2 { get; init; } = string.Empty;
        public string SellerLogoPath { get; init; } = string.Empty;

        public string DocumentType { get; init; } = string.Empty;
        public string DocumentNumber { get; init; } = string.Empty;

        public IReadOnlyList<InvoiceLabelValueLine> CustomerInfoLines { get; init; } = Array.Empty<InvoiceLabelValueLine>();

        public string TableHeaderCode { get; init; } = string.Empty;
        public string TableHeaderConcept { get; init; } = string.Empty;
        public string TableHeaderAmount { get; init; } = string.Empty;
        public IReadOnlyList<InvoiceTableItemViewModel> Items { get; init; } = Array.Empty<InvoiceTableItemViewModel>();

        public string AmountInWordsLine { get; init; } = string.Empty;
        public string TotalAmountLine { get; init; } = string.Empty;

        public string AuthenticityTitle { get; init; } = string.Empty;
        public string AuthenticityMessage { get; init; } = string.Empty;
        public string VerificationCodeLine { get; init; } = string.Empty;
        public string VerificationUrlLine { get; init; } = string.Empty;
        public string AuthenticityLogoPath { get; init; } = string.Empty;

        public string Message { get; init; } = string.Empty;
        public string MessageWarning { get; init; } = string.Empty;

        public string FooterPrefix { get; init; } = string.Empty;
        public string FooterDocumentNumber { get; init; } = string.Empty;
    }

    public static class InvoiceDocumentViewModelFactory
    {
        public static InvoiceDocumentViewModel Create(
            InvoiceModel model,
            InvoiceTextResources textResources = null,
            CultureInfo cultureInfo = null)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            textResources ??= new InvoiceTextResources();
            cultureInfo ??= new CultureInfo(textResources.CultureName);

            var documentType = model.Details?.DocumentType ?? string.Empty;
            var documentNumber = model.Details?.DocumentNumber ?? string.Empty;

            var issueDateText = model.Details != null
                ? model.Details.IssueDate.ToString("D", cultureInfo)
                : string.Empty;

            var items = model.Items ?? new List<InvoiceItemModel>();

            var viewItems = items.Select(x =>
                    new InvoiceTableItemViewModel(
                        x.Index.ToString(cultureInfo),
                        x.Description ?? string.Empty,
                        x.Total.ToString("C", cultureInfo)))
                .ToList();

            var totalAmount = items.Sum(x => x.Total);
            var amountInWords = NumberLetter.ConvertToLetter(totalAmount, textResources.CurrencyName);

            return new InvoiceDocumentViewModel
            {
                SellerName = model.Seller?.Name ?? string.Empty,
                SellerSubtext1 = model.Seller?.Subtext1 ?? string.Empty,
                SellerSubtext2 = model.Seller?.Subtext2 ?? string.Empty,
                SellerLogoPath = model.Seller?.LogoPath ?? string.Empty,

                DocumentType = documentType,
                DocumentNumber = documentNumber,

                CustomerInfoLines = new List<InvoiceLabelValueLine>
                {
                    new(textResources.LabelCustomer, model.Customer?.Name ?? string.Empty),
                    new(textResources.LabelPaymentMethod, model.PaymentMethod ?? string.Empty),
                    new(textResources.LabelIssueDate, issueDateText),
                    new(textResources.LabelPhone, model.Customer?.Phone ?? string.Empty),
                    new(textResources.LabelTransactionId, model.Details?.TransactionId ?? string.Empty),
                    new(textResources.LabelDocumentNumber, documentNumber),
                    new(textResources.LabelRequestId, model.Details?.RequestId ?? string.Empty)
                },

                TableHeaderCode = textResources.LabelTableHeaderCode,
                TableHeaderConcept = textResources.LabelTableHeaderConcept,
                TableHeaderAmount = textResources.LabelTableHeaderAmount,
                Items = viewItems,

                AmountInWordsLine = $"{textResources.LabelAmountInWordsPrefix}{amountInWords}",
                TotalAmountLine = $"{textResources.LabelTotalAmount}{totalAmount.ToString("C", cultureInfo)}",

                AuthenticityTitle = textResources.LabelAutenticidadTitle,
                AuthenticityMessage = textResources.LabelAutenticidadMessage,
                VerificationCodeLine = $"{textResources.LabelVerificationCode}{model.Details?.VerificationCode ?? string.Empty}",
                VerificationUrlLine = $"{textResources.LabelVerificationUrl}{model.Details?.VerificationUrl ?? string.Empty}",
                AuthenticityLogoPath = model.Seller?.LogoPath ?? string.Empty,

                Message = textResources.Message,
                MessageWarning = textResources.MessageWarning,

                FooterPrefix = textResources.LabelFooterText,
                FooterDocumentNumber = documentNumber
            };
        }
    }
}
