using System;
using System.Runtime.Serialization;

namespace Taxomania.ReceiptBank.Model
{
    [DataContract]
    public sealed class ReceiptBankDetailedReceipt
    {
        [DataMember(Name = "id")]
        public long ReceiptId { get; set; }

        [DataMember(Name = "type")]
        public string Type { get; set; }

        [DataMember(Name = "date")]
        public DateTime Date { get; set; }

        [DataMember(Name = "invoice_number")]
        public string InvoiceNumber { get; set; }

        [DataMember(Name = "paid")]
        public bool Paid { get; set; }

        [DataMember(Name = "payee")]
        public string Payee { get; set; }

        [DataMember(Name = "payee_id")]
        public string PayeeId { get; set; }

        [DataMember(Name = "client")]
        public string Client { get; set; }

        [DataMember(Name = "project")]
        public string Project { get; set; }

        [DataMember(Name = "category")]
        public string Category { get; set; }

        [DataMember(Name = "category_id")]
        public string CategoryId { get; set; }

        [DataMember(Name = "category_code")]
        public long? CategoryCode { get; set; }

        [DataMember(Name = "currency")]
        public string Currency { get; set; }

        [DataMember(Name = "total_amount")]
        public decimal? TotalAmount { get; set; }

        [DataMember(Name = "vat_amount")]
        public decimal? VatAmount { get; set; }

        [DataMember(Name = "base_currency")]
        public string BaseCurrency { get; set; }

        [DataMember(Name = "base_total")]
        public decimal? BaseTotal { get; set; }

        [DataMember(Name = "base_vat")]
        public decimal? BaseVat { get; set; }

        [DataMember(Name = "payment_method")]
        public string PaymentMethod { get; set; }

        [DataMember(Name = "payment_method_ref")]
        public long? PaymentMethodReference { get; set; }

        [DataMember(Name = "description")]
        public string Description { get; set; }

        [DataMember(Name = "status")]
        public string Status { get; set; }
    }
}