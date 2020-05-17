using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Web;

namespace iHealth2.CustomClasses
{
    [DataContract]
    public class PaymentResponse
    {
        [DataMember]
        public int Amount { get; set; }
        [DataMember]
        public string CardNumber { get; set; }
        [DataMember]
        public string MerchantReference { get; set; }
        [DataMember]
        public string PaymentReference { get; set; }
        [DataMember]
        public string RetrievalReferenceNumber { get; set; }
        [DataMember]
        public List<object> SplitAccounts { get; set; }
        [DataMember]
        public string TransactionDate { get; set; }
        [DataMember]
        public string ResponseCode { get; set; }
        [DataMember]
        public string ResponseDescription { get; set; }
    }
    
}