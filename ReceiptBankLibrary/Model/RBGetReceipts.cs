using System.Runtime.Serialization;

namespace Taxomania.ReceiptBank.Model
{
    [DataContract]
    internal sealed class RBGetReceipts
    {
        [DataMember(Name = "id")]
        public long? ReceiptId { get; set; }

        [DataMember(Name = "new")]
        public bool? New { get; set; }
    }
}