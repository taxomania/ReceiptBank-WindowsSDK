using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Taxomania.ReceiptBank.Model
{
    [DataContract]
    internal sealed class ReceiptBankGetReceipts
    {
        [DataMember(Name = "receipts")]
        public IEnumerable<long> ReceiptIds { get; set; }
    }
}