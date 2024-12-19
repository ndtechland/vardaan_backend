using AutoMapper;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Vardaan.Services.IContract;
using VardaanCab.DataAccessLayer.DataLayer;
using VardaanCab.Domain.DTO;
using VardaanCab.Utilities;

namespace VardaanCab.Controllers
{
    public class BannerController : Controller
    {
        Vardaan_AdminEntities ent = new Vardaan_AdminEntities();
        private readonly IBanner _banner;
        public BannerController(IBanner banner)
        {
             _banner = banner;
        }
        // GET: Banner
        public async Task<ActionResult> All()
        {
            var data = await _banner.BannerList();
            return View(data);
        }
        public ActionResult Add(int id=0)
        {
            var model = new BannerDTO();
            if (id > 0)
            {
                var data = ent.BannerMasters.Find(id);
                model.Id = data.Id;
                model.Role = data.Role;
                model.BannerImage = data.BannerImage;
                ViewBag.Heading = "Update Banner";
                ViewBag.BtnTXT = "Update";
                return View(model);
            }
            else
            {
                model.Id = 0;
                model.Role = "";
                model.BannerImage = "";
                ViewBag.BtnTXT = "Save";
                ViewBag.Heading = "Add Banner";
                return View(model);
            }
        }
        [HttpPost]
        public async Task<ActionResult> Add(BannerDTO model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }
                if(model.ImageFile != null)
                {
                    if (model.ImageFile.ContentLength > 2 * 1024 * 1024)
                    {
                        TempData["errormsg"] = "Image should not exceed 2 MB.";
                        return View(model);
                    }
                    var uploadResult = FileOperation.UploadImage(model.ImageFile, "Images");
                    if (uploadResult == "not allowed")
                    {
                        TempData["errormsg"] = "Only .jpg, .jpeg, .png, and .gif files are allowed.";
                        return View(model);
                    }

                    model.BannerImage = uploadResult;
                }
                bool isCreated = await _banner.Addbanners(model);
                if (isCreated)
                {
                    TempData["msg"] = model.Id > 0 ? "Record has been updated successfully." : "Record has been added successfully.";
                }

                return RedirectToAction("Add");
            }
            catch (Exception ex)
            {
                throw new Exception("Internal server error occurred. Please check the details.", ex);
            }
        }
        public ActionResult DeleteBanner(int id)
        {
            try
            {
                var data = ent.BannerMasters.Find(id);
                ent.BannerMasters.Remove(data);
                ent.SaveChanges();
                TempData["dltmsg"] = "Deleted successfully.";
                return RedirectToAction("All");
            }
            catch (Exception ex)
            {

                throw new Exception("Internal server error occurred. Please check the details.", ex);
            }
        }
    }
}