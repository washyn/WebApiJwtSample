using System;
using System.Collections.Generic;
using System.Linq;

namespace QuestPDF.Invoice
{
    public class InvoiceModel
    {
        public CompanyModel Seller { get; set; }
        public CustomerModel Customer { get; set; }
        public DocumentDetailsModel Details { get; set; }
        public List<InvoiceItemModel> Items { get; set; } = new List<InvoiceItemModel>();
        public string PaymentMethod { get; set; }
        public string HashCode { get; set; }
        public string HashImagePath { get; set; }
        public string AmountInWords { get; set; }
        public string Message { get; set; }
        public string MessageWarning { get; set; }
        public decimal TotalAmount => Items.Sum(x => x.Total);
    }

    public class CompanyModel
    {
        public string Name { get; set; }
        public string LogoPath { get; set; }
        public string Subtext1 { get; set; }
        public string Subtext2 { get; set; }
        public string Subtext3 { get; set; }
    }

    public class CustomerModel
    {
        public string Name { get; set; }
        public string DocumentNumber { get; set; }
    }

    public class DocumentDetailsModel
    {
        public string Ruc { get; set; }
        public string DocumentType { get; set; }
        public string DocumentNumber { get; set; }
        public DateTime IssueDate { get; set; }
    }

    public class InvoiceItemModel
    {
        public int Index { get; set; }
        public string Description { get; set; }
        public decimal UnitPrice { get; set; }
        public double Quantity { get; set; }
        public decimal Total => (decimal)Quantity * UnitPrice;
    }
}
