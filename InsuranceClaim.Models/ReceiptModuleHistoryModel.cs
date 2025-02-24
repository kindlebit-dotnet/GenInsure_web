using Insurance.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceClaim.Models
{
   public class ReceiptModuleHistoryModel
    {
        public int Id { get; set; }
        public int PolicyId { get; set; }
        public string PolicyNumber { get; set; }
        public string CustomerName { get; set; }
        public string InvoiceNumber { get; set; }
        public int? PaymentMethodId { get; set; }
        public decimal? AmountDue { get; set; }

        public decimal? AmountPaid { get; set; }
        public string Balance { get; set; }
        public DateTime DatePosted { get; set; }
    }


    public  class ReceiptDeliveryModule
    {
        public string customerFirstName { get; set; }
        public string customerLastName { get; set; }
        public string addressLine1 { get; set; }
        public string addressLine2 { get; set; }
        public string city { get; set; }
        public string phoneNumber { get; set; }
        public string policyID { get; set; }
        public string policyTransactionDate { get; set; }
        public decimal policyAmount { get; set; }
        public string agentID { get; set; }
        public string agentName { get; set; }
        public string zoneName { get; set; }

        public string edd { get; set; } // receipt date
         
    }



    public class ModelReceiptAndPayment
    {

        public int Id { get; set; } // Auto increment
        public int policyId { get; set; } // policy reference
        public string reference { get; set; }
        public string policyNumber { get; set; } // policy reference check as invoice
        public string type { get; set; } // reciept or invoice

        public string paymentMethod { get; set; }
        public string Description { get; set; } // Description
        public decimal Amount { get; set; } // Amount - or +
        public string currency { get; set; } // currency options
        public string CreatedOn { get; set; }
        public int CreatedBy { get; set; }
    }


    public class ReceiptCancelationModel
    {
        public List<ReceiptAndPayment> receiptAndPayments { get; set; }
        public int CustomerId { get; set; }
    }
}
