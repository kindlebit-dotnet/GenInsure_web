using Insurance.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace InsuranceClaim.Models
{
    public class CommissionPeriod
    {
        public DateTime StartPeriod { get; set; }
        public DateTime EndPeriod { get; set; }
    }
    public class Total
    {
        public int TotalNo { get; set; }
    }
    public class TotalAmount
    {
        public double AmountTotal { get; set; }
    }


    public class ClientData
    {
        public int id { get; set; }
        public String AgentName { get; set; }

    }

    public class ModelPeriodCommission
    {

        public class UserCommissionData
        {
            public String CreatedByName { get; set; }
            public int CreatedById { get; set; }

            public int PoliciesTotal { get; set; }
            public int UnpaidPoliciesNo { get; set; }

            public int paidPoliciesNo { get; set; }
            public double PremiumUnpaid { get; set; }
            public double PremiumPaid { get; set; }
            public double CommissionOfPaid { get; set; }
            public double CommissionOfUnpaid { get; set; }

        }
        public List<UserCommissionData> UserCommissions { get; set; }
    }


}
