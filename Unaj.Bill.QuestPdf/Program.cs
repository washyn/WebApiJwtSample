using System;
using System.Collections.Generic;

using QRCoder;

using QuestPDF.Companion;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace QuestPDF.Invoice

{
    class Program
    {
        static void Main(string[] args)
        {
            // TODO: Please make sure that you are eligible to use the Community license.
            // To learn more about the QuestPDF licensing, please visit:
            // https://www.questpdf.com/pricing.html
            Settings.License = LicenseType.Community;

            // Generate sample data using the newly separated model classes
            var model = CreateSampleInvoice();

            // Instantiate the dynamic document passing the data
            var document = new InvoiceDocument(model);

            // Generate PDF file on disk
            document.GeneratePdf("comprobante-poc.pdf");

            // Generate PDF file and show it in the default viewer
            document.ShowInCompanion();

            Console.WriteLine("PDF generated and sent to QuestPDF Companion successfully.");
        }

        private static byte[]? GenerateQrCode(string payload)
        {
            try
            {
                using var generator = new QRCodeGenerator();
                using var data = generator.CreateQrCode(payload, QRCodeGenerator.ECCLevel.Q);
                var qrCode = new PngByteQRCode(data);
                return qrCode.GetGraphic(8);
            }
            catch
            {
                return null;
            }
        }

        private static InvoiceModel CreateSampleInvoice()
        {
            var invoice = new InvoiceModel
            {
                Seller =
                    new CompanyModel
                    {
                        Name = "UNIVERSIDAD NACIONAL DE JULIACA",
                        // LogoPath = "opera.png",
                        LogoPath = "logo.png",
                        Subtext1 = "Av. Nueva Zelandia N° 631, Urb. La Capilla - Juliaca",
                        Subtext2 = "Teléfono: 051-323200", //  - CENTRAL TELEFÓNICA
                        Subtext3 = "Empresa S.A. 8448"
                    },
                Customer =
                    new CustomerModel
                    {
                        Name = "Chester Chester Chester", DocumentNumber = "71449257", Phone = "997 *** 563"
                    },
                Details = new DocumentDetailsModel
                {
                    Ruc = "RUC: 20448375688",
                    DocumentType = "COMPROBANTE DE PAGO",
                    DocumentNumber = "UNAJ-2026-0A54AEC8",
                    IssueDate = DateTime.Now,
                    TransactionId = "235614",
                    RequestId = "0a54aec8-d872-8e8e-48f4-3a212ed15e23",
                    VerificationCode = "EA828F8E361C",
                    VerificationUrl =
                        "https://pagos.unaj.edu.pe/verificar?r=0a54aec8-d872-8e8e-48f4-3a212ed15e23&c=EA828F8E361C"
                },
                PaymentMethod = "Billetera Digital Bipay",
                HashImagePath = "opera.png",
                Items = new List<InvoiceItemModel>(),
                QrCode = GenerateQrCode("0a54aec8-d872-8e8e-48f4-3a212ed15e23"),
                // Emido electrónicamente por UNAJ — UNAJ-2026-0A54AEC8
            };

            // Generate the 10 sample items from the original code
            for (var i = 1; i <= 5; i++)
            {
                invoice.Items.Add(new InvoiceItemModel
                {
                    Index = i, Description = $"Producto de ejemplo {i}", UnitPrice = i, Quantity = i
                });
            }

            return invoice;
        }
    }
}
