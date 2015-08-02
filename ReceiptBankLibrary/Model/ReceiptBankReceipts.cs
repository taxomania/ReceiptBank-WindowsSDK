using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Taxomania.ReceiptBank.Model
{
    [DataContract]
    public sealed class ReceiptBankReceipts<T> where T : ReceiptBankDetailedReceipt
    {
        [DataMember(Name = "size")]
        public int Size { get; set; }

        [DataMember(Name = "receipts")]
        public IEnumerable<T> Receipts { get; set; }
    }
}