using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;


namespace InsuranceClaim.Models
{
    public class BranchModel
    {
        public int Id { get; set; }
        [Display(Name = "Provider Type")]
        [Required(ErrorMessage = "Please Enter BranchName")]
        public string BranchName { get; set; }

        public string AlmId { get; set; }

        public string Location_Id { get; set; }
        public bool Status { get; set; }
        public string Partners { get; set; }
        public int PartnersId { get; set; }

        public bool StatusActive { get; set; }
        public bool StatusDeActive { get; set; }

   
    }
}
