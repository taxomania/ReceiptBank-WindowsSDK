using System.Runtime.Serialization;

namespace Taxomania.ReceiptBank.Model
{
    [DataContract]
    internal sealed class RBReceiptId
    {
        [DataMember(Name = "receiptid")]
        public long ReceiptId { get; set; }
    }
}