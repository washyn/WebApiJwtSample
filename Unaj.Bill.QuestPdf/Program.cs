using System;
using System.Collections.Generic;

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

        private static InvoiceModel CreateSampleInvoice()
        {
            var invoice = new InvoiceModel
            {
                Seller =
                    new CompanyModel
                    {
                        Name = "UNIVERSIDAD NACIONAL DE JULIACA",
                        LogoPath = "opera.png",
                        Subtext1 = "Dirección: Av. Nueva Zelandia N° 631, Urb. La Capilla - Juliaca",
                        Subtext2 = "Teléfono: 051-323200", //  - CENTRAL TELEFÓNICA
                        Subtext3 = "Empresa S.A. 8448"
                    },
                Customer = new CustomerModel { Name = "Chester Chester Chester", DocumentNumber = "71449257" },
                Details = new DocumentDetailsModel
                {
                    Ruc = "RUC: 20448375688",
                    DocumentType = "COMPROBANTE DE PAGO",
                    DocumentNumber = "UNAJ-2026-0A54AEC8",
                    IssueDate = DateTime.Now
                },
                PaymentMethod = "Contado",
                HashCode = "hjksdfhjkfdjhksdfjh5",
                HashImagePath = "opera.png",
                AmountInWords = NumberLetter.ConvertToLetter(100, "Soles"),
                Items = new List<InvoiceItemModel>(),
                MessageWarning =
                    "La reproducción no autorizada o falsificación de este documento constituye una infracción penalizada conforme a las normas vigentes.",
                Message =
                    "Documento emitido con fines de constancia de pago por servicios académicos y administrativos. No sustituye a una factura ni boleta de venta, salvo que el concepto así lo requiera. Cualquier alteración o enmendadura invalida este comprobante. En caso de dudas, verifique la validez del documento en el portal oficial de la universidad utilizando el código de verificación correspondiente."

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