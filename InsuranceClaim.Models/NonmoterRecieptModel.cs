﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceClaim.Models
{
   public class NonmoterRecieptModel
    {
        public int Id { get; set; }
        public int PolicyId { get; set; }
        public int CustomerId { get; set; }
        public string PolicyNumber { get; set; }

        public string PolicyNo { get; set; }
        public string CustomerName { get; set; }
        public string InvoiceNumber { get; set; }
        [Required]
        public int? PaymentMethodId { get; set; }
        [Required]
        public decimal? AmountDue { get; set; }
        public decimal? AmountPaid { get; set; }
        [Required]
        public string Balance { get; set; }
        public DateTime DatePosted { get; set; }
        [Required]
        public string TransactionReference { get; set; }
        public int CreatedBy { get; set; }
        public int ReceiptNo { get; set; }
        public decimal TenderedAmount { get; set; }
        public string ErrorMsg { get; set; }
        public string Currency { get; set; }
        public decimal InvoiceAmount { get; set; }
    }
}

