﻿using System.Runtime.Serialization;

namespace Taxomania.ReceiptBank.Model
{
    [DataContract]
    public sealed class ReceiptBankReceipt
    {
        [DataMember(Name = "receiptid")]
        public long ReceiptId { get; set; }

        [DataMember(Name = "status")]
        public string Status { get; set; }
    }
}