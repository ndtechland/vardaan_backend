using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using VardaanCab.DataAccessLayer.DataLayer;
using VardaanCab.Domain.DTO;
using VardaanCab.Domain.ViewModels;


namespace VardaanCab.Controllers
{
    public class EmployeeController : Controller
    {
        Vardaan_AdminEntities ent = new Vardaan_AdminEntities();

        [HttpGet]
        public ActionResult Add()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {

                throw new Exception("Server Error : " + ex.Message);
            }
        }
        [HttpPost]
        public ActionResult Add(EmployeeDTO model)
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {

                throw new Exception("Server Error : " + ex.Message);
            }
        }
    }
}