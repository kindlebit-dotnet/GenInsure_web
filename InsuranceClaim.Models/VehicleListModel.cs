using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace InsuranceClaim.Models
{
    public class VehicleListModel
    {
        public string make { get; set; }
        public string model { get; set; }
        public string covertype { get; set; }
        public string suminsured { get; set; }
        public string premium { get; set; }
        public string radio_license_fee { get; set; }
        public string excess { get; set; }
        public string vehicle_license_fee { get; set; }
        public string stampDuty { get; set; }
        public string total { get; set; }
        public string RegistrationNo { get; set; }
        public string ZTSCLevy { get; set; }
        public int Id { get; set; }
        public string CurrencyName { get; set; }
        public decimal? Discount { get; set; }
    }


    public class VehicleLicOnlyObject
    {
        public string VRN { get; set; }
        public string IDNumber { get; set; }
        public string ClientIDType { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string SuburbID { get; set; }
        public int LicFrequency { get; set; }
    }

    public class LICOnlyQuoteArguments
    {
        public string PartnerReference { get; set; }
        public string Date { get; set; }
        public string Version { get; set; }
        public string PartnerToken { get; set; }
        public LICOnlyQuoteFunctionObject Request { get; set; }
    }

    public class LICOnlyQuoteFunctionObject
    {
        public string Function { get; set; }
        public List<VehicleLicOnlyObject> Vehicles { get; set; }
    }


    public class LICOnlyQuoteRequest
    {
        public LICOnlyQuoteArguments Arguments { get; set; }
        public string MAC { get; set; }
        public string Mode { get; set; }
    }



}
