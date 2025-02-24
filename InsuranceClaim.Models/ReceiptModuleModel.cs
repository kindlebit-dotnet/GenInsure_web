using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
namespace InsuranceClaim.Models
{
    public class ReceiptModuleModel
    {
   
        public int Id { get; set; }
        public int PolicyId { get; set; }
        public int CustomerId { get; set; }
        public string PolicyNumber { get; set; }
        public string Date { get; set; }
        public string PolicyNo { get; set; }
        public string CustomerName { get; set; }
        public string InvoiceNumber { get; set; }
        public int? PaymentMethodId { get; set; }
        public string Email { get; set; }
        public string paymentMethodType { get; set; }
        public decimal AmountDue {get;set;}
        public decimal? AmountPaid { get; set; }
        public string Balance { get; set; }
        public DateTime DatePosted { get; set; }
        public string TransactionReference { get; set; }
        
        public decimal InvoiceAmount { get; set; }
        public int SummaryDetailId { get; set; }
        public int CreatedBy { get; set; }
        public int VehicleId { get; set; }
        public string RegistrationNumber { get; set; }
        public string RenewPolicyNumber { get; set; }
        public decimal TenderedAmount { get; set; }
        public string ErrorMsg { get; set; }
        public int CurrencyId { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
    }

        public class BrokerRecieptClASS
       {
           public decimal  AmountDue { get; set; }
           public string  Balance { get; set; }
           public string  CustomerName { get; set; }
           public string  PolicyNo { get; set; }
           public string TransactionReference { get; set; }
           public int PaymentMethodId { get; set; }
           public string InvoiceNo { get; set; }
           public int customerId { get; set; }
    }


    public class BrokerRecieptedmodel
    {
        public decimal AmountDue { get; set; }
        public string Balance { get; set; }
        public string CustomerName { get; set; }
        //public string PolicyNo { get; set; }
        public string TransactionReference { get; set; }
        public string paymentMethodType { get; set; }
        //public string InvoiceNo { get; set; }
        public string Date { get; set; }
        public string Email { get; set; }
        public string Address1 { get; set; }


    }

    public class RefundPolicyModel
    {

  
        public int Id { get; set; }
        public int PolicyId { get; set; }
        public int CustomerId { get; set; }
        public int SummaryDetailId { get; set; }
        public string PolicyNumber { get; set; }
        public string CustomerName { get; set; }
      
        [Required]
        public decimal Premium { get; set; }

        public decimal Deduction { get; set; }

        [Required]
        public decimal RefundAmount { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? CreatedBy { get; set; }

        [Required]
        public int VehicleId { get; set; }

        public string RegistrationNumber { get; set; }

        public decimal TotalPremium { get; set; }


        public DateTime CoverStartDate { get; set; }
        
        public DateTime CoverEndDate { get; set; }
        public int PaymentTermId { get; set; }

    }




}
