using Insurance.Domain;
using Insurance.Service;
using InsuranceClaim.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace InsuranceClaim.Controllers
{
    public class NonMotorCustomersController : Controller
    {

        private ApplicationSignInManager _signInManager;
        private ApplicationUserManager _userManager;
        string AdminEmail = WebConfigurationManager.AppSettings["BrokerEmail"];
        RoleManager<IdentityRole> roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext()));
        string connectionString = System.Configuration.ConfigurationManager.ConnectionStrings["Insurance"].ConnectionString;
        string _BrokerUser = "f97fe3e9-8494-4847-afde-627b4fdbe37b";
        string _brokerRoleName = "Broker";
        public NonMotorCustomersController()
        {

        }

        public NonMotorCustomersController(ApplicationUserManager userManager, ApplicationSignInManager signInManager)
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
        [Authorize(Roles = "Broker Manager,Broker")]
        public ActionResult Index(int id = 0)
        {
            return View();
        }

        //[Authorize(Roles = "Broker Manager")]
        [HttpPost]
        public ActionResult AddCustomer(BrokerCustomersModel model)
        {
            var userID = "";
            BrokerCustomer m1 = new BrokerCustomer();

            bool userLoggedin = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            if (userLoggedin)
            {
                userID = System.Web.HttpContext.Current.User.Identity.GetUserId();
                var roles = UserManager.GetRoles(userID).FirstOrDefault();
            }

            try
            {
                m1.Id = model.Id;
                m1.UserId = userID;
                m1.BusinessName = model.BusinessName;
                m1.BusinessAddress = model.BusinessAddress;
                m1.ContactPersonName = model.ContactPersonName;
                m1.ContactPersonPhoneNumber = model.ContactPersonPhoneNumber;
                m1.ContactPersonEmail = model.ContactPersonEmail;
                m1.CreatedOn = DateTime.Now;
                m1.FirstName = model.FirstName;
                m1.SurName = model.SurName;
                m1.NationalIdNumber = model.NationalIdentificationNumber;
                m1.Gender = model.Gender == null ? "" : model.Gender;
                m1.DateOfBirth = model.DateOfBirth;
                m1.ContactNumber = model.ContactNumber;
                m1.PhysicalAddress = model.PhysicalAddress;
                m1.BusinessPartnerNumber = model.BusinessPartnerNumber;
                m1.BusinessPhoneNumber = model.BusinessPhoneNumber;
                m1.Email = model.Email;
                m1.CustomerType = model.CustomerType;
                Session["CustomerDetails"] = m1;                
            }
            catch (Exception ex)
            {
                ex.GetBaseException();
            }
            if (m1 != null)
            {
                return RedirectToAction("AddProductDetails", new { id = 0 });
            }
            return View();
            //}
        }
             
        [HttpPost]
        public ActionResult CheckEmailExists(string email)
        {
            if (IsEmailExists(email))
            {
                return Json(new { exists = true, errorMessage = "Email already exists." });
            }
            else
            {
                return Json(new { exists = false });
            }
        }   

        private bool IsEmailExists(string email)
        {
            var existemail = InsuranceContext.BrokerCustomers.Single(where: $"Email='{email}'");
            return existemail != null;
        }

      

        public string updatebrokeCustomer(int id=0, int customerid=0)
        {
            var brokercustomerdata = InsuranceContext.BrokerCustomers.All().FirstOrDefault(mk => mk.Id == id);
            if (brokercustomerdata != null) 
            {
                brokercustomerdata.QuotationCustomerId = customerid;
                InsuranceContext.BrokerCustomers.Update(brokercustomerdata);
            }
           
                return "";
        }


        [HttpGet]
        //[Authorize(Roles = "Broker Manager,Broker")]
        public ActionResult AddProductDetails(listproduct model)
        {
            ViewBag.policyClasses = InsuranceContext.InsurancePolicyClasses.All().ToList();
            return View();
        }

        //[Authorize(Roles = "Broker Manager")]
        [HttpPost]
        public ActionResult AddProductDetails(PolicyProducts model)
        {
            var QuotationProductCustomerId = 0;
            try
            {
                BrokerCustomer m1 = new BrokerCustomer();
                int chkId = model.UserId;
               if (Session["CustomerDetails"] != null)
                {
                    var Customersession = (BrokerCustomer)Session["CustomerDetails"];

                    m1.UserId = Customersession.UserId;
                    m1.BusinessName = Customersession.BusinessName;
                    m1.BusinessAddress = Customersession.BusinessAddress;
                    m1.ContactPersonName = Customersession.ContactPersonName;
                    m1.ContactPersonPhoneNumber = Customersession.ContactPersonPhoneNumber;
                    m1.ContactPersonEmail = Customersession.ContactPersonEmail;
                    m1.CreatedOn = Customersession.CreatedOn;
                    m1.FirstName = Customersession.FirstName;
                    m1.isEdit = false;
                    m1.SurName = Customersession.SurName;
                    m1.NationalIdNumber = Customersession.NationalIdNumber;
                    m1.Email = Customersession.Email;
                    //m1.PolicyNumber = GenerateInvoiceNumber();
                    m1.Gender = Customersession.Gender;
                    m1.DateOfBirth = Customersession.DateOfBirth;
                    m1.IsActive = false;
                    m1.ContactNumber = Customersession.ContactNumber;
                    m1.PhysicalAddress = Customersession.PhysicalAddress;
                    m1.BusinessPartnerNumber = Customersession.BusinessPartnerNumber;
                    m1.BusinessPhoneNumber = Customersession.BusinessPhoneNumber;
                    m1.CustomerType = Customersession.CustomerType;


                    InsuranceContext.BrokerCustomers.Insert(m1);
                   //var QuotationcustomerID = QuotationCustomer(m1.Id,m1.PolicyNumber);
                    //InsuranceContext.QuotationCustomers.Insert(m1);
                    Session["Number"] = m1.Id;
                    Session["CustomerDetails"] = null;
                }


                if (m1.Id != 0 || chkId != 0)
                {
                    NonMoterProductDetail tbl = new NonMoterProductDetail();
                    tbl.CustomerId = m1.Id;
                    foreach (var item in model.PolicyProductList)
                    {
                        tbl.InsorancePolicyId = item.PolicyClassName;
                        tbl.RiskAddress = item.RiskAddress;
                        tbl.RiskcoverId = item.RskCoverName;
                        tbl.RiskDescription = item.DescriptionofRiskInsured;
                        tbl.RiskItemId = item.RiskItems;
                        tbl.Currency = item.Currency;
                        tbl.RiskRate = item.RiskRate;
                        tbl.SumInsured = item.SumInsured;
                        tbl.StampDuty = item.StampDuty;
                        tbl.PaymentTerm = item.PaymentTerm;

                        string dateStringfrom = item.PolicyValidityPeriodFrom;
                        string dateStringto = item.PolicyValidityPeriodTo;
                        DateTime dateTimeValuefrom;
                        DateTime dateTimeValueto;

                        if (DateTime.TryParse(dateStringfrom, out dateTimeValuefrom) && DateTime.TryParse(dateStringto, out dateTimeValueto))
                        {
                            tbl.PolicyValidityPeriodFrom = dateTimeValuefrom;
                            tbl.PolicyValidityPeriodTo = dateTimeValueto;
                        }

                        tbl.TotalPremiumPayable = item.TotalPremiumPayable;
                        InsuranceContext.NonMoterProductDetails.Insert(tbl);
                        
                        Session["_Productdata"] = tbl;
                        //QuotationCustomer(QuotationProductCustomerId);
                        ViewBag.policyClasses = InsuranceContext.InsurancePolicyClasses.All().ToList();
                        Session["_CustomerId"] = tbl.CustomerId;

                    }
                    var custid = tbl.CustomerId;
                    return Json(custid, JsonRequestBehavior.AllowGet);
                }
            }
            catch (Exception ex)
            {
                ex.Message.ToString();
            }

            return View();
        }

        //Add Policy Number Functionality
        public ActionResult GenerateInvoiceNumber(int id)
        {
            var brokerId = InsuranceContext.BrokerCustomers.Single(where: $"Id='{id}'");
            BrokerCustomer data = new BrokerCustomer();
            if (brokerId != null)
            {
                var year = DateTime.Now.Year;
                var sequenceNumber = GetNextSequenceNumber();
                string invoiceNumber = $"GFSC{year % 100:00}{sequenceNumber:D7}-1";
                brokerId.PolicyNumber = invoiceNumber;
                InsuranceContext.BrokerCustomers.Update(brokerId);
        }

            return RedirectToAction("PolicySchedule", new { id = id });
           
        }

        private string GetNextSequenceNumber()
        {
            var sequenceNumber = "";
            string _policynumber = null;
            var noproductdetail = InsuranceContext.BrokerCustomers.All().OrderByDescending(t => t.Id).ToList();


            string highestPolicyNumber = "";

            foreach (var record in noproductdetail)
            {
                _policynumber = record?.PolicyNumber;

                if (!string.IsNullOrEmpty(_policynumber) &&
                    string.Compare(_policynumber, highestPolicyNumber) > 0)
                {
                    highestPolicyNumber = _policynumber;

                }
            }


            if (highestPolicyNumber!="")
            {
                if (highestPolicyNumber.StartsWith("GFSC"))
                {
                    highestPolicyNumber = highestPolicyNumber.Substring(6);
                }
                string[] parts = highestPolicyNumber.Split('-');

                string trimmedNumber = parts.Length > 1 ? parts[0].Trim() : highestPolicyNumber.Trim();
                trimmedNumber = "1" + trimmedNumber;
                if (int.TryParse(trimmedNumber, out int parsedNumber))
                {

                    int incrementedNumber = parsedNumber + 1;
                    sequenceNumber = incrementedNumber.ToString();
                    sequenceNumber = incrementedNumber.ToString().Substring(1);
                    var sequenceEEENumber = sequenceNumber.ToString();
                    return sequenceEEENumber;
                }
            }
            else
            {
                sequenceNumber = "0000001";
            }

            return sequenceNumber;
        }
      

        [Authorize(Roles = "Broker Manager, Broker")]
        [HttpGet]
         public ActionResult ViewCustomer(int Id=0)
         {
            Session["Grenratebutton"] = Id;
            BrokerCustomer m1 = new BrokerCustomer();
            listquotation data = new listquotation();            
            List<BrokerCustomer> listmddel = new List<BrokerCustomer>();
            data.customerdetails = new List<QuotationItem>();
            //list = InsuranceContext..All().OrderByDescending(m => m.Id).ToList();
            //var list = "select BrokerCustomer.Id, BrokerCustomer.FirstName,BrokerCustomer.BusinessName,BrokerCustomer.BusinessPhoneNumber,BrokerCustomer.ContactNumber,BrokerCustomer.BusinessPartnerNumber,BrokerCustomer.Email,BrokerCustomer.BusinessAddress,BrokerCustomer.ContactPersonPhoneNumber,BrokerCustomer.ContactPersonEmail,BrokerCustomer.CreatedOn,BrokerCustomer.SurName,NonMoterProductDetail.RiskcoverId,NonMoterProductDetail.RiskItemId from BrokerCustomer INNER JOIN NonMoterProductDetail ON BrokerCustomer.Id =NonMoterProductDetail.CustomerId ORDER BY BrokerCustomer.Id DESC";
            var list = "select* from BrokerCustomer order by Id desc";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(list, connection))
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var polcynmber = reader["PolicyNumber"].ToString();
                        if (polcynmber !="")
                        {
                            var QuotationModel = new QuotationItem();
                            QuotationModel.Id = (int)reader["Id"];
                            QuotationModel.FirstName = reader["FirstName"].ToString();
                            QuotationModel.LastName = reader["SurName"].ToString();
                            QuotationModel.Email = reader["Email"].ToString();
                            QuotationModel.ContactNumber = reader["ContactNumber"].ToString();
                            QuotationModel.BusinessAddress = reader["BusinessAddress"].ToString();
                            QuotationModel.ContactPersonEmail = reader["ContactPersonEmail"].ToString();
                            QuotationModel.BusinessName = reader["BusinessName"].ToString();
                            QuotationModel.BusinessPhoneNumber = reader["BusinessPhoneNumber"].ToString();
                            QuotationModel.PolicyNumber = reader["PolicyNumber"].ToString();
                            QuotationModel.ContactpersonName = reader["ContactPersonName"].ToString();
                            
                            if(QuotationModel.ContactpersonName=="")
                            {
                                QuotationModel.ContactpersonName = QuotationModel.FirstName + ' ' + QuotationModel.LastName;
                            }
                            data.customerdetails.Add(QuotationModel);
                        }
                    }
                }
            }

            return View(data);
        }
        //Display customers product's policy class
        // [Authorize(Roles = "Broker Manager, Broker")]
        [HttpGet]
        //[Authorize(Roles = "Broker Manager,Broker")]
        public ActionResult ViewProducts(int Id)
        {
            listproduct policy = new listproduct();

            policy.productmodel = new List<BrokerCustomersItem>();
            var list = InsuranceContext.NonMoterProductDetails.All().Where(x => x.CustomerId == Id).ToList();
            var listName = InsuranceContext.BrokerCustomers.All().Where(x => x.Id == Id).FirstOrDefault();
            var CustomersName = listName.FirstName + " " + listName.SurName;

            foreach (var item in list)
            {
                var policyclassId = item.InsorancePolicyId;

                if (policyclassId != 0)
                {
                    var poilcyclass = InsuranceContext.InsurancePolicyClasses.All().Where(m => m.Id == policyclassId).FirstOrDefault();
                    var model = new BrokerCustomersItem();
                    model.PolicyClassName = poilcyclass.PolicyClassName;
                    model.Id = poilcyclass.Id;
                    Session["CustomerName"] = CustomersName;
                    model.customerid = Id;
                    policy.productmodel.Add(model);
                }
            }
            return View(policy);
        }
        //Display customers list
        [HttpGet]
        //[Authorize(Roles = "Broker Manager,Broker")]
        public ActionResult ViewRiskcover(int CustomerId, int Id)
        {
            listproduct policy = new listproduct();

            policy.productmodel = new List<BrokerCustomersItem>();

            var list = InsuranceContext.NonMoterProductDetails.All().Where(x => x.CustomerId == CustomerId).ToList();

            foreach (var item in list)
            {
                var RiskId = item.RiskcoverId;

                if (RiskId != 0)
                {
                    var poilcyclass = InsuranceContext.RiskCovers.All().Where(m => m.Id == RiskId).FirstOrDefault();
                    var model = new BrokerCustomersItem();
                    model.RiskcoversName = poilcyclass.RskCoverName;
                    model.Id = poilcyclass.Id;
                    model.customerid = Id;
                    policy.productmodel.Add(model);
                }
            }

            return View(policy);
        }

        [HttpGet]
        //[Authorize(Roles = "Broker Manager,Broker")]
        public ActionResult ViewRiskItem(int CustomerId, int Id)
        {
            listproduct policy = new listproduct();

            policy.productmodel = new List<BrokerCustomersItem>();

            var list = InsuranceContext.NonMoterProductDetails.All().Where(x => x.CustomerId == CustomerId).ToList();

            foreach (var item in list)
            {
                var RiskId = item.RiskItemId;
                var Riskaddress = item.RiskAddress;
                if (RiskId != 0)
                {
                    var poilcyclass = InsuranceContext.RiskItems.All().Where(m => m.Id == RiskId).FirstOrDefault();
                    var model = new BrokerCustomersItem();
                    model.RiskItemName = poilcyclass.RiskItemName;
                    model.Id = poilcyclass.Id;
                    ViewBag.custid = Id;
                    model.customerid = Id;
                    policy.productmodel.Add(model);
                }
            }

            return View(policy);
        }

        [HttpGet]
        //[Authorize(Roles = "Broker Manager,Broker")]
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

        [HttpGet]
        public ActionResult GenerateQuotation(int Id)
        {
            var brokercustomerId = InsuranceContext.BrokerCustomers.All().FirstOrDefault(bi => bi.Id == Id);
            if (brokercustomerId != null)
            {
                var IsQuotationId = brokercustomerId.QuotationCustomerId;
                if (IsQuotationId != 0)
                {
                    Id = IsQuotationId;
                }
            }
            var userID = "";
            BrokerCustomer m1 = new BrokerCustomer();
            bool userLoggedin = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
            if (userLoggedin)
            {
                userID = System.Web.HttpContext.Current.User.Identity.GetUserId();
                var roles = UserManager.GetRoles(userID).FirstOrDefault();
            }
            var Username = InsuranceContext.Customers.All().Where(m => m.UserID == userID).FirstOrDefault();

            var LoggedInUser = $"{Username.FirstName} {Username.LastName}";
            listquotation data = new listquotation();
            // QuotationItem model = new QuotationItem();  
            data.customerdetails = new List<QuotationItem>();
            //var ExistingCustomerId = InsuranceContext.QuotationCustomers.All().Where(m => m.ExistingCustomerId == Id).FirstOrDefault();
            //if (ExistingCustomerId != null)
            //{
            //    Id = ExistingCustomerId.Id;
            //}
            var query = "select Brokercustomer.FirstName,Brokercustomer.BusinessAddress, Brokercustomer.BusinessPhoneNumber, Brokercustomer.BusinessName,Brokercustomer.Id,Brokercustomer.SurName,Brokercustomer.Email,Brokercustomer.ContactNumber,Brokercustomer.PhysicalAddress,NonMoterProductDetail.InsorancePolicyId,NonMoterProductDetail.RiskcoverId,NonMoterProductDetail.RiskItemId,NonMoterProductDetail.RiskDescription,NonMoterProductDetail.RiskDescription,NonMoterProductDetail.RiskAddress, NonMoterProductDetail.SumInsured,NonMoterProductDetail.RiskRate,NonMoterProductDetail.PaymentTerm,NonMoterProductDetail.StampDuty,NonMoterProductDetail.TotalPremiumPayable,NonMoterProductDetail.Currency from Brokercustomer INNER JOIN NonMoterProductDetail ON Brokercustomer.Id =NonMoterProductDetail.CustomerId where  Brokercustomer.Id ='" + Id + "'";
            decimal TotalPremium = 0;
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();

                using (SqlCommand command = new SqlCommand(query, connection))
                using (SqlDataReader reader = command.ExecuteReader())
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

                    while (reader.Read())
                    {
                        var QuotationModel = new QuotationItem();
                        data.Id = (int)reader["Id"];
                        data.FirstName = reader["FirstName"].ToString();
                        data.LastName = reader["SurName"].ToString();
                        data.Email = reader["Email"].ToString();
                        data.BusinessName = reader["BusinessName"].ToString();
                        data.ContactNumber = reader["ContactNumber"].ToString();
                        if (data.ContactNumber == "")
                        {
                            data.ContactNumber = reader["BusinessPhoneNumber"].ToString();
                        }
                        data.PhysicalAddress = reader["PhysicalAddress"].ToString();
                        data.BusinessAddress = reader["BusinessAddress"].ToString();
                        var policy = reader["InsorancePolicyId"].ToString();
                        int PolicyId = Convert.ToInt32(policy);
                        var Riskcover = reader["RiskcoverId"].ToString();
                        int CoverId = Convert.ToInt32(Riskcover);
                        var RiskItem = reader["RiskItemId"].ToString();
                        int ItemId = Convert.ToInt32(RiskItem);
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
                        QuotationModel.RiskDescription = reader["RiskDescription"].ToString();
                        QuotationModel.Currency = reader["Currency"].ToString();
                        ViewBag.Currency = QuotationModel.Currency;
                        QuotationModel.TotalPremiumPayable = reader["TotalPremiumPayable"].ToString();
                        decimal sumInsuredValue;
                        decimal riskRateValue;
                        decimal basicPremium = 0;
                       

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
                                    monthlystampduty  = Convert.ToDecimal(QuotationModel.StampDuty);
                                    totbasicpremium = basicPremium - monthlystampduty;
                                    break;
                            }

                            QuotationModel.BasicPremium = Convert.ToDecimal(totbasicpremium.ToString("N2"));
                            TotalPremium += QuotationModel.BasicPremium;
                            TotAnualstampduty += Anualstampduty;
                            TotQuarterlystampduty += Quarterlystampduty;
                            Totmonthlystampduty += monthlystampduty;
                            Tottermlystampduty += termlystampduty;

                            if (QuotationModel.PaymentTerm == "Annual")
                            {
                                data.Annualtotalpayble = Convert.ToDouble(TotalPremium);
                                QuotationModel.AnnualPremium = QuotationModel.BasicPremium;
                                
                            }
                            else if (QuotationModel.PaymentTerm == "Quarterly")
                            {
                                data.Quarterlytotalpayble = Convert.ToDouble(TotalPremium);
                                QuotationModel.QuarterlyPremium = QuotationModel.BasicPremium;
                            }
                            else if (QuotationModel.PaymentTerm == "Termly")
                            {
                                data.Termlytotalpayble = Convert.ToDouble(TotalPremium);
                                QuotationModel.TerminalPremium = QuotationModel.BasicPremium;
                            }
                            else if (QuotationModel.PaymentTerm == "Monthly")
                            {
                                data.Monthlytotalpayble = Convert.ToDouble(TotalPremium);
                                QuotationModel.MonthlyPremium = QuotationModel.BasicPremium;
                            }

                           
                        }

                        data.customerdetails.Add(QuotationModel);
                    }


                    data.Annualstampvalue =Convert.ToDouble(TotAnualstampduty);
                    data.Quarterlystampvalue = Convert.ToDouble(TotQuarterlystampduty);
                    data.Monthlystampvalue = Convert.ToDouble(Totmonthlystampduty);
                    data.Termlystampvalue = Convert.ToDouble(Tottermlystampduty);

                }
            }
            data.LoggedInUser = LoggedInUser;
            ViewBag.data = data;
            ViewBag.UserId = Id;
            return View(data);
        }

        [HttpGet]
        public ActionResult ViewQuotations()
        {
            
            BrokerCustomer m1 = new BrokerCustomer();
            listquotation data = new listquotation();
            List<BrokerCustomer> listmddel = new List<BrokerCustomer>();
            data.customerdetails = new List<QuotationItem>();
            //list = InsuranceContext..All().OrderByDescending(m => m.Id).ToList();
            //var list = "select QuotationCustomer.Id, QuotationCustomer.FirstName,QuotationCustomer.ContactNumber,QuotationCustomer.EmailAddress,QuotationCustomer.LastName,QuotationProduct.RiskcoverId,QuotationProduct.RiskItemId from QuotationCustomer INNER JOIN QuotationProduct ON QuotationCustomer.Id =QuotationProduct.CustomerId ORDER BY QuotationCustomer.Id DESC";
            var list = "select * from Brokercustomer order by Id desc";
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlCommand command = new SqlCommand(list, connection))
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var QuotationModel = new QuotationItem();
                        QuotationModel.Id = (int)reader["Id"];
                        QuotationModel.FirstName = reader["FirstName"].ToString();
                        QuotationModel.LastName = reader["SurName"].ToString();
                        QuotationModel.Email = reader["Email"].ToString();
                        QuotationModel.ContactNumber = reader["ContactNumber"].ToString();
                        QuotationModel.BusinessAddress = reader["BusinessAddress"].ToString();
                        QuotationModel.ContactPersonEmail = reader["ContactPersonEmail"].ToString();
                        QuotationModel.BusinessName = reader["BusinessName"].ToString();
                        QuotationModel.ContactpersonName = reader["ContactPersonName"].ToString();
                        QuotationModel.BusinessPhoneNumber = reader["BusinessPhoneNumber"].ToString();
                        data.customerdetails.Add(QuotationModel);
                    }
                }
            }
            return View(data);
        }

        [HttpGet]
        public ActionResult QuotationDetails(int Id)

        {
            listquotation data = new listquotation();
            data.customerdetails = new List<QuotationItem>();
            decimal TotalPremium = 0;
            var productList = InsuranceContext.NonMoterProductDetails.All().Where(x => x.CustomerId == Id).ToList();
            var customerList = InsuranceContext.BrokerCustomers.All().Where(m => m.Id == Id).ToList();
            var customer = customerList[0];
            data.Id = customer.Id;

            data.FirstName = customer.FirstName;
            data.LastName = customer.SurName;
            data.ContactPersonName = customer.ContactPersonName;
            data.PhysicalAddress = customer.PhysicalAddress;
            data.ContactNumber = customer.ContactNumber;
            data.Email = customer.Email;
            data.BusinessName = customer.BusinessName;
            data.BusinessAddress = customer.BusinessAddress;
            data.EmailAddess = customer.ContactPersonEmail;
            data.BusinessPhoneNumber = customer.BusinessPhoneNumber;
           
            foreach (var item in productList)
            {
                var model = new QuotationItem();
                model.Id = item.Id;
                var policyId = item.InsorancePolicyId;
                var coverId = item.RiskcoverId;
                var itemId = item.RiskItemId;
                var policyname = InsuranceContext.InsurancePolicyClasses.All().Where(x => x.Id == policyId).FirstOrDefault();
                var policyClassNames = policyname.PolicyClassName;
                var covername = InsuranceContext.RiskCovers.All().Where(x => x.Id == coverId).FirstOrDefault();
                var RiskCoverName = covername.RskCoverName;
                var itemname = InsuranceContext.RiskItems.All().Where(x => x.Id == itemId).FirstOrDefault();
                var RiskItemName = itemname.RiskItemName;
                model.PolicyClassName = policyClassNames;
                model.RiskCoverName = RiskCoverName;
                model.RiskItemName = RiskItemName;
                model.RiskCoverId = Convert.ToString(coverId);
                model.RiskAddress = item.RiskAddress;
                model.SumInsured = item.SumInsured;
                model.PaymentTerm = item.PaymentTerm;
                model.StampDuty = item.StampDuty;
                model.RiskRate = item.RiskRate;
                model.PolicyValidityPeriodFrom = item.PolicyValidityPeriodFrom.ToString("yyyy-MM-dd");
                model.PolicyValidityPeriodTo = item.PolicyValidityPeriodTo.ToString("yyyy-MM-dd");

                model.Currency = item.Currency;
                ViewBag.Currency = item.Currency;

                decimal sumInsuredValue;
                decimal riskRateValue;
                decimal basicPremium = 0;

                if (decimal.TryParse(model.SumInsured, out sumInsuredValue) &&
                    decimal.TryParse(model.RiskRate, out riskRateValue))
                {
                    switch (model.PaymentTerm)
                    {
                        case "Annual":
                            basicPremium = sumInsuredValue * riskRateValue / 100;
                            break;

                        case "Quarterly":
                            basicPremium = (sumInsuredValue * riskRateValue / 100) / 3;
                            break;

                        case "Termly":
                            basicPremium = (sumInsuredValue * riskRateValue / 100) / 4;
                            break;

                        case "Monthly":
                            basicPremium = (sumInsuredValue * riskRateValue / 100) / 12;
                            break;
                    }
                    model.TotalPremiumPayable = basicPremium.ToString("0.00");
                    model.BasicPremium = Convert.ToDecimal(basicPremium.ToString("N2"));
                    TotalPremium += model.BasicPremium;

                    if (model.PaymentTerm == "Annual")
                    {
                        data.Annualtotalpayble = Convert.ToDouble(TotalPremium);
                        model.AnnualPremium = model.BasicPremium;

                    }
                    else if (model.PaymentTerm == "Quarterly")
                    {
                        data.Quarterlytotalpayble = Convert.ToDouble(TotalPremium);
                        model.QuarterlyPremium = model.BasicPremium;
                    }
                    else if (model.PaymentTerm == "Termly")
                    {
                        data.Termlytotalpayble = Convert.ToDouble(TotalPremium);
                        model.TerminalPremium = model.BasicPremium;
                    }
                    else if (model.PaymentTerm == "Monthly")
                    {
                        data.Monthlytotalpayble = Convert.ToDouble(TotalPremium);
                        model.MonthlyPremium = model.BasicPremium;
                    }
                }
                data.PolicyValidityPeriodFrom = model.PolicyValidityPeriodFrom;
                data.PolicyValidityPeriodTo = model.PolicyValidityPeriodTo;
                
                data.customerdetails.Add(model);
            }
            

            data.Annualstampvalue = Math.Round(data.Annualtotalpayble * 0.05, 2);
            data.Quarterlystampvalue = Math.Round(data.Quarterlytotalpayble * 0.05, 2);
            data.Termlystampvalue = Math.Round(data.Termlytotalpayble * 0.05, 2);
            data.Monthlystampvalue = Math.Round(data.Monthlytotalpayble * 0.05, 2);
            ViewBag.Quotation = data;
            return View(data);
        }
        [HttpPost]
        public JsonResult UpdateDate(int id, string fromDate, string toDate)
        {
            if (id == 0)
            {
                return Json(new { success = false, message = "Invalid policy ID." });
            }

            try
            {
                var policies = InsuranceContext.NonMoterProductDetails.All().Where(m => m.CustomerId == id).ToList();
                if (policies.Any())
                {
                    DateTime fromDateParsed;
                    DateTime toDateParsed;
                    if (!DateTime.TryParseExact(fromDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out fromDateParsed) ||
                        !DateTime.TryParseExact(toDate, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out toDateParsed))
                    {
                        return Json(new { success = false, message = "Invalid date format. Please provide dates in yyyy-MM-dd format." });
                    }

                    foreach (var policy in policies)
                    {
                        policy.PolicyValidityPeriodFrom = fromDateParsed;
                        policy.PolicyValidityPeriodTo = toDateParsed;
                        InsuranceContext.NonMoterProductDetails.Update(policy);
                    }
                    var brokercustomer = InsuranceContext.BrokerCustomers.Single(where: $"Id ='{id}'");
                    if (brokercustomer != null)
                    {
                        brokercustomer.IsActive = true;
                        InsuranceContext.BrokerCustomers.Update(brokercustomer);
                    }

                    return Json(new { success = true });
                }
                else
                {
                    return Json(new { success = false, message = $"No policies found for customer ID {id}." });
                }


                
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"An error occurred: {ex.Message}" });
            }
        }

        [HttpGet]
        public ActionResult PolicySchedule(int id)
        {
            listquotation data = new listquotation();
            data.customerdetails = new List<QuotationItem>();
            var custdetail = InsuranceContext.BrokerCustomers.All().Where(c => c.Id == id).FirstOrDefault();
            var model = new QuotationItem();
            var list = InsuranceContext.NonMoterProductDetails.All().Where(x => x.CustomerId == id).ToList();
            //GenerateQuotation generateQuotation = new GenerateQuotation();
           
            double sum = 0.00;
            double totalpremium = 0.00;
            foreach (var items in list)
            {
                var QuotationModel = new QuotationItem();
                var policy = items.InsorancePolicyId;
                var Riskcover = items.RiskcoverId;
                var RiskItem = items.RiskItemId;
                var policyclass = InsuranceContext.InsurancePolicyClasses.All().Where(p => p.Id == policy).FirstOrDefault();
                QuotationModel.PolicyClassName = policyclass.PolicyClassName;
                var cover = InsuranceContext.RiskCovers.All().Where(r => r.Id == Riskcover).FirstOrDefault();
                QuotationModel.RiskCoverName = cover.RskCoverName;
                var RiskitemId = InsuranceContext.RiskItems.All().Where(r => r.Id == RiskItem).FirstOrDefault();
                QuotationModel.RiskItemName = RiskitemId.RiskItemName;
                QuotationModel.RiskAddress = items.RiskAddress;
                string Todate = items.PolicyValidityPeriodTo.ToShortDateString();
                QuotationModel.PolicyValidityPeriodTo = Todate;
                string Fromdate = items.PolicyValidityPeriodFrom.ToShortDateString();
                QuotationModel.PolicyValidityPeriodFrom = Fromdate;
                QuotationModel.StampDuty = items.StampDuty;
                QuotationModel.Currency = items.Currency;
                QuotationModel.RiskDescription = items.RiskDescription;
                QuotationModel.PaymentTerm = items.PaymentTerm;
                QuotationModel.SumInsured = items.SumInsured;
                QuotationModel.TotalPremiumPayable = items.TotalPremiumPayable;
                QuotationModel.RiskRate = items.RiskRate;
                QuotationModel.PaymentTerm = items.PaymentTerm;
                ViewBag.Currency = items.Currency;
                sum += Convert.ToDouble(items.SumInsured);
                totalpremium += Convert.ToDouble(items.TotalPremiumPayable);
                QuotationModel.TotalSumInsured = sum;
                QuotationModel.Total = totalpremium;
                ViewBag.Totalsum = QuotationModel.TotalSumInsured;
                ViewBag.Toalpre = QuotationModel.Total;
                data.customerdetails.Add(QuotationModel);
            }
            data.CustomerName = $"{custdetail.FirstName} {custdetail.SurName}";
            data.BusinessAddress = custdetail.BusinessAddress;
            data.BusinessName = custdetail.BusinessName;

            data.PhysicalAddress = custdetail.PhysicalAddress;
            data.PolicyNumber = custdetail.PolicyNumber;
            data.BusinessPartnerNumber = custdetail.BusinessPartnerNumber;
            ViewBag.details = data;
            return View(data);
        }

        [HttpGet]
        public ActionResult EditCustomer(int id)
        {
            var editcustomer = InsuranceContext.GenerateQuotations.All().Where(m => m.Id == id).FirstOrDefault();
            return View(editcustomer);
        }

        [HttpPost]
        public ActionResult EditCustomer()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Customerquotationdetails(QuotationCustomerDetails model)
        {
            try
            {
                var customerId = model.Id;
                var lastname = model.LastName;
                var customerdetails = InsuranceContext.BrokerCustomers.Single(where: $"Id ='{customerId}'");
                if (lastname != null)
                {
                    customerdetails.FirstName = model.FirstName;
                    customerdetails.SurName = model.LastName;
                }
                else
                {
                    customerdetails.ContactPersonName = model.FirstName;
                }
                customerdetails.Email = model.Email;
                customerdetails.PhysicalAddress = model.Address;               
                customerdetails.ContactNumber = model.ContactNumber;
                customerdetails.isEdit = false;
                InsuranceContext.BrokerCustomers.Update(customerdetails);

                
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return Json(JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult EditGeneratQuotaion(QuotationDetail model)
        {
            NonMoterProductDetail product = new NonMoterProductDetail();

            var productdetails = InsuranceContext.NonMoterProductDetails.All().Where(x => x.Id == model.id).FirstOrDefault();
            try
            {
                if (model.PaymentTerm != null)
                {
                    var policyname = model.PolicyClass;
                    var covername = model.RiskCoverClass;
                    var itemname = model.RiskItemClass;

                    var PolicyId = InsuranceContext.InsurancePolicyClasses.All().Where(p => p.PolicyClassName.Trim() == policyname).FirstOrDefault().Id;
                    var CoverId = InsuranceContext.RiskCovers.All().Where(c => c.RskCoverName.Trim() == covername).FirstOrDefault().Id;
                    var RiskId = InsuranceContext.RiskItems.All().Where(i => i.RiskItemName.Trim() == itemname).FirstOrDefault().Id;
                   
                    product.Id = model.id;
                    product.CustomerId = model.customerId;
                    product.InsorancePolicyId = PolicyId;
                    product.RiskcoverId = CoverId;
                    product.RiskItemId = RiskId;
                    product.RiskAddress = model.RiskAddress;
                    product.RiskDescription = productdetails.RiskDescription;
                    product.StampDuty = productdetails.StampDuty;
                    product.SumInsured = model.SumInsured;
                    product.TotalPremiumPayable = model.TotalPayable;
                    product.PaymentTerm = model.PaymentTerm;
                    product.Currency = productdetails.Currency;
                    product.PolicyValidityPeriodTo = productdetails.PolicyValidityPeriodTo;
                    product.PolicyValidityPeriodFrom = productdetails.PolicyValidityPeriodFrom;
                    updateIsEdit(product.CustomerId);
                    product.RiskRate = model.RiskRate;
                    InsuranceContext.NonMoterProductDetails.Update(product);
                    updateIsEdit(product.CustomerId);
                }
            }
            catch(Exception ex)
            {
                throw ex;
            }
            return Json(JsonRequestBehavior.AllowGet);
        }

        public string updateIsEdit(int id)
        {
            var QuotationCustomerData = InsuranceContext.BrokerCustomers.All().FirstOrDefault(m=>m.Id==id);


            QuotationCustomerData.isEdit = false;
            InsuranceContext.BrokerCustomers.Update(QuotationCustomerData);
            return "";
        }

        [HttpGet]
        public JsonResult GetRiskItems(int selectedValue)
        {
            var data = InsuranceContext.RiskCovers.All().ToList();
            var req = data.FirstOrDefault(item => item.Id == selectedValue)?.Id;
            ///var rskcvr = InsuranceContext.RiskItems.All().Where(x => x.Fk_RskCoverID == req).ToList();
            List<string> obj = new List<string>();
            var rskcvr = InsuranceContext.RiskItems.All().Where(x => x.Fk_RskCoverID == req)
                .Select(a => new
                {
                    RiskItemName = a.RiskItemName,
                    Id = a.Id
                });
            return Json(rskcvr, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public JsonResult GetRiskItemswithoutpara()
        {

            ///var rskcvr = InsuranceContext.RiskItems.All().Where(x => x.Fk_RskCoverID == req).ToList();
            List<string> obj = new List<string>();
            var rskcvr = InsuranceContext.RiskItems.All()
                .Select(a => new
                {
                    RiskItemName = a.RiskItemName,
                    Id = a.Id
                });
            return Json(rskcvr, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public JsonResult GetRiskCoversItems(int selectedValue)
        {
            //var data = InsuranceContext.RiskCovers.All().ToList();
            //var req = data.FirstOrDefault(item => item.Id == selectedValue)?.Id;
            ///var rskcvr = InsuranceContext.RiskItems.All().Where(x => x.Fk_RskCoverID == req).ToList();
            List<string> obj = new List<string>();
            var rskcvr = InsuranceContext.RiskCovers.All().Where(x => x.Fk_Ins_plcyID == selectedValue)
                .Select(a => new
                {
                    RiskCoverName = a.RskCoverName,
                    Id = a.Id
                });
            return Json(rskcvr, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        public ActionResult AddPolicyClass(Pclass pclassData)
        {
            if (pclassData.Name != null)
            {
                InsurancePolicyClass tbl = new InsurancePolicyClass();
                //tbl.PolicyClassName = pclassData.Name;
                //InsuranceContext.InsurancePolicyClasses.Insert(tbl);
                //var datapclass = InsuranceContext.InsurancePolicyClasses.All().ToList();
                ViewBag.Riskcovers = InsuranceContext.InsurancePolicyClasses.All().ToList();
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


        [HttpPost]
        public ActionResult SavePdf(string pdfDataUri, string fileName)
        {
            try
            {                
                var base64Data = pdfDataUri.Replace("data:application/pdf;base64,", "");
                var pdfBytes = Convert.FromBase64String(base64Data);
                var folderPath = Server.MapPath("~/PdfFolder/");
                var filePath = Path.Combine(folderPath, fileName);
                if (!Directory.Exists(folderPath))
                {
                    Directory.CreateDirectory(folderPath);
                }

                string[] files = Directory.GetFiles(folderPath);
                foreach (var existingFilePath in files)
                {
                    System.IO.File.Delete(existingFilePath);
                }

                System.IO.File.WriteAllBytes(filePath, pdfBytes);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Failed to save the PDF: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult SendPdfEmail(int id, string pdfDataUri, string fileName)
        {
            var pdfFileName = fileName;
            try
            {
                Insurance.Service.EmailService objEmailService = new Insurance.Service.EmailService();
                //string pdfFileName = "QA_Test.pdf";  // PDF file name
                string pdfVirtualPath = $"~/PdfFolder/{fileName}";  // Virtual path to the PDF file
                string pdfPhysicalPath = Server.MapPath(pdfVirtualPath);  // Physical path to the PDF file

                var Email = InsuranceContext.QuotationCustomers.All().Where(x => x.Id == id).FirstOrDefault();

                if (!System.IO.File.Exists(pdfPhysicalPath))
                {
                    return Json(new { success = false, message = "PDF file not found." }, JsonRequestBehavior.AllowGet);
                }

                byte[] pdfBytes = System.IO.File.ReadAllBytes(pdfPhysicalPath);

                List<string> attachments = new List<string>
            {
                pdfVirtualPath // The path to the PDF file you want to attach
            };

                // Create a new email message
                MailMessage mailMessage = new MailMessage();
                mailMessage.Subject = "PDF Report";
                mailMessage.Body = "Please find the PDF report attached.";
                string toEmail = Email.EmailAddress;
                if (toEmail == null)
                {
                    toEmail = Email.EmailAddress;
                }

                // Attach the PDF file to the email message
                MemoryStream pdfStream = new MemoryStream(pdfBytes);
                Attachment pdfAttachment = new Attachment(pdfStream, pdfFileName, "application/pdf");
                mailMessage.Attachments.Add(pdfAttachment);


                objEmailService.SendEmail(toEmail, "", "", mailMessage.Subject, mailMessage.Body, attachments);


                //FileInfo file = new FileInfo(pdfVirtualPath);
                //file.Delete();
                return Json(new { success = true, message = "Email sent successfully." }, JsonRequestBehavior.AllowGet);

            }

            catch (Exception ex)
            {
                return Json(new { success = false, message = "Email sending failed: " + ex.Message }, JsonRequestBehavior.AllowGet);
            }


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
                var Email = InsuranceContext.BrokerCustomers.All().Where(x => x.Id == id).FirstOrDefault();
                if (!System.IO.File.Exists(pdfPhysicalPath))
                {
                    return Json(new { success = false, message = "PDF file not found." }, JsonRequestBehavior.AllowGet);
                }

                byte[] pdfBytes = System.IO.File.ReadAllBytes(pdfPhysicalPath);

                List<string> attachments = new List<string>
            {
                pdfVirtualPath // The path to the PDF file you want to attach
            };

                // Create a new email message
                MailMessage mailMessage = new MailMessage();
                mailMessage.Subject = "PDF Report";
                mailMessage.Body = "Please find the PDF report attached.";
                string toEmail = Email.Email;
                if (toEmail == null)
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

        //[HttpGet]
        //public ActionResult ExistingGenerateQuotation(int Id)
        //{
        //    var userID = "";

        //    BrokerCustomer m1 = new BrokerCustomer();

        //    bool userLoggedin = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;
        //    if (userLoggedin)
        //    {
        //        userID = System.Web.HttpContext.Current.User.Identity.GetUserId();
        //        var roles = UserManager.GetRoles(userID).FirstOrDefault();
        //    }
        //    var Username = InsuranceContext.Customers.All().Where(m => m.UserID == userID).FirstOrDefault();
        //    var LoggedInUser = $"{Username.FirstName} {Username.LastName}";
        //    listquotation data = new listquotation();          
        //    data.customerdetails = new List<QuotationItem>();
        //    var chechekNewcustomertbl = InsuranceContext.QuotationCustomers.All().Where(m => m.ExistingCustomerId == Id).FirstOrDefault();
        //    if (chechekNewcustomertbl != null)
        //    {
        //        return RedirectToAction("GenerateQuotation", new { id = Id, });
        //    }
        //    else
        //    {
        //        var query = "select NonMoterProductDetail.CustomerId,BrokerCustomer.FirstName,BrokerCustomer.SurName,BrokerCustomer.ContactNumber,BrokerCustomer.PhysicalAddress,BrokerCustomer.Email,BrokerCustomer.BusinessName,BrokerCustomer.BusinessAddress,BrokerCustomer.BusinessPartnerNumber,BrokerCustomer.ContactPersonEmail, NonMoterProductDetail.InsorancePolicyId,NonMoterProductDetail.RiskcoverId,NonMoterProductDetail.RiskItemId,NonMoterProductDetail.RiskAddress,NonMoterProductDetail.RiskDescription,NonMoterProductDetail.SumInsured,NonMoterProductDetail.RiskRate,NonMoterProductDetail.PaymentTerm,NonMoterProductDetail.StampDuty,NonMoterProductDetail.TotalPremiumPayable,NonMoterProductDetail.Currency from BrokerCustomer INNER JOIN NonMoterProductDetail ON BrokerCustomer.Id = NonMoterProductDetail.CustomerId where CustomerId = @Id";
        //        double TotalPremium = 0.00;
        //        using (SqlConnection connection = new SqlConnection(connectionString))
        //        {
        //            connection.Open();

        //            using (SqlCommand command = new SqlCommand(query, connection))
        //            {
        //                command.Parameters.AddWithValue("@Id", Id);
        //                using (SqlDataReader reader = command.ExecuteReader())
        //                {
        //                    while (reader.Read())
        //                    {
        //                        var QuotationModel = new QuotationItem();
        //                        data.FirstName = reader["FirstName"].ToString();
        //                        data.LastName = reader["SurName"].ToString();
        //                        data.Email = reader["Email"].ToString();
        //                        data.ContactNumber = reader["ContactNumber"].ToString();
        //                        data.PhysicalAddress = reader["PhysicalAddress"].ToString();
        //                        data.BusinessName = reader["BusinessName"].ToString();
        //                        data.BusinessAddress = reader["BusinessAddress"].ToString();
        //                        data.BusinessPartnerNumber = reader["BusinessPartnerNumber"].ToString();
        //                        data.ContactPersonEmail = reader["ContactPersonEmail"].ToString();
        //                        var policy = reader["InsorancePolicyId"].ToString();
        //                        int PolicyId = Convert.ToInt32(policy);
        //                        var Riskcover = reader["RiskcoverId"].ToString();
        //                        int CoverId = Convert.ToInt32(Riskcover);
        //                        var RiskItem = reader["RiskItemId"].ToString();
        //                        int ItemId = Convert.ToInt32(RiskItem);
        //                        var policyclass = InsuranceContext.InsurancePolicyClasses.All().Where(p => p.Id == PolicyId).FirstOrDefault();
        //                        QuotationModel.PolicyClassName = policyclass.PolicyClassName;
        //                        var cover = InsuranceContext.RiskCovers.All().Where(r => r.Id == CoverId).FirstOrDefault();
        //                        QuotationModel.RiskCoverName = cover.RskCoverName;
        //                        var RiskitemId = InsuranceContext.RiskItems.All().Where(r => r.Id == ItemId).FirstOrDefault();
        //                        QuotationModel.RiskItemName = RiskitemId.RiskItemName;
        //                        QuotationModel.RiskAddress = reader["RiskAddress"].ToString();
        //                        QuotationModel.SumInsured = reader["SumInsured"].ToString();
        //                        QuotationModel.StampDuty = reader["StampDuty"].ToString();
        //                        QuotationModel.RiskRate = reader["RiskRate"].ToString();
        //                        QuotationModel.PaymentTerm = reader["PaymentTerm"].ToString();
        //                        QuotationModel.Currency = reader["Currency"].ToString();
        //                        QuotationModel.TotalPremiumPayable = reader["TotalPremiumPayable"].ToString();
        //                        decimal sumInsuredValue;
        //                        decimal riskRateValue;
        //                        decimal basicPremium = 0;

        //                        if (decimal.TryParse(QuotationModel.SumInsured, out sumInsuredValue) &&
        //                            decimal.TryParse(QuotationModel.RiskRate, out riskRateValue))
        //                        {
        //                            switch (QuotationModel.PaymentTerm)
        //                            {
        //                                case "Annual":
        //                                    basicPremium = sumInsuredValue * riskRateValue / 100;
        //                                    break;

        //                                case "Quarterly":
        //                                    basicPremium = (sumInsuredValue * riskRateValue / 100) / 3;
        //                                    break;

        //                                case "Termly":
        //                                    basicPremium = (sumInsuredValue * riskRateValue / 100) / 4;
        //                                    break;

        //                                case "Monthly":
        //                                    basicPremium = (sumInsuredValue * riskRateValue / 100) / 12;
        //                                    break;
        //                            }

        //                            QuotationModel.BasicPremium = Convert.ToDouble(basicPremium.ToString("N2"));
        //                            TotalPremium += QuotationModel.BasicPremium;

        //                            if (QuotationModel.PaymentTerm == "Annual")
        //                            {
        //                                data.Annualtotalpayble = Convert.ToDouble(TotalPremium);
        //                                QuotationModel.AnnualPremium = QuotationModel.BasicPremium;
        //                            }
        //                            else if (QuotationModel.PaymentTerm == "Quarterly")
        //                            {
        //                                data.Quarterlytotalpayble = Convert.ToDouble(TotalPremium);
        //                                QuotationModel.QuarterlyPremium = QuotationModel.BasicPremium;
        //                            }
        //                            else if (QuotationModel.PaymentTerm == "Termly")
        //                            {
        //                                data.Termlytotalpayble = Convert.ToDouble(TotalPremium);
        //                                QuotationModel.TerminalPremium = QuotationModel.BasicPremium;
        //                            }
        //                            else if (QuotationModel.PaymentTerm == "Monthly")
        //                            {
        //                                data.Monthlytotalpayble = Convert.ToDouble(TotalPremium);
        //                                QuotationModel.MonthlyPremium = QuotationModel.BasicPremium;
        //                            }
        //                        }
        //                        ViewBag.currency = QuotationModel.Currency;
        //                        data.customerdetails.Add(QuotationModel);
        //                    }
        //                    data.Id = Id;
        //                    data.Annualstampvalue = Math.Round(data.Annualtotalpayble * 0.05, 2);
        //                    data.Quarterlystampvalue = Math.Round(data.Quarterlytotalpayble * 0.05, 2);
        //                    data.Termlystampvalue = Math.Round(data.Termlytotalpayble * 0.05, 2);
        //                    data.Monthlystampvalue = Math.Round(data.Monthlytotalpayble * 0.05, 2);
        //                }
        //            }
        //        }
        //        data.LoggedInUser = LoggedInUser;
        //        ViewBag.data = data;

        //        ViewBag.UserId = Id;

        //        return View(data);
        //    }

        //}
        [HttpPost]
        public ActionResult DeleteProduct(string invoiceNumber)
        {
            // Validate and parse the invoiceNumber
            if (!int.TryParse(invoiceNumber, out int id))
            {
                return Json(new { success = false, error = "Invalid invoice number format" });
            }

            try
            {
                // Attempt to find and delete the record
                var invoice = InsuranceContext.NonMoterProductDetails.Single(where: $"Id = {id}");
                if (invoice != null)
                { 
                    InsuranceContext.NonMoterProductDetails.Delete(invoice);
                }
                else
                {
                    var invoiceproduct = InsuranceContext.QuotationProducts.Single(where: $"Id = {id}");
                    if (invoiceproduct != null)
                    {
                        InsuranceContext.QuotationProducts.Delete(invoiceproduct);
                    }
                    else
                    {
                        return Json(new { success = false, error = "Record not found" });
                    }
                }

                // Return a JSON result indicating success
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                return Json(new { success = false, error = "An error occurred while deleting the record" });
            }
        }


    }
}

