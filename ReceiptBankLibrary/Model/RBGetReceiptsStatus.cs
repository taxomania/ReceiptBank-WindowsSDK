using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Taxomania.ReceiptBank.Model
{
    [DataContract]
    internal sealed class RBGetReceiptsStatus
    {
        [DataMember(Name = "receipts")]
        public IEnumerable<RBReceiptId> ReceiptIds { get; set; }
    }
}