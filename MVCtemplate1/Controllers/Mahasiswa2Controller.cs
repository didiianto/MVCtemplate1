using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using MVCtemplate1.Entities;
using PagedList;
using System.IO;
using System.Text;
using iTextSharp.text;
using iTextSharp.text.pdf;

namespace MVCtemplate1.Controllers
{
    public class Mahasiswa2Controller : Controller
    {
        private SimEntities db = new SimEntities();

        // GET: Mahasiswa2
        public ActionResult Index(string sortOrder, string searchString, string currentFilter, int? page)
        {
            ViewBag.NameSortParm = String.IsNullOrEmpty(sortOrder) ? "name_desc" : "";
            ViewBag.DateSortParm = sortOrder == "Date" ? "date_desc" : "Date";
            if (searchString != null)
            {
                page = 1;
            }
            else
            {
                searchString = currentFilter;
            }

            ViewBag.CurrentFilter = searchString;

            var mahasiswa = from s in db.Mahasiswa
                           select s;
            if (!String.IsNullOrEmpty(searchString))
            {
                mahasiswa = mahasiswa.Where(s => s.nama.Contains(searchString)
                                       || s.alamat.Contains(searchString));
            }
            switch (sortOrder)
            {
                case "name_desc":
                    mahasiswa = mahasiswa.OrderByDescending(s => s.nama);
                    break;
                case "Date":
                    mahasiswa = mahasiswa.OrderBy(s => s.tanggalLahir);
                    break;
                case "date_desc":
                    mahasiswa = mahasiswa.OrderByDescending(s => s.tanggalLahir);
                    break;
                default:
                    mahasiswa = mahasiswa.OrderBy(s => s.nama);
                    break;
            }

            int pageSize = 2;
            int pageNumber = (page ?? 1);
            return View(mahasiswa.ToPagedList(pageNumber, pageSize));
        }

        // GET: Mahasiswa2/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Mahasiswa mahasiswa = db.Mahasiswa.Find(id);
            if (mahasiswa == null)
            {
                return HttpNotFound();
            }
            return View(mahasiswa);
        }

        // GET: Mahasiswa2/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Mahasiswa2/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "nim,nama,tanggalLahir,jenisKelamin,alamat")] Mahasiswa mahasiswa)
        {
            if (ModelState.IsValid)
            {
                db.Mahasiswa.Add(mahasiswa);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(mahasiswa);
        }

        // GET: Mahasiswa2/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Mahasiswa mahasiswa = db.Mahasiswa.Find(id);
            if (mahasiswa == null)
            {
                return HttpNotFound();
            }
            return View(mahasiswa);
        }

        // POST: Mahasiswa2/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "nim,nama,tanggalLahir,jenisKelamin,alamat")] Mahasiswa mahasiswa)
        {
            if (ModelState.IsValid)
            {
                db.Entry(mahasiswa).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(mahasiswa);
        }

        // GET: Mahasiswa2/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Mahasiswa mahasiswa = db.Mahasiswa.Find(id);
            if (mahasiswa == null)
            {
                return HttpNotFound();
            }
            return View(mahasiswa);
        }

        // POST: Mahasiswa2/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            Mahasiswa mahasiswa = db.Mahasiswa.Find(id);
            db.Mahasiswa.Remove(mahasiswa);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        public FileResult CreatePdf()
        {
            MemoryStream workStream = new MemoryStream();
            StringBuilder status = new StringBuilder("");
            DateTime dTime = DateTime.Now;
            //file name to be created   
            string strPDFFileName = string.Format("SamplePdf" + dTime.ToString("yyyyMMdd") + "-" + ".pdf");
            Document doc = new Document();
            doc.SetMargins(0f, 0f, 0f, 0f);
            //Create PDF Table with 5 columns  
            PdfPTable tableLayout = new PdfPTable(5);
            doc.SetMargins(0f, 0f, 0f, 0f);
            //Create PDF Table  

            //file will created in this path  
            string strAttachment = Server.MapPath("~/Downloads/" + strPDFFileName);


            PdfWriter.GetInstance(doc, workStream).CloseStream = false;
            doc.Open();

            //Add Content to PDF   
            doc.Add(Add_Content_To_PDF(tableLayout));

            // Closing the document  
            doc.Close();

            byte[] byteInfo = workStream.ToArray();
            workStream.Write(byteInfo, 0, byteInfo.Length);
            workStream.Position = 0;


            return File(workStream, "application/pdf", strPDFFileName);

        }

        #region HELPER
        protected PdfPTable Add_Content_To_PDF(PdfPTable tableLayout)
        {

            float[] headers = { 50, 24, 45, 35, 50 }; //Header Widths  
            tableLayout.SetWidths(headers); //Set the pdf headers  
            tableLayout.WidthPercentage = 100; //Set the PDF File witdh percentage  
            tableLayout.HeaderRows = 1;
            //Add Title to the PDF file at the top  

            List<Mahasiswa> employees = db.Mahasiswa.ToList<Mahasiswa>();

            tableLayout.AddCell(new PdfPCell(new Phrase("Creating Pdf using ItextSharp", new Font(Font.FontFamily.HELVETICA, 8, 1, new iTextSharp.text.BaseColor(0, 0, 0)))) {
                Colspan = 12, Border = 0, PaddingBottom = 5, HorizontalAlignment = Element.ALIGN_CENTER
            });

            ////Add header  
            AddCellToHeader(tableLayout, "NIM");
            AddCellToHeader(tableLayout, "Nama");
            AddCellToHeader(tableLayout, "Tanggal Lahir");
            AddCellToHeader(tableLayout, "Jenis Kelamin");
            AddCellToHeader(tableLayout, "alamat");

            ////Add body  
            foreach (var emp in employees)
            {

                AddCellToBody(tableLayout, emp.nim);
                AddCellToBody(tableLayout, emp.nama);
                AddCellToBody(tableLayout, emp.tanggalLahir.ToString());
                AddCellToBody(tableLayout, emp.jenisKelamin);
                AddCellToBody(tableLayout, emp.alamat);

            }

            return tableLayout;
        }
        // Method to add single cell to the Header  
        private static void AddCellToHeader(PdfPTable tableLayout, string cellText)
        {
            tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.FontFamily.HELVETICA, 8, 1, iTextSharp.text.BaseColor.YELLOW)))
            {
                HorizontalAlignment = Element.ALIGN_LEFT, Padding = 5, BackgroundColor = new iTextSharp.text.BaseColor(128, 0, 0)
            });
        }
        // Method to add single cell to the body  
        private static void AddCellToBody(PdfPTable tableLayout, string cellText)
        {
            tableLayout.AddCell(new PdfPCell(new Phrase(cellText, new Font(Font.FontFamily.HELVETICA, 8, 1, iTextSharp.text.BaseColor.BLACK)))
             {
                HorizontalAlignment = Element.ALIGN_LEFT, Padding = 5, BackgroundColor = new iTextSharp.text.BaseColor(255, 255, 255)
            });
        }
        #endregion HELPER
    }
}
