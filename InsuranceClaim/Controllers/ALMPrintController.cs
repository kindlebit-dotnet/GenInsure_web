using Insurance.Domain;
using InsuranceClaim.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PagedList.Mvc;
using PagedList;
using System.Web.Mvc;

namespace InsuranceClaim.Controllers
{
    public class ALMPrintController : Controller
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
        // GET: ALMPrint
        public ActionResult Index(int? i)
        {
            string query = "select ALMRePrint.Id, ALMRePrint.VRN, ALMRePrint.OTP,ALMRePrint.PolicyId,ALMRePrint.CreatedOn, ALMRePrint.CreatedBy from ALMRePrint ";
            var result = InsuranceContext.Query(query).Select(x => new ALMRePrintModel()
            {
                Id = Convert.ToInt32(x.Id),
                VRN = x.VRN,
                OTP = x.OTP,
                CreatedOn = x.CreatedOn,
                PolicyId = x.PolicyId,
                CreatedBy = x.CreatedBy
            }).OrderByDescending(x => x.Id).ToList();
            return View(result);
        }

        public ActionResult Create()
        {
            ALMRePrintModel model = new ALMRePrintModel();
            Random generator = new Random();        
            model.OTP= Convert.ToInt32(generator.Next(0, 1000000).ToString("D6"));
            model.CreatedBy = LoggedCustomerId();
            model.CreatedOn = DateTime.Now;
            return View(model);
        }

        public int? LoggedCustomerId()
        {
            int customerId = 1;
            bool _userLoggedin = (System.Web.HttpContext.Current.User != null) && System.Web.HttpContext.Current.User.Identity.IsAuthenticated;

            if (_userLoggedin)
            {
                var _User = UserManager.FindById(User.Identity.GetUserId().ToString());
                var _customerData = InsuranceContext.Customers.All(where: $"UserId ='{_User.Id}'").FirstOrDefault();

                if (_customerData != null)
                {
                    customerId = _customerData.Id;
                }
            }
            return customerId;
        }

        [HttpPost]
        public ActionResult Create(ALMRePrintModel model)
        {
            if(model.VRN!=null && model.OTP!=null)
            {
             var count=   Math.Floor(Math.Log10(model.OTP) + 1);

                if (count==6)
                {
                    ALMRePrint reprint = new ALMRePrint { VRN = model.VRN, OTP = model.OTP, CreatedOn = DateTime.Now };
                    InsuranceContext.ALMRePrints.Insert(reprint);
                }
                else
                {
                    TempData["errorMsg"] = "OTP should be 6 digit number.";
                }              
            }
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int id=0)
        {
            ALMRePrintModel model = new ALMRePrintModel();
            var details = InsuranceContext.ALMRePrints.Single(id );
            if(details!=null)
            {
                model.Id = details.Id;
                model.VRN = details.VRN;
                model.OTP = details.OTP;
            }
            return View(model);
        }




        [HttpPost]
        public ActionResult Edit(ALMRePrintModel model)
        {
            if (model.VRN != null && model.OTP != null)
            {
                var count = Math.Floor(Math.Log10(model.OTP) + 1);

                var details = InsuranceContext.ALMRePrints.Single(model.Id);
                details.OTP = model.OTP;
                details.VRN = model.VRN;

                if (count == 6)
                {
                   // ALMRePrint reprint = new ALMRePrint { VRN = model.VRN, OTP = model.OTP, CreatedOn = DateTime.Now };
                    InsuranceContext.ALMRePrints.Update(details);
                }
                else
                {
                    TempData["errorMsg"] = "OTP should be 6 digit number.";
                }


            }
            return RedirectToAction("Index");
        }




    }
}