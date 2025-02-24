using AutoMapper;
using Insurance.Domain;
using Insurance.Service;
using InsuranceClaim.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace InsuranceClaim.Controllers
{
    public class LicenseController : Controller
    {
        private ApplicationUserManager _userManager;

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        // GET: License
        public ActionResult Index()
        {
            return View();
        }


        public ActionResult LicenseSummaryDetail(int summaryDetailId = 0, string paymentError = "")
        {
            var model = new SummaryDetailModel();

            decimal licenseFee = 0;

            var vehicle = (List<RiskDetailModel>)Session["VehicleDetails"];
            foreach (var item in vehicle)
            {
                decimal penalitesAmt = Convert.ToDecimal(item.PenaltiesAmt);
                licenseFee = +Convert.ToDecimal(item.VehicleLicenceFee);
                model.TotalPremium = licenseFee;

                if (item.IncludeRadioLicenseCost)
                {
                    model.TotalPremium += item.RadioLicenseCost;
                    model.TotalRadioLicenseCost += item.RadioLicenseCost;
                }
                model.Discount += item.Discount;
                var currency = InsuranceContext.Currencies.Single(where: $" Id='{item.CurrencyId}' ");

                if (currency != null)
                    item.CurrencyName = currency.Name;

                ViewBag.SummaryId = item.SummaryId;
            }

            model.TotalRadioLicenseCost = Math.Round(Convert.ToDecimal(model.TotalRadioLicenseCost), 2);
            model.Discount = Math.Round(Convert.ToDecimal(model.Discount), 2);
            model.TotalPremium = Math.Round(Convert.ToDecimal(model.TotalPremium), 2);
            model.TotalStampDuty = Math.Round(Convert.ToDecimal(vehicle.Sum(item => item.StampDuty)), 2);
            model.TotalSumInsured = Math.Round(Convert.ToDecimal(vehicle.Sum(item => item.SumInsured)), 2);
            model.TotalZTSCLevies = Math.Round(Convert.ToDecimal(vehicle.Sum(item => item.ZTSCLevy)), 2);
            model.ExcessBuyBackAmount = Math.Round(Convert.ToDecimal(vehicle.Sum(item => item.ExcessBuyBackAmount)), 2);
            model.MedicalExpensesAmount = Math.Round(Convert.ToDecimal(vehicle.Sum(item => item.MedicalExpensesAmount)), 2);
            model.PassengerAccidentCoverAmount = Math.Round(Convert.ToDecimal(vehicle.Sum(item => item.PassengerAccidentCoverAmount)), 2);
            model.RoadsideAssistanceAmount = Math.Round(Convert.ToDecimal(vehicle.Sum(item => item.RoadsideAssistanceAmount)), 2);
            model.ExcessAmount = Math.Round(Convert.ToDecimal(vehicle.Sum(item => item.ExcessAmount)), 2);
            model.AmountPaid = 0.00m;
            model.MaxAmounttoPaid = Math.Round(Convert.ToDecimal(model.TotalPremium), 2);
            var vehiclewithminpremium = vehicle.OrderBy(x => x.Premium).FirstOrDefault();

            if (vehiclewithminpremium != null)
                model.MinAmounttoPaid = Math.Round(Convert.ToDecimal(vehiclewithminpremium.Premium + vehiclewithminpremium.StampDuty + vehiclewithminpremium.ZTSCLevy + (Convert.ToBoolean(vehiclewithminpremium.IncludeRadioLicenseCost) ? vehiclewithminpremium.RadioLicenseCost : 0.00m)), 2);


            model.AmountPaid = Convert.ToDecimal(model.TotalPremium);
            model.BalancePaidDate = DateTime.Now;
            model.Notes = "";

            if (Session["PolicyData"] != null)
            {
                var PolicyData = (PolicyDetail)Session["PolicyData"];
                model.InvoiceNumber = PolicyData.PolicyNumber;
            }

            model.PaymentMethodId = (int)paymentMethod.Cash;
            

            return View(model);
        }


        //[HttpPost]
        public ActionResult GenerateQuote(RiskDetailModel model, string btnAddVehicle = "")
        {
            // for license payment term
            VehicleService _service = new VehicleService();

            // Submit & Add More Vehicle
            ModelState.Remove("MakeId");
            ModelState.Remove("ModelId");
            ModelState.Remove("CubicCapacity");
            ModelState.Remove("VehicleYear");
            ModelState.Remove("EngineNumber");
            ModelState.Remove("ChasisNumber");
            ModelState.Remove("CoverTypeId");
            ModelState.Remove("CoverStartDate");
            ModelState.Remove("CoverEndDate");
            ModelState.Remove("Premium");
            ModelState.Remove("PaymentTermId");
            ModelState.Remove("SuggestedValue");
            ModelState.Remove("TaxClassId");
            ModelState.Remove("SumInsured");

            //if (!string.IsNullOrEmpty(model.RegistrationNo) && model.ProductId > 0 && model.IncludeLicenseFee != false && model.ZinaraLicensePaymentTermId > 0)
            //{
            //    return RedirectToAction("LicenseDetail", "License");
            //}

            if (ModelState.IsValid)
            {
                try
                {
                    model.Id = 0;
                    model.vehicleindex = 1;
                    List<RiskDetailModel> listriskdetailmodel = new List<RiskDetailModel>();
                    model.PaymentTermId = Convert.ToInt32(model.ZinaraLicensePaymentTermId);
                    model.SummaryId = model.SummaryId;
                    listriskdetailmodel.Add(model);
                    Session["VehicleDetails"] = listriskdetailmodel;
                }
                catch (Exception ex)
                {
                    var result = ex.Message;
                    return RedirectToAction("LicenseDetail", "License");
                }

                return RedirectToAction("LicenseSummaryDetail", "License");
            }
            else
            {
                return View("LicenseDetail", model);
            }
        }

        [HttpGet]
        [Authorize(Roles = "Team Leaders,Staff,Agent,Administrator")]
        public ActionResult LicenseDetail(int summaryId = 0)
        {

            RiskDetailModel model = new RiskDetailModel();

            var summaryDetail = InsuranceContext.SummaryDetails.Single(summaryId);
            
            if (summaryDetail != null)
            {
                var Cusotmer = InsuranceContext.Customers.Single(summaryDetail.CustomerId);
                var customerModel = Mapper.Map<Customer, CustomerModel>(Cusotmer);
                var user = UserManager.FindById(Cusotmer.UserID);
                if (user != null)
                    customerModel.EmailAddress = user.Email;

                Session["CustomerDataModal"] = customerModel;
            }
            
                                                                                                             

            ProductDetail();

            // SetValueIntoSession(summaryId);

            ViewBag.Products = InsuranceContext.Products.All(where: "Active = 'True' or Active is null").ToList();
            ViewBag.VehicleLicensePaymentTermId = InsuranceContext.PaymentTerms.All(where: "IsActive = 'True' or IsActive is Null").ToList();
            ViewBag.RadioLicensePaymentTermId = InsuranceContext.PaymentTerms.All(where: "IsActive = 'True' or IsActive is Null").ToList();

            ViewBag.Currencies = InsuranceContext.Currencies.All(where: $"IsActive = 'True'");
            model.CurrencyId = 6; // default "RTGS$" selected

            // var vehileList = (List<RiskDetailModel>)Session["VehicleDetails"];
            // model.RegistrationNo = vehileList[0].RegistrationNo;

            ViewBag.Vehicles = GetActiveVehiclesBySummaryId(summaryId);
            model.NoOfCarsCovered = 1;
            model.SummaryId = summaryId;
            VehicleService service = new VehicleService();

            if(Session["VehicleDetails"]!=null)
            {
                try
                {
                    List<RiskDetailModel> listriskdetailmodel = (List<RiskDetailModel>)Session["VehicleDetails"];

                    var vehicleDetial = listriskdetailmodel[0];
                    model.ProductId = vehicleDetial.ProductId;
                    model.IncludeLicenseFee = vehicleDetial.IncludeLicenseFee;
                    model.IncludeRadioLicenseCost = vehicleDetial.IncludeRadioLicenseCost;
                    model.ZinaraLicensePaymentTermId = vehicleDetial.ZinaraLicensePaymentTermId;
                    model.RadioLicensePaymentTermId = vehicleDetial.RadioLicensePaymentTermId;
                    model.ArrearsAmt = vehicleDetial.ArrearsAmt;
                    model.PenaltiesAmt = vehicleDetial.PenaltiesAmt;
                    model.TransactionAmt = vehicleDetial.TransactionAmt;
                    model.VehicleLicenceFee = vehicleDetial.VehicleLicenceFee;
                    model.RadioLicenseCost = vehicleDetial.RadioLicenseCost;
                    model.LicenseId = vehicleDetial.LicenseId;
                    model.LicExpiryDate = vehicleDetial.LicExpiryDate;
                }
                catch(Exception ex)
                {

                }

              

            }


            return View(model);
        }


        public void ProductDetail()
        {

            var model = new PolicyDetailModel();
            var InsService = new InsurerService();
            model.CurrencyId = InsuranceContext.Currencies.All().FirstOrDefault().Id;
            model.PolicyStatusId = InsuranceContext.PolicyStatuses.All().FirstOrDefault().Id;
            model.BusinessSourceId = InsuranceContext.BusinessSources.All().FirstOrDefault().Id;
            //model.Products = InsuranceContext.Products.All().ToList();
            model.InsurerId = InsService.GetInsurers().FirstOrDefault().Id;
            var objList = InsuranceContext.PolicyDetails.All(orderBy: "Id desc").FirstOrDefault();
            if (objList != null)
            {
                string number = objList.PolicyNumber.Split('-')[0].Substring(4, objList.PolicyNumber.Length - 6);
                long pNumber = Convert.ToInt64(number.Substring(2, number.Length - 2)) + 1;
                string policyNumber = string.Empty;
                int length = 7;
                length = length - pNumber.ToString().Length;
                for (int i = 0; i < length; i++)
                {
                    policyNumber += "0";
                }
                policyNumber += pNumber;
                ViewBag.PolicyNumber = "GMCC" + DateTime.Now.Year.ToString().Substring(2, 2) + policyNumber + "-1";
                model.PolicyNumber = ViewBag.PolicyNumber;
            }
            else
            {
                ViewBag.PolicyNumber = ConfigurationManager.AppSettings["PolicyNumber"] + "-1";
                model.PolicyNumber = ViewBag.PolicyNumber;
            }

            model.BusinessSourceId = 3;
            Session["PolicyData"] = Mapper.Map<PolicyDetailModel, PolicyDetail>(model);
        }



        [HttpPost]
        public async Task<ActionResult> SubmitPlan(SummaryDetailModel model, string btnSendQuatation = "")
        {
            SummaryDetailService servicedetail = new SummaryDetailService();
            try
            {


                if (model != null)
                {
                    //if (ModelState.IsValid && (model.AmountPaid >= model.MinAmounttoPaid && model.AmountPaid <= model.MaxAmounttoPaid))
                    int CustomerUniquId = 0;
                    if (User.IsInRole("Administrator"))
                    {
                        TempData["SucessMsg"] = "Admin can not create policy.";
                        return RedirectToAction("SummaryDetail");
                    }


                    TempData["ErroMsg"] = null;
                    if (User.IsInRole("Staff") && model.PaymentMethodId == 1 && btnSendQuatation == "")
                    {
                        //  ModelState.Remove("InvoiceNumber");
                        if (string.IsNullOrEmpty(model.InvoiceNumber))
                        {
                            TempData["ErroMsg"] = "Please enter invoice number.";
                            return RedirectToAction("SummaryDetail");
                        }
                    }


                    if (ModelState.IsValid)
                    {
                        Insurance.Service.ICEcashService ICEcashService = new Insurance.Service.ICEcashService();
                        List<RiskDetailModel> list = new List<RiskDetailModel>();
                        string PartnerToken = "";

                        #region update  TPIQuoteUpdate
                        var customerDetails = new Customer();

                        var policyDetils = new PolicyDetail();

                        var customerEmail = "";
                        var policyNum = "";
                        var InsuranceID = "";
                        var vichelDetails = new VehicleDetail();

                        #endregion


                        #region Add All info to database

                        //var vehicle = (RiskDetailModel)Session["VehicleDetail"];
                        Session["SummaryDetailed"] = model;
                        string SummeryofReinsurance = "";
                        string SummeryofVehicleInsured = "";
                        bool userLoggedin = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
                        var customer = (CustomerModel)Session["CustomerDataModal"];

                        var policy = (PolicyDetail)Session["PolicyData"];


                        // Genrate new policy number
                        PolicyService servicePolicy = new PolicyService();
                        if (policy != null && policy.Id == 0)
                        {
                            string policyNumber = string.Empty;

                            var objList = InsuranceContext.PolicyDetails.All(orderBy: "Id desc").FirstOrDefault();
                            if (objList != null)
                            {
                                string number = objList.PolicyNumber.Split('-')[0].Substring(4, objList.PolicyNumber.Length - 6);
                                long pNumber = Convert.ToInt64(number.Substring(2, number.Length - 2)) + 1;

                                int length = 7;
                                length = length - pNumber.ToString().Length;
                                for (int i = 0; i < length; i++)
                                {
                                    policyNumber += "0";
                                }
                                policyNumber += Convert.ToString(servicePolicy.GetUniquePolicy());
                                policy.PolicyNumber = "GMCC" + DateTime.Now.Year.ToString().Substring(2, 2) + policyNumber + "-1";


                            }
                        }
                        // end genrate policy number


                        if (policy != null)
                        {
                            if (policy.Id == null || policy.Id == 0)
                            {
                                policy.CustomerId = customer.Id;
                                policy.StartDate = null;
                                policy.EndDate = null;
                                policy.TransactionDate = null;
                                policy.RenewalDate = null;
                                policy.RenewalDate = null;
                                policy.StartDate = null;
                                policy.TransactionDate = null;
                                policy.CreatedBy = customer.Id;
                                policy.CreatedOn = DateTime.Now;
                                InsuranceContext.PolicyDetails.Insert(policy);
                                Session["PolicyData"] = policy;
                            }
                            else
                            {

                                PolicyDetail policydata = InsuranceContext.PolicyDetails.All(policy.Id.ToString()).FirstOrDefault();
                                policydata.BusinessSourceId = policy.BusinessSourceId;
                                policydata.CurrencyId = policy.CurrencyId;
                                // policydata.CustomerId = policy.CustomerId;
                                policydata.CustomerId = customer.Id;
                                policydata.EndDate = null;
                                policydata.InsurerId = policy.InsurerId;
                                policydata.IsActive = policy.IsActive;
                                policydata.PolicyName = policy.PolicyName;
                                policydata.PolicyNumber = policy.PolicyNumber;
                                policydata.PolicyStatusId = policy.PolicyStatusId;
                                policydata.RenewalDate = null;
                                policydata.StartDate = null;
                                policydata.TransactionDate = null;
                                policy.ModifiedBy = customer.Id;
                                policy.ModifiedOn = DateTime.Now;
                                InsuranceContext.PolicyDetails.Update(policydata);
                            }

                        }
                        var Id = 0;
                        var listReinsuranceTransaction = new List<ReinsuranceTransaction>();
                        var vehicle = (List<RiskDetailModel>)Session["VehicleDetails"];

                        string format = "yyyyMMdd";


                        if (vehicle != null && vehicle.Count > 0)
                        {
                            foreach (var item in vehicle.ToList())
                            {
                                if (item.CoverTypeId == (int)eCoverType.Comprehensive)
                                {
                                    item.CompLicenseFee = item.VehicleLicenceFee;
                                    item.VehicleLicenceFee = 0;
                                }

                                item.IceCashRequest = "License";
                                if (item.RadioLicenseCost > 0)  // for now 
                                    item.IncludeRadioLicenseCost = Convert.ToBoolean(true);

                                if (item.LicExpiryDate!=null && item.LicExpiryDate!="")
                                {
                                    DateTime LicExpiryDate = DateTime.ParseExact(item.LicExpiryDate, format, CultureInfo.InvariantCulture);
                                    item.LicExpiryDate = LicExpiryDate.ToShortDateString();
                                }
                                    
                                item.IsMobile = false;
                                var _item = item;

                                var vehicelDetails = InsuranceContext.VehicleDetails.Single(where: $"policyid= '{policy.Id}' and RegistrationNo= '{_item.RegistrationNo}'");
                                if (vehicelDetails != null)
                                {
                                    item.Id = vehicelDetails.Id;
                                }


                                if (item.Id == 0)
                                {
                                    var service = new RiskDetailService();
                                    _item.CustomerId = customer.Id;
                                    _item.PolicyId = policy.Id;
                                    _item.CoverStartDate = DateTime.Now;
                                    //   _item.InsuranceId = model.InsuranceId;
                                    //if (model.AmountPaid < model.TotalPremium)
                                    //{
                                    //    _item.BalanceAmount = (_item.Premium + _item.ZTSCLevy + _item.StampDuty + (_item.IncludeRadioLicenseCost ? _item.RadioLicenseCost : 0.00m) - _item.Discount) - (model.AmountPaid / vehicle.Count);
                                    //}

                                    _item.Id = service.AddVehicleInformation(_item);
                                    var vehicles = (List<RiskDetailModel>)Session["VehicleDetails"];
                                    vehicles[Convert.ToInt32(_item.NoOfCarsCovered) - 1] = _item;
                                    Session["VehicleDetails"] = vehicles;

                                    // Delivery Address Save
                                    try
                                    {
                                        var LicenseAddress = new LicenceDiskDeliveryAddress();
                                        LicenseAddress.Address1 = _item.LicenseAddress1;
                                        LicenseAddress.Address2 = _item.LicenseAddress2;
                                        LicenseAddress.City = _item.LicenseCity;
                                        LicenseAddress.VehicleId = _item.Id;
                                        LicenseAddress.CreatedBy = customer.Id;
                                        LicenseAddress.CreatedOn = DateTime.Now;
                                        LicenseAddress.ModifiedBy = customer.Id;
                                        LicenseAddress.ModifiedOn = DateTime.Now;
                                        if (_item.ReceiptDate.Year == 0001)
                                        {
                                            _item.ReceiptDate = DateTime.MinValue;
                                        }

                                        LicenseAddress.ReceiptDate = _item.ReceiptDate;
                                        LicenseAddress.ExpectedDateDelivery = DateTime.Now;
                                        InsuranceContext.LicenceDiskDeliveryAddresses.Insert(LicenseAddress);
                                    }
                                    catch (Exception ex)
                                    {

                                    }



                                    ///Licence Ticket
                                    if (_item.IsLicenseDiskNeeded)
                                    {
                                        var LicenceTicket = new LicenceTicket();
                                        var Licence = InsuranceContext.LicenceTickets.All(orderBy: "Id desc").FirstOrDefault();

                                        if (Licence != null)
                                        {

                                            string number = Licence.TicketNo.Substring(3);

                                            long tNumber = Convert.ToInt64(number) + 1;
                                            string TicketNo = string.Empty;
                                            int length = 6;
                                            length = length - tNumber.ToString().Length;

                                            for (int i = 0; i < length; i++)
                                            {
                                                TicketNo += "0";
                                            }
                                            TicketNo += tNumber;
                                            var ticketnumber = "GEN" + TicketNo;

                                            LicenceTicket.TicketNo = ticketnumber;
                                        }
                                        else
                                        {
                                            var TicketNo = ConfigurationManager.AppSettings["TicketNo"];

                                            LicenceTicket.TicketNo = TicketNo;


                                        }

                                        LicenceTicket.VehicleId = _item.Id;
                                        LicenceTicket.CloseComments = "";
                                        LicenceTicket.ReopenComments = "";
                                        LicenceTicket.DeliveredTo = "";
                                        LicenceTicket.CreatedDate = DateTime.Now;
                                        LicenceTicket.CreatedBy = customer.Id;
                                        LicenceTicket.IsClosed = false;
                                        LicenceTicket.PolicyNumber = policy.PolicyNumber;

                                        InsuranceContext.LicenceTickets.Insert(LicenceTicket);
                                    }

                                    ///Reinsurance                      

                                    var ReinsuranceCases = InsuranceContext.Reinsurances.All(where: $"Type='Reinsurance'").ToList();
                                    var ownRetention = InsuranceContext.Reinsurances.All().Where(x => x.TreatyCode == "OR001").Select(x => x.MaxTreatyCapacity).SingleOrDefault();
                                    var ReinsuranceCase = new Reinsurance();

                                    foreach (var Reinsurance in ReinsuranceCases)
                                    {
                                        if (Reinsurance.MinTreatyCapacity <= item.SumInsured && item.SumInsured <= Reinsurance.MaxTreatyCapacity)
                                        {
                                            ReinsuranceCase = Reinsurance;
                                            break;
                                        }
                                    }

                                    if (ReinsuranceCase != null && ReinsuranceCase.MaxTreatyCapacity != null)
                                    {
                                        var basicPremium = item.Premium;
                                        var ReinsuranceBroker = InsuranceContext.ReinsuranceBrokers.Single(where: $"ReinsuranceBrokerCode='{ReinsuranceCase.ReinsuranceBrokerCode}'");
                                        var AutoFacSumInsured = 0.00m;
                                        var AutoFacPremium = 0.00m;
                                        var FacSumInsured = 0.00m;
                                        var FacPremium = 0.00m;

                                        if (ReinsuranceCase.MinTreatyCapacity > 200000)
                                        {
                                            var autofaccase = ReinsuranceCases.FirstOrDefault();
                                            var autofacSumInsured = autofaccase.MaxTreatyCapacity - ownRetention;
                                            var autofacReinsuranceBroker = InsuranceContext.ReinsuranceBrokers.Single(where: $"ReinsuranceBrokerCode='{autofaccase.ReinsuranceBrokerCode}'");

                                            var _reinsurance = new ReinsuranceTransaction();
                                            _reinsurance.ReinsuranceAmount = autofacSumInsured;
                                            AutoFacSumInsured = Convert.ToDecimal(_reinsurance.ReinsuranceAmount);
                                            _reinsurance.ReinsurancePremium = Math.Round(Convert.ToDecimal((_reinsurance.ReinsuranceAmount / item.SumInsured) * basicPremium), 2);
                                            AutoFacPremium = Convert.ToDecimal(_reinsurance.ReinsurancePremium);
                                            _reinsurance.ReinsuranceCommissionPercentage = Convert.ToDecimal(autofacReinsuranceBroker.Commission);
                                            _reinsurance.ReinsuranceCommission = Math.Round(Convert.ToDecimal((_reinsurance.ReinsurancePremium * _reinsurance.ReinsuranceCommissionPercentage) / 100), 2);
                                            _reinsurance.VehicleId = item.Id;
                                            _reinsurance.ReinsuranceBrokerId = autofacReinsuranceBroker.Id;
                                            _reinsurance.TreatyName = autofaccase.TreatyName;
                                            _reinsurance.TreatyCode = autofaccase.TreatyCode;
                                            _reinsurance.CreatedOn = DateTime.Now;
                                            _reinsurance.CreatedBy = customer.Id;

                                            InsuranceContext.ReinsuranceTransactions.Insert(_reinsurance);

                                            SummeryofReinsurance += "<tr><td>" + Convert.ToString(_reinsurance.Id) + "</td><td>" + ReinsuranceCase.TreatyCode + "</td><td>" + ReinsuranceCase.TreatyName + "</td><td>" + Convert.ToString(_reinsurance.ReinsuranceAmount) + "</td><td>" + Convert.ToString(ReinsuranceBroker.ReinsuranceBrokerName) + "</td><td>" + Convert.ToString(Math.Round(Convert.ToDecimal(_reinsurance.ReinsurancePremium), 2)) + "</td><td>" + Convert.ToString(ReinsuranceBroker.Commission) + "</td></tr>";

                                            listReinsuranceTransaction.Add(_reinsurance);

                                            var __reinsurance = new ReinsuranceTransaction();
                                            __reinsurance.ReinsuranceAmount = _item.SumInsured - ownRetention - autofacSumInsured;
                                            FacSumInsured = Convert.ToDecimal(__reinsurance.ReinsuranceAmount);
                                            __reinsurance.ReinsurancePremium = Math.Round(Convert.ToDecimal((__reinsurance.ReinsuranceAmount / item.SumInsured) * basicPremium), 2);
                                            FacPremium = Convert.ToDecimal(__reinsurance.ReinsurancePremium);
                                            __reinsurance.ReinsuranceCommissionPercentage = Convert.ToDecimal(ReinsuranceBroker.Commission);
                                            __reinsurance.ReinsuranceCommission = Math.Round(Convert.ToDecimal((__reinsurance.ReinsurancePremium * __reinsurance.ReinsuranceCommissionPercentage) / 100), 2);
                                            __reinsurance.VehicleId = item.Id;
                                            __reinsurance.ReinsuranceBrokerId = ReinsuranceBroker.Id;
                                            __reinsurance.TreatyName = ReinsuranceCase.TreatyName;
                                            __reinsurance.TreatyCode = ReinsuranceCase.TreatyCode;
                                            __reinsurance.CreatedOn = DateTime.Now;
                                            __reinsurance.CreatedBy = customer.Id;

                                            InsuranceContext.ReinsuranceTransactions.Insert(__reinsurance);

                                            //SummeryofReinsurance += "<tr><td>" + Convert.ToString(__reinsurance.Id) + "</td><td>" + ReinsuranceCase.TreatyCode + "</td><td>" + ReinsuranceCase.TreatyName + "</td><td>" + Convert.ToString(__reinsurance.ReinsuranceAmount) + "</td><td>" + Convert.ToString(ReinsuranceBroker.ReinsuranceBrokerName) + "</td><td>" + Convert.ToString(Math.Round(Convert.ToDecimal(__reinsurance.ReinsurancePremium), 2)) + "</td><td>" + Convert.ToString(ReinsuranceBroker.Commission) + "</td></tr>";

                                            listReinsuranceTransaction.Add(__reinsurance);
                                        }
                                        else
                                        {

                                            var reinsurance = new ReinsuranceTransaction();
                                            reinsurance.ReinsuranceAmount = _item.SumInsured - ownRetention;
                                            AutoFacSumInsured = Convert.ToDecimal(reinsurance.ReinsuranceAmount);
                                            reinsurance.ReinsurancePremium = Math.Round(Convert.ToDecimal((reinsurance.ReinsuranceAmount / item.SumInsured) * basicPremium), 2);
                                            AutoFacPremium = Convert.ToDecimal(reinsurance.ReinsurancePremium);
                                            reinsurance.ReinsuranceCommissionPercentage = Convert.ToDecimal(ReinsuranceBroker.Commission);
                                            reinsurance.ReinsuranceCommission = Math.Round(Convert.ToDecimal((reinsurance.ReinsurancePremium * reinsurance.ReinsuranceCommissionPercentage) / 100), 2);
                                            reinsurance.VehicleId = item.Id;
                                            reinsurance.ReinsuranceBrokerId = ReinsuranceBroker.Id;
                                            reinsurance.TreatyName = ReinsuranceCase.TreatyName;
                                            reinsurance.TreatyCode = ReinsuranceCase.TreatyCode;
                                            reinsurance.CreatedOn = DateTime.Now;
                                            reinsurance.CreatedBy = customer.Id;

                                            InsuranceContext.ReinsuranceTransactions.Insert(reinsurance);

                                            //SummeryofReinsurance += "<tr><td>" + Convert.ToString(reinsurance.Id) + "</td><td>" + ReinsuranceCase.TreatyCode + "</td><td>" + ReinsuranceCase.TreatyName + "</td><td>" + Convert.ToString(reinsurance.ReinsuranceAmount) + "</td><td>" + Convert.ToString(ReinsuranceBroker.ReinsuranceBrokerName) + "</td><td>" + Convert.ToString(Math.Round(Convert.ToDecimal(reinsurance.ReinsurancePremium), 2)) + "</td><td>" + Convert.ToString(ReinsuranceBroker.Commission) + "</td></tr>";

                                            listReinsuranceTransaction.Add(reinsurance);
                                        }


                                        Insurance.Service.VehicleService obj = new Insurance.Service.VehicleService();
                                        VehicleModel vehiclemodel = InsuranceContext.VehicleModels.Single(where: $"ModelCode='{item.ModelId}'");
                                        VehicleMake vehiclemake = InsuranceContext.VehicleMakes.Single(where: $" MakeCode='{item.MakeId}'");

                                        string vehicledescription = vehiclemodel.ModelDescription + " / " + vehiclemake.MakeDescription;

                                        // SummeryofVehicleInsured += "<tr><td>" + vehicledescription + "</td><td>" + Convert.ToString(item.SumInsured) + "</td><td>" + Convert.ToString(item.Premium) + "</td><td>" + AutoFacSumInsured.ToString() + "</td><td>" + AutoFacPremium.ToString() + "</td><td>" + FacSumInsured.ToString() + "</td><td>" + FacPremium.ToString() + "</td></tr>";

                                        SummeryofVehicleInsured += "<tr><td style='padding:7px 10px; font-size:14px'><font size='2'>" + vehicledescription + "</font></td><td style='padding:7px 10px; font-size:14px'><font size='2'>" + Convert.ToString(item.SumInsured) + " </font></td><td style='padding:7px 10px; font-size:14px'><font size='2'>" + Convert.ToString(item.Premium) + "</font></td><td style='padding:7px 10px; font-size:14px'><font size='2'>" + AutoFacSumInsured.ToString() + "</font></td><td style='padding:7px 10px; font-size:14px'><font size='2'>" + AutoFacPremium.ToString() + "</ font ></td><td style='padding:7px 10px; font-size:14px'><font size='2'>" + FacSumInsured.ToString() + "</font></td><td style='padding:7px 10px; font-size:14px'><font size='2'>" + FacPremium.ToString() + "</font></td></tr>";

                                    }
                                }
                                else
                                {
                                    VehicleDetail Vehicledata = InsuranceContext.VehicleDetails.All(item.Id.ToString()).FirstOrDefault();
                                    Vehicledata.AgentCommissionId = item.AgentCommissionId;
                                    Vehicledata.ChasisNumber = item.ChasisNumber;
                                    Vehicledata.CoverEndDate = item.CoverEndDate;
                                    Vehicledata.CoverNoteNo = item.CoverNoteNo;
                                    Vehicledata.CoverStartDate = item.CoverStartDate;
                                    Vehicledata.CoverTypeId = item.CoverTypeId;
                                    Vehicledata.CubicCapacity = item.CubicCapacity;
                                    Vehicledata.EngineNumber = item.EngineNumber;
                                    Vehicledata.Excess = item.Excess;
                                    Vehicledata.ExcessType = item.ExcessType;
                                    Vehicledata.MakeId = item.MakeId;
                                    Vehicledata.ModelId = item.ModelId;
                                    Vehicledata.NoOfCarsCovered = item.NoOfCarsCovered;
                                    Vehicledata.OptionalCovers = item.OptionalCovers;
                                    Vehicledata.PolicyId = item.PolicyId;
                                    Vehicledata.Premium = item.Premium;
                                    Vehicledata.RadioLicenseCost = (item.IsLicenseDiskNeeded ? item.RadioLicenseCost : 0.00m);
                                    Vehicledata.Rate = item.Rate;
                                    Vehicledata.RegistrationNo = item.RegistrationNo;
                                    Vehicledata.StampDuty = item.StampDuty;
                                    Vehicledata.SumInsured = item.SumInsured;
                                    Vehicledata.VehicleColor = item.VehicleColor;
                                    Vehicledata.VehicleUsage = item.VehicleUsage;
                                    Vehicledata.VehicleYear = item.VehicleYear;
                                    Vehicledata.ZTSCLevy = item.ZTSCLevy;
                                    Vehicledata.Addthirdparty = item.Addthirdparty;
                                    Vehicledata.AddThirdPartyAmount = item.AddThirdPartyAmount;
                                    Vehicledata.PassengerAccidentCover = item.PassengerAccidentCover;
                                    Vehicledata.ExcessBuyBack = item.ExcessBuyBack;
                                    Vehicledata.RoadsideAssistance = item.RoadsideAssistance;

                                    // 006_feb
                                    Vehicledata.RoadsideAssistanceAmount = item.RoadsideAssistanceAmount;
                                    Vehicledata.MedicalExpensesAmount = item.MedicalExpensesAmount;



                                    Vehicledata.MedicalExpenses = item.MedicalExpenses;
                                    Vehicledata.NumberofPersons = item.NumberofPersons;
                                    Vehicledata.IsLicenseDiskNeeded = item.IsLicenseDiskNeeded;
                                    Vehicledata.AnnualRiskPremium = item.AnnualRiskPremium;
                                    Vehicledata.TermlyRiskPremium = item.TermlyRiskPremium;
                                    Vehicledata.QuaterlyRiskPremium = item.QuaterlyRiskPremium;
                                    Vehicledata.TransactionDate = DateTime.Now;


                                    if (Vehicledata.ExcessBuyBack == true)
                                    {
                                        Vehicledata.ExcessBuyBackAmount = item.ExcessBuyBackAmount;
                                    }

                                    if (Vehicledata.PassengerAccidentCover == true)
                                    {
                                        Vehicledata.PassengerAccidentCoverAmount = item.PassengerAccidentCoverAmount;
                                    }
                                    if (Vehicledata.ExcessBuyBack == true)
                                    {
                                        Vehicledata.ExcessBuyBackAmount = item.ExcessBuyBackAmount;
                                    }

                                    if (Vehicledata.PassengerAccidentCover == true)
                                    {
                                        Vehicledata.PassengerAccidentCoverAmount = item.PassengerAccidentCoverAmount;
                                    }

                                    Vehicledata.CustomerId = customer.Id;
                                    // Vehicledata.InsuranceId = model.InsuranceId;

                                    InsuranceContext.VehicleDetails.Update(Vehicledata);
                                    var _summary = (SummaryDetailModel)Session["SummaryDetailed"];


                                    var ReinsuranceCases = InsuranceContext.Reinsurances.All(where: $"Type='Reinsurance'").ToList();
                                    var ownRetention = InsuranceContext.Reinsurances.All().Where(x => x.TreatyCode == "OR001").Select(x => x.MaxTreatyCapacity).SingleOrDefault();
                                    var ReinsuranceCase = new Reinsurance();

                                    foreach (var Reinsurance in ReinsuranceCases)
                                    {
                                        if (Reinsurance.MinTreatyCapacity <= item.SumInsured && item.SumInsured <= Reinsurance.MaxTreatyCapacity)
                                        {
                                            ReinsuranceCase = Reinsurance;
                                            break;
                                        }
                                    }

                                    if (ReinsuranceCase != null && ReinsuranceCase.MaxTreatyCapacity != null)
                                    {
                                        var ReinsuranceBroker = InsuranceContext.ReinsuranceBrokers.Single(where: $"ReinsuranceBrokerCode='{ReinsuranceCase.ReinsuranceBrokerCode}'");

                                        var summaryid = _summary.Id;
                                        var vehicleid = item.Id;
                                        var ReinsuranceTransactions = InsuranceContext.ReinsuranceTransactions.Single(where: $"SummaryDetailId={_summary.Id} and VehicleId={item.Id}");
                                        //var _reinsurance = new ReinsuranceTransaction();
                                        ReinsuranceTransactions.ReinsuranceAmount = _item.SumInsured - ownRetention;
                                        ReinsuranceTransactions.ReinsurancePremium = ((ReinsuranceTransactions.ReinsuranceAmount / item.SumInsured) * item.Premium);
                                        ReinsuranceTransactions.ReinsuranceCommissionPercentage = Convert.ToDecimal(ReinsuranceBroker.Commission);
                                        ReinsuranceTransactions.ReinsuranceCommission = ((ReinsuranceTransactions.ReinsurancePremium * ReinsuranceTransactions.ReinsuranceCommissionPercentage) / 100);//Convert.ToDecimal(defaultReInsureanceBroker.Commission);
                                        ReinsuranceTransactions.ReinsuranceBrokerId = ReinsuranceBroker.Id;

                                        InsuranceContext.ReinsuranceTransactions.Update(ReinsuranceTransactions);
                                    }
                                    else
                                    {
                                        var ReinsuranceTransactions = InsuranceContext.ReinsuranceTransactions.Single(where: $"SummaryDetailId={_summary.Id} and VehicleId={item.Id}");
                                        if (ReinsuranceTransactions != null)
                                        {
                                            InsuranceContext.ReinsuranceTransactions.Delete(ReinsuranceTransactions);
                                        }
                                    }

                                }
                            }
                        }

                        var summary = (SummaryDetailModel)Session["SummaryDetailed"];
                        var DbEntry = Mapper.Map<SummaryDetailModel, SummaryDetail>(model);


                        if (summary != null)
                        {
                            if (summary.Id == 0)
                            {
                                if (Session["VehicleDetails"] != null) // forcelly check because in some case summary details id is comming 0
                                {
                                    var vehicalDetailsForSummary = (List<RiskDetailModel>)Session["VehicleDetails"];
                                    if (vehicalDetailsForSummary[0].Id != 0)
                                    {
                                        var SummaryVehicalDetails = InsuranceContext.SummaryVehicleDetails.All(where: $"VehicleDetailsId={vehicalDetailsForSummary[0].Id}").ToList();

                                        if (SummaryVehicalDetails.Count() > 0)
                                        {
                                            summary.Id = SummaryVehicalDetails[0].SummaryDetailId;
                                        }
                                    }
                                }
                            }

                            if (summary.Id == 0)
                            {
                                //DbEntry.PaymentTermId = Convert.ToInt32(Session["policytermid"]);
                                //DbEntry.VehicleDetailId = vehicle[0].Id;
                                //  bool _userLoggedin = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;



                                // DbEntry.CustomerId = vehicle[0].CustomerId;
                                DbEntry.CustomerId = customer.Id;

                                bool _userLoggedin = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;

                                if (_userLoggedin)
                                {
                                    var _User = UserManager.FindById(User.Identity.GetUserId().ToString());
                                    var _customerData = InsuranceContext.Customers.All(where: $"UserId ='{_User.Id}'").FirstOrDefault();

                                    if (_customerData != null)
                                    {
                                        DbEntry.CreatedBy = _customerData.Id;
                                    }
                                }


                                DbEntry.CreatedOn = DateTime.Now;
                                if (DbEntry.BalancePaidDate.Value.Year == 0001)
                                {
                                    DbEntry.BalancePaidDate = DateTime.Now;
                                }
                                if (DbEntry.Notes == null)
                                {
                                    DbEntry.Notes = "";
                                }

                                if (!string.IsNullOrEmpty(btnSendQuatation))
                                {
                                    DbEntry.isQuotation = true;
                                }

                                if (DbEntry.PaymentMethodId == (int)paymentMethod.PayLater)
                                {
                                    DbEntry.PayLaterStatus = true;
                                }

                                try
                                {
                                    InsuranceContext.SummaryDetails.Insert(DbEntry);
                                }
                                catch (Exception ex)
                                {
                                    LogDetailTbl log = new LogDetailTbl();
                                    log.Request = "SummaryDetails " + ex.Message;
                                    string vehicleInfo = customer.EmailAddress;
                                    log.Response = vehicleInfo;


                                    InsuranceContext.LogDetailTbls.Insert(log);

                                    throw;
                                }




                                model.Id = DbEntry.Id;
                                Session["SummaryDetailed"] = model;
                            }
                            else
                            {
                                // SummaryDetail summarydata = InsuranceContext.SummaryDetails.All(summary.Id.ToString()).FirstOrDefault(); // on 05-oct for updatig qutation

                                var summarydata = Mapper.Map<SummaryDetailModel, SummaryDetail>(model);
                                summarydata.Id = summary.Id;
                                summarydata.CreatedOn = DateTime.Now;

                                if (!string.IsNullOrEmpty(btnSendQuatation))
                                {
                                    summarydata.isQuotation = true;
                                }


                                //summarydata.PaymentTermId = Convert.ToInt32(Session["policytermid"]);
                                //summarydata.VehicleDetailId = vehicle[0].Id;


                                bool _userLoggedin = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
                                if (_userLoggedin)
                                {
                                    var _User = UserManager.FindById(User.Identity.GetUserId().ToString());
                                    var _customerData = InsuranceContext.Customers.All(where: $"UserId ='{_User.Id}'").FirstOrDefault();

                                    if (_customerData != null)
                                    {
                                        summarydata.CreatedBy = _customerData.Id;
                                    }
                                }


                                summarydata.ModifiedBy = customer.Id;
                                summarydata.ModifiedOn = DateTime.Now;
                                if (summarydata.BalancePaidDate.Value.Year == 0001)
                                {
                                    summarydata.BalancePaidDate = DateTime.Now;
                                }
                                if (DbEntry.Notes == null)
                                {
                                    summarydata.Notes = "";
                                }
                                //summarydata.CustomerId = vehicle[0].CustomerId;

                                summarydata.CustomerId = customer.Id;


                                if (summarydata.PaymentMethodId == (int)paymentMethod.PayLater)
                                {
                                    summarydata.PayLaterStatus = true;
                                }


                                try
                                {
                                    InsuranceContext.SummaryDetails.Update(summarydata);
                                }
                                catch (Exception ex)
                                {
                                    LogDetailTbl log = new LogDetailTbl();
                                    log.Request = "Summarydetails " + ex.Message;
                                    string vehicleInfo = customer.EmailAddress;
                                    log.Response = vehicleInfo;
                                    InsuranceContext.LogDetailTbls.Insert(log);
                                    throw;
                                }
                            }



                            if (listReinsuranceTransaction != null && listReinsuranceTransaction.Count > 0)
                            {
                                foreach (var item in listReinsuranceTransaction)
                                {
                                    var InsTransac = InsuranceContext.ReinsuranceTransactions.Single(item.Id);
                                    InsTransac.SummaryDetailId = summary.Id;
                                    InsuranceContext.ReinsuranceTransactions.Update(InsTransac);
                                }
                            }
                        }



                        if (vehicle != null && vehicle.Count > 0 && summary.Id > 0)
                        {
                            var SummaryDetails = InsuranceContext.SummaryVehicleDetails.All(where: $"SummaryDetailId={summary.Id}").ToList();

                            if (SummaryDetails != null && SummaryDetails.Count > 0)
                            {
                                foreach (var item in SummaryDetails)
                                {
                                    InsuranceContext.SummaryVehicleDetails.Delete(item);
                                }
                            }

                            foreach (var item in vehicle.ToList())
                            {
                                try
                                {
                                    var summarydetails = new SummaryVehicleDetail();
                                    summarydetails.SummaryDetailId = summary.Id;
                                    summarydetails.VehicleDetailsId = item.Id;
                                    summarydetails.CreatedBy = customer.Id;
                                    summarydetails.CreatedOn = DateTime.Now;
                                    InsuranceContext.SummaryVehicleDetails.Insert(summarydetails);
                                }
                                catch (Exception ex)
                                {
                                    //Insurance.Service.EmailService log = new Insurance.Service.EmailService();
                                    //log.WriteLog("exception during insert vehicel :" + ex.Message);

                                    LogDetailTbl log = new LogDetailTbl();
                                    log.Request = "SummaryVehicleDetails " + ex.Message;
                                    string vehicleInfo = item.RegistrationNo + "," + customer.EmailAddress + "," + summary.Id;
                                    log.Response = vehicleInfo;
                                    InsuranceContext.LogDetailTbls.Insert(log);

                                    throw;

                                }
                            }
                            MiscellaneousService.UpdateBalanceForVehicles(summary.AmountPaid, summary.Id, Convert.ToDecimal(summary.TotalPremium), false);
                        }

                        if (listReinsuranceTransaction != null && listReinsuranceTransaction.Count > 0)
                        {
                            string filepath = System.Configuration.ConfigurationManager.AppSettings["urlPath"];
                            int _vehicleId = 0;
                            int count = 0;
                            bool MailSent = false;

                        }

                        #endregion

                        // return RedirectToAction("InitiatePaynowTransaction", "Paypal", new { id = DbEntry.Id, TotalPremiumPaid = Convert.ToString(model.AmountPaid), PolicyNumber = policy.PolicyNumber, Email = customer.EmailAddress });


                        Session["PollUrl"] = null;
                        if (model.PaymentMethodId == 1 || model.PaymentMethodId == (int)paymentMethod.PayLater)
                            return RedirectToAction("SaveDetailList", "Paypal", new { id = DbEntry.Id, invoiceNumer = model.InvoiceNumber, Paymentid = model.PaymentMethodId.Value });
                        else if (model.PaymentMethodId == (int)paymentMethod.PayNow)
                        {
                            Insurance.Service.EmailService log = new Insurance.Service.EmailService();

                            CustomerRegistrationController customerController = new CustomerRegistrationController();

                            var payNow = customerController.PayNow(DbEntry.Id, model.InvoiceNumber, model.PaymentMethodId.Value, Convert.ToDecimal(model.TotalPremium));
                            if (payNow.IsSuccessPayment)
                            {
                                Session["PayNowSummmaryId"] = DbEntry.Id;
                                Session["PollUrl"] = payNow.PollUrl;
                                return Redirect(payNow.ReturnUrl);
                            }
                            else
                            {
                                return RedirectToAction("failed_url", "Paypal");
                            }
                        }
                        else if (model.PaymentMethodId == (int)paymentMethod.ecocash)
                        {
                            //return RedirectToAction("InitiatePaynowTransaction", "Paypal", new { id = DbEntry.Id, TotalPremiumPaid = Convert.ToString(model.AmountPaid), PolicyNumber = policy.PolicyNumber, Email = customer.EmailAddress });
                            return RedirectToAction("EcoCashPayment", "Paypal", new { id = DbEntry.Id, invoiceNumber = model.InvoiceNumber, Paymentid = model.PaymentMethodId.Value });

                            // return RedirectToAction("SaveDetailList", "Paypal", new { id = DbEntry.Id, invoiceNumer = model.InvoiceNumber, Paymentid = model.PaymentMethodId.Value });
                        }
                        else if (model.PaymentMethodId == (int)paymentMethod.Zimswitch)
                        {
                            TempData["PaymentMethodId"] = model.PaymentMethodId;
                            return RedirectToAction("IceCashPayment", "Paypal", new { id = model.Id, amount = Convert.ToString(model.AmountPaid), Paymentid = model.PaymentMethodId.Value });
                        }
                        else
                            return RedirectToAction("PaymentDetail", new { id = DbEntry.Id, invoiceNumer = model.InvoiceNumber, Paymentid = model.PaymentMethodId.Value });
                    }
                    else
                    {
                        return RedirectToAction("LicenseSummaryDetail");
                    }
                }
                else
                {
                    return RedirectToAction("LicenseSummaryDetail");
                }
            }
            catch (Exception ex)
            {
                //return RedirectToAction("SummaryDetail");

                LogDetailTbl log = new LogDetailTbl();
                log.Request = "SubmitPlan " + ex.Message;
                string vehicleInfo = model.InvoiceNumber;
                log.Response = vehicleInfo;
                InsuranceContext.LogDetailTbls.Insert(log);
                throw;
            }

        }

        public void SetValueIntoSession(int summaryId)
        {
            Session["ICEcashToken"] = null;
            Session["issummaryformvisited"] = true;

            Session["SummaryDetailId"] = summaryId;

            var summaryDetail = InsuranceContext.SummaryDetails.Single(summaryId);
            var SummaryVehicleDetails = InsuranceContext.SummaryVehicleDetails.All(where: $"SummaryDetailId={summaryId}").ToList();
            var vehicle = InsuranceContext.VehicleDetails.Single(SummaryVehicleDetails[0].VehicleDetailsId);
            var policy = InsuranceContext.PolicyDetails.Single(vehicle.PolicyId);
            var product = InsuranceContext.Products.Single(Convert.ToInt32(policy.PolicyName));

            Session["PolicyData"] = policy;

            List<RiskDetailModel> listRiskDetail = new List<RiskDetailModel>();
            foreach (var item in SummaryVehicleDetails)
            {
                //  var _vehicle = InsuranceContext.VehicleDetails.Single(item.VehicleDetailsId);

                var _vehicle = InsuranceContext.VehicleDetails.Single(where: "id=" + item.VehicleDetailsId + " and IsActive=1");
                if (_vehicle != null)
                {
                    RiskDetailModel riskDetail = Mapper.Map<VehicleDetail, RiskDetailModel>(_vehicle);
                    listRiskDetail.Add(riskDetail);
                }

            }
            Session["VehicleDetails"] = listRiskDetail;

            SummaryDetailModel summarymodel = Mapper.Map<SummaryDetail, SummaryDetailModel>(summaryDetail);
            summarymodel.Id = summaryDetail.Id;
            Session["SummaryDetailed"] = summarymodel;

        }

        public List<VehicleDetail> GetActiveVehiclesBySummaryId(int summaryId)
        {
            var summaryDetail = InsuranceContext.SummaryDetails.Single(summaryId);
            var SummaryVehicleDetails = InsuranceContext.SummaryVehicleDetails.All(where: $"SummaryDetailId={summaryId}").ToList();
            var vehicleDetail = InsuranceContext.VehicleDetails.Single(SummaryVehicleDetails[0].VehicleDetailsId);

            var list = InsuranceContext.VehicleDetails.All(where: "PolicyId=" + vehicleDetail.PolicyId + " and IsActive=1").ToList();

            return list;
        }


    }
}