using Insurance.Domain;
using InsuranceClaim.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace InsuranceClaim.Controllers
{
    public class BranchController : Controller
    {
        // GET: Branch
        public ActionResult Index()
        {
            //return View(InsuranceContext.Branches.All());
            var list = InsuranceContext.Query("select Branch.Id,  BranchName,AlmId ,Partners.PartnerName,Location_Id,Branch.Status   from Branch left  join Partners on  Partners.Id=Branch.PartnerId")
          .Select(x => new BranchModel()
          {
              BranchName = x.BranchName,
              AlmId = x.AlmId,
              Partners = x.PartnerName,
              Location_Id = x.Location_Id,
              Status = x.Status,
              Id = x.Id == null ? 0 : Convert.ToInt32(x.Id)

          }).ToList();

            return View(list);
        }


        // GET: Home/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Branch branch = InsuranceContext.Branches.Single(id);
            if (branch == null)
            {
                return HttpNotFound();
            }
            return View(branch);
        }

        // GET: Home/Create
        public ActionResult Create()
        {
            ViewBag.Partners = GetListOfPartner();
            return View();
        }


        private List<Partner> GetListOfPartner()
        {
            var list = InsuranceContext.Query("select * from Partners")
          .Select(x => new Partner()
          {
              Id = x.Id,
              PartnerName = x.PartnerName,
          }).ToList();

            return list;
        }


        // POST: Home/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Branch branch)
        {
            ViewBag.Partners = GetListOfPartner();
            if (ModelState.IsValid)
            {
                branch.AlmId = GetALMId();
                var branchDetials = InsuranceContext.Branches.All(where: "Location_Id ='" + branch.Location_Id + "'").FirstOrDefault();

                if (branchDetials == null)
                {
                    branch.Status = true;
                   // branch.PartnerId = branch.PartnerId;
                    InsuranceContext.Branches.Insert(branch);
                    return RedirectToAction("Index");
                }
            }
            return View(branch);
        }



        public string GetALMId()
        {

            string almId = "";

            var getcustomerdetail = InsuranceContext.Query(" select top 1 AlmId  from [dbo].[Branch] where AlmId is not null order by id desc ")
         .Select(x => new Customer()
         {
             ALMId = x.AlmId
         }).ToList().FirstOrDefault();


            if (getcustomerdetail != null && getcustomerdetail.ALMId != null)
            {
                string number = getcustomerdetail.ALMId.Split('K')[1];
                long pernumer = Convert.ToInt64(number) + 1;
                string policyNumbera = string.Empty;
                int lengths = 3;
                lengths = lengths - pernumer.ToString().Length;
                for (int i = 0; i < lengths; i++)
                {
                    policyNumbera += "0";
                }
                policyNumbera += pernumer;
                //  customer.ALMId = "GENE-SSK" + policyNumbera;
                almId = "GENE-SSK" + policyNumbera;
            }
            else
            {
                almId = "GENE-SSK003";
            }

            return almId;
        }

        // GET: Home/Edit/5
        public ActionResult Edit(int? id)
        {
            var branchModel = new BranchModel();
            ViewBag.Partners = GetListOfPartner();

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Branch branch = InsuranceContext.Branches.Single(id);
            if (branch == null)
            {
                return HttpNotFound();
            }
            else
            {
                branchModel.Id = branch.Id;
                branchModel.BranchName = branch.BranchName;
                branchModel.Location_Id = branch.Location_Id;
                branchModel.Status = branch.Status;
                branchModel.AlmId = branch.AlmId;
              //  branchModel.PartnersId = branch.PartnerId;
            }

            return View(branchModel);
        }

        // POST: Home/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(BranchModel branchModel)
        {
            ViewBag.Partners = GetListOfPartner();
            if (ModelState.IsValid)
            {
       
                var branchDetails = InsuranceContext.Branches.Single(branchModel.Id);
                if (branchDetails != null)
                {
                    branchDetails.Status = true;
                    branchDetails.PartnerId = branchModel.PartnersId;
                    branchModel.BranchName = branchModel.BranchName;
                    branchModel.Location_Id = branchModel.Location_Id;
                    branchModel.AlmId = branchModel.AlmId;

                    string query = "update Branch set BranchName='"+branchModel.BranchName+"', AlmId='"+ branchModel.AlmId+"', PartnerId="+ branchModel.PartnersId+", Location_Id='"+ branchModel.Location_Id + "',  [Status]='"+branchModel.Status+"' where id=" + branchModel.Id;

                     InsuranceContext.Execute(query);

                   // InsuranceContext.Branches.Update(branchDetails);
                    return RedirectToAction("Index");
                }
             
            }
            return View(branchModel);
        }

        //private bool CheckDuplicate(string newBranchId, string oldBranchId)
        //{
        //    bool result = false;


        //    var branchDetials = InsuranceContext.Branches.Single(Convert.ToInt32(newBranchId));
        //    if (branchDetials != null)
        //    {
        //        if (branchDetials.Location_Id == branch.Location_Id)
        //        {

        //        }

        //    }


        //    return result;
        //}

        // GET: Home/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Branch branch = InsuranceContext.Branches.Single(id);
            if (branch == null)
            {
                return HttpNotFound();
            }
            return View(branch);
        }

        // POST: Home/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Branch branch = InsuranceContext.Branches.Single(id);
            InsuranceContext.Branches.Delete(branch);

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                //InsuranceContext.Dispose();
            }
            base.Dispose(disposing);
        }

        public JsonResult EditBranch(int branchJson)
        {

            //var js = new JavaScriptSerializer();
            //BranchModel branchRequest = js.Deserialize<BranchModel>(branchJson);

            var branchModel = new BranchModel();
            if (branchJson == null)
            {
                return Json(HttpStatusCode.BadRequest, JsonRequestBehavior.AllowGet);
                //return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Branch branch = InsuranceContext.Branches.Single(branchJson);
            if (branch == null)
            {
                return Json(HttpStatusCode.NotFound, JsonRequestBehavior.AllowGet);
            }
            else
            {
                branchModel.Id = branch.Id;
                branchModel.BranchName = branch.BranchName;
                branchModel.Location_Id = branch.Location_Id;
                branchModel.Status = !branch.Status;
                branchModel.AlmId = branch.AlmId;
                InsuranceContext.Branches.Update(branch);

            }

            return Json(branchModel, JsonRequestBehavior.AllowGet);
        }



    }
}