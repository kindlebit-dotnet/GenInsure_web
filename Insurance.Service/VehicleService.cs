using Insurance.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using InsuranceClaim.Models;
using RestSharp;
using Newtonsoft.Json;

namespace Insurance.Service
{
    public class VehicleService
    {
        public List<VehicleMake> GetMakers()
        {
            var list = InsuranceContext.VehicleMakes.All().ToList();
            return list;
        }

        public List<ClsVehicleModel> GetModel(string makeCode)
        {
            var list = InsuranceContext.VehicleModels.All(where: $"MakeCode='{makeCode}'").ToList();
            
            var map = Mapper.Map<List<VehicleModel>, List<ClsVehicleModel>>(list);
            return map;

        }

        public async Task SaveDeliveryAddress(ReceiptDeliveryModule deliveryDetail)
        {
            //HttpClient client = new HttpClient();

            //var values = new Dictionary<string, string>
            //    {
            //        { "customerFirstName", deliveryDetail.customerFirstName},
            //        { "customerLastName", deliveryDetail.customerLastName },
            //        { "addressLine1", deliveryDetail.addressLine1 },
            //        { "addressLine2", deliveryDetail.addressLine2 },
            //        { "city", deliveryDetail.city },
            //        { "phoneNumber", deliveryDetail.phoneNumber },
            //        { "policyID", deliveryDetail.policyID },
            //        { "policyTransactionDate", deliveryDetail.policyTransactionDate },
            //         { "policyAmount", deliveryDetail.policyAmount },
            //         { "agentID", deliveryDetail.agentID },
            //         { "agentName", deliveryDetail.agentName },
            //         { "zoneName", deliveryDetail.zoneName }
            //    };

            //var content = new FormUrlEncodedContent(values);

            //var response = await client.PostAsync("http://41.190.32.215:5001/api/deliveries/", content);

            //var responseString = await response.Content.ReadAsStringAsync();



            var client = new RestClient("http://api-deliver.gene.co.zw/api/deliveries/");
            var request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/json");
            request.RequestFormat = DataFormat.Json;
            request.AddJsonBody(deliveryDetail);

            //request.Timeout = 5000;
            //request.ReadWriteTimeout = 5000;
            IRestResponse response = client.Execute(request);

            var re = response;

            // var res = JsonConvert.DeserializeObject<SummaryDetailModel>(response.Content);
        }


        public List<AreasModel> GetAreaList()
        {
           
            // var client = new RestClient("http://41.190.32.215:4002/api/zoneareas/all");
            var client = new RestClient("http://41.190.32.215:5001/api/zoneareas/all");
            var request = new RestRequest(Method.GET);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("content-type", "application/json");
            request.RequestFormat = DataFormat.Json;
           // request.AddJsonBody(deliveryDetail);

            //request.Timeout = 5000;
            //request.ReadWriteTimeout = 5000;
            IRestResponse response = client.Execute(request);

             var res = JsonConvert.DeserializeObject <List<AreasModel>>(response.Content);

            return res;
        }


        public Product GetVehicleTypeByProductId(int productId)
        {
            return InsuranceContext.Products.Single(productId);
        }

        //VehicleTaxClassModel


        public List<VehicleTaxClassModel> GetVehicleTax(string VehicleType)
        {

            var product = InsuranceContext.Products.Single(where: $"Id='{VehicleType}'");

            int vehicleTypeId = 0;

            if(product!=null)
            {
                vehicleTypeId = product.VehicleTypeId;
            }

            var list = InsuranceContext.VehicleTaxClasses.All(where: $"VehicleType='{vehicleTypeId}'").ToList();

         //  var list = InsuranceContext.VehicleTaxClasses.All(where: $"VehicleUsageId='{VehicleType}'").ToList();


            var map = Mapper.Map<List<VehicleTaxClass>, List<VehicleTaxClassModel>>(list);
            return map;

        }


        public List<CoverType> GetCoverType()
        {
            var list = InsuranceContext.CoverTypes.All(where: $"IsActive=1").ToList();
            return list;
        }
        public List<AgentCommission> GetAgentCommission()
        {
            var list = InsuranceContext.AgentCommissions.All().ToList();
            return list;
        }
        public List<VehicleUsage> GetVehicleUsage(string PolicyName)
        {
            var list = InsuranceContext.VehicleUsages.All(where: $"ProductId='{PolicyName}'").ToList();
            return list;
        }
        public List<VehicleUsage> GetAllVehicleUsage()
        {
            var list = InsuranceContext.VehicleUsages.All().ToList();
            return list;
        }

        public PolicyDetail GetPolicy(int policyId)
        {
            var policy = InsuranceContext.PolicyDetails.Single(policyId);
            return policy;
        }
        public VehicleDetail GetVehicles(int policyId)
        {
            var Vehicleinfo = InsuranceContext.VehicleDetails.Single(policyId);
            return Vehicleinfo;
        }

        public List<Product> GetAllProducts()
        {
            return InsuranceContext.Products.All().ToList();
        }

        public List<Domestic_Product> GetDemosticProducts()
        {
            return InsuranceContext.Domestic_Products.All(where: "ProductName = 'Domestic All In One'").ToList();
        }

        public List<RiskCoverModel> Domestic_RiskCovers(int ProductId)
        {
            return InsuranceContext.Domestic_RiskCovers.All(where: $"ProductId='{ProductId}'").ToList().Select(x => new RiskCoverModel { Id = x.Id, RiskCover = x.CoverName }).ToList();     
        }

        public Domestic_RiskItem Domestic_RiskItem(int riskId)
        {
            return InsuranceContext.Domestic_RiskItems.Single(riskId);
        }

      
        public List<VehicleTaxClass> GetAllTaxClasses()
        {
            return InsuranceContext.VehicleTaxClasses.All().ToList();
        }

        public List<PaymentTerm> GetAllPaymentTerms()
        {
            return InsuranceContext.PaymentTerms.All(where: "IsActive = 'True' or IsActive is null").ToList();
        }

        public List<Currency> GetAllCurrency()
        {
            return InsuranceContext.Currencies.All(where: $"IsActive = 'True'").ToList();
        }

        public List<VehicleUsage> GetVehicleUsageByRiskId(string RiskCoverId)
        {
            var list = InsuranceContext.VehicleUsages.All(where: $"RiskCoverId='{RiskCoverId}'").ToList();
            return list;
        }


        public CustomerModel GetAgentDetails(SummaryDetail summaryDetail, string agentEmail)
        {
            var customerDetial = InsuranceContext.Customers.Single(summaryDetail.CreatedBy);
            CustomerModel model = new CustomerModel();

            if (customerDetial != null)
            {
                model = new CustomerModel { FirstName = customerDetial.FirstName, LastName = customerDetial.LastName, PhoneNumber = customerDetial.PhoneNumber, NationalIdentificationNumber = customerDetial.NationalIdentificationNumber, EmailAddress = agentEmail, AddressLine1 = customerDetial.AddressLine1 + ' ' + customerDetial.City };
            }

            return model;
        }


        public List<Domestic_RiskItem> GetRiskCoverItem(string RiskCoverId)
        {
            return InsuranceContext.Domestic_RiskItems.All(where: $"CoverId='{RiskCoverId}'").ToList();
        }


        public string ValidationMessage(RiskDetailModel model)
        {
            string msg = "";

            if (model.IncludeLicenseFee == true && (model.ZinaraLicensePaymentTermId == 0 || model.ZinaraLicensePaymentTermId==null))
            {
                msg = "Please select payment term of vehicle license.";
            }

            if (model.IncludeRadioLicenseCost == true && (model.RadioLicensePaymentTermId == 0 || model.RadioLicensePaymentTermId==null))
            {
                msg = "Please select payment term of radio license.";
            }




            if (model.IncludeLicenseFee)
            {
                if (model.ZinaraLicensePaymentTermId > 0)
                {
                    if (!IsPaymentTermValidForInsuranceLicense(model.PaymentTermId, Convert.ToInt32(model.ZinaraLicensePaymentTermId)))
                    {
                        msg = "Licence payment term should be equal or less than Insurance payment term.";
                    }
                }
            }




            if (model.IncludeRadioLicenseCost)
            {
                if (model.RadioLicensePaymentTermId > 0)
                {
                    if (!IsPaymentTermValidForInsuranceLicense(model.PaymentTermId, Convert.ToInt32(model.RadioLicensePaymentTermId)))
                    {
                        msg = "Licence payment term should be equal or less than Insurance payment term.";

                    }
                }
            }

            return msg;

        }


        public string EndorsmentValidationMessage(EndorsementRiskDetailModel model)
        {
            string msg = "";

            if (model.IncludeLicenseFee == true && (model.ZinaraLicensePaymentTermId == 0 || model.ZinaraLicensePaymentTermId == null))
            {
                msg = "Please select payment term of vehicle license.";
            }

            if (model.IncludeRadioLicenseCost == true && (model.RadioLicensePaymentTermId == 0 || model.RadioLicensePaymentTermId == null))
            {
                msg = "Please select payment term of radio license.";
            }




            if (model.IncludeLicenseFee)
            {
                if (model.ZinaraLicensePaymentTermId > 0)
                {
                    if (!IsPaymentTermValidForInsuranceLicense(model.PaymentTermId, Convert.ToInt32(model.ZinaraLicensePaymentTermId)))
                    {
                        msg = "Licence payment term should be equal or less than Insurance payment term.";
                    }
                }
            }




            if (model.IncludeRadioLicenseCost)
            {
                if (model.RadioLicensePaymentTermId > 0)
                {
                    if (!IsPaymentTermValidForInsuranceLicense(model.PaymentTermId, Convert.ToInt32(model.RadioLicensePaymentTermId)))
                    {
                        msg = "Licence payment term should be equal or less than Insurance payment term.";

                    }
                }
            }

            return msg;

        }




        public bool IsPaymentTermValidForInsuranceLicense(int insurancePaymentTerm, int licesnePaymentTerm)
        {
            bool result = true;

            if (insurancePaymentTerm == 1)
                insurancePaymentTerm = 12;

            if (licesnePaymentTerm == 1)
                licesnePaymentTerm = 12;

            if (insurancePaymentTerm != licesnePaymentTerm)
            {
                if (licesnePaymentTerm > insurancePaymentTerm)
                {
                    result = false;
                }
            }
            return result;
        }

        public  int GetMonthKey(int monthId)
        {
            int licFreequency = 0;
            switch (monthId)
            {
                case 1: // represent to 12 month
                    licFreequency = 3;
                    break;
                case 2:
                    Console.WriteLine("Case 2");
                    break;
                case 3:
                    Console.WriteLine("Case 1");
                    break;
                case 4:
                    licFreequency = 1;
                    break;
                case 5:
                    licFreequency = 4;
                    break;
                case 6:
                    licFreequency = 2;
                    break;
                case 7:
                    licFreequency = 5;
                    break;
                case 8:
                    licFreequency = 6;
                    break;
                case 9:
                    licFreequency = 7;
                    break;
                case 10:
                    licFreequency = 8;
                    break;
                case 11:
                    licFreequency = 9;
                    break;
                default:
                    licFreequency = 3;
                    break;
            }

            return licFreequency;
        }


        public void SaveAccountPolicy(AccountPolicyModel model)
        {

            try
            {

                AccountPolicy accPolicyPremium = new AccountPolicy();
                accPolicyPremium.CreatedAt = DateTime.Now;
                accPolicyPremium.RecieptAndPaymentId = model.RecieptAndPaymentId;
                accPolicyPremium.PolicyId = model.PolicyId;
                accPolicyPremium.PolicyNumber = model.PolicyNumber;
                accPolicyPremium.AccountType = (int)PolicyAccountType.Premium;
                accPolicyPremium.Amount = model.Premium==null?  0: Math.Round(model.Premium.Value, 2) ;
                accPolicyPremium.AccountName = PolicyAccountType.Premium.ToString();
                accPolicyPremium.Status = model.Status;
                InsuranceContext.AccountPolices.Insert(accPolicyPremium);


                AccountPolicy accPolicyStamp = new AccountPolicy();
                accPolicyStamp.CreatedAt = DateTime.Now;
                accPolicyStamp.RecieptAndPaymentId = model.RecieptAndPaymentId;
                accPolicyStamp.PolicyId = model.PolicyId;
                accPolicyStamp.PolicyNumber = model.PolicyNumber;
                accPolicyStamp.AccountType = (int)PolicyAccountType.StampDuty;
                accPolicyStamp.Amount = model.StampDuty==null? 0 : Math.Round(model.StampDuty.Value,2);
                accPolicyStamp.AccountName = PolicyAccountType.StampDuty.ToString();
                accPolicyStamp.Status = model.Status;
                InsuranceContext.AccountPolices.Insert(accPolicyStamp);


                AccountPolicy accPolicyZtsc = new AccountPolicy();
                accPolicyZtsc.CreatedAt = DateTime.Now;
                accPolicyZtsc.RecieptAndPaymentId = model.RecieptAndPaymentId;
                accPolicyZtsc.PolicyId = model.PolicyId;
                accPolicyZtsc.PolicyNumber = model.PolicyNumber;
                accPolicyZtsc.AccountType = (int)PolicyAccountType.ZtscLevy;
                accPolicyZtsc.Amount = model.ZtscLevy==null? 0: Math.Round(model.ZtscLevy.Value);
                accPolicyZtsc.AccountName = PolicyAccountType.ZtscLevy.ToString();
                accPolicyZtsc.Status = model.Status;
                InsuranceContext.AccountPolices.Insert(accPolicyZtsc);

                if (model.RadioLicenseCost > 0)
                {
                    AccountPolicy accPolicyRadioLic = new AccountPolicy();
                    accPolicyRadioLic.CreatedAt = DateTime.Now;
                    accPolicyRadioLic.RecieptAndPaymentId = model.RecieptAndPaymentId;
                    accPolicyRadioLic.PolicyId = model.PolicyId;
                    accPolicyRadioLic.PolicyNumber = model.PolicyNumber;
                    accPolicyRadioLic.AccountType = (int)PolicyAccountType.RadioLicense;
                    accPolicyRadioLic.Amount = model.RadioLicenseCost==null? 0:  Math.Round(model.RadioLicenseCost.Value,2);
                    accPolicyRadioLic.AccountName = PolicyAccountType.RadioLicense.ToString();
                    accPolicyRadioLic.Status = model.Status;
                    InsuranceContext.AccountPolices.Insert(accPolicyRadioLic);
                }

                if (model.ZinaraLicenseCost > 0)
                {
                    AccountPolicy accPolicyZinaraLic = new AccountPolicy();
                    accPolicyZinaraLic.CreatedAt = DateTime.Now;
                    accPolicyZinaraLic.RecieptAndPaymentId = model.RecieptAndPaymentId;
                    accPolicyZinaraLic.PolicyId = model.PolicyId;
                    accPolicyZinaraLic.PolicyNumber = model.PolicyNumber;
                    accPolicyZinaraLic.AccountType = (int)PolicyAccountType.ZinaraLicense;
                    accPolicyZinaraLic.Amount = Math.Round(model.ZinaraLicenseCost.Value,2);
                    accPolicyZinaraLic.AccountName = PolicyAccountType.ZinaraLicense.ToString();
                    accPolicyZinaraLic.Status = model.Status;
                    InsuranceContext.AccountPolices.Insert(accPolicyZinaraLic);
                }

            }
            catch (Exception ex)
            {
                SummaryDetailService.WriteLog("", ex.Message, "SaveAccountPolicy");
            }
        }






    }


    public static class CommonService
    {
        public static decimal GetTotalVehilePremium(int coverTypeId, decimal pemium, decimal stampDuty, decimal ztscLevy, decimal radioLicenseFee, decimal licenseFee)
        {
            decimal total = 0;

            if(coverTypeId==(int)eCoverType.Comprehensive)
                total = pemium + stampDuty + ztscLevy + radioLicenseFee;
            else
                total = pemium + stampDuty + ztscLevy + radioLicenseFee + licenseFee;

            return total;
        }


    }
}
