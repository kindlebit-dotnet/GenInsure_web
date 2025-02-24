using AutoMapper;
using Insurance.Domain;
using Insurance.Service;
using InsuranceClaim.Models;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace InsuranceClaim.Controllers
{
  
    public class GenerateQuotationController : Controller
    {
        ApplicationDbContext db = new ApplicationDbContext();
        // GET: BrokerNonmoter
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

            return RedirectToAction("NonMoterRiskCover", new { Id = polictId.Fk_Ins_plcyID});
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
    }
}
