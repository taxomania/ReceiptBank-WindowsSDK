using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Taxomania.ReceiptBank.Model
{
    [DataContract]
    public sealed class ReceiptBankError
    {
        [DataMember(Name = "errorcode")]
        public string Code { get; set; }

        [DataMember(Name = "errormessage")]
        public string Message { get; set; }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}