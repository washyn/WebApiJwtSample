using System;
using System.Collections.Generic;

using QuestPDF.Companion;
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
                        Subtext1 = "Av. Circunvalación s/n, Urb. Ciudad Universitaria, Juliaca — Puno",
                        Subtext2 = "Teléfono: (051) 320012",
                        Subtext3 = "Empresa S.A. 8448"
                    },
                Customer = new CustomerModel { Name = "Chester Chester Chester", DocumentNumber = "71449257" },
                Details = new DocumentDetailsModel
                {
                    Ruc = "00000000",
                    DocumentType = "COMPROBANTE DE PAGO",
                    DocumentNumber = "B00-345788475",
                    IssueDate = DateTime.Now
                },
                PaymentMethod = "Contado",
                HashCode = "hjksdfhjkfdjhksdfjh5",
                HashImagePath = "opera.png",
                AmountInWords = "Mucho dineroo",
                Items = new List<InvoiceItemModel>()
            };

            // Generate the 10 sample items from the original code
            for (var i = 1; i <= 10; i++)
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