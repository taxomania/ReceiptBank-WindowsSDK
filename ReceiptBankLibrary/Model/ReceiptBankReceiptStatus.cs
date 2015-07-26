using System.Runtime.Serialization;

namespace Taxomania.ReceiptBank.Model
{
    [DataContract]
    public sealed class ReceiptBankReceiptStatus
    {
        [DataMember(Name = "id")]
        public long ReceiptId { get; set; }

        [DataMember(Name = "status")]
        public string Status { get; set; }

        [DataMember(Name = "processed")]
        public bool Processed { get; set; }
    }
}