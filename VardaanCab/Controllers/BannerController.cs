using AutoMapper;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using VardaanCab.DataAccessLayer.DataLayer;
using VardaanCab.Domain.DTO;
using VardaanCab.Utilities;

namespace VardaanCab.Controllers
{
    public class BannerController : Controller
    {
        Vardaan_AdminEntities ent = new Vardaan_AdminEntities();
        // GET: Banner
        public ActionResult All()
        {
            var data = ent.BannerMasters.ToList();
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
        public ActionResult Add(BannerDTO model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                //if (model.ImageFile == null)
                //{
                //    TempData["errormsg"] = "Image file is required. Only .jpg, .jpeg, .png, and .gif files are allowed.";
                //    return View(model);
                //}
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
                
               
                if (model.Id == 0)
                {
                    var domainModel = new BannerMaster
                    {
                        Role = model.Role,
                        BannerImage = model.BannerImage,
                        CreatedDate = DateTime.Now
                    };
                    ent.BannerMasters.Add(domainModel);
                }
                else
                {
                    var data = ent.BannerMasters.Find(model.Id);
                    data.Role = model.Role;
                    if (model.BannerImage != null)
                    {
                        data.BannerImage = model.BannerImage;
                    }
                }
                ent.SaveChanges();

                TempData["msg"] = model.Id>0 ? "Record has been updated successfully." : "Record has been added successfully.";
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