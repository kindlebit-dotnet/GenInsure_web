using AutoMapper;
using Insurance.Domain;
using Insurance.Service;
using InsuranceClaim.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;
using System.Data.Entity;
using Newtonsoft.Json;

namespace InsuranceClaim.Controllers
{

    public class BrokerNonmoterController : Controller
    {



        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        string AdminEmail = WebConfigurationManager.AppSettings["BrokerEmail"];
        RoleManager<IdentityRole> roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext()));
        BrokerPaymentREcipt data = new BrokerPaymentREcipt();

        string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["Insurance"].ConnectionString;
        string _BrokerUser = "f97fe3e9-8494-4847-afde-627b4fdbe37b";

        string _brokerRoleName = "Broker";
        string ErrorMsg = "";
        bool condition = true;
        public BrokerNonmoterController()
        {


        }
        public BrokerNonmoterController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
        {
            UserManager = userManager;
            SignInManager = signInManager;
        }

        public ApplicationSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<ApplicationSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

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


        [Authorize(Roles = "Broker Manager")]
        public ActionResult Index()
        {
            ViewBag.PolicyClass = InsuranceContext.InsurancePolicyClasses.All().ToList();
            ViewBag.Riskcovers = InsuranceContext.RiskCovers.All().ToList();
            return View();
        }
        //Add Policy Class Name
        [HttpPost]
          public ActionResult AddPolicyClass(Pclass pclassData)
        {

            if (pclassData.Name != null)
            {
                InsurancePolicyClass tbl = new InsurancePolicyClass();
                tbl.PolicyClassName = pclassData.Name;
                InsuranceContext.InsurancePolicyClasses.Insert(tbl);
                //var datapclass = InsuranceContext.InsurancePolicyClasses.All().ToList();
                ViewBag.Riskcovers = InsuranceContext.RiskCovers.All().ToList();
                //return Json( datapclass, JsonRequestBehavior.AllowGet);
                //return View(datapclass);
                var datapclass = InsuranceContext.InsurancePolicyClasses.All()
              .Select(a => new
              {
                  PolicyClassName = a.PolicyClassName,
                  Id = a.Id

              });
                return Json(datapclass, JsonRequestBehavior.AllowGet);

            }
            return Json("", JsonRequestBehavior.AllowGet);
        }
 
         //Add Risk cover Name
        [HttpPost]
        public ActionResult AddRiskcover(Pclass pclassData)
        {
            if (pclassData.Name != null)
            {

                RiskCover tbl = new RiskCover();
                tbl.Fk_Ins_plcyID = pclassData.Fk_Ins_plcyID;
                tbl.RskCoverName = pclassData.Name;
                InsuranceContext.RiskCovers.Insert(tbl);
                var datapclass = InsuranceContext.RiskCovers.All()
             .Select(a => new
             {
                 RskCoverName = a.RskCoverName,
                 Id = a.Id,
                 Fk_Ins_plcyID = a.Fk_Ins_plcyID,
             });
                return Json(datapclass, JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AddRiskItem(Pclass pclassData)
        {
            if (pclassData.Name != null)
            {
                var policyId = InsuranceContext.RiskCovers.All().Where(m => m.Id == pclassData.Fk_RskCoverID).FirstOrDefault();
                RiskItem tbl = new RiskItem();
                tbl.Fk_Ins_plcyID = policyId.Fk_Ins_plcyID;
                tbl.Fk_RskCoverID = pclassData.Fk_RskCoverID;
                tbl.RiskItemName = pclassData.Name;
                InsuranceContext.RiskItems.Insert(tbl);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }
        [HttpGet]

        public JsonResult GetRiskCover(int selectedValue)
        {

            var data = InsuranceContext.InsurancePolicyClasses.All().ToList();
            var req = data.FirstOrDefault(item => item.Id == selectedValue)?.Id;

            var rskcvr = InsuranceContext.RiskCovers.All().Where(x => x.Fk_Ins_plcyID == req)
                .Select(a => new
                {
                    Rskcover = a.RskCoverName,
                    Id = a.Id

                });
            return Json(rskcvr, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles = "Broker Manager")]
        public ActionResult NonMoterProductList()
        {
            //var db = InsuranceContext.Products.All(where: "Active ='True' or Active is null").ToList();
            var db = InsuranceContext.InsurancePolicyClasses.All().ToList();
            return View(db);
        }

        public ActionResult DeletePolicyClass(int Id)
        {
            var _riskitems = InsuranceContext.RiskItems.All(where: $"Fk_Ins_plcyID = {Id}").ToList();
            var _riskcovers = InsuranceContext.RiskCovers.All(where: $"Fk_Ins_plcyID = {Id}").ToList();
            var _policyclass = InsuranceContext.InsurancePolicyClasses.Single(where: $"Id = {Id}");
            foreach (var item in _riskitems)
            {
                var item1 = item.Single(where: $"Fk_Ins_plcyID = {item.Fk_Ins_plcyID}");

                InsuranceContext.RiskItems.Delete(item1);
            }

            foreach (var item in _riskcovers)
            {
                var item2 = item.Single(where: $"Fk_Ins_plcyID = {item.Fk_Ins_plcyID}");
                InsuranceContext.RiskCovers.Delete(item2);
            }

            InsuranceContext.InsurancePolicyClasses.Delete(_policyclass);
            return RedirectToAction("ProductList");
        }

        [Authorize(Roles = "Broker Manager")]
        public ActionResult NonMoterRiskCover(int id)
        {
            //var db = InsuranceContext.Products.All(where: "Active ='True' or Active is null").ToList();
            var db = InsuranceContext.RiskCovers.All(where: $"Fk_Ins_plcyID = {id}").ToList();
            return View(db);
        }

        public ActionResult ProductEdit(int Id)
        {
            RiskCoverModel1 m1 = new RiskCoverModel1();
            var record = InsuranceContext.InsurancePolicyClasses.All(where: $"Id ={Id}").FirstOrDefault();
            m1.Id = record.Id;
            m1.PolicyClassName = record.PolicyClassName;
            return View(m1);
        }

        [HttpPost]
        public ActionResult ProductEdit(RiskCoverModel1 model)
        {
            if (ModelState.IsValid)
            {
                InsurancePolicyClass m1 = new InsurancePolicyClass();
                //var db = InsuranceContext.InsurancePolicyClasses.Single(where: $"Id = {model.Id}");
                m1.PolicyClassName = model.PolicyClassName;
                m1.Id = model.Id;
                InsuranceContext.InsurancePolicyClasses.Update(m1);

            }
            return RedirectToAction("NonMoterProductList");
        }
        [Authorize(Roles = "Broker Manager")]
        public ActionResult RiskCoverEdit(int Id)
        {
            RiskCoverModel2 m1 = new RiskCoverModel2();
            var record = InsuranceContext.RiskCovers.All(where: $"Id ={Id}").FirstOrDefault();
            m1.Id = record.Id;
            m1.RskCoverName = record.RskCoverName;
            //var model = Mapper.Map<InsurancePolicyClass, InsurancePolicyClasses>(record);
            return View(m1);
        }

        [HttpPost]
        public ActionResult RiskCoverEdit(RiskCoverModel2 model)
        {
            var polictId = InsuranceContext.RiskCovers.Single(where: $"Id = {model.Id}");
            if (ModelState.IsValid)
            {
                RiskCover m1 = new RiskCover();


                m1.RskCoverName = model.RskCoverName;
                m1.Id = model.Id;
                m1.Fk_Ins_plcyID = polictId.Fk_Ins_plcyID;
                InsuranceContext.RiskCovers.Update(m1);

            }
            return RedirectToAction("NonMoterRiskCover", new { Id = polictId.Fk_Ins_plcyID });
        }

        public ActionResult DeleteRiskCover(int Id)
        {
            var _riskitems = InsuranceContext.RiskItems.All(where: $"Fk_RskCoverID = {Id}").ToList();
            var _riskcovers = InsuranceContext.RiskCovers.Single(where: $"Id = {Id}");
            foreach (var item in _riskitems)
            {
                var item1 = item.Single(where: $"Fk_RskCoverID = {item.Fk_RskCoverID}");
                InsuranceContext.RiskItems.Delete(item1);
            }

            InsuranceContext.RiskCovers.Delete(_riskcovers);
            return RedirectToAction("NonMoterRiskCover", new { Id = _riskcovers.Fk_Ins_plcyID });
        }

        [Authorize(Roles = "Broker Manager")]
        public ActionResult NonMoterRiskItem(int id)
        {
            //var db = InsuranceContext.Products.All(where: "Active ='True' or Active is null").ToList();
            var db = InsuranceContext.RiskItems.All(where: $"Fk_RskCoverID = {id}").ToList();
            return View(db);
        }

        [Authorize(Roles = "Broker Manager")]
        public ActionResult RiskItemEdit(int Id)
        {
            RiskItem1 m1 = new RiskItem1();
            var record = InsuranceContext.RiskItems.All(where: $"Id ={Id}").FirstOrDefault();
            m1.Id = record.Id;
            m1.RiskItemName = record.RiskItemName;

            return View(m1);
        }

        [HttpPost]
        public ActionResult RiskItemEdit(RiskItem1 model)
        {
            var policyId = InsuranceContext.RiskItems.Single(where: $"Id = {model.Id}");
            if (ModelState.IsValid)
            {
                RiskItem m1 = new RiskItem();
                m1.RiskItemName = model.RiskItemName;
                m1.Id = model.Id;
                m1.Fk_Ins_plcyID = policyId.Fk_Ins_plcyID;
                m1.Fk_RskCoverID = policyId.Fk_RskCoverID;
                InsuranceContext.RiskItems.Update(m1);
            }

            return RedirectToAction("NonMoterRiskItem", new { Id = policyId.Fk_RskCoverID });
        }

        public ActionResult DeleteRiskItem(int Id)
        {
            var _riskitems = InsuranceContext.RiskItems.Single(where: $"Id = {Id}");

            InsuranceContext.RiskItems.Delete(_riskitems);

            return RedirectToAction("NonMoterRiskItem", new { Id = _riskitems.Fk_RskCoverID });
        }
        public ActionResult PaymentReciept()
        {
            var paymentMethod = InsuranceContext.PaymentMethods.All().ToList();
            ViewBag.PaymentMethod = paymentMethod;
            ViewBag.Currencies = InsuranceContext.Currencies.All(where: $"IsActive = 'True'");
            return View();
        }

        
        
        public JsonResult GetCustomername(string txtvalue)
         {
           listquotation model = new listquotation();
            var policyNumber = InsuranceContext.NonMotorCustomerInvoices.Single(where: $"InvoiceNumber='{txtvalue}'");
            if (policyNumber != null)
            {
                var policyid = policyNumber.CustomerId;
                int customerId = Convert.ToInt32(policyid);
                var brokercustomerdetails = InsuranceContext.BrokerCustomers.Single(where: $"Id='{customerId}'");
                var product = new NonMoterProductDetail();
                var Customerdetail = new QuotationCustomer();
                var Invoicenumber = new NonMotorCustomerInvoice();
                var receiptInvoiceNumber = new BrokerPaymentREcipt(); 
                if (brokercustomerdetails != null)
                {
                    //var nonmoterdetail = InsuranceContext.NonMoterProductDetails.(where: $"CustomerId='{customerId}'");
                    var nonmoterdetail = InsuranceContext.NonMoterProductDetails.All().Where(mh => mh.CustomerId == customerId).ToList();

                    if (nonmoterdetail != null)
                    {
                        decimal totalAmountDue = 0;
                        foreach (var nonmoterdetails in nonmoterdetail)
                        {
                            totalAmountDue += Convert.ToDecimal(nonmoterdetails.TotalPremiumPayable);
                        }

                        model.AmountDue = totalAmountDue.ToString();
                       var Currency = nonmoterdetail.Where(s => s.CustomerId == customerId).FirstOrDefault();
                        model.Currency = Currency.Currency;
                    }
                  

                    model.FirstName = brokercustomerdetails.FirstName;
                    model.SurName = brokercustomerdetails.SurName;
                    if (model.FirstName == null)
                    {
                        model.CustomerName = brokercustomerdetails.BusinessName;
                    }
                    else
                    {
                        model.CustomerName = model.FirstName + " " + model.SurName;
                    }
                    model.BusinessName = brokercustomerdetails.BusinessName;
                    model.PolicyNumber = brokercustomerdetails.PolicyNumber;
                    model.CustomerId = brokercustomerdetails.Id;
                  
                }
                product = InsuranceContext.NonMoterProductDetails.Single(where: $"CustomerId ={customerId}");
                if (product != null)
                {
                    
                    model.StampDuty = product.StampDuty;
                    model.Id = product.CustomerId;

                }
                receiptInvoiceNumber = InsuranceContext.BrokerPaymentREcipts.All().Where(receipt => receipt.InvoiceNo == txtvalue).OrderByDescending(receipt => receipt.Id).FirstOrDefault();              
                if(receiptInvoiceNumber!=null)
                {
                    model.AmountDue = receiptInvoiceNumber.ReceiptAmount;
                }
                Invoicenumber = InsuranceContext.NonMotorCustomerInvoices.Single(where: $"PolicyNumber = '{policyNumber}'");
                if (Invoicenumber != null)
                {
                    model.InvoiceNumber = Invoicenumber.InvoiceNumber;
                }
            }
            return Json(model, JsonRequestBehavior.AllowGet);
        }

      
        public ActionResult GenerateInvoice(int Id = 0)
        {
            var userID = "";
            int PolicyId = 0;
            int CoverId = 0;
            int ItemId = 0;
            bool IsQutationId = false;
            bool IsBrokerId = false;
            decimal TotPreimium = 0;
            decimal TotPayble = 0;
            decimal totalpayble = 0;
            var isActive = "";
            decimal TotalPremium = 0;
            BrokerCustomer m1 = new BrokerCustomer();
            NonMotorCustomerInvoice datatable = new NonMotorCustomerInvoice();
            bool userLoggedin = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            if (userLoggedin)
            {
                userID = System.Web.HttpContext.Current.User.Identity.GetUserId();
                var roles = UserManager.GetRoles(userID).FirstOrDefault();
            }
            var Username = InsuranceContext.Customers.All().Where(m => m.UserID == userID).FirstOrDefault();
            var LoggedInUser = $"{Username.FirstName} {Username.LastName}";
            listquotation data = new listquotation();
            data.customerdetails = new List<QuotationItem>();          
            decimal Totalannualsuminsured = 0;
            decimal Totalquarterlysuminsured = 0;
            decimal Totaltermlysuminsured = 0;
            decimal Totalmonthlysuminsured = 0;
            var ExistingCustomerId = InsuranceContext.BrokerCustomers.All().Where(m => m.Id == Id).FirstOrDefault();
            if (ExistingCustomerId != null)
            {
                var query = "select NonMoterProductDetail.CustomerId,BrokerCustomer.PolicyNumber,BrokerCustomer.BusinessPhoneNumber,BrokerCustomer.isEdit,BrokerCustomer.PolicyNumber,BrokerCustomer.FirstName,BrokerCustomer.SurName,BrokerCustomer.ContactNumber,BrokerCustomer.PhysicalAddress,BrokerCustomer.Email,BrokerCustomer.BusinessName,BrokerCustomer.BusinessAddress,BrokerCustomer.BusinessPartnerNumber,BrokerCustomer.ContactPersonName,BrokerCustomer.ContactPersonEmail, NonMoterProductDetail.InsorancePolicyId,NonMoterProductDetail.RiskcoverId,NonMoterProductDetail.RiskItemId,NonMoterProductDetail.RiskAddress,NonMoterProductDetail.RiskDescription,NonMoterProductDetail.SumInsured,NonMoterProductDetail.RiskRate,NonMoterProductDetail.PaymentTerm,NonMoterProductDetail.StampDuty,NonMoterProductDetail.TotalPremiumPayable,NonMoterProductDetail.Currency from BrokerCustomer INNER JOIN NonMoterProductDetail ON BrokerCustomer.Id = NonMoterProductDetail.CustomerId where CustomerId = @Id";
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    using (SqlCommand command = new SqlCommand(query, connection))
                    {
                        decimal Anualstampduty = 0;
                        decimal Quarterlystampduty = 0;
                        decimal monthlystampduty = 0;
                        decimal termlystampduty = 0;
                        decimal TotAnualstampduty = 0;
                        decimal TotQuarterlystampduty = 0;
                        decimal Totmonthlystampduty = 0;
                        decimal Tottermlystampduty = 0;
                        decimal totbasicpremium = 0;
                        command.Parameters.AddWithValue("@Id", Id);
                        using (SqlDataReader reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var QuotationModel = new QuotationItem();
                                data.FirstName = reader["FirstName"].ToString();
                                data.LastName = reader["SurName"].ToString();
                                data.CustomerName = reader["ContactPersonName"].ToString();
                                data.Id = Convert.ToInt32(reader["CustomerId"].ToString());
                                data.Email = reader["Email"].ToString();
                                data.ContactNumber = reader["ContactNumber"].ToString();
                                data.PhysicalAddress = reader["PhysicalAddress"].ToString();
                                data.BusinessName = reader["BusinessName"].ToString();
                                data.BusinessAddress = reader["BusinessAddress"].ToString();
                                data.BusinessPartnerNumber = reader["BusinessPartnerNumber"].ToString();
                                data.BusinessPhoneNumber = reader["BusinessPhoneNumber"].ToString();
                                data.ContactPersonEmail = reader["ContactPersonEmail"].ToString();
                                data.PolicyNumber = reader["PolicyNumber"].ToString();
                                data.isEdit = (bool)reader["isEdit"];
                                var policy = reader["InsorancePolicyId"].ToString();
                                PolicyId = Convert.ToInt32(policy);
                                var Riskcover = reader["RiskcoverId"].ToString();
                                CoverId = Convert.ToInt32(Riskcover);
                                var RiskItem = reader["RiskItemId"].ToString();
                                ItemId = Convert.ToInt32(RiskItem);
                                var policyclass = InsuranceContext.InsurancePolicyClasses.All().Where(p => p.Id == PolicyId).FirstOrDefault();
                                QuotationModel.PolicyClassName = policyclass.PolicyClassName;
                                var cover = InsuranceContext.RiskCovers.All().Where(r => r.Id == CoverId).FirstOrDefault();
                                QuotationModel.RiskCoverName = cover.RskCoverName;
                                var RiskitemId = InsuranceContext.RiskItems.All().Where(r => r.Id == ItemId).FirstOrDefault();
                                QuotationModel.RiskItemName = RiskitemId.RiskItemName;
                                QuotationModel.RiskAddress = reader["RiskAddress"].ToString();
                                QuotationModel.SumInsured = reader["SumInsured"].ToString();
                                QuotationModel.StampDuty = reader["StampDuty"].ToString();
                                QuotationModel.RiskRate = reader["RiskRate"].ToString();
                                QuotationModel.PaymentTerm = reader["PaymentTerm"].ToString();
                                QuotationModel.Currency = reader["Currency"].ToString();
                                QuotationModel.TotalPremiumPayable = reader["TotalPremiumPayable"].ToString();
                                decimal sumInsuredValue;
                                decimal riskRateValue;
                                decimal basicPremium = 0;

                                data.PolicyNumber = reader["PolicyNumber"].ToString();
                                if (decimal.TryParse(QuotationModel.SumInsured, out sumInsuredValue) &&
                                    decimal.TryParse(QuotationModel.RiskRate, out riskRateValue))
                               {
                                    switch (QuotationModel.PaymentTerm)
                                    {
                                       case "Annual":
                                            basicPremium = Convert.ToDecimal(QuotationModel.TotalPremiumPayable);
                                            Anualstampduty = Convert.ToDecimal(QuotationModel.StampDuty);
                                            totbasicpremium = basicPremium - Anualstampduty;
                                            break;

                                        case "Quarterly":
                                            basicPremium = Convert.ToDecimal(QuotationModel.TotalPremiumPayable);
                                            Quarterlystampduty = Convert.ToDecimal(QuotationModel.StampDuty);
                                            totbasicpremium = basicPremium - Quarterlystampduty;
                                            break;

                                        case "Termly":
                                            basicPremium = Convert.ToDecimal(QuotationModel.TotalPremiumPayable);
                                            termlystampduty = Convert.ToDecimal(QuotationModel.StampDuty);
                                            totbasicpremium = basicPremium - termlystampduty;
                                            break;

                                        case "Monthly":
                                            basicPremium = Convert.ToDecimal(QuotationModel.TotalPremiumPayable);
                                            monthlystampduty = Convert.ToDecimal(QuotationModel.StampDuty);
                                            totbasicpremium = basicPremium - monthlystampduty;
                                            break;
                                    }

                                    QuotationModel.BasicPremium = Convert.ToDecimal(totbasicpremium.ToString("N2"));
                                    QuotationModel.TotalPremiumPayable = QuotationModel.BasicPremium.ToString();
                                    TotalPremium += QuotationModel.BasicPremium;
                                    TotAnualstampduty += Anualstampduty;
                                    TotQuarterlystampduty += Quarterlystampduty;
                                    Totmonthlystampduty += monthlystampduty;
                                    Tottermlystampduty += termlystampduty;

                                    if (QuotationModel.PaymentTerm == "Annual")
                                    {
                                        data.Annualtotalpayble = Convert.ToDouble(TotalPremium);
                                        QuotationModel.AnnualPremium = QuotationModel.BasicPremium;
                                        TotPayble = TotalPremium + TotAnualstampduty;


                                    }
                                    else if (QuotationModel.PaymentTerm == "Quarterly")
                                    {
                                        data.Quarterlytotalpayble = Convert.ToDouble(TotalPremium);
                                        QuotationModel.QuarterlyPremium = QuotationModel.BasicPremium;
                                        TotPayble  = TotalPremium + TotQuarterlystampduty;
                                        
                                    }
                                    else if (QuotationModel.PaymentTerm == "Termly")
                                    {
                                        data.Termlytotalpayble = Convert.ToDouble(TotalPremium);
                                        QuotationModel.TerminalPremium = QuotationModel.BasicPremium;
                                        TotPayble = TotalPremium + Tottermlystampduty;
                                    }
                                    else if (QuotationModel.PaymentTerm == "Monthly")
                                    {
                                        data.Monthlytotalpayble = Convert.ToDouble(TotalPremium);
                                        QuotationModel.MonthlyPremium = QuotationModel.BasicPremium;
                                        TotPayble = TotalPremium + Totmonthlystampduty;
                                    }


                                }
                                
                                data.customerdetails.Add(QuotationModel);
                            }

                            ViewBag.CustomerId = data.Id;
                            data.Annualstampvalue = Convert.ToDouble(TotAnualstampduty);
                            data.Quarterlystampvalue = Convert.ToDouble(TotQuarterlystampduty);
                            data.Monthlystampvalue = Convert.ToDouble(Totmonthlystampduty);
                            data.Termlystampvalue = Convert.ToDouble(Tottermlystampduty);

                        }
                    }
                }
                data.LoggedInUser = LoggedInUser;
                ViewBag.data = data;
                ViewBag.UserId = Id;
                ViewBag.InvoiceId = Id;
                ViewBag.Policygeneratenumber = data.PolicyNumber;
                IsBrokerId = data.isEdit;
            }

            if (IsBrokerId == false)
            {
                var QuotationModel = new QuotationItem();
                datatable.BusineessContactNumber = data.ContactNumber;
                datatable.BusinessEmail = data.ContactPersonEmail;
                datatable.BusinessName = data.BusinessName;
                datatable.CreatedBy = LoggedInUser;
                datatable.CreatedOn = DateTime.Now;
                datatable.Currency = QuotationModel.Currency;
                datatable.CusotmerAddress = data.PhysicalAddress;
                datatable.CustomerEmail = data.Email;
                datatable.CustomerName = data.FirstName + " " + data.LastName;             
                datatable.InvoiceNumber = GenerateInvoiceNumber();
                datatable.PolicyClassId = PolicyId;
                datatable.RiskCoverId = CoverId;
                datatable.RiskItem = ItemId;
                datatable.StampDuty = QuotationModel.StampDuty;
                datatable.CustomerId = Id;
                datatable.PolicyNumber = data.PolicyNumber;
                datatable.TotalPremium =TotalPremium.ToString();
                datatable.TotalPayable = Convert.ToString(TotPayble);
                datatable.TotalInsured = data.TotalIsured;
                UpdateBrokerOrQuotation(IsBrokerId, IsQutationId, Id);
                if (datatable != null)
                {
                    InsuranceContext.NonMotorCustomerInvoices.Insert(datatable);
                    data.InvoiceNumber = datatable.InvoiceNumber;
                    ViewBag.InvoiceId = datatable.Id;
                }
            }
            var Inv = InsuranceContext.NonMotorCustomerInvoices.All().FirstOrDefault(x => x.CustomerId == Id);
            if (Inv != null)
            {
                data.InvoiceNumber = Inv.InvoiceNumber;
            }
            return View(data);
        }

        public void UpdateBrokerOrQuotation(bool IsQutationId, bool IsBrokerId, int Id)
        {
            if (IsQutationId == false && Id != 0)
            {
                var updateQuotation = InsuranceContext.BrokerCustomers.All().FirstOrDefault(qc => qc.Id == Id);

                if (updateQuotation != null)
                {
                    updateQuotation.isEdit = true;
                    InsuranceContext.BrokerCustomers.Update(updateQuotation);
                    // Assuming you need to save changes to the database.
                }
            }
            if (IsBrokerId == false && Id != 0)
            {
                var updateQuotation = InsuranceContext.QuotationCustomers.All().FirstOrDefault(qc => qc.Id == Id);

                if (updateQuotation != null)
                {
                    updateQuotation.isEdit = true;
                    InsuranceContext.QuotationCustomers.Update(updateQuotation);
                }
            }
        }


        public string GenerateInvoiceNumber()
        {
            string invoiceNumber = "";
            var year = DateTime.Now.Year;
            var sequenceNumber = GetNextSequenceNumber();
            var check = year % 100;
            if (sequenceNumber.Contains(check.ToString()))
            {
                invoiceNumber = $"INV{sequenceNumber:D7}-1";
            }
            else
            {
                invoiceNumber = $"INV{year % 100:00}{sequenceNumber:D7}-1";
            }
            return invoiceNumber;
        }

        private string GetNextSequenceNumber()
        {
            var sequenceNumber = "";
            var noproductdetail = InsuranceContext.NonMotorCustomerInvoices.All().OrderByDescending(t => t.Id).FirstOrDefault();
            var _invoiceNumber = noproductdetail?.InvoiceNumber;

            if (_invoiceNumber != null && _invoiceNumber.StartsWith("INV"))
            {
                // Extract the sequence part from the existing invoice number
                string[] parts = _invoiceNumber.Split('-');

                if (parts.Length > 1 && int.TryParse(parts[0].Substring(3), out int parsedNumber))
                {
                    // Increment the parsed number
                    int incrementedNumber = parsedNumber + 1;

                    // Format the incremented number with leading zeros
                    sequenceNumber = incrementedNumber.ToString("D7");

                    return sequenceNumber;
                }
            }

            // If there is no existing invoice number, or parsing fails, start from 1 with leading zeros
            return "0000001";
        }



        [HttpPost]
        public ActionResult BrokerSendPdfEmail(int id, string pdfDataUri, string fileName)
        {
            var pdfFileName = fileName;
            try
            {
                Insurance.Service.EmailService objEmailService = new Insurance.Service.EmailService();
                string pdfVirtualPath = $"~/PdfFolder/{fileName}";
                string pdfPhysicalPath = Server.MapPath(pdfVirtualPath); 
                var Email = InsuranceContext.QuotationCustomers.All().Where(x => x.Id == id).FirstOrDefault();

                if (!System.IO.File.Exists(pdfPhysicalPath))
                {
                    return Json(new { success = false, message = "PDF file not found." }, JsonRequestBehavior.AllowGet);
                }

                byte[] pdfBytes = System.IO.File.ReadAllBytes(pdfPhysicalPath);

                List<string> attachments = new List<string>
            {
                pdfVirtualPath
            };

                // Create a new email message
                MailMessage mailMessage = new MailMessage();
                mailMessage.Subject = "PDF Report";
                mailMessage.Body = "Please find the PDF report attached.";
                string toEmail = Email.EmailAddress;
                if (toEmail != null)
                {
                    toEmail = Email.EmailAddress;
                }
                else
                {
                    toEmail = Email.ContactPersonEmail;
                }
                // Attach the PDF file to the email message
                MemoryStream pdfStream = new MemoryStream(pdfBytes);
                Attachment pdfAttachment = new Attachment(pdfStream, pdfFileName, "application/pdf");
                mailMessage.Attachments.Add(pdfAttachment);
                objEmailService.SendEmail(toEmail, "", "", mailMessage.Subject, mailMessage.Body, attachments);
                //DeleteFile(pdfFileName);
                return Json(new { success = true, message = "Email sent successfully." }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Email sending failed: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }

        }


        [HttpPost]
        public ActionResult SavePdf(string pdfDataUri, string fileName)
        {
            try
            {
                // Convert base64 data to bytes
                var base64Data = pdfDataUri.Replace("data:application/pdf;base64,", "");
                var pdfBytes = Convert.FromBase64String(base64Data);
                // Specify the path where you want to save the PDF file
                var folderPath = Server.MapPath("~/PdfFolder/");
                var filePath = Path.Combine(folderPath, fileName);
                // Check if the folder exists; if not, create it.
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }
                // Save the new PDF file to the specified path
                System.IO.File.WriteAllBytes(filePath, pdfBytes);
                return Json(new { success = true, message = "PDF saved successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Failed to save the PDF: " + ex.Message });
            }
        }

        //view Quotation list
        [HttpGet]
        public ActionResult ViewInvoices()
        {
            var Quotations = InsuranceContext.NonMotorCustomerInvoices.All().ToList();
            return View(Quotations);
        }

        public ActionResult DeleteInvoice(string invoiceNumber)
        {
            if (invoiceNumber != null)
            {
                var invoice = InsuranceContext.NonMotorCustomerInvoices.Single(where: $"InvoiceNumber='{invoiceNumber}'");
                if (invoice != null)
                {
                    var customerid = invoice.CustomerId;
                    UpdadteIsedit(customerid);
                }
                InsuranceContext.NonMotorCustomerInvoices.Delete(invoice);
            }
            return RedirectToAction("ViewInvoices");
        }

      public string UpdadteIsedit(int Id = 0)
        {

            var customerUpdate = InsuranceContext.BrokerCustomers.Single(where: $"Id='{Id}'");
            if (customerUpdate != null)
            {
                customerUpdate.isEdit = false;
                customerUpdate.PolicyNumber = "";
                InsuranceContext.BrokerCustomers.Update(customerUpdate);
            }

            return "";
        }

        [HttpPost]
        public ActionResult BrokerReceipt(NonmoterRecieptModel model)
        {
        string userID = "";
        decimal InvoiceAmount = 0;
        //Session["BrokerRecieptClASS"] = model;
        bool userLoggedin = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
        if (userLoggedin)
             {
            userID = System.Web.HttpContext.Current.User.Identity.GetUserId();
            var roles = UserManager.GetRoles(userID).FirstOrDefault();
        }
        var Username = InsuranceContext.Customers.All().Where(m => m.UserID == userID).FirstOrDefault();
        var LoggedInUser = $"{Username.FirstName} {Username.LastName}";

        if (string.IsNullOrEmpty(model.PolicyNumber))
        {
            return Json(new { Success = false, ErrorMessage = "Policy number is not valid." });
        }

        if (model.AmountDue <= 0)
        {
            return Json(new { Success = false, ErrorMessage = "Amount due is not valid." });
        }

        if (model.Balance != null && Convert.ToDecimal(model.Balance) < 0)
        {
            return Json(new { Success = false, ErrorMessage = "Balance amount is in negative." });
        }

            //var invoice = InsuranceContext.BrokerPaymentREcipts.All().FirstOrDefault(mt => mt.InvoiceNo == model.PolicyNumber);
            //if (invoice != null)
            //{
            //  return RedirectToAction("BrokerPaymentReceipt");
            //}
            //else
            var InvoiceNumber = model.PolicyNumber;
            if(InvoiceNumber!=null)
            {
                var invoicetble = InsuranceContext.NonMotorCustomerInvoices.Single(where: $"InvoiceNumber = '{InvoiceNumber}'");
                InvoiceAmount = Convert.ToDecimal(invoicetble.TotalPayable);
                model.InvoiceAmount = InvoiceAmount;
        }
            //{            
            var data = new BrokerPaymentREcipt
            {
                AmountDue = Convert.ToDecimal(model.AmountDue),
                CustomerName = model.CustomerName,
                InvoiceNo = model.PolicyNumber,
                PolicyNo = model.PolicyNo,
                PaymentMethod = model.PaymentMethodId.ToString(),
                TransactionReference = model.TransactionReference,
                ReceiptAmount = model.Balance,
                CreatedDate = DateTime.Now,
                brokerCustomerID = model.CustomerId,
                TenderedAmount = model.TenderedAmount.ToString(),
                Balance = Convert.ToDecimal(model.Balance),
                ReceiptedBy = LoggedInUser,
                Currency = model.Currency,
            };
               InsuranceContext.BrokerPaymentREcipts.Insert(data);
               model.ReceiptNo = data.Id;
               Session["BrokerRecieptClASS"] = model;
           
            return RedirectToAction("BrokerPaymentReceipt");
        //}
          }
        //Getting Payment type
        public string returnpayemnt(int id = 0)
        {
            Dictionary<int, string> paymentMethods = new Dictionary<int, string>()
            {
              { 1, "Cash" },
              { 2, "Ecocash" },
              { 3, "Swipe" },
              { 4, "MasterVisa Card"},
              { 5, "Bank Transfer" }
            };

            return paymentMethods.ContainsKey(id) ? paymentMethods[id] : "";
        }

         //save recipt file
        public void SaveReciptPdf(int receiptHistoryId)
        {
            var ReceiptHistory = InsuranceContext.ReceiptHistorys.Single(where: $"Id='{receiptHistoryId}'");
            var policyDetails = InsuranceContext.PolicyDetails.Single(where: $"PolicyNumber='{ReceiptHistory.PolicyNumber}'");
            var customer = InsuranceContext.Customers.Single(where: $"Id='{policyDetails.CustomerId}'");
            //var AspNetUsers = InsuranceContext.AspNetUsersUpdates
            var user = UserManager.FindById(customer.UserID);
            Insurance.Service.EmailService objEmailService = new Insurance.Service.EmailService();
            //string userRegisterationEmailPath = "~/Views/Shared/EmailTemplates/UserPaymentEmail.cshtml";
            string userRegisterationEmailPath = "/Views/Shared/EmaiTemplates/UserPaymentReceipt.cshtml";
            string EmailBody2 = System.IO.File.ReadAllText(System.Web.Hosting.HostingEnvironment.MapPath(userRegisterationEmailPath));
            string filepath = System.Configuration.ConfigurationManager.AppSettings["urlPath"];
            var Body2 = EmailBody2.Replace("#DATE#", DateTime.Now.ToShortDateString())
                .Replace("##path##", filepath).Replace("#FirstName#", customer.FirstName)
                .Replace("#LastName#", customer.LastName)
                .Replace("#AccountName#", ReceiptHistory.CustomerName)
                .Replace("#Address1#", customer.AddressLine1).Replace("#Address2#", customer.AddressLine2)
                .Replace("#Amount#", Convert.ToString(ReceiptHistory.AmountPaid))
                .Replace("#PaymentDetails#", "New Premium").Replace("#ReceiptNumber#", ReceiptHistory.Id.ToString())           
                .Replace("#TransactionReference#", ReceiptHistory.TransactionReference).Replace("#TransactionReference#", ReceiptHistory.TransactionReference)
                .Replace("#PaymentType#", (ReceiptHistory.PaymentMethodId == 1 ? "Cash" : (ReceiptHistory.PaymentMethodId == 2 ? "PayPal" : "PayNow")));
             List<string> attachements = new List<string>();
            attachements.Add("");
            var attacehmetn_File = MiscellaneousService.EmailPdf(Body2, customer.Id, ReceiptHistory.PolicyNumber, "Receipt");

       }

        //Making Receipted report when filled all details here
        public ActionResult BrokerPaymentReceipt()
        {
            NonmoterRecieptModel model = Session["BrokerRecieptClASS"] as NonmoterRecieptModel;
            Session.Remove("BrokerRecieptClASS"); 
          
            if (model != null)
            {
                ReceiptModuleModel viewModel = new ReceiptModuleModel();
                var custoID = model.CustomerId;

                if (custoID != 0)
                {
                    var policyAddress = InsuranceContext.BrokerCustomers.All().FirstOrDefault(pd =>pd.Id == custoID);
                    if (policyAddress != null)
                    {
                        viewModel.Address1 = policyAddress?.PhysicalAddress ?? policyAddress?.BusinessAddress;
                        viewModel.Email = policyAddress?.ContactPersonEmail ?? policyAddress?.Email;
                        viewModel.CustomerName = policyAddress.BusinessName ?? (policyAddress.FirstName + " " + policyAddress.SurName);
                        viewModel.AmountDue = Convert.ToDecimal(model.AmountDue);
                        viewModel.Balance = model.Balance;
                        viewModel.Id = model.ReceiptNo;
                        viewModel.TransactionReference = model.TransactionReference;
                        viewModel.TenderedAmount = model.TenderedAmount;
                        viewModel.InvoiceNumber = model.PolicyNumber;
                        viewModel.PolicyNo = model.PolicyNo;
                        viewModel.InvoiceAmount = model.InvoiceAmount;
                        viewModel.paymentMethodType = returnpayemnt(model.PaymentMethodId.HasValue ? model.PaymentMethodId.Value : 0);
                        viewModel.Date = DateTime.Now.ToString("dd/MM/yyyy");                 
                    }
                }
                return View(viewModel);
            }
            else
            {
                return View();
            }
        }

        //Send report by mail "Receiptedrepot"
        public ActionResult ReceiptSendPdfEmail(string id, string fileName)
        {
            try
            {
                Insurance.Service.EmailService objEmailService = new Insurance.Service.EmailService();

                // Specify the path to the PDF file
                string pdfVirtualPath = $"~/PdfFolder/{fileName}";
                string pdfPhysicalPath = Server.MapPath(pdfVirtualPath);

                // Check if the PDF file exists
                if (!System.IO.File.Exists(pdfPhysicalPath))
                {
                 return Json(new { success = false, message = "PDF file not found." });
                }

                // Read the PDF file bytes
                byte[] pdfBytes = System.IO.File.ReadAllBytes(pdfPhysicalPath);

                // Create a new email message
                MailMessage mailMessage = new MailMessage();
                mailMessage.Subject = "PDF Report";
                mailMessage.Body = "Please find the PDF report attached.";
                string toEmail = id;

                // Attach the PDF file to the email message
                MemoryStream pdfStream = new MemoryStream(pdfBytes);
                Attachment pdfAttachment = new Attachment(pdfStream, fileName, "application/pdf");
                mailMessage.Attachments.Add(pdfAttachment);

                // Send the email
                objEmailService.SendEmail(toEmail, "", "", mailMessage.Subject, mailMessage.Body, new List<string> { pdfVirtualPath });

                return Json(new { success = true, message = "Email sent successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Email sending failed: " + ex.Message });
            }
        }

        //For Gwp report list
        public ActionResult GWPReport(DateTime? FormDate, DateTime? EndDate, string Currency = null)
        {
         listquotation data = new listquotation();
         data.customerdetails = new List<QuotationItem>();
         var query = "SELECT Brokercustomer.FirstName, Brokercustomer.Id, Brokercustomer.ContactPersonName,Brokercustomer.BusinessName , Brokercustomer.SurName, Brokercustomer.PolicyNumber, " +
                     "Brokercustomer.Email, Brokercustomer.ContactNumber, Brokercustomer.UserId, " +
                     "Brokercustomer.CreatedOn, NonMoterProductDetail.RiskDescription, " +
                     "NonMoterProductDetail.SumInsured, NonMoterProductDetail.RiskRate, " +
                     "NonMoterProductDetail.PaymentTerm, NonMoterProductDetail.StampDuty, " +
                     "NonMoterProductDetail.TotalPremiumPayable, NonMoterProductDetail.Currency, NonMoterProductDetail.TotalPremiumPayable, NonMoterProductDetail.SumInsured, " +
                     "NonMotorCustomerInvoice.InvoiceNumber " +
                     "FROM NonMotorCustomerInvoice " +                   
                     "INNER JOIN NonMoterProductDetail ON NonMotorCustomerInvoice.CustomerId = NonMoterProductDetail.CustomerId " +
                     "INNER JOIN Brokercustomer ON NonMotorCustomerInvoice.CustomerId = Brokercustomer.Id " +
                     "WHERE(@FormDate IS NULL OR NonMotorCustomerInvoice.CreatedOn >= @FormDate) " +
                     "AND(@EndDate IS NULL OR NonMotorCustomerInvoice.CreatedOn <= @EndDate OR NonMotorCustomerInvoice.CreatedOn IS NULL) " +
                     "AND(@Currency IS NULL OR NonMoterProductDetail.Currency = @Currency) " +                       
                     "AND Brokercustomer.PolicyNumber IS NOT NULL";
                        
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                connection.Open(); 
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                   command.Parameters.AddWithValue("@FormDate", FormDate ?? (object)DBNull.Value);
                   command.Parameters.AddWithValue("@EndDate", EndDate ?? (object)DBNull.Value);
                   command.Parameters.AddWithValue("@Currency", Currency ?? (object)DBNull.Value);
                   data.FormDate = DateTime.Now.Date;
                   data.EndDate = DateTime.Now.Date.AddDays(1);
                   Dictionary<int, QuotationItem> quotationByCustomerId = new Dictionary<int, QuotationItem>();
                   using (SqlDataReader reader = command.ExecuteReader())
                   {
                       while (reader.Read())
                       {
                           var customerId = (int)reader["Id"];
                           if (quotationByCustomerId.ContainsKey(customerId))
                           {
                               quotationByCustomerId[customerId].TotalPremiumPayable = (Convert.ToDecimal(quotationByCustomerId[customerId].TotalPremiumPayable) + Convert.ToDecimal(reader["TotalPremiumPayable"])).ToString();
                               quotationByCustomerId[customerId].SumInsured = (Convert.ToDecimal(quotationByCustomerId[customerId].SumInsured) + Convert.ToDecimal(reader["SumInsured"])).ToString();
                              
                           }
                           else
                           {
                               var QuotationModel = new QuotationItem();
                               QuotationModel.Id = customerId;
                               QuotationModel.FirstName = reader["FirstName"].ToString() + " " + reader["SurName"].ToString();
                               if (QuotationModel.FirstName == " ")
                               {
                                   QuotationModel.FirstName = reader["BusinessName"].ToString();
                               }
                              QuotationModel.UserId = reader["UserId"].ToString();
                               if (QuotationModel.UserId != "")
                               {
                                   var custoname = InsuranceContext.Customers.Single($"UserID='{QuotationModel.UserId}'");
                                    var name1 = custoname.FirstName;
                                    var name2 = custoname.LastName;
                                    QuotationModel.UserId = name1 + " " + name2;
                               }
                                QuotationModel.CreatedOn = (DateTime)reader["CreatedOn"];
                                QuotationModel.SumInsured = Convert.ToDecimal(reader["SumInsured"]).ToString("0.00");
                                QuotationModel.RiskRate = reader["RiskRate"].ToString();
                                QuotationModel.PaymentTerm = reader["PaymentTerm"].ToString();
                                QuotationModel.StampDuty = reader["StampDuty"].ToString();
                                QuotationModel.TotalPremiumPayable = Convert.ToDecimal(reader["TotalPremiumPayable"]).ToString("0.00");
                                QuotationModel.Currency = reader["Currency"].ToString();
                                QuotationModel.InvoiceNumber = reader["InvoiceNumber"].ToString();
                                QuotationModel.PolicyNumber = reader["PolicyNumber"].ToString();
                                data.customerdetails.Add(QuotationModel);
                                quotationByCustomerId[customerId] = QuotationModel;
                           }
                        }
                    }
                }   
            }           
            return View(data);
        }

        //for receiptreport list
        public ActionResult ReceiptedReport(DateTime? FormDate, DateTime? EndDate, string Currency = null)
        {
            listquotation data = new listquotation();
            //BrokerCustomer brokercust = new BrokerCustomer();
            data.customerdetails = new List<QuotationItem>();
            var query = "SELECT Brokercustomer.FirstName, Brokercustomer.ContactPersonName, Brokercustomer.SurName, Brokercustomer.IsActive, Brokercustomer.PolicyNumber,Brokercustomer.BusinessName, BrokerPaymentREcipt.Id ,BrokerPaymentREcipt.PaymentMethod, BrokerPaymentREcipt.InvoiceNo, BrokerPaymentREcipt.Currency, BrokerPaymentREcipt.ReceiptAmount, BrokerPaymentREcipt.TransactionReference,BrokerPaymentREcipt.AmountDue, BrokerPaymentREcipt.TenderedAmount, BrokerPaymentREcipt.CreatedDate, BrokerPaymentREcipt.Balance, BrokerPaymentREcipt.ReceiptedBy, " +
            "Brokercustomer.Email, Brokercustomer.ContactNumber, Brokercustomer.UserId, " +
            "Brokercustomer.CreatedOn, " +
            "NonMotorCustomerInvoice.TotalPremium " +
            //"NonMoterProductDetail.SumInsured, NonMoterProductDetail.RiskRate, " +
            //"NonMoterProductDetail.PaymentTerm, NonMoterProductDetail.StampDuty, " +
            //"NonMoterProductDetail.TotalPremiumPayable, NonMoterProductDetail.Currency, NonMoterProductDetail.TotalPremiumPayable, NonMoterProductDetail.SumInsured, " +
            //"NonMotorCustomerInvoice.InvoiceNumber " +
            "FROM BrokerPaymentREcipt " +
            "INNER JOIN NonMotorCustomerInvoice ON BrokerPaymentREcipt.brokerCustomerID = NonMotorCustomerInvoice.CustomerId " +
            "INNER JOIN Brokercustomer ON NonMotorCustomerInvoice.CustomerId = Brokercustomer.Id " +
            "WHERE (@FormDate IS NULL OR BrokerPaymentREcipt.CreatedDate >= @FormDate) " +
            "AND (@EndDate IS NULL OR BrokerPaymentREcipt.CreatedDate <= @EndDate OR BrokerPaymentREcipt.CreatedDate IS NULL)" +
            "AND (@Currency IS NULL OR BrokerPaymentREcipt.Currency = @Currency)";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@FormDate", FormDate ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@EndDate", EndDate ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Currency", Currency ?? (object)DBNull.Value);
                    data.FormDate = DateTime.Now.Date;
                    data.EndDate = DateTime.Now.Date.AddDays(1);
                    Dictionary<int, QuotationItem> quotationByCustomerId = new Dictionary<int, QuotationItem>();
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var customerId = (int)reader["Id"];
                            
                            var QuotationModel = new QuotationItem();
                            QuotationModel.Id = customerId;
                            QuotationModel.FirstName = reader["FirstName"].ToString() + " " + reader["SurName"].ToString();
                            if (QuotationModel.FirstName == " ")
                            {
                                QuotationModel.FirstName = reader["BusinessName"].ToString();
                            }

                            QuotationModel.UserId = reader["UserId"].ToString();
                            if (QuotationModel.UserId != "")
                            {
                                var custoname = InsuranceContext.Customers.Single($"UserID='{QuotationModel.UserId}'");
                                var name1 = custoname.FirstName;
                                var name2 = custoname.LastName;
                                QuotationModel.UserId = name1 + " " + name2;
                            }
                             QuotationModel.CreatedOn = (DateTime)reader["CreatedDate"];
                             QuotationModel.Currency = reader["Currency"].ToString();
                             QuotationModel.InvoiceNumber = reader["InvoiceNo"].ToString();
                             QuotationModel.ReceiptNumber = Convert.ToInt32(reader["Id"]);
                             QuotationModel.PolicyNumber = reader["PolicyNumber"].ToString();
                             QuotationModel.Amountdue = Convert.ToDecimal(reader["AmountDue"]);
                             QuotationModel.TotalPremiumPayable = reader["TotalPremium"].ToString();
                             decimal balance = (reader["Balance"] != DBNull.Value) ? Convert.ToDecimal(reader["Balance"]) : 0;
                             QuotationModel.Balance = balance;
                             QuotationModel.PaymentMethod = Convert.ToInt32(reader["PaymentMethod"]);
                             QuotationModel.IsActive = Convert.ToBoolean(reader["IsActive"]);
                             var paymentMethodId = QuotationModel.PaymentMethod;
                             if (paymentMethodId != 0)
                             {
                               QuotationModel.PaymentMethodName = returnpayemnt(QuotationModel.PaymentMethod.HasValue ? paymentMethodId.Value : 0);
                             }
                             QuotationModel.ReceiptAmount = (reader["ReceiptAmount"] != DBNull.Value) ? Convert.ToDecimal(reader["ReceiptAmount"]) : 0;
                             if (balance == 0)
                             {
                               QuotationModel.ReceiptAmount = QuotationModel.Amountdue;
                             }
                             else
                             {
                               QuotationModel.ReceiptAmount = QuotationModel.ReceiptAmount;
                             }
                             QuotationModel.TransactionReference = (reader["TransactionReference"] != DBNull.Value) ? reader["TransactionReference"].ToString() : string.Empty;
                             QuotationModel.TenderedAmount = (reader["TenderedAmount"] != DBNull.Value) ? Convert.ToDecimal(reader["TenderedAmount"]) : 0;
                             QuotationModel.ReceiptedBy = reader["ReceiptedBy"].ToString();
                             data.customerdetails.Add(QuotationModel);
                             quotationByCustomerId[customerId] = QuotationModel;
                        }
                        //}
                    }
                }
            }

            return View(data);
        }
        //Renew Policy
        [HttpGet]
        public ActionResult RenewPolicy()
        {
            BrokerCustomer m1 = new BrokerCustomer();
            listquotation data = new listquotation();
            List<BrokerCustomer> listmddel = new List<BrokerCustomer>();
            data.customerdetails = new List<QuotationItem>();

            var list = "select BrokerCustomer.Id, BrokerCustomer.FirstName, BrokerCustomer.BusinessName, BrokerCustomer.BusinessPhoneNumber, BrokerCustomer.ContactNumber, BrokerCustomer.PolicyNumber, BrokerCustomer.ContactPersonName, BrokerCustomer.BusinessPartnerNumber, BrokerCustomer.Email, BrokerCustomer.BusinessAddress, BrokerCustomer.ContactPersonPhoneNumber, BrokerCustomer.ContactPersonEmail, BrokerCustomer.CreatedOn, BrokerCustomer.SurName, NonMoterProductDetail.RiskcoverId,NonMoterProductDetail.PolicyValidityPeriodFrom,NonMoterProductDetail.PolicyValidityPeriodTo, NonMoterProductDetail.RiskItemId from BrokerCustomer INNER JOIN NonMoterProductDetail ON BrokerCustomer.Id = NonMoterProductDetail.CustomerId ORDER BY BrokerCustomer.Id DESC";
            // HashSet to store unique policy numbers
            HashSet<string> uniquePolicyNumbers = new HashSet<string>();

            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(list, connection))
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var policyNumber = reader["PolicyNumber"].ToString();
                        if (!string.IsNullOrEmpty(policyNumber) && !uniquePolicyNumbers.Contains(policyNumber))
                        {
                            uniquePolicyNumbers.Add(policyNumber);

                            var quotationModel = new QuotationItem();
                            quotationModel.Id = (int)reader["Id"];
                            quotationModel.FirstName = reader["FirstName"].ToString();
                            quotationModel.LastName = reader["SurName"].ToString();
                            quotationModel.Email = reader["Email"].ToString();
                            quotationModel.ContactNumber = reader["ContactNumber"].ToString();
                            quotationModel.BusinessAddress = reader["BusinessAddress"].ToString();
                            quotationModel.ContactPersonEmail = reader["ContactPersonEmail"].ToString();
                            quotationModel.BusinessName = reader["BusinessName"].ToString();
                            quotationModel.BusinessPhoneNumber = reader["BusinessPhoneNumber"].ToString();
                            DateTime fromDate = Convert.ToDateTime(reader["PolicyValidityPeriodFrom"]);
                            DateTime toDate = Convert.ToDateTime(reader["PolicyValidityPeriodTo"]);

                            quotationModel.PolicyValidityPeriodFrom = fromDate.ToString("yyyy-MM-dd");
                            quotationModel.PolicyValidityPeriodTo = toDate.ToString("yyyy-MM-dd");

                            quotationModel.PolicyNumber = policyNumber;
                            quotationModel.ContactpersonName = reader["ContactPersonName"].ToString();
                            if (string.IsNullOrEmpty(quotationModel.ContactpersonName))
                            {
                                quotationModel.ContactpersonName = quotationModel.FirstName + ' ' + quotationModel.LastName;
                            }
                            data.customerdetails.Add(quotationModel);
                        }
                    }
                }
            }

            return View(data);
        }


        [HttpPost]
        public JsonResult Deletpolicy(int? policyId)
        {
            if (policyId == null)
            {
                return Json(new { success = false, message = "Invalid policy ID." });
            }

            var policy = InsuranceContext.BrokerCustomers.All().Where(m => m.Id == policyId).FirstOrDefault();
            if (policy != null)
            {

                policy.PolicyNumber = "";
                InsuranceContext.BrokerCustomers.Update(policy);
                
                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false, message = "Policy not found or number does not match." });
            }
        }

        public JsonResult Disablepolicy(int? policyId)
        {

            if (policyId == null)
            {
                return Json(new { success = false, message = "Invalid policy ID." });
            }

            var policy = InsuranceContext.BrokerCustomers.All().Where(m => m.Id == policyId).FirstOrDefault();
            if (policy != null)
            {

                policy.IsActive = false;
                InsuranceContext.BrokerCustomers.Update(policy);

                return Json(new { success = true });
            }
            else
            {
                return Json(new { success = false, message = "Policy not found or number does not match." });
            }
        }

    }
}