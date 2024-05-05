using PagedList;
using Project.Models;
using Project.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Project.Controllers
{
    [Authorize]
    public class ShareesController : Controller
    {
        private readonly ShareeArchivesDbContext db = new ShareeArchivesDbContext();
        // GET: Shoes
        [AllowAnonymous]
        public ActionResult Index()
        {
            return View();
        }
        [AllowAnonymous]
        public PartialViewResult ShareeDetails(int pg = 1)
        {
            var data = db.Sharees
                         .Include(x => x.Stocks)
                         .Include(x => x.Model)
                       .Include(x => x.Brand)
                       .OrderBy(x => x.ShareeId)
                       .ToPagedList(pg, 10);
            return PartialView("_ShareeDetails", data);
        }
        public ActionResult Create()
        {
            return View();
        }
        public ActionResult CreateForm()
        {
            ShareeInputModel model = new ShareeInputModel();
            model.Stocks.Add(new Stock());
            ViewBag.Models = db.Models.ToList();
            ViewBag.Brands = db.Brands.ToList();
            return PartialView("_CreateForm", model);
        }
        [HttpPost]
        public ActionResult Create(ShareeInputModel model, string act = "")
        {
            if (act == "add")
            {
                model.Stocks.Add(new Stock());
                foreach (var e in ModelState.Values)
                {
                    e.Errors.Clear();
                    e.Value = null;
                }
            }
            if (act.StartsWith("remove"))
            {
                int index = int.Parse(act.Substring(act.IndexOf("_") + 1));
                model.Stocks.RemoveAt(index);
                foreach (var e in ModelState.Values)
                {
                    e.Errors.Clear();
                    e.Value = null;
                }
            }
            if (act == "insert")
            {
                if (ModelState.IsValid)
                {
                    var sharee = new Sharee
                    {
                        BrandId = model.BrandId,
                        ModelId = model.ModelId,
                        ShareeName = model.ShareeName,
                        FirstIntroduceOn = model.FirstIntroduceOn,
                        OnSale = model.OnSale
                    };
                    //For Image
                    string ext = Path.GetExtension(model.Picture.FileName);
                    string f = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + ext;
                    string savePath = Path.Combine(Server.MapPath("~/Images"), f);
                    model.Picture.SaveAs(savePath);
                    sharee.Picture = f;

                    db.Sharees.Add(sharee);
                    db.SaveChanges();
                    //Stocks
                    foreach (var s in model.Stocks)
                    {
                        db.Database.ExecuteSqlCommand($"spInsertStock {(int)s.Category},{s.Price},{(int)s.Quantity},{sharee.ShareeId}");
                    }
                    ShareeInputModel newModel = new ShareeInputModel()
                    {
                        ShareeName = "",
                        FirstIntroduceOn = DateTime.Today
                    };
                    newModel.Stocks.Add(new Stock());

                    ViewBag.Models = db.Models.ToList();
                    ViewBag.Brands = db.Brands.ToList();
                    foreach (var e in ModelState.Values)
                    {
                        e.Value = null;
                    }
                    return View("_CreateForm", newModel);
                }
            }
            ViewBag.Models = db.Models.ToList();
            ViewBag.Brands = db.Brands.ToList();
            return View("_CreateForm", model);
        }
        public ActionResult Edit(int id)
        {
            ViewBag.Id = id;
            return View();
        }
        public ActionResult EditForm(int id)
        {
            var data = db.Sharees.FirstOrDefault(x => x.ShareeId == id);
            if (data == null) return new HttpNotFoundResult();
            db.Entry(data).Collection(x => x.Stocks).Load();
            ShareeEditModel model = new ShareeEditModel
            {
                ShareeId = id,
                BrandId = data.BrandId,
                ModelId = data.ModelId,
                ShareeName = data.ShareeName,
                FirstIntroduceOn = data.FirstIntroduceOn,
                OnSale = data.OnSale,
                Stocks = data.Stocks.ToList()
            };
            ViewBag.Models = db.Models.ToList();
            ViewBag.Brands = db.Brands.ToList();
            ViewBag.CurrentPic = data.Picture;
            return PartialView("_EditForm", model);
        }
        [HttpPost]
        public ActionResult Edit(ShareeEditModel model, string act = "")
        {
            if (act == "add")
            {
                model.Stocks.Add(new Stock());
                foreach (var e in ModelState.Values)
                {
                    e.Errors.Clear();
                    e.Value = null;
                }
            }
            if (act.StartsWith("remove"))
            {
                int index = int.Parse(act.Substring(act.IndexOf("_") + 1));
                model.Stocks.RemoveAt(index);
                foreach (var e in ModelState.Values)
                {
                    e.Errors.Clear();
                    e.Value = null;
                }
            }
            if (act == "update")
            {
                if (ModelState.IsValid)
                {
                    var sharee = db.Sharees.FirstOrDefault(x => x.ShareeId == model.ShareeId);
                    if (sharee == null) { return new HttpNotFoundResult(); }
                    sharee.ShareeName = model.ShareeName;
                    sharee.FirstIntroduceOn = model.FirstIntroduceOn;
                    sharee.OnSale = model.OnSale;
                    sharee.BrandId = model.BrandId;
                    sharee.ModelId = model.ModelId;
                    if (model.Picture != null)
                    {
                        string ext = Path.GetExtension(model.Picture.FileName);
                        string f = Path.GetFileNameWithoutExtension(Path.GetRandomFileName()) + ext;
                        string savePath = Path.Combine(Server.MapPath("~/Images"), f);
                        model.Picture.SaveAs(savePath);
                        sharee.Picture = f;
                    }
                    else
                    {

                    }

                    db.SaveChanges();
                    db.Database.ExecuteSqlCommand($"EXEC DeleteStockByShareeId {sharee.ShareeId}");
                    foreach (var s in model.Stocks)
                    {
                        db.Database.ExecuteSqlCommand($"EXEC spInsertStock {(int)s.Category}, {s.Price}, {s.Quantity}, {sharee.ShareeId}");
                    }
                }
            }
            ViewBag.Models = db.Models.ToList();
            ViewBag.Brands = db.Brands.ToList();
            ViewBag.CurrentPic = db.Sharees.FirstOrDefault(x => x.ShareeId == model.ShareeId)?.Picture;
            return View("_EditForm", model);
        }

        public ActionResult Delete(int? id)
        {
            if (id != null)
            {
                var shareeDetails = db.Sharees.FirstOrDefault(x => x.ShareeId == id);
                var shareeInfo = db.Sharees.FirstOrDefault(x => x.ShareeId == id);

                db.Sharees.Remove(shareeInfo);
                db.Sharees.Remove(shareeDetails);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View("Index");
        }
    }
}