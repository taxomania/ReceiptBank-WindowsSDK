using System;
using Windows.Web.Http;

namespace Taxomania.ReceiptBank.Model
{
    public sealed class ReceiptBankException : Exception
    {
        public ReceiptBankError Error { get; set; }
        public HttpStatusCode StatusCode { get; set; }
    }
}