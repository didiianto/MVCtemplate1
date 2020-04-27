using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MVCtemplate1.Entities;

namespace MVCtemplate1.Controllers
{
    [Authorize]
    public class MahasiswaController : Controller
    {
        public ActionResult Index()
        {
            List<Mahasiswa> r;
            using (var s = new SimEntities())
            {
                r = s.Mahasiswa.ToList();
            }
            return View(r);
        }

        [HttpGet]
        [ActionName("Create")]
        public ActionResult Create_Get()
        {
            return View();
        }

        [HttpPost]
        [ActionName("Create")]
        public ActionResult Create_Post()
        {
            var model = new Mahasiswa();
            TryUpdateModel(model);
            using (var s = new SimEntities())
            {
                s.Mahasiswa.Add(model);
                s.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
    
}