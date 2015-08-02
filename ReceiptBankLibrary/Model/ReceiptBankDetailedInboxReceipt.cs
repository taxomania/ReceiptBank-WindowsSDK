using System.Runtime.Serialization;

namespace Taxomania.ReceiptBank.Model
{
    [DataContract]
    public class ReceiptBankDetailedInboxReceipt : ReceiptBankDetailedReceipt
    {
        [DataMember(Name = "integration_status")]
        public string IntegrationStatus { get; set; }

        [DataMember(Name = "user_status")]
        public string UserStatus { get; set; }

        [DataMember(Name = "integration_state")]
        public string IntegrationSte { get; set; }

        [DataMember(Name = "integration_state_message")]
        public string IntegrationStateMessage { get; set; }
    }
}