using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceClaim.Models
{
    public class ProductModel
    {

        public int Id { get; set; }
        [Display(Name = "Product Name")]
        [Required(ErrorMessage = "Please Enter Product Name.")]
        public string ProductName { get; set; }
        [Display(Name = "Product Code")]
        [Required(ErrorMessage = "Please Enter Product Code.")]
        public string ProductCode { get; set; }

        public bool? Active { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public int? ModifiedBy { get; set; }
    }


    public class BrokerProductModel
    {

        public int Id { get; set; }
        [Display(Name = "Policy Class Name")]
        [Required(ErrorMessage = "Please Enter Policy Class Name.")]
        public string PolicyClassName { get; set; }
        public string PolicyClassId { get; set; }
        public string RiskCoverId { get; set; }
        //public string PolicyClassName { get; set; }
        [Display(Name = "Risk Cover Name")]
        [Required(ErrorMessage = "Please Enter Risk Cover Name.")]
        public string RskCoverName { get; set; }

        [Display(Name = "Risk Item Name")]
        [Required(ErrorMessage = "Please Enter Risk Item Name.")]
        public string RiskItemName { get; set; }
        public string Fk_Ins_plcyID { get; set; }
        public int Fk_RskCoverID { get; set; }
      
        //public bool? Active { get; set; }
        //public DateTime? CreatedOn { get; set; }
        //public int? CreatedBy { get; set; }
        //public DateTime? ModifiedOn { get; set; }
        //public int? ModifiedBy { get; set; }
    }
    public class RiskCoverModel1
    {
        
        public int Id { get; set; }
        [Display(Name = "Policy Class Name")]
        [Required(ErrorMessage = "Please Enter Policy Class Name.")]
        public string PolicyClassName { get; set; }
    }

    public class RiskCoverModel2
    {
        public int Id { get; set; }
        [Display(Name = "Risk Cover Name")]
        [Required(ErrorMessage = "Please Enter Risk Cover Name.")]
        public string RskCoverName { get; set; }
        public int Fk_Ins_plcyID { get; set; }
    }

    public class RiskItem1
    {
        public int Id { get; set; }
        [Display(Name = "Risk Item Name")]
        [Required(ErrorMessage = "Please Enter Risk Item Name.")]
        public string RiskItemName { get; set; }
        public int Fk_Ins_plcyID { get; set; }
        public int Fk_RskCoverID { get; set; }
    }
    public class Pclass
    {
        public string Name { get; set; }
        public int Fk_Ins_plcyID { get; set; }
        public int Fk_RskCoverID { get; set; }
    }

    public class ListDetails
    {
        public int Id { get; set; }
        public List<QuotationDetails> quotationmodel { get; set; }
        public string CustomerName { get; set; }
        public string Address { get; set; }
        public string Email { get; set; }
        public string ContactNumber { get; set; }
    }
    public class QuotationDetails
    {
        public int Id { get; set; }

        public string PolicyClass { get; set; }
        public string RiskCover { get; set; }
        public string RiskItem { get; set; }
        public string RiskAddress { get; set; }
        public string SumInsured { get; set; }
        public string Currency { get; set; }
        public string RiskRate { get; set; }
        public string StampDuty { get; set; }
        public string TotalPayable { get; set; }
        public string PaymentTerm { get; set; }

        public QuotationDetails Clone()
        {
            throw new NotImplementedException();
        }
    }

    public class EditGenerateCustomer
    {
        public int Id { get; set; }      
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string BusinessName { get; set; }
        public string BusinessAddress { get; set; }
        public string ContactNumber { get; set; }
        public string EmailAddress { get; set; }
    }

}