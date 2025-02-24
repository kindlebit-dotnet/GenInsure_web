using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;


namespace InsuranceClaim.Models
{
    public class BrokerCustomersModel
    {
        public int Id { get; set; }
        public int UserID { get; set; }

        [Display(Name = "First Name")]
        [Required(ErrorMessage = "Please Enter First Name.")]
        [MaxLength(30, ErrorMessage = "First name must be less than 30 characters long.")]
        public string FirstName { get; set; }

        [Display(Name = "Surname")]
        [Required(ErrorMessage = "Please Enter Last Name.")]
        [MaxLength(30, ErrorMessage = "Last name must be less than 30 characters long.")]
        public string SurName { get; set; }

        // [RegularExpression(@"^([0-9]{2}-[0-9]{6,7}[a-zA-Z]{1}[0-9]{2})$", ErrorMessage = "Not a Valid Identification Number")]
        [Required(ErrorMessage = "Please Enter National Identification Number.")]
        public string NationalIdentificationNumber { get; set; }

        [Display(Name = "Date Of Birth")]
        [Required(ErrorMessage = "Please Enter Date Of Birth.")]
        public string DateOfBirth { get; set; }
        [Required(ErrorMessage = "Please Select Gender.")]
        public string Gender { get; set; }
        public DateTime? CreatedOn { get; set; }

        [Display(Name = "Business Name")]
        [Required(ErrorMessage = "Please Enter Business Name.")]
        public string BusinessName { get; set; }

        [Display(Name = "Business Partner Number")]
        [Required(ErrorMessage = "Please Enter Business Partner Number.")]
        public string BusinessPartnerNumber { get; set; }

        [Display(Name = "Business Address")]
        [Required(ErrorMessage = "Please Enter Business Address.")]
        public string BusinessAddress { get; set; }

        [Display(Name = "Business Phone Number")]
        [Required(ErrorMessage = "Please Enter Business Phone Number.")]
       // [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid phone number")]
        public string BusinessPhoneNumber { get; set; }

        [Display(Name = "Contact Person Name")]
        [Required(ErrorMessage = "Please Enter Contact Person Name.")]
        public string ContactPersonName { get; set; }

        [Display(Name = "Contact Person Phone Number")]
        [Required(ErrorMessage = "Please Enter Contact Person Phone Number.")]
        //[RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid phone number")]
        public string ContactPersonPhoneNumber { get; set; }

        [Display(Name = "Contact Person Email")]
        [Required(ErrorMessage = "Please Enter Contact Person Email.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string ContactPersonEmail { get; set; }

        [Display(Name = "Email Address")]
        [Required(ErrorMessage = "Please Enter Email Address.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        public string Email { get; set; }

        [Display(Name = "Contact Number")]
        [Required(ErrorMessage = "Please Enter Contact Number.")]
       // [RegularExpression(@"^\(?([0-9]{3})\)?[-. ]?([0-9]{3})[-. ]?([0-9]{4})$", ErrorMessage = "Not a valid phone number")]
        public string ContactNumber { get; set; }

        [Display(Name = "Physical Address")]
        [Required(ErrorMessage = "Physical Address.")]
        public string PhysicalAddress { get; set; }

        [Display(Name = "Customer Type ")]
        [Required(ErrorMessage = "Please Select Customer Type.")]
        public string CustomerType { get; set; }

        public int CustomerId { get; set; }

        [Required(ErrorMessage = "Please Select Insorance Policy Class.")]
        public int InsorancePolicyId { get; set; }

        [Required(ErrorMessage = "Please Select Risk Cover.")]
        public int RiskcoverId { get; set; }

        [Required(ErrorMessage = "Please Select Risk Risk Item.")]
        public int RiskItemId { get; set; }

        [Required(ErrorMessage = "Please Enter Risk Description .")]
        public string RiskDescription { get; set; }

        [Required(ErrorMessage = "Please EnterRiskAddress.")]
        public string RiskAddress { get; set; }

        [Required(ErrorMessage = "Please Select PolicyValidityPeriod From .")]
        public string PolicyValidityPeriodFrom { get; set; }

        [Required(ErrorMessage = "Please Select PolicyValidityPeriod To .")]
        public string PolicyValidityPeriodTo { get; set; }


        public string Riskinsured { get; set; }
        [RegularExpression("^[0-9]*$", ErrorMessage = "SumInsured must be numeric")]
        public string SumInsured { get; set; }
        public string RiskRate { get; set; }
        [Display(Name = "Payment Term")]
        [Required(ErrorMessage = "Please Select Payment Term.")]
        public string PaymentTerm { get; set; }
        public string StampDuty { get; set; }
        public string TotalPremiumPayable { get; set; }
        public string Currency { get; set; }
        public string PolicyClassName { get; set; }
        public string RiskCoverName { get; set; }
        public string RiskItemName { get; set; }
        public int ExistingCustomerId { get; set; }
        public int CUSTOMERIDHdn { get; set; }
        
        public int QuotationCustomerId { get; set; }
    }

    public class CustDetailList
    {
        public List<BrokerCustomersModel> custmodel { get; set; }
    }
    public class listproduct
    {
        public List<BrokerCustomersItem> productmodel { get; set; }

    }

    public class BrokerCustomersItem
    {
        public string PolicyClassName { get; set; }
        public int Id { get; set; }
        public int customerid { get; set; }
        public string RiskItemName { get; set; }
        public string CustomersName { get; set; }
        public string RiskcoversName { get; set; }
        public string RiskAddress { get; set; }


    }
    public class PolicyProducts
    {
        public int UserId { get; set; }
        public List<PolicyProductList> PolicyProductList { get; set; }

    }
    public class PolicyProductList
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int PolicyClassName { get; set; }
        public int RskCoverName { get; set; }
        public int RiskItems { get; set; }
        public string DescriptionofRiskInsured { get; set; }
        public string RiskAddress { get; set; }
        public string PolicyValidityPeriodFrom { get; set; }
        public string PolicyValidityPeriodTo { get; set; }
        public string SumInsured { get; set; }

        [Required(ErrorMessage = "Please select a currency")]
        public string Currency { get; set; }
        public string Email { get; set; }
        [Required(ErrorMessage = "Please enter a Riskrate")]
        public string RiskRate { get; set; }
        [Required(ErrorMessage = "Please select a paymentterm")]
        public string PaymentTerm { get; set; }
        public string StampDuty { get; set; }
        public string TotalPremiumPayable { get; set; }
        public int SequenceNumber { get; set; }

    }

    public class listquotation
    {
        public List<QuotationItem> customerdetails { get; set; }
        public string BusinessPhoneNumber { get; set; }
        public DateTime FormDate { get; set; }
        public DateTime EndDate { get; set; }
        public string FirstName { get; set; }
         public string AmountDue { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string BusinessName { get; set; }
        public string BusinessPartnerNumber { get; set; }
        public string BusinessAddress { get; set; }
        public string ContactPersonName { get; set; }
        public string ContactPersonPhoneNumber { get; set; }
        public string ContactPersonEmail { get; set; }
        public string Currency { get; set; }
        public string SurName { get; set; }
        public string CustomerName { get; set; }
        public string ContactNumber { get; set; }
        public string PhysicalAddress { get; set; }
        public string MainAddress { get; set; }
        public int QuotationId { get; set; }
        public double Annualtotalpayble { get; set; }
        public double Annualstampvalue { get; set; }
        public double Quarterlystampvalue { get; set; }
        public string TotalPremium { get; set; }
        public double Quarterlytotalpayble { get; set; }
        public double Termlystampvalue { get; set; }
        public double Termlytotalpayble { get; set; }
        public double Monthlystampvalue { get; set; }
        public double Monthlytotalpayble { get; set; }
        public DateTime CreatedOn { get; set; }
        public string LoggedInUser { get; set; }
        public int Id { get; set; }
        public string PolicyClassName { get; set; }
        public string RiskCoverName { get; set; }
        public string RiskItemName { get; set; }
        public string RiskAddress { get; set; }
        public string PolicyValidityPeriodFrom { get; set; }
        public string PolicyValidityPeriodTo { get; set; }
        public string PaymentTerm { get; set; }
        public string SumInsured { get; set; }
        public double TotalSumInsured { get; set; }
        public string TotalPremiumPayable { get; set; }
        public string StampDuty { get; set; }
        public double BasicPremium { get; set; }
        public string PolicyClassId { get; set; }
        public string RiskCoverId { get; set; }
        public string RiskItemId { get; set; }
        public decimal TotalIsured { get; set; }
        public string RiskRate { get; set; }
        public double AnnualPremium { get; set; }
        public double TerminalPremium { get; set; }
        public double QuarterlyPremium { get; set; }
        public string PolicyNumber { get; set; }
        public string InvoiceNumber { get; set; }
        public string EmailAddess { get; set; }
        public bool isEdit { get; set; }
        public int CustomerId { get; set; }
    }

    public class QuotationItem
    {
        public int Id { get; set; }
        public string PolicyClassName { get; set; }
        public DateTime FormDate { get; set; }
        public DateTime EndDate { get; set; }
        public string RiskCoverName { get; set; }
        public string RiskItemName { get; set; }
        public string RiskAddress { get; set; }
        public string PolicyValidityPeriodFrom { get; set; }
        public string PolicyValidityPeriodTo { get; set; }
        public string Currency { get; set; }
        public string PaymentTerm { get; set; }
        public string SumInsured { get; set; }
        public double TotalSumInsured { get; set; }
        public double Total { get; set; }
        public string TotalPremiumPayable { get; set; }
        public string StampDuty { get; set; }
        public string PolicyNumber { get; set; }
        public decimal BasicPremium { get; set; }
        public string PolicyClassId { get; set; }
        public string RiskCoverId { get; set; }
        public string RiskItemId { get; set; }
        public string NationalIdNumber { get; set; }
        public string Gender { get; set; }
        public string DateOfBirth { get; set; }
        public string RiskRate { get; set; }
        public decimal AnnualPremium { get; set; }
        public decimal TerminalPremium { get; set; }
        public decimal QuarterlyPremium { get; set; }
        public decimal MonthlyPremium { get; set; }
        public string BusinessPhoneNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string ContactNumber { get; set; }
        public string BusinessName { get; set; }
        public string ContactPersonEmail { get; set; }
        public string BusinessAddress { get; set; }
        public string ContactpersonName { get; set; }
        public DateTime CreatedOn { get; set; }
        public string RiskDescription { get; set; }
        public string InvoiceNumber { get; set; }
        public string UserId { get; set; }

        public int? PaymentMethod { get; set; }
        public decimal ReceiptAmount { get; set; }
        public string PaymentMethodName { get; set; }
        public string TransactionReference { get; set; }
        public decimal TenderedAmount { get; set; }
        public string ReceiptedBy { get; set; }
        public decimal Balance { get; set; } 
        public int ReceiptNumber { get; set; }
        public decimal Amountdue { get; set; }

       public bool IsActive { get; set; }
    }

    public class QuotationCustomerDetails
    {
        
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string ContactNumber { get; set; }

    }
    public class QuotationDetail
    {
        public int id { get; set; }
        public int customerId { get; set; }
        public string PolicyClass { get; set; }
        public string RiskCoverClass { get; set; }
        public string RiskItemClass { get; set; }
        public string RiskAddress { get; set; }
        public string SumInsured { get; set; }
        public string RiskRate { get; set; }
        public string PaymentTerm { get; set; }
        public string StampDuty { get; set; }
        public string TotalPayable { get; set; }

    }

    public class InvoiceList
    {
      public int Id { get; set; }
      public string InvoiceNumber { get; set; }
      public string CustomerName { get; set; }
      public string Email { get; set; }
    }
}
