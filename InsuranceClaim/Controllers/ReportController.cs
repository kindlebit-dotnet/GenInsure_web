﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using InsuranceClaim.Models;
using Insurance.Domain;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using Insurance.Service;
using System.Configuration;
using System.Diagnostics;
using System.Data;
using System.IO;
using System.ComponentModel;
using System.Text;
using System.Reflection;
using System.Globalization;
using OfficeOpenXml;
using System.Data.SqlClient;

namespace InsuranceClaim.Controllers
{
    [HandleError]
    public class ReportController : Controller
    {

        SummaryDetailService _summaryDetailService = new SummaryDetailService();
        private ApplicationUserManager _userManager;

        string CallCenterBranch = "Gene Call Centre";
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
        // GET: Report
        [Authorize(Roles = "Administrator,Reports,Finance")]
        public ActionResult ZTSCLevyReport()
        {
            //List<ZTSCLevyReportModels> listZTSCLevyreport = new List<ZTSCLevyReportModels>();
            //ListZTSCLevyReportModels _listZTSCLevyreport = new ListZTSCLevyReportModels();
            //_listZTSCLevyreport.ListZTSCreportdata = new List<ZTSCLevyReportModels>();
            //ZTSCLevyReportSeachModels model = new ZTSCLevyReportSeachModels();
            //var currencyList = _summaryDetailService.GetAllCurrency();


            //var vehicledetail = InsuranceContext.VehicleDetails.All(where: "IsActive=1").ToList();
            //foreach (var item in vehicledetail)
            //{
            //    ZTSCLevyReportModels obj = new ZTSCLevyReportModels();
            //    var policy = InsuranceContext.PolicyDetails.Single(item.PolicyId);
            //    var customer = InsuranceContext.Customers.Single(item.CustomerId);
            //    var Vehicle = InsuranceContext.VehicleDetails.Single(item.Id);

            //    obj.Currency = _summaryDetailService.GetCurrencyName(currencyList, item.CurrencyId);
            //    obj.Customer_Name = customer.FirstName + " " + customer.LastName;
            //    obj.Policy_Number = policy.PolicyNumber;
            //    obj.Premium_due = Convert.ToDecimal(item.Premium);
            //    obj.Transaction_date = Convert.ToDateTime(Vehicle.TransactionDate).ToString("dd/MM/yyy");
            //    obj.ZTSCLevy = Convert.ToDecimal(item.ZTSCLevy);

            //    listZTSCLevyreport.Add(obj);
            //}
            //model.ListZTSCreportdata = listZTSCLevyreport.OrderByDescending(x => x.Transaction_date).ToList();


            var query = "SELECT top 100 PolicyNumber, VEHICLEDETAIL.TransactionDate,Premium AS PREMIUMDUE,ZTSCLevy, FirstName AS CUSTOMERNAME,Name AS CURRENCY FROM VEHICLEDETAIL left JOIN CUSTOMER ON CUSTOMER.ID = VehicleDetail.CUSTOMERID left  JOIN POLICYDETAIL ON POLICYDETAIL.ID = VehicleDetail.POLICYID left JOIN CURRENCY ON CURRENCY.ID = VehicleDetail.CurrencyId WHERE VEHICLEDETAIL.IsActive = 1";

            ZTSCLevyReportSeachModels model = new ZTSCLevyReportSeachModels();
            List<ZTSCLevyReportModels> listZTSCLevyreport = new List<ZTSCLevyReportModels>();
            var vehicledetails = InsuranceContext.Query(query)
                           .Select(x => new ZTSCLevyReportModels
                           {
                               Policy_Number = x.PolicyNumber,
                               Customer_Name = x.CUSTOMERNAME,
                               Premium_due = x.PREMIUMDUE,
                               Transaction_date = Convert.ToDateTime(x.TransactionDate).ToString("dd/MM/yyy"),
                               ZTSCLevy = Convert.ToDecimal(x.ZTSCLevy),
                               Currency = x.CURRENCY == null ? "USD" : x.CURRENCY,
                           }).ToList();
            listZTSCLevyreport = vehicledetails;
            model.ListZTSCreportdata = listZTSCLevyreport.OrderByDescending(x => x.Transaction_date).ToList();

            return View(model);
        }

        public ActionResult InsuredVehical()
        {
            var results = new List<RiskDetailModel>();
            try
            {
                //results = (from vehicalDetials in InsuranceContext.VehicleDetails.All()
                //           join customer in InsuranceContext.Customers.All()
                //           on vehicalDetials.CustomerId equals customer.Id
                //           join make in InsuranceContext.VehicleMakes.All()
                //           on vehicalDetials.MakeId equals make.MakeCode
                //           join vehicalModel in InsuranceContext.VehicleModels.All()
                //           on vehicalDetials.ModelId equals vehicalModel.ModelCode
                //           join coverType in InsuranceContext.CoverTypes.All().ToList()
                //           on vehicalDetials.CoverTypeId equals coverType.Id
                //           join user in UserManager.Users
                //           on customer.UserID equals user.Id

                //           select new RiskDetailModel
                //           {
                //               PolicyExpireDate = vehicalDetials.CoverEndDate.Value.ToShortDateString(),
                //               CoverTypeName = coverType.Name,
                //               SumInsured = vehicalDetials.SumInsured,
                //               VechicalMake = make.MakeDescription,
                //               VechicalModel = vehicalModel.ModelDescription,
                //               VehicleYear = vehicalDetials.VehicleYear,
                //               CustomerDetails = new CustomerModel { FirstName = customer.FirstName, LastName = customer.LastName, PhoneNumber = customer.PhoneNumber, EmailAddress = user.Email }


                //           }).ToList();



                var strQuery = "select Email, customer.PhoneNumber, LastName,  FirstName, VehicleYear, ModelDescription, MakeDescription, CoverEndDate, coverType.Name, SumInsured from VehicleDetail join Customer on VehicleDetail.CustomerId=Customer.Id" +
" join VehicleMake on VehicleDetail.MakeId = VehicleMake.MakeCode " +
" join vehicleModel on VehicleDetail.ModelId = vehicleModel.ModelCode" +
" join CoverType on VehicleDetail.CoverTypeId = CoverType.Id " +
" join AspNetUsers on Customer.UserID = AspNetUsers.Id";

                results = InsuranceContext.Query(strQuery)
.Select(x => new RiskDetailModel()
{
    PolicyExpireDate = x.CoverEndDate.ToShortDateString(),
    CoverTypeName = x.Name,
    SumInsured = x.SumInsured,
    VechicalMake = x.MakeDescription,
    VechicalModel = x.ModelDescription,
    VehicleYear = x.VehicleYear,
    CustomerDetails = new CustomerModel { FirstName = x.FirstName, LastName = x.LastName, PhoneNumber = x.PhoneNumber, EmailAddress = x.Email }


}).ToList();





            }
            catch (Exception ex)
            {

            }
            return View(results);
        }

        public ActionResult SearchZtscReports(ZTSCLevyReportSeachModels Model)
        {

            List<ZTSCLevyReportModels> listZTSCLevyreport = new List<ZTSCLevyReportModels>();
            ListZTSCLevyReportModels _listZTSCLevyreport = new ListZTSCLevyReportModels();
            _listZTSCLevyreport.ListZTSCreportdata = new List<ZTSCLevyReportModels>();

            ZTSCLevyReportSeachModels model = new ZTSCLevyReportSeachModels();

            var vehicledetail = InsuranceContext.VehicleDetails.All(where: "IsActive = 1").ToList();

            DateTime fromDate = DateTime.Now.AddDays(-1);
            DateTime endDate = DateTime.Now;

            if (!string.IsNullOrEmpty(Model.FromDate) && !string.IsNullOrEmpty(Model.EndDate))
            {
                fromDate = Convert.ToDateTime(Model.FromDate);
                endDate = Convert.ToDateTime(Model.EndDate);
                ViewBag.fromdate = fromDate.ToString("dd/MM/yyyy");
                ViewBag.enddate = endDate.ToString("dd/MM/yyyy");
            }

            var currencyList = _summaryDetailService.GetAllCurrency();


            vehicledetail = vehicledetail.Where(c => c.TransactionDate >= fromDate && c.TransactionDate <= endDate).ToList();



            foreach (var item in vehicledetail)
            {
                // if (Model.FromDate ==Convert.ToString(item.TransactionDate) && Model.EndDate > Convert.ToString(item.TransactionDate))

                ZTSCLevyReportModels obj = new ZTSCLevyReportModels();
                var policy = InsuranceContext.PolicyDetails.Single(item.PolicyId);
                var customer = InsuranceContext.Customers.Single(item.CustomerId);
                var Vehicle = InsuranceContext.VehicleDetails.Single(item.Id);

                obj.Currency = _summaryDetailService.GetCurrencyName(currencyList, item.CurrencyId);
                obj.Customer_Name = customer.FirstName + " " + customer.LastName;
                obj.Policy_Number = policy.PolicyNumber;
                obj.Premium_due = Convert.ToDecimal(item.Premium);
                obj.Transaction_date = Convert.ToDateTime(Vehicle.TransactionDate).ToString("dd/MM/yyy");
                obj.ZTSCLevy = Convert.ToDecimal(item.ZTSCLevy);

                listZTSCLevyreport.Add(obj);


            }

            //_listZTSCLevyreport.ListZTSCreportdata = listZTSCLevyreport.OrderByDescending(x => x.Transaction_date).ToList();
            model.ListZTSCreportdata = listZTSCLevyreport.OrderByDescending(x => x.Transaction_date).ToList();
            return View("ZTSCLevyReport", model);
        }
        public ActionResult StampDutyReport()
        {
            List<StampDutyReportModels> ListStampDutyReport = new List<StampDutyReportModels>();
            ListStampDutyReportModels _ListStampDutyReport = new ListStampDutyReportModels();
            _ListStampDutyReport.ListStampDutyReportdata = new List<StampDutyReportModels>();
            StampDutySearchReportModels model = new StampDutySearchReportModels();

            //    var currenyList = InsuranceContext.Currencies.All();
            var vehicledetail = InsuranceContext.VehicleDetails.All(where: " IsActive = 1").ToList();

            var currenyList = _summaryDetailService.GetAllCurrency();


            foreach (var item in vehicledetail.Take(100))
            {
                StampDutyReportModels obj = new StampDutyReportModels();
                var policy = InsuranceContext.PolicyDetails.Single(item.PolicyId);
                var customer = InsuranceContext.Customers.Single(item.CustomerId);
                var Vehicle = InsuranceContext.VehicleDetails.Single(item.Id);
                obj.Currency = _summaryDetailService.GetCurrencyName(currenyList, item.CurrencyId);

                obj.Customer_Name = customer.FirstName + " " + customer.LastName;
                obj.Policy_Number = policy.PolicyNumber;
                obj.Premium_due = Convert.ToDecimal(item.Premium);
                obj.Transaction_date = Convert.ToDateTime(Vehicle.TransactionDate).ToString("dd/MM/yyy");
                obj.Stamp_duty = Convert.ToDecimal(item.StampDuty);

                ListStampDutyReport.Add(obj);
            }
            // _ListStampDutyReport.ListStampDutyReportdata = ListStampDutyReport.OrderBy(x => x.Customer_Name).ToList();
            model.ListStampDutyReportdata = ListStampDutyReport.OrderBy(x => x.Customer_Name).ToList();
            return View(model);
        }
        public ActionResult StampDutySearchReport(StampDutySearchReportModels Model)
        {
            List<StampDutyReportModels> ListStampDutyReport = new List<StampDutyReportModels>();
            ListStampDutyReportModels _ListStampDutyReport = new ListStampDutyReportModels();
            _ListStampDutyReport.ListStampDutyReportdata = new List<StampDutyReportModels>();
            StampDutySearchReportModels model = new StampDutySearchReportModels();

            var vehicledetail = InsuranceContext.VehicleDetails.All(where: "IsActive=1").ToList();


            DateTime fromDate = DateTime.Now.AddDays(-1);
            DateTime endDate = DateTime.Now;

            if (!string.IsNullOrEmpty(Model.FromDate) && !string.IsNullOrEmpty(Model.EndDate))
            {
                fromDate = Convert.ToDateTime(Model.FromDate);
                endDate = Convert.ToDateTime(Model.EndDate);
                ViewBag.fromdate = fromDate.ToString("dd/MM/yyyy");
                ViewBag.enddate = endDate.ToString("dd/MM/yyyy");
            }

            vehicledetail = vehicledetail.Where(c => c.TransactionDate >= fromDate && c.TransactionDate <= endDate).ToList();
            var currencyList = _summaryDetailService.GetAllCurrency();


            foreach (var item in vehicledetail)
            {
                StampDutyReportModels obj = new StampDutyReportModels();
                var policy = InsuranceContext.PolicyDetails.Single(item.PolicyId);
                var customer = InsuranceContext.Customers.Single(item.CustomerId);
                var Vehicle = InsuranceContext.VehicleDetails.Single(item.Id);

                obj.Customer_Name = customer.FirstName + " " + customer.LastName;
                obj.Policy_Number = policy.PolicyNumber;
                obj.Premium_due = Convert.ToDecimal(item.Premium);
                obj.Currency = _summaryDetailService.GetCurrencyName(currencyList, item.CurrencyId);

                obj.Transaction_date = Convert.ToDateTime(Vehicle.TransactionDate).ToString("dd/MM/yyy");
                obj.Stamp_duty = Convert.ToDecimal(item.StampDuty);

                ListStampDutyReport.Add(obj);
            }
            // _ListStampDutyReport.ListStampDutyReportdata = ListStampDutyReport.OrderBy(x => x.Customer_Name).ToList();
            model.ListStampDutyReportdata = ListStampDutyReport.OrderBy(x => x.Customer_Name).ToList();

            return View("StampDutyReport", model);
        }


        public ActionResult VehicleRiskAboutExpire()
        {
            List<VehicleRiskAboutExpireModels> ListVehicleRiskAboutExpire = new List<VehicleRiskAboutExpireModels>();
            ListVehicleRiskAboutExpireModels _ListVehicleRiskAboutExpire = new ListVehicleRiskAboutExpireModels();
            _ListVehicleRiskAboutExpire.ListVehicleRiskAboutExpiredata = new List<VehicleRiskAboutExpireModels>();
            VehicleRiskAboutSearchExpireModels Model = new VehicleRiskAboutSearchExpireModels();
            List<VehicleDetail> vehicledetail = new List<VehicleDetail>();

            var makeList = InsuranceContext.VehicleMakes.All();
            var modelList = InsuranceContext.VehicleModels.All();
            var currenyList = InsuranceContext.Currencies.All();


            var query = "select top 100 VehicleDetail.Id,   VehicleDetail.RegistrationNo, Customer.FirstName + ' ' + Customer.LastName as FullName, Customer.PhoneNumber , PolicyDetail.PolicyNumber,  VehicleDetail.MakeId, VehicleDetail.ModelId, ";
            query += "  VehicleDetail.CoverStartDate, VehicleDetail.CoverEndDate, VehicleDetail.SumInsured, VehicleDetail.TransactionDate, VehicleDetail.Premium, VehicleDetail.CurrencyId, VehicleMake.MakeDescription + ' ' + VehicleModel.ModelDescription as Vehicle_makeandmodel from VehicleDetail ";
            query += " join PolicyDetail on VehicleDetail.PolicyId = PolicyDetail.Id ";
            query += "  left join VehicleMake on VehicleDetail.MakeId= VehicleMake.MakeCode ";
            query += "  left join VehicleModel on VehicleDetail.ModelId=VehicleModel.ModelCode ";
            query += " join Customer on VehicleDetail.CustomerId = Customer.Id  order by  VehicleDetail.Id desc";



            ListVehicleRiskAboutExpire = InsuranceContext.Query(query)
        .Select(x => new VehicleRiskAboutExpireModels()
        {
            Customer_Name = x.FullName,
            Policy_Number = x.PhoneNumber,
            phone_number = x.PolicyNumber,
            Vehicle_makeandmodel = x.Vehicle_makeandmodel,
            Vehicle_startdate = x.CoverStartDate.ToShortDateString(),
            Vehicle_enddate = x.CoverEndDate.ToShortDateString(),
            Premium_due = x.Premium,
            Transaction_date = x.TransactionDate == null ? "" : x.TransactionDate.ToShortDateString(),
            Sum_Insured = x.SumInsured == null ? 0 : x.SumInsured,
            Currency = currenyList.FirstOrDefault(c => c.Id == x.CurrencyId) == null ? "USD" : currenyList.FirstOrDefault(c => c.Id == x.CurrencyId).Name,
            RegistrationNumber = x.RegistrationNo
        }).ToList();


            Model.ListVehicleRiskAboutExpiredata = ListVehicleRiskAboutExpire;
            return View(Model);
        }
        public ActionResult VehicleRiskAboutSearchExpire(VehicleRiskAboutSearchExpireModels _Model)
        {
            List<VehicleRiskAboutExpireModels> ListVehicleRiskAboutExpire = new List<VehicleRiskAboutExpireModels>();
            ListVehicleRiskAboutExpireModels _ListVehicleRiskAboutExpire = new ListVehicleRiskAboutExpireModels();
            _ListVehicleRiskAboutExpire.ListVehicleRiskAboutExpiredata = new List<VehicleRiskAboutExpireModels>();
            VehicleRiskAboutSearchExpireModels Model = new VehicleRiskAboutSearchExpireModels();
            List<VehicleDetail> vehicledetail = new List<VehicleDetail>();
            //VehicleRiskAboutExpireModels obj = new VehicleRiskAboutExpireModels();

            //if (Date == null)
            vehicledetail = InsuranceContext.VehicleDetails.All(where: "IsActive = 1").ToList();

            DateTime fromDate = DateTime.Now.AddDays(-1);
            DateTime endDate = DateTime.Now;

            if (!string.IsNullOrEmpty(_Model.FromDate) && !string.IsNullOrEmpty(_Model.EndDate))
            {
                fromDate = Convert.ToDateTime(_Model.FromDate);
                endDate = Convert.ToDateTime(_Model.EndDate);
                ViewBag.fromdate = fromDate.ToString("dd/MM/yyyy");
                ViewBag.enddate = endDate.ToString("dd/MM/yyyy");
            }

            //vehicledetail = vehicledetail.Where(c => c.TransactionDate >= fromDate && c.TransactionDate <= endDate).ToList();
            //vehicledetail = vehicledetail.Where(c => c.CoverStartDate >= fromDate && c.CoverEndDate <= endDate).ToList();

            //vehicledetail = vehicledetail.Where(c => c.CoverStartDate >= fromDate && c.CoverEndDate <= endDate).ToList();

            //vehicledetail = vehicledetail.Where(c => c.CoverEndDate >= endDate).ToList();

            vehicledetail = vehicledetail.Where(c => c.CoverEndDate >= fromDate && c.CoverEndDate <= endDate).ToList();
            //var VehicleDetails = InsuranceContext.VehicleDetails.All().ToList();
            var policyDetails = InsuranceContext.PolicyDetails.All().ToList();
            var customerDetails = InsuranceContext.Customers.All().ToList();


            var currencyList = _summaryDetailService.GetAllCurrency();

            //else
            //vehicledetail = InsuranceContext.VehicleDetails.All().Where(p => p.CoverEndDate.Value.ToShortDateString() == (Date == null ? DateTime.Now.ToShortDateString() : Date.Value.ToShortDateString())).ToList();
            foreach (var item in vehicledetail)
            {
                var obj = new VehicleRiskAboutExpireModels();
                var Vehicle = vehicledetail.Where(m => m.Id == item.Id).First();
                var policy = policyDetails.Where(m => m.Id == item.PolicyId).First();
                var customer = customerDetails.Where(m => m.Id == item.CustomerId).First();
                var make = InsuranceContext.VehicleMakes.Single(where: $"MakeCode='{item.MakeId}'");
                string makeDescription = "";
                string modelDescription = "";
                if (make != null)
                    makeDescription = make.MakeDescription;

                var model = InsuranceContext.VehicleModels.Single(where: $"ModelCode='{item.ModelId}'");

                if (model != null)
                    modelDescription = model.ModelDescription;


                obj.Customer_Name = customer.FirstName + " " + customer.LastName;
                obj.Policy_Number = policy.PolicyNumber;
                obj.phone_number = customer.PhoneNumber;
                obj.Vehicle_makeandmodel = makeDescription + "/" + modelDescription;
                //obj.Vehicle_startdate = Convert.ToDateTime(item.CoverStartDate).ToString("dd/MM/yyy");
                //obj.Vehicle_enddate = Convert.ToDateTime(item.CoverEndDate).ToString("dd/MM/yyy");
                obj.Vehicle_startdate = Convert.ToDateTime(item.CoverStartDate).ToString("MM/dd/yyyy");
                obj.Vehicle_enddate = Convert.ToDateTime(item.CoverEndDate).ToString("MM/dd/yyyy");
                obj.Premium_due = Convert.ToDecimal(item.Premium);
                //obj.Transaction_date = Convert.ToDateTime(Vehicle.TransactionDate).ToString("dd/MM/yyy");
                obj.Transaction_date = Convert.ToDateTime(Vehicle.TransactionDate).ToString("MM/dd/yyyy");
                obj.Sum_Insured = Convert.ToDecimal(item.SumInsured);
                obj.Currency = _summaryDetailService.GetCurrencyName(currencyList, item.CurrencyId);
                obj.RegistrationNumber = item.RegistrationNo;

                ListVehicleRiskAboutExpire.Add(obj);
            }
            //_ListVehicleRiskAboutExpire.ListVehicleRiskAboutExpiredata = ListVehicleRiskAboutExpire;
            Model.ListVehicleRiskAboutExpiredata = ListVehicleRiskAboutExpire;



            return View("VehicleRiskAboutExpire", Model);
        }


        [Authorize(Roles = "Staff,Renewals,Team Leaders,ALMSupport,Agent")]
        public ActionResult VehicleReport()
        {
            RiskDetailModel Model = new RiskDetailModel();
            ViewBag.PaymentTermId = InsuranceContext.PaymentTerms.All(where: "IsActive = 'True' or IsActive is null").ToList();

            return View(Model);
        }


        [Authorize(Roles = "Administrator,Reports,Finance")]
        public ActionResult SiteDailyReport()
        {
            List<GrossWrittenPremiumReportModels> ListGrossWrittenPremiumReport = new List<GrossWrittenPremiumReportModels>();
            ListGrossWrittenPremiumReportModels _ListGrossWrittenPremiumReport = new ListGrossWrittenPremiumReportModels();
            _ListGrossWrittenPremiumReport.ListGrossWrittenPremiumReportdata = new List<GrossWrittenPremiumReportModels>();
            GrossWrittenPremiumReportSearchModels Model = new GrossWrittenPremiumReportSearchModels();

            List<BranchModel> obj = InsuranceContext.Query("select * from Branch where id != 6").Select(x => new BranchModel
            {
                Id = x.Id,
                BranchName = x.BranchName
            }).ToList();


            var branchList = InsuranceContext.Branches.All();


            var query = " select PolicyDetail.PolicyNumber as Policy_Number, Customer.ALMId, case when Customer.ALMId is null  then  [dbo].fn_GetUserCallCenterAgent(SummaryDetail.CreatedBy) else [dbo].fn_GetUserALM(Customer.BranchId) end  as PolicyCreatedBy, Customer.FirstName + ' ' + Customer.LastName as Customer_Name,VehicleDetail.TransactionDate as Transaction_date, ";
            query += "  case when Customer.id=SummaryDetail.CreatedBy then [dbo].fn_GetUserBranch(Customer.id) else [dbo].fn_GetUserBranch(SummaryDetail.CreatedBy) end as BranchName, ";
            query += " VehicleDetail.CoverNote as CoverNoteNum, PaymentMethod.Name as Payment_Mode, PaymentTerm.Name as Payment_Term,CoverType.Name as CoverType, Currency.Name as Currency, ";
            query += " VehicleDetail.Premium + VehicleDetail.StampDuty + VehicleDetail.ZTSCLevy as Premium_due, VehicleDetail.StampDuty as Stamp_duty, VehicleDetail.ZTSCLevy as ZTSC_Levy, ";
            query += " cast(VehicleDetail.Premium * 30 / 100 as decimal(10, 2))    as Comission_Amount, VehicleDetail.IncludeRadioLicenseCost, ";
            query += " CASE WHEN IncludeRadioLicenseCost = 1 THEN VehicleDetail.RadioLicenseCost else 0 end as RadioLicenseCost, VehicleDetail.VehicleLicenceFee as Zinara_License_Fee, ";
            query += " VehicleDetail.RenewalDate as PolicyRenewalDate, VehicleDetail.IsActive, VehicleDetail.RenewPolicyNumber as RenewPolicyNumber, ";
            query += " VehicleDetail.BusinessSourceDetailId, BusinessSource.Source as BusinessSourceName, SourceDetail.FirstName + ' ' + SourceDetail.LastName as SourceDetailName from PolicyDetail ";
            query += " join Customer on PolicyDetail.CustomerId = Customer.Id ";
            query += "join Branch on Customer.BranchId = Branch.Id";
            query += "  join VehicleDetail on PolicyDetail.Id = VehicleDetail.PolicyId ";
            query += "join SummaryVehicleDetail on VehicleDetail.id = SummaryVehicleDetail.VehicleDetailsId ";
            query += " join SummaryDetail on SummaryDetail.id = SummaryVehicleDetail.SummaryDetailId ";
            query += " join PaymentMethod on SummaryDetail.PaymentMethodId = PaymentMethod.Id ";
            query += "join PaymentTerm on VehicleDetail.PaymentTermId = PaymentTerm.Id ";
            query += " left join CoverType on VehicleDetail.CoverTypeId = CoverType.Id ";
            query += " left join Currency on VehicleDetail.CurrencyId = Currency.Id ";
            query += " left join BusinessSource on BusinessSource.Id = VehicleDetail.BusinessSourceDetailId ";
            query += " left   join SourceDetail on VehicleDetail.BusinessSourceDetailId = SourceDetail.Id join AspNetUsers on AspNetUsers.id=customer.UserID join AspNetUserRoles on AspNetUserRoles.UserId=AspNetUsers.Id ";
            query += " where VehicleDetail.IsActive = 1 and SummaryDetail.isQuotation=0 and Customer.id=SummaryDetail.CreatedBy and Branch.BranchName != 'Gene Call Centre' order by  VehicleDetail.Id desc ";

            try
            {

                ListGrossWrittenPremiumReport = InsuranceContext.Query(query).
                    Select(x => new GrossWrittenPremiumReportModels()
                    {

                        Policy_Number = x.Policy_Number,
                        BranchName = x.BranchName,
                        PolicyCreatedBy = x.PolicyCreatedBy,
                        Customer_Name = x.Customer_Name,
                        Transaction_date = x.Transaction_date.ToShortDateString(),
                        CoverNoteNum = x.CoverNoteNum,
                        Payment_Mode = x.Payment_Mode,
                        Payment_Term = x.Payment_Term,
                        CoverType = x.CoverType,
                        Currency = x.Currency,
                        Premium_due = x.Premium_due,
                        Stamp_duty = x.Stamp_duty,
                        ZTSC_Levy = x.ZTSC_Levy,
                        ALMId = x.ALMId,
                        Comission_Amount = x.Comission_Amount,
                        //IncludeRadioLicenseCost = x.IncludeRadioLicenseCost,
                        RadioLicenseCost = x.RadioLicenseCost,
                        Zinara_License_Fee = x.Zinara_License_Fee,
                        PolicyRenewalDate = x.PolicyRenewalDate,
                        IsActive = x.IsActive,
                        RenewPolicyNumber = x.RenewPolicyNumber,
                        // BusinessSourceName = x.BusinessSourceName,
                        //IncludeRadioLicenseCost = x.IncludeRadioLicenseCost,
                        SourceDetailName = x.SourceDetailName,
                    }).ToList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            List<SiteDailyModel> siteDaily = new List<SiteDailyModel>();
            SiteDailySearchModel searchmodel = new SiteDailySearchModel();

            obj.ForEach(x =>
            {
                var site = new SiteDailyModel();
                var cou = ListGrossWrittenPremiumReport.FindAll(p => p.BranchName == x.BranchName).Count();
                site.BranchName = x.BranchName;
                var month = DateTime.Now.Month;
                //var month = 2;
                var year = DateTime.Now.Year;

                var i = 0;
                foreach (PropertyInfo prop in site.GetType().GetProperties())
                {
                    if (prop.Name != "BranchName")
                    {
                        i++;
                        prop.SetValue(site, ListGrossWrittenPremiumReport.FindAll(p => p.BranchName == x.BranchName && p.Transaction_date.Equals(month + "/" + i + "/" + year)).Count());
                    }
                    Debug.WriteLine(i + " " + prop.Name + " " + prop.GetValue(site));
                }
                // Debug.WriteLine(month + " " + year + " " + site.BranchName + " " + site.fourteen + " " + site.fifteen + " " + site.sixteen + " " + site.seventeen);
                siteDaily.Add(site);

                ViewBag.Months = new SelectList(Enumerable.Range(1, 12).Select(p =>
                   new SelectListItem()
                   {
                       Text = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(p),
                       Value = p.ToString()
                   }), "Value", "Text");


                ViewBag.Years = new SelectList(Enumerable.Range(DateTime.Today.Year - 19, 20).Select(y =>

                   new SelectListItem()
                   {
                       Text = y.ToString(),
                       Value = y.ToString()
                   }), "Value", "Text").OrderByDescending(b => b.Text);

            });
            searchmodel.SiteDailyList = siteDaily;
            return View(searchmodel);
        }



        [Authorize(Roles = "Administrator,Reports, Finance")]
        public ActionResult SearchSiteDailyReport(SiteDailySearchModel siteDailySearch)
        {
            List<GrossWrittenPremiumReportModels> ListGrossWrittenPremiumReport = new List<GrossWrittenPremiumReportModels>();
            ListGrossWrittenPremiumReportModels _ListGrossWrittenPremiumReport = new ListGrossWrittenPremiumReportModels();
            _ListGrossWrittenPremiumReport.ListGrossWrittenPremiumReportdata = new List<GrossWrittenPremiumReportModels>();
            GrossWrittenPremiumReportSearchModels Model = new GrossWrittenPremiumReportSearchModels();

            List<BranchModel> obj = InsuranceContext.Query("select * from Branch where id != 6").Select(x => new BranchModel
            {
                Id = x.Id,
                BranchName = x.BranchName
            }).ToList();


            var branchList = InsuranceContext.Branches.All();


            var query = " select PolicyDetail.PolicyNumber as Policy_Number, Customer.ALMId, case when Customer.ALMId is null  then  [dbo].fn_GetUserCallCenterAgent(SummaryDetail.CreatedBy) else [dbo].fn_GetUserALM(Customer.BranchId) end  as PolicyCreatedBy, Customer.FirstName + ' ' + Customer.LastName as Customer_Name,VehicleDetail.TransactionDate as Transaction_date, ";
            query += "  case when Customer.id=SummaryDetail.CreatedBy then [dbo].fn_GetUserBranch(Customer.id) else [dbo].fn_GetUserBranch(SummaryDetail.CreatedBy) end as BranchName, ";
            query += " VehicleDetail.CoverNote as CoverNoteNum, PaymentMethod.Name as Payment_Mode, PaymentTerm.Name as Payment_Term,CoverType.Name as CoverType, Currency.Name as Currency, ";
            query += " VehicleDetail.Premium + VehicleDetail.StampDuty + VehicleDetail.ZTSCLevy as Premium_due, VehicleDetail.StampDuty as Stamp_duty, VehicleDetail.ZTSCLevy as ZTSC_Levy, ";
            query += " cast(VehicleDetail.Premium * 30 / 100 as decimal(10, 2))    as Comission_Amount, VehicleDetail.IncludeRadioLicenseCost, ";
            query += " CASE WHEN IncludeRadioLicenseCost = 1 THEN VehicleDetail.RadioLicenseCost else 0 end as RadioLicenseCost, VehicleDetail.VehicleLicenceFee as Zinara_License_Fee, ";
            query += " VehicleDetail.RenewalDate as PolicyRenewalDate, VehicleDetail.IsActive, VehicleDetail.RenewPolicyNumber as RenewPolicyNumber, ";
            query += " VehicleDetail.BusinessSourceDetailId, BusinessSource.Source as BusinessSourceName, SourceDetail.FirstName + ' ' + SourceDetail.LastName as SourceDetailName from PolicyDetail ";
            query += " join Customer on PolicyDetail.CustomerId = Customer.Id ";
            query += "join Branch on Customer.BranchId = Branch.Id";
            query += "  join VehicleDetail on PolicyDetail.Id = VehicleDetail.PolicyId ";
            query += "join SummaryVehicleDetail on VehicleDetail.id = SummaryVehicleDetail.VehicleDetailsId ";
            query += " join SummaryDetail on SummaryDetail.id = SummaryVehicleDetail.SummaryDetailId ";
            query += " join PaymentMethod on SummaryDetail.PaymentMethodId = PaymentMethod.Id ";
            query += "join PaymentTerm on VehicleDetail.PaymentTermId = PaymentTerm.Id ";
            query += " left join CoverType on VehicleDetail.CoverTypeId = CoverType.Id ";
            query += " left join Currency on VehicleDetail.CurrencyId = Currency.Id ";
            query += " left join BusinessSource on BusinessSource.Id = VehicleDetail.BusinessSourceDetailId ";
            query += " left   join SourceDetail on VehicleDetail.BusinessSourceDetailId = SourceDetail.Id join AspNetUsers on AspNetUsers.id=customer.UserID join AspNetUserRoles on AspNetUserRoles.UserId=AspNetUsers.Id ";
            query += " where VehicleDetail.IsActive = 1 and SummaryDetail.isQuotation=0 and Customer.id=SummaryDetail.CreatedBy and Branch.BranchName != 'Gene Call Centre' order by  VehicleDetail.Id desc ";

            try
            {

                ListGrossWrittenPremiumReport = InsuranceContext.Query(query).
                    Select(x => new GrossWrittenPremiumReportModels()
                    {

                        Policy_Number = x.Policy_Number,
                        BranchName = x.BranchName,
                        PolicyCreatedBy = x.PolicyCreatedBy,
                        Customer_Name = x.Customer_Name,
                        Transaction_date = x.Transaction_date.ToShortDateString(),
                        CoverNoteNum = x.CoverNoteNum,
                        Payment_Mode = x.Payment_Mode,
                        Payment_Term = x.Payment_Term,
                        CoverType = x.CoverType,
                        Currency = x.Currency,
                        Premium_due = x.Premium_due,
                        Stamp_duty = x.Stamp_duty,
                        ZTSC_Levy = x.ZTSC_Levy,
                        ALMId = x.ALMId,
                        Comission_Amount = x.Comission_Amount,
                        //IncludeRadioLicenseCost = x.IncludeRadioLicenseCost,
                        RadioLicenseCost = x.RadioLicenseCost,
                        Zinara_License_Fee = x.Zinara_License_Fee,
                        PolicyRenewalDate = x.PolicyRenewalDate,
                        IsActive = x.IsActive,
                        RenewPolicyNumber = x.RenewPolicyNumber,
                        // BusinessSourceName = x.BusinessSourceName,
                        //IncludeRadioLicenseCost = x.IncludeRadioLicenseCost,
                        SourceDetailName = x.SourceDetailName,
                    }).ToList();
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

            List<SiteDailyModel> siteDaily = new List<SiteDailyModel>();
            SiteDailySearchModel searchmodel = new SiteDailySearchModel();

            obj.ForEach(x =>
            {
                var site = new SiteDailyModel();
                var cou = ListGrossWrittenPremiumReport.FindAll(p => p.BranchName == x.BranchName).Count();



                site.BranchName = x.BranchName;
                var month = siteDailySearch.SelectedMonth;
                //var month = 2;
                var year = siteDailySearch.SelectedYear;

                var i = 0;
                foreach (PropertyInfo prop in site.GetType().GetProperties())
                {
                    if (prop.Name != "BranchName")
                    {
                        i++;
                        prop.SetValue(site, ListGrossWrittenPremiumReport.FindAll(p => p.BranchName == x.BranchName && p.Transaction_date.Equals(month + "/" + i + "/" + year)).Count());
                    }
                    Debug.WriteLine(i + " " + prop.Name + " " + prop.GetValue(site));
                }
                // Debug.WriteLine(month + " " + year + " " + site.BranchName + " " + site.fourteen + " " + site.fifteen + " " + site.sixteen + " " + site.seventeen);
                siteDaily.Add(site);

                ViewBag.Months = new SelectList(Enumerable.Range(1, 12).Select(p =>
                    new SelectListItem()
                    {
                        Text = CultureInfo.CurrentCulture.DateTimeFormat.GetMonthName(p),
                        Value = p.ToString()
                    }), "Value", "Text");

                ViewBag.Years = new SelectList(Enumerable.Range(DateTime.Today.Year - 19, 20).Select(y =>

                   new SelectListItem()
                   {
                       Text = y.ToString(),
                       Value = y.ToString()
                   }), "Value", "Text").OrderByDescending(b => b.Text);

            });
            searchmodel.SiteDailyList = siteDaily;
            return View(searchmodel);
        }


        public ActionResult SummaryProductivityReport()
        {
            return View();
        }

        public ActionResult SummaryProductivityReportData()
        {
            //var ListProductiviyReport = new List<ProductiviyReportModel>();
            List<ProductiviyReportModel> listProductiviyReport = new List<ProductiviyReportModel>();
            List<UserDetails> data = new List<UserDetails>();
            List<ProductiviyReportModel> listProductiviyReport_Endorsed = new List<ProductiviyReportModel>();
            ListProductiviyReportModel _listListProductiviyReport = new ListProductiviyReportModel();
            _listListProductiviyReport.ListProductiviyReport = new List<ProductiviyReportModel>();
            ProductiviySearchReportModel model = new ProductiviySearchReportModel();
            List<UserDetails> userDetail = new List<UserDetails>();
            //   var vehicledetail = InsuranceContext.VehicleDetails.All(where: $"IsActive = 'True'or IsActive is null").OrderByDescending(c=>c.Id).ToList();

            try
            {
                // var query = "select * from AspNetUsers";
                var name = "Staff";
                var secondQuery = "select AspNetUsers.* from AspNetUsers join AspNetUserRoles on AspNetUsers.Id = AspNetUserRoles.UserId join AspNetRoles on AspNetUserRoles.RoleId = AspNetRoles.Id  where AspNetRoles.Name ='" + name + "'";

                var userSum = InsuranceContext.Query(secondQuery).Select(x => new AspNetUsersModel()
                {
                    Email = x.Email,
                    UserName = x.UserName
                }).ToList();


                foreach (var i in userSum)
                {
                    var emailUsername = i.Email;
                    var vehicleDetailsQuery = "select SummaryDetail.TotalSumInsured as Sum_Insured, AspNetUsers.Email as PolicyCreatedBy from VehicleDetail join SummaryVehicleDetail on VehicleDetail.Id = SummaryVehicleDetail.VehicleDetailsId join SummaryDetail on SummaryVehicleDetail.SummaryDetailId = SummaryDetail.Id join Customer on SummaryDetail.CreatedBy = Customer.Id join AspNetUsers on Customer.UserID = AspNetUsers.Id where VehicleDetail.IsActive = 1 and SummaryDetail.isQuotation=0 and AspNetUsers.Email ='" + emailUsername + "'";


                    var vehicleDetailsSummary = InsuranceContext.Query(vehicleDetailsQuery).
                         Select(x => new GrossWrittenPremiumReportModels()
                         {
                             Sum_Insured = x.Sum_Insured,
                             PolicyCreatedBy = x.PolicyCreatedBy

                         }).ToList();


                    UserDetails userDetails = new UserDetails();
                    var count = 0;
                    decimal sum = 0;

                    foreach (var v in vehicleDetailsSummary)
                    {
                        sum = sum + v.Sum_Insured;
                        count++;
                    }
                    userDetails.UserName = i.UserName;
                    userDetails.counts = count;
                    userDetails.SumInsured = sum;
                    userDetail.Add(userDetails);

                }
                data = userDetail;
            }
            catch (Exception ex)
            {

            }
            return Json(new { data = data }, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Administrator,Reports")]
        public ActionResult SearchManagementReports(GrossWrittenPremiumReportSearchModels _model)
        {
            try
            {
                List<GrossWrittenPremiumReportModels> ListGrossWrittenPremiumReport = new List<GrossWrittenPremiumReportModels>();
                ListGrossWrittenPremiumReportModels _ListGrossWrittenPremiumReport = new ListGrossWrittenPremiumReportModels();
                _ListGrossWrittenPremiumReport.ListGrossWrittenPremiumReportdata = new List<GrossWrittenPremiumReportModels>();
                GrossWrittenPremiumReportSearchModels Model = new GrossWrittenPremiumReportSearchModels();
                BranchModel Branch = new BranchModel();
                List<BranchModel> ListBranchModel = new List<BranchModel>();

                var branchQuery = "select Branch.BranchName, Branch.AlmId from Branch";

                ListBranchModel = InsuranceContext.Branches.Query(branchQuery).Select(x => new BranchModel()
                {
                    BranchName = x.BranchName,
                    AlmId = x.AlmId
                }).ToList();


                var query = " select PolicyDetail.PolicyNumber as Policy_Number, Customer.ALMId, case when Customer.ALMId is null  then  [dbo].fn_GetUserCallCenterAgent(SummaryDetail.CreatedBy) else Customer.ALMId end  as PolicyCreatedBy, Customer.FirstName + ' ' + Customer.LastName as Customer_Name,VehicleDetail.TransactionDate as Transaction_date, ";
                query += "  case when Customer.id=SummaryDetail.CreatedBy then [dbo].fn_GetUserBranch(Customer.id) else [dbo].fn_GetUserBranch(SummaryDetail.CreatedBy) end as BranchName, ";
                query += " VehicleDetail.CoverNote as CoverNoteNum, PaymentMethod.Name as Payment_Mode, PaymentTerm.Name as Payment_Term,CoverType.Name as CoverType, Currency.Name as Currency, ";
                query += " VehicleDetail.Premium + VehicleDetail.StampDuty + VehicleDetail.ZTSCLevy as Premium_due, VehicleDetail.StampDuty as Stamp_duty, VehicleDetail.ZTSCLevy as ZTSC_Levy, ";
                query += " cast(VehicleDetail.Premium * 30 / 100 as decimal(10, 2))    as Comission_Amount, VehicleDetail.IncludeRadioLicenseCost, ";
                query += " CASE WHEN IncludeRadioLicenseCost = 1 THEN VehicleDetail.RadioLicenseCost else 0 end as RadioLicenseCost, VehicleDetail.VehicleLicenceFee as Zinara_License_Fee, ";
                query += " VehicleDetail.RenewalDate as PolicyRenewalDate, VehicleDetail.IsActive, VehicleDetail.RenewPolicyNumber as RenewPolicyNumber, ";
                query += " VehicleDetail.BusinessSourceDetailId, BusinessSource.Source as BusinessSourceName, SourceDetail.FirstName + ' ' + SourceDetail.LastName as SourceDetailName from PolicyDetail ";
                query += " join Customer on PolicyDetail.CustomerId = Customer.Id ";
                query += " join VehicleDetail on PolicyDetail.Id = VehicleDetail.PolicyId ";
                query += "join SummaryVehicleDetail on VehicleDetail.id = SummaryVehicleDetail.VehicleDetailsId ";
                query += " join SummaryDetail on SummaryDetail.id = SummaryVehicleDetail.SummaryDetailId ";
                query += " join PaymentMethod on SummaryDetail.PaymentMethodId = PaymentMethod.Id ";
                query += "join PaymentTerm on VehicleDetail.PaymentTermId = PaymentTerm.Id ";
                query += " left join CoverType on VehicleDetail.CoverTypeId = CoverType.Id ";
                query += " left join Currency on VehicleDetail.CurrencyId = Currency.Id ";
                query += " left join BusinessSource on BusinessSource.Id = VehicleDetail.BusinessSourceDetailId ";
                query += " left   join SourceDetail on VehicleDetail.BusinessSourceDetailId = SourceDetail.Id join AspNetUsers on AspNetUsers.id=customer.UserID join AspNetUserRoles on AspNetUserRoles.UserId=AspNetUsers.Id ";
                query += " where VehicleDetail.IsActive = 1 and SummaryDetail.isQuotation=0 and (  CONVERT(date, VehicleDetail.TransactionDate) >= convert(date, '" + _model.FormDate + "', 101)  and CONVERT(date, VehicleDetail.TransactionDate) <= convert(date, '" + _model.EndDate + "', 101))";

                ListGrossWrittenPremiumReport = InsuranceContext.Query(query).
                    Select(x => new GrossWrittenPremiumReportModels()
                    {

                        Policy_Number = x.Policy_Number,
                        BranchName = x.BranchName,
                        PolicyCreatedBy = x.PolicyCreatedBy,
                        Customer_Name = x.Customer_Name,
                        Transaction_date = x.Transaction_date.ToShortDateString(),
                        CoverNoteNum = x.CoverNoteNum,
                        Payment_Mode = x.Payment_Mode,
                        Payment_Term = x.Payment_Term,
                        CoverType = x.CoverType,
                        Currency = x.Currency,
                        Premium_due = x.Premium_due == null ? 0 : Convert.ToDecimal(x.Premium_due),
                        Stamp_duty = x.Stamp_duty == null ? 0 : Convert.ToDecimal(x.Stamp_duty),
                        ZTSC_Levy = x.ZTSC_Levy == null ? 0 : Convert.ToDecimal(x.ZTSC_Levy),
                        ALMId = x.ALMId,
                        Comission_Amount = x.Comission_Amount == null ? 0 : Convert.ToDecimal(x.Comission_Amount),
                        //IncludeRadioLicenseCost = x.IncludeRadioLicenseCost,
                        RadioLicenseCost = x.RadioLicenseCost == null ? 0 : Convert.ToDecimal(x.RadioLicenseCost),
                        Zinara_License_Fee = x.Zinara_License_Fee == null ? 0 : Convert.ToDecimal(x.Zinara_License_Fee),
                        PolicyRenewalDate = x.PolicyRenewalDate,
                        IsActive = x.IsActive,
                        RenewPolicyNumber = x.RenewPolicyNumber,
                        // BusinessSourceName = x.BusinessSourceName,
                        //IncludeRadioLicenseCost = x.IncludeRadioLicenseCost,
                        SourceDetailName = x.SourceDetailName,
                    }).ToList();

                Model.ListGrossWrittenPremiumReportdata = ListGrossWrittenPremiumReport.OrderByDescending(p => p.Id).ToList();
                Model.ListBranchModelData = ListBranchModel.OrderByDescending(p => p.Id).ToList();


                return View("ManagementReport", Model);
            }

            catch (Exception)
            {
                throw;
            }
        }

        [Authorize(Roles = "Administrator,Reports")]
        public ActionResult ManagementReport()
        {
            try
            {
                List<GrossWrittenPremiumReportModels> ListGrossWrittenPremiumReport = new List<GrossWrittenPremiumReportModels>();
                ListGrossWrittenPremiumReportModels _ListGrossWrittenPremiumReport = new ListGrossWrittenPremiumReportModels();
                _ListGrossWrittenPremiumReport.ListGrossWrittenPremiumReportdata = new List<GrossWrittenPremiumReportModels>();
                GrossWrittenPremiumReportSearchModels Model = new GrossWrittenPremiumReportSearchModels();
                BranchModel Branch = new BranchModel();
                List<BranchModel> ListBranchModel = new List<BranchModel>();

                var branchQuery = "select Branch.BranchName, Branch.AlmId from Branch";
                ListBranchModel = InsuranceContext.Branches.Query(branchQuery).Select(x => new BranchModel()
                {
                    BranchName = x.BranchName,
                    AlmId = x.AlmId
                }).ToList();


                var query = " select PolicyDetail.PolicyNumber as Policy_Number, Customer.ALMId, case when Customer.ALMId is null  then  [dbo].fn_GetUserCallCenterAgent(SummaryDetail.CreatedBy) else Customer.ALMId end  as PolicyCreatedBy, Customer.FirstName + ' ' + Customer.LastName as Customer_Name,VehicleDetail.TransactionDate as Transaction_date, ";
                query += "  case when Customer.id=SummaryDetail.CreatedBy then [dbo].fn_GetUserBranch(Customer.id) else [dbo].fn_GetUserBranch(SummaryDetail.CreatedBy) end as BranchName, ";
                query += " VehicleDetail.CoverNote as CoverNoteNum, PaymentMethod.Name as Payment_Mode, PaymentTerm.Name as Payment_Term,CoverType.Name as CoverType, Currency.Name as Currency, ";
                query += " VehicleDetail.Premium + VehicleDetail.StampDuty + VehicleDetail.ZTSCLevy as Premium_due, VehicleDetail.StampDuty as Stamp_duty, VehicleDetail.ZTSCLevy as ZTSC_Levy, ";
                query += " cast(VehicleDetail.Premium * 30 / 100 as decimal(10, 2))    as Comission_Amount, VehicleDetail.IncludeRadioLicenseCost, ";
                query += " CASE WHEN IncludeRadioLicenseCost = 1 THEN VehicleDetail.RadioLicenseCost else 0 end as RadioLicenseCost, VehicleDetail.VehicleLicenceFee as Zinara_License_Fee, ";
                query += " VehicleDetail.RenewalDate as PolicyRenewalDate, VehicleDetail.IsActive, VehicleDetail.RenewPolicyNumber as RenewPolicyNumber, ";
                query += " VehicleDetail.BusinessSourceDetailId, BusinessSource.Source as BusinessSourceName, SourceDetail.FirstName + ' ' + SourceDetail.LastName as SourceDetailName from PolicyDetail ";
                query += " join Customer on PolicyDetail.CustomerId = Customer.Id ";
                query += " join VehicleDetail on PolicyDetail.Id = VehicleDetail.PolicyId ";
                query += "join SummaryVehicleDetail on VehicleDetail.id = SummaryVehicleDetail.VehicleDetailsId ";
                query += " join SummaryDetail on SummaryDetail.id = SummaryVehicleDetail.SummaryDetailId ";
                query += " join PaymentMethod on SummaryDetail.PaymentMethodId = PaymentMethod.Id ";
                query += "join PaymentTerm on VehicleDetail.PaymentTermId = PaymentTerm.Id ";
                query += " left join CoverType on VehicleDetail.CoverTypeId = CoverType.Id ";
                query += " left join Currency on VehicleDetail.CurrencyId = Currency.Id ";
                query += " left join BusinessSource on BusinessSource.Id = VehicleDetail.BusinessSourceDetailId ";
                query += " left   join SourceDetail on VehicleDetail.BusinessSourceDetailId = SourceDetail.Id join AspNetUsers on AspNetUsers.id=customer.UserID join AspNetUserRoles on AspNetUserRoles.UserId=AspNetUsers.Id ";
                query += " where VehicleDetail.IsActive = 1 and SummaryDetail.isQuotation=0  order by  VehicleDetail.Id desc ";

                ListGrossWrittenPremiumReport = InsuranceContext.Query(query).
                    Select(x => new GrossWrittenPremiumReportModels()
                    {

                        Policy_Number = x.Policy_Number,
                        BranchName = x.BranchName,
                        PolicyCreatedBy = x.PolicyCreatedBy,
                        Customer_Name = x.Customer_Name,
                        Transaction_date = x.Transaction_date.ToShortDateString(),
                        CoverNoteNum = x.CoverNoteNum,
                        Payment_Mode = x.Payment_Mode,
                        Payment_Term = x.Payment_Term,
                        CoverType = x.CoverType,
                        Currency = x.Currency,
                        Premium_due = x.Premium_due == null ? 0 : Convert.ToDecimal(x.Premium_due),
                        Stamp_duty = x.Stamp_duty == null ? 0 : Convert.ToDecimal(x.Stamp_duty),
                        ZTSC_Levy = x.ZTSC_Levy == null ? 0 : Convert.ToDecimal(x.ZTSC_Levy),
                        ALMId = x.ALMId,
                        Comission_Amount = x.Comission_Amount == null ? 0 : Convert.ToDecimal(x.Comission_Amount),
                        //IncludeRadioLicenseCost = x.IncludeRadioLicenseCost,
                        RadioLicenseCost = x.RadioLicenseCost == null ? 0 : Convert.ToDecimal(x.RadioLicenseCost),
                        Zinara_License_Fee = x.Zinara_License_Fee == null ? 0 : Convert.ToDecimal(x.Zinara_License_Fee),
                        PolicyRenewalDate = x.PolicyRenewalDate,
                        IsActive = x.IsActive,
                        RenewPolicyNumber = x.RenewPolicyNumber,
                        // BusinessSourceName = x.BusinessSourceName,
                        //IncludeRadioLicenseCost = x.IncludeRadioLicenseCost,
                        SourceDetailName = x.SourceDetailName,
                    }).ToList();

                Model.ListGrossWrittenPremiumReportdata = ListGrossWrittenPremiumReport.OrderByDescending(p => p.Id).ToList();
                Model.ListBranchModelData = ListBranchModel.OrderByDescending(p => p.Id).ToList();


                return View(Model);
            }

            catch (Exception)
            {
                throw;
            }
        }

        [Authorize(Roles = "Administrator,Reports,Finance")]
        public ActionResult ALMSearchGrossReports(GrossWrittenPremiumReportSearchModels _model)
        {
            IEnumerable<Branch> objBranch = InsuranceContext.Branches.All();
            List<BranchModel> obj = InsuranceContext.Query("select * from Branch where id != 6").Select(x => new BranchModel
            {
                Id = x.Id,
                BranchName = x.BranchName
            }).ToList();


            MultiSelectList selectListItems = new MultiSelectList(obj, "Id", "BranchName");

            ViewBag.Branch = selectListItems;

            List<GrossWrittenPremiumReportModels> ListGrossWrittenPremiumReport = new List<GrossWrittenPremiumReportModels>();
            ListGrossWrittenPremiumReportModels _ListGrossWrittenPremiumReport = new ListGrossWrittenPremiumReportModels();
            _ListGrossWrittenPremiumReport.ListGrossWrittenPremiumReportdata = new List<GrossWrittenPremiumReportModels>();

            var newList = new List<GrossWrittenPremiumReportModels>();

            var paymentInformationList = InsuranceContext.PaymentInformations.All().ToList();

            var db = _model.BranchId;
            /*            foreach (var item in db)
                        {
                            Debug.WriteLine(item);
                        }
                        Debug.WriteLine(db);*/
            var query = "";

            if (db == null)
            {
                query = " select  PolicyDetail.PolicyNumber as Policy_Number, SummaryDetail.Id as SummaryDetailId, Customer.ALMId, case when Customer.ALMId is null  then  [dbo].fn_GetUserCallCenterAgent(SummaryDetail.CreatedBy) else [dbo].fn_GetUserALM(Customer.BranchId) end  as PolicyCreatedBy, Customer.FirstName + ' ' + Customer.LastName as Customer_Name,VehicleDetail.TransactionDate as Transaction_date, ";
                query += "  case when Customer.id=SummaryDetail.CreatedBy then [dbo].fn_GetUserBranch(Customer.id) else [dbo].fn_GetUserBranch(SummaryDetail.CreatedBy) end as BranchName, ";
                query += " VehicleDetail.RegistrationNo, VehicleDetail.CoverNote as CoverNoteNum, PaymentMethod.Name as Payment_Mode, PaymentTerm.Name as Payment_Term,CoverType.Name as CoverType, Currency.Name as Currency, ";
                query += " VehicleDetail.Premium + VehicleDetail.StampDuty + VehicleDetail.ZTSCLevy + VehicleDetail.VehicleLicenceFee + COALESCE(CAST(VehicleDetail.RadioLicenseCost as DECIMAL(9,2)),0) as Premium_due, VehicleDetail.StampDuty as Stamp_duty, VehicleDetail.ZTSCLevy as ZTSC_Levy, ";
                query += " cast(VehicleDetail.Premium * 30 / 100 as decimal(10, 2))    as Comission_Amount, VehicleDetail.IncludeRadioLicenseCost, ";
                query += " CASE WHEN IncludeRadioLicenseCost = 1 THEN VehicleDetail.RadioLicenseCost else 0 end as RadioLicenseCost, VehicleDetail.VehicleLicenceFee as Zinara_License_Fee, ";
                query += " VehicleDetail.RenewalDate as PolicyRenewalDate, VehicleDetail.IsActive, VehicleDetail.RenewPolicyNumber as RenewPolicyNumber, ";
                query += " VehicleDetail.BusinessSourceDetailId, BusinessSource.Source as BusinessSourceName, SourceDetail.FirstName + ' ' + SourceDetail.LastName as SourceDetailName from PolicyDetail ";
                query += " join Customer on PolicyDetail.CustomerId = Customer.Id ";
                query += "join Branch on Customer.BranchId = Branch.Id";
                query += "  join VehicleDetail on PolicyDetail.Id = VehicleDetail.PolicyId ";
                query += "join SummaryVehicleDetail on VehicleDetail.id = SummaryVehicleDetail.VehicleDetailsId ";
                query += " join SummaryDetail on SummaryDetail.id = SummaryVehicleDetail.SummaryDetailId ";
                query += " join PaymentMethod on SummaryDetail.PaymentMethodId = PaymentMethod.Id ";
                query += "join PaymentTerm on VehicleDetail.PaymentTermId = PaymentTerm.Id ";
                query += " left join CoverType on VehicleDetail.CoverTypeId = CoverType.Id ";
                query += " left join Currency on VehicleDetail.CurrencyId = Currency.Id ";
                query += " left join BusinessSource on BusinessSource.Id = VehicleDetail.BusinessSourceDetailId ";
                query += " left   join SourceDetail on VehicleDetail.BusinessSourceDetailId = SourceDetail.Id join AspNetUsers on AspNetUsers.id=customer.UserID join AspNetUserRoles on AspNetUserRoles.UserId=AspNetUsers.Id ";
                query += " where VehicleDetail.IsActive = 1 and SummaryDetail.isQuotation=0 and Customer.id=SummaryDetail.CreatedBy and Branch.BranchName != 'Gene Call Centre' and (  CONVERT(date, VehicleDetail.TransactionDate) >= convert(date, '" + _model.FormDate + "', 101)  and CONVERT(date, VehicleDetail.TransactionDate) <= convert(date, '" + _model.EndDate + "', 101))  order by  VehicleDetail.Id desc ";

            }
            else
            {
                query = " select  PolicyDetail.PolicyNumber as Policy_Number, SummaryDetail.Id as SummaryDetailId, Customer.ALMId, case when Customer.ALMId is null  then  [dbo].fn_GetUserCallCenterAgent(SummaryDetail.CreatedBy) else [dbo].fn_GetUserALM(Customer.BranchId) end  as PolicyCreatedBy, Customer.FirstName + ' ' + Customer.LastName as Customer_Name,VehicleDetail.TransactionDate as Transaction_date, ";
                query += "  case when Customer.id=SummaryDetail.CreatedBy then [dbo].fn_GetUserBranch(Customer.id) else [dbo].fn_GetUserBranch(SummaryDetail.CreatedBy) end as BranchName, ";
                query += " VehicleDetail.RegistrationNo, VehicleDetail.CoverNote as CoverNoteNum, PaymentMethod.Name as Payment_Mode, PaymentTerm.Name as Payment_Term,CoverType.Name as CoverType, Currency.Name as Currency, ";
                query += " VehicleDetail.Premium + VehicleDetail.StampDuty + VehicleDetail.ZTSCLevy + VehicleDetail.VehicleLicenceFee + COALESCE(CAST(VehicleDetail.RadioLicenseCost as DECIMAL(9,2)),0) as Premium_due, VehicleDetail.StampDuty as Stamp_duty, VehicleDetail.ZTSCLevy as ZTSC_Levy, ";
                query += " cast(VehicleDetail.Premium * 30 / 100 as decimal(10, 2))    as Comission_Amount, VehicleDetail.IncludeRadioLicenseCost, ";
                query += " CASE WHEN IncludeRadioLicenseCost = 1 THEN VehicleDetail.RadioLicenseCost else 0 end as RadioLicenseCost, VehicleDetail.VehicleLicenceFee as Zinara_License_Fee, ";
                query += " VehicleDetail.RenewalDate as PolicyRenewalDate, VehicleDetail.IsActive, VehicleDetail.RenewPolicyNumber as RenewPolicyNumber, ";
                query += " VehicleDetail.BusinessSourceDetailId, BusinessSource.Source as BusinessSourceName, SourceDetail.FirstName + ' ' + SourceDetail.LastName as SourceDetailName from PolicyDetail ";
                query += " join Customer on PolicyDetail.CustomerId = Customer.Id ";
                query += "join Branch on Customer.BranchId = Branch.Id";
                query += "  join VehicleDetail on PolicyDetail.Id = VehicleDetail.PolicyId ";
                query += "join SummaryVehicleDetail on VehicleDetail.id = SummaryVehicleDetail.VehicleDetailsId ";
                query += " join SummaryDetail on SummaryDetail.id = SummaryVehicleDetail.SummaryDetailId ";
                query += " join PaymentMethod on SummaryDetail.PaymentMethodId = PaymentMethod.Id ";
                query += "join PaymentTerm on VehicleDetail.PaymentTermId = PaymentTerm.Id ";
                query += " left join CoverType on VehicleDetail.CoverTypeId = CoverType.Id ";
                query += " left join Currency on VehicleDetail.CurrencyId = Currency.Id ";
                query += " left join BusinessSource on BusinessSource.Id = VehicleDetail.BusinessSourceDetailId ";
                query += " left join SourceDetail on VehicleDetail.BusinessSourceDetailId = SourceDetail.Id join AspNetUsers on AspNetUsers.id=customer.UserID join AspNetUserRoles on AspNetUserRoles.UserId=AspNetUsers.Id ";
                query += " where VehicleDetail.IsActive = 1 and SummaryDetail.isQuotation=0 ";
                query += " and  Branch.Id in (";
                var last = db.Last();
                foreach (var item in db)
                {

                    if (item.Equals(last))
                    {
                        // do something different with the last item
                        query += "" + item + "";
                    }
                    else
                    {
                        // do something different with every item but the last
                        query += "" + item + ",";
                    }
                }
                query += " )";
                query += " and Customer.id=SummaryDetail.CreatedBy and Branch.BranchName != 'Gene Call Centre' and (  CONVERT(date, VehicleDetail.TransactionDate) >= convert(date, '" + _model.FormDate + "', 101)  and CONVERT(date, VehicleDetail.TransactionDate) <= convert(date, '" + _model.EndDate + "', 101))";
                query += "  order by  VehicleDetail.Id desc ";
            }



            ListGrossWrittenPremiumReport = InsuranceContext.Query(query).
                     Select(x => new GrossWrittenPremiumReportModels()
                     {

                         Policy_Number = x.Policy_Number,
                         SummaryDetailId = x.SummaryDetailId,
                         BranchName = x.BranchName,
                         PolicyCreatedBy = x.PolicyCreatedBy,
                         Customer_Name = x.Customer_Name,
                         Transaction_date = x.Transaction_date.ToShortDateString(),
                         CoverNoteNum = x.CoverNoteNum,
                         Payment_Mode = x.Payment_Mode,
                         Payment_Term = x.Payment_Term,
                         CoverType = x.CoverType,
                         Currency = x.Currency,
                         Premium_due = x.Premium_due,
                         Stamp_duty = x.Stamp_duty,
                         ZTSC_Levy = x.ZTSC_Levy,
                         ALMId = x.ALMId,
                         Comission_Amount = x.Comission_Amount,
                         //IncludeRadioLicenseCost = x.IncludeRadioLicenseCost,
                         RadioLicenseCost = x.RadioLicenseCost == null ? 0 : Convert.ToDecimal(x.RadioLicenseCost),
                         Zinara_License_Fee = x.Zinara_License_Fee,
                         PolicyRenewalDate = x.PolicyRenewalDate,
                         IsActive = x.IsActive,
                         RenewPolicyNumber = x.RenewPolicyNumber,
                         // BusinessSourceName = x.BusinessSourceName,
                         //IncludeRadioLicenseCost = x.IncludeRadioLicenseCost,
                         SourceDetailName = x.SourceDetailName,
                         RegistrationNo= x.RegistrationNo
                     }).ToList();

            foreach (var item in ListGrossWrittenPremiumReport)
            {
                var paymentDetail = paymentInformationList.Where(c => c.SummaryDetailId == item.SummaryDetailId);
                if (paymentDetail == null)
                    continue;

                newList.Add(item);
            }


            GrossWrittenPremiumReportSearchModels Model = new GrossWrittenPremiumReportSearchModels();


            // Model.ListGrossWrittenPremiumReportdata = ListGrossWrittenPremiumReport.OrderByDescending(p => p.Transaction_date).ToList();
            Model.ListGrossWrittenPremiumReportdata = newList.OrderByDescending(p => p.Transaction_date).ToList();

            return View("ALMGWPReport", Model);
        }

        [Authorize(Roles = "Administrator,Reports,Finance")]
        public ActionResult ALMGWPReport()
        {
            List<GrossWrittenPremiumReportModels> ListGrossWrittenPremiumReport = new List<GrossWrittenPremiumReportModels>();
            ListGrossWrittenPremiumReportModels _ListGrossWrittenPremiumReport = new ListGrossWrittenPremiumReportModels();
            _ListGrossWrittenPremiumReport.ListGrossWrittenPremiumReportdata = new List<GrossWrittenPremiumReportModels>();
            GrossWrittenPremiumReportSearchModels Model = new GrossWrittenPremiumReportSearchModels();

            //IEnumerable<Branch> branches = InsuranceContext.Branches.All();

            List<BranchModel> obj = InsuranceContext.Query("select * from Branch where id != 6").Select(x => new BranchModel
            {
                Id = x.Id,
                BranchName = x.BranchName
            }).ToList();

            MultiSelectList selectListItems = new MultiSelectList(obj, "Id", "BranchName");

            ViewBag.Branch = selectListItems;

            var branchList = InsuranceContext.Branches.All();

            List<GrossWrittenPremiumReportModels> newList = new List<GrossWrittenPremiumReportModels>();

            var paymentInformationList = InsuranceContext.PaymentInformations.All().ToList();


            var query = " select top 100 PolicyDetail.PolicyNumber as Policy_Number, SummaryDetail.Id as SummaryDetailId, Customer.ALMId, case when Customer.ALMId is null  then  [dbo].fn_GetUserCallCenterAgent(SummaryDetail.CreatedBy) else [dbo].fn_GetUserALM(Customer.BranchId) end  as PolicyCreatedBy, Customer.FirstName + ' ' + Customer.LastName as Customer_Name,VehicleDetail.TransactionDate as Transaction_date, ";
            query += "  case when Customer.id=SummaryDetail.CreatedBy then [dbo].fn_GetUserBranch(Customer.id) else [dbo].fn_GetUserBranch(SummaryDetail.CreatedBy) end as BranchName, ";
            query += " VehicleDetail.RegistrationNo, VehicleDetail.CoverNote as CoverNoteNum, PaymentMethod.Name as Payment_Mode, PaymentTerm.Name as Payment_Term,CoverType.Name as CoverType, Currency.Name as Currency, ";
            query += " VehicleDetail.Premium + VehicleDetail.StampDuty + VehicleDetail.ZTSCLevy + VehicleDetail.VehicleLicenceFee + COALESCE(CAST(VehicleDetail.RadioLicenseCost as DECIMAL(9,2)),0)   as Premium_due, VehicleDetail.StampDuty as Stamp_duty, VehicleDetail.ZTSCLevy as ZTSC_Levy, ";
            query += " cast(VehicleDetail.Premium * 30 / 100 as decimal(10, 2))    as Comission_Amount, VehicleDetail.IncludeRadioLicenseCost, ";
            query += " CASE WHEN IncludeRadioLicenseCost = 1 THEN VehicleDetail.RadioLicenseCost else 0 end as RadioLicenseCost, VehicleDetail.VehicleLicenceFee as Zinara_License_Fee, ";
            query += " VehicleDetail.RenewalDate as PolicyRenewalDate, VehicleDetail.IsActive, VehicleDetail.RenewPolicyNumber as RenewPolicyNumber, ";
            query += " VehicleDetail.BusinessSourceDetailId, BusinessSource.Source as BusinessSourceName, SourceDetail.FirstName + ' ' + SourceDetail.LastName as SourceDetailName from PolicyDetail ";
            query += " join Customer on PolicyDetail.CustomerId = Customer.Id ";
            query += "join Branch on Customer.BranchId = Branch.Id";
            query += "  join VehicleDetail on PolicyDetail.Id = VehicleDetail.PolicyId ";
            query += "join SummaryVehicleDetail on VehicleDetail.id = SummaryVehicleDetail.VehicleDetailsId ";
            query += " join SummaryDetail on SummaryDetail.id = SummaryVehicleDetail.SummaryDetailId ";
            query += " join PaymentMethod on SummaryDetail.PaymentMethodId = PaymentMethod.Id ";
            query += "join PaymentTerm on VehicleDetail.PaymentTermId = PaymentTerm.Id ";
            query += " left join CoverType on VehicleDetail.CoverTypeId = CoverType.Id ";
            query += " left join Currency on VehicleDetail.CurrencyId = Currency.Id ";
            query += " left join BusinessSource on BusinessSource.Id = VehicleDetail.BusinessSourceDetailId ";
            query += " left   join SourceDetail on VehicleDetail.BusinessSourceDetailId = SourceDetail.Id join AspNetUsers on AspNetUsers.id=customer.UserID join AspNetUserRoles on AspNetUserRoles.UserId=AspNetUsers.Id ";
            query += " where VehicleDetail.IsActive = 1 and SummaryDetail.isQuotation=0 and Customer.id=SummaryDetail.CreatedBy and Branch.BranchName != 'Gene Call Centre' order by  VehicleDetail.Id desc ";



            ListGrossWrittenPremiumReport = InsuranceContext.Query(query).
                Select(x => new GrossWrittenPremiumReportModels()
                {

                    Policy_Number = x.Policy_Number,
                    SummaryDetailId = x.SummaryDetailId,
                    BranchName = x.BranchName,
                    PolicyCreatedBy = x.PolicyCreatedBy,
                    Customer_Name = x.Customer_Name,
                    Transaction_date = x.Transaction_date.ToShortDateString(),
                    CoverNoteNum = x.CoverNoteNum,
                    Payment_Mode = x.Payment_Mode,
                    Payment_Term = x.Payment_Term,
                    CoverType = x.CoverType,
                    Currency = x.Currency,
                    Premium_due = x.Premium_due,
                    Stamp_duty = x.Stamp_duty,
                    ZTSC_Levy = x.ZTSC_Levy,
                    ALMId = x.ALMId,
                    Comission_Amount = x.Comission_Amount,
                    //IncludeRadioLicenseCost = x.IncludeRadioLicenseCost,
                    RadioLicenseCost = x.RadioLicenseCost == null ? 0 : Convert.ToDecimal(x.RadioLicenseCost),
                    Zinara_License_Fee = x.Zinara_License_Fee,
                    PolicyRenewalDate = x.PolicyRenewalDate,
                    IsActive = x.IsActive,
                    RenewPolicyNumber = x.RenewPolicyNumber,
                    // BusinessSourceName = x.BusinessSourceName,
                    //IncludeRadioLicenseCost = x.IncludeRadioLicenseCost,
                    SourceDetailName = x.SourceDetailName,
                    RegistrationNo= x.RegistrationNo
                }).ToList();


            foreach (var item in ListGrossWrittenPremiumReport)
            {
                var paymentDetail = paymentInformationList.Where(c => c.SummaryDetailId == item.SummaryDetailId);
                if (paymentDetail == null)
                    continue;

                newList.Add(item);
            }

            // Model.ListGrossWrittenPremiumReportdata = ListGrossWrittenPremiumReport.OrderByDescending(p => p.Transaction_date).ToList();

            Model.ListGrossWrittenPremiumReportdata = newList.OrderByDescending(p => p.Transaction_date).ToList();

            return View(Model);
        }

        [Authorize(Roles = "Administrator,Reports,Finance,Team Leaders")]
        public ActionResult GrossWrittenPremiumReport()
        {
            var includecancelledpolicies = false;
            TempData["iscancel"] = "false";
            var iscancel = System.Web.HttpContext.Current.Request["iscancelled"];
            if (iscancel != null && iscancel.ToLower() == "true")
            {
                includecancelledpolicies = true;
                TempData["iscancel"] = "true";
            }

            //  GetAllGwpReport();

            List<GrossWrittenPremiumReportModels> ListGrossWrittenPremiumReport = new List<GrossWrittenPremiumReportModels>();
            ListGrossWrittenPremiumReportModels _ListGrossWrittenPremiumReport = new ListGrossWrittenPremiumReportModels();
            _ListGrossWrittenPremiumReport.ListGrossWrittenPremiumReportdata = new List<GrossWrittenPremiumReportModels>();

            GrossWrittenPremiumReportSearchModels Model = new GrossWrittenPremiumReportSearchModels();

           // var branchList = InsuranceContext.Branches.All(where: "Status=0");
            var paymentInformationsList = InsuranceContext.PaymentInformations.All();
           
            ViewBag.ReportList = ReportsList();
            ViewBag.PaymentMethod = PaymentMethodsList();

            //  var recieptList = InsuranceContext.ReceiptHistorys.All().ToList();

            // Customer.ALMId is null replace condition with SummaryDetail.CreatedBy is not null
            //var query = " select top 100 SummaryDetail.PaymentMethodId, PolicyDetail.Id as policyId, PolicyDetail.PolicyNumber as Policy_Number, Customer.ALMId, case when VehicleDetail.ALMBranchId = 0  then  [dbo].fn_GetUserCallCenterAgent(SummaryDetail.CreatedBy) else [dbo].fn_GetUserALM(VehicleDetail.ALMBranchId) end  as PolicyCreatedBy, Customer.FirstName + ' ' + Customer.LastName as Customer_Name,VehicleDetail.TransactionDate as Transaction_date, ";
            //query += "  case when Customer.id=SummaryDetail.CreatedBy then [dbo].fn_GetUserBranch(Customer.id) else [dbo].fn_GetUserBranch(SummaryDetail.CreatedBy) end as BranchName, ";
            //query += " VehicleDetail.CoverNote as CoverNoteNum,VehicleDetail.AdministrationAmt, PaymentMethod.Name as Payment_Mode, PaymentTerm.Name as Payment_Term,CoverType.Name as CoverType, Currency.Name as Currency, ";
            //query += " VehicleDetail.Premium  as Premium_due, VehicleDetail.StampDuty as Stamp_duty, VehicleDetail.ZTSCLevy as ZTSC_Levy, ";
            //query += " cast(VehicleDetail.Premium * 30 / 100 as decimal(10, 2))    as Comission_Amount,RegistrationNo, VehicleDetail.IncludeRadioLicenseCost, ";
            //query += " CASE WHEN IncludeRadioLicenseCost = 1 THEN VehicleDetail.RadioLicenseCost else 0 end as RadioLicenseCost, VehicleDetail.VehicleLicenceFee as Zinara_License_Fee, ";
            //query += " VehicleDetail.RenewalDate as PolicyRenewalDate, VehicleDetail.IsActive, VehicleDetail.RenewPolicyNumber as RenewPolicyNumber, ";
            //query += " VehicleDetail.BusinessSourceDetailId,SummaryDetail.id as SummaryDetailId, BusinessSource.Source as BusinessSourceName, SourceDetail.FirstName + ' ' + SourceDetail.LastName as SourceDetailName, VehicleDetail.SumInsured from PolicyDetail ";
            //query += " join Customer on PolicyDetail.CustomerId = Customer.Id ";
            //query += "  join VehicleDetail on PolicyDetail.Id = VehicleDetail.PolicyId ";
            //query += "join SummaryVehicleDetail on VehicleDetail.id = SummaryVehicleDetail.VehicleDetailsId ";
            //query += " join SummaryDetail on SummaryDetail.id = SummaryVehicleDetail.SummaryDetailId ";
            //// query += "  join PaymentInformation on SummaryDetail.Id=PaymentInformation.SummaryDetailId ";
            //query += " left join PaymentMethod on SummaryDetail.PaymentMethodId = PaymentMethod.Id ";
            //query += "join PaymentTerm on VehicleDetail.PaymentTermId = PaymentTerm.Id ";
            //query += " left join CoverType on VehicleDetail.CoverTypeId = CoverType.Id ";
            //query += " left join Currency on VehicleDetail.CurrencyId = Currency.Id ";
            //query += " left join BusinessSource on BusinessSource.Id = VehicleDetail.BusinessSourceDetailId ";
            //query += " left   join SourceDetail on VehicleDetail.BusinessSourceDetailId = SourceDetail.Id join AspNetUsers on AspNetUsers.id=customer.UserID join AspNetUserRoles on AspNetUserRoles.UserId=AspNetUsers.Id ";
            //// query += " left join Branch on VehicleDetail.ALMBranchId=Branch.Id ";
            //query += " where (VehicleDetail.IsActive = 1 or VehicleDetail.IsActive = null)  and SummaryDetail.isQuotation=0   order by  VehicleDetail.Id desc ";


            //ListGrossWrittenPremiumReport = InsuranceContext.Query(query).
            //    Select(x => new GrossWrittenPremiumReportModels()
            //    {
            //        PaymentMethodId = x.PaymentMethodId,
            //        Policy_Number = x.Policy_Number,
            //        RegistrationNo = x.RegistrationNo,
            //        BranchName = x.BranchName,
            //        PolicyCreatedBy = x.PolicyCreatedBy,
            //        Customer_Name = x.Customer_Name,
            //        Transaction_date = x.Transaction_date.ToShortDateString(),
            //        CoverNoteNum = x.CoverNoteNum,
            //        Payment_Mode = x.Payment_Mode == null ? "Cash" : x.Payment_Mode,
            //        Payment_Term = x.Payment_Term,
            //        CoverType = x.CoverType,
            //        Currency = x.Currency,
            //        Premium_due = x.Premium_due == null ? 0 : x.Premium_due,
            //        //GrandPremium = x.Premium_due + x.Zinara_License_Fee +
            //        Stamp_duty = x.Stamp_duty == null ? 0 : x.Stamp_duty,
            //        ZTSC_Levy = x.ZTSC_Levy == null ? 0 : x.ZTSC_Levy,
            //        ALMId = x.ALMId,
            //        Comission_Amount = x.Comission_Amount == null ? 0 : x.Comission_Amount,
            //        //IncludeRadioLicenseCost = x.IncludeRadioLicenseCost,
            //        RadioLicenseCost = x.RadioLicenseCost == null ? 0 : x.RadioLicenseCost,
            //        Zinara_License_Fee = x.Zinara_License_Fee == null ? 0 : x.Zinara_License_Fee,
            //        PolicyRenewalDate = x.PolicyRenewalDate,
            //        IsActive = x.IsActive,
            //        RenewPolicyNumber = x.RenewPolicyNumber,
            //        BusinessSourceName = x.BusinessSourceName,
            //        //IncludeRadioLicenseCost = x.IncludeRadioLicenseCost,
            //        SourceDetailName = x.SourceDetailName,
            //        SummaryDetailId = x.SummaryDetailId,
            //        Sum_Insured = x.SumInsured == null ? 0 : Convert.ToDecimal(x.SumInsured),
            //        AdministrationAmt = x.AdministrationAmt == null ? 0 : Convert.ToDecimal(x.AdministrationAmt),
            //        ReceiptNumber = RecieptNumber(x.policyId, x.RenewPolicyNumber, recieptList),
            //    }).ToList();

           
                ListGrossWrittenPremiumReport = GetAllGwpReport();
            

            


            List<GrossWrittenPremiumReportModels> list = new List<GrossWrittenPremiumReportModels>();

            foreach (var item in ListGrossWrittenPremiumReport)
            {
                var paymentDetail = paymentInformationsList.Where(c => c.SummaryDetailId == item.SummaryDetailId);
                if (paymentDetail == null)
                    continue;

                //var branchDetial = branchList.FirstOrDefault(c => c.Id == item.ALMBranchId);
                //if(branchDetial!=null)
                //    continue;




                GrossWrittenPremiumReportModels model = new GrossWrittenPremiumReportModels();
                model.Policy_Number = item.Policy_Number;
                model.RegistrationNo = item.RegistrationNo;
                model.BranchName = item.BranchName;
                model.PolicyCreatedBy = item.PolicyCreatedBy;
                model.Customer_Name = item.Customer_Name;
                model.Transaction_date = item.Transaction_date;
                model.CoverNoteNum = item.CoverNoteNum;
                model.Payment_Mode = item.Payment_Mode;
                model.Payment_Term = item.Payment_Term;
                model.CoverType = item.CoverType;
                model.Currency = item.Currency;
                model.RenewPolicyNumber = item.RenewPolicyNumber;
                model.ALMId = item.ALMId;
                model.IsActive = item.IsActive;
                model.PolicyRenewalDate = item.PolicyRenewalDate;
                model.SourceDetailName = item.SourceDetailName;
                model.BusinessSourceName = item.BusinessSourceName;
                model.PaymentMethodId = item.PaymentMethodId;
                model.ReceiptNumber = item.ReceiptNumber;
                model.AdministrationAmt = item.AdministrationAmt;
                //IncludeRadioLicenseCost = x.IncludeRadioLicenseCost,

                var index = list.FindIndex(c => c.Policy_Number == item.Policy_Number);
                if (index != -1)
                {
                    list[index].Premium_due += item.Premium_due;
                    list[index].GrandPremium = item.Premium_due + item.Zinara_License_Fee.Value + item.RadioLicenseCost.Value + item.ZTSC_Levy + item.Stamp_duty;
                    list[index].Stamp_duty += item.Stamp_duty;
                    list[index].ZTSC_Levy += item.ZTSC_Levy;
                    list[index].Comission_Amount += item.Comission_Amount;
                    //IncludeRadioLicenseCost = x.IncludeRadioLicenseCost,
                    list[index].RadioLicenseCost += item.RadioLicenseCost;
                    list[index].Zinara_License_Fee += item.Zinara_License_Fee;
                    list[index].Sum_Insured += item.Sum_Insured;
                }
                else
                {
                    model.Premium_due = item.Premium_due;
                    model.GrandPremium = item.Premium_due + item.Zinara_License_Fee.Value + item.RadioLicenseCost.Value + item.ZTSC_Levy + item.Stamp_duty;
                    model.Stamp_duty = item.Stamp_duty;
                    model.ZTSC_Levy = item.ZTSC_Levy;
                    model.Comission_Amount = item.Comission_Amount;
                    //IncludeRadioLicenseCost = x.IncludeRadioLicenseCost,
                    model.RadioLicenseCost = item.RadioLicenseCost;
                    model.Zinara_License_Fee = item.Zinara_License_Fee;
                    model.Sum_Insured = item.Sum_Insured;
                    list.Add(model);
                }
            }



            Model.ListGrossWrittenPremiumReportdata = list.OrderByDescending(p => p.Id).ToList();
            var endorsmentList = GetGWPEndorsmentReport(new GrossWrittenPremiumReportSearchModels());
            Model.ListGrossWrittenPremiumReportdata.AddRange(endorsmentList);

            //  var res = Convert.ToDateTime("dd");

            return View(Model);
        }


        private List<GrossWrittenPremiumReportModels> GetAllGwpReport()
        {
            var recieptList = InsuranceContext.ReceiptHistorys.All().ToList();

            List<GrossWrittenPremiumReportModels> ListGrossWrittenPremiumReport = new List<GrossWrittenPremiumReportModels>();
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["Insurance"].ConnectionString;

            using (SqlConnection myConnection = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_GeGwpReport", myConnection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    myConnection.Open();

                    //  SqlParameter custId = cmd.Parameters.AddWithValue("@CustomerId", 10);

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {

                            GrossWrittenPremiumReportModels model = new GrossWrittenPremiumReportModels();
                            model.PaymentMethodId = dr["PaymentMethodId"] == DBNull.Value ? 0 : Convert.ToInt32(dr["PaymentMethodId"]);
                            model.Policy_Number = dr["Policy_Number"] == DBNull.Value ? "" : Convert.ToString(dr["Policy_Number"]);
                            model.RegistrationNo = dr["RegistrationNo"] == DBNull.Value ? "" : Convert.ToString(dr["RegistrationNo"]);

                            model.BranchName = dr["BranchName"] == DBNull.Value ? "" : Convert.ToString(dr["BranchName"]);
                            model.ALMBranchId= dr["ALMBranchId"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ALMBranchId"]);
                            model.PolicyCreatedBy = dr["PolicyCreatedBy"] == DBNull.Value ? "" : Convert.ToString(dr["PolicyCreatedBy"]);
                            model.Customer_Name = dr["Customer_Name"] == DBNull.Value ? "" : Convert.ToString(dr["Customer_Name"]);

                            model.Transaction_date = dr["Transaction_date"] == DBNull.Value ? DateTime.Now.ToShortDateString() : Convert.ToDateTime(dr["Transaction_date"]).ToShortDateString();
                            model.CoverNoteNum = dr["CoverNoteNum"] == DBNull.Value ? "" : Convert.ToString(dr["CoverNoteNum"]);
                            model.Payment_Mode = dr["Payment_Mode"] == DBNull.Value ? "Cash" : Convert.ToString(dr["Payment_Mode"]);
                            model.Payment_Term = dr["Payment_Term"] == DBNull.Value ? "" : Convert.ToString(dr["Payment_Term"]);
                            model.CoverType = dr["CoverType"] == DBNull.Value ? "" : Convert.ToString(dr["CoverType"]);

                            model.Currency = dr["Currency"] == DBNull.Value ? "" : Convert.ToString(dr["Currency"]);
                            model.Premium_due = dr["Premium_due"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["Premium_due"]);
                            model.Zinara_License_Fee = dr["Zinara_License_Fee"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["Zinara_License_Fee"]);
                            model.GrandPremium = Convert.ToDecimal(model.Premium_due + model.Zinara_License_Fee);
                            model.Stamp_duty = dr["Stamp_duty"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["Stamp_duty"]);
                            model.ZTSC_Levy = dr["ZTSC_Levy"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["ZTSC_Levy"]);
                            model.ALMId = dr["ALMId"] == DBNull.Value ? "" : Convert.ToString(dr["ALMId"]);
                            model.Comission_Amount = dr["Comission_Amount"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["Comission_Amount"]);
                            model.RadioLicenseCost = dr["RadioLicenseCost"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["RadioLicenseCost"]);
                            model.Zinara_License_Fee = dr["Zinara_License_Fee"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["Zinara_License_Fee"]);
                            model.PolicyRenewalDate = dr["PolicyRenewalDate"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(dr["PolicyRenewalDate"]);
                            model.IsActive = dr["IsActive"] == DBNull.Value ? false : Convert.ToBoolean(dr["IsActive"]);

                            model.RenewPolicyNumber = dr["RenewPolicyNumber"] == DBNull.Value ? "" : Convert.ToString(dr["RenewPolicyNumber"]);
                            model.BusinessSourceName = dr["BusinessSourceName"] == DBNull.Value ? "" : Convert.ToString(dr["BusinessSourceName"]);

                            model.SourceDetailName = dr["SourceDetailName"] == DBNull.Value ? "" : Convert.ToString(dr["SourceDetailName"]);
                            model.SummaryDetailId = dr["SummaryDetailId"] == DBNull.Value ? 0 : Convert.ToInt32(dr["SummaryDetailId"]);

                            model.Sum_Insured = dr["SumInsured"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["SumInsured"]);
                            model.AdministrationAmt = dr["AdministrationAmt"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["AdministrationAmt"]);

                            int policyId = dr["policyId"] == DBNull.Value ? 0 : Convert.ToInt32(dr["policyId"]);

                            model.ReceiptNumber = RecieptNumber(policyId, model.RenewPolicyNumber, recieptList);
                            ListGrossWrittenPremiumReport.Add(model);
                        }
                    }
                }
            }
            return ListGrossWrittenPremiumReport;
        }



        public string RecieptNumber(int policyId, string renewPolicyNumber, List<ReceiptModuleHistory> list)
        {
            string recieptNumber = "";
            int number = 0;

            if (!string.IsNullOrEmpty(renewPolicyNumber))
            {
                var renewPolicyDetail = renewPolicyNumber.Split('-');
                number = Convert.ToInt32(renewPolicyDetail[1]);
            }

            if (number > 1)
            {
                var details = list.Where(c => c.RenewPolicyNumber == renewPolicyNumber && c.PolicyId == policyId).OrderByDescending(c => c.Id).ToList();
                if (details.Count() > 0)
                    recieptNumber = Convert.ToString(details.FirstOrDefault().Id);
            }
            else
            {
                var details = list.Where(c => c.PolicyId == policyId).OrderByDescending(c => c.Id).ToList();
                if (details.Count() > 0)
                    recieptNumber = Convert.ToString(details.FirstOrDefault().Id);
            }
            return recieptNumber;
        }



        public ActionResult CallcentreGrosswrittenReport(GrossWrittenPremiumReportSearchModels _model)
        {

            List<GrossWrittenPremiumReportModels> ListGrossWrittenPremiumReport = new List<GrossWrittenPremiumReportModels>();
            ListGrossWrittenPremiumReportModels _ListGrossWrittenPremiumReport = new ListGrossWrittenPremiumReportModels();
            _ListGrossWrittenPremiumReport.ListGrossWrittenPremiumReportdata = new List<GrossWrittenPremiumReportModels>();
            var paymentInformationsList = InsuranceContext.PaymentInformations.All();

            var callCenterTargetSetting = InsuranceContext.Settings.Single(where: "keyname='CallCenterTarget'");
            var policyAmountSetting = InsuranceContext.Settings.Single(where: "keyname='PolicyAmount'");


            // ViewBag.ReportList = ReportsList();

            var query = " select SummaryDetail.PaymentMethodId, SummaryDetail.CreatedBy, PolicyDetail.PolicyNumber as Policy_Number, Customer.ALMId, case when VehicleDetail.ALMBranchId = 0  then  [dbo].fn_GetUserCallCenterAgent(SummaryDetail.CreatedBy) else [dbo].fn_GetUserALM(VehicleDetail.ALMBranchId) end  as PolicyCreatedBy, Customer.FirstName + ' ' + Customer.LastName as Customer_Name,VehicleDetail.TransactionDate as Transaction_date, ";
            query += "  case when Customer.id=SummaryDetail.CreatedBy then [dbo].fn_GetUserBranch(Customer.id) else [dbo].fn_GetUserBranch(SummaryDetail.CreatedBy) end as BranchName, ";
            query += " VehicleDetail.CoverNote as CoverNoteNum, ";
            query += " VehicleDetail.Premium + VehicleDetail.StampDuty + VehicleDetail.ZTSCLevy as Premium_due, VehicleDetail.StampDuty as Stamp_duty, VehicleDetail.ZTSCLevy as ZTSC_Levy, ";
            query += " cast(VehicleDetail.Premium * 30 / 100 as decimal(10, 2))    as Comission_Amount, VehicleDetail.IncludeRadioLicenseCost, ";
            query += " CASE WHEN IncludeRadioLicenseCost = 1 THEN VehicleDetail.RadioLicenseCost else 0 end as RadioLicenseCost, VehicleDetail.VehicleLicenceFee as Zinara_License_Fee, ";
            query += " VehicleDetail.RenewalDate as PolicyRenewalDate, VehicleDetail.IsActive, VehicleDetail.RenewPolicyNumber as RenewPolicyNumber, [dbo].[fn_GetUserWorkType] (SummaryDetail.CreatedBy) as WorkDesc ";
            query += " from PolicyDetail join Customer on PolicyDetail.CustomerId = Customer.Id ";
            query += " join VehicleDetail on PolicyDetail.Id = VehicleDetail.PolicyId ";
            query += "join SummaryVehicleDetail on VehicleDetail.id = SummaryVehicleDetail.VehicleDetailsId ";
            query += " join SummaryDetail on SummaryDetail.id = SummaryVehicleDetail.SummaryDetailId  ";
            //query += "  join PaymentInformation on SummaryDetail.Id=PaymentInformation.SummaryDetailId ";
            query += " where (VehicleDetail.IsActive = 1 or VehicleDetail.IsActive = null) and SummaryDetail.isQuotation=0  and   CONVERT(date, VehicleDetail.TransactionDate) >= convert(date, '" + DateTime.Now.ToShortDateString() + "', 101)   ";
            //  query += "and Customer.BranchId=" + (int)ALMBranch.GeneCallCentre;
            query += " order by  VehicleDetail.Id desc ";


            ListGrossWrittenPremiumReport = InsuranceContext.Query(query).
                Select(x => new GrossWrittenPremiumReportModels()
                {
                    PaymentMethodId = x.PaymentMethodId == null ? 0 : Convert.ToInt32(x.PaymentMethodId),
                    CreatedBy = x.CreatedBy == null ? 0 : Convert.ToInt32(x.CreatedBy),
                    Policy_Number = x.Policy_Number,
                    BranchName = x.BranchName,
                    PolicyCreatedBy = x.PolicyCreatedBy,
                    Customer_Name = x.Customer_Name,
                    Transaction_date = x.Transaction_date.ToShortDateString(),
                    CoverNoteNum = x.CoverNoteNum,
                    Premium_due = x.Premium_due == null ? 0 : x.Premium_due,
                    Stamp_duty = x.Stamp_duty == null ? 0 : x.Stamp_duty,
                    ZTSC_Levy = x.ZTSC_Levy == null ? 0 : x.ZTSC_Levy,
                    ALMId = x.ALMId,
                    Comission_Amount = x.Comission_Amount == null ? 0 : x.Comission_Amount,
                    //IncludeRadioLicenseCost = x.IncludeRadioLicenseCost,
                    RadioLicenseCost = x.RadioLicenseCost == null ? 0 : x.RadioLicenseCost,
                    Zinara_License_Fee = x.Zinara_License_Fee == null ? 0 : x.Zinara_License_Fee,
                    PolicyRenewalDate = x.PolicyRenewalDate,
                    IsActive = x.IsActive,
                    WorkDesc = x.WorkDesc

                }).ToList();


            ListGrossWrittenPremiumReport = ListGrossWrittenPremiumReport.Where(c => c.BranchName == CallCenterBranch).ToList();


            List<GrossWrittenPremiumReportModels> list = new List<GrossWrittenPremiumReportModels>();

            foreach (var item in ListGrossWrittenPremiumReport)
            {
                var paymentDetail = paymentInformationsList.Where(c => c.SummaryDetailId == item.SummaryDetailId);
                if (paymentDetail == null)
                    continue;

                GrossWrittenPremiumReportModels model = new GrossWrittenPremiumReportModels();
                model.Policy_Number = item.Policy_Number;
                model.CreatedBy = item.CreatedBy;
                model.BranchName = item.BranchName;
                model.PolicyCreatedBy = item.PolicyCreatedBy;
                model.Customer_Name = item.Customer_Name;
                model.Transaction_date = item.Transaction_date;
                model.CoverNoteNum = item.CoverNoteNum;
                model.Payment_Mode = item.Payment_Mode;
                model.Payment_Term = item.Payment_Term;
                model.CoverType = item.CoverType;
                model.Currency = item.Currency;
                model.RenewPolicyNumber = item.RenewPolicyNumber;
                model.ALMId = item.ALMId;
                model.IsActive = item.IsActive;
                model.PolicyRenewalDate = item.PolicyRenewalDate;
                model.SourceDetailName = item.SourceDetailName;
                model.BusinessSourceName = item.BusinessSourceName;
                model.PaymentMethodId = item.PaymentMethodId;
                model.WorkDesc = item.WorkDesc;
                //IncludeRadioLicenseCost = x.IncludeRadioLicenseCost,

                var index = list.FindIndex(c => c.Policy_Number == item.Policy_Number);
                if (index != -1)
                {
                    list[index].Premium_due += item.Premium_due;
                    list[index].Stamp_duty += item.Stamp_duty;
                    list[index].ZTSC_Levy += item.ZTSC_Levy;
                    list[index].Comission_Amount += item.Comission_Amount;
                    //IncludeRadioLicenseCost = x.IncludeRadioLicenseCost,
                    list[index].RadioLicenseCost += item.RadioLicenseCost;
                    list[index].Zinara_License_Fee += item.Zinara_License_Fee;
                    list[index].Sum_Insured += item.Sum_Insured;
                }
                else
                {
                    model.Premium_due = item.Premium_due;
                    model.Stamp_duty = item.Stamp_duty;
                    model.ZTSC_Levy = item.ZTSC_Levy;
                    model.Comission_Amount = item.Comission_Amount;
                    //IncludeRadioLicenseCost = x.IncludeRadioLicenseCost,
                    model.RadioLicenseCost = item.RadioLicenseCost;
                    model.Zinara_License_Fee = item.Zinara_License_Fee;
                    model.Sum_Insured = item.Sum_Insured;
                    list.Add(model);
                }
            }

            var listCustomer = InsuranceContext.Customers.All(where: "BranchId=" + (int)ALMBranch.GeneCallCentre).ToList();
            var WorkTypsList = InsuranceContext.WorkTypes.All().ToList();

            List<GrossWrittenPremiumReportModels> resultList = new List<GrossWrittenPremiumReportModels>();
            var listCreatedBy = list.Select(o => o.CreatedBy).Distinct().ToList();

            foreach (var item in listCreatedBy)
            {
                GrossWrittenPremiumReportModels model = new GrossWrittenPremiumReportModels();
                model.AgentActualPolicy = list.Where(c => c.CreatedBy == item).Count();

                // 69 policy for monthly
                model.AgentTargetPolicy = Convert.ToInt32(callCenterTargetSetting.value);

                model.AgentActualGwp = list.Where(c => c.CreatedBy == item).Select(c => c.Premium_due).Sum();

                // 22080 for monthly, 320 per policy
                model.AgentTargetGwp = Convert.ToDecimal(policyAmountSetting.value) * Convert.ToInt32(callCenterTargetSetting.value);

                var agentDetail = listCustomer.FirstOrDefault(c => c.Id == item);
                if (agentDetail != null)
                    model.PolicyCreatedBy = agentDetail.FirstName + ' ' + agentDetail.LastName;

                model.TransactionPerformance = Math.Round(((model.AgentActualPolicy / model.AgentTargetPolicy) * 100), 2);
                model.TransactionVariance = model.AgentActualPolicy - model.AgentTargetPolicy;

                model.GwpPerformance = Math.Round(((model.AgentActualGwp / model.AgentTargetGwp) * 100), 2);
                model.GwpVariance = model.AgentActualGwp - model.AgentTargetGwp;

                var WorkDescDetails = list.Where(c => c.CreatedBy == item).FirstOrDefault();
                if (WorkDescDetails != null)
                    model.WorkDesc = WorkDescDetails.WorkDesc;



                resultList.Add(model);
            }


            _model.ListGrossWrittenPremiumReportdata = resultList.OrderByDescending(p => p.Id).ToList();

            _model.NumOfUser = listCreatedBy.Count();

            return View("CallcentreGrosswrittenReport", _model);
        }



        [HttpPost]
        public ActionResult SearchCallcentreGrosswrittenReport(GrossWrittenPremiumReportSearchModels _model)
        {

            List<GrossWrittenPremiumReportModels> ListGrossWrittenPremiumReport = new List<GrossWrittenPremiumReportModels>();
            ListGrossWrittenPremiumReportModels _ListGrossWrittenPremiumReport = new ListGrossWrittenPremiumReportModels();
            _ListGrossWrittenPremiumReport.ListGrossWrittenPremiumReportdata = new List<GrossWrittenPremiumReportModels>();
            var paymentInformationsList = InsuranceContext.PaymentInformations.All();

            var callCenterTargetSetting = InsuranceContext.Settings.Single(where: "keyname='CallCenterTarget'");
            var policyAmountSetting = InsuranceContext.Settings.Single(where: "keyname='PolicyAmount'");

            DateTime start = Convert.ToDateTime(_model.FormDate);
            DateTime end = Convert.ToDateTime(_model.EndDate);

            TimeSpan difference = end - start; //create TimeSpan object

            int differntDays = difference.Days + 1;


            // ViewBag.ReportList = ReportsList();

            var query = " select SummaryDetail.PaymentMethodId, SummaryDetail.CreatedBy, PolicyDetail.PolicyNumber as Policy_Number, Customer.ALMId, case when VehicleDetail.ALMBranchId = 0  then  [dbo].fn_GetUserCallCenterAgent(SummaryDetail.CreatedBy) else [dbo].fn_GetUserALM(VehicleDetail.ALMBranchId) end  as PolicyCreatedBy, Customer.FirstName + ' ' + Customer.LastName as Customer_Name,VehicleDetail.TransactionDate as Transaction_date, ";
            query += "  case when Customer.id=SummaryDetail.CreatedBy then [dbo].fn_GetUserBranch(Customer.id) else [dbo].fn_GetUserBranch(SummaryDetail.CreatedBy) end as BranchName, ";
            query += " VehicleDetail.CoverNote as CoverNoteNum, ";
            query += " VehicleDetail.Premium + VehicleDetail.StampDuty + VehicleDetail.ZTSCLevy as Premium_due, VehicleDetail.StampDuty as Stamp_duty, VehicleDetail.ZTSCLevy as ZTSC_Levy, ";
            query += " cast(VehicleDetail.Premium * 30 / 100 as decimal(10, 2))    as Comission_Amount, VehicleDetail.IncludeRadioLicenseCost, ";
            query += " CASE WHEN IncludeRadioLicenseCost = 1 THEN VehicleDetail.RadioLicenseCost else 0 end as RadioLicenseCost, VehicleDetail.VehicleLicenceFee as Zinara_License_Fee, ";
            query += " VehicleDetail.RenewalDate as PolicyRenewalDate, VehicleDetail.IsActive, VehicleDetail.RenewPolicyNumber as RenewPolicyNumber,[dbo].[fn_GetUserWorkType] (SummaryDetail.CreatedBy) as WorkDesc ";
            query += " from PolicyDetail join Customer on PolicyDetail.CustomerId = Customer.Id ";
            query += " join VehicleDetail on PolicyDetail.Id = VehicleDetail.PolicyId ";
            query += "join SummaryVehicleDetail on VehicleDetail.id = SummaryVehicleDetail.VehicleDetailsId ";
            query += " join SummaryDetail on SummaryDetail.id = SummaryVehicleDetail.SummaryDetailId ";
            //query += "  join PaymentInformation on SummaryDetail.Id=PaymentInformation.SummaryDetailId ";
            query += " where (VehicleDetail.IsActive = 1 or VehicleDetail.IsActive = null) and SummaryDetail.isQuotation=0    and (  CONVERT(date, VehicleDetail.TransactionDate) >= convert(date, '" + _model.FormDate + "', 101)  and CONVERT(date, VehicleDetail.TransactionDate) <= convert(date, '" + _model.EndDate + "', 101))  ";
            //  query += "and Customer.BranchId=" + (int)ALMBranch.GeneCallCentre;
            query += " order by  VehicleDetail.Id desc ";


            ListGrossWrittenPremiumReport = InsuranceContext.Query(query).
                Select(x => new GrossWrittenPremiumReportModels()
                {
                    PaymentMethodId = x.PaymentMethodId == null ? 0 : x.PaymentMethodId,
                    CreatedBy = x.CreatedBy == null ? 0 : Convert.ToInt32(x.CreatedBy),
                    Policy_Number = x.Policy_Number,
                    BranchName = x.BranchName,
                    PolicyCreatedBy = x.PolicyCreatedBy,
                    Customer_Name = x.Customer_Name,
                    Transaction_date = x.Transaction_date.ToShortDateString(),
                    CoverNoteNum = x.CoverNoteNum,
                    Premium_due = x.Premium_due == null ? 0 : x.Premium_due,
                    Stamp_duty = x.Stamp_duty == null ? 0 : x.Stamp_duty,
                    ZTSC_Levy = x.ZTSC_Levy == null ? 0 : x.ZTSC_Levy,
                    ALMId = x.ALMId == null ? "" : Convert.ToString(x.ALMId),
                    Comission_Amount = x.Comission_Amount == null ? 0 : x.Comission_Amount,
                    //IncludeRadioLicenseCost = x.IncludeRadioLicenseCost,
                    RadioLicenseCost = x.RadioLicenseCost == null ? 0 : x.RadioLicenseCost,
                    Zinara_License_Fee = x.Zinara_License_Fee == null ? 0 : x.Zinara_License_Fee,
                    PolicyRenewalDate = x.PolicyRenewalDate,
                    IsActive = x.IsActive,
                    WorkDesc = x.WorkDesc
                }).ToList();

            ListGrossWrittenPremiumReport = ListGrossWrittenPremiumReport.Where(c => c.BranchName == CallCenterBranch).ToList();


            List<GrossWrittenPremiumReportModels> list = new List<GrossWrittenPremiumReportModels>();

            foreach (var item in ListGrossWrittenPremiumReport)
            {
                var paymentDetail = paymentInformationsList.Where(c => c.SummaryDetailId == item.SummaryDetailId);
                if (paymentDetail == null)
                    continue;


                GrossWrittenPremiumReportModels model = new GrossWrittenPremiumReportModels();

                model.Policy_Number = item.Policy_Number;
                model.CreatedBy = item.CreatedBy;
                model.BranchName = item.BranchName;
                model.PolicyCreatedBy = item.PolicyCreatedBy;
                model.Customer_Name = item.Customer_Name;
                model.Transaction_date = item.Transaction_date;
                model.CoverNoteNum = item.CoverNoteNum;
                model.Payment_Mode = item.Payment_Mode;
                model.Payment_Term = item.Payment_Term;
                model.CoverType = item.CoverType;
                model.Currency = item.Currency;
                model.RenewPolicyNumber = item.RenewPolicyNumber;
                model.ALMId = item.ALMId;
                model.IsActive = item.IsActive;
                model.PolicyRenewalDate = item.PolicyRenewalDate;
                model.SourceDetailName = item.SourceDetailName;
                model.BusinessSourceName = item.BusinessSourceName;
                model.PaymentMethodId = item.PaymentMethodId;
                model.WorkDesc = item.WorkDesc;
                //IncludeRadioLicenseCost = x.IncludeRadioLicenseCost,

                var index = list.FindIndex(c => c.Policy_Number == item.Policy_Number);
                if (index != -1)
                {
                    list[index].Premium_due += item.Premium_due;
                    list[index].Stamp_duty += item.Stamp_duty;
                    list[index].ZTSC_Levy += item.ZTSC_Levy;
                    list[index].Comission_Amount += item.Comission_Amount;
                    //IncludeRadioLicenseCost = x.IncludeRadioLicenseCost,
                    list[index].RadioLicenseCost += item.RadioLicenseCost;
                    list[index].Zinara_License_Fee += item.Zinara_License_Fee;
                    list[index].Sum_Insured += item.Sum_Insured;
                }
                else
                {
                    model.Premium_due = item.Premium_due;
                    model.Stamp_duty = item.Stamp_duty;
                    model.ZTSC_Levy = item.ZTSC_Levy;
                    model.Comission_Amount = item.Comission_Amount;
                    //IncludeRadioLicenseCost = x.IncludeRadioLicenseCost,
                    model.RadioLicenseCost = item.RadioLicenseCost;
                    model.Zinara_License_Fee = item.Zinara_License_Fee;
                    model.Sum_Insured = item.Sum_Insured;
                    list.Add(model);
                }
            }

            var listCustomer = InsuranceContext.Customers.All(where: "BranchId=" + (int)ALMBranch.GeneCallCentre).ToList();

            List<GrossWrittenPremiumReportModels> resultList = new List<GrossWrittenPremiumReportModels>();
            var listCreatedBy = list.Select(o => o.CreatedBy).Distinct().ToList();

            foreach (var item in listCreatedBy)
            {
                GrossWrittenPremiumReportModels model = new GrossWrittenPremiumReportModels();
                model.AgentActualPolicy = list.Where(c => c.CreatedBy == item).Count();

                // 69 policy for monthly
                // model.AgentTargetPolicy = 3 * differntDays;
                model.AgentTargetPolicy = Convert.ToInt32(callCenterTargetSetting.value) * differntDays;

                model.AgentActualGwp = list.Where(c => c.CreatedBy == item).Select(c => c.Premium_due).Sum();

                // 22080 for monthly // 320 per policy

                var perDayTarget = Convert.ToDecimal(policyAmountSetting.value) * Convert.ToInt32(callCenterTargetSetting.value);
                model.AgentTargetGwp = perDayTarget * differntDays;

                var agentDetail = listCustomer.FirstOrDefault(c => c.Id == item);
                if (agentDetail != null)
                    model.PolicyCreatedBy = agentDetail.FirstName + ' ' + agentDetail.LastName;

                model.TransactionPerformance = Math.Round(((model.AgentActualPolicy / model.AgentTargetPolicy) * 100), 2);
                model.TransactionVariance = model.AgentActualPolicy - model.AgentTargetPolicy;

                model.GwpPerformance = Math.Round(((model.AgentActualGwp / model.AgentTargetGwp) * 100), 2);
                model.GwpVariance = model.AgentActualGwp - model.AgentTargetGwp;

                var WorkDescDetails = list.Where(c => c.CreatedBy == item).FirstOrDefault();
                if (WorkDescDetails != null)
                    model.WorkDesc = WorkDescDetails.WorkDesc;

                resultList.Add(model);
            }

            _model.ListGrossWrittenPremiumReportdata = resultList.OrderByDescending(p => p.Id).ToList();
            _model.NumOfUser = listCreatedBy.Count();
            return View("CallcentreGrosswrittenReport", _model);
        }


        public ActionResult ALMBranchReport()
        {


            List<GrossWrittenPremiumReportModels> ListGrossWrittenPremiumReport = new List<GrossWrittenPremiumReportModels>();
            ListGrossWrittenPremiumReportModels _ListGrossWrittenPremiumReport = new ListGrossWrittenPremiumReportModels();
            _ListGrossWrittenPremiumReport.ListGrossWrittenPremiumReportdata = new List<GrossWrittenPremiumReportModels>();
            GrossWrittenPremiumReportSearchModels Model = new GrossWrittenPremiumReportSearchModels();


            var callCenterTargetSetting = InsuranceContext.Settings.Single(where: "keyname='CallCenterTarget'");
            var policyAmountSetting = InsuranceContext.Settings.Single(where: "keyname='PolicyAmount'");

            //IEnumerable<Branch> branches = InsuranceContext.Branches.All();

            List<BranchModel> obj = InsuranceContext.Query("select * from Branch where id != 6").Select(x => new BranchModel
            {
                Id = x.Id,
                BranchName = x.BranchName
            }).ToList();

            MultiSelectList selectListItems = new MultiSelectList(obj, "Id", "BranchName");

            ViewBag.Branch = selectListItems;

            var branchList = InsuranceContext.Branches.All();


            var query = " select top 100 PolicyDetail.PolicyNumber as Policy_Number, SummaryDetail.CreatedBy, Customer.ALMId, case when Customer.ALMId is null  then  [dbo].fn_GetUserCallCenterAgent(SummaryDetail.CreatedBy) else [dbo].fn_GetUserALM(Customer.BranchId) end  as PolicyCreatedBy, Customer.FirstName + ' ' + Customer.LastName as Customer_Name,VehicleDetail.TransactionDate as Transaction_date, ";
            query += "  case when Customer.id=SummaryDetail.CreatedBy then [dbo].fn_GetUserBranch(Customer.id) else [dbo].fn_GetUserBranch(SummaryDetail.CreatedBy) end as BranchName, ";
            query += " VehicleDetail.CoverNote as CoverNoteNum, PaymentMethod.Name as Payment_Mode, PaymentTerm.Name as Payment_Term,CoverType.Name as CoverType, Currency.Name as Currency, ";
            query += " VehicleDetail.Premium + VehicleDetail.StampDuty + VehicleDetail.ZTSCLevy + VehicleDetail.VehicleLicenceFee + COALESCE(CAST(VehicleDetail.RadioLicenseCost as DECIMAL(9,2)),0)   as Premium_due, VehicleDetail.StampDuty as Stamp_duty, VehicleDetail.ZTSCLevy as ZTSC_Levy, ";
            query += " cast(VehicleDetail.Premium * 30 / 100 as decimal(10, 2))    as Comission_Amount, VehicleDetail.IncludeRadioLicenseCost, ";
            query += " CASE WHEN IncludeRadioLicenseCost = 1 THEN VehicleDetail.RadioLicenseCost else 0 end as RadioLicenseCost, VehicleDetail.VehicleLicenceFee as Zinara_License_Fee, ";
            query += " VehicleDetail.RenewalDate as PolicyRenewalDate, VehicleDetail.IsActive, VehicleDetail.RenewPolicyNumber as RenewPolicyNumber, ";
            query += " VehicleDetail.BusinessSourceDetailId, BusinessSource.Source as BusinessSourceName, SourceDetail.FirstName + ' ' + SourceDetail.LastName as SourceDetailName from PolicyDetail ";
            query += " join Customer on PolicyDetail.CustomerId = Customer.Id ";
            query += "join Branch on Customer.BranchId = Branch.Id";
            query += "  join VehicleDetail on PolicyDetail.Id = VehicleDetail.PolicyId ";
            query += "join SummaryVehicleDetail on VehicleDetail.id = SummaryVehicleDetail.VehicleDetailsId ";
            query += " join SummaryDetail on SummaryDetail.id = SummaryVehicleDetail.SummaryDetailId ";
            query += " join PaymentMethod on SummaryDetail.PaymentMethodId = PaymentMethod.Id ";
            query += "join PaymentTerm on VehicleDetail.PaymentTermId = PaymentTerm.Id ";
            query += " left join CoverType on VehicleDetail.CoverTypeId = CoverType.Id ";
            query += " left join Currency on VehicleDetail.CurrencyId = Currency.Id ";
            query += " left join BusinessSource on BusinessSource.Id = VehicleDetail.BusinessSourceDetailId ";
            query += " left   join SourceDetail on VehicleDetail.BusinessSourceDetailId = SourceDetail.Id join AspNetUsers on AspNetUsers.id=customer.UserID join AspNetUserRoles on AspNetUserRoles.UserId=AspNetUsers.Id ";
            query += " where VehicleDetail.IsActive = 1 and SummaryDetail.isQuotation=0 and Customer.id=SummaryDetail.CreatedBy and Branch.BranchName != 'Gene Call Centre' ";
            query += " and   CONVERT(date, VehicleDetail.TransactionDate) = convert(date, '" + DateTime.Now.ToShortDateString() + "', 101) order by  VehicleDetail.Id desc";


            ListGrossWrittenPremiumReport = InsuranceContext.Query(query).
                Select(x => new GrossWrittenPremiumReportModels()
                {

                    Policy_Number = x.Policy_Number,
                    CreatedBy = x.CreatedBy,
                    BranchName = x.BranchName,
                    PolicyCreatedBy = x.PolicyCreatedBy,
                    Customer_Name = x.Customer_Name,
                    Transaction_date = x.Transaction_date.ToShortDateString(),
                    CoverNoteNum = x.CoverNoteNum,
                    Payment_Mode = x.Payment_Mode,
                    Payment_Term = x.Payment_Term,
                    CoverType = x.CoverType,
                    Currency = x.Currency,
                    Premium_due = x.Premium_due,
                    Stamp_duty = x.Stamp_duty,
                    ZTSC_Levy = x.ZTSC_Levy,
                    ALMId = x.ALMId,
                    Comission_Amount = x.Comission_Amount,
                    //IncludeRadioLicenseCost = x.IncludeRadioLicenseCost,
                    RadioLicenseCost = x.RadioLicenseCost,
                    Zinara_License_Fee = x.Zinara_License_Fee,
                    PolicyRenewalDate = x.PolicyRenewalDate,
                    IsActive = x.IsActive,
                    RenewPolicyNumber = x.RenewPolicyNumber,
                    // BusinessSourceName = x.BusinessSourceName,
                    //IncludeRadioLicenseCost = x.IncludeRadioLicenseCost,
                    SourceDetailName = x.SourceDetailName,
                }).ToList();

            var listCreatedBy = ListGrossWrittenPremiumReport.Select(o => o.BranchName).Distinct().ToList();


            int differntDays = 1;

            var resultList = new List<GrossWrittenPremiumReportModels>();

            foreach (var item in listCreatedBy)
            {
                GrossWrittenPremiumReportModels model = new GrossWrittenPremiumReportModels();
                model.AgentActualPolicy = ListGrossWrittenPremiumReport.Where(c => c.BranchName == item).Count();

                // 69 policy for monthly
                // model.AgentTargetPolicy = 3 * differntDays;
                model.AgentTargetPolicy = Convert.ToInt32(callCenterTargetSetting.value) * differntDays;

                model.AgentActualGwp = ListGrossWrittenPremiumReport.Where(c => c.BranchName == item).Select(c => c.Premium_due).Sum();

                // 22080 for monthly
                // model.AgentTargetGwp = 960 * differntDays;

                var perDayTarget = Convert.ToDecimal(policyAmountSetting.value) * Convert.ToInt32(callCenterTargetSetting.value);

                model.AgentTargetGwp = perDayTarget * differntDays;



                model.PolicyCreatedBy = item;

                model.TransactionPerformance = Math.Round(((model.AgentActualPolicy / model.AgentTargetPolicy) * 100), 2);
                model.TransactionVariance = model.AgentActualPolicy - model.AgentTargetPolicy;

                model.GwpPerformance = Math.Round(((model.AgentActualGwp / model.AgentTargetGwp) * 100), 2);
                model.GwpVariance = model.AgentActualGwp - model.AgentTargetGwp;

                resultList.Add(model);
            }


            Model.ListGrossWrittenPremiumReportdata = resultList;
            Model.NumOfUser = listCreatedBy.Count();

            return View(Model);
        }


        [HttpPost]
        public ActionResult SearchALMBranchReport(GrossWrittenPremiumReportSearchModels _model)
        {


            List<GrossWrittenPremiumReportModels> ListGrossWrittenPremiumReport = new List<GrossWrittenPremiumReportModels>();
            ListGrossWrittenPremiumReportModels _ListGrossWrittenPremiumReport = new ListGrossWrittenPremiumReportModels();
            _ListGrossWrittenPremiumReport.ListGrossWrittenPremiumReportdata = new List<GrossWrittenPremiumReportModels>();
            GrossWrittenPremiumReportSearchModels Model = new GrossWrittenPremiumReportSearchModels();


            DateTime start = Convert.ToDateTime(_model.FormDate);
            DateTime end = Convert.ToDateTime(_model.EndDate);

            var callCenterTargetSetting = InsuranceContext.Settings.Single(where: "keyname='CallCenterTarget'");
            var policyAmountSetting = InsuranceContext.Settings.Single(where: "keyname='PolicyAmount'");


            TimeSpan difference = end - start; //create TimeSpan object

            int differntDays = difference.Days + 1;

            //IEnumerable<Branch> branches = InsuranceContext.Branches.All();

            List<BranchModel> obj = InsuranceContext.Query("select * from Branch where id != 6").Select(x => new BranchModel
            {
                Id = x.Id,
                BranchName = x.BranchName
            }).ToList();

            MultiSelectList selectListItems = new MultiSelectList(obj, "Id", "BranchName");

            ViewBag.Branch = selectListItems;

            var branchList = InsuranceContext.Branches.All();


            var query = " select  PolicyDetail.PolicyNumber as Policy_Number, SummaryDetail.CreatedBy, Customer.ALMId, case when Customer.ALMId is null  then  [dbo].fn_GetUserCallCenterAgent(SummaryDetail.CreatedBy) else [dbo].fn_GetUserALM(Customer.BranchId) end  as PolicyCreatedBy, Customer.FirstName + ' ' + Customer.LastName as Customer_Name,VehicleDetail.TransactionDate as Transaction_date, ";
            query += "  case when Customer.id=SummaryDetail.CreatedBy then [dbo].fn_GetUserBranch(Customer.id) else [dbo].fn_GetUserBranch(SummaryDetail.CreatedBy) end as BranchName, ";
            query += " VehicleDetail.CoverNote as CoverNoteNum, PaymentMethod.Name as Payment_Mode, PaymentTerm.Name as Payment_Term,CoverType.Name as CoverType, Currency.Name as Currency, ";
            query += " VehicleDetail.Premium + VehicleDetail.StampDuty + VehicleDetail.ZTSCLevy + VehicleDetail.VehicleLicenceFee + COALESCE(CAST(VehicleDetail.RadioLicenseCost as DECIMAL(9,2)),0)   as Premium_due, VehicleDetail.StampDuty as Stamp_duty, VehicleDetail.ZTSCLevy as ZTSC_Levy, ";
            query += " cast(VehicleDetail.Premium * 30 / 100 as decimal(10, 2))    as Comission_Amount, VehicleDetail.IncludeRadioLicenseCost, ";
            query += " CASE WHEN IncludeRadioLicenseCost = 1 THEN VehicleDetail.RadioLicenseCost else 0 end as RadioLicenseCost, VehicleDetail.VehicleLicenceFee as Zinara_License_Fee, ";
            query += " VehicleDetail.RenewalDate as PolicyRenewalDate, VehicleDetail.IsActive, VehicleDetail.RenewPolicyNumber as RenewPolicyNumber, ";
            query += " VehicleDetail.BusinessSourceDetailId, BusinessSource.Source as BusinessSourceName, SourceDetail.FirstName + ' ' + SourceDetail.LastName as SourceDetailName from PolicyDetail ";
            query += " join Customer on PolicyDetail.CustomerId = Customer.Id ";
            query += "join Branch on Customer.BranchId = Branch.Id";
            query += "  join VehicleDetail on PolicyDetail.Id = VehicleDetail.PolicyId ";
            query += "join SummaryVehicleDetail on VehicleDetail.id = SummaryVehicleDetail.VehicleDetailsId ";
            query += " join SummaryDetail on SummaryDetail.id = SummaryVehicleDetail.SummaryDetailId ";
            query += " join PaymentMethod on SummaryDetail.PaymentMethodId = PaymentMethod.Id ";
            query += "join PaymentTerm on VehicleDetail.PaymentTermId = PaymentTerm.Id ";
            query += " left join CoverType on VehicleDetail.CoverTypeId = CoverType.Id ";
            query += " left join Currency on VehicleDetail.CurrencyId = Currency.Id ";
            query += " left join BusinessSource on BusinessSource.Id = VehicleDetail.BusinessSourceDetailId ";
            query += " left   join SourceDetail on VehicleDetail.BusinessSourceDetailId = SourceDetail.Id join AspNetUsers on AspNetUsers.id=customer.UserID join AspNetUserRoles on AspNetUserRoles.UserId=AspNetUsers.Id ";
            query += " where VehicleDetail.IsActive = 1 and SummaryDetail.isQuotation=0 and Customer.id=SummaryDetail.CreatedBy and Branch.BranchName != 'Gene Call Centre' ";
            query += " and   (  CONVERT(date, VehicleDetail.TransactionDate) >= convert(date, '" + _model.FormDate + "', 101)  and CONVERT(date, VehicleDetail.TransactionDate) <= convert(date, '" + _model.EndDate + "', 101)) order by  VehicleDetail.Id desc";


            ListGrossWrittenPremiumReport = InsuranceContext.Query(query).
                Select(x => new GrossWrittenPremiumReportModels()
                {

                    Policy_Number = x.Policy_Number,
                    CreatedBy = x.CreatedBy,
                    BranchName = x.BranchName,
                    PolicyCreatedBy = x.PolicyCreatedBy,
                    Customer_Name = x.Customer_Name,
                    Transaction_date = x.Transaction_date.ToShortDateString(),
                    CoverNoteNum = x.CoverNoteNum,
                    Payment_Mode = x.Payment_Mode,
                    Payment_Term = x.Payment_Term,
                    CoverType = x.CoverType,
                    Currency = x.Currency,
                    Premium_due = x.Premium_due,
                    Stamp_duty = x.Stamp_duty,
                    ZTSC_Levy = x.ZTSC_Levy,
                    ALMId = x.ALMId,
                    Comission_Amount = x.Comission_Amount,
                    //IncludeRadioLicenseCost = x.IncludeRadioLicenseCost,
                    RadioLicenseCost = x.RadioLicenseCost,
                    Zinara_License_Fee = x.Zinara_License_Fee,
                    PolicyRenewalDate = x.PolicyRenewalDate,
                    IsActive = x.IsActive,
                    RenewPolicyNumber = x.RenewPolicyNumber,
                    // BusinessSourceName = x.BusinessSourceName,
                    //IncludeRadioLicenseCost = x.IncludeRadioLicenseCost,
                    SourceDetailName = x.SourceDetailName,
                }).ToList();

            var listCreatedBy = ListGrossWrittenPremiumReport.Select(o => o.BranchName).Distinct().ToList();




            var resultList = new List<GrossWrittenPremiumReportModels>();

            foreach (var item in listCreatedBy)
            {
                GrossWrittenPremiumReportModels model = new GrossWrittenPremiumReportModels();
                model.AgentActualPolicy = ListGrossWrittenPremiumReport.Where(c => c.BranchName == item).Count();

                // 69 policy for monthly
                //  model.AgentTargetPolicy = 3 * differntDays;
                model.AgentTargetPolicy = Convert.ToInt32(callCenterTargetSetting.value) * differntDays;

                model.AgentActualGwp = ListGrossWrittenPremiumReport.Where(c => c.BranchName == item).Select(c => c.Premium_due).Sum();

                // 22080 for monthly
                //model.AgentTargetGwp = 960 * differntDays;
                var perDayTarget = Convert.ToDecimal(policyAmountSetting.value) * Convert.ToInt32(callCenterTargetSetting.value);
                model.AgentTargetGwp = perDayTarget * differntDays;


                model.PolicyCreatedBy = item;

                model.TransactionPerformance = Math.Round(((model.AgentActualPolicy / model.AgentTargetPolicy) * 100), 2);
                model.TransactionVariance = model.AgentActualPolicy - model.AgentTargetPolicy;

                model.GwpPerformance = Math.Round(((model.AgentActualGwp / model.AgentTargetGwp) * 100), 2);
                model.GwpVariance = model.AgentActualGwp - model.AgentTargetGwp;

                resultList.Add(model);
            }


            Model.ListGrossWrittenPremiumReportdata = resultList;
            Model.NumOfUser = listCreatedBy.Count();

            return View("ALMBranchReport", Model);
        }



        public List<ReportType> ReportsList()
        {
            List<ReportType> list = new List<ReportType>();

            ReportType reportCombine = new ReportType { Id = 1, ReportName = "Combined" };
            ReportType reportALM = new ReportType { Id = 2, ReportName = "ALM" };
            ReportType reportCallCenter = new ReportType { Id = 3, ReportName = "Call Center" };

            list.Add(reportCombine);
            list.Add(reportALM);
            list.Add(reportCallCenter);

            return list;
        }



        private List<GrossWrittenPremiumReportModels> GetGWPEndorsmentReport(GrossWrittenPremiumReportSearchModels _model)
        {
            var paymentInformationsList = InsuranceContext.PaymentInformations.All();

            List<GrossWrittenPremiumReportModels> list = new List<GrossWrittenPremiumReportModels>();

            string getRecord = "";

            if (_model.FormDate == null && _model.EndDate == null)
            {
                getRecord = "top 100";
            }

            string query = " select " + getRecord + " [dbo].fn_GetUserCallCenterAgent(EndorsementSummaryDetail.CreatedBy) as AgentName, EndorsementCustomer.FirstName + ' ' + EndorsementCustomer.LastName as CustomerName, EndorsementPolicyDetail.PolicyNumber, ";
            query +=  "EndorsementVehicleDetail.CoverNoteNo, EndorsementVehicleDetail.TransactionDate, PaymentMethod.Name as PaymentMethod,  ";
            query +=  "PaymentTerm.Name as PaymentTerm, Currency.Name as Currency, EndorsementVehicleDetail.SumInsured, EndorsementVehicleDetail.Premium  as Premium_due ,";
            query += " EndorsementVehicleDetail.StampDuty, EndorsementVehicleDetail.ZTSCLevy,EndorsementVehicleDetail.RadioLicenseCost,";
            query += " EndorsementVehicleDetail.VehicleLicenceFee, EndorsementVehicleDetail.IsActive, CoverType.Name as CoverType from EndorsementPolicyDetail ";
            query += " join EndorsementVehicleDetail on EndorsementPolicyDetail.Id = EndorsementVehicleDetail.EndorsementPolicyId ";
            query += " join EndorsementSummaryVehicleDetail on EndorsementSummaryVehicleDetail.EndorsementVehicleId = EndorsementVehicleDetail.Id join ";
            query += " EndorsementSummaryDetail on EndorsementSummaryDetail.Id = EndorsementSummaryVehicleDetail.EndorsementSummaryId ";
            query += " join EndorsementCustomer on EndorsementCustomer.Id = EndorsementVehicleDetail.EndorsementCustomerId ";
            query += " join EndorsementPaymentInformation on EndorsementPaymentInformation.EndorsementSummaryId = EndorsementSummaryDetail.Id ";
            query += " left join CoverType on EndorsementVehicleDetail.CoverTypeId= CoverType.Id left join PaymentMethod on  EndorsementSummaryDetail.PaymentMethodId = PaymentMethod.Id ";
            query += " left join PaymentTerm on EndorsementVehicleDetail.PaymentTermId = PaymentTerm.Id ";
            query += " left join Currency on EndorsementVehicleDetail.CurrencyId = Currency.Id where EndorsementSummaryDetail.IsCompleted = 1 and EndorsementVehicleDetail.IsActive=1";

            if (_model.FormDate != null && _model.EndDate != null)
            {
                query += " and (  CONVERT(date, EndorsementVehicleDetail.TransactionDate) >= convert(date, '" + _model.FormDate + "', 101)  and CONVERT(date, EndorsementVehicleDetail.TransactionDate) <= convert(date, '" + _model.EndDate + "', 101))  order by  EndorsementVehicleDetail.Id desc ";
            }


            var result = InsuranceContext.Query(query).Select(x => new GrossWrittenPremiumReportModels()
            {
                Policy_Number = x.PolicyNumber,
                //BranchName = x.BranchName,
                PolicyCreatedBy = x.AgentName,
                Customer_Name = x.CustomerName,
                Transaction_date = x.TransactionDate.ToShortDateString(),
                CoverNoteNum = x.CoverNoteNo,
                Payment_Mode = x.PaymentMethod == null ? "Cash" : x.PaymentMethod,
                Payment_Term = x.PaymentTerm,
                CoverType = x.CoverType,
                Currency = x.Currency,
                Premium_due = x.Premium_due == null ? 0 : x.Premium_due,
                Stamp_duty = x.StampDuty == null ? 0 : x.StampDuty,
                ZTSC_Levy = x.ZTSCLevy == null ? 0 : x.ZTSCLevy,
                RadioLicenseCost = x.RadioLicenseCost == null ? 0 : x.RadioLicenseCost,
                Zinara_License_Fee = x.VehicleLicenceFee == null ? 0 : x.VehicleLicenceFee,
                // PolicyRenewalDate = x.PolicyRenewalDate,
                IsActive = x.IsActive,
                Sum_Insured = x.SumInsured
            });


            foreach (var item in result)
            {
                var paymentDetail = paymentInformationsList.Where(c => c.SummaryDetailId == item.SummaryDetailId);
                if (paymentDetail == null)
                    continue;

                GrossWrittenPremiumReportModels model = new GrossWrittenPremiumReportModels();
                model.Policy_Number = item.Policy_Number;
                //BranchName = x.BranchName,
                model.PolicyCreatedBy = item.PolicyCreatedBy;
                model.Customer_Name = item.Customer_Name;
                model.Transaction_date = Convert.ToDateTime(item.Transaction_date).ToShortDateString();
                model.CoverNoteNum = item.CoverNoteNum;
                model.Payment_Mode = item.Payment_Mode == null ? "Cash" : item.Payment_Mode;
                model.Payment_Term = item.Payment_Term;
                model.CoverType = item.CoverType;
                model.Currency = model.Currency;
                model.Premium_due = item.Premium_due;
                model.Stamp_duty = item.Stamp_duty;
                model.ZTSC_Levy = item.ZTSC_Levy;
                model.RadioLicenseCost = item.RadioLicenseCost == null ? 0 : item.RadioLicenseCost;
                model.Zinara_License_Fee = item.Zinara_License_Fee;
                model.PolicyRenewalDate = item.PolicyRenewalDate;
                model.IsActive = item.IsActive;
                model.Sum_Insured = item.Sum_Insured;

                //IncludeRadioLicenseCost = x.IncludeRadioLicenseCost,

                var index = list.FindIndex(c => c.Policy_Number == item.Policy_Number);
                if (index != -1)
                {
                    list[index].Premium_due += item.Premium_due;
                    list[index].Stamp_duty += item.Stamp_duty;
                    list[index].ZTSC_Levy += item.ZTSC_Levy;
                    list[index].Comission_Amount += item.Comission_Amount;
                    //IncludeRadioLicenseCost = x.IncludeRadioLicenseCost,
                    list[index].RadioLicenseCost += item.RadioLicenseCost;
                    list[index].Zinara_License_Fee += item.Zinara_License_Fee;
                    list[index].Sum_Insured += item.Sum_Insured;
                }
                else
                {
                    model.Premium_due = item.Premium_due;
                    model.Stamp_duty = item.Stamp_duty;
                    model.ZTSC_Levy = item.ZTSC_Levy;
                    model.Comission_Amount = item.Comission_Amount;
                    //IncludeRadioLicenseCost = x.IncludeRadioLicenseCost,
                    model.RadioLicenseCost = item.RadioLicenseCost;
                    model.Zinara_License_Fee = item.Zinara_License_Fee;
                    model.Sum_Insured = item.Sum_Insured;
                    list.Add(model);
                }
            }


            return list;
        }

        //private string  GetALMID(string branchId, List<Branches> branches)
        //{
        //    string almId = "";

        //    var braches = branches.FirstOrDefault(c=>c.id);
        //}

        private void UpdateGwp()
        {
            var summaryList = InsuranceContext.SummaryDetails.All(where: "PaymentMethodId is null");
            var paymentDetails = InsuranceContext.PaymentInformations.All();
            var paymentMethods = InsuranceContext.PaymentMethods.All();

            foreach (var item in summaryList)
            {
                var payment = paymentDetails.FirstOrDefault(c => c.SummaryDetailId == item.Id);
                if (payment != null)
                {
                    var paymentMethod = paymentMethods.FirstOrDefault(c => c.Name == payment.PaymentId);
                    if (paymentMethod != null)
                    {
                        item.PaymentMethodId = paymentMethod.Id;
                        InsuranceContext.SummaryDetails.Update(item);
                    }
                }
            }
        }


        [Authorize(Roles = "Administrator,Reports,Finance,Team Leaders")]
        public ActionResult SearchGrossReports(GrossWrittenPremiumReportSearchModels _model)
        {

            List<GrossWrittenPremiumReportModels> ListGrossWrittenPremiumReport = new List<GrossWrittenPremiumReportModels>();
            ListGrossWrittenPremiumReportModels _ListGrossWrittenPremiumReport = new ListGrossWrittenPremiumReportModels();
            _ListGrossWrittenPremiumReport.ListGrossWrittenPremiumReportdata = new List<GrossWrittenPremiumReportModels>();
            var paymentInformationsList = InsuranceContext.PaymentInformations.All();

            ViewBag.ReportList = ReportsList();
            ViewBag.PaymentMethod = PaymentMethodsList();
            var recieptList = InsuranceContext.ReceiptHistorys.All().ToList();

            var query = " select SummaryDetail.PaymentMethodId, PolicyDetail.PolicyNumber as Policy_Number,PolicyDetail.Id as policyId, Customer.ALMId, case when VehicleDetail.ALMBranchId = 0  then  [dbo].fn_GetUserCallCenterAgent(SummaryDetail.CreatedBy) else [dbo].fn_GetUserALM(VehicleDetail.ALMBranchId) end  as PolicyCreatedBy, Customer.FirstName + ' ' + Customer.LastName as Customer_Name,VehicleDetail.TransactionDate as Transaction_date, ";
            query += "  case when Customer.id=SummaryDetail.CreatedBy then [dbo].fn_GetUserBranch(Customer.id) else [dbo].fn_GetUserBranch(SummaryDetail.CreatedBy) end as BranchName, ";
            query += " VehicleDetail.CoverNote as CoverNoteNum, PaymentMethod.Name as Payment_Mode, PaymentTerm.Name as Payment_Term,CoverType.Name as CoverType, Currency.Name as Currency, ";
            query += " VehicleDetail.Premium  as Premium_due, VehicleDetail.StampDuty as Stamp_duty, VehicleDetail.ZTSCLevy as ZTSC_Levy, VehicleDetail.AdministrationAmt, ";
            query += " cast(VehicleDetail.Premium * 30 / 100 as decimal(10, 2))    as Comission_Amount, VehicleDetail.IncludeRadioLicenseCost, ";
            query += " CASE WHEN IncludeRadioLicenseCost = 1 THEN VehicleDetail.RadioLicenseCost else 0 end as RadioLicenseCost, VehicleDetail.VehicleLicenceFee as Zinara_License_Fee, ";
            query += " VehicleDetail.RenewalDate as PolicyRenewalDate, VehicleDetail.IsActive,RegistrationNo, VehicleDetail.RenewPolicyNumber as RenewPolicyNumber, ";
            query += " VehicleDetail.BusinessSourceDetailId,VehicleDetail.ALMBranchId, SummaryDetail.id as SummaryDetailId, BusinessSource.Source as BusinessSourceName, SourceDetail.FirstName + ' ' + SourceDetail.LastName as SourceDetailName, VehicleDetail.SumInsured from PolicyDetail ";
            query += " join Customer on PolicyDetail.CustomerId = Customer.Id ";
            query += " join VehicleDetail on PolicyDetail.Id = VehicleDetail.PolicyId ";
            query += "join SummaryVehicleDetail on VehicleDetail.id = SummaryVehicleDetail.VehicleDetailsId ";
            query += " join SummaryDetail on SummaryDetail.id = SummaryVehicleDetail.SummaryDetailId ";
            //query += "  join PaymentInformation on SummaryDetail.Id=PaymentInformation.SummaryDetailId ";
            query += " left join PaymentMethod on SummaryDetail.PaymentMethodId = PaymentMethod.Id ";
            query += "join PaymentTerm on VehicleDetail.PaymentTermId = PaymentTerm.Id ";
            query += " left join CoverType on VehicleDetail.CoverTypeId = CoverType.Id ";
            query += " left join Currency on VehicleDetail.CurrencyId = Currency.Id ";
            query += " left join BusinessSource on BusinessSource.Id = VehicleDetail.BusinessSourceDetailId ";
            query += " left   join SourceDetail on VehicleDetail.BusinessSourceDetailId = SourceDetail.Id join AspNetUsers on AspNetUsers.id=customer.UserID join AspNetUserRoles on AspNetUserRoles.UserId=AspNetUsers.Id ";
            if (_model.iscancelrequired == false)
            {
                query += " where (VehicleDetail.IsActive = 1 or VehicleDetail.IsActive = null) and   VehicleDetail.ALMBranchId<>2 and VehicleDetail.ALMBranchId<>1  and SummaryDetail.isQuotation=0  and (  CONVERT(date, VehicleDetail.TransactionDate) >= convert(date, '" + _model.FormDate + "', 101)  and CONVERT(date, VehicleDetail.TransactionDate) <= convert(date, '" + _model.EndDate + "', 101))  ";
            }
            else
            {
                query += " where VehicleDetail.ALMBranchId<>2 and VehicleDetail.ALMBranchId<>1  and SummaryDetail.isQuotation=0  and (  CONVERT(date, VehicleDetail.TransactionDate) >= convert(date, '" + _model.FormDate + "', 101)  and CONVERT(date, VehicleDetail.TransactionDate) <= convert(date, '" + _model.EndDate + "', 101))  ";
            }
            if (_model.ReportTypeId == (int)ReportTypeEnum.ALM)
                query += "and ALMBranchId <>0 ";

            if (_model.PaymentStatus != null)
                query += " and SummaryDetail.PaymentMethodId= " + _model.PaymentStatus.ToString() + " ";

            query += " order by  VehicleDetail.Id desc";





            ListGrossWrittenPremiumReport = InsuranceContext.Query(query).
         Select(x => new GrossWrittenPremiumReportModels()
         {
             PaymentMethodId = x.PaymentMethodId,
             Policy_Number = x.Policy_Number,
             RegistrationNo = x.RegistrationNo,
             BranchName = x.BranchName,
             ALMBranchId = x.ALMBranchId == null ? 0 : Convert.ToInt32(x.ALMBranchId),
             PolicyCreatedBy = x.PolicyCreatedBy,
             Customer_Name = x.Customer_Name,
             Transaction_date = x.Transaction_date.ToShortDateString(),
             CoverNoteNum = x.CoverNoteNum,
             Payment_Mode = x.Payment_Mode,
             Payment_Term = x.Payment_Term,
             CoverType = x.CoverType,
             Currency = x.Currency,
             Premium_due = x.Premium_due == null ? 0 : x.Premium_due,
             Stamp_duty = x.Stamp_duty == null ? 0 : x.Stamp_duty,
             ZTSC_Levy = x.ZTSC_Levy == null ? 0 : x.ZTSC_Levy,
             ALMId = x.ALMId,
             Comission_Amount = x.Comission_Amount == null ? 0 : x.Comission_Amount,
                    //IncludeRadioLicenseCost = x.IncludeRadioLicenseCost,
                    RadioLicenseCost = x.RadioLicenseCost == null ? 0 : x.RadioLicenseCost,
             Zinara_License_Fee = x.Zinara_License_Fee == null ? 0 : x.Zinara_License_Fee,
             PolicyRenewalDate = x.PolicyRenewalDate,
             IsActive = x.IsActive,
             RenewPolicyNumber = x.RenewPolicyNumber,
             BusinessSourceName = x.BusinessSourceName,
                    //IncludeRadioLicenseCost = x.IncludeRadioLicenseCost,
                    SourceDetailName = x.SourceDetailName,
             SummaryDetailId = x.SummaryDetailId,
             Sum_Insured = x.SumInsured == null ? 0 : Convert.ToDecimal(x.SumInsured),
             AdministrationAmt = x.AdministrationAmt,
             ReceiptNumber = RecieptNumber(x.policyId, x.RenewPolicyNumber, recieptList)
         }).ToList();

            if (_model.ReportTypeId == (int)ReportTypeEnum.CallCenter)
                ListGrossWrittenPremiumReport = ListGrossWrittenPremiumReport.Where(c => c.BranchName == CallCenterBranch).ToList();




            List<GrossWrittenPremiumReportModels> list = new List<GrossWrittenPremiumReportModels>();

            foreach (var item in ListGrossWrittenPremiumReport)
            {
                var paymentDetail = paymentInformationsList.Where(c => c.SummaryDetailId == item.SummaryDetailId);
                if (paymentDetail == null)
                    continue;

                //var branchDetail = branchList.FirstOrDefault(c => c.Id == item.ALMBranchId);
                //if(branchDetail!=null)
                //    continue;

                GrossWrittenPremiumReportModels model = new GrossWrittenPremiumReportModels();

                model.Policy_Number = item.Policy_Number;
                model.RegistrationNo = item.RegistrationNo;
                model.BranchName = item.BranchName;
                model.PolicyCreatedBy = item.PolicyCreatedBy;
                model.Customer_Name = item.Customer_Name;
                model.Transaction_date = item.Transaction_date;
                model.CoverNoteNum = item.CoverNoteNum;
                model.Payment_Mode = item.Payment_Mode;
                model.Payment_Term = item.Payment_Term;
                model.CoverType = item.CoverType;
                model.Currency = item.Currency;
                model.RenewPolicyNumber = item.RenewPolicyNumber;
                model.ALMId = item.ALMId;
                model.IsActive = item.IsActive;
                model.PolicyRenewalDate = item.PolicyRenewalDate;
                model.SourceDetailName = item.SourceDetailName;
                model.BusinessSourceName = item.BusinessSourceName;
                model.PaymentMethodId = item.PaymentMethodId;
                model.ReceiptNumber = item.ReceiptNumber;
                model.AdministrationAmt = item.AdministrationAmt;
                //IncludeRadioLicenseCost = x.IncludeRadioLicenseCost,

                var index = list.FindIndex(c => c.Policy_Number == item.Policy_Number);
                if (index != -1)
                {
                    list[index].Premium_due += item.Premium_due;
                    list[index].Stamp_duty += item.Stamp_duty;
                    list[index].ZTSC_Levy += item.ZTSC_Levy;
                    list[index].Comission_Amount += item.Comission_Amount;
                    //IncludeRadioLicenseCost = x.IncludeRadioLicenseCost,
                    list[index].RadioLicenseCost += item.RadioLicenseCost;
                    list[index].Zinara_License_Fee += item.Zinara_License_Fee;
                    list[index].Sum_Insured += item.Sum_Insured;
                    list[index].GrandPremium = item.Premium_due + item.Zinara_License_Fee.Value + item.RadioLicenseCost.Value + item.Stamp_duty + item.ZTSC_Levy;
                }
                else
                {
                    model.Premium_due = item.Premium_due;
                    model.Stamp_duty = item.Stamp_duty;
                    model.ZTSC_Levy = item.ZTSC_Levy;
                    model.Comission_Amount = item.Comission_Amount;
                    //IncludeRadioLicenseCost = x.IncludeRadioLicenseCost,
                    model.RadioLicenseCost = item.RadioLicenseCost;
                    model.Zinara_License_Fee = item.Zinara_License_Fee;
                    model.Sum_Insured = item.Sum_Insured;
                    model.GrandPremium = item.Premium_due + item.Zinara_License_Fee.Value + item.RadioLicenseCost.Value + item.Stamp_duty + item.ZTSC_Levy;
                    list.Add(model);
                }
            }

            //_model.ListGrossWrittenPremiumReportdata = ListGrossWrittenPremiumReport.OrderByDescending(p => p.Id).ToList();
            _model.ListGrossWrittenPremiumReportdata = list.OrderByDescending(p => p.Id).ToList();


            var endorsmentList = GetGWPEndorsmentReport(_model);
            _model.ListGrossWrittenPremiumReportdata.AddRange(endorsmentList);

            //if (_model.iscancelrequired == false)
            //{
            //    ListGrossWrittenPremiumReport = _model.ListGrossWrittenPremiumReportdata.Where(m => m.IsActive == true || m.IsActive==null).ToList();
            //}
  
            return View("GrossWrittenPremiumReport", _model);
        }


        private List<GrossWrittenPremiumReportModels> SearchGwpReportByDate(string fromDate, string endDate)
        {
            var recieptList = InsuranceContext.ReceiptHistorys.All().ToList();

            List<GrossWrittenPremiumReportModels> ListGrossWrittenPremiumReport = new List<GrossWrittenPremiumReportModels>();
            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["Insurance"].ConnectionString;

            using (SqlConnection myConnection = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("sp_SearchGwp", myConnection))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@FromDate", fromDate);
                    cmd.Parameters.AddWithValue("@EndDate", endDate);
                    myConnection.Open();

                    //  SqlParameter custId = cmd.Parameters.AddWithValue("@CustomerId", 10);

                    using (SqlDataReader dr = cmd.ExecuteReader())
                    {
                        while (dr.Read())
                        {

                            GrossWrittenPremiumReportModels model = new GrossWrittenPremiumReportModels();
                            model.PaymentMethodId = dr["PaymentMethodId"] == DBNull.Value ? 0 : Convert.ToInt32(dr["PaymentMethodId"]);
                            model.Policy_Number = dr["Policy_Number"] == DBNull.Value ? "" : Convert.ToString(dr["Policy_Number"]);
                            model.RegistrationNo = dr["RegistrationNo"] == DBNull.Value ? "" : Convert.ToString(dr["RegistrationNo"]);

                            model.BranchName = dr["BranchName"] == DBNull.Value ? "" : Convert.ToString(dr["BranchName"]);
                            model.ALMBranchId = dr["ALMBranchId"] == DBNull.Value ? 0 : Convert.ToInt32(dr["ALMBranchId"]);
                            model.PolicyCreatedBy = dr["PolicyCreatedBy"] == DBNull.Value ? "" : Convert.ToString(dr["PolicyCreatedBy"]);
                            model.Customer_Name = dr["Customer_Name"] == DBNull.Value ? "" : Convert.ToString(dr["Customer_Name"]);

                            model.Transaction_date = dr["Transaction_date"] == DBNull.Value ? DateTime.Now.ToShortDateString() : Convert.ToDateTime(dr["Transaction_date"]).ToShortDateString();
                            model.CoverNoteNum = dr["CoverNoteNum"] == DBNull.Value ? "" : Convert.ToString(dr["CoverNoteNum"]);
                            model.Payment_Mode = dr["Payment_Mode"] == DBNull.Value ? "Cash" : Convert.ToString(dr["Payment_Mode"]);
                            model.Payment_Term = dr["Payment_Term"] == DBNull.Value ? "" : Convert.ToString(dr["Payment_Term"]);
                            model.CoverType = dr["CoverType"] == DBNull.Value ? "" : Convert.ToString(dr["CoverType"]);

                            model.Currency = dr["Currency"] == DBNull.Value ? "" : Convert.ToString(dr["Currency"]);
                            model.Premium_due = dr["Premium_due"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["Premium_due"]);
                            model.Zinara_License_Fee = dr["Zinara_License_Fee"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["Zinara_License_Fee"]);
                            model.GrandPremium = Convert.ToDecimal(model.Premium_due + model.Zinara_License_Fee);
                            model.Stamp_duty = dr["Stamp_duty"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["Stamp_duty"]);
                            model.ZTSC_Levy = dr["ZTSC_Levy"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["ZTSC_Levy"]);
                            model.ALMId = dr["ALMId"] == DBNull.Value ? "" : Convert.ToString(dr["ALMId"]);
                            model.Comission_Amount = dr["Comission_Amount"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["Comission_Amount"]);
                            model.RadioLicenseCost = dr["RadioLicenseCost"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["RadioLicenseCost"]);
                            model.Zinara_License_Fee = dr["Zinara_License_Fee"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["Zinara_License_Fee"]);
                            model.PolicyRenewalDate = dr["PolicyRenewalDate"] == DBNull.Value ? DateTime.Now : Convert.ToDateTime(dr["PolicyRenewalDate"]);
                            model.IsActive = dr["IsActive"] == DBNull.Value ? false : Convert.ToBoolean(dr["IsActive"]);

                            model.RenewPolicyNumber = dr["RenewPolicyNumber"] == DBNull.Value ? "" : Convert.ToString(dr["RenewPolicyNumber"]);
                            model.BusinessSourceName = dr["BusinessSourceName"] == DBNull.Value ? "" : Convert.ToString(dr["BusinessSourceName"]);

                            model.SourceDetailName = dr["SourceDetailName"] == DBNull.Value ? "" : Convert.ToString(dr["SourceDetailName"]);
                            model.SummaryDetailId = dr["SummaryDetailId"] == DBNull.Value ? 0 : Convert.ToInt32(dr["SummaryDetailId"]);

                            model.Sum_Insured = dr["SumInsured"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["SumInsured"]);
                            model.AdministrationAmt = dr["AdministrationAmt"] == DBNull.Value ? 0 : Convert.ToDecimal(dr["AdministrationAmt"]);

                            int policyId = dr["policyId"] == DBNull.Value ? 0 : Convert.ToInt32(dr["policyId"]);

                            model.ReceiptNumber = RecieptNumber(policyId, model.RenewPolicyNumber, recieptList);
                            ListGrossWrittenPremiumReport.Add(model);
                        }
                    }
                }
            }
            return ListGrossWrittenPremiumReport;
        }


        public List<PaymentMethod> PaymentMethodsList()
        {
            List<PaymentMethod> list = new List<PaymentMethod>();
            string query = "select * from PaymentMethod";
            var result = InsuranceContext.Query(query).Select(x => new PaymentMethod()
            {
                Id = x.Id,
                Name = x.Name
            }).ToList();
            return (List<PaymentMethod>)result;
        }



        public ActionResult SearchGrossReportsForService(GrossWrittenPremiumReportSearchModels _model)
        {

            List<GrossWrittenPremiumReportModels> ListGrossWrittenPremiumReport = new List<GrossWrittenPremiumReportModels>();
            ListGrossWrittenPremiumReportModels _ListGrossWrittenPremiumReport = new ListGrossWrittenPremiumReportModels();
            _ListGrossWrittenPremiumReport.ListGrossWrittenPremiumReportdata = new List<GrossWrittenPremiumReportModels>();

            var yesterdayDate = DateTime.Now.AddDays(-1);

            var query = " select PolicyDetail.PolicyNumber as Policy_Number, Customer.ALMId, case when Customer.ALMId is null  then  [dbo].fn_GetUserCallCenterAgent(SummaryDetail.CreatedBy) else [dbo].fn_GetUserALM(Customer.BranchId) end  as PolicyCreatedBy, Customer.FirstName + ' ' + Customer.LastName as Customer_Name,VehicleDetail.TransactionDate as Transaction_date, ";
            query += "  case when Customer.id=SummaryDetail.CreatedBy then [dbo].fn_GetUserBranch(Customer.id) else [dbo].fn_GetUserBranch(SummaryDetail.CreatedBy) end as BranchName, ";
            query += " VehicleDetail.CoverNote as CoverNoteNum, PaymentMethod.Name as Payment_Mode, PaymentTerm.Name as Payment_Term,CoverType.Name as CoverType, Currency.Name as Currency, ";
            query += " VehicleDetail.Premium + VehicleDetail.StampDuty + VehicleDetail.ZTSCLevy as Premium_due, VehicleDetail.StampDuty as Stamp_duty, VehicleDetail.ZTSCLevy as ZTSC_Levy, ";
            query += " cast(VehicleDetail.Premium * 30 / 100 as decimal(10, 2))    as Comission_Amount, VehicleDetail.IncludeRadioLicenseCost, ";
            query += " CASE WHEN IncludeRadioLicenseCost = 1 THEN VehicleDetail.RadioLicenseCost else 0 end as RadioLicenseCost, VehicleDetail.VehicleLicenceFee as Zinara_License_Fee, ";
            query += " VehicleDetail.RenewalDate as PolicyRenewalDate, VehicleDetail.IsActive, VehicleDetail.RenewPolicyNumber as RenewPolicyNumber, ";
            query += " VehicleDetail.BusinessSourceDetailId, BusinessSource.Source as BusinessSourceName, SourceDetail.FirstName + ' ' + SourceDetail.LastName as SourceDetailName from PolicyDetail ";
            query += " join Customer on PolicyDetail.CustomerId = Customer.Id ";
            query += " join VehicleDetail on PolicyDetail.Id = VehicleDetail.PolicyId ";
            query += "join SummaryVehicleDetail on VehicleDetail.id = SummaryVehicleDetail.VehicleDetailsId ";
            query += " join SummaryDetail on SummaryDetail.id = SummaryVehicleDetail.SummaryDetailId ";
            query += " join PaymentMethod on SummaryDetail.PaymentMethodId = PaymentMethod.Id ";
            query += "join PaymentTerm on VehicleDetail.PaymentTermId = PaymentTerm.Id ";
            query += " left join CoverType on VehicleDetail.CoverTypeId = CoverType.Id ";
            query += " left join Currency on VehicleDetail.CurrencyId = Currency.Id ";
            query += " left join BusinessSource on BusinessSource.Id = VehicleDetail.BusinessSourceDetailId ";
            query += " left   join SourceDetail on VehicleDetail.BusinessSourceDetailId = SourceDetail.Id join AspNetUsers on AspNetUsers.id=customer.UserID join AspNetUserRoles on AspNetUserRoles.UserId=AspNetUsers.Id ";
            query += " where (VehicleDetail.IsActive = 1 or VehicleDetail.IsActive = null) and SummaryDetail.isQuotation=0 and (  CONVERT(date, VehicleDetail.TransactionDate) >= convert(date, '" + yesterdayDate.ToShortDateString() + "', 101)  order by  VehicleDetail.Id desc ";



            ListGrossWrittenPremiumReport = InsuranceContext.Query(query).
                Select(x => new GrossWrittenPremiumReportModels()
                {

                    Policy_Number = x.Policy_Number,
                    BranchName = x.BranchName,
                    PolicyCreatedBy = x.PolicyCreatedBy,
                    Customer_Name = x.Customer_Name,
                    Transaction_date = x.Transaction_date.ToShortDateString(),
                    CoverNoteNum = x.CoverNoteNum,
                    Payment_Mode = x.Payment_Mode,
                    Payment_Term = x.Payment_Term,
                    CoverType = x.CoverType,
                    Currency = x.Currency,
                    Premium_due = x.Premium_due,
                    Stamp_duty = x.Stamp_duty,
                    ZTSC_Levy = x.ZTSC_Levy,
                    ALMId = x.ALMId,
                    Comission_Amount = x.Comission_Amount,
                    //IncludeRadioLicenseCost = x.IncludeRadioLicenseCost,
                    RadioLicenseCost = x.RadioLicenseCost,
                    Zinara_License_Fee = x.Zinara_License_Fee,
                    PolicyRenewalDate = x.PolicyRenewalDate,
                    IsActive = x.IsActive,
                    RenewPolicyNumber = x.RenewPolicyNumber,
                    // BusinessSourceName = x.BusinessSourceName,
                    //IncludeRadioLicenseCost = x.IncludeRadioLicenseCost,
                    SourceDetailName = x.SourceDetailName,
                }).ToList();

            _model.ListGrossWrittenPremiumReportdata = ListGrossWrittenPremiumReport.OrderByDescending(p => p.Id).ToList();
            return View("GrossWrittenPremiumReport", _model);


        }

        public ActionResult ReinsuranceCommissionReport()
        {
            //ReinsuranceCommissionReportModel ReinsurancReportListmodel = new ReinsuranceCommissionReportModel();
            var ListReinsurancelist = new List<ReinsuranceCommissionReportModel>();

            ReinsuranceCommissionReportModel obj = new ReinsuranceCommissionReportModel();
            ReinsuranceCommissionSearchReportModel model = new ReinsuranceCommissionSearchReportModel();
            ListReinsurance _Reinsurance = new ListReinsurance();
            _Reinsurance.Reinsurance = new List<ReinsuranceCommissionReportModel>();
            var VehicleDetails = InsuranceContext.VehicleDetails.All(where: "IsActive ='True'").ToList();

            var currenyList = _summaryDetailService.GetAllCurrency();



            foreach (var item in VehicleDetails)
            {
                var Policy = InsuranceContext.PolicyDetails.Single(item.PolicyId);
                var Customer = InsuranceContext.Customers.Single(item.CustomerId);
                var Vehicle = InsuranceContext.VehicleDetails.Single(item.Id);
                var ReinsuranceTransaction = InsuranceContext.ReinsuranceTransactions.All(where: $"VehicleId={Vehicle.Id}").ToList();

                var currencyDetails = currenyList.FirstOrDefault(c => c.Id == Vehicle.CurrencyId);
                if (currencyDetails != null)
                    obj.Currency = currencyDetails.Name;
                else
                    obj.Currency = "USD";



                if (ReinsuranceTransaction.Count > 0 && ReinsuranceTransaction != null)
                {
                    ListReinsurancelist.Add(new ReinsuranceCommissionReportModel()
                    {
                        FirstName = Customer.FirstName,
                        LastName = Customer.LastName,
                        PolicyNumber = Policy.PolicyNumber,
                        StartDate = Vehicle.CoverStartDate == null ? null : Vehicle.CoverStartDate.Value.ToString("dd/MM/yyyy"),
                        EndDate = Vehicle.CoverEndDate == null ? null : Vehicle.CoverEndDate.Value.ToString("dd/MM/yyyy"),
                        TransactionDate = Vehicle.TransactionDate == null ? null : Vehicle.TransactionDate.Value.ToString("dd/MM/yyyy"),
                        AutoFacultativeReinsurance = (ReinsuranceTransaction != null && ReinsuranceTransaction.Count > 0 ? Convert.ToDecimal(ReinsuranceTransaction[0].ReinsuranceCommission) : 0.00m),
                        FacultativeReinsurance = (ReinsuranceTransaction != null && ReinsuranceTransaction.Count > 1 ? Convert.ToDecimal(ReinsuranceTransaction[1].ReinsuranceCommission) : 0.00m),//FacultativeReinsurance = "";
                        Currency = _summaryDetailService.GetCurrencyName(currenyList, Vehicle.CurrencyId)
                    });
                }
            }
            model.Reinsurance = ListReinsurancelist.OrderBy(x => x.FirstName).ToList();
            return View(model);
        }

        public ActionResult ReinsuranceCommissionSearchReport(ReinsuranceCommissionSearchReportModel Model)
        {

            var ListReinsurancelist = new List<ReinsuranceCommissionReportModel>();

            ReinsuranceCommissionReportModel obj = new ReinsuranceCommissionReportModel();
            ReinsuranceCommissionSearchReportModel model = new ReinsuranceCommissionSearchReportModel();
            ListReinsurance _Reinsurance = new ListReinsurance();
            _Reinsurance.Reinsurance = new List<ReinsuranceCommissionReportModel>();
            var VehicleDetails = InsuranceContext.VehicleDetails.All(where: "IsActive ='True'").ToList();
            DateTime fromDate = DateTime.Now.AddDays(-1);
            DateTime endDate = DateTime.Now;

            var currencyList = _summaryDetailService.GetAllCurrency();


            if (!string.IsNullOrEmpty(Model.FromDate) && !string.IsNullOrEmpty(Model.EndDate))
            {
                fromDate = Convert.ToDateTime(Model.FromDate);
                endDate = Convert.ToDateTime(Model.EndDate);
                ViewBag.fromdate = fromDate.ToString("dd/MM/yyyy");
                ViewBag.enddate = endDate.ToString("dd/MM/yyyy");
            }
            VehicleDetails = VehicleDetails.Where(c => c.TransactionDate >= fromDate && c.TransactionDate <= endDate).ToList();
            foreach (var item in VehicleDetails)
            {
                var Policy = InsuranceContext.PolicyDetails.Single(item.PolicyId);
                var Customer = InsuranceContext.Customers.Single(item.CustomerId);
                var Vehicle = InsuranceContext.VehicleDetails.Single(item.Id);
                var ReinsuranceTransaction = InsuranceContext.ReinsuranceTransactions.All(where: $"VehicleId={Vehicle.Id}").ToList();
                if (ReinsuranceTransaction.Count > 0 && ReinsuranceTransaction != null)
                {
                    ListReinsurancelist.Add(new ReinsuranceCommissionReportModel()
                    {
                        FirstName = Customer.FirstName,
                        LastName = Customer.LastName,
                        PolicyNumber = Policy.PolicyNumber,
                        StartDate = Vehicle.CoverStartDate == null ? null : Vehicle.CoverStartDate.Value.ToString("dd/MM/yyyy"),
                        EndDate = Vehicle.CoverEndDate == null ? null : Vehicle.CoverEndDate.Value.ToString("dd/MM/yyyy"),
                        TransactionDate = Vehicle.TransactionDate == null ? null : Vehicle.TransactionDate.Value.ToString("dd/MM/yyyy"),
                        AutoFacultativeReinsurance = (ReinsuranceTransaction != null && ReinsuranceTransaction.Count > 0 ? Convert.ToDecimal(ReinsuranceTransaction[0].ReinsuranceCommission) : 0.00m),
                        FacultativeReinsurance = (ReinsuranceTransaction != null && ReinsuranceTransaction.Count > 1 ? Convert.ToDecimal(ReinsuranceTransaction[1].ReinsuranceCommission) : 0.00m),
                        Currency = _summaryDetailService.GetCurrencyName(currencyList, Vehicle.CurrencyId)
                    });

                }
            }
            model.Reinsurance = ListReinsurancelist.OrderBy(x => x.FirstName).ToList();

            return View("ReinsuranceCommissionReport", model);
        }
        public ActionResult BasicCommissionReport()
        {
            //ReinsuranceCommissionReportModel ReinsurancReportListmodel = new ReinsuranceCommissionReportModel();
            var ListBasicCommissionReport = new List<BasicCommissionReportModel>();
            ListBasicCommissionReport _BasicCommissionReport = new ListBasicCommissionReport();
            _BasicCommissionReport.BasicCommissionReport = new List<BasicCommissionReportModel>();
            BasicCommissionReportSearchModel model = new BasicCommissionReportSearchModel();

            var VehicleDetails = InsuranceContext.VehicleDetails.All(where: "IsActive ='True'").ToList().OrderByDescending(c => c.Id).Take(200);

            var currenyList = _summaryDetailService.GetAllCurrency();


            foreach (var item in VehicleDetails)
            {
                var Policy = InsuranceContext.PolicyDetails.Single(item.PolicyId);
                var Customer = InsuranceContext.Customers.Single(item.CustomerId);

                if (Policy != null)
                {
                    //var Vehicle = InsuranceContext.VehicleDetails.Single(item.Id);
                    var ReinsuranceTransaction = InsuranceContext.ReinsuranceTransactions.All(where: $"VehicleId={item.Id}").ToList();
                    var commision = InsuranceContext.AgentCommissions.Single(item.AgentCommissionId);
                    if (ReinsuranceTransaction.Count > 0 && ReinsuranceTransaction != null)
                    {
                        ListBasicCommissionReport.Add(new BasicCommissionReportModel()
                        {
                            FirstName = Customer.FirstName,
                            LastName = Customer.LastName,
                            PolicyNumber = Policy.PolicyNumber,
                            TransactionDate = item.TransactionDate == null ? null : item.TransactionDate.Value.ToString("dd/MM/yyyy"),
                            SumInsured = item.SumInsured,
                            Premium = item.Premium,
                            Commission = (item.Premium - item.RoadsideAssistanceAmount - item.PassengerAccidentCoverAmount - item.ExcessBuyBackAmount - item.ExcessAmount - item.MedicalExpensesAmount) * Convert.ToDecimal(commision.CommissionAmount) / 100,
                            ManagementCommission = (item.Premium - item.RoadsideAssistanceAmount - item.PassengerAccidentCoverAmount - item.ExcessBuyBackAmount - item.ExcessAmount - item.MedicalExpensesAmount) * Convert.ToDecimal(commision.ManagementCommission) / 100,
                            Currency = _summaryDetailService.GetCurrencyName(currenyList, item.CurrencyId)
                        });
                    }
                }

            }
            model.BasicCommissionReport = ListBasicCommissionReport.OrderBy(x => x.FirstName).ToList();
            return View(model);
        }
        public ActionResult BasicCommissionSearchReport(BasicCommissionReportSearchModel Model)
        {

            //ReinsuranceCommissionReportModel ReinsurancReportListmodel = new ReinsuranceCommissionReportModel();
            var ListBasicCommissionReport = new List<BasicCommissionReportModel>();
            ListBasicCommissionReport _BasicCommissionReport = new ListBasicCommissionReport();
            _BasicCommissionReport.BasicCommissionReport = new List<BasicCommissionReportModel>();
            BasicCommissionReportSearchModel model = new BasicCommissionReportSearchModel();

            var VehicleDetails = InsuranceContext.VehicleDetails.All(where: "IsActive ='True'").ToList();

            DateTime fromDate = DateTime.Now.AddDays(-1);
            DateTime endDate = DateTime.Now;

            if (!string.IsNullOrEmpty(Model.FromDate) && !string.IsNullOrEmpty(Model.EndDate))
            {
                fromDate = Convert.ToDateTime(Model.FromDate);
                endDate = Convert.ToDateTime(Model.EndDate);
                ViewBag.fromdate = fromDate.ToString("dd/MM/yyyy");
                ViewBag.enddate = endDate.ToString("dd/MM/yyyy");
            }


            VehicleDetails = VehicleDetails.Where(c => c.TransactionDate >= fromDate && c.TransactionDate <= endDate).ToList();


            var currenyList = _summaryDetailService.GetAllCurrency();


            foreach (var item in VehicleDetails)
            {
                var Policy = InsuranceContext.PolicyDetails.Single(item.PolicyId);
                var Customer = InsuranceContext.Customers.Single(item.CustomerId);
                //var Vehicle = InsuranceContext.VehicleDetails.Single(item.Id);
                var ReinsuranceTransaction = InsuranceContext.ReinsuranceTransactions.All(where: $"VehicleId={item.Id}").ToList();
                var commision = InsuranceContext.AgentCommissions.Single(item.AgentCommissionId);
                if (ReinsuranceTransaction.Count > 0 && ReinsuranceTransaction != null)
                {
                    ListBasicCommissionReport.Add(new BasicCommissionReportModel()
                    {
                        FirstName = Customer.FirstName,
                        LastName = Customer.LastName,
                        PolicyNumber = Policy.PolicyNumber,
                        TransactionDate = item.TransactionDate == null ? null : item.TransactionDate.Value.ToString("dd/MM/yyyy"),
                        SumInsured = item.SumInsured,
                        Premium = item.Premium,
                        Commission = (item.Premium - item.RoadsideAssistanceAmount - item.PassengerAccidentCoverAmount - item.ExcessBuyBackAmount - item.ExcessAmount - item.MedicalExpensesAmount) * Convert.ToDecimal(commision.CommissionAmount) / 100,
                        ManagementCommission = (item.Premium - item.RoadsideAssistanceAmount - item.PassengerAccidentCoverAmount - item.ExcessBuyBackAmount - item.ExcessAmount - item.MedicalExpensesAmount) * Convert.ToDecimal(commision.ManagementCommission) / 100,
                        Currency = _summaryDetailService.GetCurrencyName(currenyList, item.CurrencyId)
                    });
                }
            }
            model.BasicCommissionReport = ListBasicCommissionReport.OrderBy(x => x.FirstName).ToList();
            return View("BasicCommissionReport", model);
        }
        public ActionResult ReinsuranceReport()
        {
            var ListReinsuranceReport = new List<ReinsuranceReport>();
            ListReinsuranceReport _ReinsuranceReport = new ListReinsuranceReport();
            _ReinsuranceReport.ReinsuranceReport = new List<Models.ReinsuranceReport>();
            var VehicleDetails = InsuranceContext.VehicleDetails.All(where: "IsActive ='True'").ToList();
            ReinsuranceSearchReport model = new ReinsuranceSearchReport();

            // var currenyList = InsuranceContext.Currencies.All();

            var currenyList = _summaryDetailService.GetAllCurrency();


            foreach (var item in VehicleDetails)
            {
                var Policy = InsuranceContext.PolicyDetails.Single(item.PolicyId);
                var Customer = InsuranceContext.Customers.Single(item.CustomerId);
                var Vehicle = InsuranceContext.VehicleDetails.Single(item.Id);
                var ReinsuranceTransaction = InsuranceContext.ReinsuranceTransactions.All(where: $"VehicleId={Vehicle.Id}").ToList();

                if (ReinsuranceTransaction.Count > 0 && ReinsuranceTransaction != null)
                {
                    ListReinsuranceReport.Add(new ReinsuranceReport()
                    {
                        FirstName = Customer.FirstName,
                        LastName = Customer.LastName,
                        PhoneNumber = Customer.PhoneNumber,
                        PolicyNumber = Policy.PolicyNumber,
                        StartDate = Vehicle.CoverStartDate == null ? null : Vehicle.CoverStartDate.Value.ToString("dd/MM/yyyy"),
                        EndDate = Vehicle.CoverEndDate == null ? null : Vehicle.CoverEndDate.Value.ToString("dd/MM/yyyy"),
                        TransactionDate = Vehicle.TransactionDate == null ? null : Vehicle.TransactionDate.Value.ToString("dd/MM/yyyy"),
                        SumInsured = Vehicle.SumInsured,
                        Premium = Vehicle.Premium,
                        AutoFacSumInsured = (ReinsuranceTransaction != null && ReinsuranceTransaction.Count > 0 ? Convert.ToDecimal(ReinsuranceTransaction[0].ReinsuranceAmount) : 0.00m),
                        AutoFacPremium = (ReinsuranceTransaction != null && ReinsuranceTransaction.Count > 0 ? Convert.ToDecimal(ReinsuranceTransaction[0].ReinsurancePremium) : 0.00m),
                        AutoFacCommission = (ReinsuranceTransaction != null && ReinsuranceTransaction.Count > 0 ? Convert.ToDecimal(ReinsuranceTransaction[0].ReinsuranceCommission) : 0.00m),
                        FacSumInsured = (ReinsuranceTransaction != null && ReinsuranceTransaction.Count > 1 ? Convert.ToDecimal(ReinsuranceTransaction[1].ReinsuranceAmount) : 0.00m),
                        FacPremium = (ReinsuranceTransaction != null && ReinsuranceTransaction.Count > 1 ? Convert.ToDecimal(ReinsuranceTransaction[1].ReinsurancePremium) : 0.00m),
                        FacCommission = (ReinsuranceTransaction != null && ReinsuranceTransaction.Count > 1 ? Convert.ToDecimal(ReinsuranceTransaction[1].ReinsuranceCommission) : 0.00m),
                        Currency = _summaryDetailService.GetCurrencyName(currenyList, item.CurrencyId)
                    });
                }
            }
            model.ReinsuranceReport = ListReinsuranceReport.OrderBy(x => x.FirstName).ToList();
            return View(model);
        }
        public ActionResult ReinsuranceSearchReport(ReinsuranceSearchReport Model)
        {

            var ListReinsuranceReport = new List<ReinsuranceReport>();
            ListReinsuranceReport _ReinsuranceReport = new ListReinsuranceReport();
            _ReinsuranceReport.ReinsuranceReport = new List<Models.ReinsuranceReport>();
            ReinsuranceSearchReport model = new ReinsuranceSearchReport();
            var VehicleDetails = InsuranceContext.VehicleDetails.All(where: "IsActive ='True'").ToList();


            // var VehicleDetails = InsuranceContext.VehicleDetails.All().ToList();

            DateTime fromDate = DateTime.Now.AddDays(-1);
            DateTime endDate = DateTime.Now;

            if (!string.IsNullOrEmpty(Model.FromDate) && !string.IsNullOrEmpty(Model.EndDate))
            {
                fromDate = Convert.ToDateTime(Model.FromDate);
                endDate = Convert.ToDateTime(Model.EndDate);
                ViewBag.fromdate = fromDate.ToString("dd/MM/yyyy");
                ViewBag.enddate = endDate.ToString("dd/MM/yyyy");
            }


            VehicleDetails = VehicleDetails.Where(c => c.TransactionDate >= fromDate && c.TransactionDate <= endDate).ToList();


            var currencyList = _summaryDetailService.GetAllCurrency();

            foreach (var item in VehicleDetails)
            {
                var Policy = InsuranceContext.PolicyDetails.Single(item.PolicyId);
                var Customer = InsuranceContext.Customers.Single(item.CustomerId);
                var Vehicle = InsuranceContext.VehicleDetails.Single(item.Id);
                var ReinsuranceTransaction = InsuranceContext.ReinsuranceTransactions.All(where: $"VehicleId={Vehicle.Id}").ToList();

                if (ReinsuranceTransaction.Count > 0 && ReinsuranceTransaction != null)
                {
                    ListReinsuranceReport.Add(new ReinsuranceReport()
                    {
                        FirstName = Customer.FirstName,
                        LastName = Customer.LastName,
                        PhoneNumber = Customer.PhoneNumber,
                        PolicyNumber = Policy.PolicyNumber,
                        StartDate = Vehicle.CoverStartDate == null ? null : Vehicle.CoverStartDate.Value.ToString("dd/MM/yyyy"),
                        EndDate = Vehicle.CoverEndDate == null ? null : Vehicle.CoverEndDate.Value.ToString("dd/MM/yyyy"),
                        TransactionDate = Vehicle.TransactionDate == null ? null : Vehicle.TransactionDate.Value.ToString("dd/MM/yyyy"),
                        SumInsured = Vehicle.SumInsured,
                        Premium = Vehicle.Premium,
                        AutoFacSumInsured = (ReinsuranceTransaction != null && ReinsuranceTransaction.Count > 0 ? Convert.ToDecimal(ReinsuranceTransaction[0].ReinsuranceAmount) : 0.00m),
                        AutoFacPremium = (ReinsuranceTransaction != null && ReinsuranceTransaction.Count > 0 ? Convert.ToDecimal(ReinsuranceTransaction[0].ReinsurancePremium) : 0.00m),
                        AutoFacCommission = (ReinsuranceTransaction != null && ReinsuranceTransaction.Count > 0 ? Convert.ToDecimal(ReinsuranceTransaction[0].ReinsuranceCommission) : 0.00m),
                        FacSumInsured = (ReinsuranceTransaction != null && ReinsuranceTransaction.Count > 1 ? Convert.ToDecimal(ReinsuranceTransaction[1].ReinsuranceAmount) : 0.00m),
                        FacPremium = (ReinsuranceTransaction != null && ReinsuranceTransaction.Count > 1 ? Convert.ToDecimal(ReinsuranceTransaction[1].ReinsurancePremium) : 0.00m),
                        FacCommission = (ReinsuranceTransaction != null && ReinsuranceTransaction.Count > 1 ? Convert.ToDecimal(ReinsuranceTransaction[1].ReinsuranceCommission) : 0.00m),
                        Currency = _summaryDetailService.GetCurrencyName(currencyList, item.CurrencyId)
                    });
                }
            }
            model.ReinsuranceReport = ListReinsuranceReport.OrderBy(x => x.FirstName).ToList();


            return View("ReinsuranceReport", model);
        }

        public ActionResult InsuranceReport()
        {
            var ListReinsuranceReport = new List<ReinsuranceReport>();
            ListReinsuranceReport _ReinsuranceReport = new ListReinsuranceReport();
            _ReinsuranceReport.ReinsuranceReport = new List<Models.ReinsuranceReport>();
            var VehicleDetails = InsuranceContext.VehicleDetails.All(where: "IsActive ='True'").ToList();
            ReinsuranceSearchReport model = new ReinsuranceSearchReport();

            // var currenyList = InsuranceContext.Currencies.All();

            var currenyList = _summaryDetailService.GetAllCurrency();

            var MakeList = InsuranceContext.VehicleMakes.All();
            var ModelList = InsuranceContext.VehicleModels.All();


            var PolicyList = InsuranceContext.PolicyDetails.All();
            var CustomerList = InsuranceContext.Customers.All();
            var VehicleList = InsuranceContext.VehicleDetails.All();
            var ReinsuranceTransactionList = InsuranceContext.ReinsuranceTransactions.All();



            foreach (var item in VehicleDetails.Take(100))
            {
                var Policy = PolicyList.FirstOrDefault(c => c.Id == item.PolicyId); // InsuranceContext.PolicyDetails.Single(item.PolicyId);
                var Customer = CustomerList.FirstOrDefault(c => c.Id == item.CustomerId);  //InsuranceContext.Customers.Single(item.CustomerId);
                var Vehicle = VehicleList.FirstOrDefault(c => c.Id == item.Id); //InsuranceContext.VehicleDetails.Single(item.Id);
                var ReinsuranceTransaction = ReinsuranceTransactionList.Where(c => c.VehicleId == item.Id).ToList(); //InsuranceContext.ReinsuranceTransactions.All(where: $"VehicleId={Vehicle.Id}").ToList();

                //if (ReinsuranceTransaction.Count > 0 && ReinsuranceTransaction != null)
                //{
                ListReinsuranceReport.Add(new ReinsuranceReport()
                {
                    FirstName = Customer.FirstName,
                    LastName = Customer.LastName,
                    PhoneNumber = Customer.PhoneNumber,
                    PolicyNumber = Policy.PolicyNumber,
                    StartDate = Vehicle.CoverStartDate == null ? null : Vehicle.CoverStartDate.Value.ToString("dd/MM/yyyy"),
                    EndDate = Vehicle.CoverEndDate == null ? null : Vehicle.CoverEndDate.Value.ToString("dd/MM/yyyy"),
                    TransactionDate = Vehicle.TransactionDate == null ? null : Vehicle.TransactionDate.Value.ToString("dd/MM/yyyy"),
                    SumInsured = Vehicle.SumInsured,
                    Premium = Vehicle.Premium,
                    AutoFacSumInsured = (ReinsuranceTransaction != null && ReinsuranceTransaction.Count > 0 ? Convert.ToDecimal(ReinsuranceTransaction[0].ReinsuranceAmount) : 0.00m),
                    AutoFacPremium = (ReinsuranceTransaction != null && ReinsuranceTransaction.Count > 0 ? Convert.ToDecimal(ReinsuranceTransaction[0].ReinsurancePremium) : 0.00m),
                    AutoFacCommission = (ReinsuranceTransaction != null && ReinsuranceTransaction.Count > 0 ? Convert.ToDecimal(ReinsuranceTransaction[0].ReinsuranceCommission) : 0.00m),
                    FacSumInsured = (ReinsuranceTransaction != null && ReinsuranceTransaction.Count > 1 ? Convert.ToDecimal(ReinsuranceTransaction[1].ReinsuranceAmount) : 0.00m),
                    FacPremium = (ReinsuranceTransaction != null && ReinsuranceTransaction.Count > 1 ? Convert.ToDecimal(ReinsuranceTransaction[1].ReinsurancePremium) : 0.00m),
                    FacCommission = (ReinsuranceTransaction != null && ReinsuranceTransaction.Count > 1 ? Convert.ToDecimal(ReinsuranceTransaction[1].ReinsuranceCommission) : 0.00m),
                    Currency = _summaryDetailService.GetCurrencyName(currenyList, item.CurrencyId),
                    Make = MakeList.FirstOrDefault(c => c.MakeCode == Vehicle.MakeId) == null ? "" : MakeList.FirstOrDefault(c => c.MakeCode == Vehicle.MakeId).MakeDescription,
                    Model = ModelList.FirstOrDefault(c => c.ModelCode == Vehicle.ModelId) == null ? "" : ModelList.FirstOrDefault(c => c.ModelCode == Vehicle.ModelId).ModelDescription,
                    RegistrationNumber = Vehicle.RegistrationNo

                });
                //}
            }
            model.ReinsuranceReport = ListReinsuranceReport.OrderBy(x => x.FirstName).ToList();
            return View(model);
        }
        public ActionResult InsuranceSearchReport(ReinsuranceSearchReport Model)
        {

            var ListReinsuranceReport = new List<ReinsuranceReport>();
            ListReinsuranceReport _ReinsuranceReport = new ListReinsuranceReport();
            _ReinsuranceReport.ReinsuranceReport = new List<Models.ReinsuranceReport>();
            ReinsuranceSearchReport model = new ReinsuranceSearchReport();
            var VehicleDetails = InsuranceContext.VehicleDetails.All(where: "IsActive ='True'").ToList();


            // var VehicleDetails = InsuranceContext.VehicleDetails.All().ToList();

            DateTime fromDate = DateTime.Now.AddDays(-1);
            DateTime endDate = DateTime.Now;

            var MakeList = InsuranceContext.VehicleMakes.All();
            var ModelList = InsuranceContext.VehicleModels.All();
            var PolicyList = InsuranceContext.PolicyDetails.All();
            var CustomerList = InsuranceContext.Customers.All();
            var VehicleList = InsuranceContext.VehicleDetails.All();
            var ReinsuranceTransactionList = InsuranceContext.ReinsuranceTransactions.All();

            if (!string.IsNullOrEmpty(Model.FromDate) && !string.IsNullOrEmpty(Model.EndDate))
            {
                fromDate = Convert.ToDateTime(Model.FromDate);
                endDate = Convert.ToDateTime(Model.EndDate);
                ViewBag.fromdate = fromDate.ToString("dd/MM/yyyy");
                ViewBag.enddate = endDate.ToString("dd/MM/yyyy");
            }


            VehicleDetails = VehicleDetails.Where(c => c.TransactionDate >= fromDate && c.TransactionDate <= endDate).ToList();


            var currencyList = _summaryDetailService.GetAllCurrency();

            foreach (var item in VehicleDetails)
            {
                var Policy = PolicyList.FirstOrDefault(c => c.Id == item.PolicyId); // InsuranceContext.PolicyDetails.Single(item.PolicyId);
                var Customer = CustomerList.FirstOrDefault(c => c.Id == item.CustomerId);  //InsuranceContext.Customers.Single(item.CustomerId);
                var Vehicle = VehicleList.FirstOrDefault(c => c.Id == item.Id); //InsuranceContext.VehicleDetails.Single(item.Id);
                var ReinsuranceTransaction = ReinsuranceTransactionList.Where(c => c.VehicleId == item.Id).ToList(); //InsuranceContext.ReinsuranceTransactions.All(where: $"VehicleId={Vehicle.Id}").ToList();

                //if (ReinsuranceTransaction.Count > 0 && ReinsuranceTransaction != null)
                //{
                ListReinsuranceReport.Add(new ReinsuranceReport()
                {
                    FirstName = Customer.FirstName,
                    LastName = Customer.LastName,
                    PhoneNumber = Customer.PhoneNumber,
                    PolicyNumber = Policy.PolicyNumber,
                    StartDate = Vehicle.CoverStartDate == null ? null : Vehicle.CoverStartDate.Value.ToString("dd/MM/yyyy"),
                    EndDate = Vehicle.CoverEndDate == null ? null : Vehicle.CoverEndDate.Value.ToString("dd/MM/yyyy"),
                    TransactionDate = Vehicle.TransactionDate == null ? null : Vehicle.TransactionDate.Value.ToString("dd/MM/yyyy"),
                    SumInsured = Vehicle.SumInsured,
                    Premium = Vehicle.Premium,
                    AutoFacSumInsured = (ReinsuranceTransaction != null && ReinsuranceTransaction.Count > 0 ? Convert.ToDecimal(ReinsuranceTransaction[0].ReinsuranceAmount) : 0.00m),
                    AutoFacPremium = (ReinsuranceTransaction != null && ReinsuranceTransaction.Count > 0 ? Convert.ToDecimal(ReinsuranceTransaction[0].ReinsurancePremium) : 0.00m),
                    AutoFacCommission = (ReinsuranceTransaction != null && ReinsuranceTransaction.Count > 0 ? Convert.ToDecimal(ReinsuranceTransaction[0].ReinsuranceCommission) : 0.00m),
                    FacSumInsured = (ReinsuranceTransaction != null && ReinsuranceTransaction.Count > 1 ? Convert.ToDecimal(ReinsuranceTransaction[1].ReinsuranceAmount) : 0.00m),
                    FacPremium = (ReinsuranceTransaction != null && ReinsuranceTransaction.Count > 1 ? Convert.ToDecimal(ReinsuranceTransaction[1].ReinsurancePremium) : 0.00m),
                    FacCommission = (ReinsuranceTransaction != null && ReinsuranceTransaction.Count > 1 ? Convert.ToDecimal(ReinsuranceTransaction[1].ReinsuranceCommission) : 0.00m),
                    Currency = _summaryDetailService.GetCurrencyName(currencyList, item.CurrencyId),
                    Make = MakeList.FirstOrDefault(c => c.MakeCode == Vehicle.MakeId) == null ? "" : MakeList.FirstOrDefault(c => c.MakeCode == Vehicle.MakeId).MakeDescription,
                    Model = ModelList.FirstOrDefault(c => c.ModelCode == Vehicle.ModelId) == null ? "" : ModelList.FirstOrDefault(c => c.ModelCode == Vehicle.ModelId).ModelDescription,
                    RegistrationNumber = Vehicle.RegistrationNo
                });
                //}
            }
            model.ReinsuranceReport = ListReinsuranceReport.OrderBy(x => x.FirstName).ToList();


            return View("InsuranceReport", model);
        }

        public ActionResult InsuranceAndLicenceDeliveryReport()
        {

            List<InsuranceAndLicenceDeliveryReportModel> ListLicenceDeliveryReport = new List<InsuranceAndLicenceDeliveryReportModel>();
            ListInsuranceAndLicenceDeliveryReport _LicenceDeliveryReport = new ListInsuranceAndLicenceDeliveryReport();
            _LicenceDeliveryReport.InsuranceAndLicense = new List<InsuranceAndLicenceDeliveryReportModel>();
            InsuraceAndLicenseSearchReportModel model = new InsuraceAndLicenseSearchReportModel();
            var Receipthistory = InsuranceContext.ReceiptHistorys.All().ToList().OrderByDescending(c => c.Id);
            var policydetail = InsuranceContext.PolicyDetails.All().ToList();
            var customers = InsuranceContext.Customers.All().ToList();
            var paymentmethod = InsuranceContext.PaymentMethods.All();
            var userList = UserManager.Users.ToList();

            foreach (var item in Receipthistory)
            {

                var policy = policydetail.FirstOrDefault(x => x.Id == item.PolicyId);
                var payment = paymentmethod.FirstOrDefault(x => x.Id == item.PaymentMethodId) == null ? "" : paymentmethod.FirstOrDefault(x => x.Id == item.PaymentMethodId).Name;
                var customerdetail = customers.FirstOrDefault(x => x.CustomerId == item.CreatedBy);
                if (customerdetail != null)
                {
                    var customerfirstname = customers.FirstOrDefault(x => x.CustomerId == item.CreatedBy) == null ? "" : customers.FirstOrDefault(x => x.CustomerId == item.CreatedBy).FirstName;
                    var customerlastname = customers.FirstOrDefault(x => x.CustomerId == item.CreatedBy) == null ? "" : customers.FirstOrDefault(x => x.CustomerId == item.CreatedBy).LastName;
                    var user = UserManager.FindById(customerdetail.UserID);
                    InsuranceAndLicenceDeliveryReportModel obj = new InsuranceAndLicenceDeliveryReportModel();
                    obj.CustomerName = item.CustomerName;
                    obj.Courier = customerfirstname + " " + customerlastname;
                    obj.PolicyNo = item.PolicyNumber;
                    obj.PaymentMethod = payment;
                    obj.TransactionReference = item.TransactionReference;
                    obj.Receiptno = item.Id;
                    obj.ReceiptAmount = item.AmountPaid;
                    obj.DateDeliverd = item.CreatedOn.Date;
                    obj.TimeofDelivery = item.CreatedOn.ToString("hh:mm");
                    obj.ContactDetails = user.PhoneNumber + ", " + user.Email;
                    obj.AddressOfCustomer = customerdetail.AddressLine1 + "," + customerdetail.AddressLine2;
                    obj.SignaturePath = ConfigurationManager.AppSettings["SignaturePath"] + item.SignaturePath;

                    ListLicenceDeliveryReport.Add(obj);
                }
                else { }
            }

            model.InsuranceAndLicense = ListLicenceDeliveryReport.OrderBy(x => x.CustomerName).ToList();
            return View(model);
        }

        public ActionResult InsuranceAndLicenceDeliverySearchReports(InsuraceAndLicenseSearchReportModel _model)
        {
            List<InsuranceAndLicenceDeliveryReportModel> ListLicenceDeliveryReport = new List<InsuranceAndLicenceDeliveryReportModel>();
            ListInsuranceAndLicenceDeliveryReport _LicenceDeliveryReport = new ListInsuranceAndLicenceDeliveryReport();
            _LicenceDeliveryReport.InsuranceAndLicense = new List<InsuranceAndLicenceDeliveryReportModel>();
            InsuraceAndLicenseSearchReportModel model = new InsuraceAndLicenseSearchReportModel();

            var receipthistory = InsuranceContext.ReceiptHistorys.All().ToList();
            DateTime fromDate = DateTime.Now.AddDays(-1);
            DateTime endDate = DateTime.Now;

            if (!string.IsNullOrEmpty(_model.FromDate) && !string.IsNullOrEmpty(_model.EndDate))
            {
                fromDate = Convert.ToDateTime(_model.FromDate);
                endDate = Convert.ToDateTime(_model.EndDate);


                ViewBag.fromdate = fromDate.ToString("dd/MM/yyyy");
                ViewBag.enddate = endDate.ToString("dd/MM/yyyy");

            }

            receipthistory = receipthistory.Where(c => c.DatePosted >= fromDate && c.DatePosted <= endDate).ToList();
            var policydetail = InsuranceContext.PolicyDetails.All().ToList();
            var customers = InsuranceContext.Customers.All().ToList();
            var paymentmethod = InsuranceContext.PaymentMethods.All();
            var userList = UserManager.Users.ToList();
            foreach (var item in receipthistory)
            {

                var policy = policydetail.FirstOrDefault(x => x.Id == item.PolicyId);
                var payment = paymentmethod.FirstOrDefault(x => x.Id == item.PaymentMethodId) == null ? "" : paymentmethod.FirstOrDefault(x => x.Id == item.PaymentMethodId).Name;
                var customerdetail = customers.FirstOrDefault(x => x.CustomerId == item.CreatedBy);
                if (customerdetail != null)
                {
                    var customerfirstname = customers.FirstOrDefault(x => x.CustomerId == item.CreatedBy) == null ? "" : customers.FirstOrDefault(x => x.CustomerId == item.CreatedBy).FirstName;
                    var customerlastname = customers.FirstOrDefault(x => x.CustomerId == item.CreatedBy) == null ? "" : customers.FirstOrDefault(x => x.CustomerId == item.CreatedBy).LastName;
                    var user = UserManager.FindById(customerdetail.UserID);
                    InsuranceAndLicenceDeliveryReportModel obj = new InsuranceAndLicenceDeliveryReportModel();
                    obj.CustomerName = item.CustomerName;
                    obj.Courier = customerfirstname + " " + customerlastname;
                    obj.PolicyNo = item.PolicyNumber;
                    obj.PaymentMethod = payment;
                    obj.TransactionReference = item.TransactionReference;
                    obj.Receiptno = item.Id;
                    obj.ReceiptAmount = item.AmountPaid;
                    obj.DateDeliverd = item.CreatedOn.Date;
                    obj.TimeofDelivery = item.CreatedOn.ToString("hh:mm");
                    obj.ContactDetails = user.PhoneNumber + ", " + user.Email;
                    obj.AddressOfCustomer = customerdetail.AddressLine1 + "," + customerdetail.AddressLine2;
                    ListLicenceDeliveryReport.Add(obj);
                }
                else { }
            }
            model.InsuranceAndLicense = ListLicenceDeliveryReport.OrderBy(x => x.CustomerName).ToList();
            return View("InsuranceandLicenceDeliveryReport", model);
        }

        public ActionResult RadioLicenceReport()
        {
            var ListRadioLicenceReport = new List<RadioLicenceReportModel>();
            ListRadioLicenceReport _RadioLicence = new ListRadioLicenceReport();
            _RadioLicence.RadioLicence = new List<RadioLicenceReportModel>();
            RadioLicenceSearchReportModel model = new RadioLicenceSearchReportModel();
            var VehicleDetails = InsuranceContext.VehicleDetails.All(where: "IsActive ='True'").ToList();

            var currenyList = _summaryDetailService.GetAllCurrency();


            foreach (var item in VehicleDetails)
            {
                var Policy = InsuranceContext.PolicyDetails.Single(item.PolicyId);
                var Customer = InsuranceContext.Customers.Single(item.CustomerId);
                // var Vehicle = InsuranceContext.VehicleDetails.Single(item.Id);

                ListRadioLicenceReport.Add(new RadioLicenceReportModel()
                {

                    FirstName = Customer.FirstName,
                    LastName = Customer.LastName,
                    Transaction_date = item.TransactionDate == null ? null : item.TransactionDate.Value.ToString("dd/MM/yyyy"),
                    RadioLicenseCost = item.RadioLicenseCost,
                    Policy_Number = Policy.PolicyNumber,
                    Currency = _summaryDetailService.GetCurrencyName(currenyList, item.CurrencyId)
                });
            }
            model.RadioLicence = ListRadioLicenceReport.OrderBy(x => x.FirstName).ToList();
            return View(model);
        }
        public ActionResult RadioLicenceSearchReports(RadioLicenceSearchReportModel Model)
        {


            var ListRadioLicenceReport = new List<RadioLicenceReportModel>();
            ListRadioLicenceReport _RadioLicence = new ListRadioLicenceReport();
            _RadioLicence.RadioLicence = new List<RadioLicenceReportModel>();
            RadioLicenceSearchReportModel model = new RadioLicenceSearchReportModel();
            var VehicleDetails = InsuranceContext.VehicleDetails.All(where: "IsActive ='True'").ToList();
            DateTime fromDate = DateTime.Now.AddDays(-1);
            DateTime endDate = DateTime.Now;

            if (!string.IsNullOrEmpty(Model.FromDate) && !string.IsNullOrEmpty(Model.EndDate))
            {
                fromDate = Convert.ToDateTime(Model.FromDate);
                endDate = Convert.ToDateTime(Model.EndDate);
                ViewBag.fromdate = fromDate.ToString("dd/MM/yyyy");
                ViewBag.enddate = endDate.ToString("dd/MM/yyyy");
            }


            VehicleDetails = VehicleDetails.Where(c => c.TransactionDate >= fromDate && c.TransactionDate <= endDate).ToList();


            foreach (var item in VehicleDetails)
            {
                var Policy = InsuranceContext.PolicyDetails.Single(item.PolicyId);
                var Customer = InsuranceContext.Customers.Single(item.CustomerId);
                // var Vehicle = InsuranceContext.VehicleDetails.Single(item.Id);

                ListRadioLicenceReport.Add(new RadioLicenceReportModel()
                {

                    FirstName = Customer.FirstName,
                    LastName = Customer.LastName,
                    Transaction_date = item.TransactionDate == null ? null : item.TransactionDate.Value.ToString("dd/MM/yyyy"),
                    RadioLicenseCost = item.RadioLicenseCost,
                    Policy_Number = Policy.PolicyNumber
                });
            }
            model.RadioLicence = ListRadioLicenceReport.OrderBy(x => x.FirstName).ToList();


            return View("RadioLicenceReport", model);
        }
        public ActionResult CustomerListingReport()
        {
            List<CustomerListingReportModel> ListCustomerListingReport = new List<CustomerListingReportModel>();
            ListCustomerListingReport _CustomerListingReport = new ListCustomerListingReport();

            _CustomerListingReport.CustomerListingReport = new List<CustomerListingReportModel>();

            CustomerListingSearchReportModel model = new CustomerListingSearchReportModel();
            var VehicleDetails = InsuranceContext.VehicleDetails.All(where: "IsActive ='True'").ToList();

            var userList = UserManager.Users.ToList();


            foreach (var item in VehicleDetails.Take(100))
            {
                var Policy = InsuranceContext.PolicyDetails.Single(item.PolicyId);
                var Customer = InsuranceContext.Customers.Single(item.CustomerId);
                // var Vehicle = InsuranceContext.VehicleDetails.Single(item.Id);

                var vehicleSummarydetail = InsuranceContext.SummaryVehicleDetails.Single(where: $"VehicleDetailsId='{item.Id}'");

                if (vehicleSummarydetail != null)
                {

                    var summary = InsuranceContext.SummaryDetails.Single(vehicleSummarydetail.SummaryDetailId);

                    //var _User = UserManager.FindById(Customer.UserID.ToString());

                    var _User = userList.FirstOrDefault(c => c.Id == Customer.UserID.ToString());


                    ListCustomerListingReport.Add(new CustomerListingReportModel()
                    {

                        FirstName = Customer.FirstName == null ? "" : Customer.FirstName,
                        LastName = Customer.LastName == null ? "" : Customer.LastName,
                        Gender = Customer.Gender == null ? "" : Customer.Gender,
                        EmailAddress = _User.Email == null ? "" : _User.Email,
                        ContactNumber = Customer.Countrycode == null ? "" : Customer.Countrycode + "-" + Customer.PhoneNumber == null ? "" : Customer.PhoneNumber,
                        Dateofbirth = Convert.ToDateTime(Customer.DateOfBirth),
                        NationalIdentificationNumber = Customer.NationalIdentificationNumber == null ? "" : Customer.NationalIdentificationNumber,
                        City = Customer.City == null ? "" : Customer.City,
                        Product = InsuranceContext.Products.Single(item.ProductId) == null ? "" : InsuranceContext.Products.Single(item.ProductId).ProductName,
                        VehicleMake = InsuranceContext.VehicleMakes.Single(where: $"MakeCode='{item.MakeId}'") == null ? "" : InsuranceContext.VehicleMakes.Single(where: $"MakeCode='{item.MakeId}'").MakeDescription,
                        VehicleModel = InsuranceContext.VehicleModels.Single(where: $"ModelCode='{item.ModelId}'") == null ? "" : InsuranceContext.VehicleModels.Single(where: $"ModelCode='{item.ModelId}'").ModelDescription,
                        VehicleUsage = InsuranceContext.VehicleUsages.Single(item.VehicleUsage == null ? 0 : item.VehicleUsage) == null ? "" : InsuranceContext.VehicleUsages.Single(item.VehicleUsage).VehUsage,
                        PaymentTerm = InsuranceContext.PaymentTerms.Single(item.PaymentTermId) == null ? "" : InsuranceContext.PaymentTerms.Single(item.PaymentTermId).Name,
                        PaymentType = InsuranceContext.PaymentMethods.Single(summary == null ? 0 : summary.PaymentMethodId) == null ? "Cash" : InsuranceContext.PaymentMethods.Single(summary.PaymentMethodId).Name,



                    });

                }
            }
            model.CustomerListingReport = ListCustomerListingReport.OrderBy(x => x.FirstName).ToList();

            return View(model);
        }

        public ActionResult CustomerListingSearchReport(CustomerListingSearchReportModel Model)
        {

            var ListCustomerListingReport = new List<CustomerListingReportModel>();
            ListCustomerListingReport _CustomerListingReport = new ListCustomerListingReport();

            _CustomerListingReport.CustomerListingReport = new List<CustomerListingReportModel>();

            CustomerListingSearchReportModel model = new CustomerListingSearchReportModel();
            var VehicleDetails = InsuranceContext.VehicleDetails.All(where: "IsActive ='True'").ToList();

            DateTime fromDate = DateTime.Now.AddDays(-1);
            DateTime endDate = DateTime.Now;

            if (!string.IsNullOrEmpty(Model.FromDate) && !string.IsNullOrEmpty(Model.EndDate))
            {
                fromDate = Convert.ToDateTime(Model.FromDate);
                endDate = Convert.ToDateTime(Model.EndDate);
                ViewBag.fromdate = fromDate.ToString("dd/MM/yyyy");
                ViewBag.enddate = endDate.ToString("dd/MM/yyyy");
            }

            VehicleDetails = VehicleDetails.Where(c => c.TransactionDate >= fromDate && c.TransactionDate <= endDate).ToList();

            var userList = UserManager.Users.ToList();


            foreach (var item in VehicleDetails)
            {
                var Policy = InsuranceContext.PolicyDetails.Single(item.PolicyId);
                var Customer = InsuranceContext.Customers.Single(item.CustomerId);
                // var Vehicle = InsuranceContext.VehicleDetails.Single(item.Id);

                var vehicleSummarydetail = InsuranceContext.SummaryVehicleDetails.Single(where: $"VehicleDetailsId='{item.Id}'");

                if (vehicleSummarydetail != null)
                {

                    var summary = InsuranceContext.SummaryDetails.Single(vehicleSummarydetail.SummaryDetailId);

                    //var _User = UserManager.FindById(Customer.UserID.ToString());

                    var _User = userList.FirstOrDefault(c => c.Id == Customer.UserID.ToString());


                    ListCustomerListingReport.Add(new CustomerListingReportModel()
                    {

                        FirstName = Customer.FirstName == null ? "" : Customer.FirstName,
                        LastName = Customer.LastName == null ? "" : Customer.LastName,
                        Gender = Customer.Gender == null ? "" : Customer.Gender,
                        EmailAddress = _User.Email == null ? "" : _User.Email,
                        ContactNumber = Customer.Countrycode == null ? "" : Customer.Countrycode + "-" + Customer.PhoneNumber == null ? "" : Customer.PhoneNumber,
                        Dateofbirth = Convert.ToDateTime(Customer.DateOfBirth),
                        NationalIdentificationNumber = Customer.NationalIdentificationNumber == null ? "" : Customer.NationalIdentificationNumber,
                        City = Customer.City == null ? "" : Customer.City,
                        Product = InsuranceContext.Products.Single(item.ProductId) == null ? "" : InsuranceContext.Products.Single(item.ProductId).ProductName,
                        VehicleMake = InsuranceContext.VehicleMakes.Single(where: $"MakeCode='{item.MakeId}'") == null ? "" : InsuranceContext.VehicleMakes.Single(where: $"MakeCode='{item.MakeId}'").MakeDescription,
                        VehicleModel = InsuranceContext.VehicleModels.Single(where: $"ModelCode='{item.ModelId}'") == null ? "" : InsuranceContext.VehicleModels.Single(where: $"ModelCode='{item.ModelId}'").ModelDescription,
                        VehicleUsage = InsuranceContext.VehicleUsages.Single(item.VehicleUsage == null ? 0 : item.VehicleUsage) == null ? "" : InsuranceContext.VehicleUsages.Single(item.VehicleUsage).VehUsage,
                        PaymentTerm = InsuranceContext.PaymentTerms.Single(item.PaymentTermId) == null ? "" : InsuranceContext.PaymentTerms.Single(item.PaymentTermId).Name,
                        PaymentType = InsuranceContext.PaymentMethods.Single(summary == null ? 0 : summary.PaymentMethodId) == null ? "Cash" : InsuranceContext.PaymentMethods.Single(summary.PaymentMethodId).Name,
                    });

                }
            }


            model.CustomerListingReport = ListCustomerListingReport.OrderBy(x => x.FirstName).ToList();
            return View("CustomerListingReport", model);
        }


        [Authorize(Roles = "Administrator,Reports,Finance,Team Leaders")]
        public ActionResult DailyReceiptsReport()
        {
            var ListDailyReceiptsReport = new List<DailyReceiptsReportModel>();
            DailyReceiptsSearchReportModel model = new DailyReceiptsSearchReportModel();
            SummaryDetailService _summaryDetailService = new SummaryDetailService();
            var currenyList = _summaryDetailService.GetAllCurrency();
            ViewBag.Currency = currenyList;

            var query = "select  DISTINCT top 500 VehicleDetail.PolicyId as PId,VehicleDetail.Id as VehicleId, VehicleDetail.RenewPolicyNumber as VehileRenewPolicyNum, ";
            query += " SummaryDetail.CreatedOn as TransactionDate, [dbo].[fn_GetUserCallCenterAgent] (SummaryDetail.CreatedBy) as Agent,RegistrationNo, VehicleDetail.[CurrencyId], VehicleDetail.Premium, VehicleDetail.AdministrationAmt, VehicleDetail.StampDuty, VehicleDetail.ZTSCLevy, VehicleDetail.RadioLicenseCost as RadioCost, ";
            query += " VehicleDetail.VehicleLicenceFee as ZinaraFee, ReceiptModuleHistory.*, Customer.FirstName +' ' + Customer.LastName as PolicyCreatedBy, WorkType.Name as Location, Currency.Name as Currency from ReceiptModuleHistory ";
            query += " join SummaryDetail on ReceiptModuleHistory.SummaryDetailId = SummaryDetail.id ";
            //query +=  " join Customer on SummaryDetail.CreatedBy = Customer.Id";
            query += " Left join Customer  on ReceiptModuleHistory.CreatedBy = Customer.Id  ";
            query += "join VehicleDetail  on ReceiptModuleHistory.PolicyId=VehicleDetail.PolicyId ";
            query += " left join Currency  on VehicleDetail.CurrencyId=Currency.Id ";
            query += " left join WorkType on Customer.WorkTypeId=WorkType.Id ";
            query += " where VehicleDetail.IsActive=1  and (ReceiptModuleHistory.IsActive is null or ReceiptModuleHistory.IsActive=1) order by ReceiptModuleHistory.CreatedOn desc";


            var list = InsuranceContext.Query(query)
               .Select(res => new PreviewReceiptListModel()
               {
                   Id = res.Id,
                   PolicyId = res.PId,
                   CustomerName = res.CustomerName,
                   PolicyNumber = res.PolicyNumber,
                   RegistrationNo = res.RegistrationNo,
                   DatePosted = res.DatePosted,
                   TransactionDate = res.TransactionDate,
                   AmountDue = res.AmountDue == null ? 0 : res.AmountDue,
                   AmountPaid = res.AmountPaid == null ? 0 : res.AmountPaid,
                   Balance = res.Balance == null ? "0" : res.Balance,
                   paymentMethodType = (res.PaymentMethodId == 1 ? "Cash" : (res.PaymentMethodId == 2 ? "Ecocash" : (res.PaymentMethodId == 3 ? "Swipe" : "MasterVisa Card"))),
                   InvoiceNumber = res.InvoiceNumber,
                   TransactionReference = res.TransactionReference,
                   PolicyCreatedBy = res.PolicyCreatedBy,
                   Premium = res.Premium == null ? 0 : res.Premium,
                   StampDuty = res.StampDuty == null ? 0 : res.StampDuty,
                   ZTSCLevy = res.ZTSCLevy == null ? 0 : res.ZTSCLevy,
                   ZinaraFee = res.ZinaraFee == null ? 0 : res.ZinaraFee,
                   RadioCost = res.RadioCost == null ? 0 : res.RadioCost,
                   TenderedAmount = res.TenderedAmount == null ? 0 : res.TenderedAmount,
                   VehicleId = res.VehicleId,
                   RenewPolicyNumber = res.RenewPolicyNumber,
                   Agent = res.Agent,
                   Location = res.Location,
                   AdministrationAmt = res.AdministrationAmt == null ? 0 : Convert.ToDecimal(res.AdministrationAmt),
                   Currency = res.Currency
               }).ToList();


            var newList = new List<PreviewReceiptListModel>();

            foreach (var item in list)
            {
                var details = newList.FirstOrDefault(c => c.PolicyId == item.PolicyId);
                if (details == null)
                {
                    newList.Add(item);
                }
                else
                {
                    int index = newList.FindIndex(c => c.PolicyId == item.PolicyId);
                    newList[index].RadioCost += item.RadioCost;
                    newList[index].ZinaraFee += item.ZinaraFee;
                }
            }

            model.DailyReceiptsReport = newList.OrderByDescending(c => c.Id).ToList();
            return View(model);
        }

        [Authorize(Roles = "Administrator,Reports,Finance,Team Leaders")]
        public ActionResult DailyReceiptsSearchReport(DailyReceiptsSearchReportModel Model)
        {
            var ListDailyReceiptsReport = new List<DailyReceiptsReportModel>();
            DailyReceiptsSearchReportModel model = new DailyReceiptsSearchReportModel();

            SummaryDetailService _summaryDetailService = new SummaryDetailService();
            var currenyList = _summaryDetailService.GetAllCurrency();
            ViewBag.Currency = currenyList;

            var query = "select DISTINCT ReceiptModuleHistory.PolicyId as PID, VehicleDetail.Id as VehicleId, ";
            query += " SummaryDetail.CreatedOn as TransactionDate,[dbo].[fn_GetUserCallCenterAgent] (SummaryDetail.CreatedBy) as Agent, VehicleDetail.RenewPolicyNumber as VehileRenewPolicyNum, VehicleDetail.AdministrationAmt,RegistrationNo, VehicleDetail.[CurrencyId], VehicleDetail.Premium, VehicleDetail.StampDuty,VehicleDetail.ZTSCLevy, ReceiptModuleHistory.*,  VehicleDetail.RadioLicenseCost as RadioCost, VehicleDetail.VehicleLicenceFee as ZinaraFee, Customer.FirstName +' ' + Customer.LastName as PolicyCreatedBy, WorkType.Name as Location, Currency.Name as Currency from ReceiptModuleHistory ";
            query += " join SummaryDetail on ReceiptModuleHistory.SummaryDetailId = SummaryDetail.id ";
            //query += " join Customer on SummaryDetail.CreatedBy = Customer.Id";
            query += "Left join Customer  on ReceiptModuleHistory.CreatedBy = Customer.Id ";
            query += "join VehicleDetail  on ReceiptModuleHistory.PolicyId=VehicleDetail.PolicyId ";
            query += " left join Currency  on VehicleDetail.CurrencyId=Currency.Id ";
            query += " left join WorkType on Customer.WorkTypeId=WorkType.Id ";
            query += " where VehicleDetail.IsActive=1   ";
            if (Model.CurrencyId > 0)
                query += " and  CurrencyId=   " + Model.CurrencyId;

            query += " and   (  CONVERT(date, ReceiptModuleHistory.CreatedOn) >= convert(date, '" + Model.FromDate + "', 101)  and CONVERT(date, ReceiptModuleHistory.CreatedOn ) <= convert(date, '" + Model.EndDate + "', 101)) and (ReceiptModuleHistory.IsActive is null or ReceiptModuleHistory.IsActive=1) order by ReceiptModuleHistory.CreatedOn desc ";


            var list = InsuranceContext.Query(query)
               .Select(res => new PreviewReceiptListModel()
               {
                   Id = res.Id,
                   PolicyId = res.PID,
                   CustomerName = res.CustomerName,
                   PolicyNumber = res.PolicyNumber,
                   RegistrationNo = res.RegistrationNo,
                   DatePosted = res.DatePosted,
                   TransactionDate = res.TransactionDate,
                   AmountDue = res.AmountDue == null ? 0 : res.AmountDue,
                   AmountPaid = res.AmountPaid == null ? 0 : res.AmountPaid,
                   Balance = res.Balance == null ? "0" : res.Balance,
                   paymentMethodType = (res.PaymentMethodId == 1 ? "Cash" : (res.PaymentMethodId == 2 ? "Ecocash" : (res.PaymentMethodId == 3 ? "Swipe" : "MasterVisa Card"))),
                   InvoiceNumber = res.InvoiceNumber,
                   TransactionReference = res.TransactionReference,
                   PolicyCreatedBy = res.PolicyCreatedBy,
                   // Currency = _summaryDetailService.GetCurrencyName(currenyList, res.CurrencyId),
                   Premium = res.Premium == null ? 0 : res.Premium,
                   StampDuty = res.StampDuty == null ? 0 : res.StampDuty,
                   ZTSCLevy = res.ZTSCLevy == null ? 0 : res.ZTSCLevy,
                   ZinaraFee = res.ZinaraFee == null ? 0 : res.ZinaraFee,
                   RadioCost = res.RadioCost == null ? 0 : res.RadioCost,
                   TenderedAmount = res.TenderedAmount == null ? 0 : res.TenderedAmount,
                   VehicleId = res.VehicleId,
                   RenewPolicyNumber = res.VehileRenewPolicyNum,
                   Agent = res.Agent,
                   Location = res.Location,
                   AdministrationAmt = res.AdministrationAmt == null ? 0 : Convert.ToDecimal(res.AdministrationAmt),
                   Currency = res.Currency,


               }).ToList();

            ViewBag.fromdate = Model.FromDate;
            ViewBag.enddate = Model.EndDate;


            var newList = new List<PreviewReceiptListModel>();

            foreach (var item in list)
            {
                var details = newList.FirstOrDefault(c => c.PolicyId == item.PolicyId);

                if (details == null)
                {
                    newList.Add(item);
                }
                else
                {
                    int index = newList.FindIndex(c => c.PolicyId == item.PolicyId);
                    newList[index].RadioCost += item.RadioCost;
                    newList[index].ZinaraFee += item.ZinaraFee;
                }
            }


            // model.DailyReceiptsReport = newList.Where(c => c.CreatedOn >= Convert.ToDateTime(Model.FromDate) && c.CreatedOn <= Convert.ToDateTime(Model.EndDate)).OrderByDescending(c => c.CreatedOn).ToList();

            model.DailyReceiptsReport = newList;


            return View("DailyReceiptsReport", model);

            // return View("ReconciliationReport", model);
        }

        [Authorize(Roles = "Administrator,Reports,Finance,Team Leaders")]
        public ActionResult ReconciliationReport()
        {
            var ListDailyReceiptsReport = new List<PreviewReceiptListModel>();
            DailyReceiptsSearchReportModel model = new DailyReceiptsSearchReportModel();


            var currenyList = _summaryDetailService.GetAllCurrency();
            ViewBag.BranchList = InsuranceContext.Branches.All().ToList();
            var recieptList = InsuranceContext.ReceiptHistorys.All().ToList();

            var query1 = "select top 100 PolicyDetail.PolicyNumber,PolicyDetail.Id as policyId, VehicleDetail.RenewPolicyNumber, createcust.FirstName + '' + createcust.LastName as Created, prcustomer.FirstName + ' ' + prcustomer.LastName as CustomerName, SummaryDetail.CreatedOn as TransactionDate,";
            query1 += "Summarydetail.createdby , ReceiptModuleHistory.PaymentMethodId, SummaryDetail.TotalPremium, PolicyDetail.PolicyNumber as InvoiceNumber, ReceiptModuleHistory.AmountDue,";
            query1 += "ReceiptModuleHistory.Id as ReceiptNo, ReceiptModuleHistory.AmountPaid, ";
            query1 += " case  ReceiptModuleHistory.Id when 0 then 'Yes' else 'No' end as Paid, ReceiptModuleHistory.DatePosted, ";
            query1 += " ReceiptModuleHistory.Balance, VehicleDetail.CurrencyId, Branch.BranchName from Customer as prcustomer join PolicyDetail on prcustomer.Id = PolicyDetail.CustomerId";
            query1 += " join VehicleDetail on PolicyDetail.id = VehicleDetail.PolicyId";
            query1 += " join SummaryVehicleDetail on VehicleDetail.Id = SummaryVehicleDetail.VehicleDetailsId";
            query1 += " join SummaryDetail on SummaryDetail.Id = SummaryVehicleDetail.SummaryDetailId";
            // query1 += " left join ReceiptModuleHistory on ReceiptModuleHistory.SummaryDetailId = SummaryDetail.Id";
            query1 += "  join ReceiptModuleHistory on ReceiptModuleHistory.SummaryDetailId = SummaryDetail.Id";
            query1 += " left join Branch on VehicleDetail.ALMBranchId= Branch.Id ";
            query1 += " left join customer as createcust on createcust.id = summarydetail.createdby where VehicleDetail.IsActive=1  and SummaryDetail.isQuotation=0 and (ReceiptModuleHistory.IsActive is null or ReceiptModuleHistory.IsActive=1) order by ReceiptModuleHistory.CreatedOn desc";


            var list = InsuranceContext.Query(query1)
               .Select(res => new PreviewReceiptListModel()
               {
                   Id = Convert.ToInt32(res.ReceiptNo),
                   CustomerName = res.CustomerName,
                   PolicyCreatedBy = res.Created,
                   PolicyNumber = res.PolicyNumber,
                   DatePosted = Convert.ToDateTime(res.DatePosted),
                   TransactionDate = res.TransactionDate,
                   AmountDue = res.AmountDue == null ? 0 : res.AmountDue,
                   AmountPaid = res.AmountPaid == null ? 0 : res.AmountPaid,
                   Balance = res.Balance == null ? "0" : res.Balance,
                   TotalPremium = res.TotalPremium == null ? 0 : Convert.ToInt32(res.TotalPremium),
                   paymentMethodType = (res.PaymentMethodId == 1 ? "Cash" : (res.PaymentMethodId == 2 ? "Ecocash" : (res.PaymentMethodId == 3 ? "Swipe" : "MasterVisa Card"))),
                   // paymentMethodType = (res.PaymentMethodId == 1 ? "Cash" : (res.PaymentMethodId == 2 ? "Ecocash" : (res.PaymentMethodId == 3 ? "Swipe" : "MasterVisa Card"))),
                   PaymentMethodId = res.PaymentMethodId == null ? 0 : Convert.ToInt32(res.PaymentMethodId),
                   InvoiceNumber = res.InvoiceNumber,
                   Currency = _summaryDetailService.GetCurrencyName(currenyList, res.CurrencyId),
                   BranchName = res.BranchName,
                   RenewPolicyNumber = res.RenewPolicyNumber,
                   ReceiptNumber = RecieptNumber(res.policyId, res.RenewPolicyNumber, recieptList)

               }).ToList();


            model.DailyReceiptsReport = list.OrderByDescending(c => c.Id).ToList();

            return View(model);
        }

        public ActionResult ReconciliationSearchReport(DailyReceiptsSearchReportModel Model)
        {
            var ListDailyReceiptsReport = new List<PreviewReceiptListModel>();
            DailyReceiptsSearchReportModel model = new DailyReceiptsSearchReportModel();


            var currenyList = _summaryDetailService.GetAllCurrency();

            var recieptList = InsuranceContext.ReceiptHistorys.All().ToList();

            ViewBag.BranchList = InsuranceContext.Branches.All().ToList();

            var query1 = "select PolicyDetail.PolicyNumber, PolicyDetail.Id as policyId,VehicleDetail.RenewPolicyNumber, createcust.FirstName + '' + createcust.LastName as Created, prcustomer.FirstName + ' ' + prcustomer.LastName as CustomerName,   SummaryDetail.CreatedOn TransactionDate,";
            query1 += "Summarydetail.createdby , SummaryDetail.TotalPremium, PolicyDetail.PolicyNumber as InvoiceNumber, ReceiptModuleHistory.AmountDue, ReceiptModuleHistory.PaymentMethodId,";
            query1 += "ReceiptModuleHistory.Id as ReceiptNo, ReceiptModuleHistory.AmountPaid, VehicleDetail.CurrencyId , ";
            query1 += " case  ReceiptModuleHistory.Id when 0 then 'Yes' else 'No' end as Paid, ReceiptModuleHistory.DatePosted, ";
            query1 += " ReceiptModuleHistory.Balance,Branch.BranchName from Customer as prcustomer join PolicyDetail on prcustomer.Id = PolicyDetail.CustomerId";
            query1 += " join VehicleDetail on PolicyDetail.id = VehicleDetail.PolicyId";
            query1 += " join SummaryVehicleDetail on VehicleDetail.Id = SummaryVehicleDetail.VehicleDetailsId";
            query1 += " join SummaryDetail on SummaryDetail.Id = SummaryVehicleDetail.SummaryDetailId";
            // query1 += " left join ReceiptModuleHistory on ReceiptModuleHistory.SummaryDetailId = SummaryDetail.Id";
            query1 += "  join ReceiptModuleHistory on ReceiptModuleHistory.SummaryDetailId = SummaryDetail.Id";
            query1 += " left join customer as createcust on createcust.id = summarydetail.createdby ";
            query1 += " left join Branch on VehicleDetail.ALMBranchId= Branch.Id ";
            query1 += " where VehicleDetail.IsActive=1 and SummaryDetail.isQuotation=0  and (ReceiptModuleHistory.IsActive is null or ReceiptModuleHistory.IsActive=1)  ";

            if (Model.BranchId > 0)
                query1 += " and ALMBranchId =" + Model.BranchId;

            query1 += " order by ReceiptModuleHistory.CreatedOn desc";


            //var query1 = "select Distinct PolicyDetail.PolicyNumber, Customer.FirstName + ' ' + Customer.LastName as CustomerName, CONVERT(datetime, CONVERT(varchar,SummaryDetail.CreatedOn, 101)) as TransactionDate, ";
            //query1 += "SummaryDetail.TotalPremium, PolicyDetail.PolicyNumber as InvoiceNumber, ReceiptModuleHistory.AmountDue, ";
            //query1 += "ReceiptModuleHistory.Id as ReceiptNo, ReceiptModuleHistory.AmountPaid, ";
            //query1 += " case  ReceiptModuleHistory.Id when 0 then 'Yes' else 'No' end as Paid, ReceiptModuleHistory.DatePosted, ";
            //query1 += " ReceiptModuleHistory.Balance  from Customer join PolicyDetail on Customer.Id = PolicyDetail.CustomerId ";
            //query1 += " join VehicleDetail on PolicyDetail.id= VehicleDetail.PolicyId ";
            //query1 += " join SummaryVehicleDetail on VehicleDetail.Id = SummaryVehicleDetail.VehicleDetailsId";
            //query1 += " join SummaryDetail on SummaryDetail.Id= SummaryVehicleDetail.SummaryDetailId";
            //query1 += " left join ReceiptModuleHistory on ReceiptModuleHistory.SummaryDetailId= SummaryDetail.Id";




            var list = InsuranceContext.Query(query1)
               .Select(res => new PreviewReceiptListModel()
               {
                   Id = Convert.ToInt32(res.ReceiptNo),
                   CustomerName = res.CustomerName,
                   PolicyCreatedBy = res.Created,
                   PolicyNumber = res.PolicyNumber,
                   RenewPolicyNumber = res.RenewPolicyNumber,
                   DatePosted = Convert.ToDateTime(res.DatePosted),
                   TransactionDate = res.TransactionDate,
                   AmountDue = res.AmountDue == null ? 0 : res.AmountDue,
                   AmountPaid = res.AmountPaid == null ? 0 : res.AmountPaid,
                   Balance = res.Balance == null ? "0" : res.Balance,
                   TotalPremium = res.TotalPremium == null ? 0 : Convert.ToInt32(res.TotalPremium),
                   // paymentMethodType = (res.PaymentMethodId == 1 ? "Cash" : (res.PaymentMethodId == 2 ? "Ecocash" : (res.PaymentMethodId == 3 ? "Swipe" : "MasterVisa Card"))),
                   InvoiceNumber = res.InvoiceNumber,
                   PaymentMethodId = res.PaymentMethodId == null ? 0 : Convert.ToInt32(res.PaymentMethodId),
                   Currency = _summaryDetailService.GetCurrencyName(currenyList, res.CurrencyId),
                   BranchName = res.BranchName,
                   ReceiptNumber = RecieptNumber(res.policyId, res.RenewPolicyNumber, recieptList)
                   //TransactionReference = res.TransactionReference,
                   //PolicyCreatedBy = res.PolicyCreatedBy
               }).ToList();

            ViewBag.fromdate = Model.FromDate;
            ViewBag.enddate = Model.EndDate;

            model.DailyReceiptsReport = list.Where(c => Convert.ToDateTime(c.TransactionDate.ToShortDateString()) >= Convert.ToDateTime(Model.FromDate) && Convert.ToDateTime(c.TransactionDate.ToShortDateString()) <= Convert.ToDateTime(Model.EndDate)).OrderByDescending(c => c.TransactionDate).ToList();


            return View("ReconciliationReport", model);
        }

        [Authorize(Roles = "Administrator,Finance")]
        public ActionResult ActiveDeactiveReceipts()
        {
            var ListDailyReceiptsReport = new List<DailyReceiptsReportModel>();
            DailyReceiptsSearchReportModel model = new DailyReceiptsSearchReportModel();
            SummaryDetailService _summaryDetailService = new SummaryDetailService();
            var currenyList = _summaryDetailService.GetAllCurrency();


            var query = "select  DISTINCT top 500 VehicleDetail.PolicyId as PId,VehicleDetail.Id as VehicleId, VehicleDetail.RenewPolicyNumber as VehileRenewPolicyNum, ReceiptModuleHistory.isActive, ";
            query += " SummaryDetail.CreatedOn as TransactionDate, [dbo].[fn_GetUserCallCenterAgent] (SummaryDetail.CreatedBy) as Agent, VehicleDetail.[CurrencyId], VehicleDetail.Premium, VehicleDetail.AdministrationAmt, VehicleDetail.StampDuty, VehicleDetail.ZTSCLevy, VehicleDetail.RadioLicenseCost as RadioCost, VehicleDetail.VehicleLicenceFee as ZinaraFee, ReceiptModuleHistory.*, Customer.FirstName +' ' + Customer.LastName as PolicyCreatedBy, WorkType.Name as Location from ReceiptModuleHistory ";
            query += " join SummaryDetail on ReceiptModuleHistory.SummaryDetailId = SummaryDetail.id ";
            //query +=  " join Customer on SummaryDetail.CreatedBy = Customer.Id";
            query += "Left join Customer  on ReceiptModuleHistory.CreatedBy = Customer.Id  ";
            query += "join VehicleDetail  on ReceiptModuleHistory.PolicyId=VehicleDetail.PolicyId ";
            query += " left join WorkType on Customer.WorkTypeId=WorkType.Id ";
            query += " where VehicleDetail.IsActive=1 order by ReceiptModuleHistory.CreatedOn desc";




            var list = InsuranceContext.Query(query)
               .Select(res => new PreviewReceiptListModel()
               {
                   Id = res.Id,
                   PolicyId = res.PId,
                   CustomerName = res.CustomerName,
                   PolicyNumber = res.PolicyNumber,
                   DatePosted = res.DatePosted,
                   TransactionDate = res.TransactionDate,
                   AmountDue = res.AmountDue == null ? 0 : res.AmountDue,
                   AmountPaid = res.AmountPaid == null ? 0 : res.AmountPaid,
                   Balance = res.Balance == null ? "0" : res.Balance,
                   paymentMethodType = (res.PaymentMethodId == 1 ? "Cash" : (res.PaymentMethodId == 2 ? "Ecocash" : (res.PaymentMethodId == 3 ? "Swipe" : "MasterVisa Card"))),
                   InvoiceNumber = res.InvoiceNumber,
                   TransactionReference = res.TransactionReference,
                   PolicyCreatedBy = res.PolicyCreatedBy,
                   Currency = _summaryDetailService.GetCurrencyName(currenyList, res.CurrencyId),
                   Premium = res.Premium == null ? 0 : res.Premium,
                   StampDuty = res.StampDuty == null ? 0 : res.StampDuty,
                   ZTSCLevy = res.ZTSCLevy == null ? 0 : res.ZTSCLevy,
                   ZinaraFee = res.ZinaraFee == null ? 0 : res.ZinaraFee,
                   RadioCost = res.RadioCost == null ? 0 : res.RadioCost,
                   TenderedAmount = res.TenderedAmount == null ? 0 : res.TenderedAmount,
                   VehicleId = res.VehicleId,
                   RenewPolicyNumber = res.RenewPolicyNumber,
                   Agent = res.Agent,
                   Location = res.Location,
                   AdministrationAmt = res.AdministrationAmt == null ? 0 : Convert.ToDecimal(res.AdministrationAmt),
                   IsActive = res.IsActive
               }).ToList();


            var newList = new List<PreviewReceiptListModel>();

            foreach (var item in list)
            {
                var details = newList.FirstOrDefault(c => c.PolicyId == item.PolicyId);
                if (details == null)
                {
                    newList.Add(item);
                }
                else
                {
                    int index = newList.FindIndex(c => c.PolicyId == item.PolicyId);
                    newList[index].RadioCost += item.RadioCost;
                    newList[index].ZinaraFee += item.ZinaraFee;
                }
            }

            model.DailyReceiptsReport = newList.OrderByDescending(c => c.Id).ToList();
            return View(model);
        }

        [Authorize(Roles = "Administrator,Reports,Finance,Team Leaders")]
        public ActionResult ActiveDeactiveReceiptSearch(DailyReceiptsSearchReportModel Model)
        {
            var ListDailyReceiptsReport = new List<DailyReceiptsReportModel>();
            DailyReceiptsSearchReportModel model = new DailyReceiptsSearchReportModel();

            SummaryDetailService _summaryDetailService = new SummaryDetailService();
            var currenyList = _summaryDetailService.GetAllCurrency();

            var query = "select DISTINCT ReceiptModuleHistory.PolicyId as PID, VehicleDetail.Id as VehicleId, ReceiptModuleHistory.isActive, ";
            query += " SummaryDetail.CreatedOn as TransactionDate,[dbo].[fn_GetUserCallCenterAgent] (SummaryDetail.CreatedBy) as Agent, VehicleDetail.RenewPolicyNumber as VehileRenewPolicyNum, VehicleDetail.AdministrationAmt, VehicleDetail.[CurrencyId], VehicleDetail.Premium, VehicleDetail.StampDuty,VehicleDetail.ZTSCLevy, ReceiptModuleHistory.*,  VehicleDetail.RadioLicenseCost as RadioCost, VehicleDetail.VehicleLicenceFee as ZinaraFee, Customer.FirstName +' ' + Customer.LastName as PolicyCreatedBy, WorkType.Name as Location from ReceiptModuleHistory ";
            query += " join SummaryDetail on ReceiptModuleHistory.SummaryDetailId = SummaryDetail.id ";
            //query += " join Customer on SummaryDetail.CreatedBy = Customer.Id";
            query += "Left join Customer  on ReceiptModuleHistory.CreatedBy = Customer.Id ";
            query += "join VehicleDetail  on ReceiptModuleHistory.PolicyId=VehicleDetail.PolicyId ";
            query += " left join WorkType on Customer.WorkTypeId=WorkType.Id ";
            query += " where VehicleDetail.IsActive=1 ";
            query += " and   (  CONVERT(date, ReceiptModuleHistory.CreatedOn) >= convert(date, '" + Model.FromDate + "', 101)  and CONVERT(date, ReceiptModuleHistory.CreatedOn ) <= convert(date, '" + Model.EndDate + "', 101)) order by ReceiptModuleHistory.CreatedOn desc ";


            var list = InsuranceContext.Query(query)
               .Select(res => new PreviewReceiptListModel()
               {
                   Id = res.Id,
                   PolicyId = res.PID,
                   CustomerName = res.CustomerName,
                   PolicyNumber = res.PolicyNumber,
                   DatePosted = res.DatePosted,
                   TransactionDate = res.TransactionDate,
                   AmountDue = res.AmountDue == null ? 0 : res.AmountDue,
                   AmountPaid = res.AmountPaid == null ? 0 : res.AmountPaid,
                   Balance = res.Balance == null ? "0" : res.Balance,
                   paymentMethodType = (res.PaymentMethodId == 1 ? "Cash" : (res.PaymentMethodId == 2 ? "Ecocash" : (res.PaymentMethodId == 3 ? "Swipe" : "MasterVisa Card"))),
                   InvoiceNumber = res.InvoiceNumber,
                   TransactionReference = res.TransactionReference,
                   PolicyCreatedBy = res.PolicyCreatedBy,
                   Currency = _summaryDetailService.GetCurrencyName(currenyList, res.CurrencyId),
                   Premium = res.Premium == null ? 0 : res.Premium,
                   StampDuty = res.StampDuty == null ? 0 : res.StampDuty,
                   ZTSCLevy = res.ZTSCLevy == null ? 0 : res.ZTSCLevy,
                   ZinaraFee = res.ZinaraFee == null ? 0 : res.ZinaraFee,
                   RadioCost = res.RadioCost == null ? 0 : res.RadioCost,
                   TenderedAmount = res.TenderedAmount == null ? 0 : res.TenderedAmount,
                   VehicleId = res.VehicleId,
                   RenewPolicyNumber = res.VehileRenewPolicyNum,
                   Agent = res.Agent,
                   Location = res.Location,
                   AdministrationAmt = res.AdministrationAmt == null ? 0 : Convert.ToDecimal(res.AdministrationAmt),
                   IsActive = res.IsActive
               }).ToList();

            ViewBag.fromdate = Model.FromDate;
            ViewBag.enddate = Model.EndDate;


            var newList = new List<PreviewReceiptListModel>();

            foreach (var item in list)
            {
                var details = newList.FirstOrDefault(c => c.PolicyId == item.PolicyId);

                if (details == null)
                {
                    newList.Add(item);
                }
                else
                {
                    int index = newList.FindIndex(c => c.PolicyId == item.PolicyId);
                    newList[index].RadioCost += item.RadioCost;
                    newList[index].ZinaraFee += item.ZinaraFee;
                }
            }


            // model.DailyReceiptsReport = newList.Where(c => c.CreatedOn >= Convert.ToDateTime(Model.FromDate) && c.CreatedOn <= Convert.ToDateTime(Model.EndDate)).OrderByDescending(c => c.CreatedOn).ToList();

            model.DailyReceiptsReport = newList;


            return View("ActiveDeactiveReceipts", model);

            // return View("ReconciliationReport", model);
        }


        [Authorize(Roles = "Administrator,Reports,Finance")]
        public ActionResult LapsedPoliciesReport()
        {
            var ListLapsedPoliciesReport = new List<LapsedPoliciesReportModels>();
            ListLapsedPoliciesReport _LapsedPoliciesReport = new ListLapsedPoliciesReport();
            _LapsedPoliciesReport.LapsedPoliciesReport = new List<LapsedPoliciesReportModels>();
            LapsedPoliciesSearchReportModels _model = new LapsedPoliciesSearchReportModels();
            var whereClause = "isLapsed = 'True' or " + $"CAST(RenewalDate as date) < '{DateTime.Now.ToString("yyyy-MM-dd")}'";
            var VehicleDetails = InsuranceContext.VehicleDetails.All(where: whereClause).ToList();

            var currenyList = _summaryDetailService.GetAllCurrency();

            foreach (var item in VehicleDetails)
            {
                var Policy = InsuranceContext.PolicyDetails.Single(item.PolicyId);
                var Customer = InsuranceContext.Customers.Single(item.CustomerId);
                var Vehicle = InsuranceContext.VehicleDetails.Single(item.Id);
                var make = InsuranceContext.VehicleMakes.Single(where: $"MakeCode='{item.MakeId}'");
                var model = InsuranceContext.VehicleModels.Single(where: $"ModelCode='{item.ModelId}'");
                var ReinsuranceTransaction = InsuranceContext.ReinsuranceTransactions.All(where: $"VehicleId={Vehicle.Id}").ToList();
                if (ReinsuranceTransaction.Count > 0 && ReinsuranceTransaction != null)
                {
                    ListLapsedPoliciesReport.Add(new LapsedPoliciesReportModels()
                    {
                        customerName = Customer.FirstName + " " + Customer.LastName,
                        contactDetails = Customer.Countrycode + "-" + Customer.PhoneNumber,
                        Premium = Vehicle.Premium,
                        sumInsured = Vehicle.SumInsured,
                        vehicleMake = make.MakeDescription,
                        vehicleModel = model.ModelDescription,
                        startDate = Vehicle.CoverStartDate == null ? null : Vehicle.CoverStartDate.Value.ToString("dd/MM/yyyy"),
                        endDate = Vehicle.CoverEndDate == null ? null : Vehicle.CoverEndDate.Value.ToString("dd/MM/yyyy"),
                        Currency = _summaryDetailService.GetCurrencyName(currenyList, item.CurrencyId)
                    });
                }
            }
            _model.LapsedPoliciesReport = ListLapsedPoliciesReport.OrderBy(x => x.customerName).ToList();
            return View(_model);
        }
        public ActionResult LapsedPoliciesSearchReport(LapsedPoliciesSearchReportModels Model)
        {
            var ListLapsedPoliciesReport = new List<LapsedPoliciesReportModels>();
            ListLapsedPoliciesReport _LapsedPoliciesReport = new ListLapsedPoliciesReport();
            _LapsedPoliciesReport.LapsedPoliciesReport = new List<LapsedPoliciesReportModels>();
            LapsedPoliciesSearchReportModels _model = new LapsedPoliciesSearchReportModels();
            var whereClause = "isLapsed = 'True' or " + $"CAST(RenewalDate as date) < '{DateTime.Now.ToString("yyyy-MM-dd")}'";
            var VehicleDetails = InsuranceContext.VehicleDetails.All(where: whereClause).ToList();

            #region
            DateTime fromDate = DateTime.Now.AddDays(-1);
            DateTime endDate = DateTime.Now;
            if (!string.IsNullOrEmpty(Model.FromDate) && !string.IsNullOrEmpty(Model.EndDate))
            {
                fromDate = Convert.ToDateTime(Model.FromDate);
                endDate = Convert.ToDateTime(Model.EndDate);
            }
            VehicleDetails = VehicleDetails.Where(c => c.TransactionDate >= fromDate && c.TransactionDate <= endDate).ToList();

            #endregion


            foreach (var item in VehicleDetails)
            {
                var Policy = InsuranceContext.PolicyDetails.Single(item.PolicyId);
                var Customer = InsuranceContext.Customers.Single(item.CustomerId);
                var Vehicle = InsuranceContext.VehicleDetails.Single(item.Id);
                var make = InsuranceContext.VehicleMakes.Single(where: $"MakeCode='{item.MakeId}'");
                var model = InsuranceContext.VehicleModels.Single(where: $"ModelCode='{item.ModelId}'");
                var ReinsuranceTransaction = InsuranceContext.ReinsuranceTransactions.All(where: $"VehicleId={Vehicle.Id}").ToList();
                if (ReinsuranceTransaction.Count > 0 && ReinsuranceTransaction != null)
                {
                    ListLapsedPoliciesReport.Add(new LapsedPoliciesReportModels()
                    {
                        customerName = Customer.FirstName + " " + Customer.LastName,
                        contactDetails = Customer.Countrycode + "-" + Customer.PhoneNumber,
                        Premium = Vehicle.Premium,
                        sumInsured = Vehicle.SumInsured,
                        vehicleMake = make.MakeDescription,
                        vehicleModel = model.ModelDescription,
                        startDate = Vehicle.CoverStartDate == null ? null : Vehicle.CoverStartDate.Value.ToString("dd/MM/yyyy"),
                        endDate = Vehicle.CoverEndDate == null ? null : Vehicle.CoverEndDate.Value.ToString("dd/MM/yyyy")
                    });
                }
            }

            ViewBag.fromdate = Model.FromDate;

            ViewBag.enddate = Model.EndDate;
            _model.LapsedPoliciesReport = ListLapsedPoliciesReport.OrderBy(x => x.customerName).ToList();


            return View("LapsedPoliciesReport", _model);
        }

        public ActionResult ProductivityReport()
        {

            //var ListProductiviyReport = new List<ProductiviyReportModel>();
            List<ProductiviyReportModel> listProductiviyReport = new List<ProductiviyReportModel>();
            List<ProductiviyReportModel> listProductiviyReport_Endorsed = new List<ProductiviyReportModel>();
            ListProductiviyReportModel _listListProductiviyReport = new ListProductiviyReportModel();
            _listListProductiviyReport.ListProductiviyReport = new List<ProductiviyReportModel>();
            ProductiviySearchReportModel model = new ProductiviySearchReportModel();
            //   var vehicledetail = InsuranceContext.VehicleDetails.All(where: $"IsActive = 'True'or IsActive is null").OrderByDescending(c=>c.Id).ToList();

            try
            {


                var vehicledetail = InsuranceContext.VehicleDetails.All().OrderByDescending(c => c.Id).ToList();
                var endorsedVehicle = InsuranceContext.EndorsementVehicleDetails.All().OrderByDescending(x => x.PrimaryVehicleId).ToList();
                var currencyList = _summaryDetailService.GetAllCurrency();
                var endorsePayment = InsuranceContext.EndorsementPaymentInformations.All().ToList();
                var endorsedPolicy = InsuranceContext.EndorsementPolicyDetails.All();
                var productlist = InsuranceContext.Products.All().ToList();
                var endorsed_customer = InsuranceContext.EndorsementCustomers.All();
                foreach (var item in vehicledetail.Take(100))
                {
                    var item_endorsed = endorsedVehicle.FirstOrDefault(x => x.PrimaryVehicleId == item.Id);
                    var endorsepay = endorsePayment.FirstOrDefault(x => x.PrimaryPolicyId == item.PolicyId);
                    //if (item.IsActive == false && !item.isLapsed) // for disable vehicle
                    //    continue;


                    var policy = InsuranceContext.PolicyDetails.Single(item.PolicyId);
                    var enfpol = endorsedPolicy.Where(x => x.PrimaryPolicyId == policy.Id);


                    var vehicleSummarydetail = InsuranceContext.SummaryVehicleDetails.Single(where: $"VehicleDetailsId='{item.Id}'");
                    if (vehicleSummarydetail != null)
                    {
                        var summary = InsuranceContext.SummaryDetails.Single(vehicleSummarydetail.SummaryDetailId);

                        if (summary != null)
                        {
                            var customer = InsuranceContext.Customers.Single(item.CustomerId);
                            var customer_endorsed = endorsed_customer.FirstOrDefault(x => x.PrimeryCustomerId == item.CustomerId);
                            var customerForRole = InsuranceContext.Customers.Single(summary.CreatedBy);

                            if (customerForRole != null)
                            {

                                var userDetials = UserManager.FindById(customerForRole.UserID);

                                if (userDetials != null)
                                {

                                    var roles = InsuranceContext.Query("select Name from AspNetUserRoles join AspNetRoles on AspNetUserRoles.RoleId = AspNetRoles.Id where UserId='" + userDetials.Id + "'  ")
                     .Select(x => new CustomerModel()
                     {
                         UserRoleName = x.Name,
                     }).FirstOrDefault();


                                    //if (roles != null && roles.UserRoleName.ToString() == "Staff")
                                    //{
                                    ProductiviyReportModel obj = new ProductiviyReportModel();
                                    obj.CustomerName = customer.FirstName + " " + customer.LastName;
                                    obj.VehicleId = item.Id;
                                    obj.PolicyNumber = policy.PolicyNumber;
                                    obj.TransactionDate = item.TransactionDate == null ? null : item.TransactionDate.Value.ToString("dd/MM/yyyy");

                                    obj.PremiumDue = Convert.ToDecimal(item.Premium);

                                    obj.SumInsured = Convert.ToDecimal(item.SumInsured);
                                    obj.UserName = userDetials.Email;
                                    obj.Product = productlist.FirstOrDefault(x => x.Id == item.ProductId).ProductName;
                                    //InsuranceContext.Products.Single(item.ProductId).ProductName;
                                    obj.Currency = _summaryDetailService.GetCurrencyName(currencyList, item.CurrencyId);

                                    obj.RenewPolicyNumber = item.RenewPolicyNumber;
                                    if (summary.isQuotation)
                                        obj.PolicyStatus = "Quotation";
                                    else
                                        obj.PolicyStatus = "Policy";

                                    obj.isLapsed = item.isLapsed;
                                    obj.IsActive = item.IsActive == null ? false : item.IsActive.Value;
                                    obj.IsEndorsed = false;



                                    listProductiviyReport.Add(obj);
                                    if (item_endorsed != null)
                                    {
                                        if (endorsepay != null)
                                        {


                                            ProductiviyReportModel obj_endorsed = new ProductiviyReportModel();
                                            if (customer_endorsed != null)
                                            {
                                                obj_endorsed.CustomerName = customer_endorsed.FirstName + "" + customer_endorsed.LastName;
                                            }
                                            obj_endorsed.VehicleId = item.Id;
                                            obj_endorsed.PolicyNumber = policy.PolicyNumber;
                                            obj_endorsed.TransactionDate = item_endorsed.TransactionDate.Value.ToString("dd/MM/yyyy");
                                            obj_endorsed.PremiumDue = Convert.ToDecimal(item_endorsed.Premium);
                                            obj_endorsed.SumInsured = Convert.ToDecimal(item_endorsed.SumInsured);
                                            obj_endorsed.isLapsed = item_endorsed.isLapsed;
                                            obj_endorsed.IsActive = item_endorsed.IsActive == null ? false : item_endorsed.IsActive.Value;
                                            obj_endorsed.RenewPolicyNumber = item.RenewPolicyNumber;
                                            obj_endorsed.UserName = userDetials.Email;
                                            obj_endorsed.Product = productlist.FirstOrDefault(x => x.Id == item_endorsed.ProductId).ProductName;
                                            obj_endorsed.Currency = _summaryDetailService.GetCurrencyName(currencyList, item_endorsed.CurrencyId);
                                            if (summary.isQuotation)
                                                obj_endorsed.PolicyStatus = "Quotation";
                                            else
                                                obj_endorsed.PolicyStatus = "Policy";
                                            obj_endorsed.IsEndorsed = true;
                                            listProductiviyReport.Add(obj_endorsed);
                                        }
                                    }
                                    //}
                                }
                            }




                        }
                    }
                }
                model.ListProductiviyReport = listProductiviyReport.OrderBy(x => x.VehicleId).ToList();



            }
            catch (Exception ex)
            {

            }


            return View(model);
        }

        public ActionResult ProductivitySearchReport(ProductiviySearchReportModel Model)
        {

            List<ProductiviyReportModel> listProductiviyReport = new List<ProductiviyReportModel>();
            ListProductiviyReportModel _listListProductiviyReport = new ListProductiviyReportModel();
            _listListProductiviyReport.ListProductiviyReport = new List<ProductiviyReportModel>();
            ProductiviySearchReportModel model = new ProductiviySearchReportModel();
            ViewBag.fromdate = Model.FromDate;
            ViewBag.enddate = Model.EndDate;
            var vehicledetail = InsuranceContext.VehicleDetails.All().OrderByDescending(c => c.Id).ToList();

            var currencyList = _summaryDetailService.GetAllCurrency();

            #region
            DateTime fromDate = DateTime.Now.AddDays(-1);
            DateTime endDate = DateTime.Now;
            if (!string.IsNullOrEmpty(Model.FromDate) && !string.IsNullOrEmpty(Model.EndDate))
            {
                fromDate = Convert.ToDateTime(Model.FromDate);
                endDate = Convert.ToDateTime(Model.EndDate);
            }
            var Vehicledetail = vehicledetail.Where(c => c.TransactionDate >= fromDate && c.TransactionDate <= endDate).ToList();


            var endorsedVehicle = InsuranceContext.EndorsementVehicleDetails.All().OrderByDescending(x => x.PrimaryVehicleId).ToList();

            var endorsePayment = InsuranceContext.EndorsementPaymentInformations.All().ToList();
            var endorsedPolicy = InsuranceContext.EndorsementPolicyDetails.All();
            var productlist = InsuranceContext.Products.All().ToList();
            var endorsed_customer = InsuranceContext.EndorsementCustomers.All();

            #endregion
            foreach (var item in Vehicledetail)
            {
                var item_endorsed = endorsedVehicle.FirstOrDefault(x => x.PrimaryVehicleId == item.Id);
                var endorsepay = endorsePayment.FirstOrDefault(x => x.PrimaryPolicyId == item.PolicyId);
                //if (item.IsActive == false && !item.isLapsed) // for disable vehicle
                //    continue;


                var policy = InsuranceContext.PolicyDetails.Single(item.PolicyId);


                var vehicleSummarydetail = InsuranceContext.SummaryVehicleDetails.Single(where: $"VehicleDetailsId='{item.Id}'");
                if (vehicleSummarydetail != null)
                {
                    var summary = InsuranceContext.SummaryDetails.Single(vehicleSummarydetail.SummaryDetailId);

                    if (summary != null)
                    {
                        var customer = InsuranceContext.Customers.Single(item.CustomerId);
                        var customer_endorsed = endorsed_customer.FirstOrDefault(x => x.PrimeryCustomerId == item.CustomerId);
                        var customerForRole = InsuranceContext.Customers.Single(summary.CreatedBy);

                        if (customerForRole != null)
                        {

                            var userDetials = UserManager.FindById(customerForRole.UserID);

                            if (userDetials != null)
                            {

                                var roles = InsuranceContext.Query("select Name from AspNetUserRoles join AspNetRoles on AspNetUserRoles.RoleId = AspNetRoles.Id where UserId='" + userDetials.Id + "'  ")
                 .Select(x => new CustomerModel()
                 {
                     UserRoleName = x.Name,
                 }).FirstOrDefault();


                                if (roles != null && roles.UserRoleName.ToString() == "Staff")
                                {
                                    ProductiviyReportModel obj = new ProductiviyReportModel();
                                    obj.CustomerName = customer.FirstName + " " + customer.LastName;
                                    obj.PolicyNumber = policy.PolicyNumber;

                                    obj.VehicleId = item.Id;
                                    //obj.TransactionDate = item.TransactionDate == null ? null : item.TransactionDate.Value.ToString("dd/MM/yyyy");
                                    obj.TransactionDate = item.TransactionDate == null ? null : item.TransactionDate.Value.ToString("MM/dd/yyyy");
                                    // obj.PremiumDue = Convert.ToDecimal(item.Premium + item.StampDuty + item.ZTSCLevy + item.RadioLicenseCost); // 28_may_2019
                                    obj.PremiumDue = Convert.ToDecimal(item.Premium);
                                    obj.SumInsured = Convert.ToDecimal(item.SumInsured);
                                    obj.UserName = userDetials.Email;
                                    obj.Product = InsuranceContext.Products.Single(item.ProductId).ProductName;
                                    obj.Currency = _summaryDetailService.GetCurrencyName(currencyList, item.CurrencyId);

                                    obj.RenewPolicyNumber = item.RenewPolicyNumber;

                                    if (summary.isQuotation)
                                        obj.PolicyStatus = "Quotation";
                                    else
                                        obj.PolicyStatus = "Policy";

                                    obj.isLapsed = item.isLapsed;
                                    obj.IsActive = item.IsActive.Value;
                                    obj.IsEndorsed = false;

                                    listProductiviyReport.Add(obj);

                                    if (item_endorsed != null)
                                    {
                                        if (endorsepay != null)
                                        {

                                            ProductiviyReportModel obj_endorsed = new ProductiviyReportModel();
                                            if (customer_endorsed != null)
                                            {
                                                obj_endorsed.CustomerName = customer_endorsed.FirstName + "" + customer_endorsed.LastName;
                                            }
                                            obj_endorsed.PolicyNumber = policy.PolicyNumber;
                                            obj_endorsed.VehicleId = item.Id;
                                            obj_endorsed.TransactionDate = item_endorsed.TransactionDate.Value.ToString("dd/MM/yyyy");
                                            obj_endorsed.PremiumDue = Convert.ToDecimal(item_endorsed.Premium);
                                            obj_endorsed.SumInsured = Convert.ToDecimal(item_endorsed.SumInsured);
                                            obj_endorsed.isLapsed = item_endorsed.isLapsed;
                                            obj_endorsed.IsActive = item_endorsed.IsActive == null ? false : item_endorsed.IsActive.Value;
                                            obj_endorsed.RenewPolicyNumber = item.RenewPolicyNumber;
                                            obj_endorsed.UserName = userDetials.Email;
                                            obj_endorsed.Product = productlist.FirstOrDefault(x => x.Id == item_endorsed.ProductId).ProductName;
                                            obj_endorsed.Currency = _summaryDetailService.GetCurrencyName(currencyList, item_endorsed.CurrencyId);
                                            if (summary.isQuotation)
                                                obj_endorsed.PolicyStatus = "Quotation";
                                            else
                                                obj_endorsed.PolicyStatus = "Policy";
                                            obj_endorsed.IsEndorsed = true;
                                            listProductiviyReport.Add(obj_endorsed);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            model.ListProductiviyReport = listProductiviyReport.OrderByDescending(x => x.VehicleId).ToList();
            return View("ProductivityReport", model);
        }

        //public ActionResult ProductivityReport()
        //{

        //    //var ListProductiviyReport = new List<ProductiviyReportModel>();
        //    List<ProductiviyReportModel> listProductiviyReport = new List<ProductiviyReportModel>();
        //    ListProductiviyReportModel _listListProductiviyReport = new ListProductiviyReportModel();
        //    _listListProductiviyReport.ListProductiviyReport = new List<ProductiviyReportModel>();
        //    ProductiviySearchReportModel model = new ProductiviySearchReportModel();
        //    //   var vehicledetail = InsuranceContext.VehicleDetails.All(where: $"IsActive = 'True'or IsActive is null").OrderByDescending(c=>c.Id).ToList();

        //    var vehicledetail = InsuranceContext.VehicleDetails.All().OrderByDescending(c => c.Id).ToList();

        //    var currencyList = _summaryDetailService.GetAllCurrency();


        //    foreach (var item in vehicledetail)
        //    {

        //        //if (item.IsActive == false && !item.isLapsed) // for disable vehicle
        //        //    continue;


        //        var policy = InsuranceContext.PolicyDetails.Single(item.PolicyId);


        //        var vehicleSummarydetail = InsuranceContext.SummaryVehicleDetails.Single(where: $"VehicleDetailsId='{item.Id}'");
        //        if (vehicleSummarydetail != null)
        //        {
        //            var summary = InsuranceContext.SummaryDetails.Single(vehicleSummarydetail.SummaryDetailId);

        //            if (summary != null)
        //            {
        //                var customer = InsuranceContext.Customers.Single(item.CustomerId);
        //                var customerForRole = InsuranceContext.Customers.Single(summary.CreatedBy);

        //                if (customerForRole != null)
        //                {

        //                    var userDetials = UserManager.FindById(customerForRole.UserID);

        //                    if (userDetials != null)
        //                    {

        //                        var roles = InsuranceContext.Query("select Name from AspNetUserRoles join AspNetRoles on AspNetUserRoles.RoleId = AspNetRoles.Id where UserId='" + userDetials.Id + "'  ")
        //         .Select(x => new CustomerModel()
        //         {
        //             UserRoleName = x.Name,
        //         }).FirstOrDefault();


        //                        //if (roles != null && roles.UserRoleName.ToString() == "Staff")
        //                        //{
        //                        ProductiviyReportModel obj = new ProductiviyReportModel();
        //                        obj.CustomerName = customer.FirstName + " " + customer.LastName;
        //                        obj.PolicyNumber = policy.PolicyNumber;
        //                        obj.TransactionDate = item.TransactionDate == null ? null : item.TransactionDate.Value.ToString("dd/MM/yyyy");
        //                        //obj.PremiumDue = Convert.ToDecimal(item.Premium + item.StampDuty + item.ZTSCLevy + item.RadioLicenseCost); // 28_may_2019
        //                        obj.PremiumDue = Convert.ToDecimal(item.Premium);
        //                        obj.SumInsured = Convert.ToDecimal(item.SumInsured);
        //                        obj.UserName = userDetials.Email;
        //                        obj.Product = InsuranceContext.Products.Single(item.ProductId).ProductName;
        //                        obj.Currency = _summaryDetailService.GetCurrencyName(currencyList, item.CurrencyId);

        //                        obj.RenewPolicyNumber = item.RenewPolicyNumber;
        //                        if (summary.isQuotation)
        //                            obj.PolicyStatus = "Quotation";
        //                        else
        //                            obj.PolicyStatus = "Policy";

        //                        obj.isLapsed = item.isLapsed;
        //                        obj.IsActive = item.IsActive == null ? false : item.IsActive.Value;



        //                        listProductiviyReport.Add(obj);
        //                        //}
        //                    }
        //                }




        //            }
        //        }
        //    }
        //    model.ListProductiviyReport = listProductiviyReport.OrderBy(x => x.UserName).ToList();

        //    return View(model);
        //}
        //public ActionResult ProductivitySearchReport(ProductiviySearchReportModel Model)
        //{

        //    List<ProductiviyReportModel> listProductiviyReport = new List<ProductiviyReportModel>();
        //    ListProductiviyReportModel _listListProductiviyReport = new ListProductiviyReportModel();
        //    _listListProductiviyReport.ListProductiviyReport = new List<ProductiviyReportModel>();
        //    ProductiviySearchReportModel model = new ProductiviySearchReportModel();
        //    ViewBag.fromdate = Model.FromDate;
        //    ViewBag.enddate = Model.EndDate;
        //    var vehicledetail = InsuranceContext.VehicleDetails.All().OrderByDescending(c => c.Id).ToList();

        //    var currencyList = _summaryDetailService.GetAllCurrency();

        //    #region
        //    DateTime fromDate = DateTime.Now.AddDays(-1);
        //    DateTime endDate = DateTime.Now;
        //    if (!string.IsNullOrEmpty(Model.FromDate) && !string.IsNullOrEmpty(Model.EndDate))
        //    {
        //        fromDate = Convert.ToDateTime(Model.FromDate);
        //        endDate = Convert.ToDateTime(Model.EndDate);
        //    }
        //    var Vehicledetail = vehicledetail.Where(c => c.TransactionDate >= fromDate && c.TransactionDate <= endDate).ToList();

        //    #endregion
        //    foreach (var item in Vehicledetail)
        //    {

        //        //if (item.IsActive == false && !item.isLapsed) // for disable vehicle
        //        //    continue;


        //        var policy = InsuranceContext.PolicyDetails.Single(item.PolicyId);


        //        var vehicleSummarydetail = InsuranceContext.SummaryVehicleDetails.Single(where: $"VehicleDetailsId='{item.Id}'");
        //        if (vehicleSummarydetail != null)
        //        {
        //            var summary = InsuranceContext.SummaryDetails.Single(vehicleSummarydetail.SummaryDetailId);

        //            if (summary != null)
        //            {
        //                var customer = InsuranceContext.Customers.Single(item.CustomerId);
        //                var customerForRole = InsuranceContext.Customers.Single(summary.CreatedBy);

        //                if (customerForRole != null)
        //                {

        //                    var userDetials = UserManager.FindById(customerForRole.UserID);

        //                    if (userDetials != null)
        //                    {

        //                        var roles = InsuranceContext.Query("select Name from AspNetUserRoles join AspNetRoles on AspNetUserRoles.RoleId = AspNetRoles.Id where UserId='" + userDetials.Id + "'  ")
        //         .Select(x => new CustomerModel()
        //         {
        //             UserRoleName = x.Name,
        //         }).FirstOrDefault();


        //                        if (roles != null && roles.UserRoleName.ToString() == "Staff")
        //                        {
        //                            ProductiviyReportModel obj = new ProductiviyReportModel();
        //                            obj.CustomerName = customer.FirstName + " " + customer.LastName;
        //                            obj.PolicyNumber = policy.PolicyNumber;
        //                            //obj.TransactionDate = item.TransactionDate == null ? null : item.TransactionDate.Value.ToString("dd/MM/yyyy");
        //                            obj.TransactionDate = item.TransactionDate == null ? null : item.TransactionDate.Value.ToString("MM/dd/yyyy");
        //                            // obj.PremiumDue = Convert.ToDecimal(item.Premium + item.StampDuty + item.ZTSCLevy + item.RadioLicenseCost); // 28_may_2019
        //                            obj.PremiumDue = Convert.ToDecimal(item.Premium);
        //                            obj.SumInsured = Convert.ToDecimal(item.SumInsured);
        //                            obj.UserName = userDetials.Email;
        //                            obj.Product = InsuranceContext.Products.Single(item.ProductId).ProductName;
        //                            obj.Currency = _summaryDetailService.GetCurrencyName(currencyList, item.CurrencyId);

        //                            obj.RenewPolicyNumber = item.RenewPolicyNumber;

        //                            if (summary.isQuotation)
        //                                obj.PolicyStatus = "Quotation";
        //                            else
        //                                obj.PolicyStatus = "Policy";

        //                            obj.isLapsed = item.isLapsed;
        //                            obj.IsActive = item.IsActive.Value;


        //                            listProductiviyReport.Add(obj);
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    model.ListProductiviyReport = listProductiviyReport.OrderByDescending(x => x.UserName).ToList();
        //    return View("ProductivityReport", model);
        //}

        [Authorize(Roles = "Administrator,Reports,Claim")]
        public ActionResult ClaimsPaymentReport()
        {
            var results = new List<ClaimRegistrationProviderModel>();
            try
            {
                //results = (from vehicalDetials in InsuranceContext.VehicleDetails.All()
                //           join customer in InsuranceContext.Customers.All()
                //           on vehicalDetials.CustomerId equals customer.Id
                //           join make in InsuranceContext.VehicleMakes.All()
                //           on vehicalDetials.MakeId equals make.MakeCode
                //           join vehicalModel in InsuranceContext.VehicleModels.All()
                //           on vehicalDetials.ModelId equals vehicalModel.ModelCode
                //           join coverType in InsuranceContext.CoverTypes.All().ToList()
                //           on vehicalDetials.CoverTypeId equals coverType.Id
                //           join user in UserManager.Users
                //           on customer.UserID equals user.Id

                //           select new RiskDetailModel
                //           {
                //               PolicyExpireDate = vehicalDetials.CoverEndDate.Value.ToShortDateString(),
                //               CoverTypeName = coverType.Name,
                //               SumInsured = vehicalDetials.SumInsured,
                //               VechicalMake = make.MakeDescription,
                //               VechicalModel = vehicalModel.ModelDescription,
                //               VehicleYear = vehicalDetials.VehicleYear,
                //               CustomerDetails = new CustomerModel { FirstName = customer.FirstName, LastName = customer.LastName, PhoneNumber = customer.PhoneNumber, EmailAddress = user.Email }


                //           }).ToList();



                var query = "select ClaimRegistrationProviderDetial.Id,ClaimRegistrationId,RegistrationNo,PolicyNumber,ClaimantName, ProviderType, ServiceProviderName,ClaimRegistrationProviderDetial.CreatedOn, ServiceProviderFee from ClaimRegistrationProviderDetial";
                query += " join ServiceProvider on ClaimRegistrationProviderDetial.ServiceProviderId = ServiceProvider.Id ";
                query += " left join ClaimRegistration on ClaimRegistration.Id=ClaimRegistrationProviderDetial.ClaimRegistrationId";


                query += " join ServiceProviderType on ClaimRegistrationProviderDetial.ServiceProviderTypeId = ServiceProviderType.id  where ClaimRegistrationProviderDetial.IsActive=1 ";


                results = InsuranceContext.Query(query)
.Select(x => new ClaimRegistrationProviderModel()
{
    Id = x.Id,
    ClaimRegistrationId = x.ClaimRegistrationId,
    ServiceProviderType = x.ProviderType,
    ServiceProviderName = x.ServiceProviderName,
    CreatedOn = x.CreatedOn,
    ServiceProviderFee = x.ServiceProviderFee,
    RegistrationNo = x.RegistrationNo,
    PolicyNumber = x.PolicyNumber,
    ClaimantName = x.ClaimantName



}).ToList();





            }
            catch (Exception ex)
            {

            }


            return View(results);
        }
        public ActionResult LoyaltyPointsReport()
        {
            var ListDailyReceiptsReport = new List<LoyaltyPointsModel>();
            LoyaltyPointsReport _LoyaltyPt = new LoyaltyPointsReport();
            _LoyaltyPt.LoyaltyPoints = new List<LoyaltyPointsModel>();
            LoyaltyPointsReportSeachModels model = new LoyaltyPointsReportSeachModels();


            var VehicleDetails = InsuranceContext.VehicleDetails.All(where: "IsActive ='True'").ToList().Take(500);
            var currencyList = _summaryDetailService.GetAllCurrency();

            if (VehicleDetails != null)
            {
                foreach (var item in VehicleDetails)
                {
                    var Customer = InsuranceContext.Customers.Single(item.CustomerId);
                    var User = UserManager.FindById(Customer.UserID.ToString());
                    //var loyalityDetail = InsuranceContext.LoyaltyDetails.All(where: $"CustomerId={item.CustomerId}").Sum(x => x.PointsEarned);


                    var vehicleSummarydetail = InsuranceContext.SummaryVehicleDetails.Single(where: $"VehicleDetailsId='{item.Id}'");
                    if (vehicleSummarydetail != null)
                    {
                        var summary = InsuranceContext.SummaryDetails.Single(vehicleSummarydetail.SummaryDetailId);
                        if (summary != null)
                        {

                            if (summary.isQuotation != true)
                            {

                                var loyalityDetail = InsuranceContext.LoyaltyDetails.All(where: $"CustomerId={item.CustomerId}").Sum(x => x.PointsEarned);

                                if (loyalityDetail != null)
                                {

                                    var ListDailyReceiptsDetails = ListDailyReceiptsReport.FirstOrDefault(c => c.PolicyId == item.PolicyId);

                                    if (ListDailyReceiptsDetails == null)
                                    {
                                        ListDailyReceiptsReport.Add(new LoyaltyPointsModel()
                                        {
                                            CustomerName = Customer.FirstName + " " + Customer.LastName,
                                            CellPhoneNumber = Customer.Countrycode + "-" + Customer.PhoneNumber,
                                            Address = Customer.AddressLine1 + " " + Customer.AddressLine2,
                                            SumInsured = Convert.ToDecimal(summary.TotalSumInsured),
                                            PremiumPaid = Convert.ToDecimal(summary.TotalPremium),
                                            EmailAddress = User.Email,
                                            LoyaltyPoints = Convert.ToString(loyalityDetail),
                                            PolicyId = item.PolicyId,
                                            Currency = _summaryDetailService.GetCurrencyName(currencyList, item.CurrencyId),
                                            TransactionDate = item.TransactionDate == null ? DateTime.MinValue : item.TransactionDate.Value

                                        });
                                    }


                                }
                                else
                                {

                                }

                            }
                        }
                    }
                }
            }
            model.LoyaltyPoints = ListDailyReceiptsReport.OrderBy(x => x.CustomerName).ToList();
            return View(model);
        }

        public ActionResult LoyaltyPointsSearchReport(LoyaltyPointsReportSeachModels Model)
        {
            var ListDailyReceiptsReport = new List<LoyaltyPointsModel>();
            LoyaltyPointsReport _LoyaltyPointsReport = new LoyaltyPointsReport();
            _LoyaltyPointsReport.LoyaltyPoints = new List<LoyaltyPointsModel>();
            LoyaltyPointsReportSeachModels _model = new LoyaltyPointsReportSeachModels();
            var VehicleDetails = InsuranceContext.VehicleDetails.All(where: "IsActive ='True'").ToList();
            ViewBag.fromdate = Model.FromDate;
            ViewBag.enddate = Model.EndDate;

            var Vehicledetail = VehicleDetails.Where(c => c.TransactionDate >= Convert.ToDateTime(Model.FromDate) && c.TransactionDate <= Convert.ToDateTime(Model.EndDate)).ToList();



            #region
            DateTime fromDate = DateTime.Now.AddDays(-1);
            DateTime endDate = DateTime.Now;
            if (!string.IsNullOrEmpty(Model.FromDate) && !string.IsNullOrEmpty(Model.EndDate))
            {
                fromDate = Convert.ToDateTime(Model.FromDate);
                endDate = Convert.ToDateTime(Model.EndDate);
            }
            VehicleDetails = VehicleDetails.Where(c => c.TransactionDate >= fromDate && c.TransactionDate <= endDate).ToList();

            var currencyList = _summaryDetailService.GetAllCurrency();


            #endregion
            if (VehicleDetails != null)
            {
                foreach (var item in VehicleDetails)
                {

                    var vehicleSummarydetail = InsuranceContext.SummaryVehicleDetails.Single(where: $"VehicleDetailsId='{item.Id}'");
                    if (vehicleSummarydetail != null)
                    {
                        var summary = InsuranceContext.SummaryDetails.Single(vehicleSummarydetail.SummaryDetailId);
                        if (summary != null)
                        {

                            if (summary.isQuotation != true)
                            {

                                var Customer = InsuranceContext.Customers.Single(item.CustomerId);
                                var User = UserManager.FindById(Customer.UserID.ToString());
                                var loyalityDetail = InsuranceContext.LoyaltyDetails.All(where: $"CustomerId={item.CustomerId}").Sum(x => x.PointsEarned);
                                if (loyalityDetail != null)
                                {

                                    var ListDailyReceiptsDetails = ListDailyReceiptsReport.FirstOrDefault(c => c.PolicyId == item.PolicyId);

                                    if (ListDailyReceiptsDetails == null)
                                    {
                                        ListDailyReceiptsReport.Add(new LoyaltyPointsModel()
                                        {
                                            CustomerName = Customer.FirstName + " " + Customer.LastName,
                                            CellPhoneNumber = Customer.Countrycode + "-" + Customer.PhoneNumber,
                                            Address = Customer.AddressLine1 + " " + Customer.AddressLine2,
                                            SumInsured = Convert.ToDecimal(summary.TotalSumInsured),
                                            PremiumPaid = Convert.ToDecimal(summary.TotalPremium),
                                            EmailAddress = User.Email,
                                            LoyaltyPoints = Convert.ToString(loyalityDetail),
                                            PolicyId = item.PolicyId,
                                            Currency = _summaryDetailService.GetCurrencyName(currencyList, item.CurrencyId),
                                            TransactionDate = item.TransactionDate == null ? DateTime.MinValue : item.TransactionDate.Value
                                        });
                                    }
                                }
                                else
                                {

                                }

                            }
                        }
                    }



                }
            }
            _model.LoyaltyPoints = ListDailyReceiptsReport.OrderBy(x => x.CustomerName).ToList();
            return View("LoyaltyPointsReport", _model);
        }


        [Authorize(Roles = "Administrator,Reports,Claim")]
        public ActionResult ClaimsRegistrationReport()
        {
            try
            {
                ClaimSearchReport model = new ClaimSearchReport();
                List<ClaimReportModel> ListClaimReport = new List<ClaimReportModel>();

                // Claim Register report
                // var query = "Select a.PolicyNumber as Policyno,a.ClaimNumber as ClaimNo,a.ClaimStatus as Claimstatus,a.DateOfLoss as LossOfDate,
                //b.ClaimantName as INsuredName,a.DateOfNotifications as DateNotifications,a.DescriptionOfLoss as LossofDescriptiom,a.RegistrationNo as registrationno ,
                //a.MakeId,a.ModelId,a.EstimatedValueOfLoss as EstimatedLoss,b.ClaimantName as Name,b.CoverStartDate as PStartDate,b.CoverEndDate as PEndDate,v.SumInsured as suminsured ,
                //c.Name as CoverType,make.MakeDescription as CoverMake,model.ModelDescription as covermodel from ClaimRegistration as a inner join ClaimNotification as b on a.ClaimNotificationId = b.Id 
                //left join VehicleDetail as v on b.VehicleId = v.Id inner join CoverType as c on v.CoverTypeId = c.Id inner join VehicleMake as make on a.MakeId = make.MakeCode inner join VehicleModel as 
                //model on a.ModelId = model.ModelCode";


                var query = "select PolicyDetail.Id, Customer.FirstName + ' ' + Customer.LastName as InsuredName, PolicyDetail.PolicyNumber, ";
                query += "VehicleDetail.CoverStartDate, VehicleDetail.CoverEndDate, ClaimRegistration.ClaimNumber, ClaimRegistration.ClaimantName, ClaimStatus.Status, ";
                query += "ClaimRegistration.DateOfLoss, ClaimRegistration.CreatedOn, ClaimRegistration.DescriptionOfLoss, Product.ProductName, ";
                query += "CoverType.Name,VehicleDetail.SumInsured, VehicleMake.MakeDescription + ' ' + VehicleModel.ModelDescription as VehicleDesc, ";
                query += " VehicleDetail.RegistrationNo , ClaimRegistration.EstimatedValueOfLoss from ClaimRegistration ";
                query += " join PolicyDetail on ClaimRegistration.PolicyNumber= PolicyDetail.PolicyNumber ";
                query += " join VehicleDetail on PolicyDetail.Id= VehicleDetail.PolicyId ";
                query += " join Customer on PolicyDetail.CustomerId = Customer.Id ";
                query += " join ClaimNotification on ClaimNotification.id= ClaimRegistration.ClaimNotificationId ";
                query += " join ClaimStatus on ClaimRegistration.ClaimStatus= ClaimStatus.Id ";
                query += " join Product on VehicleDetail.ProductId = Product.Id ";
                query += " join CoverType on VehicleDetail.CoverTypeId = CoverType.Id ";
                query += " join VehicleMake on VehicleMake.MakeCode =VehicleDetail.MakeId ";
                query += " join VehicleModel on VehicleModel.ModelCode = VehicleDetail.ModelId order by ClaimRegistration.Id desc";

                var Claimdetail = InsuranceContext.Query(query)
                .Select(x => new ClaimReportModel
                {
                    Id = x.Id,
                    InsuredName = x.InsuredName,
                    PolicyNumber = x.PolicyNumber,
                    PolicyStartDate = x.CoverStartDate.ToShortDateString(),
                    PolicyEndDate = x.CoverEndDate.ToShortDateString(),
                    ClaimNumber = x.ClaimNumber,
                    ClaimantName = x.ClaimantName,
                    ProductName = x.ProductName,
                    ClaimStatus = x.Status,
                    DateOfLoss = x.DateOfLoss.ToShortDateString(),
                    DateOfNotification = x.CreatedOn.ToShortDateString(),
                    DescripationOfLoss = x.DescriptionOfLoss,
                    CoverType = x.Name,
                    SumInsured = x.SumInsured,
                    VehicleDescription = x.VehicleDesc,
                    VRN = x.RegistrationNo,
                    EstimatedLoss = x.EstimatedValueOfLoss
                }).ToList();


                var distinctClaimList = new List<ClaimReportModel>();


                foreach (var item in Claimdetail)
                {

                    if (distinctClaimList.FirstOrDefault(c => c.Id == item.Id) == null)
                    {
                        distinctClaimList.Add(item);
                    }


                }




                ListClaimReport = distinctClaimList;
                model.ClaimReportModelData = ListClaimReport;
                return View(model);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        [Authorize(Roles = "Administrator,Reports")]
        public ActionResult SearchClaimRegisterReport(ClaimSearchReport model)
        {
            try
            {

                ClaimSearchReport _model = new ClaimSearchReport();
                List<ClaimReportModel> ListClaimReport = new List<ClaimReportModel>();
                DateTime fromDate = DateTime.Now.AddDays(-1);
                DateTime endDate = DateTime.Now;

                if (!string.IsNullOrEmpty(model.FromDate) && !string.IsNullOrEmpty(model.EndDate))
                {
                    fromDate = Convert.ToDateTime(model.FromDate);
                    endDate = Convert.ToDateTime(model.EndDate);


                    ViewBag.fromdate = fromDate.ToString("dd/MM/yyyy");
                    ViewBag.enddate = endDate.ToString("dd/MM/yyyy");

                }



                var query = "select PolicyDetail.Id, Customer.FirstName + ' ' + Customer.LastName as InsuredName, PolicyDetail.PolicyNumber, ";
                query += "VehicleDetail.CoverStartDate, VehicleDetail.CoverEndDate, ClaimRegistration.ClaimNumber, ClaimRegistration.ClaimantName, ClaimStatus.Status, ";
                query += "ClaimRegistration.DateOfLoss, ClaimRegistration.CreatedOn, ClaimRegistration.DescriptionOfLoss, Product.ProductName, ";
                query += "CoverType.Name,VehicleDetail.SumInsured, VehicleMake.MakeDescription + ' ' + VehicleModel.ModelDescription as VehicleDesc, ";
                query += " VehicleDetail.RegistrationNo , ClaimRegistration.EstimatedValueOfLoss from ClaimRegistration ";
                query += " join PolicyDetail on ClaimRegistration.PolicyNumber= PolicyDetail.PolicyNumber ";
                query += " join VehicleDetail on PolicyDetail.Id= VehicleDetail.PolicyId ";
                query += " join Customer on PolicyDetail.CustomerId = Customer.Id ";
                query += " join ClaimNotification on ClaimNotification.id= ClaimRegistration.ClaimNotificationId ";
                query += " join ClaimStatus on ClaimRegistration.ClaimStatus= ClaimStatus.Id ";
                query += " join Product on VehicleDetail.ProductId = Product.Id ";
                query += " join CoverType on VehicleDetail.CoverTypeId = CoverType.Id ";
                query += " join VehicleMake on VehicleMake.MakeCode =VehicleDetail.MakeId ";
                query += " join VehicleModel on VehicleModel.ModelCode = VehicleDetail.ModelId Where ClaimRegistration.CreatedOn>= '" + fromDate + "' And ClaimRegistration.CreatedOn<='" + endDate + "'" + "order by ClaimRegistration.Id desc";


                var Claimdetail = InsuranceContext.Query(query)
                .Select(x => new ClaimReportModel
                {
                    Id = x.Id,
                    InsuredName = x.InsuredName,
                    PolicyNumber = x.PolicyNumber,
                    PolicyStartDate = x.CoverStartDate.ToShortDateString(),
                    PolicyEndDate = x.CoverEndDate.ToShortDateString(),
                    ClaimNumber = x.ClaimNumber,
                    ClaimantName = x.ClaimantName,
                    ProductName = x.ProductName,
                    ClaimStatus = x.Status,
                    DateOfLoss = x.DateOfLoss.ToShortDateString(),
                    DateOfNotification = x.CreatedOn.ToShortDateString(),
                    DescripationOfLoss = x.DescriptionOfLoss,
                    CoverType = x.Name,
                    SumInsured = x.SumInsured,
                    VehicleDescription = x.VehicleDesc,
                    VRN = x.RegistrationNo,
                    EstimatedLoss = x.EstimatedValueOfLoss

                }).Distinct().ToList();


                var distinctClaimList = new List<ClaimReportModel>();


                foreach (var item in Claimdetail)
                {

                    if (distinctClaimList.FirstOrDefault(c => c.Id == item.Id) == null)
                    {
                        distinctClaimList.Add(item);
                    }


                }





                ListClaimReport = distinctClaimList;
                model.ClaimReportModelData = ListClaimReport.OrderBy(x => x.DateOfNotification).ToList();



                return View("ClaimsRegistrationReport", model);

            }
            catch (Exception ex)
            {
                return View("ClaimsRegistrationReport");
            }


        }

        [Authorize(Roles = "Administrator,Reports,Claim")]
        public ActionResult ClaimsNotificationReport()
        {
            try
            {
                ClaimSearchReport model = new ClaimSearchReport();
                List<ClaimReportModel> ListClaimReport = new List<ClaimReportModel>();

                //  Claim Register report
                // var query = "Select a.PolicyNumber as Policyno,a.ClaimNumber as ClaimNo,a.ClaimStatus as Claimstatus,a.DateOfLoss as LossOfDate,
                //b.ClaimantName as INsuredName,a.DateOfNotifications as DateNotifications,a.DescriptionOfLoss as LossofDescriptiom,a.RegistrationNo as registrationno ,
                //a.MakeId,a.ModelId,a.EstimatedValueOfLoss as EstimatedLoss,b.ClaimantName as Name,b.CoverStartDate as PStartDate,b.CoverEndDate as PEndDate,v.SumInsured as suminsured ,
                //c.Name as CoverType,make.MakeDescription as CoverMake,model.ModelDescription as covermodel from ClaimRegistration as a inner join ClaimNotification as b on a.ClaimNotificationId = b.Id 
                //left join VehicleDetail as v on b.VehicleId  = v.Id inner join CoverType as c on v.CoverTypeId = c.Id  inner join VehicleMake as make on a.MakeId = make.MakeCode inner join VehicleModel as 
                //model on a.ModelId = model.ModelCode";


                var query = "select PolicyDetail.Id, Customer.FirstName + ' ' + Customer.LastName as InsuredName, PolicyDetail.PolicyNumber, ";
                query += "VehicleDetail.CoverStartDate, VehicleDetail.CoverEndDate, ClaimRegistration.ClaimNumber, ClaimNotification.ClaimantName, ClaimStatus.Status, ";
                query += "ClaimNotification.DateOfLoss, ClaimNotification.CreatedOn, ClaimNotification.DescriptionOfLoss, Product.ProductName, ";
                query += "CoverType.Name,VehicleDetail.SumInsured, VehicleMake.MakeDescription + ' ' + VehicleModel.ModelDescription as VehicleDesc, ";
                query += " VehicleDetail.RegistrationNo , ClaimNotification.EstimatedValueOfLoss  from ClaimNotification ";
                query += " join PolicyDetail on ClaimNotification.PolicyNumber= PolicyDetail.PolicyNumber ";
                query += " join VehicleDetail on PolicyDetail.Id= VehicleDetail.PolicyId ";
                query += " join Customer on PolicyDetail.CustomerId = Customer.Id ";
                query += " left join ClaimRegistration on ClaimNotification.id= ClaimRegistration.ClaimNotificationId ";
                query += " left join ClaimStatus on ClaimRegistration.ClaimStatus= ClaimStatus.Id ";
                query += " join Product on VehicleDetail.ProductId = Product.Id ";
                query += " join CoverType on VehicleDetail.CoverTypeId = CoverType.Id ";
                query += " join VehicleMake on VehicleMake.MakeCode =VehicleDetail.MakeId  ";
                query += " join VehicleModel on VehicleModel.ModelCode = VehicleDetail.ModelId order  by ClaimNotification.Id desc";

                var Claimdetail = InsuranceContext.Query(query)
                          .Select(x => new ClaimReportModel
                          {
                              Id = x.Id,
                              InsuredName = x.InsuredName,
                              PolicyNumber = x.PolicyNumber,
                              PolicyStartDate = x.CoverStartDate.ToShortDateString(),
                              PolicyEndDate = x.CoverEndDate.ToShortDateString(),
                              ClaimNumber = x.ClaimNumber,
                              ClaimantName = x.ClaimantName,
                              ProductName = x.ProductName,
                              ClaimStatus = x.Status,
                              DateOfLoss = x.DateOfLoss.ToShortDateString(),
                              DateOfNotification = x.CreatedOn.ToShortDateString(),
                              DescripationOfLoss = x.DescriptionOfLoss,
                              CoverType = x.Name,
                              SumInsured = x.SumInsured,
                              VehicleDescription = x.VehicleDesc,
                              VRN = x.RegistrationNo,
                              EstimatedLoss = x.EstimatedValueOfLoss
                          }).ToList();


                var distinctClaimList = new List<ClaimReportModel>();


                foreach (var item in Claimdetail)
                {

                    if (distinctClaimList.FirstOrDefault(c => c.Id == item.Id) == null)
                    {
                        distinctClaimList.Add(item);
                    }


                }




                ListClaimReport = distinctClaimList;
                model.ClaimReportModelData = ListClaimReport;
                return View(model);
            }
            catch (Exception ex)
            {

                throw;
            }
        }

        [Authorize(Roles = "Administrator,Reports")]
        public ActionResult SearchClaimReport(ClaimSearchReport model)
        {
            try
            {

                ClaimSearchReport _model = new ClaimSearchReport();
                List<ClaimReportModel> ListClaimReport = new List<ClaimReportModel>();
                DateTime fromDate = DateTime.Now.AddDays(-1);
                DateTime endDate = DateTime.Now;

                if (!string.IsNullOrEmpty(model.FromDate) && !string.IsNullOrEmpty(model.EndDate))
                {
                    fromDate = Convert.ToDateTime(model.FromDate);
                    endDate = Convert.ToDateTime(model.EndDate);


                    ViewBag.fromdate = fromDate.ToString("dd/MM/yyyy");
                    ViewBag.enddate = endDate.ToString("dd/MM/yyyy");

                }



                var query = "select PolicyDetail.Id, Customer.FirstName + ' ' + Customer.LastName as InsuredName, PolicyDetail.PolicyNumber, ";
                query += "VehicleDetail.CoverStartDate, VehicleDetail.CoverEndDate, ClaimRegistration.ClaimNumber, ClaimNotification.ClaimantName, ClaimStatus.Status, ";
                query += "ClaimNotification.DateOfLoss, ClaimNotification.CreatedOn, ClaimNotification.DescriptionOfLoss, Product.ProductName, ";
                query += "CoverType.Name,VehicleDetail.SumInsured, VehicleMake.MakeDescription + ' ' + VehicleModel.ModelDescription as VehicleDesc, ";
                query += " VehicleDetail.RegistrationNo , ClaimNotification.EstimatedValueOfLoss  from ClaimNotification ";
                query += " join PolicyDetail on ClaimNotification.PolicyNumber= PolicyDetail.PolicyNumber ";
                query += " join VehicleDetail on PolicyDetail.Id= VehicleDetail.PolicyId ";
                query += " join Customer on PolicyDetail.CustomerId = Customer.Id ";
                query += " left join ClaimRegistration on ClaimNotification.id= ClaimRegistration.ClaimNotificationId ";
                query += " left join ClaimStatus on ClaimRegistration.ClaimStatus= ClaimStatus.Id ";
                query += " join Product on VehicleDetail.ProductId = Product.Id ";
                query += " join CoverType on VehicleDetail.CoverTypeId = CoverType.Id ";
                query += " join VehicleMake on VehicleMake.MakeCode =VehicleDetail.MakeId  ";
                query += " join VehicleModel on VehicleModel.ModelCode = VehicleDetail.ModelId Where ClaimNotification.CreatedOn>= '" + fromDate + "' And ClaimNotification.CreatedOn<='" + endDate + "'" + "order  by ClaimNotification.Id desc";

                var Claimdetail = InsuranceContext.Query(query)
                           .Select(x => new ClaimReportModel
                           {
                               Id = x.Id,
                               InsuredName = x.InsuredName,
                               PolicyNumber = x.PolicyNumber,
                               PolicyStartDate = x.CoverStartDate.ToShortDateString(),
                               PolicyEndDate = x.CoverEndDate.ToShortDateString(),
                               ClaimNumber = x.ClaimNumber,
                               ClaimantName = x.ClaimantName,
                               ProductName = x.ProductName,
                               ClaimStatus = x.Status,
                               DateOfLoss = x.DateOfLoss.ToShortDateString(),
                               DateOfNotification = x.CreatedOn.ToShortDateString(),
                               DescripationOfLoss = x.DescriptionOfLoss,
                               CoverType = x.Name,
                               SumInsured = x.SumInsured,
                               VehicleDescription = x.VehicleDesc,
                               VRN = x.RegistrationNo,
                               EstimatedLoss = x.EstimatedValueOfLoss
                           }).Distinct().ToList();


                var distinctClaimList = new List<ClaimReportModel>();
                foreach (var item in Claimdetail)
                {
                    if (distinctClaimList.FirstOrDefault(c => c.Id == item.Id) == null)
                    {
                        distinctClaimList.Add(item);
                    }
                }

                ListClaimReport = distinctClaimList;
                model.ClaimReportModelData = ListClaimReport.OrderBy(x => x.DateOfNotification).ToList();

                return View("ClaimsNotificationReport", model);

            }
            catch (Exception ex)
            {
                return View("ClaimsNotificationReport");
            }
        }



        [Authorize(Roles = "Administrator,Reports,Finance,Team Leaders")]
        public ActionResult CertSerialNoReport()
        {
            //SendGWPExcelFile();
            CertSerialNoReportModel model = new CertSerialNoReportModel();

            ViewBag.BranchList = InsuranceContext.Branches.All(where: "Id=" + (int)ALMBranch.GeneCallCentre).ToList();


            string query = "select top 100 [dbo].[fn_GetUserCallCenterAgent] (SummaryDetail.CreatedBy) as AgentName, VRN, PolicyNumber,CertSerialNoDetail.CertSerialNo,ALMBranchId, Branch.BranchName, CertSerialNoDetail.CreatedOn from CertSerialNoDetail";
            query += " join PolicyDetail on CertSerialNoDetail.PolicyId = PolicyDetail.Id ";
            query += "  join VehicleDetail on VehicleDetail.Id=CertSerialNoDetail.VehicleId  ";
            query += " join SummaryVehicleDetail on VehicleDetail.Id=SummaryVehicleDetail.VehicleDetailsId ";
            query += " join SummaryDetail on SummaryDetail.Id=SummaryVehicleDetail.SummaryDetailId  ";
            query += " left join Branch on VehicleDetail.ALMBranchId= Branch.Id ";
            // query += " where CertSerialNo IS NOT NULL and [dbo].[fn_GetUserBranch] (CertSerialNoDetail.CreatedBy)='Gene Call Centre'";
            query += " where CertSerialNo IS NOT NULL ";
            query += " order by CertSerialNoDetail.id desc";


            //string query = "select Customer.FirstName + ' '+ Customer.LastName as AgentName, VRN, PolicyNumber,CertSerialNoDetail.CertSerialNo,ALMBranchId,  CertSerialNoDetail.CreatedOn from CertSerialNoDetail";
            //query += " join PolicyDetail on CertSerialNoDetail.PolicyId = PolicyDetail.Id ";
            //query += " join Customer on CertSerialNoDetail.CreatedBy = Customer.Id ";
            //query += " join VehicleDetail on VehicleDetail.Id=CertSerialNoDetail.VehicleId ";
            //query += " where [dbo].[fn_GetUserBranch] (CertSerialNoDetail.CreatedBy)='Gene Call Centre' and and ALMBranchId=0";
            //query += " order by CertSerialNoDetail.id desc";

            var list = InsuranceContext.Query(query).Select(c => new CertSerialNoModel()
            {
                VRN = c.VRN,
                AgentName = c.AgentName,
                PolicyNumber = c.PolicyNumber,
                CertSerialNo = c.CertSerialNo,
                CreatedOn = c.CreatedOn == null ? "" : Convert.ToString(c.CreatedOn),
                ALMBranchId = c.ALMBranchId,
                BranchName = c.BranchName == null ? "Gene Call Centre" : c.BranchName
            }).ToList();

            model.List = list;
            return View(model);
        }

        [HttpPost]
        [Authorize(Roles = "Administrator,Reports,Finance,Team Leaders")]
        public ActionResult CertSerialNoSearchReport(CertSerialNoReportModel model)
        {
            CertSerialNoReportModel policylist = new CertSerialNoReportModel();
            policylist.List = new List<CertSerialNoModel>();
            DateTime fromDate = DateTime.Now.AddDays(-1);
            DateTime endDate = DateTime.Now;

            if (!string.IsNullOrEmpty(model.FromDate) && !string.IsNullOrEmpty(model.EndDate))
            {
                fromDate = Convert.ToDateTime(model.FromDate);
                endDate = Convert.ToDateTime(model.EndDate);

                ViewBag.fromdate = fromDate.ToString("dd/MM/yyyy");
                ViewBag.enddate = endDate.ToString("dd/MM/yyyy");
            }

            var BranchList = InsuranceContext.Branches.All(where: "Id=" + (int)ALMBranch.GeneCallCentre).ToList();

            ViewBag.BranchList = BranchList;

            var AlmList = InsuranceContext.LogDetailTbls.All(where: "branchId is not null").ToList();

            string query = "select  [dbo].[fn_GetUserCallCenterAgent] (SummaryDetail.CreatedBy) as AgentName, VRN, PolicyNumber,CertSerialNoDetail.CertSerialNo,ALMBranchId, Branch.BranchName, CertSerialNoDetail.CreatedOn, VehicleDetail.IsMobile from CertSerialNoDetail";
            query += " join PolicyDetail on CertSerialNoDetail.PolicyId = PolicyDetail.Id ";
            query += "  join VehicleDetail on VehicleDetail.Id=CertSerialNoDetail.VehicleId  ";
            query += " join SummaryVehicleDetail on VehicleDetail.Id=SummaryVehicleDetail.VehicleDetailsId ";
            query += " join SummaryDetail on SummaryDetail.Id=SummaryVehicleDetail.SummaryDetailId  ";
            query += " left join Branch on VehicleDetail.ALMBranchId= Branch.Id ";
            // query += " where  CertSerialNoDetail.CreatedOn>= '" + fromDate + "' And CertSerialNoDetail.CreatedOn<='" + endDate + "'";
            query += " where CertSerialNo IS NOT NULL and (  CONVERT(date, CertSerialNoDetail.CreatedOn) >= convert(date, '" + model.FromDate + "', 101)  and CONVERT(date, CertSerialNoDetail.CreatedOn) <= convert(date, '" + model.EndDate + "', 101)) ";
            if (model.BranchId == (int)ALMBranch.GeneCallCentre)
            {
                // query += " and [dbo].[fn_GetUserBranch] (CertSerialNoDetail.CreatedBy)='Gene Call Centre' ";
                query += " and VehicleDetail.ALMBranchId=0 ";
            }

            //if (model.BranchId > 0 && model.BranchId != 6) // it's for alm
            //    query += " and ALMBranchId=" + model.BranchId;

            query += " order  by CertSerialNoDetail.Id desc";

            var list = InsuranceContext.Query(query).Select(c => new CertSerialNoModel()
            {
                VRN = c.VRN,
                AgentName = c.AgentName,
                PolicyNumber = c.PolicyNumber,
                CertSerialNo = c.CertSerialNo,
                CreatedOn = c.CreatedOn == null ? "" : Convert.ToString(c.CreatedOn),
                BranchName = c.BranchName == null ? "Gene Call Centre" : c.BranchName,
                ALMBranchId = c.ALMBranchId,
                ALMBranch = GetBranchByLog(c.VRN, c.ALMBranchId, c.IsMobile, AlmList, BranchList)
            }).ToList();




            policylist.List = list;
            return View("CertSerialNoReport", policylist);
            // return View(model);
        }


        public string GetBranchByLog(string vrn, int ALMBranchId, bool IsMobile, List<LogDetailTbl> almList, List<Branch> BranchList)
        {
            string almbranch = "";

            if (ALMBranchId == 0 && IsMobile == true)
            {
                var logDetail = almList.FirstOrDefault(c => c.VRN.Contains(vrn));

                if (logDetail != null)
                {
                    var branchDetial = BranchList.FirstOrDefault(c => c.Id == Convert.ToInt32(logDetail.BranchId));

                    if (branchDetial != null)
                    {
                        almbranch = branchDetial.BranchName;
                    }
                }
            }
            return almbranch;
        }


        [Authorize(Roles = "Administrator,Reports,Finance,Team Leaders")]
        public ActionResult ALMCertSerialNoReport()
        {
            //SendGWPExcelFile();
            CertSerialNoReportModel model = new CertSerialNoReportModel();
            ViewBag.BranchList = InsuranceContext.Branches.All(where: "Id<>" + (int)ALMBranch.GeneCallCentre).ToList();

            string query = "select Customer.FirstName + ' '+ Customer.LastName as AgentName, VRN, PolicyNumber,CertSerialNoDetail.CertSerialNo,VehicleDetail.ALMBranchId, VehicleDetail.IsMobile, Branch.BranchName, CertSerialNoDetail.CreatedOn from CertSerialNoDetail";
            query += " join PolicyDetail on CertSerialNoDetail.PolicyId = PolicyDetail.Id ";
            query += " join Customer on CertSerialNoDetail.CreatedBy = Customer.Id ";
            query += " join VehicleDetail on VehicleDetail.Id=CertSerialNoDetail.VehicleId ";
            query += "  join Branch on VehicleDetail.ALMBranchId= Branch.Id ";
            query += " where CertSerialNo IS NOT NULL and  [dbo].[fn_GetUserBranch] (CertSerialNoDetail.CreatedBy)<>'Gene Call Centre'";
            query += " order by CertSerialNoDetail.id desc";

            var list = InsuranceContext.Query(query).Select(c => new CertSerialNoModel()
            {
                VRN = c.VRN,
                AgentName = c.AgentName,
                PolicyNumber = c.PolicyNumber,
                CertSerialNo = c.CertSerialNo,
                CreatedOn = c.CreatedOn == null ? "" : Convert.ToString(c.CreatedOn),
                ALMBranchId = c.ALMBranchId,
                BranchName = c.BranchName
            }).ToList();

            model.List = list;
            return View(model);
        }




        [HttpPost]
        [Authorize(Roles = "Administrator,Reports,Finance,Team Leaders")]
        public ActionResult ALMCertSerialNoSearchReport(CertSerialNoReportModel model)
        {

            //DateTime fromDate = DateTime.Now.AddDays(-1);
            //DateTime endDate = DateTime.Now;

            //if (!string.IsNullOrEmpty(model.FromDate) && !string.IsNullOrEmpty(model.EndDate))
            //{
            //    fromDate = Convert.ToDateTime(model.FromDate);
            //    endDate = Convert.ToDateTime(model.EndDate);

            //    ViewBag.fromdate = fromDate.ToString("dd/MM/yyyy");
            //    ViewBag.enddate = endDate.ToString("dd/MM/yyyy");
            //}

            ViewBag.BranchList = InsuranceContext.Branches.All(where: "Id<>" + (int)ALMBranch.GeneCallCentre).ToList();

            string query = "select [dbo].[fn_GetUserBranch] (CertSerialNoDetail.CreatedBy) as CallCenterBranch, Customer.FirstName + ' '+ Customer.LastName as AgentName, ";
            query += " VRN, PolicyNumber,CertSerialNoDetail.CertSerialNo, CertSerialNoDetail.CreatedOn,ALMBranchId, Branch.BranchName, CertSerialNoDetail.PolicyId from CertSerialNoDetail";
            query += " join PolicyDetail on CertSerialNoDetail.PolicyId = PolicyDetail.Id ";
            query += " join Customer on CertSerialNoDetail.CreatedBy = Customer.Id ";
            query += " join VehicleDetail on VehicleDetail.Id=CertSerialNoDetail.VehicleId ";
            query += " join Branch on VehicleDetail.ALMBranchId= Branch.Id ";
            query += " where  (  CONVERT(date, CertSerialNoDetail.CreatedOn) >= convert(date, '" + model.FromDate + "', 101)  and CONVERT(date, CertSerialNoDetail.CreatedOn) <= convert(date, '" + model.EndDate + "', 101)) ";

            if (model.BranchId > 0)
                query += " and ALMBranchId=" + model.BranchId;
            query += " order  by CertSerialNoDetail.Id desc";

            var list = InsuranceContext.Query(query).Select(c => new CertSerialNoModel()
            {
                VRN = c.VRN,
                AgentName = c.AgentName,
                PolicyNumber = c.PolicyNumber,
                PolicyId = c.PolicyId,
                CertSerialNo = c.CertSerialNo,
                CreatedOn = c.CreatedOn == null ? "" : Convert.ToString(c.CreatedOn),
                BranchName = c.BranchName,
                ALMBranchId = c.ALMBranchId,

            }).ToList();

            var newList = new List<CertSerialNoModel>();

            foreach (var item in list)
            {
                var policyDetail = newList.FirstOrDefault(c => c.PolicyId == item.PolicyId);
                if (policyDetail == null)
                    newList.Add(item);
            }


            model.List = newList;

            return View("ALMCertSerialNoReport", model);
            // return View(model);
        }


        #region gwp report sending email

        public void SendGWPExcelFile()
        {
            List<GrossWrittenPremiumReportModels> ListGrossWrittenPremiumReport = new List<GrossWrittenPremiumReportModels>();
            ListGrossWrittenPremiumReportModels _ListGrossWrittenPremiumReport = new ListGrossWrittenPremiumReportModels();
            _ListGrossWrittenPremiumReport.ListGrossWrittenPremiumReportdata = new List<GrossWrittenPremiumReportModels>();

            //var yesterdayDate = DateTime.Now.AddDays(-1);
            var yesterdayDate = DateTime.Now.AddMonths(-2);


            var query = " select PolicyDetail.PolicyNumber as Policy_Number, Customer.ALMId, case when Customer.ALMId is null  then  [dbo].fn_GetUserCallCenterAgent(SummaryDetail.CreatedBy) else [dbo].fn_GetUserALM(Customer.BranchId) end  as PolicyCreatedBy, Customer.FirstName + ' ' + Customer.LastName as Customer_Name,VehicleDetail.TransactionDate as Transaction_date, ";
            query += "  case when Customer.id=SummaryDetail.CreatedBy then [dbo].fn_GetUserBranch(Customer.id) else [dbo].fn_GetUserBranch(SummaryDetail.CreatedBy) end as BranchName, ";
            query += " VehicleDetail.CoverNote as CoverNoteNum, PaymentMethod.Name as Payment_Mode, PaymentTerm.Name as Payment_Term,CoverType.Name as CoverType, Currency.Name as Currency, ";
            query += " VehicleDetail.Premium + VehicleDetail.StampDuty + VehicleDetail.ZTSCLevy as Premium_due, VehicleDetail.StampDuty as Stamp_duty, VehicleDetail.ZTSCLevy as ZTSC_Levy, ";
            query += " cast(VehicleDetail.Premium * 30 / 100 as decimal(10, 2))    as Comission_Amount, VehicleDetail.IncludeRadioLicenseCost, ";
            query += " CASE WHEN IncludeRadioLicenseCost = 1 THEN VehicleDetail.RadioLicenseCost else 0 end as RadioLicenseCost, VehicleDetail.VehicleLicenceFee as Zinara_License_Fee, ";
            query += " VehicleDetail.RenewalDate as PolicyRenewalDate, VehicleDetail.IsActive, VehicleDetail.RenewPolicyNumber as RenewPolicyNumber, ";
            query += " VehicleDetail.BusinessSourceDetailId, BusinessSource.Source as BusinessSourceName, SourceDetail.FirstName + ' ' + SourceDetail.LastName as SourceDetailName from PolicyDetail ";
            query += " join Customer on PolicyDetail.CustomerId = Customer.Id ";
            query += " join VehicleDetail on PolicyDetail.Id = VehicleDetail.PolicyId ";
            query += "join SummaryVehicleDetail on VehicleDetail.id = SummaryVehicleDetail.VehicleDetailsId ";
            query += " join SummaryDetail on SummaryDetail.id = SummaryVehicleDetail.SummaryDetailId ";
            query += " join PaymentMethod on SummaryDetail.PaymentMethodId = PaymentMethod.Id ";
            query += "join PaymentTerm on VehicleDetail.PaymentTermId = PaymentTerm.Id ";
            query += " left join CoverType on VehicleDetail.CoverTypeId = CoverType.Id ";
            query += " left join Currency on VehicleDetail.CurrencyId = Currency.Id ";
            query += " left join BusinessSource on BusinessSource.Id = VehicleDetail.BusinessSourceDetailId ";
            query += " left   join SourceDetail on VehicleDetail.BusinessSourceDetailId = SourceDetail.Id join AspNetUsers on AspNetUsers.id=customer.UserID join AspNetUserRoles on AspNetUserRoles.UserId=AspNetUsers.Id ";
            query += " where (VehicleDetail.IsActive = 1 or VehicleDetail.IsActive = null) and SummaryDetail.isQuotation=0 and   CONVERT(date, VehicleDetail.TransactionDate) = convert(date, '" + yesterdayDate.ToShortDateString() + "', 101)  order by  VehicleDetail.Id desc ";

            ListGrossWrittenPremiumReport = InsuranceContext.Query(query).
                Select(x => new GrossWrittenPremiumReportModels()
                {

                    Policy_Number = x.Policy_Number,
                    BranchName = x.BranchName,
                    PolicyCreatedBy = x.PolicyCreatedBy,
                    Customer_Name = x.Customer_Name,
                    Transaction_date = x.Transaction_date.ToShortDateString(),
                    CoverNoteNum = x.CoverNoteNum,
                    Payment_Mode = x.Payment_Mode,
                    Payment_Term = x.Payment_Term,
                    CoverType = x.CoverType,
                    Currency = x.Currency,
                    Premium_due = x.Premium_due,
                    Stamp_duty = x.Stamp_duty,
                    ZTSC_Levy = x.ZTSC_Levy,
                    ALMId = x.ALMId,
                    Comission_Amount = x.Comission_Amount,
                    RadioLicenseCost = x.RadioLicenseCost,
                    Zinara_License_Fee = x.Zinara_License_Fee,
                    PolicyRenewalDate = x.PolicyRenewalDate,
                    IsActive = x.IsActive,
                    RenewPolicyNumber = x.RenewPolicyNumber,
                    SourceDetailName = x.SourceDetailName,
                }).ToList();

            DataTable dt = ConvertToDataTable(ListGrossWrittenPremiumReport);//your datatable

            DateTime date = DateTime.Now;
            int uniqueFile = date.Year * 10000 + date.Month * 100 + date.Day;
            string path = HttpContext.Server.MapPath("~/GwpExc/" + uniqueFile + "/");

            if (!Directory.Exists(path))   // CHECK IF THE FOLDER EXISTS. IF NOT, CREATE A NEW FOLDER.
            {
                Directory.CreateDirectory(path);
            }

            System.IO.File.Delete(path + "GwpReport.xlsx");

            string customExcelSavingPath = path + "GwpReport.csv";
            GenerateExcel(dt, customExcelSavingPath);
        }
        public DataTable ConvertToDataTable<T>(IList<T> data)
        {

            PropertyDescriptorCollection properties =
            TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();

            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);

            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;

                table.Rows.Add(row);
            }

            return table;
        }
        public static void GenerateExcel(DataTable dataTable, string path)
        {

            string destFilePath = "";
            //path = @"C:\inetpub\GeneWebsite_latest\CsvFile\GwpReport.csv";


            // Initilization  
            bool isSuccess = false;
            StreamWriter sw = null;

            try
            {
                // Initialization.  
                StringBuilder stringBuilder = new StringBuilder();

                // Saving Column header.  
                stringBuilder.Append(string.Join(",", dataTable.Columns.Cast<DataColumn>().Select(column => column.ColumnName).ToList()) + "\n");

                // Saving rows.  
                dataTable.AsEnumerable().ToList<DataRow>().ForEach(row => stringBuilder.Append(string.Join(",", row.ItemArray) + "\n"));

                // Initialization.  
                string fileContent = stringBuilder.ToString();
                sw = new StreamWriter(new FileStream(destFilePath, FileMode.Create, FileAccess.Write));

                // Saving.  
                sw.Write(fileContent);

                // Settings.  
                isSuccess = true;


                List<string> _attachements = new List<string>();
                //urlPath

                string urlPath = System.Configuration.ConfigurationManager.AppSettings["urlPath"];

                path = urlPath + @"CsvFile/GwpReport.csv";

                _attachements.Add(path);

                //  string body = "Please check attached =" + DateTime.Now.ToShortDateString() + " GWP Report";


                StringBuilder mailBody = new StringBuilder();
                mailBody.AppendFormat("<h1>Please click below link to get gwp report.</h1>");
                mailBody.AppendFormat("<p><a href='" + path + "'>GWPReport</a></p>");

                Insurance.Service.EmailService objEmailService = new Insurance.Service.EmailService();
                objEmailService.SendEmail("it@gene.co.zw", "", "", "GWPReport_" + DateTime.Now.ToShortDateString(), mailBody.ToString(), _attachements);




            }
            catch (Exception ex)
            {
                // Info.  
                throw ex;
            }
            finally
            {
                // Closing.  
                sw.Flush();
                sw.Dispose();
                sw.Close();
            }
        }



        #endregion



        // [Authorize(Roles = "Administrator,Reports,Finance")]
        public ActionResult DisablePolicy()
        {
            ListPolicy policylist = new ListPolicy();
            try
            {



                string query = "select top 500 PolicyDetail.PolicyNumber,  VehicleDetail.RegistrationNo, VehicleDetail.ReasonContent, VehicleMake.MakeDescription, VehicleModel.ModelDescription, ";
                query += " Customer.FirstName + ' '+ Customer.LastName as CustomerName, VehicleDetail.Premium , VehicleDetail.StampDuty, VehicleDetail.ZTSCLevy ,VehicleDetail.VehicleLicenceFee, ";
                //query += " VehicleDetail.IncludeRadioLicenseCost, convert(varchar, VehicleDetail.ModifiedOn, 1) as ModifiedOn , VehicleDetail.RadioLicenseCost, VehicleDetail.TransactionDate, CASE WHEN Branch.BranchName IS NULL THEN 'Gene Call Centre' ELSE Branch.BranchName END as BranchName from VehicleDetail ";
                query += " VehicleDetail.IncludeRadioLicenseCost, VehicleDetail.ModifiedOn , VehicleDetail.RadioLicenseCost, VehicleDetail.TransactionDate, CASE WHEN Branch.BranchName IS NULL THEN 'Gene Call Centre' ELSE Branch.BranchName END as BranchName from VehicleDetail ";
                query += " join PolicyDetail on PolicyDetail.Id=VehicleDetail.PolicyId ";
                query += " join SummaryVehicleDetail on VehicleDetail.Id=SummaryVehicleDetail.Id ";
                query += " join SummaryDetail on SummaryDetail.Id= SummaryVehicleDetail.SummaryDetailId ";
                query += "  join Customer on VehicleDetail.CustomerId=Customer.Id ";
                query += " left join VehicleMake on VehicleDetail.MakeId=VehicleMake.MakeCode ";
                query += " left join VehicleModel on VehicleDetail.ModelId=VehicleModel.ModelDescription  ";
                query += " left join Branch on VehicleDetail.ALMBranchId=Branch.Id  ";
                query += " where VehicleDetail.IsActive=0 and isLapsed=0 order by VehicleDetail.id desc ";


                policylist.listpolicy = new List<PolicyListViewModel>();
                PolicyListViewModel policylistviewmodel = new PolicyListViewModel();
                var paymentInformationList = InsuranceContext.PaymentInformations.All();


                List<PolicyListViewModel> list = InsuranceContext.Query(query).Select(x => new PolicyListViewModel()
                {
                    PolicyNumber = x.PolicyNumber,
                    RegisterationNumber = x.RegistrationNo,
                    Make = x.MakeDescription,
                    Model = x.ModelDescription,
                    CustomerName = x.CustomerName,
                    Premium = x.Premium == null ? 0 : Convert.ToDecimal(x.Premium),
                    StampDuty = x.StampDuty == null ? 0 : Convert.ToDecimal(x.StampDuty),
                    VehicleLicenseFee = x.VehicleLicenceFee == null ? 0 : Convert.ToDecimal(x.VehicleLicenceFee),
                    RadioLicesneCost = x.RadioLicenseCost == null ? 0 : Convert.ToDecimal(x.RadioLicenseCost),
                    ZtscLevy = x.ZTSCLevy == null ? 0 : Convert.ToDecimal(x.ZTSCLevy),
                    TotalPremium = CalculatePremium(x.Premium == null ? 0 : Convert.ToDecimal(x.Premium), x.StampDuty == null ? 0 : Convert.ToDecimal(x.StampDuty), x.ZTSCLevy == null ? 0 : Convert.ToDecimal(x.ZTSCLevy), x.VehicleLicenceFee == null ? 0 : Convert.ToDecimal(x.VehicleLicenceFee), x.IncludeRadioLicenseCost, x.RadioLicenseCost == null ? 0 : Convert.ToDecimal(x.RadioLicenseCost)),
                    //createdOn = x.TransactionDate,
                    createdOndate= x.TransactionDate == null ? "" : x.TransactionDate.ToString("MM/dd/yyyy"),
                    // AgentName = x.CreatedBy,
                    ModifiedOn = x.ModifiedOn == null ? "" : x.ModifiedOn.ToString("MM/dd/yyyy"),
                    CancelReason = x.ReasonContent,
                    // CreatedBy= Convert.ToString(x.CreatedBy),
                    BranchName = x.BranchName
                }).ToList();

                policylist.listpolicy = list;

            }
            catch (Exception ex)
            {

            }

            return View(policylist);
        }

        [HttpPost]
        public ActionResult SearchDisablePolicy(ListPolicy model)
        {

            ListPolicy policylist = new ListPolicy();
            try
            {


                string query = "select PolicyDetail.PolicyNumber, VehicleDetail.RegistrationNo, VehicleDetail.ReasonContent, VehicleMake.MakeDescription, VehicleModel.ModelDescription, ";
                query += " Customer.FirstName + ' '+ Customer.LastName as CustomerName, VehicleDetail.Premium , VehicleDetail.StampDuty, VehicleDetail.ZTSCLevy ,VehicleDetail.VehicleLicenceFee,  ";
                query += " VehicleDetail.IncludeRadioLicenseCost, VehicleDetail.ModifiedOn , VehicleDetail.RadioLicenseCost, VehicleDetail.TransactionDate, CASE WHEN Branch.BranchName IS NULL THEN 'Gene Call Centre' ELSE Branch.BranchName END as BranchName from VehicleDetail ";
                query += " join PolicyDetail on PolicyDetail.Id=VehicleDetail.PolicyId ";
                query += " join SummaryVehicleDetail on VehicleDetail.Id=SummaryVehicleDetail.Id ";
                query += " join SummaryDetail on SummaryDetail.Id= SummaryVehicleDetail.SummaryDetailId ";
                query += "  join Customer on VehicleDetail.CustomerId=Customer.Id ";
                query += " left join VehicleMake on VehicleDetail.MakeId=VehicleMake.MakeCode ";
                query += " left join VehicleModel on VehicleDetail.ModelId=VehicleModel.ModelDescription  ";
                query += " left join Branch on VehicleDetail.ALMBranchId=Branch.Id   ";
                query += " where VehicleDetail.IsActive=0 and isLapsed=0  ";
                query += "   and (CONVERT(date, VehicleDetail.ModifiedOn) >= convert(date, '" + model.FromDate + "', 101)  and CONVERT(date, VehicleDetail.ModifiedOn) <= convert(date, '" + model.EndDate + "', 101))  order by  VehicleDetail.Id desc";

                //query += "   and (CONVERT(date, VehicleDetail.TransactionDate) >= convert(date, '" + model.FromDate + "', 101)  and CONVERT(date, VehicleDeta
                //il.TransactionDate) <= convert(date, '" + model.ModifiedOn + "', 101))  order by  VehicleDetail.Id desc";



                policylist.listpolicy = new List<PolicyListViewModel>();
                PolicyListViewModel policylistviewmodel = new PolicyListViewModel();
                var paymentInformationList = InsuranceContext.PaymentInformations.All();


                List<PolicyListViewModel> list = InsuranceContext.Query(query).Select(x => new PolicyListViewModel()
                {
                    PolicyNumber = x.PolicyNumber,
                    RegisterationNumber = x.RegistrationNo,
                    Make = x.MakeDescription,
                    Model = x.ModelDescription,
                    CustomerName = x.CustomerName,
                    Premium = x.Premium == null ? 0 : Convert.ToDecimal(x.Premium),
                    StampDuty = x.StampDuty == null ? 0 : Convert.ToDecimal(x.StampDuty),
                    VehicleLicenseFee = x.VehicleLicenceFee == null ? 0 : Convert.ToDecimal(x.VehicleLicenceFee),
                    RadioLicesneCost = x.RadioLicenseCost == null ? 0 : Convert.ToDecimal(x.RadioLicenseCost),
                    ZtscLevy = x.ZTSCLevy == null ? 0 : Convert.ToDecimal(x.ZTSCLevy),
                    TotalPremium = CalculatePremium(x.Premium == null ? 0 : Convert.ToDecimal(x.Premium), x.StampDuty == null ? 0 : Convert.ToDecimal(x.StampDuty), x.ZTSCLevy == null ? 0 : Convert.ToDecimal(x.ZTSCLevy), x.VehicleLicenceFee == null ? 0 : Convert.ToDecimal(x.VehicleLicenceFee), x.IncludeRadioLicenseCost, x.RadioLicenseCost == null ? 0 : Convert.ToDecimal(x.RadioLicenseCost)),
                    // createdOn = x.TransactionDate,
                    //AgentName = x.CreatedBy,
                    createdOndate = x.TransactionDate == null ? "" : x.TransactionDate.ToString("MM/dd/yyyy"),
                    // AgentName = x.CreatedBy,
                    ModifiedOn = x.ModifiedOn == null ? "" : x.ModifiedOn.ToString("MM/dd/yyyy"),

                    BranchName = x.BranchName
                }).ToList();

                policylist.listpolicy = list;
            }
            catch (Exception ex)
            {

            }

            return View("DisablePolicy", policylist);
        }

        public ActionResult Endorsments()
        {
            GrossWrittenPremiumReportSearchModels model = new GrossWrittenPremiumReportSearchModels();
            model.ListGrossWrittenPremiumReportdata = GetGWPEndorsmentReport(new GrossWrittenPremiumReportSearchModels());
            return View(model);
        }

        [HttpPost]
        public ActionResult SearchEndorsments(GrossWrittenPremiumReportSearchModels model)
        {
            model.ListGrossWrittenPremiumReportdata = GetGWPEndorsmentReport(model);
            return View("Endorsments", model);
        }

        public decimal CalculatePremium(decimal premium, decimal stampDuty, decimal ztscLevy, decimal licensefee, bool IsIncludePrem, decimal? radioLicenseCost)
        {
            decimal calpremium = 0;

            calpremium = premium + stampDuty + ztscLevy + licensefee;
            if (IsIncludePrem)
                calpremium = calpremium + Convert.ToDecimal(radioLicenseCost);

            return calpremium;
        }


        public ActionResult DailyReport()
        {
            // SendZinaraDailyReport();
            SendFWPSummaryReport();
            return View();
        }


        [HttpPost]
        public ActionResult ALMPartnerReports(ALMParnterSearchModels _model)
        {

            List<PartnerModel> ListPartnerModel = InsuranceContext.Query("select * from Partners").Select(x => new PartnerModel
            {
                Id = x.Id,
                PartnerName = x.PartnerName,
                Status = x.Status
            }).ToList();

            List<ALMParnterSearchModels> ListALMParnterSearchdata = new List<ALMParnterSearchModels>();
            ALMParnterSearchModels Model = new ALMParnterSearchModels();
            int Selected = _model.PartnerId;

            string query = "select PolicyDetail.PolicyNumber as PolicyNumber, Branch.BranchName as BranchName,  convert(varchar, PolicyDetail.CreatedOn, 106) as CreatedOn , PaymentInformation.PaymentId, VehicleDetail.Premium,cast(VehicleDetail.Premium * 30 / 100 as decimal(10, 2))    as Comission_Amount, Customer.ALMId from PolicyDetail join Customer on Customer.Id = PolicyDetail.CustomerId join VehicleDetail on VehicleDetail.PolicyId = PolicyDetail.Id join PaymentInformation on PaymentInformation.PolicyId = PolicyDetail.Id join Branch on VehicleDetail.ALMBranchId = Branch.Id join Partners on Partners.Id = Branch.PartnerId where PolicyDetail.CreatedOn BETWEEN '" + _model.FormDate + "' AND '" + _model.EndDate + "' and branch.PartnerId= " + _model.PartnerId;

            List<ALMParnterSearchModelsData> data = InsuranceContext.Query(query).Select(x => new ALMParnterSearchModelsData
            {
                BranchName = x.BranchName,
                PolicyNumber = x.PolicyNumber,
                PaymentDetail = x.PaymentId,
                PolicyDate = x.CreatedOn,
                GrossPremium = x.Premium,
                CommissionAmount = x.Comission_Amount,

            }).ToList();

            Model.ListReportdata = data;
            ViewBag.Partner = ListPartnerModel;

            return View("ALMGWPPartnerReport", Model);



        }


        [Authorize(Roles = "Administrator,Reports,Finance")]
        public ActionResult ALMGWPPartnerReport()
        {

            //  GetRecieptReport();

            List<PartnerModel> ListPartnerModel = InsuranceContext.Query("select * from Partners").Select(x => new PartnerModel
            {
                Id = x.Id,
                PartnerName = x.PartnerName,
                Status = x.Status
            }).ToList();


            List<ALMParnterSearchModels> ListALMParnterSearchdata = new List<ALMParnterSearchModels>();
            ALMParnterSearchModels Model = new ALMParnterSearchModels();
            int Selected = 1;

            string query = "select PolicyDetail.PolicyNumber as PolicyNumber, PartnerCommissions.CommissionPercentage ,Branch.BranchName as BranchName,  convert(varchar, PolicyDetail.CreatedOn, 106) as CreatedOn , PaymentInformation.PaymentId,  VehicleDetail.Premium,cast(VehicleDetail.Premium * PartnerCommissions.CommissionPercentage as decimal(10, 2)) as Comission_Amount, Customer.ALMId from PolicyDetail join Customer on Customer.Id = PolicyDetail.CustomerId join VehicleDetail on VehicleDetail.PolicyId = PolicyDetail.Id join PaymentInformation on PaymentInformation.PolicyId = PolicyDetail.Id join Branch on VehicleDetail.ALMBranchId = Branch.Id join Partners on Partners.Id = Branch.PartnerId join PartnerCommissions on CommissionEffectiveDate <= PolicyDetail.CreatedOn and PartnerCommissions.PartnerId = branch.PartnerId where branch.PartnerId=" + Selected;

            List<ALMParnterSearchModelsData> data = InsuranceContext.Query(query).Select(x => new ALMParnterSearchModelsData
            {
                BranchName = x.BranchName,
                PolicyNumber = x.PolicyNumber,
                PaymentDetail = x.PaymentId,
                PolicyDate = x.CreatedOn,
                GrossPremium = x.Premium,
                CommissionAmount = x.Comission_Amount,

            }).ToList();
            Model.ListReportdata = data;
            Model.ListPartnerModelData = ListPartnerModel;
            ViewBag.Partner = ListPartnerModel;

            return View(Model);
        }

        public ActionResult UnRecieptReport()
        {
            var recieptlist = InsuranceContext.ReceiptHistorys.All().ToList();
            var vehicleinform = InsuranceContext.VehicleDetails.All().ToList();
            UnRecieptDetail UnReciptModel = new UnRecieptDetail();

            string startDate = DateTime.Now.AddMonths(-2).ToShortDateString();
            string endDate = DateTime.Now.ToShortDateString();

            var query = " select top 800 [dbo].[fn_GetUserCallCenterAgent] (SummaryDetail.CreatedBy) as AgentName, SummaryDetail.Id as SummaryDetailId, PolicyDetail.id as PolicyId,  PolicyDetail.PolicyNumber,VehicleDetail.ALMBranchId, VehicleDetail.RegistrationNo,VehicleDetail.TransactionDate, VehicleDetail.RenewPolicyNumber, Customer.FirstName + ' ' + Customer.LastName as CustomerName, SummaryDetail.TotalPremium  from PolicyDetail join VehicleDetail on PolicyDetail.Id = VehicleDetail.PolicyId ";
            query += " join SummaryVehicleDetail on VehicleDetail.Id = SummaryVehicleDetail.VehicleDetailsId ";
            query += " join SummaryDetail on SummaryDetail.Id = SummaryVehicleDetail.SummaryDetailId join Customer on VehicleDetail.CustomerId=Customer.Id ";
            query += " where(VehicleDetail.IsActive = 1 or VehicleDetail.IsActive = null) and (VehicleDetail.ALMBranchId <=0) and SummaryDetail.isQuotation = 0  order by VehicleDetail.TransactionDate  desc";

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["Insurance"].ToString();

            var result = new List<RecieptModel>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    SqlDataReader reader = command.ExecuteReader();
                    // Call Read before accessing data.
                    while (reader.Read())
                    {
                        RecieptModel model = new RecieptModel();
                        //if (reader["ALMBranchId"] == null || Convert.ToInt32(reader["ALMBranchId"]) <= 0)
                        //{
                       
                           model.ALMBranchId = reader["ALMBranchId"] == DBNull.Value ? 0: Convert.ToInt32(reader["ALMBranchId"]);
                        if (model.ALMBranchId <= 0 || model.ALMBranchId==null)
                        {
                            model.AgentName = reader["AgentName"] == null ? "" : Convert.ToString(reader["AgentName"]);
                            model.Policy_Number = reader["PolicyNumber"] == null ? "" : Convert.ToString(reader["PolicyNumber"]);
                            model.VRN = reader["RegistrationNo"] == null ? "" : Convert.ToString(reader["RegistrationNo"]);
                            model.Transaction_date = reader["TransactionDate"] == null ? "" : Convert.ToString(reader["TransactionDate"]);
                            model.Customer_Name = reader["CustomerName"] == null ? "" : Convert.ToString(reader["CustomerName"]);
                            model.Premium_due = reader["TotalPremium"] == null ? 0 : Convert.ToDecimal(reader["TotalPremium"]);
                            model.PolicyId = reader["PolicyId"] == null ? 0 : Convert.ToInt32(reader["PolicyId"]);
                            model.SummaryDetailId = reader["SummaryDetailId"] == null ? 0 : Convert.ToInt32(reader["SummaryDetailId"]);
                            model.RenewPolicyNumber = reader["RenewPolicyNumber"] == null ? "" : Convert.ToString(reader["RenewPolicyNumber"]);

                            result.Add(model);
                        }
                    }
                    reader.Close();
                }
                connection.Close();
            }


            var query2 = "select * from ReceiptModuleHistory where (CONVERT(date, ReceiptModuleHistory.CreatedOn) >= convert(date, '" + startDate + "', 101) ";
            query2 += " and CONVERT(date, ReceiptModuleHistory.CreatedOn) <= convert(date, '" + endDate + "', 101)) ";

            var recieptList = new List<RecieptModel>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query2, connection))
                {
                    SqlDataReader reader = command.ExecuteReader();
                    // Call Read before accessing data.
                    while (reader.Read())
                    {
                        RecieptModel model = new RecieptModel();
                        model.Transaction_date = reader["CreatedOn"] == null ? "" : Convert.ToString(reader["CreatedOn"]);
                        model.PolicyId = reader["PolicyId"] == null ? 0 : Convert.ToInt32(reader["PolicyId"]);
                        model.SummaryDetailId = reader["SummaryDetailId"] == null ? 0 : Convert.ToInt32(reader["SummaryDetailId"]);
                        recieptList.Add(model);
                    }
                    reader.Close();
                }
                connection.Close();
            }

            List<RecieptDetail> list = new List<RecieptDetail>();
            DateTime dtCurrent = Convert.ToDateTime(DateTime.Now.ToShortDateString());
            foreach (var item in result)
            {
                //var rslt = vehicleinform.Where(c => c.ALMBranchId == item.ALMBranchId).FirstOrDefault();

                //    if (rslt.ALMBranchId==0)
                //{ 
                //item.RecieptStatus = GetSummaryStatus(recieptlist, item.RenewPolicyNumber, item.SummaryDetailId, item.ALMBranchId);
                DateTime policyCreatedDate = Convert.ToDateTime(item.Transaction_date);
                TimeSpan t = dtCurrent.Subtract(policyCreatedDate);

                    //if (item.RecieptStatus == "Unreceipted")
                    //{
                        var receiptDetail = recieptlist.Where(c => c.PolicyId == item.PolicyId).FirstOrDefault();
                        if (receiptDetail == null)
                        {
                            item.Days = t.TotalDays;
                            RecieptDetail detail = new RecieptDetail
                            {
                                AgentName = item.AgentName,
                                Policy_Number = item.Policy_Number,
                                Renew_Policy_Number= item.RenewPolicyNumber,
                                Customer_Name = item.Customer_Name,
                                Premium_due = item.Premium_due,
                                Days = item.Days,
                                VRN = item.VRN,
                                Transaction_date = item.Transaction_date
                            };
                            list.Add(detail);
                        }
                    //}
                //}
            }

            UnReciptModel.RecieptDetails = list;


            return View(UnReciptModel);
        }

        public string GetSummaryStatus(List<ReceiptModuleHistory> recieptList, string renewPolicyNumber, int Id, int? almbranchid)
        {
            string summaryStatus = "";

            int number = 0;

            if (!string.IsNullOrEmpty(renewPolicyNumber))
            {
                var renewPolicyDetail = renewPolicyNumber.Split('-');
                number = Convert.ToInt32(renewPolicyDetail[1]);
            }



            var Summarydetaild = recieptList.FirstOrDefault(c => c.SummaryDetailId == Id);
            if (Summarydetaild != null || almbranchid > 0)
            {
                if (number > 1)
                {
                    var renewReceptDetail = recieptList.Where(c => c.SummaryDetailId == Id && c.RenewPolicyNumber == renewPolicyNumber).FirstOrDefault(); ;

                    if (renewReceptDetail != null)
                        summaryStatus = "Receipted";
                    else
                        summaryStatus = "Unreceipted";

                }
                else
                {
                    summaryStatus = "Receipted";
                }

            }
            else
            {
                summaryStatus = "Unreceipted";
            }
            return summaryStatus;
        }


        [HttpPost]
        public ActionResult SearchUnRecieptReport(UnRecieptDetail _model)
        {
            var recieptlist = InsuranceContext.ReceiptHistorys.All().ToList();
            string startDate = _model.FromDate;
            string endDate = _model.EndDate;

            UnRecieptDetail UnReciptModel = new UnRecieptDetail();

            var query = " select [dbo].[fn_GetUserCallCenterAgent] (SummaryDetail.CreatedBy) as AgentName, SummaryDetail.Id as SummaryDetailId, PolicyDetail.id as PolicyId,  PolicyDetail.PolicyNumber,VehicleDetail.ALMBranchId, VehicleDetail.RegistrationNo,VehicleDetail.TransactionDate, VehicleDetail.RenewPolicyNumber, Customer.FirstName + ' ' + Customer.LastName as CustomerName, SummaryDetail.TotalPremium  from PolicyDetail join VehicleDetail on PolicyDetail.Id = VehicleDetail.PolicyId ";
            query += " join SummaryVehicleDetail on VehicleDetail.Id = SummaryVehicleDetail.VehicleDetailsId ";
            query += " join SummaryDetail on SummaryDetail.Id = SummaryVehicleDetail.SummaryDetailId join Customer on VehicleDetail.CustomerId=Customer.Id ";
            query += " where(VehicleDetail.IsActive = 1 or VehicleDetail.IsActive = null)  and SummaryDetail.isQuotation = 0 and(CONVERT(date, VehicleDetail.TransactionDate) >= convert(date, '" + startDate + "', 101)  and CONVERT(date, VehicleDetail.TransactionDate) <= convert(date, '" + endDate + "', 101))  order by VehicleDetail.TransactionDate  desc";

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["Insurance"].ToString();

            var result = new List<RecieptModel>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    SqlDataReader reader = command.ExecuteReader();
                    // Call Read before accessing data.
                    while (reader.Read())
                    {
                        RecieptModel model = new RecieptModel();
                        //if (reader["ALMBranchId"] == null || Convert.ToInt32(reader["ALMBranchId"]) <= 0)
                        //{
                            model.ALMBranchId = reader["ALMBranchId"] == null ? 0 : Convert.ToInt32(reader["ALMBranchId"]);
                        if (model.ALMBranchId <= 0 || model.ALMBranchId == null)
                        { 
                            model.AgentName = reader["AgentName"] == null ? "" : Convert.ToString(reader["AgentName"]);
                            model.Policy_Number = reader["PolicyNumber"] == null ? "" : Convert.ToString(reader["PolicyNumber"]);
                            model.VRN = reader["RegistrationNo"] == null ? "" : Convert.ToString(reader["RegistrationNo"]);
                            model.Transaction_date = reader["TransactionDate"] == null ? "" : Convert.ToString(reader["TransactionDate"]);
                            model.Customer_Name = reader["CustomerName"] == null ? "" : Convert.ToString(reader["CustomerName"]);
                            model.Premium_due = reader["TotalPremium"] == null ? 0 : Convert.ToDecimal(reader["TotalPremium"]);
                            model.PolicyId = reader["PolicyId"] == null ? 0 : Convert.ToInt32(reader["PolicyId"]);
                            model.SummaryDetailId = reader["SummaryDetailId"] == null ? 0 : Convert.ToInt32(reader["SummaryDetailId"]);
                            model.RenewPolicyNumber = reader["RenewPolicyNumber"] == null ? "" : Convert.ToString(reader["RenewPolicyNumber"]);

                            result.Add(model);
                        }
                    }
                    reader.Close();
                }
                connection.Close();
            }


            var query2 = "select * from ReceiptModuleHistory where (CONVERT(date, ReceiptModuleHistory.CreatedOn) >= convert(date, '" + startDate + "', 101) ";
            query2 += " and CONVERT(date, ReceiptModuleHistory.CreatedOn) <= convert(date, '" + endDate + "', 101)) ";

            var recieptList = new List<RecieptModel>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query2, connection))
                {
                    SqlDataReader reader = command.ExecuteReader();
                    // Call Read before accessing data.
                    while (reader.Read())
                    {
                        RecieptModel model = new RecieptModel();
                        model.Transaction_date = reader["CreatedOn"] == null ? "" : Convert.ToString(reader["CreatedOn"]);
                        model.PolicyId = reader["PolicyId"] == null ? 0 : Convert.ToInt32(reader["PolicyId"]);
                        model.SummaryDetailId = reader["SummaryDetailId"] == null ? 0 : Convert.ToInt32(reader["SummaryDetailId"]);
                        recieptList.Add(model);
                    }
                    reader.Close();
                }
                connection.Close();
            }

            List<RecieptDetail> list = new List<RecieptDetail>();
            DateTime dtCurrent = Convert.ToDateTime(DateTime.Now.ToShortDateString());
            foreach (var item in result)
            {
                //item.RecieptStatus = GetSummaryStatus(recieptlist, item.RenewPolicyNumber, item.SummaryDetailId, item.ALMBranchId);
                DateTime policyCreatedDate = Convert.ToDateTime(item.Transaction_date);
                TimeSpan t = dtCurrent.Subtract(policyCreatedDate);

                //if (item.RecieptStatus == "Unreceipted")
                //{
                   var receiptDetail = recieptlist.Where(c => c.PolicyId == item.PolicyId).FirstOrDefault();
                        if (receiptDetail == null)
                    {
                        item.Days = t.TotalDays;
                        RecieptDetail detail = new RecieptDetail
                        {
                            AgentName = item.AgentName,
                            Policy_Number = item.Policy_Number,
                            Customer_Name = item.Customer_Name,
                            Premium_due = item.Premium_due,
                            Days = item.Days,
                            VRN = item.VRN,
                            Transaction_date = item.Transaction_date
                        };
                        list.Add(detail);
                    }
                
            }

            UnReciptModel.RecieptDetails = list;

            return View("UnRecieptReport", UnReciptModel);


        }


        public void GetRecieptReport()
        {

            string startDate = DateTime.Now.AddMonths(-2).ToShortDateString();
            string endDate = DateTime.Now.ToShortDateString();

            var query = " select [dbo].[fn_GetUserCallCenterAgent] (SummaryDetail.CreatedBy) as AgentName, SummaryDetail.Id as SummaryDetailId, PolicyDetail.id as PolicyId,  PolicyDetail.PolicyNumber,VehicleDetail.ALMBranchId, VehicleDetail.RegistrationNo,VehicleDetail.TransactionDate, Customer.FirstName + ' ' + Customer.LastName as CustomerName, SummaryDetail.TotalPremium  from PolicyDetail join VehicleDetail on PolicyDetail.Id = VehicleDetail.PolicyId ";
            query += " join SummaryVehicleDetail on VehicleDetail.Id = SummaryVehicleDetail.Id ";
            query += " join SummaryDetail on SummaryDetail.Id = SummaryVehicleDetail.SummaryDetailId join Customer on VehicleDetail.CustomerId=Customer.Id ";
            query += " where(VehicleDetail.IsActive = 1 or VehicleDetail.IsActive = null) and SummaryDetail.isQuotation = 0 and [dbo].[fn_GetUserBranch] (SummaryDetail.CreatedBy) ='Gene Call Centre'  and(CONVERT(date, VehicleDetail.TransactionDate) >= convert(date, '" + startDate + "', 101)  and CONVERT(date, VehicleDetail.TransactionDate) <= convert(date, '" + endDate + "', 101)) order by VehicleDetail.TransactionDate  desc";

            string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["Insurance"].ToString();

            var result = new List<RecieptModel>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    SqlDataReader reader = command.ExecuteReader();
                    // Call Read before accessing data.
                    while (reader.Read())
                    {
                        RecieptModel model = new RecieptModel();

                        if (reader["ALMBranchId"] == null || Convert.ToInt32(reader["ALMBranchId"]) <= 0)
                        {
                            model.AgentName = reader["AgentName"] == null ? "" : Convert.ToString(reader["AgentName"]);
                            model.Policy_Number = reader["PolicyNumber"] == null ? "" : Convert.ToString(reader["PolicyNumber"]);
                            model.VRN = reader["RegistrationNo"] == null ? "" : Convert.ToString(reader["RegistrationNo"]);
                            model.Transaction_date = reader["TransactionDate"] == null ? "" : Convert.ToString(reader["TransactionDate"]);
                            model.Customer_Name = reader["CustomerName"] == null ? "" : Convert.ToString(reader["CustomerName"]);
                            model.Premium_due = reader["TotalPremium"] == null ? 0 : Convert.ToDecimal(reader["TotalPremium"]);
                            model.PolicyId = reader["PolicyId"] == null ? 0 : Convert.ToInt32(reader["PolicyId"]);
                            model.SummaryDetailId = reader["SummaryDetailId"] == null ? 0 : Convert.ToInt32(reader["SummaryDetailId"]);

                            result.Add(model);
                        }
                    }
                    reader.Close();
                }
                connection.Close();
            }



            var query2 = "select * from ReceiptModuleHistory where (CONVERT(date, ReceiptModuleHistory.CreatedOn) >= convert(date, '" + startDate + "', 101) ";
            query2 += " and CONVERT(date, ReceiptModuleHistory.CreatedOn) <= convert(date, '" + endDate + "', 101)) ";



            var recieptList = new List<RecieptModel>();
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query2, connection))
                {
                    SqlDataReader reader = command.ExecuteReader();
                    // Call Read before accessing data.
                    while (reader.Read())
                    {
                        RecieptModel model = new RecieptModel();
                        model.Transaction_date = reader["CreatedOn"] == null ? "" : Convert.ToString(reader["CreatedOn"]);
                        model.PolicyId = reader["PolicyId"] == null ? 0 : Convert.ToInt32(reader["PolicyId"]);
                        model.SummaryDetailId = reader["SummaryDetailId"] == null ? 0 : Convert.ToInt32(reader["SummaryDetailId"]);
                        recieptList.Add(model);

                    }
                    reader.Close();
                }
                connection.Close();
            }




            List<RecieptDetail> list = new List<RecieptDetail>();

            DateTime dtCurrent = Convert.ToDateTime(DateTime.Now.ToShortDateString());

            foreach (var item in result)
            {
                DateTime policyCreatedDate = Convert.ToDateTime(item.Transaction_date);
                TimeSpan t = dtCurrent.Subtract(policyCreatedDate);
                if (t.TotalDays == 7 || t.TotalDays == 14 || t.TotalDays == 21 || t.TotalDays == 30)
                {
                    var receiptDetail = recieptList.FirstOrDefault(c => c.PolicyId == item.PolicyId);
                    if (receiptDetail == null)
                    {
                        item.Days = t.TotalDays;

                        RecieptDetail detail = new RecieptDetail
                        {
                            AgentName = item.AgentName,
                            Policy_Number = item.Policy_Number,
                            Customer_Name = item.Customer_Name,
                            Premium_due = item.Premium_due,
                            Days = item.Days,
                            VRN = item.VRN,
                            Transaction_date = item.Transaction_date
                        };


                        list.Add(detail);
                    }
                }
            }


            MemoryStream outputStream = new MemoryStream();
            using (ExcelPackage package = new ExcelPackage(outputStream))
            {
                var src = DateTime.Now;
                var hm = new DateTime(src.Year, src.Month, src.Day, src.Hour, src.Minute, 0);


                // export each facility's rollup and detail to tabs in Excel (two tabs per facility)
                ExcelWorksheet facilityWorksheet = package.Workbook.Worksheets.Add("Unreceipt Report");
                facilityWorksheet.Cells["A1"].LoadFromText("Receipt Report").Style.Font.Bold = true;
                facilityWorksheet.Cells["A3"].Value = "Report Generated Date: " + DateTime.Now.ToString("HH:mm") + " " + DateTime.Now.ToShortDateString();
                facilityWorksheet.Cells[4, 1].LoadFromCollection(list, true, OfficeOpenXml.Table.TableStyles.Light1);

                //facilityDetail.Cells.LoadFromDataTable(dataTable, true);

                package.Save();

                outputStream.Position = 0;

                List<Stream> _attachements = new List<Stream>();
                List<AttachmentModel> attachmentModels = new List<AttachmentModel>();
                AttachmentModel attachment = new AttachmentModel();
                attachment.Attachment = outputStream;
                attachment.Name = "Unreceipt Report.xlsx";
                attachmentModels.Add(attachment);

                _attachements.Add(outputStream);
                Debug.WriteLine("************Attached*************");

                StringBuilder mailBody = new StringBuilder();
                mailBody.AppendFormat("<div>Please Find Attached.</div>");

                Debug.WriteLine("***********SendEmail**************");
                string email = System.Configuration.ConfigurationManager.AppSettings["gwpemail"];

                email = "kindlebit.net@gmail.com";
                Insurance.Service.EmailService objEmailService = new Insurance.Service.EmailService();
                objEmailService.SendAttachedEmail(email, "", "", "Unreceipt Report - " + DateTime.Now.ToLongDateString(), mailBody.ToString(), attachmentModels);


            }





        }



        public void SendZinaraDailyReport()
        {
            WeeklyGWPService service = new WeeklyGWPService();
            service.SendWeeklyGwpFile();
        }

        public void SendFWPSummaryReport()
        {
            WeeklyGWPService service = new WeeklyGWPService();
            service.SendWeeklyReport();
        }
        public ActionResult NewReconciliationSearchReport(NewReconcilationReportModel model)
        {
            var customerList = InsuranceContext.Customers.All().ToList();

            string invoiceSQL = "select sum(Amount) as invoice, policyId , policyNumber , CreatedOn, CreatedBy from ReceiptAndPayment  where type='invoice' " +
                 "AND CreatedOn BETWEEN '" + model.FromDate + "' AND '" + model.EndDate + "' " +
                 "group by  policyId,policyNumber , CreatedOn, CreatedBy ";



            var InvoiceList = InsuranceContext.Query(invoiceSQL).Select(x => new PolicyInvoice
            {


                PolicyId = x.policyId,
                InvoiceAmount = x.invoice * -1,
                PolicyNumber = x.policyNumber,
                CreateOn = x.CreatedOn,
                CreatedBy = x.CreatedBy

            });

            model.listInvoiceAndReciept = new List<RecieptAndPaymentModel>();
            foreach (PolicyInvoice invoice in InvoiceList)
            {

                RecieptAndPaymentModel recieptAndPaymentModel = new RecieptAndPaymentModel();
                recieptAndPaymentModel.PremiumDue = invoice.InvoiceAmount;
                recieptAndPaymentModel.PolicyNumber = invoice.PolicyNumber;
                recieptAndPaymentModel.PolicyCreatedOn = invoice.CreateOn;
                recieptAndPaymentModel.TransactionDate = invoice.CreateOn;
                var AmountPaid = InsuranceContext.Query("select  COALESCE(sum(Amount), 0 )  as balance , max(CreatedOn) as CreatedOn from ReceiptAndPayment  where type='reciept' and  policyId='" + invoice.PolicyId + "' ").Select(x => new Balance
                {
                    balance = x.balance,
                    CreatedOn = x.CreatedOn == null ? "--" : x.CreatedOn.ToString("MM/dd/yyyy")
                }).FirstOrDefault();

                recieptAndPaymentModel.AmountPaid = AmountPaid.balance;

                var balance = InsuranceContext.Query("select  COALESCE(sum(Amount), 0) as balance, max(CreatedOn) as CreatedOn  from ReceiptAndPayment  where policyId = '" + invoice.PolicyId + "'").Select(x => new Balance
                {
                    balance = x.balance * -1,
                    CreatedOn = x.CreatedOn == null ? "--" : x.CreatedOn.ToString("MM/dd/yyyy"),
                    // Reference = x.reference == null ? "--" : x.reference,

                }).FirstOrDefault();

                recieptAndPaymentModel.Balance = balance.balance;
                recieptAndPaymentModel.ReceiptDate = AmountPaid.CreatedOn;
                recieptAndPaymentModel.PaymentReference = balance.Reference;



                var customerDetail = customerList.FirstOrDefault(c => c.Id == invoice.CreatedBy);

                if (customerDetail != null)
                    recieptAndPaymentModel.PolicyCreatedBy = customerDetail == null ? " " : customerDetail.FirstName + " " + customerDetail.LastName;

                string CustomerNameSql = "select Customer.FirstName+' '+Customer.LastName as userName , Currency.name as CurrrecyName from VehicleDetail join Customer on Customer.Id = VehicleDetail.CustomerId join Currency on VehicleDetail.CurrencyId = Currency.Id where VehicleDetail.PolicyId ='" + invoice.PolicyId + "'";
                var CustomerName = InsuranceContext.Query(CustomerNameSql).Select(x => new UserName
                {
                    UserNameValue = x.userName == null ? " " : x.userName,
                    PolicyCurrencyValue = x.CurrrecyName == null ? " " : x.CurrrecyName

                }).FirstOrDefault();

                recieptAndPaymentModel.CustomerName = CustomerName == null ? "" : CustomerName.UserNameValue;
                recieptAndPaymentModel.Currency = CustomerName == null ? "" : CustomerName.PolicyCurrencyValue;



                model.listInvoiceAndReciept.Add(recieptAndPaymentModel);
            }

            return View("NewReconciliationReport", model);
        }

        public ActionResult NewReconciliationReport()
        {

            NewReconcilationReportModel model = new NewReconcilationReportModel();
            string invoiceSQL = "select sum(Amount) as invoice, policyId , policyNumber , CreatedOn, CreatedBy from ReceiptAndPayment  where type='invoice' " +
                //"AND CreatedOn BETWEEN '"+model.FromDate+"' AND '"+model.EndDate+"' " +
                "group by  policyId,policyNumber , CreatedOn, CreatedBy ";



            var InvoiceList = InsuranceContext.Query(invoiceSQL).Select(x => new PolicyInvoice
            {
                PolicyId = x.policyId,
                InvoiceAmount = x.invoice * -1,
                PolicyNumber = x.policyNumber,
                CreateOn = x.CreatedOn,
                CreatedBy = x.CreatedBy

            });


            var customerList = InsuranceContext.Customers.All().ToList();


            model.listInvoiceAndReciept = new List<RecieptAndPaymentModel>();
            foreach (PolicyInvoice invoice in InvoiceList)
            {
                // need to add class : RecieptAndPaymentModel
                RecieptAndPaymentModel recieptAndPaymentModel = new RecieptAndPaymentModel();
                recieptAndPaymentModel.PremiumDue = invoice.InvoiceAmount;
                recieptAndPaymentModel.PolicyNumber = invoice.PolicyNumber;
                recieptAndPaymentModel.PolicyCreatedOn = invoice.CreateOn;
                recieptAndPaymentModel.TransactionDate = invoice.CreateOn;
                var AmountPaid = InsuranceContext.Query("select  COALESCE(sum(Amount), 0 )  as balance , max(CreatedOn) as CreatedOn from ReceiptAndPayment  where type='reciept' and  policyId='" + invoice.PolicyId + "' ").Select(x => new Balance
                {
                    balance = x.balance,
                    CreatedOn = x.CreatedOn == null ? "--" : x.CreatedOn.ToString("MM/dd/yyyy")
                }).FirstOrDefault();

                recieptAndPaymentModel.AmountPaid = AmountPaid.balance;

                var balance = InsuranceContext.Query("select  COALESCE(sum(Amount), 0) as balance, max(CreatedOn) as CreatedOn  from ReceiptAndPayment  where policyId = '" + invoice.PolicyId + "'").Select(x => new Balance
                {
                    balance = x.balance * -1,
                    CreatedOn = x.CreatedOn == null ? "--" : x.CreatedOn.ToString("MM/dd/yyyy")

                }).FirstOrDefault();

                recieptAndPaymentModel.Balance = balance.balance;
                recieptAndPaymentModel.ReceiptDate = AmountPaid.CreatedOn;

                //string agentNameSql = "select Customer.FirstName+' '+Customer.LastName as userName from PolicyDetail join Customer on Customer.CustomerId =PolicyDetail.CreatedBy where PolicyDetail.id ='" + invoice.PolicyId + "'";
                //var AgentName = InsuranceContext.Query(agentNameSql).Select(x => new UserName
                //{
                //    UserNameValue = x.userName

                //}).FirstOrDefault();

                var customerDetail = customerList.FirstOrDefault(c => c.Id == invoice.CreatedBy);

                if (customerDetail != null)
                    recieptAndPaymentModel.PolicyCreatedBy = customerDetail == null ? " " : customerDetail.FirstName + " " + customerDetail.LastName;


                string CustomerNameSql = "select Customer.FirstName+' '+Customer.LastName as userName , Currency.name as CurrrecyName from VehicleDetail join Customer on Customer.Id = VehicleDetail.CustomerId join Currency on VehicleDetail.CurrencyId = Currency.Id where VehicleDetail.PolicyId ='" + invoice.PolicyId + "'";
                var CustomerName = InsuranceContext.Query(CustomerNameSql).Select(x => new UserName
                {
                    UserNameValue = x.userName == null ? " " : x.userName,
                    PolicyCurrencyValue = x.CurrrecyName == null ? " " : x.CurrrecyName

                }).FirstOrDefault();

                recieptAndPaymentModel.CustomerName = CustomerName == null ? "" : CustomerName.UserNameValue;
                recieptAndPaymentModel.Currency = CustomerName == null ? "" : CustomerName.PolicyCurrencyValue;

                model.listInvoiceAndReciept.Add(recieptAndPaymentModel);
            }

            return View("NewReconciliationReport", model);
        }


        [Authorize(Roles = "Administrator,Reports,Finance,Team Leaders")]
        public ActionResult NewDailyReceiptsReport()
        {

            var customerList = InsuranceContext.Customers.All().ToList();
            NewReconcilationReportModel model = new NewReconcilationReportModel();
            string invoiceSQL = "select sum(Amount) as invoice, policyId , policyNumber , CreatedOn, CreatedBy from ReceiptAndPayment  where type='invoice' " +
                //"AND CreatedOn BETWEEN '"+model.FromDate+"' AND '"+model.EndDate+"' " +
                "group by  policyId,policyNumber , CreatedOn, CreatedBy ";

            var InvoiceList = InsuranceContext.Query(invoiceSQL).Select(x => new PolicyInvoice
            {
                PolicyId = x.policyId,
                InvoiceAmount = x.invoice * -1,
                PolicyNumber = x.policyNumber,
                CreateOn = x.CreatedOn,
                CreatedBy = x.CreatedBy

            });

            model.listInvoiceAndReciept = new List<RecieptAndPaymentModel>();
            foreach (PolicyInvoice invoice in InvoiceList)
            {

                RecieptAndPaymentModel recieptAndPaymentModel = new RecieptAndPaymentModel();
                recieptAndPaymentModel.PremiumDue = invoice.InvoiceAmount;
                recieptAndPaymentModel.PolicyNumber = invoice.PolicyNumber;
                recieptAndPaymentModel.PolicyCreatedOn = invoice.CreateOn;
                recieptAndPaymentModel.TransactionDate = invoice.CreateOn;
                var AmountPaid = InsuranceContext.Query("select  COALESCE(sum(Amount), 0 )  as balance , max(CreatedOn) as CreatedOn from ReceiptAndPayment  where type='reciept' and  policyId='" + invoice.PolicyId + "' ").Select(x => new Balance
                {
                    balance = x.balance,
                    CreatedOn = x.CreatedOn == null ? "--" : x.CreatedOn.ToString("MM/dd/yyyy")
                }).FirstOrDefault();

                recieptAndPaymentModel.AmountPaid = AmountPaid.balance;

                var balance = InsuranceContext.Query("select  COALESCE(sum(Amount), 0) as balance, max(CreatedOn) as CreatedOn  from ReceiptAndPayment  where policyId = '" + invoice.PolicyId + "'").Select(x => new Balance
                {
                    balance = x.balance * -1,
                    CreatedOn = x.CreatedOn == null ? "--" : x.CreatedOn.ToString("MM/dd/yyyy"),
                    // Reference = x.reference == null ? "--" : x.reference,

                }).FirstOrDefault();

                recieptAndPaymentModel.Balance = balance.balance;
                recieptAndPaymentModel.ReceiptDate = AmountPaid.CreatedOn;
                recieptAndPaymentModel.PaymentReference = "";

                //string agentNameSql = "select Customer.FirstName+' '+Customer.LastName as userName from PolicyDetail join Customer on Customer.Id =PolicyDetail.CreatedBy where PolicyDetail.id ='" + invoice.PolicyId + "'";
                //var AgentName = InsuranceContext.Query(agentNameSql).Select(x => new UserName
                //{
                //    UserNameValue = x.userName

                //}).FirstOrDefault();

                //recieptAndPaymentModel.PolicyCreatedBy = AgentName == null ? " " : AgentName.UserNameValue;

                var customerDetail = customerList.FirstOrDefault(c => c.Id == invoice.CreatedBy);

                if (customerDetail != null)
                    recieptAndPaymentModel.PolicyCreatedBy = customerDetail == null ? " " : customerDetail.FirstName + " " + customerDetail.LastName;



                string CustomerNameSql = "select Customer.FirstName+' '+Customer.LastName as userName , Currency.name as CurrrecyName from PolicyDetail  join Customer on Customer.Id = PolicyDetail.CustomerId  join Currency on PolicyDetail.CurrencyId = Currency.Id where PolicyDetail.id='" + invoice.PolicyId + "'";
                var CustomerName = InsuranceContext.Query(CustomerNameSql).Select(x => new UserName
                {
                    UserNameValue = x.userName == null ? " " : x.userName,
                    PolicyCurrencyValue = x.CurrrecyName == null ? " " : x.CurrrecyName

                }).FirstOrDefault();

                recieptAndPaymentModel.CustomerName = CustomerName == null ? "" : CustomerName.UserNameValue;
                recieptAndPaymentModel.Currency = CustomerName == null ? "" : CustomerName.PolicyCurrencyValue;

                if (recieptAndPaymentModel.AmountPaid > 0)
                {
                    model.listInvoiceAndReciept.Add(recieptAndPaymentModel);
                }
            }
            return View(model);
        }


        public ActionResult NewDailyReceiptsSearchReport(NewReconcilationReportModel model)
        {

            var customerList = InsuranceContext.Customers.All().ToList();
            //NewReconcilationReportModel model = new NewReconcilationReportModel();
            string invoiceSQL = "select sum(Amount) as invoice, policyId , policyNumber , CreatedOn, CreatedBy from ReceiptAndPayment  where type='invoice' " +
                "AND CreatedOn BETWEEN '" + model.FromDate + "' AND '" + model.EndDate + "' " +
                "group by  policyId,policyNumber , CreatedOn, CreatedBy ";

            var InvoiceList = InsuranceContext.Query(invoiceSQL).Select(x => new PolicyInvoice
            {


                PolicyId = x.policyId,
                InvoiceAmount = x.invoice * -1,
                PolicyNumber = x.policyNumber,
                CreateOn = x.CreatedOn,
                CreatedBy = x.CreatedBy

            });

            model.listInvoiceAndReciept = new List<RecieptAndPaymentModel>();
            foreach (PolicyInvoice invoice in InvoiceList)
            {

                RecieptAndPaymentModel recieptAndPaymentModel = new RecieptAndPaymentModel();
                recieptAndPaymentModel.PremiumDue = invoice.InvoiceAmount;
                recieptAndPaymentModel.PolicyNumber = invoice.PolicyNumber;
                recieptAndPaymentModel.PolicyCreatedOn = invoice.CreateOn;
                recieptAndPaymentModel.TransactionDate = invoice.CreateOn;
                var AmountPaid = InsuranceContext.Query("select  COALESCE(sum(Amount), 0 )  as balance , max(CreatedOn) as CreatedOn from ReceiptAndPayment  where type='reciept' and  policyId='" + invoice.PolicyId + "' ").Select(x => new Balance
                {
                    balance = x.balance,
                    CreatedOn = x.CreatedOn == null ? "--" : x.CreatedOn.ToString("MM/dd/yyyy")
                }).FirstOrDefault();

                recieptAndPaymentModel.AmountPaid = AmountPaid.balance;

                var balance = InsuranceContext.Query("select  COALESCE(sum(Amount), 0) as balance, max(CreatedOn) as CreatedOn , reference from ReceiptAndPayment  where policyId = '" + invoice.PolicyId + "' group by  reference").Select(x => new Balance
                {
                    balance = x.balance * -1,
                    CreatedOn = x.CreatedOn == null ? "--" : x.CreatedOn.ToString("MM/dd/yyyy"),
                    Reference = x.reference == null ? "--" : x.reference,

                }).FirstOrDefault();

                recieptAndPaymentModel.Balance = balance.balance;
                recieptAndPaymentModel.ReceiptDate = AmountPaid.CreatedOn;
                recieptAndPaymentModel.PaymentReference = balance.Reference;

                var customerDetail = customerList.FirstOrDefault(c => c.Id == invoice.CreatedBy);

                if (customerDetail != null)
                    recieptAndPaymentModel.PolicyCreatedBy = customerDetail == null ? " " : customerDetail.FirstName + " " + customerDetail.LastName;


                string CustomerNameSql = "select Customer.FirstName+' '+Customer.LastName as userName , Currency.name as CurrrecyName from PolicyDetail  join Customer on Customer.CustomerId = PolicyDetail.CustomerId  join Currency on PolicyDetail.CurrencyId = Currency.Id where PolicyDetail.id='" + invoice.PolicyId + "'";
                var CustomerName = InsuranceContext.Query(CustomerNameSql).Select(x => new UserName
                {
                    UserNameValue = x.userName == null ? " " : x.userName,
                    PolicyCurrencyValue = x.CurrrecyName == null ? " " : x.CurrrecyName

                }).FirstOrDefault();

                recieptAndPaymentModel.CustomerName = CustomerName == null ? "" : CustomerName.UserNameValue;
                recieptAndPaymentModel.Currency = CustomerName == null ? "" : CustomerName.PolicyCurrencyValue;

                if (recieptAndPaymentModel.Balance <= 0)
                {
                    model.listInvoiceAndReciept.Add(recieptAndPaymentModel);
                }
            }

            return View("NewDailyReceiptsReport", model);
        }


        public JsonResult getReceipt(int id)
        {
            string query = "select top 1 * from ReceiptAndPayment where Id='" + id + "'";

            //StringBuilder builder = new StringBuilder();
            //builder.Append("")
            var reciept = InsuranceContext.Query(query).Select(x => new ModelReceiptAndPayment
            {
                Id = x.Id,
                Amount = x.Amount,
                policyId = x.policyId,
                paymentMethod = x.paymentMethod,
                policyNumber = x.policyNumber,
                CreatedOn = x.CreatedOn.ToString("dd MMM yyyy"),
                reference = x.reference,
                currency = x.currency

            }).First();

            return Json(reciept, JsonRequestBehavior.AllowGet);
        }

        public JsonResult getPeriodCommissions()
        {
            ModelPeriodCommission model = new ModelPeriodCommission();
            model.UserCommissions = new List<ModelPeriodCommission.UserCommissionData>();
            string AgentsData = "SELECT Customer.id,[FirstName],[LastName],AspNetRoles.Name FROM[InsuranceClaim_dev].[dbo].[Customer] inner join AspNetUsers on Customer.UserID = AspNetUsers.Id inner join AspNetUserRoles on  AspNetUserRoles.UserId = AspNetUsers.Id inner join AspNetRoles on AspNetRoles.Id = AspNetUserRoles.RoleId where AspNetRoles.Name = 'Staff'";
            var CustomerName = InsuranceContext.Query(AgentsData).Select(x => new ClientData
            {
                id = x.id,
                AgentName = x.FirstName + " " + x.LastName
            }).ToList();

            string periods = "SELECT StartPeriod,EndPeriod FROM CommissionPeriods where PeriodStatus = 'active'";
            var commissionPeriods = InsuranceContext.Query(periods).Select(x => new Insurance.Domain.CommissionPeriod
            {
                StartPeriod = x.StartPeriod,
                EndPeriod = x.EndPeriod
            }).Single();

            foreach (ClientData agent in CustomerName)
            {
                // model.UserCommissions = new List<ModelPeriodCommission.UserCommissionData>();

                model.UserCommissions.Add(
                    new ModelPeriodCommission.UserCommissionData
                    {
                        CreatedById = agent.id,
                        CreatedByName = agent.AgentName,

                        PoliciesTotal = InsuranceContext.Query("SELECT Count(*) as PoliciesTotal FROM [InsuranceClaim_dev].[dbo].[SummaryDetail] where createdby='" + agent.id + "' and  CONVERT(DATE ,SummaryDetail.CreatedOn)>=CONVERT(DATE ,'" + commissionPeriods.StartPeriod + "') ").Select(x => new Total
                        {
                            TotalNo = x.PoliciesTotal
                        }).Single().TotalNo,

                        paidPoliciesNo = InsuranceContext.Query("SELECT count(*) as PoliciesTotal FROM SummaryDetail inner join ReceiptModuleHistory on ReceiptModuleHistory.SummaryDetailId = SummaryDetail.id where SummaryDetail.createdby ='" + agent.id + "' and  CONVERT(DATE ,SummaryDetail.CreatedOn)>=CONVERT(DATE ,'" + commissionPeriods.StartPeriod + "') ").Select(x => new Total
                        {
                            TotalNo = x.PoliciesTotal

                        }).Single().TotalNo,

                        UnpaidPoliciesNo = (InsuranceContext.Query("SELECT Count(*) as PoliciesTotal FROM [InsuranceClaim_dev].[dbo].[SummaryDetail] where createdby='" + agent.id + "' and  CONVERT(DATE ,SummaryDetail.CreatedOn)>=CONVERT(DATE ,'" + commissionPeriods.StartPeriod + "') ").Select(x => new Total
                        {
                            TotalNo = x.PoliciesTotal
                        }).Single().TotalNo - InsuranceContext.Query("SELECT count(*) as PoliciesTotal FROM SummaryDetail inner join ReceiptModuleHistory on ReceiptModuleHistory.SummaryDetailId = SummaryDetail.id where SummaryDetail.createdby ='" + agent.id + "' and  CONVERT(DATE ,SummaryDetail.CreatedOn)>=CONVERT(DATE ,'" + commissionPeriods.StartPeriod + "') ").Select(x => new Total
                        {
                            TotalNo = x.PoliciesTotal

                        }).Single().TotalNo),

                        PremiumUnpaid = InsuranceContext.Query("SELECT Count(*) as PoliciesTotal FROM [InsuranceClaim_dev].[dbo].[SummaryDetail] where createdby='" + agent.id + "' and  CONVERT(DATE ,SummaryDetail.CreatedOn)>=CONVERT(DATE ,'" + commissionPeriods.StartPeriod + "') ").Select(x => new Total
                        {
                            TotalNo = x.PoliciesTotal
                        }).Single().TotalNo
                        ,
                        PremiumPaid = InsuranceContext.Query("SELECT Count(*) as PoliciesTotal FROM [InsuranceClaim_dev].[dbo].[SummaryDetail] where createdby='" + agent.id + "' and  CONVERT(DATE ,SummaryDetail.CreatedOn)>=CONVERT(DATE ,'" + commissionPeriods.StartPeriod + "') ").Select(x => new Total
                        {
                            TotalNo = x.PoliciesTotal
                        }).Single().TotalNo,

                        CommissionOfPaid = InsuranceContext.Query("SELECT COALESCE(sum([SummaryDetail].TotalPremium),0)  as AmountTotal FROM SummaryDetail inner join ReceiptModuleHistory on ReceiptModuleHistory.SummaryDetailId = SummaryDetail.id where SummaryDetail.createdby ='" + agent.id + "' and  CONVERT(DATE ,SummaryDetail.CreatedOn)>=CONVERT(DATE ,'" + commissionPeriods.StartPeriod + "') ").Select(x => new TotalAmount
                        {
                            AmountTotal = Decimal.ToDouble(x.AmountTotal)

                        }).Single().AmountTotal,

                        CommissionOfUnpaid = (InsuranceContext.Query("SELECT COALESCE(sum([SummaryDetail].TotalPremium),0)  as AmountTotal FROM SummaryDetail inner join ReceiptModuleHistory on ReceiptModuleHistory.SummaryDetailId = SummaryDetail.id where SummaryDetail.createdby ='" + agent.id + "' and  CONVERT(DATE ,SummaryDetail.CreatedOn)>=CONVERT(DATE ,'" + commissionPeriods.StartPeriod + "') ").Select(x => new TotalAmount
                        {
                            AmountTotal = Decimal.ToDouble(x.AmountTotal)

                        }).Single().AmountTotal - InsuranceContext.Query("SELECT COALESCE(sum([SummaryDetail].TotalPremium),0)  as AmountTotal FROM SummaryDetail inner join ReceiptModuleHistory on ReceiptModuleHistory.SummaryDetailId = SummaryDetail.id where SummaryDetail.createdby ='" + agent.id + "' and  CONVERT(DATE ,SummaryDetail.CreatedOn)>=CONVERT(DATE ,'" + commissionPeriods.StartPeriod + "') ").Select(x => new TotalAmount
                        {
                            AmountTotal = Decimal.ToDouble(x.AmountTotal)

                        }).Single().AmountTotal),

                    });
            }
            return Json(model.UserCommissions, JsonRequestBehavior.AllowGet);
        }

        public ActionResult PeriodCommissionsReport()
        {

            ModelPeriodCommission model = new ModelPeriodCommission();
            model.UserCommissions = new List<ModelPeriodCommission.UserCommissionData>();
            string AgentsData = "SELECT Customer.id,[FirstName],[LastName],AspNetRoles.Name FROM[InsuranceClaim_dev].[dbo].[Customer] inner join AspNetUsers on Customer.UserID = AspNetUsers.Id inner join AspNetUserRoles on  AspNetUserRoles.UserId = AspNetUsers.Id inner join AspNetRoles on AspNetRoles.Id = AspNetUserRoles.RoleId where AspNetRoles.Name = 'Staff'";
            var CustomerName = InsuranceContext.Query(AgentsData).Select(x => new ClientData
            {
                id = x.id,
                AgentName = x.FirstName + " " + x.LastName
            }).ToList();


            string periods = "SELECT StartPeriod,EndPeriod FROM CommissionPeriods where PeriodStatus = 'active'";
            var commissionPeriods = InsuranceContext.Query(periods).Select(x => new Models.CommissionPeriod
            {
                StartPeriod = x.StartPeriod,
                EndPeriod = x.EndPeriod

            }).FirstOrDefault();

            model.UserCommissions = new List<ModelPeriodCommission.UserCommissionData>();

            foreach (ClientData agent in CustomerName)
            {
                var userComissionData = new ModelPeriodCommission.UserCommissionData();

                userComissionData.CreatedById = agent.id;
                userComissionData.CreatedByName = agent.AgentName;

                userComissionData.PoliciesTotal = InsuranceContext.Query("SELECT Count(*) as PoliciesTotal FROM [InsuranceClaim_dev].[dbo].[SummaryDetail] where createdby='" + agent.id + "' and  CONVERT(DATE ,SummaryDetail.CreatedOn)>=CONVERT(DATE ,'" + commissionPeriods.StartPeriod + "') ").Select(x => new Total
                {
                    TotalNo = x.PoliciesTotal
                }).Single().TotalNo;

                userComissionData.paidPoliciesNo = InsuranceContext.Query("SELECT count(*) as PoliciesTotal FROM SummaryDetail inner join ReceiptModuleHistory on ReceiptModuleHistory.SummaryDetailId = SummaryDetail.id where SummaryDetail.createdby ='" + agent.id + "' and  CONVERT(DATE ,SummaryDetail.CreatedOn)>=CONVERT(DATE ,'" + commissionPeriods.StartPeriod + "') ").Select(x => new Total
                {
                    TotalNo = x.PoliciesTotal

                }).Single().TotalNo;

                userComissionData.UnpaidPoliciesNo = (InsuranceContext.Query("SELECT Count(*) as PoliciesTotal FROM [InsuranceClaim_dev].[dbo].[SummaryDetail] where createdby='" + agent.id + "' and  CONVERT(DATE ,SummaryDetail.CreatedOn)>=CONVERT(DATE ,'" + commissionPeriods.StartPeriod + "') ").Select(x => new Total
                {
                    TotalNo = x.PoliciesTotal
                }).Single().TotalNo - InsuranceContext.Query("SELECT count(*) as PoliciesTotal FROM SummaryDetail inner join ReceiptModuleHistory on ReceiptModuleHistory.SummaryDetailId = SummaryDetail.id where SummaryDetail.createdby ='" + agent.id + "' and  CONVERT(DATE ,SummaryDetail.CreatedOn)>=CONVERT(DATE ,'" + commissionPeriods.StartPeriod + "') ").Select(x => new Total
                {
                    TotalNo = x.PoliciesTotal

                }).Single().TotalNo);

                userComissionData.PremiumUnpaid = InsuranceContext.Query("SELECT Count(*) as PoliciesTotal FROM [InsuranceClaim_dev].[dbo].[SummaryDetail] where createdby='" + agent.id + "' and  CONVERT(DATE ,SummaryDetail.CreatedOn)>=CONVERT(DATE ,'" + commissionPeriods.StartPeriod + "') ").Select(x => new Total
                {
                    TotalNo = x.PoliciesTotal
                }).Single().TotalNo
                        ;
                userComissionData.PremiumPaid = InsuranceContext.Query("SELECT Count(*) as PoliciesTotal FROM [InsuranceClaim_dev].[dbo].[SummaryDetail] where createdby='" + agent.id + "' and  CONVERT(DATE ,SummaryDetail.CreatedOn)>=CONVERT(DATE ,'" + commissionPeriods.StartPeriod + "') ").Select(x => new Total
                {
                    TotalNo = x.PoliciesTotal
                }).Single().TotalNo;

                userComissionData.CommissionOfPaid = InsuranceContext.Query("SELECT COALESCE(sum([SummaryDetail].TotalPremium),0)  as AmountTotal FROM SummaryDetail inner join ReceiptModuleHistory on ReceiptModuleHistory.SummaryDetailId = SummaryDetail.id where SummaryDetail.createdby ='" + agent.id + "' and  CONVERT(DATE ,SummaryDetail.CreatedOn)>=CONVERT(DATE ,'" + commissionPeriods.StartPeriod + "') ").Select(x => new TotalAmount
                {
                    AmountTotal = Decimal.ToDouble(x.AmountTotal)

                }).Single().AmountTotal;

                userComissionData.CommissionOfUnpaid = (InsuranceContext.Query("SELECT COALESCE(sum([SummaryDetail].TotalPremium),0)  as AmountTotal FROM SummaryDetail inner join ReceiptModuleHistory on ReceiptModuleHistory.SummaryDetailId = SummaryDetail.id where SummaryDetail.createdby ='" + agent.id + "' and  CONVERT(DATE ,SummaryDetail.CreatedOn)>=CONVERT(DATE ,'" + commissionPeriods.StartPeriod + "') ").Select(x => new TotalAmount
                {
                    AmountTotal = Decimal.ToDouble(x.AmountTotal)

                }).Single().AmountTotal - InsuranceContext.Query("SELECT COALESCE(sum([SummaryDetail].TotalPremium),0)  as AmountTotal FROM SummaryDetail inner join ReceiptModuleHistory on ReceiptModuleHistory.SummaryDetailId = SummaryDetail.id where SummaryDetail.createdby ='" + agent.id + "' and  CONVERT(DATE ,SummaryDetail.CreatedOn)>=CONVERT(DATE ,'" + commissionPeriods.StartPeriod + "') ").Select(x => new TotalAmount
                {
                    AmountTotal = Decimal.ToDouble(x.AmountTotal)

                }).Single().AmountTotal);

                model.UserCommissions.Add(userComissionData);
            }
            return View(model);
        }

        [Authorize(Roles = "Zimnata,Administrator")]
        public ActionResult AccountsGrossWrittenPremuimReport()
        {

            List<GrossWrittenPremiumReportModels> ListGrossWrittenPremiumReport = new List<GrossWrittenPremiumReportModels>();
            ListGrossWrittenPremiumReportModels _ListGrossWrittenPremiumReport = new ListGrossWrittenPremiumReportModels();
            _ListGrossWrittenPremiumReport.ListGrossWrittenPremiumReportdata = new List<GrossWrittenPremiumReportModels>();

            GrossWrittenPremiumReportSearchModels Model = new GrossWrittenPremiumReportSearchModels();

            var branchList = InsuranceContext.Branches.All();
            var paymentInformationsList = InsuranceContext.PaymentInformations.All();

            ViewBag.ReportList = ReportsList();
            var service = new VehicleService();
            ViewBag.CoverType = service.GetCoverType();
            //ViewBag.PaymentMethod = PaymentMethodsList();

            ViewBag.Currencies = InsuranceContext.Currencies.All(where: $"IsActive = 'True'");


            var recieptList = InsuranceContext.ReceiptHistorys.All().ToList();

            // Customer.ALMId is null replace condition with SummaryDetail.CreatedBy is not null
            var query = " select top 500 SummaryDetail.PaymentMethodId, PolicyDetail.Id as policyId, PolicyDetail.PolicyNumber as Policy_Number, Customer.ALMId, case when VehicleDetail.ALMBranchId = 0  then  [dbo].fn_GetUserCallCenterAgent(SummaryDetail.CreatedBy) else [dbo].fn_GetUserALM(VehicleDetail.ALMBranchId) end  as PolicyCreatedBy, Customer.FirstName + ' ' + Customer.LastName as Customer_Name,VehicleDetail.TransactionDate as Transaction_date, ";
            query += "  case when Customer.id=SummaryDetail.CreatedBy then [dbo].fn_GetUserBranch(Customer.id) else [dbo].fn_GetUserBranch(SummaryDetail.CreatedBy) end as BranchName, ";
            query += " VehicleDetail.CoverNote as CoverNoteNum,VehicleDetail.AdministrationAmt, RegistrationNo, VehicleDetail.CoverStartDate, VehicleDetail.CoverEndDate, PaymentMethod.Name as Payment_Mode, PaymentTerm.Name as Payment_Term,CoverType.Name as CoverType, Currency.Name as Currency, ";
            query += " VehicleDetail.Premium  as Premium_due, VehicleDetail.StampDuty as Stamp_duty, VehicleDetail.ZTSCLevy as ZTSC_Levy,ChasisNumber,EngineNumber, ";
            query += " cast(VehicleDetail.Premium * 30 / 100 as decimal(10, 2))    as Comission_Amount, VehicleDetail.IncludeRadioLicenseCost, ";
            query += " CASE WHEN IncludeRadioLicenseCost = 1 THEN VehicleDetail.RadioLicenseCost else 0 end as RadioLicenseCost, VehicleDetail.VehicleLicenceFee as Zinara_License_Fee, ";
            query += " VehicleDetail.RenewalDate as PolicyRenewalDate, VehicleDetail.IsActive, VehicleDetail.RenewPolicyNumber as RenewPolicyNumber, ";
            query += " VehicleDetail.BusinessSourceDetailId,SummaryDetail.id as SummaryDetailId, BusinessSource.Source as BusinessSourceName, SourceDetail.FirstName + ' ' + SourceDetail.LastName as SourceDetailName, VehicleDetail.SumInsured, VehicleMake.MakeDescription , VehicleModel.ModelDescription from PolicyDetail ";
            query += " join Customer on PolicyDetail.CustomerId = Customer.Id ";
            query += "  join VehicleDetail on PolicyDetail.Id = VehicleDetail.PolicyId ";
            query += "join SummaryVehicleDetail on VehicleDetail.id = SummaryVehicleDetail.VehicleDetailsId ";
            query += " join SummaryDetail on SummaryDetail.id = SummaryVehicleDetail.SummaryDetailId ";
            // query += "  join PaymentInformation on SummaryDetail.Id=PaymentInformation.SummaryDetailId ";
            query += "  left join VehicleMake on  VehicleDetail.MakeId = VehicleMake.MakeCode  left join VehicleModel on VehicleDetail.ModelId = VehicleModel.ModelCode";
            query += " left join PaymentMethod on SummaryDetail.PaymentMethodId = PaymentMethod.Id ";
            query += "join PaymentTerm on VehicleDetail.PaymentTermId = PaymentTerm.Id ";
            query += " left join CoverType on VehicleDetail.CoverTypeId = CoverType.Id ";
            query += " left join Currency on VehicleDetail.CurrencyId = Currency.Id ";
            query += " left join BusinessSource on BusinessSource.Id = VehicleDetail.BusinessSourceDetailId ";
            query += " left   join SourceDetail on VehicleDetail.BusinessSourceDetailId = SourceDetail.Id join AspNetUsers on AspNetUsers.id=customer.UserID join AspNetUserRoles on AspNetUserRoles.UserId=AspNetUsers.Id ";
            query += " where (VehicleDetail.IsActive = 1 or VehicleDetail.IsActive = null) and SummaryDetail.isQuotation=0   order by  VehicleDetail.Id desc ";


            ListGrossWrittenPremiumReport = InsuranceContext.Query(query).
                Select(x => new GrossWrittenPremiumReportModels()
                {
                    PaymentMethodId = x.PaymentMethodId,
                    Policy_Number = x.Policy_Number,
                    BranchName = x.BranchName,
                    RegistrationNo = x.RegistrationNo,
                    MakeDescription = x.MakeDescription,
                    ModelDescription = x.ModelDescription,
                    PolicyCreatedBy = x.PolicyCreatedBy,
                    Customer_Name = x.Customer_Name,
                    Transaction_date = x.Transaction_date.ToShortDateString(),
                    CoverNoteNum = x.CoverNoteNum,
                    Payment_Mode = x.Payment_Mode == null ? "Cash" : x.Payment_Mode,
                    Payment_Term = x.Payment_Term,
                    CoverType = x.CoverType,
                    Currency = x.Currency,
                    Premium_due = x.Premium_due == null ? 0 : x.Premium_due,
                    //GrandPremium = x.Premium_due + x.Zinara_License_Fee +
                    Stamp_duty = x.Stamp_duty == null ? 0 : x.Stamp_duty,
                    ZTSC_Levy = x.ZTSC_Levy == null ? 0 : x.ZTSC_Levy,
                    ALMId = x.ALMId,
                    Comission_Amount = x.Comission_Amount == null ? 0 : x.Comission_Amount,
                    //IncludeRadioLicenseCost = x.IncludeRadioLicenseCost,
                    RadioLicenseCost = x.RadioLicenseCost == null ? 0 : x.RadioLicenseCost,
                    Zinara_License_Fee = x.Zinara_License_Fee == null ? 0 : x.Zinara_License_Fee,
                    PolicyRenewalDate = x.PolicyRenewalDate,
                    IsActive = x.IsActive,
                    RenewPolicyNumber = x.RenewPolicyNumber,
                    BusinessSourceName = x.BusinessSourceName,
                    CoverStartDate = x.CoverStartDate == null ? DateTime.MinValue.ToShortDateString() : Convert.ToDateTime(x.CoverStartDate).ToShortDateString(),
                    CoverEndDate = x.CoverEndDate == null ? DateTime.MinValue.ToShortDateString() : Convert.ToDateTime(x.CoverEndDate).ToShortDateString(),
                    SourceDetailName = x.SourceDetailName,
                    SummaryDetailId = x.SummaryDetailId,
                    Sum_Insured = x.SumInsured == null ? 0 : Convert.ToDecimal(x.SumInsured),
                    AdministrationAmt = x.AdministrationAmt == null ? 0 : Convert.ToDecimal(x.AdministrationAmt),
                    ReceiptNumber = RecieptNumber(x.policyId, x.RenewPolicyNumber, recieptList),
                    ChasisNumber = x.ChasisNumber,
                    EngineNumber = x.EngineNumber
                }).ToList();


            List<GrossWrittenPremiumReportModels> list = new List<GrossWrittenPremiumReportModels>();

            foreach (var item in ListGrossWrittenPremiumReport)
            {
                var paymentDetail = paymentInformationsList.Where(c => c.SummaryDetailId == item.SummaryDetailId);
                if (paymentDetail == null)
                    continue;

                GrossWrittenPremiumReportModels model = new GrossWrittenPremiumReportModels();
                model.Policy_Number = item.Policy_Number;
                model.BranchName = item.BranchName;
                model.PolicyCreatedBy = item.PolicyCreatedBy;
                model.Customer_Name = item.Customer_Name;
                model.RegistrationNo = item.RegistrationNo;
                model.MakeDescription = item.MakeDescription;
                model.ModelDescription = item.ModelDescription;
                model.Transaction_date = item.Transaction_date;
                model.CoverNoteNum = item.CoverNoteNum;
                model.Payment_Mode = item.Payment_Mode;
                model.Payment_Term = item.Payment_Term;
                model.CoverType = item.CoverType;
                model.Currency = item.Currency;
                model.RenewPolicyNumber = item.RenewPolicyNumber;
                model.ALMId = item.ALMId;
                model.IsActive = item.IsActive;
                model.PolicyRenewalDate = item.PolicyRenewalDate;
                model.SourceDetailName = item.SourceDetailName;
                model.BusinessSourceName = item.BusinessSourceName;
                model.PaymentMethodId = item.PaymentMethodId;
                model.ReceiptNumber = item.ReceiptNumber;
                model.AdministrationAmt = item.AdministrationAmt;
                model.CoverStartDate = item.CoverStartDate;
                model.CoverEndDate = item.CoverEndDate;
                model.ChasisNumber = item.ChasisNumber;
                model.EngineNumber = item.EngineNumber;

                //IncludeRadioLicenseCost = x.IncludeRadioLicenseCost,

                var index = list.FindIndex(c => c.Policy_Number == item.Policy_Number);
                if (index != -1)
                {
                    list[index].Premium_due += item.Premium_due;
                    list[index].GrandPremium = item.Premium_due + item.Zinara_License_Fee.Value + item.RadioLicenseCost.Value + item.ZTSC_Levy + item.Stamp_duty;
                    list[index].Stamp_duty += item.Stamp_duty;
                    list[index].ZTSC_Levy += item.ZTSC_Levy;
                    list[index].Comission_Amount += item.Comission_Amount;
                    //IncludeRadioLicenseCost = x.IncludeRadioLicenseCost,
                    list[index].RadioLicenseCost += item.RadioLicenseCost;
                    list[index].Zinara_License_Fee += item.Zinara_License_Fee;
                    list[index].Sum_Insured += item.Sum_Insured;
                }
                else
                {
                    model.Premium_due = item.Premium_due;
                    model.GrandPremium = item.Premium_due + item.Zinara_License_Fee.Value + item.RadioLicenseCost.Value + item.ZTSC_Levy + item.Stamp_duty;
                    model.Stamp_duty = item.Stamp_duty;
                    model.ZTSC_Levy = item.ZTSC_Levy;
                    model.Comission_Amount = item.Comission_Amount;
                    //IncludeRadioLicenseCost = x.IncludeRadioLicenseCost,
                    model.RadioLicenseCost = item.RadioLicenseCost;
                    model.Zinara_License_Fee = item.Zinara_License_Fee;
                    model.Sum_Insured = item.Sum_Insured;
                    list.Add(model);
                }
            }



            Model.ListGrossWrittenPremiumReportdata = list.OrderByDescending(p => p.Id).ToList();
            var endorsmentList = GetGWPEndorsmentReport(new GrossWrittenPremiumReportSearchModels());
            Model.ListGrossWrittenPremiumReportdata.AddRange(endorsmentList);


            return View(Model);
        }

        [Authorize(Roles = "Zimnata,Administrator")]
        public ActionResult SearhAccountsGrossWrittenPremuimReport(GrossWrittenPremiumReportSearchModels _model)
        {

            List<GrossWrittenPremiumReportModels> ListGrossWrittenPremiumReport = new List<GrossWrittenPremiumReportModels>();
            ListGrossWrittenPremiumReportModels _ListGrossWrittenPremiumReport = new ListGrossWrittenPremiumReportModels();
            _ListGrossWrittenPremiumReport.ListGrossWrittenPremiumReportdata = new List<GrossWrittenPremiumReportModels>();
            var paymentInformationsList = InsuranceContext.PaymentInformations.All();

            ViewBag.ReportList = ReportsList();
            // ViewBag.PaymentMethod = PaymentMethodsList();
            ViewBag.Currencies = InsuranceContext.Currencies.All(where: $"IsActive = 'True'");
            var recieptList = InsuranceContext.ReceiptHistorys.All().ToList();

            var service = new VehicleService();
            ViewBag.CoverType = service.GetCoverType();

            var query = " select SummaryDetail.PaymentMethodId, PolicyDetail.PolicyNumber as Policy_Number,PolicyDetail.Id as policyId, Customer.ALMId, case when VehicleDetail.ALMBranchId = 0  then  [dbo].fn_GetUserCallCenterAgent(SummaryDetail.CreatedBy) else [dbo].fn_GetUserALM(VehicleDetail.ALMBranchId) end  as PolicyCreatedBy, Customer.FirstName + ' ' + Customer.LastName as Customer_Name,VehicleDetail.TransactionDate as Transaction_date, ";
            query += "  case when Customer.id=SummaryDetail.CreatedBy then [dbo].fn_GetUserBranch(Customer.id) else [dbo].fn_GetUserBranch(SummaryDetail.CreatedBy) end as BranchName, ";
            query += " VehicleDetail.CoverNote as CoverNoteNum, PaymentMethod.Name as Payment_Mode, PaymentTerm.Name as Payment_Term,CoverType.Name as CoverType, Currency.Name as Currency, ";
            query += " VehicleDetail.Premium  as Premium_due, VehicleDetail.StampDuty as Stamp_duty, VehicleDetail.ZTSCLevy as ZTSC_Levy, VehicleDetail.AdministrationAmt, ";
            query += " cast(VehicleDetail.Premium * 30 / 100 as decimal(10, 2))    as Comission_Amount, VehicleDetail.IncludeRadioLicenseCost, ";
            query += " CASE WHEN IncludeRadioLicenseCost = 1 THEN VehicleDetail.RadioLicenseCost else 0 end as RadioLicenseCost, VehicleDetail.VehicleLicenceFee as Zinara_License_Fee, ChasisNumber, EngineNumber,";
            query += " VehicleDetail.RenewalDate as PolicyRenewalDate, RegistrationNo, VehicleDetail.IsActive, VehicleDetail.CoverStartDate, VehicleDetail.CoverEndDate, VehicleDetail.RenewPolicyNumber as RenewPolicyNumber, ";
            query += " VehicleDetail.BusinessSourceDetailId, SummaryDetail.id as SummaryDetailId, BusinessSource.Source as BusinessSourceName, SourceDetail.FirstName + ' ' + SourceDetail.LastName as SourceDetailName, VehicleDetail.SumInsured, VehicleMake.MakeDescription , VehicleModel.ModelDescription  from PolicyDetail ";
            query += " join Customer on PolicyDetail.CustomerId = Customer.Id ";
            query += " join VehicleDetail on PolicyDetail.Id = VehicleDetail.PolicyId ";
            query += "join SummaryVehicleDetail on VehicleDetail.id = SummaryVehicleDetail.VehicleDetailsId ";
            query += " join SummaryDetail on SummaryDetail.id = SummaryVehicleDetail.SummaryDetailId ";
            //query += "  join PaymentInformation on SummaryDetail.Id=PaymentInformation.SummaryDetailId ";
            query += "  left join VehicleMake on  VehicleDetail.MakeId = VehicleMake.MakeCode  left join VehicleModel on VehicleDetail.ModelId = VehicleModel.ModelCode";
            query += " left join PaymentMethod on SummaryDetail.PaymentMethodId = PaymentMethod.Id ";
            query += "join PaymentTerm on VehicleDetail.PaymentTermId = PaymentTerm.Id ";
            query += " left join CoverType on VehicleDetail.CoverTypeId = CoverType.Id ";
            query += " left join Currency on VehicleDetail.CurrencyId = Currency.Id ";
            query += " left join BusinessSource on BusinessSource.Id = VehicleDetail.BusinessSourceDetailId ";
            query += " left   join SourceDetail on VehicleDetail.BusinessSourceDetailId = SourceDetail.Id join AspNetUsers on AspNetUsers.id=customer.UserID join AspNetUserRoles on AspNetUserRoles.UserId=AspNetUsers.Id ";
            query += " where (VehicleDetail.IsActive = 1 or VehicleDetail.IsActive = null) and SummaryDetail.isQuotation=0  and (  CONVERT(date, VehicleDetail.TransactionDate) >= convert(date, '" + _model.FormDate + "', 101)  and CONVERT(date, VehicleDetail.TransactionDate) <= convert(date, '" + _model.EndDate + "', 101))  ";

            if (_model.ReportTypeId == (int)ReportTypeEnum.ALM)
                query += "and ALMBranchId <>0";

            if (_model.PaymentStatus != null)
                query += " and SummaryDetail.PaymentMethodId= " + _model.PaymentStatus.ToString() + " ";

            //if (_model.ReportTypeId == (int)ReportTypeEnum.CallCenter)
            //    query += "and Customer.BranchId=" + (int)ALMBranch.GeneCallCentre;

            if (_model.CurrencyId != null)
                query += " and VehicleDetail.CurrencyId= " + _model.CurrencyId + " ";

            if (_model.CoverTypeId != null)
                query += " and VehicleDetail.CoverTypeId= " + _model.CoverTypeId + " ";

            query += " order by  VehicleDetail.Id desc ";


            ListGrossWrittenPremiumReport = InsuranceContext.Query(query).
                Select(x => new GrossWrittenPremiumReportModels()
                {
                    PaymentMethodId = x.PaymentMethodId,
                    Policy_Number = x.Policy_Number,
                    BranchName = x.BranchName,
                    RegistrationNo = x.RegistrationNo,
                    MakeDescription = x.MakeDescription,
                    ModelDescription = x.ModelDescription,
                    PolicyCreatedBy = x.PolicyCreatedBy,
                    Customer_Name = x.Customer_Name,
                    Transaction_date = x.Transaction_date.ToShortDateString(),
                    CoverNoteNum = x.CoverNoteNum,
                    Payment_Mode = x.Payment_Mode,
                    Payment_Term = x.Payment_Term,
                    CoverType = x.CoverType,
                    Currency = x.Currency,
                    Premium_due = x.Premium_due == null ? 0 : x.Premium_due,
                    Stamp_duty = x.Stamp_duty == null ? 0 : x.Stamp_duty,
                    ZTSC_Levy = x.ZTSC_Levy == null ? 0 : x.ZTSC_Levy,
                    ALMId = x.ALMId,
                    CoverStartDate = x.CoverStartDate == null ? DateTime.MinValue.ToShortDateString() : Convert.ToDateTime(x.CoverStartDate).ToShortDateString(),
                    CoverEndDate = x.CoverEndDate == null ? DateTime.MinValue.ToShortDateString() : Convert.ToDateTime(x.CoverEndDate).ToShortDateString(),

                    Comission_Amount = x.Comission_Amount == null ? 0 : x.Comission_Amount,
                    //IncludeRadioLicenseCost = x.IncludeRadioLicenseCost,
                    RadioLicenseCost = x.RadioLicenseCost == null ? 0 : x.RadioLicenseCost,
                    Zinara_License_Fee = x.Zinara_License_Fee == null ? 0 : x.Zinara_License_Fee,
                    PolicyRenewalDate = x.PolicyRenewalDate,
                    IsActive = x.IsActive,
                    RenewPolicyNumber = x.RenewPolicyNumber,
                    BusinessSourceName = x.BusinessSourceName,
                    //IncludeRadioLicenseCost = x.IncludeRadioLicenseCost,
                    SourceDetailName = x.SourceDetailName,
                    SummaryDetailId = x.SummaryDetailId,
                    Sum_Insured = x.SumInsured == null ? 0 : Convert.ToDecimal(x.SumInsured),
                    AdministrationAmt = x.AdministrationAmt,
                    ReceiptNumber = RecieptNumber(x.policyId, x.RenewPolicyNumber, recieptList),
                    EngineNumber = x.EngineNumber,
                    ChasisNumber = x.ChasisNumber
                }).ToList();

            if (_model.ReportTypeId == (int)ReportTypeEnum.CallCenter)
                ListGrossWrittenPremiumReport = ListGrossWrittenPremiumReport.Where(c => c.BranchName == CallCenterBranch).ToList();




            List<GrossWrittenPremiumReportModels> list = new List<GrossWrittenPremiumReportModels>();

            foreach (var item in ListGrossWrittenPremiumReport)
            {
                var paymentDetail = paymentInformationsList.Where(c => c.SummaryDetailId == item.SummaryDetailId);
                if (paymentDetail == null)
                    continue;

                GrossWrittenPremiumReportModels model = new GrossWrittenPremiumReportModels();

                model.Policy_Number = item.Policy_Number;
                model.BranchName = item.BranchName;
                model.PolicyCreatedBy = item.PolicyCreatedBy;
                model.RegistrationNo = item.RegistrationNo;
                model.MakeDescription = item.MakeDescription;
                model.ModelDescription = item.ModelDescription;
                model.Customer_Name = item.Customer_Name;
                model.Transaction_date = item.Transaction_date;
                model.CoverNoteNum = item.CoverNoteNum;
                model.Payment_Mode = item.Payment_Mode;
                model.Payment_Term = item.Payment_Term;
                model.CoverType = item.CoverType;
                model.Currency = item.Currency;
                model.RenewPolicyNumber = item.RenewPolicyNumber;
                model.ALMId = item.ALMId;
                model.IsActive = item.IsActive;
                model.PolicyRenewalDate = item.PolicyRenewalDate;
                model.SourceDetailName = item.SourceDetailName;
                model.BusinessSourceName = item.BusinessSourceName;
                model.PaymentMethodId = item.PaymentMethodId;
                model.ReceiptNumber = item.ReceiptNumber;
                model.AdministrationAmt = item.AdministrationAmt;
                model.CoverStartDate = item.CoverStartDate;
                model.CoverEndDate = item.CoverEndDate;
                model.ChasisNumber = item.ChasisNumber;
                model.EngineNumber = item.EngineNumber;
                //IncludeRadioLicenseCost = x.IncludeRadioLicenseCost,

                var index = list.FindIndex(c => c.Policy_Number == item.Policy_Number);
                if (index != -1)
                {
                    list[index].Premium_due += item.Premium_due;
                    list[index].Stamp_duty += item.Stamp_duty;
                    list[index].ZTSC_Levy += item.ZTSC_Levy;
                    list[index].Comission_Amount += item.Comission_Amount;
                    //IncludeRadioLicenseCost = x.IncludeRadioLicenseCost,
                    list[index].RadioLicenseCost += item.RadioLicenseCost;
                    list[index].Zinara_License_Fee += item.Zinara_License_Fee;
                    list[index].Sum_Insured += item.Sum_Insured;
                    list[index].GrandPremium = item.Premium_due + item.Zinara_License_Fee.Value + item.RadioLicenseCost.Value + item.Stamp_duty + item.ZTSC_Levy;
                }
                else
                {
                    model.Premium_due = item.Premium_due;
                    model.Stamp_duty = item.Stamp_duty;
                    model.ZTSC_Levy = item.ZTSC_Levy;
                    model.Comission_Amount = item.Comission_Amount;
                    //IncludeRadioLicenseCost = x.IncludeRadioLicenseCost,
                    model.RadioLicenseCost = item.RadioLicenseCost;
                    model.Zinara_License_Fee = item.Zinara_License_Fee;
                    model.Sum_Insured = item.Sum_Insured;
                    model.GrandPremium = item.Premium_due + item.Zinara_License_Fee.Value + item.RadioLicenseCost.Value + item.Stamp_duty + item.ZTSC_Levy;
                    list.Add(model);
                }
            }

            //_model.ListGrossWrittenPremiumReportdata = ListGrossWrittenPremiumReport.OrderByDescending(p => p.Id).ToList();
            _model.ListGrossWrittenPremiumReportdata = list.OrderByDescending(p => p.Id).ToList();


            var endorsmentList = GetGWPEndorsmentReport(_model);
            _model.ListGrossWrittenPremiumReportdata.AddRange(endorsmentList);

            return View("AccountsGrossWrittenPremuimReport", _model);
        }

        class Balance
        {
            public decimal balance { get; set; }
            public string CreatedOn { get; set; }
            public string Reference { get; set; }
        }

        class UserName
        {
            public string UserNameValue { get; set; }
            public string PolicyCurrencyValue { get; set; }
        }
        class PolicyInvoice
        {
            public int PolicyId { get; set; }
            public string PolicyNumber { get; set; }
            public decimal InvoiceAmount { get; set; }
            public int CreatedBy { get; set; }

            public DateTime CreateOn { get; set; }


        }








    }
}